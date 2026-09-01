# 《暗影地牢》(Shadow Dungeon) 装备锻造与强化效果体系全景速查

《暗影地牢》中的装备锻造与强化体系是一个由**武器三锻造面板（WeaponManager）**、**宝石与符文加工中枢（BaoshiManager）**、**特殊道具即时赋能（WeaponBaoshiApplyUtil）** 以及 **底层模板再生（ItemManager）** 深度协同的多元装备养成架构。玩家在游戏交互中可通过 NPC 锻造台呼出武器三锻造面板（包含**元素重铸 Elm**、**特技重铸 Spc**、**基础强化 Enh** 三大核心模式），亦可在背包或宝石面板中使用**魔法功能石（Stones）**、**元素/属性精华（Essences）**、**职业技能/特技/词条符文（Runes）** 与 **六色八阶宝石（Gems）**。整个锻造链条的产出涵盖了基础三维（伤害/生命/法力）指数级成长、元素属性重组分布、特技元素流派转属、凹槽打孔、数值双倍跃升（翻倍石）以及装备等级按玩家当前等级重铸升级（复生石）。

---

## 1. 核心锻造与强化效果速查全表

| 效果名称 | 效果与数值 | 消耗/材料 | 成功率/等级阶段 | 适用装备 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| **元素重铸**<br>(Element Rebuild / Elm) | 重新随机分配装备上的 6 大元素数值（火、冰、雷、毒、物、暗）。总元素值严格守恒，随机打乱类型；按概率触发 4 种分配模式：<br>• 保持分布（50%）：数值切片打乱重排<br>• 平均分配（20%）：总值均分给各元素<br>• 小幅暴击（20%）：单元素额外 $+15\%$ 总值，其余扣减<br>• 大幅暴击（10%）：单元素额外 $+30\%$ 总值，其余扣减 | 金币（消耗公式见说明）<br>• 基准：300 金币<br>• 随等级指数：$1.065^{\text{Level}}$<br>• 随次数指数：$1.07^{\text{Reb\_Count}}$<br>• 受玩家天赋 `QH_Price` 折扣 | 100% 成功<br>单件装备默认上限 1000 次<br>（受天赋 `Reforge_Inc` 提升） | 具有任意元素属性（>0）的所有武器 | `UI.Panels.WeaponManager`<br>`TryRandomElm`<br>([WeaponManager.cs:424-463](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Panels/WeaponManager.cs#L424-L463)) | 必须空手且在背包选中武器；计数器 `Reb_CountMax` 与特技重铸共享。 |
| **特技重铸 / 转属**<br>(Special Effect Rebuild / Spc) | 重新随机改变武器主特技及副特技的元素属性类型（火0、冰1、雷2、毒3、物4、暗5）。100% 变换为与当前元素不同的其余 5 种元素之一（各 20% 均等概率），同步刷新所有有效特技槽位的元素属性。 | 金币（与元素重铸相同公式）<br>• 基准 300 金币<br>• 随等级与重铸次数增长<br>• 受天赋 `QH_Price` 折扣 | 100% 成功<br>与元素重铸共享 1000 次上限<br>（受天赋 `Reforge_Inc` 提升） | 拥有至少 1 个特技词条（`HasSPC(0)`）的武器 | `UI.Panels.WeaponManager`<br>`TryRandomSpc`<br>([WeaponManager.cs:922-972](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Panels/WeaponManager.cs#L922-L972)) | 同步修改武器所有激活特技的 `spc2.EL`，实现流派元素无损转换。 |
| **基础强化 / 数值淬炼**<br>(Base Stat Enhance / Enh) | 随机提升武器固有三维基础属性（伤害 `Damage`、生命 `Health`、法力 `Mana`）。<br>• 单次随机提升 $[1.0\%, 2.5\%]$<br>• 享受玩家天赋倍率加成：$\times (1 + \frac{\text{QH\_Bei}}{100})$<br>• 单次提升至少保底 $+1$ 点<br>• 武器售价相应累加提升属性总和 | 金币（消耗公式见说明）<br>• 基准：300 金币<br>• 随等级指数：$1.065^{\text{Level}}$<br>• 随强化次数指数：$1.08^{\text{ZQ\_Count}}$<br>• 受玩家天赋 `QH_Price` 折扣 | 100% 成功<br>按装备品质划分强化上限：<br>• 普通(0)/优秀(1): 5 次<br>• 稀有(2)/史诗(3): 10 次<br>• 传说(4)/神话(5): 15 次<br>• 暗金特殊(6): 20 次<br>（受天赋 `QH_Inc` 额外提升） | 具有伤害/生命/法力基础属性（>0）的所有武器 | `UI.Panels.WeaponManager`<br>`TryRandomEnh`<br>([WeaponManager.cs:1093-1155](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Panels/WeaponManager.cs#L1093-L1155)) | 计数器为 `ZQ_CountMax`；强化直接改变基础数值并支持后续乘法放大。 |
| **凿孔石**<br>(Chiseling Stone / `Stone_KZ`) | 为装备开辟 1 个新的宝石凹槽（`AocaoCount++`），使未激活的凹槽变为可用空槽。 | 消耗 1 个【凿孔石】<br>(Item ID: 50061) | 100% 成功<br>上限为装备自身设定的最大凹槽数 `MaxAocaoCount` | 武器与防具<br>(CharType 0 ~ 5) | `WeaponBaoshiApplyUtil`<br>`TryAddSocket`<br>([WeaponBaoshiApplyUtil.cs:408-426](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L408-L426)) | 达到最大凹槽数后无法再次使用；成功后即时刷新凹槽 UI。 |
| **复生石**<br>(Ascension Stone / `Stone_FS`) | 将旧装备的基础模板等级重置提升至玩家当前等级（`PlayerManager.Level`），并依据装备初始模板重新随机生成全部属性、元素分布与词条。 | 消耗 1 个【复生石】<br>(Item ID: 50062) | 100% 成功<br>限制：装备不得含有已镶嵌宝石、特技符文、属性符文或技能符文加点 | 尚未进行深度镶嵌改造的基础装备 | `ItemManager`<br>`TryRegenerateWeaponFromTemplate`<br>([ItemManager.cs:1156-1170](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L1156-L1170)) | 低级极品底子跨等级升级的神器道具；已深度镶嵌加工的装备无法使用。 |
| **幻化石**<br>(Transmutation Stone / `Stone_HH`) | 提升装备特技伤害倍率 `SPC_DMG_Bei` $+5\%$。多件装备累加可极大增强特技技能输出。 | 消耗 1 个【幻化石】<br>(Item ID: 50063) | 100% 成功<br>单件装备默认上限 10 次<br>（受天赋 `HH_Inc` 提升） | 全部位装备<br>(武器、防具、饰品) | `WeaponBaoshiApplyUtil`<br>`TryAddTransmutation`<br>([WeaponBaoshiApplyUtil.cs:443-458](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L443-L458)) | 计数器为 `HHCount`；满 10 次可提供高达 $+50\%$ 的特技独立伤害乘区加成。 |
| **奥秘石**<br>(Arcane Stone / `Stone_AM`) | 装备可镶嵌技能符文数量上限 $+1$（`SkillFW_CountMax++`）。 | 消耗 1 个【奥秘石】<br>(Item ID: 50064) | 100% 成功<br>单件装备上限 6 个技能符文槽 | 全部位装备<br>(武器、防具、饰品) | `WeaponBaoshiApplyUtil`<br>`TryApplyStone`<br>([WeaponBaoshiApplyUtil.cs:306-308](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L306-L308)) | 突破技能符文镶嵌容量限制的核心道具。 |
| **毁灭石**<br>(Annihilation Stone / `Stone_HM`) | 武器基础数值翻倍（`BaseValueDoubled = true`, `BaseValueMultiplier = 2.0`），基础伤害/生命/法力直接提升 100%。 | 消耗 1 个【毁灭石】<br>(Item ID: 50065) | 100% 成功<br>每件武器终身仅限生效 1 次 | 仅限武器<br>(CharType 0=主手, 1=副手) | `WeaponClass`<br>`TryApplyBaseValueDouble`<br>([WeaponClass.cs:394-404](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L394-L404)) | 武器专属翻倍石，直接乘以 2 倍结算基础三维属性。 |
| **崇高石**<br>(Exalted Stone / `Stone_CG`) | 防具基础数值翻倍（`BaseValueDoubled = true`, `BaseValueMultiplier = 2.0`），基础生命/法力等直接提升 100%。 | 消耗 1 个【崇高石】<br>(Item ID: 50066) | 100% 成功<br>每件防具终身仅限生效 1 次 | 仅限防具<br>(CharType 2=头, 3=身, 4=手, 5=腿) | `WeaponBaoshiApplyUtil`<br>`TryApplyBaseValueDouble`<br>([WeaponBaoshiApplyUtil.cs:280-285](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L280-L285)) | 防具专属翻倍石。 |
| **棱彩石**<br>(Prismatic Stone / `Stone_LC`) | 饰品基础数值翻倍（`BaseValueDoubled = true`, `BaseValueMultiplier = 2.0`），基础三维直接提升 100%。 | 消耗 1 个【棱彩石】<br>(Item ID: 50067) | 100% 成功<br>每件饰品终身仅限生效 1 次 | 仅限饰品<br>(CharType 6=盾/副手, 7=项链, 8=副手, 9=戒指) | `WeaponBaoshiApplyUtil`<br>`TryApplyBaseValueDouble`<br>([WeaponBaoshiApplyUtil.cs:294-299](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L294-L299)) | 饰品专属翻倍石。 |
| **火胆精华**<br>(Ember Essence / `JHEL0`) | 注入固定火元素数值（`Fire += Value`）：<br>• 主手武器：$+4$ 点<br>• 饰品（项链/戒指）：$+3$ 点<br>• 防具与副手：$+1$ 点 | 消耗 1 个【火胆精华】<br>(Item ID: 50049) | 100% 成功<br>单件装备元素精华累计上限 12 次<br>（`JHEL_Count < 12`） | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:214-232](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)) | 不占用普通词条槽位，直接提升面板元素数值；所有元素精华共享 12 次上限。 |
| **冰霜精华**<br>(Frost Essence / `JHEL1`) | 注入固定冰霜元素数值（`Frozen += Value`）：<br>• 主手武器：$+4$ 点<br>• 饰品：$+3$ 点<br>• 防具与副手：$+1$ 点 | 消耗 1 个【冰霜精华】<br>(Item ID: 50050) | 100% 成功<br>单件装备元素精华累计上限 12 次 | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:214-232](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)) | 同上。 |
| **雷电精华**<br>(Storm Essence / `JHEL2`) | 注入固定雷电元素数值（`Thunder += Value`）：<br>• 主手武器：$+4$ 点<br>• 饰品：$+3$ 点<br>• 防具与副手：$+1$ 点 | 消耗 1 个【雷电精华】<br>(Item ID: 50051) | 100% 成功<br>单件装备元素精华累计上限 12 次 | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:214-232](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)) | 同上。 |
| **毒胆精华**<br>(Venom Essence / `JHEL3`) | 注入固定剧毒元素数值（`Poison += Value`）：<br>• 主手武器：$+4$ 点<br>• 饰品：$+3$ 点<br>• 防具与副手：$+1$ 点 | 消耗 1 个【毒胆精华】<br>(Item ID: 50052) | 100% 成功<br>单件装备元素精华累计上限 12 次 | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:214-232](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)) | 同上。 |
| **穿刺精华**<br>(Piercing Essence / `JHEL4`) | 注入固定物理元素数值（`Physics += Value`）：<br>• 主手武器：$+4$ 点<br>• 饰品：$+3$ 点<br>• 防具与副手：$+1$ 点 | 消耗 1 个【穿刺精华】<br>(Item ID: 50053) | 100% 成功<br>单件装备元素精华累计上限 12 次 | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:214-232](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)) | 同上。 |
| **诅咒精华**<br>(Curse Essence / `JHEL5`) | 注入固定暗影元素数值（`Shadow += Value`）：<br>• 主手武器：$+4$ 点<br>• 饰品：$+3$ 点<br>• 防具与副手：$+1$ 点 | 消耗 1 个【诅咒精华】<br>(Item ID: 50054) | 100% 成功<br>单件装备元素精华累计上限 12 次 | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:214-232](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)) | 同上。 |
| **伤害精华**<br>(Damage Essence / `JH_damage`) | 注入武器词条 Index=10（伤害百分比），数值 $+3\%$。 | 消耗 1 个【伤害精华】<br>(Item ID: 50055) | 100% 成功<br>单件普通精华累计上限 8 次<br>（`JH_Count < 8`） | 仅限武器<br>(CharType 0/1) | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:240-242](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L240-L242)) | 若武器已有该词条则直接累加，无则在 `Main` 数组扩容新增。所有属性精华共享 8 次上限。 |
| **迅捷精华**<br>(Swiftness Essence / `JH_ats`) | 注入武器词条 Index=11（攻击速度百分比），数值 $+3\%$。 | 消耗 1 个【迅捷精华】<br>(Item ID: 50056) | 100% 成功<br>单件普通精华累计上限 8 次 | 仅限武器<br>(CharType 0/1) | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:249-251](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L249-L251)) | 同上。 |
| **狂魔精华**<br>(Demonic Essence / `JH_CPdamage`) | 注入武器词条 Index=101（同伴伤害百分比），数值 $+5\%$。 | 消耗 1 个【狂魔精华】<br>(Item ID: 50057) | 100% 成功<br>单件普通精华累计上限 8 次 | 仅限武器<br>(CharType 0/1) | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:252-254](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L252-L254)) | 同上。 |
| **生命精华**<br>(Vitality Essence / `JH_heal`) | 注入防具词条 Index=1（生命上限百分比），数值 $+3\%$。 | 消耗 1 个【生命精华】<br>(Item ID: 50058) | 100% 成功<br>单件普通精华累计上限 8 次 | 仅限防具<br>(CharType 2~5) | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:243-245](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L243-L245)) | 同上。 |
| **智慧精华**<br>(Wisdom Essence / `JH_mana`) | 注入防具词条 Index=2（法力上限百分比），数值 $+3\%$。 | 消耗 1 个【智慧精华】<br>(Item ID: 50059) | 100% 成功<br>单件普通精华累计上限 8 次 | 仅限防具<br>(CharType 2~5) | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:246-248](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L246-L248)) | 同上。 |
| **泰坦精华**<br>(Golem Essence / `JH_CPheal`) | 注入防具词条 Index=100（同伴生命上限百分比），数值 $+5\%$。 | 消耗 1 个【泰坦精华】<br>(Item ID: 50060) | 100% 成功<br>单件普通精华累计上限 8 次 | 仅限防具<br>(CharType 2~5) | `WeaponBaoshiApplyUtil`<br>`TryApplyEssence`<br>([WeaponBaoshiApplyUtil.cs:255-257](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L255-L257)) | 同上。 |
| **红宝石镶嵌**<br>(Ruby Sockets / 8阶) | 依部位提供属性（数值 $N = 1\% \sim 10\%$）：<br>• 武器：火焰伤害 $+N\%$<br>• 头部/腿部：生命上限 $+N\%$<br>• 衣服：火焰抗性 $+N\%$<br>• 手部：火焰穿透 $+N\%$ | 消耗对应品质红宝石 1 颗<br>(Item ID: 50001 ~ 50008) | 100% 成功<br>装备需有未镶嵌凹槽 | 全部位已开槽装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySocketedGem`<br>([WeaponBaoshiApplyUtil.cs:31-69](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L31-L69)) | 阶位：裂开(1%)、标准(2%)、精致(3%)、卓越(4%)、无瑕(5%)、完美(6%)、史诗(8%)、传奇(10%)。实际数值受天赋 `BS_Add` 与 `BS_Multi` 增幅。 |
| **黄宝石镶嵌**<br>(Topaz Sockets / 8阶) | 依部位提供属性（数值 $N = 1\% \sim 10\%$）：<br>• 武器：雷电伤害 $+N\%$<br>• 头部/腿部：掉落率 $+N\%$<br>• 衣服：雷电抗性 $+N\%$<br>• 手部：雷电穿透 $+N\%$ | 消耗对应品质黄宝石 1 颗<br>(Item ID: 50017 ~ 50024) | 100% 成功<br>装备需有未镶嵌凹槽 | 全部位已开槽装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySocketedGem`<br>([WeaponBaoshiApplyUtil.cs:31-69](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L31-L69)) | 阶位同上。头部与腿部提供宝贵掉落率加成。 |
| **绿宝石镶嵌**<br>(Emerald Sockets / 8阶) | 依部位提供属性（数值 $N = 1\% \sim 10\%$）：<br>• 武器：剧毒伤害 $+N\%$<br>• 头部：同伴生命上限 $+N\%$<br>• 衣服：剧毒抗性 $+N\%$<br>• 手部：剧毒穿透 $+N\%$<br>• 腿部：同伴攻击速度 $+N\%$ | 消耗对应品质绿宝石 1 颗<br>(Item ID: 50025 ~ 50032) | 100% 成功<br>装备需有未镶嵌凹槽 | 全部位已开槽装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySocketedGem`<br>([WeaponBaoshiApplyUtil.cs:31-69](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L31-L69)) | 阶位同上。专注于同伴生存与攻速增益。 |
| **蓝宝石镶嵌**<br>(Sapphire Sockets / 8阶) | 依部位提供属性（数值 $N = 1\% \sim 10\%$）：<br>• 武器：冰霜伤害 $+N\%$<br>• 头部/腿部：法力上限 $+N\%$<br>• 衣服：冰霜抗性 $+N\%$<br>• 手部：冰霜穿透 $+N\%$ | 消耗对应品质蓝宝石 1 颗<br>(Item ID: 50009 ~ 50016) | 100% 成功<br>装备需有未镶嵌凹槽 | 全部位已开槽装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySocketedGem`<br>([WeaponBaoshiApplyUtil.cs:31-69](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L31-L69)) | 阶位同上。 |
| **紫宝石镶嵌**<br>(Amethyst Sockets / 8阶) | 依部位提供属性（数值 $N = 1\% \sim 10\%$）：<br>• 武器：暗影伤害 $+N\%$<br>• 头部：同伴伤害 $+N\%$<br>• 衣服：暗影抗性 $+N\%$<br>• 手部：暗影穿透 $+N\%$<br>• 腿部：移动速度 $+N\%$ | 消耗对应品质紫宝石 1 颗<br>(Item ID: 50041 ~ 50048) | 100% 成功<br>装备需有未镶嵌凹槽 | 全部位已开槽装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySocketedGem`<br>([WeaponBaoshiApplyUtil.cs:31-69](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L31-L69)) | 阶位同上。腿部提供移速加成，头部提供同伴输出。 |
| **白钻石镶嵌**<br>(Diamond Sockets / 8阶) | 依部位提供属性（数值 $N = 1\% \sim 10\%$）：<br>• 武器：物理伤害 $+N\%$<br>• 头部/腿部：攻击速度 $+N\%$<br>• 衣服：物理抗性 $+N\%$<br>• 手部：物理穿透 $+N\%$ | 消耗对应品质白钻石 1 颗<br>(Item ID: 50033 ~ 50040) | 100% 成功<br>装备需有未镶嵌凹槽 | 全部位已开槽装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySocketedGem`<br>([WeaponBaoshiApplyUtil.cs:31-69](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L31-L69)) | 阶位同上。头部与腿部提供全职业核心攻速。 |
| **技能符文镶嵌**<br>(Talent Skill Rune / `FW_SK`) | 赋予装备指定天赋技能等级 $+1$（`wPSkill.Number2++`），直接突破角色技能点上限。 | 消耗对应技能名称符文 1 块<br>(Item ID: 51000) | 100% 成功<br>已镶嵌符文数小于 `SkillFW_CountMax` | 全部位装备 | `WeaponBaoshiApplyUtil`<br>`TryApplySkillRune`<br>([WeaponBaoshiApplyUtil.cs:313-354](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L313-L354)) | 同技能可多次累加等级；槽位不足时可使用奥秘石扩充。 |
| **特技符文镶嵌**<br>(Equipment Skill Rune / `FW_SPC`) | 将指定特技效果（`SPC_MB`）写入装备的第 2 特技槽位（`SPC[1]`），赋予装备额外特效。 | 消耗对应特技符文 1 块<br>(Item ID: 51001) | 100% 成功<br>单件装备仅限 1 个特技符文槽位；必须符合部位与职业限制 | 职业与部位匹配的装备<br>(FWtype 0=武器, 1=头身, 2=手足, 3=副手, 4=饰品) | `WeaponBaoshiApplyUtil`<br>`TryApplySPCRune`<br>([WeaponBaoshiApplyUtil.cs:356-385](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L356-L385)) | 写入包含特技模板索引 `Index`、元素属性 `EL` 与伤害倍率 `PRC` 的完整特效定义。 |
| **基础属性符文**<br>(Attribute Runes / `FW_Base`)<br>*16类共64种条目* | 写入装备基础符文词条 `FW_Base`，赋予高额独立属性加成：<br>• 武器类：伤害(20%~100%)、攻速(30%~150%)、暴击点(+10~+50)、全穿透(10%~40%)、持续伤害(50%~200%)、同伴伤害(30%~150%)、同伴攻速(30%~150%)<br>• 防具类：生命(10%~60%)、法力(10%~60%)、全抗(5%~20%)、移速(5%~20%)、同伴生命(15%~80%)、同伴全抗(5%~20%)<br>• 饰品类：法球伤害(15%~60%)、特殊伤害(15%~60%)、掉落率(3%~12%) | 消耗对应属性符文 1 块<br>(Item ID: 59000 ~ 59063) | 100% 成功<br>单件装备仅限生效 1 个 `FW_Base` 符文 | 对应部位装备：<br>• FWtype 0: 武器<br>• FWtype 1: 防具<br>• FWtype 2: 饰品 | `WeaponBaoshiApplyUtil`<br>`TryApplyAttributeRune`<br>([WeaponBaoshiApplyUtil.cs:387-406](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L387-L406)) | 共有 16 种属性方向，各细分为 4 个品质梯队（普通/强悍/致命/毁灭等）。 |
| **宝石合成**<br>(Gem Combination) | 将 5 颗同类型同阶低级宝石合成为 1 颗高一阶品质的宝石。 | 5 颗初级宝石 + 金币：<br>• 升标准: 100<br>• 升精致: 500<br>• 升卓越: 1,000<br>• 升无瑕: 3,000<br>• 升完美: 8,000<br>• 升史诗: 20,000<br>• 升传奇: 50,000 | 100% 成功<br>共 7 级进阶阶段 | 宝石加工面板中操作 | `UI.Managers.BaoshiManager`<br>`CreateBaoshi`<br>([BaoshiManager.cs:727-848](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Managers/BaoshiManager.cs#L727-L848)) | 支持单次合成 1 颗或批量合成 5 颗；参数由 `BaoshiSettings.asset` 控制。 |
| **宝石与符文拆卸**<br>(Gem & Rune Splitting) | 将装备上已镶嵌的宝石、天赋技能符文、特技符文或属性符文安全拆除，无损返还至玩家背包。 | 金币（按宝石/符文品质收费）：<br>• 0级裂开: 100<br>• 1级标准: 500<br>• 2级精致: 1,000<br>• 3级卓越: 3,000<br>• 4级无瑕: 8,000<br>• 5级完美: 15,000<br>• 6级史诗: 30,000<br>• 7级传奇: 80,000 | 100% 成功 | 所有已镶嵌宝石或符文的装备 | `UI.Managers.BaoshiManager`<br>`SplitBaoshi` / `SplitRune`<br>([BaoshiManager.cs:912-1180](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Managers/BaoshiManager.cs#L912-L1180)) | 拆卸后装备对应凹槽清空或计数器扣减，装备扣除对应售价，宝石/符文原样返还。 |

---

## 2. 基础属性符文（Attribute Runes）完整数值梯队表 (59000 ~ 59063)

属性符文共 16 种效果分类，每种效果覆盖 4 个品质等级（9=标准/10=强悍/11=致命/12=毁灭），共计 64 个独立条目：

| 适用部位 | 属性效果分类 | 传奇/极品 (Level 12) | 史诗/高级 (Level 11) | 卓越/中级 (Level 10) | 标准/初级 (Level 9) | 对应数据字段 |
|---|---|---|---|---|---|---|
| **武器** (CharType 0/1) | **伤害加成** | 毁灭的符文 (`DMG`, +100%) | 致命的符文 (`DMG`, +70%) | 强悍的符文 (`DMG`, +40%) | 伤害的符文 (`DMG`, +20%) | `ItemClass.Number` (59000~59003) |
| **武器** (CharType 0/1) | **攻击速度** | 全能的符文 (`ATS`, +150%) | 狂暴的符文 (`ATS`, +100%) | 残暴的符文 (`ATS`, +60%) | 暴力的符文 (`ATS`, +30%) | `ItemClass.Number` (59004~59007) |
| **武器** (CharType 0/1) | **暴击点数** | 爆裂的符文 (`BJD`, +50) | 狂怒的符文 (`BJD`, +36) | 狙击的符文 (`BJD`, +20) | 精准的符文 (`BJD`, +10) | `ItemClass.Number` (59008~59011) |
| **武器** (CharType 0/1) | **全穿透** | 湮灭的符文 (`ALLC`, +40%) | 斩首的符文 (`ALLC`, +30%) | 穿刺的符文 (`ALLC`, +20%) | 锋利的符文 (`ALLC`, +10%) | `ItemClass.Number` (59012~59015) |
| **武器** (CharType 0/1) | **持续伤害 (DOT)** | 梦魇的符文 (`DOT`, +200%) | 恐怖的符文 (`DOT`, +150%) | 诅咒的符文 (`DOT`, +100%) | 污秽的符文 (`DOT`, +50%) | `ItemClass.Number` (59016~59019) |
| **武器** (CharType 0/1) | **同伴伤害** | 恶魔的符文 (`C_DMG`, +150%) | 恶毒的符文 (`C_DMG`, +100%) | 残忍的符文 (`C_DMG`, +60%) | 锯齿的符文 (`C_DMG`, +30%) | `ItemClass.Number` (59020~59023) |
| **武器** (CharType 0/1) | **同伴攻速** | 暴怒的符文 (`C_ATS`, +150%) | 狂强的符文 (`C_ATS`, +100%) | 禁忌的符文 (`C_ATS`, +60%) | 敏捷的符文 (`C_ATS`, +30%) | `ItemClass.Number` (59024~59027) |
| **防具** (CharType 2~5) | **生命上限** | 庇护的符文 (`Heal`, +60%) | 装甲的符文 (`Heal`, +36%) | 强壮的符文 (`Heal`, +20%) | 坚硬的符文 (`Heal`, +10%) | `ItemClass.Number` (59028~59031) |
| **防具** (CharType 2~5) | **法力上限** | 天空的符文 (`Mana`, +60%) | 秘境的符文 (`Mana`, +36%) | 精湛的符文 (`Mana`, +20%) | 清澈的符文 (`Mana`, +10%) | `ItemClass.Number` (59032~59035) |
| **防具** (CharType 2~5) | **全元素抗性** | 虚无的符文 (`Anti`, +20%) | 吸收的符文 (`Anti`, +15%) | 坚决的符文 (`Anti`, +10%) | 刚毅的符文 (`Anti`, +5%) | `ItemClass.Number` (59036~59039) |
| **防具** (CharType 2~5) | **移动速度** | 荒野的符文 (`MVS`, +20%) | 极速的符文 (`MVS`, +15%) | 迅捷的符文 (`MVS`, +10%) | 轻盈的符文 (`MVS`, +5%) | `ItemClass.Number` (59040~59043) |
| **防具** (CharType 2~5) | **同伴生命** | 复仇的符文 (`C_Heal`, +80%) | 唤魂的符文 (`C_Heal`, +50%) | 掌控的符文 (`C_Heal`, +30%) | 复苏的符文 (`C_Heal`, +15%) | `ItemClass.Number` (59044~59047) |
| **防具** (CharType 2~5) | **同伴全抗** | 虚影的符文 (`C_Anti`, +20%) | 暗影的符文 (`C_Anti`, +15%) | 闪避的符文 (`C_Anti`, +10%) | 荆棘的符文 (`C_Anti`, +5%) | `ItemClass.Number` (59048~59051) |
| **饰品** (CharType 6~9) | **法球伤害** | 百变的符文 (`ORB_DMG`, +60%) | 幸运的符文 (`ORB_DMG`, +45%) | 高贵的符文 (`ORB_DMG`, +30%) | 附魔的符文 (`ORB_DMG`, +15%) | `ItemClass.Number` (59052~59055) |
| **饰品** (CharType 6~9) | **星界/特殊伤害**| 神级的符文 (`XJ_DMG`, +60%) | 大师的符文 (`XJ_DMG`, +45%) | 战术的符文 (`XJ_DMG`, +30%) | 伏击的符文 (`XJ_DMG`, +15%) | `ItemClass.Number` (59056~59059) |
| **饰品** (CharType 6~9) | **物品掉落率** | 宝石的符文 (`Drop`, +12%) | 辉煌的符文 (`Drop`, +9%) | 华丽的符文 (`Drop`, +6%) | 精华的符文 (`Drop`, +3%) | `ItemClass.Number` (59060~59063) |

---

## 3. CSV 与资源数据表结构说明

游戏在运行时通过 `ItemManager` 与 `LOC` 读取打包在 Unity TextAsset 中的数据表：

1. **宝石与道具数据表 (`0 2 Baoshi.csv` / `ItemManager.BStext`)**:
   - **位置**: 打包在 `sharedassets1.assets`（预览转储可见于 `sharedassets1-1217-0 2 Baoshi.txt`）。
   - **表头结构**: `IndexName, GlobalID, ItemName, Price, Quality, Icon, Level, UseType, BS_Quality, SoundDrop, SoundUse, RotateType, Bstype, Number, MstackSize, CstackSize, DropSpriteSize, FWType, DropScene`
   - **关键字段逻辑**:
     - `UseType`: `0`=凹槽宝石, `1`=淬炼精华, `2`=魔法功能石, `3`=技能符文, `4`=特技符文, `5`=基础属性符文。
     - `BStype`: 标识宝石颜色（`red`, `yellow`, `green`, `blue`, `white`, `purple`）或特殊石标识（`Stone_KZ`, `Stone_FS`, `Stone_HH`, `Stone_AM`, `Stone_HM`, `Stone_CG`, `Stone_LC` 等）。
     - `FWType`: 标识符文适用装备类型（`0`=武器, `1`=防具, `2`=饰品, `3`=副手等）。
2. **特技模板定义表 (`0 1 SPC.csv` / `ItemManager.SPCtext`)**:
   - **位置**: 打包在 `sharedassets1.assets`。
   - **职责**: 定义武器特技/符文特技的触发类型、伤害倍率、技能投射物与附加 Buff 机制。
3. **全局配置资产 (ScriptableObject Assets)**:
   - **武器重铸配置 (`Core.Settings.WeaponSettings`)**:
     - `Reb_Price_Base` (300f), `Reb_PriceUP_Count` (1.07f), `Reb_PriceUP_Level` (1.065f), `Reb_CountMax` (1000).
     - `ZQ_Price_Base` (300f), `ZQ_Price_Count` (1.08f), `ZQ_Price_Level` (1.065f), `ZQ_Min` (0.01f), `ZQ_Max` (0.025f).
     - 各品质强化上限：`maxZQ0`=5, `maxZQ1`=5, `maxZQ2`=10, `maxZQ3`=10, `maxZQ4`=15, `maxZQ5`=15, `maxZQ6`=20.
   - **宝石加工配置 (`Core.Settings.BaoshiSettings`)**:
     - `needCount` (5)
     - `createPrice1~7`: 100, 500, 1000, 3000, 8000, 20000, 50000.
     - `splitPrice0~7`: 100, 500, 1000, 3000, 8000, 15000, 30000, 80000.
4. **多语言本地化资源 (`Main_FY.json`, `Item_FY.json`, `SPC_FY.json`)**:
   - **位置**: `res://Localization/`。
   - **职责**: 维护全语言（包含中简、中繁、英、日、韩等 24 种语言）的锻造提示文本、石头名称及词条描述。

---

## 4. 说明

### 1. 核心类清单与系统职责
- [`UI.Panels.WeaponManager`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Panels/WeaponManager.cs): 武器三锻造 UI 状态机中枢，负责元素重铸（Elm）、特技重铸（Spc）、基础强化（Enh）的输入响应、金币校验、数值运算与背包武器数据写回。
- [`WeaponBaoshiApplyUtil`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs): 装备即时应用调度器，统一分发处理宝石凹槽镶嵌、元素/属性精华注入、魔法石功能执行（打孔/复生/幻化/翻倍）以及技能/特技/属性符文附魔。
- [`UI.Managers.BaoshiManager`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Managers/BaoshiManager.cs): 宝石与符文加工中枢，管理宝石合成（5合1升级）与各类镶嵌物（宝石、天赋技能符文、特技符文、属性符文）的无损拆卸。
- [`WeaponClass`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs): 装备运行时核心数据模型，维护强化计数（`ZQ_CountMax`）、重铸计数（`Reb_CountMax`）、翻倍标记（`BaseValueDoubled`, `BaseValueMultiplier`）、凹槽数据（`Aocao`）、特技槽（`SPC`）与符文词条（`FW_Base`）。
- [`Core.Settings.WeaponSettings`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Core.Settings/WeaponSettings.cs) & [`Core.Settings.BaoshiSettings`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Core.Settings/BaoshiSettings.cs): 全局项目配置 ScriptableObject，存储重铸/强化/合成/拆卸的价格常数与增长曲线。
- [`ItemManager`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs): 数据表加载与装备再生中枢，负责解析 `0 2 Baoshi.csv`、`0 1 SPC.csv`、`0 0 Weapon.csv`，并提供 `TryRegenerateWeaponFromTemplate` 装备重铸再生支持。

### 2. 表格列含义与数学计算公式
- **消耗/材料（金币价格公式）**:
  - **重铸价格 (Elm / Spc)**:
    $$\text{Price} = \text{round}\left( \text{Reb\_Price\_Base} \times (\text{Reb\_PriceUP\_Level}^{\text{Level}}) \times (\text{Reb\_PriceUP\_Count}^{\text{Reb\_CountMax}}) \right) \times \frac{100 - \text{QH\_Price}}{100}$$
  - **强化价格 (Enh)**:
    $$\text{Price} = \text{round}\left( \text{ZQ\_Price\_Base} \times (\text{ZQ\_Price\_Level}^{\text{Level}}) \times (\text{ZQ\_Price\_Count}^{\text{ZQ\_CountMax}}) \right) \times \frac{100 - \text{QH\_Price}}{100}$$
  - **基础强化单次增量**:
    $$\Delta = \max\left(1, \text{round}\left(\text{BaseValue} \times \text{Random.Range}(0.01, 0.025) \times \left(1 + \frac{\text{QH\_Bei}}{100}\right)\right)\right)$$
  - **翻倍乘数**:
    $$\text{FinalStat} = \text{Stat} \times \text{BaseValueMultiplier} \quad (\text{使用对应翻倍石后 } \text{BaseValueMultiplier} = 2.0)$$

### 3. MOD 开发与修改注意事项
1. **源码级修改与构建闭环**: 修改 `MODworkv2/decompiled` 源码后，使用 `dotnet build -c Release` 编译，产物覆盖至 `ShadowDungeon/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll` 即可生效。
2. **重铸次数防爆与指数溢出**: `Reb_PriceUP_Count` 与 `ZQ_PriceUP_Count` 均为指数成长，若大幅提升重铸上限（如超过 1500 次），金币价格计算公式中的 `Mathf.Pow` 可能迅速超出 `long.MaxValue` 或出现 `Infinity`，修改时应同步压低成长底数或增加 `Mathf.Clamp` 价格上限保护。
3. **装备翻倍逻辑一致性**: `BaseValueMultiplier` 依赖 `NormalizeBaseValueMultiplier` 进行合法性校验（若 `BaseValueDoubled=true` 则强制设为 `2f`）。在扩展其他倍率石时需注意 `SaveDataEquipmentSanitizer` 对存档的清洗规则，避免读档时被强行重置为 `1f` 或 `2f`。
4. **复生石前置约束**: `CanRegenerateWeapon` 严格要求装备不得含有符文、宝石或技能符文投资，修改装备模板匹配时须保证 `ItemManager.FindWeaponTemplate` 能正确命中 `Weapon_Group`，否则重置会返回 `false`。

### 4. 未覆盖或存疑项说明
- **预留未制作石头 (Unimplemented Stones)**:
  在 `0 2 Baoshi.csv` 中定义了 4 种特殊石：`Stone_FM` (附魔石, 50068)、`Stone_HD` (混沌石, 50069)、`Stone_XL` (洗炼石, 50070)、`Stone_CL` (淬炼石, 50071)。源码 `WeaponBaoshiApplyUtil.cs:308` 的 `switch` 分支中未对这 4 种类型做逻辑实现（直接返回 `false`），且多语言文本标注为 `“未制作 / Not implemented”`。此为官方开发预留占位符，可作为后续 MOD 自定义高级锻造机制（如洗词条、重置强化次数等）的理想扩展锚点。
