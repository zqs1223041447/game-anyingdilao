# 铁匠工艺台设计记录（POE metamods 工艺 → ShadowDungeon 原生映射）

> 2026-08-29，V1.24 落地。参考页：https://poedb.tw/cn/metamods 「工艺互动」表（货币 × 变形词缀交互矩阵）。
> 关联文档：`docs/research/poe-fx-fusion-survey.md`（POE 融合全景）、`docs/code-index.md` 已落地修改记录 V1.24 行。

## 1. 参考页原文结构

poedb「工艺互动」页 = **货币（行）× 变形词缀/metamod（列）** 交互矩阵：

- 列（4 个 metamod）：前缀无法被变更 / 后缀无法被变更 / 无法骰出攻击词缀 / 无法骰出法术词缀
- 行（货币）：蜕变石、改造石、增幅石、富豪石、点金石、混沌石、隐匿混沌石、重铸石、崇高石、征服者崇高石、无效石、神圣石、化石/精髓（受锁阻止）、支配/唤醒者崇高石、丰收园艺（增/移/重骰/移前增后/移后增前等）、兽猎工艺（移前增后/移后增前，不保存）、灾魇（玷污崇高/神圣泪珠）、异能（异能混沌/崇高/无效石）
- 交互值：YES=受该锁约束，N/A=不受约束

## 2. 原生游戏映射（设计决策与依据）

| POE 概念 | 游戏原生对应 | 依据 |
|---|---|---|
| 装备稀有度 Normal/Magic/Rare | Quality 0/1/2（+3-6 = 精致/史诗/传说/神话，游戏特色高阶） | `Quality_Group.{Normal,Magic,Rare,Exquisite,Epic,Legendary,Mythical}`，QualityColor 0-6 |
| 词缀 | Main[]/DOT[]（WPDT_A）+ SK[]/CP[]（WPDT_B）+ 元素六行 + SPC 特效 | `WeaponClass` 字段；词缀池 `Item_MB.RateMain/RateDot/RateSK/RateCP/SPC`（指向 `WP_Main/WP_DOT/WP_SK/WP_CP` 字典） |
| 词缀数值随机 | `GenerateWeaponStatValue`（等级/品质/秘境成长公式）+ `GivePRC_Base/PRC/SPC` | 掉落生成 `ItemManager.SetWPdata` 同款，工艺不引入第二套公式 |
| 前缀（输出词条） | Main + DOT + SK + 武器类元素行（伤害/穿透） | 分组按原生数组边界；元素语义按部位切换（武器=伤害、防具/饰品=抗性）已在显示层（GetMain）原生区分 |
| 后缀（功能词条） | CP + 防具/饰品元素行（抗性）+ SPC | 同上 |
| 攻击词缀 / 法术词缀 | 攻击 = Main + SK + 武器元素；法术 = DOT + CP | DOT 表（Index 2000+）全部为异常/持续系（GetDotArrayLine），SK 为命中触发技（GetSK），CP 为同伴系（GetCP） |
| metamod 附加在物品上 | `WeaponClass.Craft_LockPrefix/Suffix/NoAttack/NoCaster` 4 bool，随 WeaponSaveData 持久化 | POE 变形词缀是物品工艺词缀；存档为字段级拷贝，新增字段向后兼容（旧档=false） |
| 锻造台费用 | 每项操作固定 1 金币（`CraftBenchOps.CraftPrice`，`InventoryManager.RemoveMoney`） | 用户要求"暂时先固定 1 金币" |
| 装备品质锁定 | 品质/名字/等级/凹槽/武器技能不被工艺改变 | POE 精神：货币只动词缀；重铸例外（POE 重铸=清空回白） |

## 3. 锁矩阵实现（操作 × 4 锁，对齐 poedb 表）

| 操作 | 前缀锁 | 后缀锁 | 攻击禁骰 | 法术禁骰 | 实现要点 |
|---|---|---|---|---|---|
| 蜕变石（普通→魔法，+1 词缀） | N/A | N/A | N/A | N/A | `Quality=1` + TryAddAffix 全组放开 |
| 增幅石（魔法 +1 词缀） | N/A | N/A | YES | YES | 加词缀时排除 Main/SK（禁攻）或 DOT/CP（禁法）组 |
| 改造石（重骰魔法全部词缀） | YES | YES | N/A | N/A | 锁定组不重骰；其余组按原数量重骰 |
| 富豪石（魔法→稀有 +1 词缀） | N/A | N/A | YES | YES | `Quality=2` + TryAddAffix；失败回退品质 |
| 点金石（普通→稀有，4-6 词缀） | N/A | N/A | YES | YES | `Quality=2` + 循环加词缀至 Random(4,7) 或撞上限 |
| 混沌石（重骰稀有全部词缀） | YES | YES | YES | YES | 禁骰组=清空后不生成（POE：重骰不产出该类词缀） |
| 隐匿混沌石 | YES | YES | N/A | N/A | 与混沌同路径，`ignoreAttackCasterLocks=true` |
| 崇高石（稀有 +1 词缀） | N/A | N/A | YES | YES | 同增幅，品质 ≥2 |
| 无效石（移除 1 条随机词缀） | YES | YES | YES | YES | 候选按标签过滤后随机移除 |
| 神圣石（重骰词缀数值） | YES | YES | N/A | N/A | `CraftRerollStatValues`：按 Index/SkillName 回查模板基础值再走原生成长公式；SPC 重骰 PRC；元素按模板 Element 重分摊；锁定组跳过 |
| 重铸石（清空回普通） | YES(保留) | YES(保留) | N/A | N/A | 锁定组保留、其余清空；有保留则品质不变，否则 `Quality=0`；**清除全部 4 个工艺限制**（POE：变形词缀属工艺词缀，重铸即被移除） |
| 兽猎·移前增后 / 移后增前 | YES | YES | YES | YES | 先按标签移除（POE 语义不保存/无保护提示→本实现保留锁保护），再加对侧词缀；新增方向被锁则整体拒绝执行 |

