# 《暗影地牢》(Shadow Dungeon) 装备词缀（Affix）体系全景解析

## 概述与体系运作机制

《暗影地牢》(Shadow Dungeon) 的装备词缀（Affix）体系是一个深度结合**装备基底、动态数值成长、概率剥离补偿、多层衍生转化与战地实体联动**的复杂 RPG 属性系统。所有装备（包含主手武器、副手装备、四件防具、四件饰品共 10 个槽位）在底层统一采用 [`WeaponClass`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs) 进行封装与管理。

### 1. 核心架构与数据模型
- **基底定义 (`Item_MB`)**：由数据表 `0 0 Weapon.csv` 定义每件装备的名称、品级、基础伤害/生命/法力、元素白字、固定词缀（`Main`/`DOT`/`SK`/`CP`）、随机词缀池引用（`RateMain`/`RateDot`/`RateSK`）、专属技能槽（`SkillA`~`SkillF`）、特技池（`SPC`）、孔数（`CurAocaoCount`）及套装 ID（`Set_Index`）。
- **运行时对象 (`WeaponClass`)**：装备生成时将基底数据转化为运行时对象，挂载 8 大属性/词缀容器：
  1. `Damage`, `Health`, `Mana` + 6 大元素值（`Fire`, `Frozen`, `Thunder`, `Poison`, `Physics`, `Shadow`）
  2. `WPDT_A[] Main`（基础与机制主词缀）
  3. `WPDT_A[] DOT`（异常状态与持续伤害词缀）
  4. `WPDT_B[] SK`（主动法术/物理技能特化词缀）
  5. `WPDT_B[] CP`（同伴/召唤物特化词缀）
  6. `List<WPSkill> WPSK`（提升指定主动天赋等级）
  7. `List<WPSPC> SPC`（特殊投射物/法球/仙灵特技词缀）
  8. `WPFW_Base FW_Base` / `List<WPAocao> Aocao`（符文附魔与宝石镶嵌槽）
  9. `Set_DT SetRuntimeData`（套装激活数据）

### 2. 品质档位（Quality Rarity）
游戏分为 7 个稀有度档位，由 [`QualityColor`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/QualityColor.cs) 及 [`ItemManager`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs) 驱动：
- **0 - 普通 (Normal, 白色 `#ffffffff`)**：基础白字属性，通常无随机词条或少量基础词条。
- **1 - 魔法 (Magic, 绿色 `#53FF6B`)**：包含 1~2 条随机词缀。
- **2 - 稀有 (Rare, 蓝色 `#37C5FF`)**：包含 2~3 条随机词缀，开始出现技能特化。
- **3 - 极品 (Exquisite, 紫色 `#B63EFF`)**：包含 3~4 条随机词缀，词缀数值区间放大。
- **4 - 史诗 (Epic, 粉色 `#FF50B5`)**：包含 4~5 条随机词缀，大概率生成 2 条以上技能特化词缀。
- **5 - 传说 (Legendary, 橙色 `#FF7200`)**：高阶词缀组合，全词缀满配，可洗练出高阶机制词条。
- **6 - 神话 (Mythical, 金色 `#FFCA00`)**：顶级神装，自带顶级套装/技能共鸣与额外随机强化乘数。

### 3. 词缀生成与数值计算规则
1. **基础白字成长公式**：
   $$\text{DamageFinal} = \lfloor \text{BaseDamage} \times 1.066^{\text{Level}} \times (1 \pm \text{RandomCount}) \times \text{DropPRC} \rfloor \times \text{BaseValueMultiplier}$$
2. **词缀数值浮动机制 (`GenerateWeaponStatValue`)**：
   - **普通浮点百分比**：受角色等级与秘境层数乘数缩放（普通地图 90 级以上为 $1.1 \sim 1.3$ 倍；秘境炼狱难度高达 $1.4 \sim 1.6$ 倍）。
   - **整数阶梯成长 (`ApplyWeaponIntegerGrowth`)**：50 级以上或秘境掉落时，特定整数词缀（如层数、数量）有 $30\% \sim 80\%$ 概率额外递增 $+1 \sim +2$。
   - **品质剥离与白字补偿 (`ApplyQualityAttributeRemoval`)**：低品质装备在生成时有 $10\% \sim 30\%$ 概率剥离末尾词缀，但每剥离一条，装备基础白字（Damage/Health/Mana）会获得 $10\% \sim 20\%$ 的乘算补偿。
   - **特技未命中补偿**：武器若在 70%~80% 的特技检定中未生成 `WPSPC`，系统会自动按品质给予 $10\% \sim 50\%$ 的基础白字额外补偿。

---

## 装备词缀全条目一览表

### 1. 核心主词缀（Main Affixes: WPDT_A, Index 1 ~ 1955）

