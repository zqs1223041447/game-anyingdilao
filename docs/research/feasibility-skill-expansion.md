# 可行性报告：新增技能节点 与 新视觉/动画/音效

> 日期：2026-08-23 ｜ 输入：exp-2（代码侧事实）× lib-1（工具链）｜ 状态：**定稿**
> 一句话结论：**可行。数值层扩展接近零门槛；资产墙集中在 sharedassets1.assets 一个容器；音频墙在 Skill.bank；Spine 新动画是唯一硬墙。**

## 结论速览

| 目标 | 可行性 | 推荐路线 | 工作量 |
|---|---|---|---|
| 新增技能树节点（复用现有弹体/图标/动画/音效） | ✅ 高 | A1：UABEA 改 sharedassets1 内 CSV 加行 | 小（小时级） |
| 新增技能节点（全新行为逻辑） | ✅ 高 | A1 + 源码重编译（已验证管线）或 A2 BepInEx | 中 |
| 替换现有美术 | ✅ 高 | B1：UABEA 同尺寸 Texture2D 替换 | 小 |
| 全新投射物/特效预制体 | ⚠️ 中 | 扩 SKprefab 数组（UABEA/AssetsTools.NET）或运行时代码扩容 | 中 |
| 新增技能音效 | ⚠️ 中 | B2：预制体 SoundA 字符串改指既有 event:/ 或代码播外部音频；B3 自建 Mod.bank 进阶 | 小-中 |
| 新施法动画 | ⚠️ 低-中 | 复用 UseAni 0-3 零成本；全新动画需 Spine Editor（付费墙） | 大 |
| FMOD bank 结构性修改 | ❌ 不建议 | 无成熟工具，绕行成本远低于破墙 | - |

## 事实基础

### 数据层（exp-2 实证）
- 8 个技能 CSV（XiTA + skillTA[0-6]）是 `TalentManager` 的序列化字段，**全部内嵌于 `sharedassets1.assets`**（level1 主场景资产包，201.89MB），字节偏移已实测定位（SampleF@13650460、Xi@~9395134 等）
- 解析器 `LoadTextFile` 仅按 `\n`/`,` 切分、**不支持引号转义**；逐列 int.Parse/float.Parse → **新行必须给满全部列且数值列可解析**，否则 Awake 抛异常加载失败
- SampleF 表头 ~150 列；关键列语义：IndexName=唯一键、icon=IconData 组内 Sprite 下标、Xi=树索引兼图标组索引、FrontSkill/FatherSkill=解锁链、UnLock_Point=系内门槛、UseAni=玩家侧硬编码 switch 0-4（>4 静默落空）、OBJ=SKPB.SK_OBJ 弹体下标
- SKPB（SKprefab SO）同在 sharedassets1.assets；数组为编辑器序列化、运行时只读，越界即 IndexOutOfRange
- 图标：`IconData.icon[Sprite[]]` 直接引用，每组 ≥29 个；**复用旧索引零成本**；本地化键缺失仅 tooltip 显示原文不报错
- 存档按 IndexName 键控字典 Flush/Restore → **新键自动兼容旧档**
- UI 按钮 SkillBT 是场景对象、运行时从不实例化 → 新节点需 Clone 现有按钮改 IndexName 并 RegisterSkillBT

### 工具层（lib-1 实证）
- UABEA/UABEANext：TextAsset 编辑最成熟用法，长度可变；Mono 游戏 type-tree 从全套 Managed DLL 反推无障碍；本游戏无 Addressables → CRC 风险为零
- AssetsTools.NET：脚本化新增条目+ResourceManager 容器登记的正规路径
- FMOD 2.2.24：bank 只能等长替换；绕行 = 代码播外部音频（标准 mod 做法）或免费版自建 Mod.bank
- Spine 4.0：运行时 API 可混合/换肤复用；无 Editor 无法造新动画

## 方案 A：新增技能树节点（落地步骤）

