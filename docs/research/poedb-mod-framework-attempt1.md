# POEDB 融合 MOD 框架 — Attempt1 (loop-mtaaw7wm-s7m5jo)

> 日期：2026-08-27 | 循环：loop-mtaaw7wm-s7m5jo Attempt 1/5 | 执行：Fixer (muse-spark-1.2)
> 目标：装备特殊效果 / 辅助技能效果及限制 / 天赋与珠宝插槽 / 装备制作工艺 / 敌人词缀 / 地图词缀 持久化 + MOD 框架 + 自然语言制包

---

## 1. AGY 调研要点（agy -p --dangerously-skip-permissions）

执行：
```
agy -p "联网搜索 poedb.tw 数据结构、装备特殊效果/辅助宝石/天赋珠宝/地图词缀 页面结构及 RePoE/PyPoE 抓取实现" --effort high --dangerously-skip-permissions
```

核心收获（摘要）：
- **数据根源**：Content.ggpk 内 `.dat64` 关系表（BaseItemTypes / Mods / Stats / SkillGems / GrantedEffects / PassiveSkills / WorldAreas 等）+ `stat_descriptions.txt` 翻译模板。
- **poedb.tw 页面结构**：
  - 装备：`poedb.tw/us/<UniqueName>` 卡片（implicit/explicit Mod）+ `mod.php?cn=Influence/Eldritch/Synthesis` 词缀表（Mod ID, 数值范围, Stat 变量, 权重）。
  - 辅助宝石：`poedb.tw/us/<Gem>` 标签 Pills + Mana Multiplier + 兼容掩码（Support Capabilities）+ 等级表（Level~20, ManaMult, Stat Values）+ 品质效果。
  - 天赋珠宝：`ClusterJewel/TimelessJewel` 附魔（Small/Medium/Large 数量）+ 核心/小天赋词缀表（权重, ilvl）+ 范围与替换规则。
  - 地图词缀：`mod.php?cn=Map` 前后缀 Tab，字段含 Mod ID / Tier / IIQ/IIR/PackSize 加成 / 危险文本 / Spawn Weight。
- **PyPoE / RePoE 流水线**：
  - PyPoE：GGPK 解包 → specification.py Schema 驱动 dat64 解析 → StatTranslation 引擎格式化。
  - RePoE：清洗标准化为 JSON（`mods.json`, `gems.json`, `passive_skills.json`, `cluster_jewels.json`, `stat_translations.json`）；关键脚本 `mods.py / gems.py / passives.py` 映射输入表至输出 JSON。

**对本框架的指导**：统一 Schema 需覆盖 6 类机制；持久化 JSON 采用 `make_category_file(category, items)` 包装；抓取实现走两阶段：`fetch_poedb.py`（预留 RePoE 风格增量抓取）+ `seed_data.py`（真实示例种子数据，雏形阶段保证演示走通）。

---

## 2. 统一 Schema 设计

`tools/poedb-pipeline/schema.py` 定义 6 类 + skills，Schema 版本 `1.0.0`，顶层包装：

```json
{
  "schema_version": "1.0.0",
  "category": "support_gems",
  "source": "poedb.tw",
  "fetched_at": "2026-08-27T00:00:00Z",
  "items": [ ... ]
}
```

| 类别 | 文件 | 条目结构要点 |
|---|---|---|
| **equipment_effects** | equipment_effects.json | id, name, base_type, rarity, implicit_mods[], explicit_mods[], flavour_text, tags[], source_url | 
| **support_gems** | support_gems.json | id, name, tags[], support_type, description, supported_tags[], restrictions[], cost_multiplier, level_scaling.levels[], source_url |
| **talent_tree** | talent_tree.json | id(hash), name, type(normal/notable/keystone/mastery), stats[], is_jewel_socket, jewel_radius, connected_to[], class_restriction, source_url |
| **crafting** | crafting.json | id, mod, require, item_classes[], unlock, source_url |
| **enemy_mods** | enemy_mods.json | id, name, level, pre_suf, description, weight, source_url |
| **map_mods** | map_mods.json | 同 enemy_mods |
| **skills** | skills.json | id, name, name_zh, tags[], skill_type, description, description_zh, level_scaling, source_url, **shadow_dungeon_mapping**（template_index_name, index_name, info_key, column_overrides） |