| 词缀名称 | 稀有度/档位 | 效果与数值 | 适用部位 | 获取/附加方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **最大生命%** (HealthMax) | 普通~神话 (Q0-Q6) | 最大生命值增加 `+{0}%` (基础 5%~25%) | 全部位通用 | 掉落 / 商店 / 锻造 / `1 0 Main.csv` | [`WeaponClass.cs:L787`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L787) | Index 1，对应 Player.Health_Bei |
| **最大法力%** (ManaMax) | 普通~神话 (Q0-Q6) | 最大法力值增加 `+{0}%` (基础 5%~25%) | 全部位通用 | 掉落 / 商店 / 锻造 / `1 0 Main.csv` | [`WeaponClass.cs:L789`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L789) | Index 2，对应 Player.Mana_Bei |
| **生命秒回** (Health Recovery) | 普通~神话 (Q0-Q6) | 每秒生命回复增加 `+{0}` (随等级指数成长) | 防具/饰品/副手 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L791`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L791) | Index 3，scaleMainRecoveryValues 缩放 |
| **法力秒回** (Mana Recovery) | 普通~神话 (Q0-Q6) | 每秒法力回复增加 `+{0}` (随等级指数成长) | 防具/饰品/副手 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L793`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L793) | Index 4，scaleMainRecoveryValues 缩放 |
| **击中生命回复** (Hit Health) | 普通~神话 (Q0-Q6) | 每次攻击命中回复生命 `+{0}` | 主手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L795`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L795) | Index 5，Attack_R_health_Base |
| **击中法力回复** (Hit Mana) | 普通~神话 (Q0-Q6) | 每次攻击命中回复法力 `+{0}` | 主手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L797`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L797) | Index 6，Attack_R_mana_Base |
| **基础伤害%** (Damage%) | 魔法~神话 (Q1-Q6) | 全局物理/法术伤害增加 `+{0}%` (3%~20%) | 主手/副手/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L799`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L799) | Index 10，对应 Player.Damage_Bei |
| **攻击速度%** (AttackSpeed%) | 魔法~神话 (Q1-Q6) | 攻击与施法速度增加 `+{0}%` (3%~15%) | 主手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L801`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L801) | Index 11，对应 Player.ATSpeed_Bei |
| **移动速度%** (MoveSpeed%) | 魔法~神话 (Q1-Q6) | 移动速度增加 `+{0}%` (2%~10%) | 鞋子/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L803`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L803) | Index 12，对应 Player.MVSpeed_Bei |
| **暴击几率%** (Critical Rate) | 稀有~神话 (Q2-Q6) | 暴击几率增加 `+{0}%` (2%~12%) | 主手/副手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L805`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L805) | Index 13，对应 Player.BJrate |
| **暴击伤害%** (Critical Damage) | 稀有~神话 (Q2-Q6) | 暴击伤害倍率增加 `+{0}%` (5%~30%) | 主手/副手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L807`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L807) | Index 14，对应 Player.BJDamage |
| **冷却缩减%** (Cooldown) | 稀有~神话 (Q2-Q6) | 技能冷却时间缩短 `+{0}%` (3%~15%) | 头部/副手/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L809`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L809) | Index 15，受到上限 200 限制 |
| **法力消耗降低%** (Mana Cost Cut) | 魔法~神话 (Q1-Q6) | 技能法力消耗降低 `+{0}%` (5%~20%) | 头部/胸甲/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L811`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L811) | Index 16，对应 Player.ManaXH |
| **格挡几率%** (Block Rate) | 稀有~神话 (Q2-Q6) | 格挡几率增加 `+{0}%` (2%~10%) | 盾牌/胸甲/手套 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L813`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L813) | Index 17，受到上限 201 限制 |
| **伤害减免%** (Damage Anti) | 稀有~神话 (Q2-Q6) | 受到伤害直接减免 `+{0}%` (2%~8%) | 全防具/饰品6 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L815`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L815) | Index 18，受到上限 202 限制 |
| **持续伤害减免%** (DOT Cut) | 魔法~神话 (Q1-Q6) | 受到持续伤害降低 `+{0}%` (5%~25%) | 防具/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L817`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L817) | Index 19，对应 Player.DOTcut |
| **减速抗性%** (Anti Slow) | 魔法~神话 (Q1-Q6) | 减速效果削减 `+{0}%` (10%~40%) | 鞋子/防具 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L819`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L819) | Index 20，对应 Player.AntiSlow |
| **全元素穿透%** (All Penetration) | 极品~神话 (Q3-Q6) | 穿透所有敌对元素抗性 `+{0}%` (1%~5%) | 副手/饰品8/主手 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L821`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L821) | Index 21，对应 Player.AllChuan |
| **全元素抗性%** (All Resistance) | 极品~神话 (Q3-Q6) | 提升全部 6 系元素抗性 `+{0}%` (1%~5%) | 防具/饰品6 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L823`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L823) | Index 22，对应 Player.AllAnti |
| **受暴击伤害减免%** (BJD Anti) | 稀有~神话 (Q2-Q6) | 受到暴击伤害降低 `+{0}%` (5%~20%) | 头部/胸甲 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L825`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L825) | Index 30 |
| **精英/首领增伤%** (Elite Damage) | 稀有~神话 (Q2-Q6) | 对精英与首领伤害增加 `+{0}%` (5%~25%) | 主手/副手/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L827`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L827) | Index 31，JYBoss_DMG |
| **精英/首领减伤%** (Elite Anti) | 稀有~神话 (Q2-Q6) | 受到精英与首领伤害减免 `+{0}%` (5%~20%) | 胸甲/盾牌/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L829`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L829) | Index 32，JYBoss_Anti |
| **掉落几率%** (Drop Rate) | 魔法~神话 (Q1-Q6) | 物品掉落率增加 `+{0}%` (3%~20%) | 头部/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L831`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L831) | Index 50，ItemDrop_Rate |
| **投射物速度%** (Projectile Speed) | 魔法~神话 (Q1-Q6) | 投射物飞行速度增加 `+{0}%` (5%~25%) | 武器/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L833`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L833) | Index 51，FlySpeed |
| **法球技能伤害%** (Orb Damage) | 稀有~神话 (Q2-Q6) | 附带法球伤害增加 `+{0}%` (5%~25%) | 饰品/副手/主手 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L835`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L835) | Index 52，ORB_Damage |
| **击晕几率%** (Stun Rate) | 稀有~神话 (Q2-Q6) | 攻击击晕敌人几率增加 `+{0}%` (2%~10%) | 主手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L837`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L837) | Index 53，JYrate |
| **投射物穿透率%** (Pierce Rate) | 稀有~神话 (Q2-Q6) | 投射物穿透几率增加 `+{0}%` (5%~20%) | 主手/手套/副手 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L839`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L839) | Index 54，ThroughRate |
| **生命百分比吸收** (Health Steal%) | 史诗~神话 (Q4-Q6) | 造成伤害转化为生命回复 `+{0}%` | 主手/手套 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L845`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L845) | Index 62，DMG_R_H |
| **法力百分比吸收** (Mana Steal%) | 史诗~神话 (Q4-Q6) | 造成伤害转化为法力回复 `+{0}%` | 主手/手套 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L847`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L847) | Index 63，DMG_R_M |
| **宝石数值加成** (Gem Flat Boost) | 史诗~神话 (Q4-Q6) | 装备镶嵌宝石基础属性 `+{0}` (固定值+1~+3) | 武器/防具 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L849`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L849) | Index 80，PL.BS_Add |
| **宝石效果增强%** (Gem Multi Boost) | 史诗~神话 (Q4-Q6) | 装备镶嵌宝石效果提升 `+{0}%` (5%~25%) | 武器/防具 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L851`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L851) | Index 81，PL.BS_Multi |
| **同伴基础属性加成** (Companion Stats) | 魔法~神话 (Q1-Q6) | 同伴生命/伤害/攻速/移速/全抗 `+{0}%` | 饰品/防具/副手 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L853`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L853) | Index 100~104 |
| **特技伤害/几率%** (Weapon SPC Boost) | 稀有~神话 (Q2-Q6) | 武器特技伤害 `+{0}%` / 触发几率 `+{0}%` | 武器/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L863`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L863) | Index 150 (伤害), 151 (几率) |
| **神殿/药水持续时间%** (Temple/Drink Time) | 魔法~神话 (Q1-Q6) | 神殿增益/药水持续时间增加 `+{0}%` | 头部/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L867`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L867) | Index 170 (神殿), 171 (药水) |
| **上限提升词缀** (Stat Cap Breakers) | 传说~神话 (Q5-Q6) | 冷却上限/格挡上限/减伤上限 `+{0}%` | 专属防具/饰品 | 掉落 / 极品掉落 | [`WeaponClass.cs:L871`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L871) | Index 200 (CD), 201 (格挡), 202 (减伤) |
| **持续伤害通用强化** (DOT Enhancements) | 稀有~神话 (Q2-Q6) | DOT伤害%/持续时间%/层数上限/移动敌人DOT增伤 | 武器/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L883`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L883) | Index 300 (伤害), 301 (时间), 302 (层数+N), 303 (移动增伤) |
| **满层DOT斩杀** (DOT Execute) | 传说~神话 (Q5-Q6) | DOT满层时斩杀非首领精英并回复生命 | 专属武器/饰品 | 极品掉落 / 套装 | [`WeaponClass.cs:L897`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L897) | Index 307，Dot_MSAll |
| **强化武器共鸣** (PerEnhancedWeapon) | 极品~神话 (Q3-Q6) | 已装备每件强化过的武器：指定属性 `+{0}%` | 全部位 | 掉落 / 锻造共鸣 | [`WeaponClass.cs:L899`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L899) | Index 400~414（伤害/攻速/移速/暴击/同伴/DOT/陷阱等） |
| **附带技能武器共鸣** (PerWeaponWithSkill) | 极品~神话 (Q3-Q6) | 已装备每件带技能的武器：指定属性 `+{0}%` | 全部位 | 掉落 / 锻造共鸣 | [`WeaponClass.cs:L929`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L929) | Index 415~429 |
| **幻化武器共鸣** (PerTransmutedWeapon) | 极品~神话 (Q3-Q6) | 已装备每件幻化过的武器：指定属性 `+{0}%` | 全部位 | 掉落 / 锻造共鸣 | [`WeaponClass.cs:L959`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L959) | Index 430~444 |
| **天赋技能点共鸣** (PerWeaponSkillPoint) | 极品~神话 (Q3-Q6) | 武器上每一个天赋技能点：指定属性 `+{0}%` | 全部位 | 掉落 / 锻造共鸣 | [`WeaponClass.cs:L989`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L989) | Index 445~454 (454 为法球数量增加) |
| **宝石镶嵌共鸣** (PerSocketedGem) | 极品~神话 (Q3-Q6) | 武器上镶嵌的每一颗宝石：指定属性 `+{0}%` | 全部位 | 掉落 / 锻造共鸣 | [`WeaponClass.cs:L1009`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1009) | Index 455~464 (464 为法球数量增加) |
| **低血/满血触发** (Health Threshold) | 稀有~神话 (Q2-Q6) | 生命低于 20%/50% 或高于 90%/满血时增伤/减伤 | 防具/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1029`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1029) | Index 500~508 (508: 低血免疫暴击) |
| **低蓝/满蓝触发** (Mana Threshold) | 稀有~神话 (Q2-Q6) | 法力低于 20%/50% 或高于 90%/满蓝时增伤/受击技能 | 饰品/防具 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1047`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1047) | Index 509~514 |
| **移动/静止/冲刺状态** (Movement States) | 稀有~神话 (Q2-Q6) | 移动中/静止站立时/冲刺中：伤害/攻速/减伤/回血加成 | 鞋子/胸甲/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1059`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1059) | Index 550~559 |
| **生命/法力属性转化** (Stat Conversion) | 史诗~神话 (Q4-Q6) | 最大/已损生命或法力值的 `{0}%` 转化为伤害 | 武器/副手/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1079`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1079) | Index 600~604 |
| **元素伤害转化** (Element Conversion) | 史诗~神话 (Q4-Q6) | 最大血量/蓝量/冷却/抗性/穿透/格挡等转化为指定元素伤害 | 武器/副手/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1089`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1089) | Index 610~618，带有 EL 参数 |
| **双向机制转化** (Special Mechanics) | 传说~神话 (Q5-Q6) | 溢出暴击率转暴伤 / 元素穿透转暴伤 / 移速转攻速 | 专属饰品/武器 | 极品掉落 | [`WeaponClass.cs:L1111`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1111) | Index 650~655 (654: 溢出暴击转暴伤) |
| **伤害与机制权衡** (Tradeoffs) | 传说~神话 (Q5-Q6) | 伤害大幅提升但耗蓝加倍/受伤害增加/无法格挡 | 专属诅咒装备 | 极品掉落 | [`WeaponClass.cs:L1123`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1123) | Index 750 (耗蓝), 751 (受增伤), 752 (DOT转直伤), 753 (禁格挡) |
| **施法叠层增益** (OnSkillCast Stacks) | 稀有~神话 (Q2-Q6) | 每次施法：伤害/攻速/穿透/暴击/DOT伤害提升，持续N秒叠N层 | 主手/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1131`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1131) | Index 800~808 |
| **击杀叠层/重置** (OnKill Mechanics) | 稀有~神话 (Q2-Q6) | 击杀敌人/精英：伤害/攻速/元素伤害/同伴伤害提升；击杀重置冷却 | 武器/手套/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1279`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1279) | Index 1250~1276 (1276: 击杀刷新所有技能) |
| **战地实体增伤** (OnField Objects) | 极品~神话 (Q3-Q6) | 场上存在指定投射物/法球/陷阱/图腾等每个提供 `{0}%` 增伤 | 专属装备/套装 | 掉落 / 锻造 / 套装 | [`WeaponClass.cs:L1261`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1261) | Index 1100~1146 (对应 47 种场上实体) |
| **距离机制与暴击斩杀** (Distance & Boom) | 史诗~神话 (Q4-Q6) | 伤害随距离递增 / 暴击尸体爆炸 / 暴击概率直接秒杀 | 远程武器/饰品 | 掉落 / 商店 / 锻造 | [`WeaponClass.cs:L1325`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1325) | Index 1390, 1391, 1395, 1396, 1397 |
| **高级被动机制** (Legendary Passives) | 传说~神话 (Q5-Q6) | 耗蓝转回血 / 蓝回转血回 / 伤害反弹 / 龟壳无敌 / 自动喝药 / 仙灵强化 | 传说防具/饰品 | 极品掉落 / 秘境 | [`WeaponClass.cs:L1371`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1371) | Index 1801~1822, 1900~1955 |