### A1 最小改动清单（在某系追加一个主动技）
1. **备份** `sharedassets1.assets`
2. **CSV**：UABEA 打开 sharedassets1.assets → 定位对应 TextAsset（如 SampleF）→ Export Dump → 在末空行前追加一行，150 列全给满：
   - IndexName 唯一键（如 `FireStorm`）；icon 复用旧索引；Price/UnLock_Point/Xi/Level_Max 按目标系填
   - UseAni 填 0-3（复用现有施法动作）；OBJ/FStype/RTtypeFX 复用现有弹体/特效索引
   - ManaCost_Base/CoolDown_Base/Damage_Base/Level 等数值列必填
   - Import Dump 回写
3. **UI**：进游戏确认天赋树出现数据行后，处理按钮显示——运行时 Clone 方案（源码加一段：Clone 现有 SkillBT → 改 IndexName/Xi/SkillType → RegisterSkillBT）或后续研究 level1 场景编辑
4. **本地化（可选）**：resources.assets 的 Skill_FY JSON 加键，否则 tooltip 显示英文原文
5. **验证**：按 AGENTS.md 流程启动游戏 → 加点 → 施放观察

### A2 叠加项（需要新行为时）
- 直接走已验证的源码重编译管线（改 Gun/SK_* 类），或装 BepInEx 5.4.23.x 做 Harmony 补丁（两者可共存）

## 方案 B：视听资源路线

| 需求 | 路线 |
|---|---|
| 改现有图标/纹理 | B1：UABEA 同尺寸替换 Texture2D（Sprite 本体不动、不改尺寸名字） |
| 新技能名/描述 | resources.assets 的 Skill_FY/Main_FY JSON 加键 |
| 新弹体/特效 | 首选复用 OBJ/MainEL 索引零改动；真新增 = 扩 SKprefab 数组（AssetsTools.NET 脚本）+ 预制体本体（最难的一档） |
| 新音效 | 快速：预制体 SoundA[] 字符串改指既有 event:/ 路径；进阶：ModAudioManager 播外部 ogg；高阶：FMOD Studio 免费版自建 Mod.bank + LoadBank（时序需实测） |
| 新施法动画 | 复用 UseAni 0-3；全新动画被 Spine Editor 授权墙挡住（程序化 Timeline 仅适合简单 FX） |

## 必须触碰的资产容器清单

| 新增内容 | 容器 |
|---|---|
| 技能 CSV 行 | `sharedassets1.assets` |
| 新图标美术 | `sharedassets1.assets`（IconData Sprite[] + .resS 纹理） |
| 本地化文案 | `resources.assets`（_FY JSON） |
| 弹体/特效预制体 | `sharedassets1.assets`（SKprefab 数组 + .resS） |
| Spine 动画 | `sharedassets1.assets`（skeleton/atlas） |
| 技能音效 | `StreamingAssets/Desktop/Skill.bank`（71,569KB）或预制体字符串 |

## 测试计划（下一步）

1. **等 fix-12**：AssetsTools.NET 扫描清单确认各 TextAsset 的 path_id（与 exp-2 的字节偏移交叉验证）
2. **试改演练**：备份 sharedassets1.assets → UABEA 改一个现有数值（如某技能 ManaCost_Base）→ 进游戏验证生效 → 还原
3. **PoC 新技能行**：SampleF 加一行完整数据（全复用索引）→ 验证天赋树出现、可加点、可施放
4. **PoC UI 按钮**：源码加 SkillBT Clone 逻辑 → 重编译部署 → 验证按钮可见可用

## 风险登记

| 风险 | 缓解 |
|---|---|
| CSV 列数/格式错误导致 Awake 异常、游戏卡死 | 先备份；逐列对照表头；先改值后加行分步验证 |
| sharedassets1.assets 写坏 | UABEA 操作前整文件备份；SHA256 记录 |
| SKprefab 数组扩槽失败（PPtr/依赖） | 优先复用索引；扩槽走 AssetsTools.NET 脚本并小步验证 |
| UseAni>4 静默落空 | 文档已记录，新行只用 0-3 |
| FMOD bank 时序兼容 | B3 路线先做最小 LoadBank 实验 |
