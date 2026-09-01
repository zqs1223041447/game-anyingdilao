# 技能标签全表与机制说明（SkillTagSystem 实时推导）

> 产物：`SkillTagSystem.cs` 在 `GameUIManager.ShowSkilltip / RefreshSkilltip` 尾部每次实时重算，零实例字段、异常全降级。标签分两维：`◆元素系`（蓝 `#6FD3FF`）+ `◇行为形态`（橙 `#FFC266`）。
> 写入：2026-08-25；源码真值以 `modwork/decompiled/SkillTagSystem.cs` 为准。

## 一、元素系标签（◆，12 系）

按 Xi CSV 行序 0-11，运行时优先 `LocalizationManager.GetSkill(XiIndexNames[xi])`（`Skill_FY` 中文），失败回退常量：

| Xi | IndexName（代码键） | 中文显示 | 颜色 | 机制含义 |
|---|---|---|---|---|
| 0 | Hell Messenger | 地狱使者 | 蓝 | 该系伤害归类，后续 DOT 关联按此系的 `damageType` 匹配 |
| 1 | Storm Lord | 风暴领主 | 蓝 | 同上 |
| 2 | Arcanist | 奥术师 | 蓝 |  |
| 3 | Blade Master | 剑圣 | 蓝 |  |
| 4 | Holy Light | 圣光 | 蓝 |  |
| 5 | Apocalypse | 天启 | 蓝 |  |
| 6 | Windwalker | 风之游侠 | 蓝 | 弓系等物理远程多在此系 |
| 7 | Doomsday Disciple | 末日信徒 | 蓝 |  |
| 8 | High Elf | 高阶精灵 | 蓝 |  |
| 9 | Undead Emissary | 亡灵使者 | 蓝 |  |
| 10 | Void Sorcerer | 虚空咒师 | 蓝 |  |
| 11 | Corrupt Priest | 腐化祭司 | 蓝 |  |

> 隐含：元素系决定 `damageType`（fire/frozen/thunder/poison/physics/shadow），影响 DOT 挂载与抗性计算；装备/天赋按 `damageType` 匹配生效（如火系点燃）。

## 二、行为形态标签（◇，按技能类型分流）

### 2.1 主技能 type0（Sample_F）—— 完整规则表

列表按代码求值顺序，去重后以 `·` 连接。标注字段来源与运行时语义：

| 标签 | 触发条件（字段） | 数值来源 | 机制含义与隐含行为 |
|---|---|---|---|
| **直射** | `FStype` ∈ {0,1,2,4,5,6} 或 `FStype==3` 且 weaponFamily!=0 | `value.FStype` | 最常见弹道：`Gun.MGC/SQS/ARC/DEADattack` 按 `FStype` 分支，朝鼠标方向直线发射。碰撞走 `SK_FlyA` 直线移动，命中即结算。 |
| **位移** | `FStype==3` 且 `weaponFamily==0`（即 `xi/3==0` 的 MGC 家族） | `value.FStype` + `xi/3` | 特殊分支：MGC 的 3 号弹道会触发 `TeleportRoutine`，角色瞬移到鼠标落点而非发射投射物。其他家族的 3 仍是直射。 |
| **环绕** | `FStype` ∈ {7,8,9} | `value.FStype` | `SetParent` 挂载到玩家本体/腰/头顶，随角色移动而转圈。无飞行碰撞，持续范围伤害。 |
| **落点** | `FStype==10` | `value.FStype` | `Raycast` 放置到鼠标落点（如陷阱/图腾），不经飞行直接生成。 |
| **穿透** | `AllChuan_F==0` | `value.AllChuan_F`（0=穿透，1=阻挡） | 弹体碰撞后不销毁，`OnTriggerEnter2D` 不调 `Stop`/`StartReturn`，可贯穿多个敌人。配合 `colEXP` 决定穿透中是否每次爆炸。 |
| **命中爆炸** | `colEXP==0` | `value.colEXP` | 碰撞瞬间生成 `EXP` 爆炸范围（`Range_BD/TypeEXP_BD` 等），即使穿透弹也会每次命中炸一次。`colEXP==1` 则仅飞行不炸。 |
| **末段爆裂** | `LastEXP==0` | `value.LastEXP` | 弹体生命周期结束（超时/到距）时再炸一次。两者叠加可实现“命中炸+末段炸”双爆。 |
| **多弹** | `CountMulti>1` 或 `Count_F_Last>1` 或 `Count_S_Last>1` 或 `Count_ORB>1` | `_Last` 访问器（含天赋加成）→ 实时变化 | 单次施法生成多枚投射物（如环形 8 箭就是 `CountMulti` 的极端）。每枚独立 `SkillOBJ_DT_SP` 与碰撞判定，伤害线性叠加。加天赋后此标签会动态出现。 |
| **追踪** | `Follow_F==0` | `value.Follow_F` | 弹体每帧 `FollowMV` 朝最近敌人转向追踪，而非直线。会覆盖部分 `StraightReturnMV` 行为。 |
| **减速** | `MoveSpeedCut_Last > 0` | `_Last` 访问器（含天赋） | 命中后给 `Buff_Enemy(type=0)` 写 `MoveSpeedCut/DebuffTime`，经 `BuffMG_EM` 持续减速。数值为百分比，实时随天赋提升。 |
| **灼烧 / 冻伤 / 感电 / 中毒 / 流血 / 侵蚀** | 同系 `Dot_F` 有任意 `Level_Base>0` 且 `damageType` 与本技能一致 | 运行时 `TalentManager.XiData[xi].Dot_F` 遍历 | 元素级开关：点出该系 DOT 后，同系同元素的所有主技能自动附带 DOT。命中后走 `SetDot → DotEM.AddDot` 挂持续伤害。技能 tooltip 上的 DOT 词是“元素级共享”的忠实描述。 |
| **回旋** | `skillName` ∈ 白名单 `{Razor Arrow, ArcBoomerang}` | `BoomerangWhitelist` 常量 | 纯 mod 行为（`SK_FlyA.ReturnToPlayer` 运行时回旋），CSV 无法推导，故登记制。命中/超时双路径触发 `StartReturn()`，返回途中关碰撞防二次伤害。新加回旋技能在此登记即可显示标签。 |