---

### 2. 持续伤害与异常词缀（DOT Affixes: WPDT_A, Index 2000 ~ 2604）

| 词缀名称 | 稀有度/档位 | 效果与数值 | 适用部位 | 获取/附加方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **附加层数增加** (Extra Layer) | 魔法~神话 (Q1-Q6) | 每次施加指定 DOT 额外附加 `+{0}` 层 (1~4层) | 武器/手套/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1814`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1814) | Index 2000，受 EL 元素绑定 |
| **暴击额外叠层** (Crit Adds Layer) | 稀有~神话 (Q2-Q6) | 攻击暴击时额外施加 1 层指定 DOT | 武器/手套/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1815`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1815) | Index 2001，Crit_One |
| **反击额外叠层** (Counter Adds Layer) | 稀有~神话 (Q2-Q6) | 成功格挡反击时额外施加 `+{0}` 层 DOT | 盾牌/胸甲 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1816`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1816) | Index 2002，FJ |
| **跳字概率叠层** (Tick Adds Layer) | 极品~神话 (Q3-Q6) | DOT 每次跳伤害有 `{0}%` 概率额外增加 1 层 | 武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1817`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1817) | Index 2003，DMG_AddOne |
| **直接满层几率** (Instant Max Stacks) | 传说~神话 (Q5-Q6) | 施加 DOT 时有 `{0}%` 概率直接施加满层 | 武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1818`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1818) | Index 2004，All_LayerR |
| **施加层数翻倍** (Double Layer) | 传说~神话 (Q5-Q6) | 施加该 DOT 时层数直接翻倍 | 专属武器/套装 | 掉落 / 套装 | [`WeaponClass.cs:L1819`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1819) | Index 2005，Double_Layer |
| **传染机制** (Infect Enabled) | 稀有~神话 (Q2-Q6) | 敌人携带 DOT 死亡时传染给周围敌人 | 武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1820`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1820) | Index 2100，Dot_Infect |
| **传染层数增加** (Infect Extra Stacks) | 稀有~神话 (Q2-Q6) | 死亡传染时额外附加 `+{0}` 层 | 武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1821`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1821) | Index 2101，Dot_Infect_Layer |
| **传染全额层数** (Infect All Stacks) | 史诗~神话 (Q4-Q6) | 死亡传染时将死亡目标身上的全部层数无损传染 | 专属装备/套装 | 掉落 / 套装 | [`WeaponClass.cs:L1822`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1822) | Index 2102，Dot_Infect_All |
| **周期引爆** (Periodic Detonation) | 史诗~神话 (Q4-Q6) | 每 3 秒自动引爆目标身上所有 DOT 层数造成爆发 | 武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1823`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1823) | Index 2200，YB |
| **引爆保留半数层数** (Detonate Half Retain) | 史诗~神话 (Q4-Q6) | 引爆 DOT 时只消耗半数层数而非全部清除 | 专属装备/套装 | 掉落 / 套装 | [`WeaponClass.cs:L1824`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1824) | Index 2201，YB_half |
| **引爆概率秒杀** (Detonate Execute) | 传说~神话 (Q5-Q6) | 引爆 DOT 时有 `{0}%` 几率直接斩杀非 Boss 目标 | 传说武器/饰品 | 极品掉落 | [`WeaponClass.cs:L1826`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1826) | Index 2203，YB_MS |
| **异常易伤深化** (Vulnerable Amp) | 魔法~神话 (Q1-Q6) | 携带该 DOT 的敌人受到的伤害加深 `+{0}%` | 武器/副手/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1827`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1827) | Index 2300，YS |
| **DOT 允许暴击** (DOT Can Crit) | 传说~神话 (Q5-Q6) | 该元素的持续伤害跳字可以产生暴击 | 专属武器/饰品 | 极品掉落 / 套装 | [`WeaponClass.cs:L1834`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1834) | Index 2400，Dot_Crit |
| **8层生命百分比扣除** (8-Stack HP Cut) | 史诗~神话 (Q4-Q6) | 叠满 8 层时每次跳字直接扣除目标最大生命 `{0}%` | 武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1836`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1836) | Index 2402，LayerPRC |
| **冰冻永久持续** (Freeze Forever) | 传说~神话 (Q5-Q6) | 冰冻效果不随时间衰减，直至受到破冰伤害 | 冰系专属装备 | 极品掉落 / 套装 | [`WeaponClass.cs:L1843`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1843) | Index 2600，FrozenFoever |
| **冰冻削减生命上限** (Freeze HP Cap Cut) | 史诗~神话 (Q4-Q6) | 处于冰冻状态的敌人最大生命上限临时削减 `{0}%` | 冰系专属防具/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1844`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1844) | Index 2601，FrozenCut |
| **冰冻敌人受伤害加深** (Frozen Damage Taken) | 稀有~神话 (Q2-Q6) | 处于冰冻状态的敌人受到伤害增加 `+{0}%` | 冰系专属武器/饰品 | 掉落 / `1 1 DOT.csv` | [`WeaponClass.cs:L1846`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L1846) | Index 2603，FrozenHurtDMG |

---

### 3. 主动技能特化词缀（Skill Affixes: WPDT_B, Index 3000 ~ 3561）

| 词缀名称 | 稀有度/档位 | 效果与数值 | 适用部位 | 获取/附加方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **技能形态蜕变** (Skill Transform) | 史诗~神话 (Q4-Q6) | 将指定技能转换为进阶形态（如全局变更弹道/判定） | 专属主手/副手 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2018`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2018) | Index 3000，读取 SkilChangeData |
| **发射数量增加** (Fire Count +N) | 稀有~神话 (Q2-Q6) | 指定技能主投射物发射数量 `+{0}` (增加 1~3 发) | 主手/手套/副手 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2028`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2028) | Index 3100，CT_F |
| **分裂数量增加** (Split Count +N) | 稀有~神话 (Q2-Q6) | 指定技能命中后分裂数量 `+{0}` | 主手/副手/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2030`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2030) | Index 3101，CT_S |
| **目标数量增加** (Target Count +N) | 稀有~神话 (Q2-Q6) | 指定范围/索敌技能最大目标数 `+{0}` | 主手/副手/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2032`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2032) | Index 3102，CT_AT |
| **连发/多发数量** (Multi Shot +N) | 极品~神话 (Q3-Q6) | 指定技能连发轮数或齐射数量 `+{0}` | 主手/手套 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2034`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2034) | Index 3103，CT_Mul |
| **技能联动触发** (Link Fire) | 史诗~神话 (Q4-Q6) | 施放此技能时，自动连带触发另一个指定技能 | 专属武器/副手 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2036`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2036) | Index 3200，LinkSK |
| **就绪全连发** (Link All Ready) | 传说~神话 (Q5-Q6) | 施放时若联动技能全部就绪，则一次性全部倾泻 | 传说专属副手 | 极品掉落 / 套装 | [`WeaponClass.cs:L2045`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2045) | Index 3201，LinkAll |
| **每次必联动** (Every Skill Link) | 传说~神话 (Q5-Q6) | 无论冷却状态，每次施法必定触发联动技能 | 传说专属武器 | 极品掉落 / 套装 | [`WeaponClass.cs:L2047`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2047) | Index 3202，EveryLink |
| **伤害完美继承** (Damage Inherit) | 史诗~神话 (Q4-Q6) | 联动触发的技能享受主触发技能的全部伤害加成 | 专属副手/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2049`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2049) | Index 3203 |
| **自动施法** (Auto Use Skill) | 极品~神话 (Q3-Q6) | 技能冷却完毕且周围有敌人时自动施放 | 头部/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2058`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2058) | Index 3300，AutoUse |
| **施法刷新冷却** (Refresh Chance) | 稀有~神话 (Q2-Q6) | 施法时有 `{0}%` 概率立即刷新冷却 | 主手/副手/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2060`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2060) | Index 3301，Refresh |
| **命中目标增伤** (Damage Per Target) | 稀有~神话 (Q2-Q6) | 技能每额外穿透/命中一个目标，伤害提升 `{0}%` | 主手/手套 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2062`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2062) | Index 3302，ATtar_DMG |
| **同伴数量增伤** (Damage Per Comp) | 稀有~神话 (Q2-Q6) | 场上每存在一个同伴，该技能伤害提升 `{0}%` | 召唤系武器/副手 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2064`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2064) | Index 3303，CompUP_DMG |
| **格挡触发施法** (Block Cast) | 史诗~神话 (Q4-Q6) | 受到攻击并成功格挡时，有 `{0}%` 几率瞬发此技能 | 盾牌/胸甲 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2070`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2070) | Index 3306，GD_Use |
| **全额伤害翻倍** (Skill Double Damage) | 传说~神话 (Q5-Q6) | 该技能的基础与附加伤害直接结算为 2 倍 | 专属传说武器 | 极品掉落 / 套装 | [`WeaponClass.cs:L2074`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2074) | Index 3308，Double |
| **施法后短暂无敌** (Invincible After Use) | 传说~神话 (Q5-Q6) | 施放该技能后进入 `{0}` 秒无敌状态 (0.5~2秒) | 专属防具/饰品 | 极品掉落 / 套装 | [`WeaponClass.cs:L2076`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2076) | Index 3400，WD |
| **施法获得增益** (Cast Gain Buff) | 稀有~神话 (Q2-Q6) | 施法后获得伤害/攻速/移速/同伴属性提升，持续 3~4 秒叠 4~5 层 | 主手/手套/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2086`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2086) | Index 3500~3535 |
| **技能存在常驻增益** (While Skill Exists) | 极品~神话 (Q3-Q6) | 技能处于激活/场上存在期间，玩家伤害/攻速/暴击/减伤大幅增加 | 专属武器/防具 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2100`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2100) | Index 3550~3561 (Has_DMG 等) |

---

### 4. 同伴/召唤物特化词缀（Companion Affixes: WPDT_B, Index 4000 ~ 4417）

| 词缀名称 | 稀有度/档位 | 效果与数值 | 适用部位 | 获取/附加方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **同伴形态蜕变** (Companion Transform) | 史诗~神话 (Q4-Q6) | 将同伴进阶转换为高阶召唤兽（修改外观、攻击模组） | 专属召唤武器 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2236`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2236) | Index 4000，读取 CPC_Data |
| **自动召唤/施法** (Companion Auto Use) | 极品~神话 (Q3-Q6) | 同伴召唤技能冷却就绪时自动召唤/重新召唤 | 头部/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2246`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2246) | Index 4050，AutoUse |
| **召唤上限数量增加** (Summon Count +N) | 稀有~神话 (Q2-Q6) | 该同伴的最大召唤上限数量增加 `+{0}` (1~3只) | 武器/副手/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2248`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2248) | Index 4100，Summon_count_Other |
| **召唤倍率模式** (Summon Count Multi) | 史诗~神话 (Q4-Q6) | 同伴召唤数量模式调整为 `x2 / x3 / x4 / x5 / 固定为1` | 专属召唤装备 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2250`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2250) | Index 4101，Summon_count_Type |
| **同伴弹道增加** (Comp Fire Count +N) | 稀有~神话 (Q2-Q6) | 同伴每次攻击发射弹道数量 `+{0}` | 武器/手套/副手 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2259`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2259) | Index 4200，CT_FS |
| **同伴双重攻击** (Comp Double Attack) | 史诗~神话 (Q4-Q6) | 同伴每次攻击必定连续触发 2 次伤害判定 | 专属武器/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2270`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2270) | Index 4202，AT_Double |
| **同伴死亡鲜血回血** (Comp Blood Die) | 稀有~神话 (Q2-Q6) | 同伴死亡时释放血池回复玩家 `{0}%` 最大生命 | 胸甲/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2274`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2274) | Index 4301，BloodDie |
| **同伴灵魂爆炸** (Comp Soul Explosion) | 极品~神话 (Q3-Q6) | 同伴死亡或消失时自爆造成 `{0}%` 范围元素伤害 | 武器/副手/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2276`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2276) | Index 4302，TGYJ |
| **同伴攻击附加DOT** (Comp Attack Dot) | 稀有~神话 (Q2-Q6) | 同伴攻击命中敌人额外附加 `+{0}` 层对应元素 DOT | 手套/副手/武器 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2278`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2278) | Index 4303，AT_DotLayer |
| **同伴无视伤害减免** (Comp Ignore Anti) | 史诗~神话 (Q4-Q6) | 同伴的所有攻击完全无视目标的减伤与护盾防御 | 专属武器/饰品 | 极品掉落 / 套装 | [`WeaponClass.cs:L2282`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2282) | Index 4305，WS_All |
| **同伴光环范围增加** (Comp Aura Range) | 魔法~神话 (Q1-Q6) | 同伴的光环/影响范围增加 `+{0}%` (15%~50%) | 饰品/防具 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2284`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2284) | Index 4306，Field_Range |
| **同伴击杀玩家回血** (Comp Kill Heal) | 稀有~神话 (Q2-Q6) | 同伴击杀敌人为玩家回复 `{0}` 点生命 | 防具/饰品 | 掉落 / `1 2 SK.csv` | [`WeaponClass.cs:L2286`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2286) | Index 4307，Kill_R_Heal |
| **每只同伴提供玩家属性** (Per Companion Stat) | 极品~神话 (Q3-Q6) | 场上每一个存活同伴：玩家伤害/攻速/移速/生命/暴击/抗性 `+{0}%` | 召唤系专属装备 | 掉落 / 锻造 / 套装 | [`WeaponClass.cs:L2290`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2290) | Index 4400~4417（EveryDMG 等 18 种属性） |

---

### 5. 特殊武器特技词缀（Special SPC Affixes: WPSPC, Index 100 ~ 1199+）

| 词缀名称 | 稀有度/档位 | 效果与数值 | 适用部位 | 获取/附加方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **小精灵·协同** (XJL_XY) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵协同打击，造成伤害与击退 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 100~108，伤害 3~8，移速 1.6~2.5 |
| **小精灵·祝福** (XJL_ZF) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵施加治愈与护盾光环 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 109~117，伤害 3~8 |
| **小精灵·勇气** (XJL_YQ) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵提升全队攻速与暴击 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 118~126，伤害 2~5 |
| **小精灵·自然** (XJL_ZR) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵释放藤蔓束缚与自然毒素 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 127~135，伤害 4~10 |
| **小精灵·暗灭** (XJL_AM) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵施放暗影斩杀与虚弱诅咒 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 136~144，倍率 0.5~1.0 |
| **小精灵·专注** (XJL_ZZ) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵提升冷却缩减与法力恢复 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 145~153，伤害 8~18 |
| **小精灵·神圣** (XJL_SH) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵召唤圣光惩戒与防御护阵 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 172~180，伤害 15~40 |
| **小精灵·心眼** (XJL_XJ) | 魔法~神话 (Q1-Q6) | 攻击触发小精灵标记敌人弱点，攻击必定暴击 | 主手/副手/饰品 | 掉落 70%~80% 随机附带 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 181~189，伤害 5~8，触发 100% |
| **元素法球·环绕** (ORB_Ball_A) | 稀有~神话 (Q2-Q6) | 自动生成 1~8 颗环绕身边的元素法球，接触敌人自爆 | 武器/副手/饰品 | 掉落 / 符文雕刻 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 1000~1017，法球数 1~8，间隔 0.06~0.3s |
| **元素法球·爆裂** (ORB_Ball_EXP_A) | 极品~神话 (Q3-Q6) | 环绕法球碰撞后引发范围二次元素爆炸 | 武器/副手/饰品 | 掉落 / 符文雕刻 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 1050~1057，爆炸半径与伤害增强 |
| **追踪法球·飞弹** (ORB_Ball_B/C/D) | 史诗~神话 (Q4-Q6) | 定期自动凝结追踪飞弹轰击最近目标 | 武器/副手/饰品 | 掉落 / 符文雕刻 / `0 1 SPC.csv` | [`ItemManager.cs:L4914`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L4914) | Index 1058~1154，具备追踪与穿透属性 |

---

### 6. 基底符文与技能等级词缀（Base Runes & WPSkill）

| 词缀类型/标识 | 稀有度/档位 | 效果与数值 | 适用部位 | 获取/附加方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **符文: DMG** | 魔法~神话 (Q1-Q6) | 伤害倍率增加 `+{0}%` (5%~30%) | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2786`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2786) | 对应 player.Damage_Bei |
| **符文: ATS / MVS** | 魔法~神话 (Q1-Q6) | 攻速 / 移速增加 `+{0}%` (5%~25%) | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2789`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2789) | 对应 ATSpeed_Bei / MVSpeed_Bei |
| **符文: BJD / ALLC** | 稀有~神话 (Q2-Q6) | 暴击伤害 `+{0}%` / 全元素穿透 `+{0}%` | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2792`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2792) | 对应 BJDamage / AllChuan |
| **符文: DOT / Anti** | 稀有~神话 (Q2-Q6) | DOT伤害 `+{0}%` / 全抗性 `+{0}%` | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2798`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2798) | 对应 AllDot_DMG / AllAnti |
| **符文: C_DMG / C_Heal** | 稀有~神话 (Q2-Q6) | 同伴伤害 `+{0}%` / 同伴生命 `+{0}%` | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2801`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2801) | 对应 C_Damage / C_Health |
| **符文: ORB_DMG / XJ_DMG** | 极品~神话 (Q3-Q6) | 法球伤害 `+{0}` / 陷阱伤害 `+{0}` | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2825`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2825) | 对应 WPSPC_DMG / XJ_DMG |
| **符文: Drop** | 魔法~神话 (Q1-Q6) | 掉落率提升 `+{0}%` (5%~20%) | 武器/防具/饰品 | 铁匠铺符文镶嵌 / `WPFW_Base` | [`WeaponClass.cs:L2831`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2831) | 对应 ItemDrop_Rate |
| **武器技能点加成** (`WPSK`) | 魔法~神话 (Q1-Q6) | 指定主动天赋技能等级 `+1 ~ +6` (基础 + 品质加成 `WPSK_multi`) | 武器（主手/副手） | 装备生成时依品质附带 | [`WeaponClass.cs:L3090`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L3090) | 最多 6 槽 (SkillA~SkillF) |

---

### 7. 凹槽与宝石属性系统（Sockets & Baoshi Stats）

装备尺寸（`SizeX * SizeY`）决定孔数上限（1~6孔）。镶嵌宝石（`BaoshiClass`）可提供 26 种不同加成，且受装备上 **宝石基础数值加成 (`BS_Add`)** 与 **宝石倍率加成 (`BS_Multi`)** 放大：
$$\text{FinalGemValue} = \lfloor (\text{BaseNumber} + \text{BS\_Add}) \times (1 + \frac{\text{BS\_Multi}}{100}) \rfloor$$

| 宝石类型代码 | 属性类型 | 加成效果 | 代码位置 |
|---|---|---|---|
| `0` / `13` | 生命% / 法力% | `Health_Bei` / `Mana_Bei` 增加 | [`WeaponClass.cs:L2863`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2863) |
| `1` / `5` / `9` / `14` / `18` / `23` | 元素抗性% | 火焰/闪电/剧毒/冰霜/暗影/物理抗性增加 | [`WeaponClass.cs:L2866`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2866) |
| `2` / `6` / `10` / `15` / `19` / `24` | 元素穿透% | 火焰/闪电/剧毒/冰霜/暗影/物理穿透增加 | [`WeaponClass.cs:L2869`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2869) |
| `3` / `7` / `12` / `16` / `21` / `25` | 元素伤害% | 火焰/闪电/剧毒/冰霜/暗影/物理伤害加成增加 | [`WeaponClass.cs:L2872`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2872) |
| `4` | 掉落率% | `ItemDrop_Rate` 增加 | [`WeaponClass.cs:L2875`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2875) |
| `8` / `11` / `17` | 同伴强化 | 同伴生命 `C_Health` / 同伴攻速 `C_ATSpeed` / 同伴伤害 `C_Damage` 增加 | [`WeaponClass.cs:L2887`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2887) |
| `20` / `22` | 速度强化 | 移动速度 `MVSpeed_Bei` / 攻击速度 `ATSpeed_Bei` 增加 | [`WeaponClass.cs:L2923`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2923) |

---

### 8. 套装词缀共鸣系统（Set Bonuses: Set_DT & Lit）

装备带有 `Set_Index` 时，佩戴 2 件、3 件、4 件同一套装时激活 [`Set_DT_Lit`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Set_DT_Lit.cs) 词缀：
- **MainTP = 0**：激活主词缀 `WPDT_A`（如增伤、减伤、全穿透、机制属性）。
- **MainTP = 1**：激活持续伤害词缀 `WPDT_A`（如引爆、层数翻倍、冰冻质变）。
- **MainTP = 2**：激活主动法术词缀 `WPDT_B`（如弹道+N、技能联动、无敌）。
- **MainTP = 3**：激活同伴特化词缀 `WPDT_B`（如召唤倍率、同伴自爆、形态蜕变）。
- **MainTP = 10**：激活专属叠层 Buff [`Buff_PL_Layer`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Buff_PL_Layer.cs)（攻击或击杀触发，每层加成 `Number_Layer`，叠满加成 `Number_Max`，最高 `LayerMax` 层）。

---

## 说明

### 1. 数据来源与核心类清单
- **数据模型类**：
  - [`ItemClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemClass.cs)：物品基类。
  - [`WeaponClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs)：装备与词缀核心承载类，包含所有词缀字符串拼接、差值计算与角色属性注入。
  - [`WPDT_A.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPDT_A.cs) / [`WPDT_B.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPDT_B.cs)：主/DOT 词缀与技能/同伴词缀数据结构。
  - [`WPDT_RandomA.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPDT_RandomA.cs) / [`WPDT_RandomB.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPDT_RandomB.cs)：随机词缀池数组封装。
  - [`WPFW_Base.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPFW_Base.cs) / [`WPSPC.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPSPC.cs) / [`WPSkill.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPSkill.cs) / [`WPAocao.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPAocao.cs)：符文、特技、技能等级、宝石槽数据模型。
  - [`Set_DT.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Set_DT.cs) / [`Set_DT_Lit.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Set_DT_Lit.cs)：套装数据结构。
- **系统逻辑中枢**：
  - [`ItemManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs)：装备生成、掉落几率计算、词缀解析与随机抽取。
  - [`InventoryManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/InventoryManager.cs)：装备栏与背包槽位交互、装备穿脱调度。
  - [`Data.SaveData.SaveDataEquipmentSanitizer.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Data.SaveData/SaveDataEquipmentSanitizer.cs)：装备词缀存档序列化清洗与脱敏。
  - [`UI.UIItems.ItemTipItem.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.UIItems/ItemTipItem.cs)：装备浮窗与词缀高亮显示。
- **配置数据表 (Unity TextAsset)**：
  - `0 0 Weapon.csv` (`WPtext`)：全部基础装备模版与固定词缀配置。
  - `1 0 Main.csv` (`Maintext`)：主属性随机词缀池。
  - `1 1 DOT.csv` (`Dottext`)：DOT 持续伤害随机词缀池。
  - `1 2 SK.csv` (`SKtext`)：主动技能/同伴随机词缀池。
  - `0 1 SPC.csv` (`SPCtext`)：武器独立特技（法球/仙灵/触发投射物）模版库。
  - `0 5 Set.csv` (`Settext`)：套装阶梯效果定义库。
  - 本地化字典：`MainDisplay_FY`（词缀文本模版）、`Main_FY`（属性名词）、`SPC_FY`（特技描述）、`Item_FY`（装备名）。

### 2. 表格列含义
- **词缀名称**：游戏内显示名称及内部英文标识。
- **稀有度/档位**：该词条首次出现或主要分布的装备品质区间（Q0=普通、Q1=魔法、Q2=稀有、Q3=极品、Q4=史诗、Q5=传说、Q6=神话）。
- **效果与数值**：词缀提供的战斗效果、公式逻辑与基准数值范围。
- **适用部位**：该词缀可生成的装备部位（主手、副手、防具4件、饰品4件）。
- **获取/附加方式**：来源链路（怪物/宝箱掉落、商店购买、铁匠铺强化、符文雕刻、套装激活）。
- **代码位置**：反编译源码中负责该词条渲染或逻辑处理的类名与行号。
- **备注**：内部字段名、索引 ID（Index）或特殊机制注意事项。

### 3. 已知的修改与 Modding 注意事项
1. **数值修改联动点**：
   - 若在 `1 0 Main.csv`、`1 1 DOT.csv`、`1 2 SK.csv` 中修改词缀数值，需注意 `GenerateWeaponStatValue` 中存在整型截断（`Mathf.Floor`）与成长乘数，直接修改源浮点数可能在低等级装备上体现不明显。
   - 词缀修改需兼顾 `SaveDataEquipmentSanitizer.cs` 中的脱敏白名单字典（`MainFloatFields`、`MainIntFields`、`DotIntFields` 等）。若新增了未登记的词条 Index，在存档保存/加载时会被当做脏数据剥离！
2. **本地化字典同步**：
   - 词缀渲染文本位于 `MainDisplay_FY.json`，若更改了词缀格式占位符（如 `{0}` 与 `{1}` 的顺序），需确保所有 24 种支持语言的 JSON 均同步修改，否则 `ItemDisplayText` 触发 `FormatException` 将导致装备 Tips 报错。
3. **品质词缀剥离机制**：
   - 测试词缀效果时，若发现掉落的魔法/稀有装备少了一条词缀，是由 `ApplyQualityAttributeRemoval` 的品质剥离机制造成的（剥离时会自动获得白字属性补偿），属于原版机制而非 Bug。

### 4. 未覆盖或存疑项
- **废弃索引**：`WeaponClass.cs` 中存在少数未在 CSV 词缀池中出现的连续保留 Index（如 445~464 之间的部分空档），系官方预留或早期测试残留，不影响正常生成的装备。
- **饰品细分**：饰品分为 4 个槽位（CharType 6~9），虽然共用 `WeaponType = "little"`，但在元素属性判定时，CharType 6 固定为元素抗性，CharType 7/9 固定为元素伤害，CharType 8 固定为元素穿透。