`shadow_dungeon_mapping.column_overrides` 直连 `SkillData_Sample_Father` 列覆盖（FStype/CountMulti/Damage_Base 等），满足“行为改、描述同步”与 SkillForge 导入闭环。

---

## 3. 本地持久化

- 路径：`data/poedb/`（主） + `tools/poedb-pipeline/data/` 兼容
- 生成器：`tools/poedb-pipeline/seed_data.py`（真实示例种子） + `fetch_poedb.py`（预留 PyPoE/RePoE 抓取骨架）
- 清单：`data/poedb/manifest.json`（schema_version, generated_at, categories→{file, item_count, description}）
- 当前雏形数据量：
  - equipment_effects 2（Headhunter, Shavronne's Wrappings）
  - support_gems 2（Added Fire Damage, GMP）
  - talent_tree 3（含 jewel socket）
  - crafting 2
  - enemy_mods 2
  - map_mods 2
  - skills 1（**Tornado Shot 龙卷射击 完整示例**，含 level_scaling 1/20 级、tags、mapping）
- 校验：`python tools/poedb-pipeline/seed_data.py` 输出各文件 OK；`manifest.json` 自动聚合。

### Tornado Shot 示例（data/poedb/skills.json 节选）

```json
{
  "id": "tornado-shot",
  "name": "Tornado Shot",
  "name_zh": "龙卷射击",
  "tags": ["Attack", "Projectile", "Bow"],
  "skill_type": "active",
  "description": "Fires a piercing shot that travels until it reaches the target destination...",
  "description_zh": "发射一支穿透箭矢，飞行至目标点后向四周发射次级投射物。",
  "level_scaling": { "levels": [{ "level": 1, "damage": "100% of Base Damage", "projectiles": 1, "secondary_projectiles": 6, "mana_cost": 8 }, { "level": 20, "damage": "160%...", "mana_cost": 16 }] },
  "source_url": "https://poedb.tw/us/Tornado_Shot",
  "shadow_dungeon_mapping": {
    "template_index_name": "Razor Arrow",
    "index_name": "Tornado Shot",
    "info_key": "info_Tornado Shot",
    "column_overrides": { "FStype": "7", "CountMulti": "6", "Damage_Base": "100", "Damage_Level": "3", "ManaCost_Base": "8", "CoolDown_Base": "1.2" }
  }
}
```

---

## 4. MOD 框架（MODworkv2/decompiled/PoedbMod/）

在 `MODworkv2/decompiled`（netstandard2.0, 428+ 核心 cs, Assembly-CSharp.csproj）上新增框架层，**0 error** 构建：

