# 效果库（Effects Library）

> 「添加技能效果」的实现模式分级。Tier 1 = 纯 CSV 列覆盖（SkillForge 全自动）；Tier 2 = 代码行为补丁（需开发一次入库复用）。
> 列名基于 SampleF 表头（152 列）；POE 注入器三技能已实证其中多列效果。

## Tier 1：CSV 列覆盖（全自动）

| 效果 | 关键列 | 实证来源 |
|---|---|---|
| 命中即爆（末段消失+爆炸） | `colEXP→0` + `EXP_*` 系列（Range_BD/TypeEXP_BD 等） | POE_Fireball |
| 全穿透 | `AllChuan_F→0`（或 ThroughType 族） | POE_IceSpear |
| 弹道减速 debuff | `MoveSpeedCut`（数值=减速量）+ `DebuffTime`（持续秒） | POE_IceSpear（30/2） |
| 末段 AoE | `LastEXP→0` | POE_LightningArrow |
| 多弹/分裂 | `Count`、`multiCount_Type`、`SonA/SonB/SonC`（子体 IndexName） | 表头结构 |
| DOT 附着 | DotF/DotS 表独立加行 + 本行 `damageType` 对齐；`Layer_Base/DOTrate_Base/DOTrate_Level/Time_base` | DotF 表头 |
| 伤害元素 | `damageType`（0=fire…5=shadow，对应 MainEL 特效变体） | GiveElement 映射 |
| 数值调整 | `Damage_Base/Damage_Level`、`ManaCost_Base`、`CoolDown_Base`、`FlySpeed_Base`、`Distance` | 通用 |
| 攻速/移速削减弹 | `ATSpeedCut_*` / `MVSpeedCut_*` | DotF 表头 |
| 爆炸跳弹 | `BoomDie_OBJ/Pos`、`BoomJump_*`、`CutJump_OBJ/Pos` | DotF 表头 |

> 使用方式：全部走 skill-spec 的 `columnOverrides`，无需改代码。新列语义确认方法：查 `docs/code-index.md` 技能系统速查 → 读 ACT_skillSample.cs 同名字段消费点。

## Tier 2：代码行为补丁（模式库）

| 模式 | 参考实现 | 要点 |
|---|---|---|
| 投射物返回（回旋镖） | `SK_FlyA.cs` ReturnToPlayer/StartReturn | ⚠️ 新增字段必须 `[NonSerialized]`（KI-001 教训：public 序列化字段改变 typetree 布局→原生崩溃）；命中/超时双路径触发；返回期关碰撞防重复伤害 |
| 运行时技能注入 | `PoeSkillInjector.cs` + `BootstrapEntry.cs` | RuntimeInitializeOnLoadMethod 引导 + 轮询注入；克隆现有 SkillXiData 行改列后注册字典；harness 断言护航 |
| 天赋树 UI 按钮 | `TalentManager.TryCloneArcBoomerangButton` | Start 尾部 Clone 现有 SkillBT→改 IndexName/Xi→靠原生 OnEnable/Start 注册（勿手动 AddListener 防双挂） |
| 场景加载安全 | 全部 Tier 2 | 禁止在场景加载期做重资产操作；防御式 try/catch + LogUtil 打点 |

## 新效果接入流程（Tier 2）

1. 会话内描述行为 → 定位宿主类（投射物行为=SK_* 家族，见 root-scripts.md）
2. 实现：优先参数化既有模式；新增 public 字段一律 `[NonSerialized]`
3. 构建 0 error → 部署 → 真实机器验收 → 模式入库本表
