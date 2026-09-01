# 技能规格 Skill Spec（v1）

> 「输入自然语言 → 添加技能 → 添加技能效果」框架的核心契约。
> 架构：**自然语言 → 会话内 LLM 解析 → spec.json → SkillForge 工具（确定性执行）→ staging 产物 → 部署 → 真实机器验收**

## 工作流

```
用户自然语言描述需求
  → 会话内解析为 spec.json（本文件架构），向用户确认关键参数
  → SkillForge run <spec> （克隆模板行+列覆盖，产出 staging assets + 自验报告）
  → 编排层部署（备份核验→覆盖→SHA256 记录）
  → 真实机器验收（天赋树可见/加点/施放）
  → 效果若需新行为：走 Tier 2 代码补丁（见 effects-library.md），完成后重跑构建部署
```

## 架构 v1

```json
{
  "indexName": "ArcBoomerang",
  "templateIndexName": "Razor Arrow",
  "infoKey": "info_ArcBoomerang",
  "price": 0,
  "unlockPoint": 0,
  "columnOverrides": { "Damage_Base": "150" }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `indexName` | string | ✅ | 唯一键（ASCII 安全），写入 col0(name) 与 col1(IndexName)；同时是存档键与本地化键前缀 |
| `templateIndexName` | string | ✅ | 现有技能 IndexName，整行克隆来源（决定弹体/图标/动画/音效/系别等全部基底） |
| `infoKey` | string | ✅* | 默认 `info_<indexName>`；写入 Info 列。缺键时 tooltip 回退原文并刷 Warn 日志（KI-002 根因） |
| `description` | object | ✅* | 本地化文案映射，至少含 `English`/`ChineseS`/`ChineseT`；给出时工具推导 infoKey=`info_<indexName>` 并在**同一产出包**内将该文案写入 Skill_FY |
| `price` | int | — | 默认 0（金币价格） |
| `unlockPoint` | int | — | 默认 0（系内解锁门槛，0=立即可点） |
| `columnOverrides` | object | — | 列名→字符串值映射；键必须与 SampleF 表头精确匹配；值必须可 Parse 为 int/float |

\* **infoKey 与 description 必须显式给出其一**（二者同给时，将 description 写入 infoKey 指向的键）。这是必填校验项：Skill_FY 中不存在目标键的 spec 不允许出包——详见[描述同步要求](#描述同步要求)。

## 描述同步要求（强制校验）

> 数据流：SampleF 表 Info 列（形如 `info_Razor Arrow`）→ `LocalizationManager.GetSkill(IndexName/Info)` 读 resources.assets 内 Skill_FY JSON（TextAsset path_id=433）→ UI tooltip。Info 为 localization key。

任何技能行为变更（CSV 加行、列覆盖改变弹幕形态/数量/飞行行为、Tier 2 代码补丁改变技能表现）必须同步满足：

1. **键存在**：Info 列指向的键必须已在 Skill_FY JSON 中定义；新技能加行的同一产出包内必须包含该键的新增/更新（spec 的 infoKey/description 二选一必填即为此服务）。
2. **语义一致**：description 必须与 columnOverrides 及模板差异后的实际行为一致——描述数量、形态（环形/单发）、飞行行为（穿透/返回）不得沿用模板旧文案。
3. **语言覆盖**：至少提供 English（全语言缺失时的回退载体）、ChineseS、ChineseT；其余语言可缺省（运行时自动回退 English）。
4. **同包自验**：staging 报告必须含新旧文案对比与导出回读验证（改前/改后各一次 export 对比）。
5. **同步时机**：效果代码合入与描述文案属同一个变更单元，禁止"先上行、后补文案"的中间态出包部署。

## 语义规则

1. **模板决定一切基底**：弹体(OBJ)、特效(RTtypeFX)、动画(UseAni)、音效、icon、Xi 系别均继承模板行——选模板即选原型
2. **覆盖必须可解析**：游戏解析器逐列 int/float.Parse，任何列缺失或不可解析都会导致 Awake 异常
3. **无引号转义**：值内不得含逗号/换行
4. **唯一性**：indexName 重复会污染字典与存档
5. **Tier 2 效果不在 spec 内**：涉及新行为模式的效果先走代码补丁入库（effects-library.md），稳定后其可参数化部分再下沉为列覆盖

## 工具

| 命令 | 用途 |
|---|---|
| `SkillForge run <spec.json> [--gamedata <dir>] [--out <file>]` | 生成 staging assets + 自验报告 |
| `SkillForge verify --assets <file> <spec.json>` | 断言产物含目标行且覆盖生效 |

## 部署与验收（编排层职责）

1. 备份核验：`modwork/backup/sharedassets1.assets` SHA256 = CA85E2A4…FEDB1
2. 覆盖游戏 `Shadow Dungeon_Data/sharedassets1.assets`，记录部署后 SHA256
3. 启动验证：35s 存活 + Player.log 零异常
4. 真实机器验收：天赋树对应系页找新节点 → 加点 → 快捷栏 → 施放
5. 回滚：两条 Copy-Item 还原命令（见 AGENTS.md 工作规范）

## 实例（ArcBoomerang 复盘）

```json
{
  "indexName": "ArcBoomerang",
  "templateIndexName": "Razor Arrow",
  "infoKey": "info_ArcBoomerang",
  "price": 0,
  "unlockPoint": 0,
  "columnOverrides": {}
}
```

该 spec 若在框架就绪前提出，可一条命令复现已验收的 ArcBoomerang 行。
复盘教训：该行首次部署时 Skill_FY 缺 `info_ArcBoomerang` 键（KI-002），tooltip 回退英文原文——正是"描述同步要求"未成文导致的中间态出包；现已补键并立规。