> **隐含机制举例**：`直射` 隐含“有飞行碰撞与弹速 `FlySpeed`”；`多弹` 隐含“每弹独立计算穿透/爆炸/追踪”；`命中爆炸` + `穿透` 组合 = 每穿一个怪炸一次 AoE；`末段爆裂` + `穿透` = 穿完全程后在终点补 AoE；`减速` 与 `灼烧` 可叠加。

### 2.2 同伴 type2（Comp_F）

| 标签 | 条件 | 含义 |
|---|---|---|
| **召唤** | 固有 | 该技能本质是召唤同伴，非投射物 |
| **多弹** | `Summon_count_Base>1` 或 `Count_A/B>1` 或 `CountMulti_A/B>1` | 同伴的攻击为多段/多召唤物 |
| **穿透** | `AllChuan_A==0` 或 `AllChuan_B==0` | 同伴的弹幕穿透 |
| **追踪** | `Follow_A==0` 或 `Follow_B==0` | 同伴弹幕追踪 |
| **命中爆炸** | `colEXP_A==0` 或 `colEXP_B==0` | 同伴命中爆炸 |
| **灼烧等 DOT** | 同 2.1 的 DOT 关联规则 | 同伴伤害同样可挂 DOT |

### 2.3 DOT 本体 type4（Dot_F）

| 标签 | 条件 | 含义 |
|---|---|---|
| **灼烧/冻伤/感电/中毒/流血/侵蚀** | 固有（按自身 `damageType`） | 该 DOT 本体的元素名 |
| **减速** | `MVSpeedCut_Last>0` 或 `ATSpeedCut_Last>0` | DOT 附带的移速/攻速削减 |

### 2.4 子节点/倍率 type1/3/5/6

当前 `SkillTagSystem` 仅对 0/2/4 产形态标签；子节点（增幅）与倍率节点只显示**元素系◆标签**，不重复形态。这样避免“增幅节点本身无弹道却显示直射”的误导。如需为子节点追加“增幅”词，可通过 `RegisterTagContributor` 扩展。

## 三、实时性与装备扩展

- **实时路径**：`SkillBT.OnPointerEnter → ShowSkilltip`（打开即算）+ `TalentManager.AddPoint → RefreshSkilltip`（加点即刷），两路汇聚同一 `ApplyToSkillTip`。
- **数值实时**：`多弹/减速` 等走 `_Last` 访问器（`XXX_Last = XXX_Base + TalentManager.GetXXX()`），加点后无需重启立即反映。
- **装备化二期**：已预留 `SkillTagSystem.RegisterTagContributor(dt => IEnumerable<string>)`，装备系统注册委托后，每次 Show 时实时回调追加标签（如“装备·吸血”），链路已打通当前留空。

## 四、快速查表（用户最关心的 6 个）

| 你问的词 | 一句话 | 隐含机制 |
|---|---|---|
| **多弹** | 单次施法 N 枚独立弹体 | 每弹独立碰撞/爆炸/追踪；伤害 N 倍；受 `CountMulti/Count_F_Last` 天赋实时影响 |
| **直射** | 朝鼠标直线飞行 | 有弹速、飞行碰撞、受 `FlySpeed` 加成 |
| **穿透** | 碰人不消失可穿多个目标 | 穿透弹若带 `命中爆炸` 则每穿一个炸一次 |
| **命中爆炸** | 碰撞瞬间 AoE | 范围由 `Range_BD` 等决定，无视穿透与否 |
| **追踪** | 弹体转向追最近敌人 | 每帧寻敌，会与回旋/直线返回叠加计算 |
| **回旋** | mod 回旋镖行为 | 命中/超时后返回玩家，途中无碰撞；白名单登记制 |

> 校验：本表与 `SkillTagSystem.cs:221-389` 的 3 个 `Collect*Tags` 方法逐行一致；Xi 映射与 `modwork/asset-inventory/desc-sync/baseline/Skill_FY-full.txt` 12 键 ChineseS 值一致。
