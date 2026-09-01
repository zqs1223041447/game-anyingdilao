# 《暗影地牢》游戏系统参考库（Game Systems Reference）

> **用途**：本文件夹是后续 AI（或人工）对《暗影地牢》**装备体系**与**技能体系**进行修改、调整、平衡性改动时的**唯一入口**。
> 所有表格由多个 AGY 独立梳理自 `MODworkv2/decompiled/`（新版 Assembly-CSharp.dll 反编译工程，895 个 .cs，Unity 2019.4.39f1 Mono）生成，已按主题持久化。

---

## 一、文件索引

| 文件 | 主题 | 覆盖内容 | 核心数据来源 |
|---|---|---|---|
| [01-equipment-affixes.md](01-equipment-affixes.md) | 装备词缀 | 装备前缀/后缀/词条（230+ 效果项、7 级稀有度规则、主属性/机制/DOT/技能特化/同伴特化/特技法球等分类） | `WeaponClass.cs`、`ItemManager.cs`、`WPDT_A.cs`、`WPDT_B.cs`、`SaveDataEquipmentSanitizer.cs`；CSV：`0 0 Weapon.csv` / `1 0 Main.csv` / `1 1 DOT.csv` / `1 2 SK.csv` / `0 1 SPC.csv` |
| [02-socket-affixes.md](02-socket-affixes.md) | 镶嵌物词缀 | 镶嵌物/宝石体系（137 项：彩色宝石 48 + 融合精华 12 + 功能铸造石 11 + 技能/特效符文 2 + 基础属性符文 64）、插槽逻辑、镶嵌/移除流程 | `BaoshiClass.cs`、`WPAocao.cs`、`WeaponBaoshiApplyUtil.cs`、`BaoshiManager.cs`、`WeaponClass.cs`、`ItemManager.cs`、`BaoshiSettings.cs` |
| [03-monster-affixes.md](03-monster-affixes.md) | 怪物词缀 | 怪物词缀（20 条：通用/高阶 16 + 防御塔专属 4）、4 品质阶梯、元素化身 6 形态、范围光环 6 种、**精英/BOSS 专属词缀池**（ID 15 召唤精通 / ID 16 多重射击）、SSIndex 生效链路 | `LevelManager.cs`（CreatEnemies/CreatJYs/CreatBoss/SetEnemyData/SetJYData）、`Enemy.cs`、`UI_EnemyTip.cs`、`Buff_Enemy.cs`、`EmptyCOL_BF.cs` |
| [04-forging-effects.md](04-forging-effects.md) | 锻造效果 | 装备锻造/强化体系（33 项核心机制：武器三重铸/强化、7 功能魔法石、12 淬炼精华、8 阶宝石镶嵌、技能/特技符文、16 类 64 阶基础属性符文） | `UI.Panels/WeaponManager.cs`、`WeaponBaoshiApplyUtil.cs`、`BaoshiManager.cs`、`WeaponClass.cs`、`ItemManager.cs`、`WeaponSettings.cs`、`BaoshiSettings.cs` |
| [05-skills.md](05-skills.md) | 技能体系 | 技能全体系（核心 131 项：主技能 70 + 同伴召唤 13 + 全局 DOT 12 + 被动倍率 36；另 267 增幅子节点 + 50 巅峰天赋）、施法链路、SP/CP 双资源、技能与天赋树关联 | `SkillData.cs`（七子类）、`PlayerActionManager.cs`、`Gun.cs`、`TalentManager.cs`、`SK_FlyA.cs`、`Skill_FY` 本地化键（resources.assets path_id=433）、SampleF CSV |

每份文档内部结构统一：**概述（体系运作机制）→ 完整 Markdown 表格（含代码位置列）→ 「## 说明」章节（数据来源、列含义、修改注意事项、未覆盖/存疑项）**。

---

## 二、给 AI 的修改指导（必读）

### 1. 定位与修改
- 所有数值与机制修改的**第一落点**是 `MODworkv2/decompiled/` 下对应核心类（见各表「代码位置」列），**不要直接改 `ShadowDungeon/` 下的 DLL**。
- 涉及 CSV 数据表（SampleF/Sample_F、Main、DOT、SK、SPC、Weapon、Baoshi 等 TextAsset）时，先阅读对应文档的「CSV 与资源数据表结构说明」章节，确认列含义后再动数据。
- 技能/词缀文案修改需要同步 `Skill_FY` 本地化键（resources.assets TextAsset path_id=433）；**加行必须有 Info 列指向的本地化键**，禁止"行为已改、描述仍旧文案"的中间态出包（细则见 `docs/skill-spec.md`「描述同步要求」）。

### 2. 修改后强制流程（AGENTS.md 工作规范）
1. `dotnet build MODworkv2/decompiled/Assembly-CSharp.csproj -c Release`，**0 error** 才可部署。
2. 部署前确认 `MODworkv2/backup/Assembly-CSharp.dll` 为原版备份；**禁止无备份覆盖**。
3. 产物覆盖到 `ShadowDungeon/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll`，SHA256 比对一致。
4. 启动游戏验证：进程存活 ≥35 秒 + Player.log 中 Exception/Crash/TypeLoad/NullReference 命中数为 0。
5. 在根目录 `CHANGELOG.md` 登记版本（V 号顺序递增，含日期/变更/涉及文件/SHA256/验证/部署状态）。

### 3. 平衡性改动注意
- 数值改动前先看各表「备注」列：词缀/词缀有**品质阶梯差异**（如精英怪数值翻倍）、**随机池限制**（如普通怪只抽 ID 1~14）、**固定组合**（如常规 BOSS 固定 `[2,7,9,10,11]`），改动时确认不会破坏生成规则。
- 技能改动涉及弹幕形态/数量/飞行行为时，需同步更新 05-skills.md 对应条目与本地化描述，保证 tooltip 与实际行为一致。
- 装备词缀与镶嵌物存在**交叉联动**（符文可生成技能/特技词缀），改动前同时查 01 / 02 / 04 三份文档。

### 4. 本库维护
- 每次装备/技能相关改动部署后，请同步更新本文件夹对应表格（数值、条目、代码位置），保持参考库与源码一致。
- 新增系统（如新词缀池、新锻造槽位）时，在对应文档追加表格行并登记说明。

---

## 三、通用说明

- 文档由多个 AGY 独立梳理，**代码位置列**标注为"类名 / 方法名 / 行号"（如 `LevelManager.cs#L1277`），可直接跳转反编译源码复核。
- 各表「备注」列包含表现特效（光环预制体 `PB.Aura_*`、`PB.LQJQ[*]`）、条件差异与存疑提示。
- 若后续代码基线变化导致行号偏移，以类名/方法名为准重新定位。
- 生成时间：2026-08-27（对应源码基线：新版 Assembly-CSharp.dll，2,313,728 B 原版 / 当前部署版见 CHANGELOG）。
