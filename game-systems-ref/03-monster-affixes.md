# 《暗影地牢》(Shadow Dungeon) 怪物词缀与强化体系参考文档

本篇文档系统梳理《暗影地牢》反编译工程中的怪物词缀系统（Monster Affix System），涵盖怪物词缀的定义、生成与随机抽取机制、品质阶梯加成、通用词缀、元素化身词缀、范围光环词缀、精英/BOSS 专属词缀池以及防御塔专属词缀。

---

## 概述与词缀生成机制

在《暗影地牢》中，怪物词缀（代码中常命名为 `SSIndex` / Special Skills Index）是在关卡生成（[`LevelManager.CreatEnemies`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L920-L1063)、[`LevelManager.CreatJYs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1383-L1462)、[`LevelManager.CreatBoss`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1889-L2300)）时动态附加给怪物实体的附加属性/特殊能力。每个怪物身上维护一个长度为 5 的整型数组 `SSIndex`（默认为 0 表示无词缀），用于承载当前怪物的附加词缀 ID。

### 1. 怪物生成与品质阶梯 (Quality System)

怪物分为 4 种基础品质等级（`Quality` / `QQ`），决定其基础生命/伤害强化倍率及附加词缀槽位数：

| 品质等级 (`Quality`) | 阶层名称 / 颜色 | 基础生命加成 (`HealthMulti`) | 基础伤害加成 (`DamageMulti`) | 词缀抽取数量 | 候选词缀池范围 | 备注 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **0** | 普通怪 (Normal / 白色) | +0% | +0% | 0 | 无 | 基础怪物，无词缀 |
| **1** | 强化怪 (Magic / 蓝色) | +60% | +50% | 1 ~ 2 个 | ID 1 ~ 14 (无重复) | 随机抽取 `Random.Range(1, 3)` 个词缀 |
| **2** | 稀有怪 (Rare / 金色) | +150% | +100% | 2 ~ 3 个 | ID 1 ~ 14 (无重复) | 随机抽取 `Random.Range(2, 4)` 个词缀 |
| **3** | 精英怪 (Elite / 紫色 `Jingying`) | +600% | +250% | 3 ~ 4 个 | ID 1 ~ 16 (无重复) | 额外获得 `SK_ELSS` 元素技能，技能率随等级加倍 (`Level/10`) |
| **BOSS** | 首领怪 (Boss) | 关卡数值曲线 | 关卡数值曲线 | 5 个 | 常规关卡固定 5 个；秘境模式全池抽取 | 普通关卡固定为 `[2, 7, 9, 10, 11]`；秘境模式从 ID 1~16 中随机抽取 5 个 |

### 2. 词缀生效与附加链路

1. **生成与抽取**：[`LevelManager`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs) 初始化由 16 个元素组成的防重复池 `RDindex[0..15]`（对应 ID 1~16）。
   - 普通/蓝/黄怪生成（`CreatEnemies`）：仅向候选列表 `NoSameList` 加入前 14 个（ID 1~14），随机抽取后填入 `SSIndex`。
   - 精英怪生成（`CreatJYs`）：将全部 16 个（ID 1~16）加入候选列表，随机抽取 3~4 个填入 `SSIndex`。
   - BOSS 生成（`CreatBoss`）：常规关卡固定赋予 5 词缀；秘境关卡（`CurLevelData.IsMJ`）从全部 16 个中随机抽取 5 个。
2. **属性应用与光环挂载**：在 [`SetEnemyData`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1099-L1371) / [`SetJYData`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1472-L1862) / [`CreatBoss`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1889-L2297) 中遍历 `SSIndex`：
   - 累加对应战斗属性（如生命百分比 `Health_Bei`、全伤减免 `DamageAnti`、暴击 `BJRate`、移速攻速等）。
   - 通过 `LeanPool.Spawn` 在怪物脚底（`foot`）挂载对应的光环特效预制体（`PB.Aura_SP[0..13]` 或 `PB.Aura_EL[0..11]` 或范围光环 `PB.LQJQ[0..5]`）。
3. **UI 悬浮与战斗信息显示**：当鼠标悬停或手柄命中目标时，[`UI_EnemyTip`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI_EnemyTip.cs) 解析 `SSIndex` 并通过本地化接口 [`LOC.MM.GetMain`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LOC.cs) 显示对应的词缀名称与元素色彩。

---

## 表格一：全部怪物词缀一览表

> **说明**：ID 为底层代码 `SSIndex` 中的对应枚举值。

| ID | 词缀名称（中 / 英） | 本地化 Key | 适用类型 | 效果与数值 | 触发 / 生效条件 | 代码位置 | 备注 / 表现特效 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **1** | **嗜血狂暴**<br>Bloodlust Fury | `Crazy_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **伤害加成**：<br>• 普通/蓝/黄/BOSS：`Damage_Bei +100%`<br>• 精英怪：`Damage_Bei +200%` | 被动常驻 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1277)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1646)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2082) | 挂载脚底光环 `PB.Aura_SP[0]`；精英怪数值翻倍 |
| **2** | **致命一击**<br>Death Strike | `Force_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **暴击与穿透**：<br>• `BJRate +30%`（暴击率）<br>• `Chuan +30%`（穿透率）<br>• `FlySpeed +20%`（弹道速度） | 被动常驻 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1281)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1651)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2086) | 挂载脚底光环 `PB.Aura_SP[1]`；BOSS 常规关卡必带 |
| **3** | **极度诅咒**<br>Extreme Curse | `Curse_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **持续伤害强化**：<br>• `DotDamage +100%`（DOT伤害）<br>• `DotTime +100%`（DOT持续时长） | 怪物造成 DOT 伤害时 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1287)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1656)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2092) | 挂载脚底光环 `PB.Aura_SP[2]` |
| **4** | **极度迅捷**<br>Extreme Swiftness | `Quick_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **速度与减速抗性**：<br>• `AttackSpeed_Bei +50%`（攻速加成）<br>• `MoveSpeed_Bei +50%`（移速加成）<br>• `AntiSlow +30%`（减速抗性） | 被动常驻 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1292)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1661)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2097) | 挂载脚底光环 `PB.Aura_SP[3]` |
| **5** | **魔法精通**<br>Magic Mastery | `Magic_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **技能释放频率**：<br>• 普通/蓝/黄怪：`SK_Rate +25%`<br>• 精英 / BOSS：`SK_Rate +30%` | AI 行为判定周期 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1298)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1667)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2103) | 挂载脚底光环 `PB.Aura_SP[4]`；直接提升怪物的技能施放概率 |
| **6** | **幻影分裂**<br>Phantom Split | `Copy_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **分裂与分身召唤**：<br>• `FS_Count +[1~5]`（增加分身上限）<br>• `SK_Rate_FS +20%`（分身触发概率） | AI 施法判定与受击分裂 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1302)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1671)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2107) | 挂载脚底光环 `PB.Aura_SP[5]`；控制 `EnemyA/B/C.cs` 的分身生成 |
| **7** | **极度强韧**<br>Extreme Toughness | `Strong_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **生命与韧性**：<br>• 普通/蓝/黄/BOSS：`Health_Bei +100%`<br>• 精英怪：`Health_Bei +300%`<br>• 通用：`MoveSpeed_Bei +20%`, `AntiSlow +10%`, `yunAnti +30%`（眩晕抗性） | 被动常驻 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1307)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1676)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2112) | 挂载脚底光环 `PB.Aura_SP[6]`；精英怪生命加成高达 300%；BOSS 常规关卡必带 |
| **8** | **再生体质**<br>Regenerative Physique | `Recover_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **生命自动恢复**：<br>• `Health_Prc +3%`（每秒恢复 3% 最大生命值） | 每秒持续触发 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1314)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1683)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2119) | 挂载脚底光环 `PB.Aura_SP[7]` |
| **9** | **石化皮肤**<br>Stone Skin | `StoneSkin_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **全伤减免**：<br>• `DamageAnti +30%`（直接减少 30% 受到的所有伤害） | 受到伤害时结算 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1318)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1687)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2123) | 挂载脚底光环 `PB.Aura_SP[8]`；BOSS 常规关卡必带 |
| **10** | **魔法抗性**<br>Magic Resistance | `MagicAnti_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **全元素抗性提升**：<br>• 火/冰/雷/毒/物/影抗性各 `+30%`（`FireAnti` ~ `ShadowAnti`） | 受到对应属性伤害时 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1322)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1691)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2127) | 挂载脚底光环 `PB.Aura_SP[9]`；BOSS 常规关卡必带 |
| **11** | **免疫体质**<br>Immune Physique | `MY_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **负面状态缩减**：<br>• `DotTimeCut +50%`（受到的 DOT 持续时间减少 50%） | 被施加 DOT / 负面状态时 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1331)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1700)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2136) | 挂载脚底光环 `PB.Aura_SP[10]`；BOSS 常规关卡必带 |
| **12** | **同归于尽**<br>Mutual Annihilation | `Die_SS` | 蓝怪 / 黄怪 / 精英 / BOSS | **自爆亡语**：<br>• `Can_DieBoom = true`<br>• 死亡时触发 [`SetDieEXP`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Enemy.cs#L2869)，根据怪物主元素释放 `SKG_Die` 亡语大范围爆炸 | 怪物生命归零死亡时 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1335)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1704)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2140)<br>[`Enemy.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Enemy.cs#L2812) | 挂载脚底光环 `PB.Aura_SP[11]`；若怪物受玩家混乱技能影响，爆炸伤害转向敌方群体 |
| **13** | **元素强化 / 元素化身**<br>Elemental Reinforcement / Avatar | 见下方子表 | 蓝怪 / 黄怪 / 精英 / BOSS | **元素技能与抗性增强**：<br>• 普通/蓝/黄怪：对应主元素抗性 `+30%`，`SK_Rate_ELSS +30%`<br>• 精英 / BOSS：对应主元素抗性 `+50%`，`SK_Rate_ELSS +50%`<br>• 频繁施放对应元素的强化主动技 `SK_ELSS` | 被动常驻与施法判定 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1339)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1708)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2144) | 挂载元素光环：普通怪为 `PB.Aura_EL[0..5]`，精英/BOSS 为高阶 `PB.Aura_EL[6..11]` |
| **14** | **范围光环**<br>Aura (6种光环) | 见下方子表 | 蓝怪 / 黄怪 / 精英 / BOSS | **光环领域增益**：<br>随机赋予 6 种群体灵气之一，在脚底生成持续脉冲的光环领域（`PB.LQJQ[0..5]`），为范围内所有友方怪物提供群体属性 Buff | 领域常驻脉冲（[`EmptyCOL_BF`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L147)） | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1738)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2174)<br>[`SK_BloodPool.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/SK_BloodPool.cs) | 怪物脚底生成半径光环圈，按秒向范围内敌怪附加 [`Buff_Enemy`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Buff_Enemy.cs) |
| **15** | **召唤精通**<br>Summoning Mastery | `Comp_SS` | **精英 / BOSS 专属** | **同伴与随从强化**：<br>• 精英怪：`Comp_Count +[2~4]`，`SK_Rate_Comp +30%`<br>• BOSS：`Comp_Count +[2~4]`，`SK_Rate_Comp +30%` | 召唤技能施放时 | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1850)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2287) | 挂载脚底光环 `PB.Aura_SP[12]`；普通/蓝/黄怪池（1~14）不生成此词缀 |
| **16** | **多重射击**<br>Multi-Shot | `MultiAT_SS` | **精英 / BOSS 专属** | **连发与弹道重发**：<br>• `CF_Rate +30`（+30% 概率触发重发模式 `CF_Type` / `CF_Count`，发射多倍弹幕） | 怪物发射弹道投射物时（[`SK_Angle_F.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/SK_Angle_F.cs#L109)等） | [`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L1855)<br>[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L2292) | 挂载脚底光环 `PB.Aura_SP[13]`；普通/蓝/黄怪池（1~14）不生成此词缀 |

---

### 词缀 13：元素强化 / 元素化身形态明细

根据怪物的 `MainElement`（0:火、1:冰、2:雷、3:毒、4:物、5:影）与品质（`Quality <= 2` 为初级强化，`Quality > 2` 为高阶化身）：

| 元素类型 | 初阶名称（Quality <= 2） | 初阶 Key | 高阶名称（Quality > 2 精英/BOSS） | 高阶 Key | 属性加成 | 专属光环预制体 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **火 (Fire)** | 火焰强化 (Fire Reinforcement) | `Fire_S` | **熔火化身** (Molten Ember Avatar) | `Fire_SS` | 火抗 +30% / +50%，火系技能率 +30% / +50% | `PB.Aura_EL[0]` / `PB.Aura_EL[6]` |
| **冰 (Frozen)** | 寒霜强化 (Frost Reinforcement) | `Frozen_S` | **风暴化身** (Storm Avatar) | `Frozen_SS` | 冰抗 +30% / +50%，冰系技能率 +30% / +50% | `PB.Aura_EL[1]` / `PB.Aura_EL[7]` |
| **雷 (Thunder)** | 闪电强化 (Lightning Reinforcement) | `Thunder_S` | **雷霆化身** (Thunder Avatar) | `Thunder_SS` | 雷抗 +30% / +50%，雷系技能率 +30% / +50% | `PB.Aura_EL[2]` / `PB.Aura_EL[8]` |
| **毒 (Poison)** | 毒素强化 (Poison Reinforcement) | `Poison_S` | **瘟疫化身** (Plague Avatar) | `Poison_SS` | 毒抗 +30% / +50%，毒系技能率 +30% / +50% | `PB.Aura_EL[3]` / `PB.Aura_EL[9]` |
| **物 (Physics)** | 物理强化 (Physical Reinforcement) | `Physics_S` | **大地化身** (Earthen Avatar) | `Physics_SS` | 物抗 +30% / +50%，物理技能率 +30% / +50% | `PB.Aura_EL[4]` / `PB.Aura_EL[10]` |
| **影 (Shadow)** | 暗影强化 (Shadow Reinforcement) | `Shadow_S` | **梦魇化身** (Nightmare Avatar) | `Shadow_SS` | 影抗 +30% / +50%，影系技能率 +30% / +50% | `PB.Aura_EL[5]` / `PB.Aura_EL[11]` |

---

### 词缀 14：范围光环（6 种灵气光环）明细

当怪物获得 ID 14 词缀时，在 `Qi = Random.Range(0, 6)` 中随机决定一种光环类型并在脚下生成 `PB.LQJQ[Qi]`：

| 灵气索引 (`Qi`) | 光环名称（中 / 英） | 本地化 Key | 影响对象 | 赋予光环内友军的效果数值 | 触发类与字段 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **0** | **战斗光环** (Combat Aura) | `Aura_Battle` | 范围所有友方怪 | • 友军伤害提升：`C_Damage +20%`<br>• 友军暴击率提升：`BF_BJrate +10%` | [`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L153)<br>`Buff_Enemy.Damage / BJrate` |
| **1** | **进攻光环** (Piercing Aura) | `Aura_Chuan` | 范围所有友方怪 | • 友军元素穿透：`BF_EL_Chuan +30%`<br>• 友军护甲/伤害穿透：`BF_Through +20%` | [`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L154)<br>`Buff_Enemy.Chuan / Through` |
| **2** | **闪避光环** (Evasion Aura) | `Aura_ShanBi` | 范围所有友方怪 | • 友军格挡率提升：`BF_GeDang +20%` | [`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L157)<br>`Buff_Enemy.GeDang` |
| **3** | **敏捷光环** (Agility Aura) | `Aura_MinJie` | 范围所有友方怪 | • 友军攻击速度提升：`C_ATspeed +20%`<br>• 友军移动速度提升：`C_MVspeed +20%` | [`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L158)<br>`Buff_Enemy.AttackSpeed / MoveSpeed` |
| **4** | **防御光环** (Defensive Aura) | `Aura_FangYu` | 范围所有友方怪 | • 友军伤害减免提升：`BF_DamageAnti +20%` | [`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L160)<br>`Buff_Enemy.DamageAnti` |
| **5** | **生命光环** (Vitality Aura) | `Aura_Recover` | 范围所有友方怪 | • 友军生命恢复：`C_Health_Prc +2%`（每秒恢复 2% 最大生命值） | [`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs#L161)<br>`Buff_Enemy.Health_Prc` |

---

## 表格二：精英 / BOSS / 防御塔专属词缀分析表

### 1. 精英怪 (Elite / Quality 3) 与 BOSS 专属词缀

在关卡生成器 [`LevelManager.CreatEnemies`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs#L969) 中，普通怪与蓝/黄强化怪的词缀池被硬编码截断为 `RDindex[0..13]`（即 ID 1~14）。ID 15 与 ID 16 仅存在于 `CreatJYs`（精英）和 `CreatBoss`（BOSS / 秘境 BOSS）的词缀池中。

| 专属 ID | 专属词缀名称 | 适用群体 | 专属机制与效果数值 | 设计定位与战术威胁 |
| :--- | :--- | :--- | :--- | :--- |
| **15** | **召唤精通**<br>(Summoning Mastery / `Comp_SS`) | **精英怪 / BOSS** | • 精英怪：`Comp_Count += [2~4]`，`SK_Rate_Comp += 30%`<br>• BOSS：`Comp_Count += [2~4]`，`SK_Rate_Comp += 30%`<br>• 挂载光环 `PB.Aura_SP[12]` | 随从召唤强化型词缀。大幅提升怪物同伴/小怪生成上限与召唤频率，形成小怪海战术 |
| **16** | **多重射击**<br>(Multi-Shot / `MultiAT_SS`) | **精英怪 / BOSS** | • `CF_Rate += 30`（+30% 几率进入多重发射模式，触发 `sp.CF_Type` 与 `sp.CF_Count`）<br>• 挂载光环 `PB.Aura_SP[13]` | 弹幕压制型词缀。怪物发射投射物时有 30% 概率分裂或重复发射成倍弹幕，极高威胁 |

---

### 2. 防御塔怪物专属词缀 (Tower Special Affixes, `EnemyType == 100`)

在 [`Tower.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Tower.cs#L168-L195) 中，防御塔类怪物（如元素塔、箭塔）在生成时有 **20% 概率**触发词缀强化（基础必定获得 `Health_Bei +50%` 与 `Xp * 2`），并在自身专属的 1~4 词缀池中独立抽取：

| 防御塔词缀 ID | 词缀名称（中 / 英） | 本地化 Key | 效果与数值 | 触发与代码位置 |
| :--- | :--- | :--- | :--- | :--- |
| **1** | **坚不可摧** (Unbreakable) | `JBKC_SS` | • 额外 `Health_Bei +100%`（叠加基础后总计 **+150% 生命值**） | 被动常驻；[`Tower.cs:L179`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Tower.cs#L179) |
| **2** | **重火力** (Heavy Ordnance) | `ZHL_SS` | • `Damage_Bei +100%`（**+100% 塔体攻击伤害**） | 被动常驻；[`Tower.cs:L182`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Tower.cs#L182) |
| **3** | **狙击哨站** (Sniper Perch) | `JJSZ_SS` | • `AttackSpeed_Bei +50%`（**+50% 攻击速度**）<br>• `Range_Base *= 1.5f`（**基础射程扩大 1.5 倍**） | 被动常驻；[`Tower.cs:L185`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Tower.cs#L185) |
| **4** | **快速补给** (Rapid Resupply) | `KSBJ_SS` | • `Health_Prc +2%`（**每秒自动恢复 2% 最大生命值**） | 每秒持续触发；[`Tower.cs:L189`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Tower.cs#L189) |

---

## 说明

### 1. 数据来源与核心类清单

- **关卡生成与词缀附加中枢**：[`LevelManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/LevelManager.cs)
  - `CreatEnemies` (普通/蓝/黄怪生成及词缀抽取)
  - `SetEnemyData` (蓝/黄怪词缀数值与特效应用)
  - `CreatJYs` / `SetJYData` (精英怪生成、专属词缀池抽取与数值倍增)
  - `CreatBoss` (BOSS 固定词缀 / 秘境 BOSS 随机 5 词缀分配)
  - `SetEnemyBaseData` / `SetBossData` (怪物体型与基础属性初始化)
- **怪物实体与战斗状态模型**：[`Enemy.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Enemy.cs)
  - `SSIndex` (词缀 ID 存储槽位)
  - `OnDie` / `SetDieEXP` (亡语自爆逻辑)
  - `AuraList` / `BuffMG` (光环与 Buff 容器)
- **光环与范围领域**：[`SK_BloodPool.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/SK_BloodPool.cs)、[`EmptyCOL_BF.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/EmptyCOL_BF.cs)
- **防御塔系统**：[`Tower.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Tower.cs)
- **UI 提示与文本映射**：[`UI_EnemyTip.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI_EnemyTip.cs)
- **数据表资产（Unity TextAsset / CSV）**：
  - `sharedassets1`: `Enemy` (怪物模板配置), `Boss` (首领模板配置), `SK_Die` (死亡自爆技能组), `SK_ELSS` (元素强化技能组)
  - `resources`: `Main_FY` (词缀中英文等多语言字典), `Enemy_FY` (怪物名称字典)

### 2. 表格列含义

- **ID / 本地化 Key**：代码底层枚举索引及 `Main_FY` 资源表中的字符串标识。
- **适用类型**：区分蓝怪/黄怪、精英怪（Quality 3）、BOSS（Quality 4+）或防御塔（EnemyType 100）。
- **效果与数值**：代码直接修改的浮点/整型变量增量（如 `Health_Bei +100%` 表示基础生命放大 100%）。
- **触发条件**：被动常驻生效、受击/受控制结算、死亡亡语、施法判定或按秒脉冲。
- **代码位置**：修改或查看该词缀逻辑对应的类与行号。

### 3. MOD 修改注意事项

1. **词缀池扩充**：若要向常规怪物开放「召唤精通 (15)」或「多重射击 (16)」，只需修改 `LevelManager.CreatEnemies` 中的 `for (int i = 0; i < 14; i++)` 循环上限为 16。
2. **新增自定义词缀**：
   - 需在 `LevelManager.RDindex` 扩展数组长度并在 `Start`/`InitData` 中注册索引。
   - 在 `SetEnemyData` / `SetJYData` / `CreatBoss` 的 `switch (sSIndex[j])` 分支中添加逻辑与特效挂载。
   - 在 `UI_EnemyTip.SimpleEM` 中添加对应 `case` 及多语言显示。
3. **词缀数值平衡**：精英怪的生命加成（`Health_Bei`）与伤害加成（`Damage_Bei`）具有极高基础（+600% / +250%），在此基础上词缀 1（嗜血狂暴）会再叠 +200%，词缀 7（极度强韧）会再叠 +300%，设计新词缀时需注意乘算/加算曲线。

### 4. 未覆盖或存疑项

- **BOSS 词缀在剧情关卡与秘境模式的差异**：常规主线关卡中，所有 BOSS 的 `SSIndex` 被硬编码为 `[2, 7, 9, 10, 11]`（致命一击 + 极度强韧 + 石化皮肤 + 魔法抗性 + 免疫体质），而在秘境爬塔（`IsMJ == true`）中 BOSS 会从全部 16 种词缀中完全随机抽取 5 种。
