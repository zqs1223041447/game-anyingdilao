# 词条档位显示设计记录（T几 · 第X名/共N档 · 可roll范围 → V1.31 落地）

> 2026-08-30，V1.31。需求：装备词条行尾追加 ①T 几（暂定上限 T1，越高越好）②占同类词条第 X 名 ③可 roll 范围。关联：`poe-affix-mapping.md`（POE 词缀数据）、`craftbench-metamods-design.md`（工艺台，重骰与本显示共用 roll 模型）。

## 1. 原生系统事实（调研结论）

- **游戏原生无 Tier 体系**。词条显示名走本地化键：`LOC.MM.GetMain(key)`（Main_FY 字典，如 HealthMax/damage/BJrate）+ `DisplayLabel("MainDisplay_FY.*")` + SK/CP 组 `SKStat_*`；行文本全部由 `WeaponClass.cs` 四个方法拼接：`GetMain`（白字+元素行+`AppendMainArrayLines`）、`GetDot`、`GetSK`、`GetCP`。
- **词条数据模型**：`WPDT_A{Index,EL,number}`（Main/DOT）/`WPDT_B{SkillName,Index,GlobleID,EL,number,LinkSK}`（SK/CP）。
- **随机池**：`Item_MB.RateMain/RateDot/RateSK/RateCP` ← `ItemManager.WP_Main/WP_DOT/WP_SK/WP_CP`（CSV `1 0 Main` / `1 1 DOT` / `1 2 SK`，每行=一个池 ID，(Index,EL,NB) 三元组×N；Weapon.csv 行按 ID 引用合并进 RateMain）。**池内同 Index 存在 2~3 条不同 NB**（实测样例：1030:[4,3,2]、2000:[3,2,1]）=天然档位阶梯；`GetRandomWeaponDataA/B` 等概率抽 1 条（同值多条=出现权重）。
- **数值 roll**（`ItemManager.GenerateWeaponStatValue`，L3478）：
  - 秒回（Main 组 Index 3-6，`scaleMainRecoveryValues`）：NB × 1.066^L × (1±0.005) × GivePRC_Base（=1，除 L≥100 秘境走曲线）。
  - 整数成长（302/1500/1910/1911/1912/2000/2101/2202/4303）：Floor(NB)+g，g 按等级/品质/秘境 0~2。
  - 秘境额外整数（80/3100-3103/4100/4200）：非秘境=Floor(NB)；秘境 Q5+ 概率 +1~+3。
  - 浮点（`IsWeaponFloatWholeIndex`/`IsWeaponFloatOneDecimalIndex` 大表）：NB × `GetWeaponStatRandomMultiplier`——非秘境按等级段 [0.9,1.0)/[0.9,1.1)/[1.0,1.1)/[1.0,1.2)/[1.0,1.3)/[1.1,1.3]；秘境按 DropScene 1→[1.2,1.3]、2→[1.2,1.4]、3→[1.3,1.5]、4+→[1.4,1.6]。
  - 其余 Fixed：原值（机制词条，无 roll）。
- **显示时可用上下文**：`ItemClass.Level`（掉落等级已持久化）/`Quality`/`WeaponClass.DropScene`/`MJ_Level`（秘境上下文已持久化）→ 可精确重建 roll 区间，无需新增存档字段。

## 2. 设计决策

| 问题 | 决策 | 依据 |
|---|---|---|
| 档位（T几）来源 | 池内同 Index（B 组同 `SkillName\|Index`）多条 NB = 档位；多档时 **T=排名**（T1 满档最好）；单档浮动词条按可达区间百分位 T1~T5（≥80/60/40/20%）；秒回单档记 T1；超上限标 `T1+` | 原生无 tier，池内 NB 档即最接近 POE tier 的真实结构；"暂定上限 T1"=无 T0 |
| 第X名 | 降序 NB 中的名次（同值去重后）；`n=1` 不显示排名 | 数据驱动、可复现 |
| 可roll范围 | 同 Index 各档 NB 重建可达区间取并集 [min_lo, max_hi]；用物品自身 Level/Quality/DropScene/MJ_Level | 0.9~1.6 倍率下显示基础 NB 会与玩家所见数值脱节 |
| 值→档位匹配 | 值落在哪档可达区间即命中（多档命中取中心最近），全不命中兜底取中心最近 | 秘境倍率/工艺重骰后仍成立；圣石重骰走同款公式 |
| 覆盖面 | Main/DOT/SK/CP 四组行尾注入；套装共鸣行/白字/元素行/SPC 不加（非池内随机词条） | 注入在循环层，不影响 `GetMainArrayLine`/`GetDotArrayLine` 的合成调用点（L2475/L2483 套装行） |
| 文案 | 后缀只追加不改原文，词条名继续走原生本地化；后缀格式 ` T{t} | [{lo}-{hi}]`（2026-08-30 用户定稿 `T1 | [9-26]`；多档时 T 即名次，不再单独显示名次段），T1 金 `#FFD24A`、其余灰 `#8F8F8F` | 颜色/文案常量收敛在文件头（工作规范先例） |
| 无池/无 roll 场景 | 模板反查失败（PoeItemMod 合成装）、池内无同 Index（固定词条）、Fixed 单档、整数成长无增量单档 → 返回空串不标注 | 诚实显示：没有 roll 就没有 tier |

## 3. 实现