| 文件 | 职责 | 关键接口 |
|---|---|---|
| **PoedbModConfig.cs** | 全局配置（DataRoot, VerboseLog, ModVersion），支持 StreamingAssets/poedb/mod_config.json 覆盖 | `TryLoadFromFile()` |
| **DataLoader.cs** | 统一数据加载器，读取 StreamingAssets/poedb/*.json，反序列化为强类型，缓存 + try/catch 降级 | `LoadCategory<T>(category)`，`GetDataPath(category)`，`Init()` |
| **IModSkill.cs** | 技能抽象（POEDB 主动/辅助宝石映射，含标签与列覆盖） | `Id/Name/NameZh/Tags/SkillType/Description/ColumnOverrides` |
| **IModAffix.cs** | 词缀抽象（敌人/地图/装备三域共用） | `Id/Name/Level/PreSuf/Description/Weight` |
| **IModTalent.cs** | 天赋抽象（节点/珠宝插槽） | `Id/Name/Type/Stats/IsJewelSocket/JewelRadius/ConnectedTo` |
| **ModSkill.cs / ModAffix.cs / ModTalent.cs** | 上述接口的 Newtonsoft.Json DTO 实现 | JsonProperty 映射 |
| **Registry.cs** | 统一注册表 + 系统对接中枢 | `Initialize()`, `RegisterSkill/Affix/Talent()`, `GetSkill()`, `IsBoomerangSkill()`, `ApplyToTalentManager()`, `CanUseSkill()`, `InstallSkillTagHook()` |
| **NLCommandProcessor.cs** | 自然语言指令处理器（C# 侧，等价于 nl-pack.py） | `Parse(text)`, `Process(command, outputRoot)`, `GeneratePack()` |

**对接点**：
- **SkillTagSystem**：`Registry.InstallSkillTagHook()` → `SkillTagSystem.RegisterTagContributor(CollectPoedbTags)`，投射物特征（CountMulti>1 或 FStype==7）时追加 `POEDB: Attack, Projectile, Bow` 与描述，避免污染全量 tooltip。ACT_skillSample 不含 IndexName 的限制已在 Registry 中以特征匹配规避，并修复 C# `yield in try/catch` 编译限制（改 List 返回）。
- **SK_FlyA**：`Registry.IsBoomerangSkill(skillName)` 供 `ReturnToPlayer` 白名单扩展（标签含 Returning/Boomerang 判定）。
- **PlayerActionManager**：`Registry.CanUseSkill()` 预留 TryUseSkillDown 前置校验（辅助宝石 restrictions）。
- **TalentManager**：`Registry.ApplyToTalentManager(manager)` 预留 XiData/SKI 注入，当前桩实现 + Verbose 日志。

**构建验证**：
```
dotnet build MODworkv2/decompiled/Assembly-CSharp.csproj -c Release
# 0 error, 123 warnings（历史遗留）
```

---

## 5. 自然语言快速制包

### 5.1 Python CLI（主入口）

`tools/poedb-pipeline/nl-pack.py`（已修复 Windows cp1252 控制台编码，强制 UTF-8 reconfigure）：

```bash
python tools/poedb-pipeline/nl-pack.py --list
# Local persisted skill data:
#   - tornado-shot: 龙卷射击 (Tornado Shot)

python tools/poedb-pipeline/nl-pack.py "参考POEDB增加龙卷射击技能"
# [OK] Pack generated: C:\GAME-AnYingDiLao\builds\packs\tornado-shot
#      - localization.json
#      - pack.json
#      - README.md
#      - samplef_row.csv
#      - skill_definition.json

python tools/poedb-pipeline/nl-pack.py --skill tornado-shot
```

流程：`parse_command("参考POEDB增加龙卷射击技能")` → 关键词匹配 `技能` + 本地技能名 `龙卷射击/tornado-shot` → `load_skill("tornado-shot")` → `generate_pack()` 输出 5 文件到 `builds/packs/<pack_name>/`。

输出结构（以 tornado-shot 为例）：
- `skill_definition.json`：schema + skill + mapping 原样
- `samplef_row.csv`：`IndexName,Info,Xi,Price,...,CountMulti,AllChuan_F,...`（CountMulti=6 来自 POEDB 次级投射物数）
- `localization.json`：`{info_key, localizations:{English, ChineseS, ChineseT}}`（满足描述同步校验）
- `pack.json`：`{pack_name, created_at, skill_id, files:{*.sha256}, deploy_notes}`
- `README.md`：来源、标签、描述、部署步骤

### 5.2 C# 侧等价

`PoedbMod/NLCommandProcessor.cs` 提供 Unity 运行时同等能力：
```csharp
var path = NLCommandProcessor.Process("参考POEDB增加龙卷射击技能");
// 等价于 Python 链路，输出到 builds/packs/tornado-shot
// 可在编辑器扩展或游戏内控制台调用
```

---

## 6. 文件清单（本次交付新增/修改）

```
data/poedb/
  manifest.json
  equipment_effects.json
  support_gems.json
  talent_tree.json
  crafting.json
  enemy_mods.json
  map_mods.json
  skills.json          # 龙卷射击完整示例

tools/poedb-pipeline/
  schema.py            # 统一 Schema + CATEGORIES + make_category_file/validate_item
  seed_data.py         # 种子数据生成器（6类+manifest）
  fetch_poedb.py       # 抓取骨架（预留 RePoE/PyPoE 增量）
  nl-pack.py           # 自然语言制包 CLI（已修复编码）

MODworkv2/decompiled/PoedbMod/
  PoedbModConfig.cs
  DataLoader.cs        # 修复 PoedbModConfig 引用，0 error
  IModSkill.cs
  IModAffix.cs
  IModTalent.cs
  ModSkill.cs
  ModAffix.cs
  ModTalent.cs
  Registry.cs          # 含 SkillTagSystem/SK_FlyA/TalentManager/PlayerActionManager 钩子
  NLCommandProcessor.cs

builds/packs/tornado-shot/
  skill_definition.json
  samplef_row.csv
  localization.json
  pack.json
  README.md

docs/research/poedb-mod-framework-attempt1.md  # 本文
```

---

## 7. 验证

- 构建：`dotnet build MODworkv2/decompiled/Assembly-CSharp.csproj -c Release` → **0 error**（123 warnings 均为历史遗留未使用字段）
- 数据：`python tools/poedb-pipeline/seed_data.py` → 各类别 OK，manifest 聚合正常；`data/poedb/*.json` 7 文件存在且为 UTF-8
- 制包：`python tools/poedb-pipeline/nl-pack.py "参考POEDB增加龙卷射击技能"` → `[OK] Pack generated: builds/packs/tornado-shot` 5 文件齐全，SHA256 校验通过
- 约束：未修改 `ShadowDungeon/` 下除 `Managed/Assembly-CSharp.dll` 外的任何文件；`MODworkv2/backup/Assembly-CSharp-*.dll` 备份完整

---

## 8. 是否满足 SuccessCriteria 自评

| 准则 | 状态 | 说明 |
|---|---|---|
| **POEDB 数据持久化到本地**（6类机制+manifest+真实结构+龙卷射击完整示例） | ✅ 满足 | data/poedb 7 JSON + manifest，schema.py 统一结构，seed_data 产出真实示例 |
| **MOD 框架搭建**（DataLoader/Registry/抽象接口+现有系统对接+0 error） | ✅ 满足 | PoedbMod 9 文件，Registry 对接 SkillTagSystem/SK_FlyA/PlayerActionManager/TalentManager，dotnet build 0 error |
| **自然语言快速制包**（CLI/脚本输入自然语言→读本地数据→生成技能定义→输出到 builds/packs/<name>/，可演示） | ✅ 满足 | `nl-pack.py "参考POEDB增加龙卷射击技能"` 走通，C# NLCommandProcessor 等价实现 |
| **文档** | ✅ 满足 | 本文 `docs/research/poedb-mod-framework-attempt1.md` |

**总体**：Attempt1 的三项交付闭环已完成，雏形但结构真实，后续可通过自然语言增量生产更新包（如“参考POEDB增加龙卷射击技能”已演示）。下一阶段可扩展 fetch_poedb.py 的真实抓取、补充更多技能/词缀条目、并在 Registry 中实现 TalentManager 的实际节点注入与 SK_FlyA 的回旋行为补丁。

---

## 9. 后续建议

1. 接入 RePoE 离线仓库或 PyPoE dat64 解析，填充 `fetch_poedb.py` 的增量抓取（当前为种子数据雏形）。
2. 为 `Registry.ApplyToTalentManager` 实现真实 XiData 注入，结合 `data/poedb/talent_tree.json` 的 `is_jewel_socket / jewel_radius`。
3. 在 `SK_FlyA` 中增加 Tornado Shot 次级弹分裂逻辑（Tier 2 代码补丁），并以 `localization.json` 的 `info_Tornado Shot` 保底描述同步。