## 4. 词缀增删重骰核心（CraftBenchOps）

- **TryAddAffix**：候选组 = Main/SK/DOT/CP/SPC 五组（按 allow* 参数放开），随机取序逐组尝试；每组从模板池抽 1 条（`CraftRollEntryA/B`，内部 `ResolveGeneratedWeaponElement` 解析 EL 6=天赋树元素/7=全随机）+ 去重（A 组按 Index，B 组按 `SkillName|Index` 键）；SPC 占槽 0/1（`FreeSpcSlot`）。
- **RerollAffixes**：各组保持原词条数（池不足取可用数），从池重新抽取（等价于重新掉落该组）；禁骰组=清空；SPC 逐激活槽重骰；元素按 `CraftRerollElement`（模板 Element 总量重新分摊，`ApplyElement` 原生逻辑）。
- **TryRemoveAffix**：候选收集（Main/DOT/SK/CP 逐条 + SPC 激活槽 + 元素行），每条打标签（前缀/后缀/攻击/法术位掩码），按锁过滤后随机移除。
- **词缀上限 AffixCap**：普通 0（禁止加词缀类操作）/ 魔法 4 / 稀有·精致 6 / 史诗+ 8（计数=Main+DOT+SK+CP 非空条 + SPC 激活槽 + 非零元素行）。
- **模板池来源 CraftPickPoolTemplate**：同 PLtype（职业）→ 同 CharType（槽位）→ 目标品质档 → WeaponType 精确匹配，两轮放宽（全职业→不筛 WeaponType）；全空回退 `CraftFindTemplate`（装备自身模板）。物品身份不换模板——蜕变/富豪/点金只改 Quality 数值，名字/图标/等级/凹槽原样（与 POE 同为"物品底子不变，稀有度变化"）。

## 5. UI（CraftBenchUI，运行时 uGUI，零资产依赖）

- 挂载：`WeaponManager.OnSingletonAwake` 尾 `CraftBenchUI.Install(this)`（try/catch 隔离，失败不影响锻造台）。
- 开关按钮：克隆锻造面板 Close 按钮（复用原生视觉），anchoredPosition 左移 70px，文案「工艺台」。
- 面板：`CraftBenchPanel` 锚定 MainGroup 同区域（运行时读取其 RectTransform），深色底 + 标题/目标行/金币行 + ScrollRect（RectMask2D viewport + 手动布局 17 行，行高 30+4）+ 图例 + 「返回锻造」按钮；字体 `Resources.GetBuiltinResource<Font>("Arial.ttf")`（2019.4 内置）。
- 目标装备：与三锻造同款选中逻辑（`ContainerGridUtil.GetMainSlot(InventoryManager.MouseSlotDT, InventoryManager.Page)` + `ItemType==0 && weapon!=null`），打开期间**缓存最后悬停**（点按钮时鼠标不在背包上，需粘性目标）；目标行实时显示 装备名（品质色）/品质名/4 锁状态/词缀数。
- 交互安全：打开工艺台时退出锻造三模态（ExitElm/Spc/Enh）+ `InteractionManager.AllInteractToggle=false` + `InventoryManager.ToggleInteract(false)`（与锻造 Enter* 同款，防点击穿透背包）；关闭反向恢复；锻造面板关闭（走远/手动）时 Update 检测 `!Opened` 自动收起并恢复。
- 执行链：手持有物品拦截（`please_take_off_hand_item`）→ 金币校验（`money_not_enough`）→ `ItemCloneUtil.CloneWeapon` 保护（同三锻造）→ Ops 执行 → `RemoveMoney(1)` → `BindWeaponToRegion` → `ShowWPTipA` 弹装备提示 → `RuntimeManager.PlayOneShot(锻造音效)` → ShowTip 反馈。
- 文案：直接中文字符串（`LOC.GetMain` 缺键回退原文并 Warn，项目先例 PoedbSkillInjector/PoeItemMod 同款；不分发，24 语言不做）。

## 6. 已知边界与后续可扩展项

- 攻击/法术按"数组组"粒度而非逐词条 Index 粒度（Main 组内含少量防御词条，如格挡/减伤——禁攻时会连这些一起排除）；逐 Index 精细分型需建 150+ 词条表，当前按组够用且可解释。
- 无效石/兽猎移除时未按词条名展示具体移除对象，只报类别（主属性/持续/技能/同伴/特效/元素词条）。
- 化石/精髓/丰收多段推进/征服者崇高/异能系列未实现（依赖游戏不存在的词缀标签体系）；兽猎两式已覆盖"移前增后/移后增前"。
- 调参入口：`CraftBenchOps.cs` 文件头 `CraftPrice`、`AffixCap`、各 `Exec*`；UI 行文案/顺序在 `CraftBenchUI.RowDefs`。