- 新文件 `PoedbMod/AffixTierDisplay.cs`（静态类）：`SuffixA(weapon, stat, isDotGroup)` / `SuffixB(weapon, stat, isCompanion)` 两个入口；`GetTemplate` 经 `ItemManager.CraftFindTemplate`（public，GlobalID 匹配）+ 静态缓存；分类表/成长上限/乘数区间**逐字复刻** ItemManager 私有谓词（注释标注源行号，改原生公式需同步）。
- 注入：`WeaponClass.cs` 四处循环行尾追加（Main 组在 `AppendMainArrayLines` 非空守卫内；DOT/SK/CP 空行不追加）。纯显示层零实例字段——typetree 铁律安全。
- 已知边界：秒回 L≥100 秘境的 GivePRC_Base 曲线乘数未复刻（按 1 近似，靠最近档兜底）；秘境掉落历史值按 DropScene 精确、跨品质重骰历史不追踪。

## 4. 验收与调参

- tooltip 悬停背包装备：词条行尾出现 `[T?｜…｜可roll x~y]`，T1 金色；工艺台重骰后档位实时变化。
- 调参入口：`PoedbMod/AffixTierDisplay.cs` 文件头 `EnableDisplay`（总开关）、`ColorTier1/ColorOther`（颜色）、`OverMaxRatio`（T1+ 阈值）、`GetTier` 百分位阈值。

## 5. V1.32 扩展（真机反馈三轮迭代，2026-08-30）

用户真机截图暴露两类问题，四个修订轮次全部走「构建→部署→42s 冒烟→SHA 校验→登记」节拍：

1. **标注覆盖双回退**：①模板固定词条（Index 不在本基底随机池，如陷阱伤害/击杀回蓝类）→ 回退**全局档位梯**（`ItemManager.WP_Main/WP_DOT/WP_SK/WP_CP` 全池行该 Index 的 NB 去重降序，T=全游戏同类名次，静态缓存）；②值落不进本池任何档可达区间（点金/蜕变升品质后词缀仍是低品质档池 roll 的，实例：Q3 血月石榴石 饰品伤害+16% vs Q3 池 [21-26]）→ 回退**品质档家族池并集**（同名基底全职业×全品质模板的池，取能容纳该值的梯）。白字基础属性与彩色元素行仍不标注（非随机池词条）。
2. **穿戴识别诊断行**（`PoeItemMod.TryGetEquipDiagnostics` 挂 `WeaponClass.GetMain`）：`⚠ 未穿戴（在背包中）——右键装备到戒指/项链槽后生效` / `✓ 已穿戴生效` + 装备槽扫描（CharBT 槽数/有货数/槽6/槽7 实际物品与 GlobalID）——用户截图即可定位，不再依赖日志回传。`IsEquipped` 同步去掉 `hasWeapon` 硬性要求。
3. **出手遥测**（`LastCastInfo`，诊断行第二行「上次出手：」）：`SpawnExtraProjectiles` 每个早退分支写入原因（已生成 N 枚追加弹 / 被 BuffTime=X 门控拦截 / 组件不匹配 / gun-dt 为空 / 异常）。
4. **星环无效果的终极根因**（遥测实锤 `[FireBall] 被 BuffTime=1 门控拦截`）：V1.23 遗留的 `dt.BuffTime>0` 门槛——技能样例普遍自带存活计时字段，而 SK_FlyA/Ball/Follow/Sowrd 四族与 `gun.CreatSP()` 均不消费该字段（纯死门槛）。**已移除**。星环问题的完整因果链=三层叠加：钩子在 case 0/2 内（并行会话以总出口修复）+ BuffTime 死门（本轮移除）+ 克隆弹 TargetPos 收拢（本轮环向修正：每枚克隆弹目标点沿各自环向、保持原射程）。克隆弹经 `gun.CreatSP()` 按当前技能样例全量构建（伤害/暴击/穿透/弹速一致，无保真度问题）。

## 6. 工作要点（执行方法论沉淀）

- **截图闭环调试**：把运行时状态（穿戴识别、装备槽扫描、上次出手处理结果）直接渲染进 tooltip 遥测行，用户一张截图=一次完整诊断，替代"回传 Player.log"的多轮往返。
- **单一出口原则**：同一效果（星环环状发射）只保留一条生效通道；并行会话已在 Gun 四攻击函数 switch 后做了总出口挂钩，我方另行试验的 ACTbar Count_F 通道经取证（SK_Fly 族不消费 Count_F + 双通道重复 +4 风险）后撤销。
- **修门先验证门是否该存在**：拦截类门槛（如 BuffTime>0）移除前先 grep 下游消费方——四族与 CreatSP 均不读=死门；有消费方时需评估克隆体行为等价性。
- **打包自检必须脚本断言**：`unzip -p zip install.ps1 | grep expectedHash` 与 `unzip -p zip DLL | sha256sum` 做字符串相等断言（HASH_MATCH_PASS），肉眼比对会漏（二轮包哈希手抄错一位即因此溜过）。
- **版本登记节拍**：每轮修订=重构建（0 error）→部署（SHA256 一致）→42s 冒烟（LOG CLEAN）→重出包（哈希断言）→CHANGELOG 同条目追加修订记录（未真机验收版本同号原地修订，不烧新版本号）。
- **typetree 铁律安全**：本功能全程纯显示层/静态类，零新增实例字段；需要运行时状态的（LastCastInfo）用 static，不进序列化面。
