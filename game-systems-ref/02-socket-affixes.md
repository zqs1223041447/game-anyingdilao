# 《暗影地牢》(Shadow Dungeon) 镶嵌物与插槽词缀体系全解析

《暗影地牢》的镶嵌体系以 [`BaoshiClass`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)（统一数据模型，涵盖宝石、精华、功能石、符文四大形态）与 [`WeaponClass.Aocao`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs)（装备插槽列表，元素类型为 [`WPAocao`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPAocao.cs)）为核心。装备插槽上限由格子尺寸决定（`MaxAocaoCount = SizeX * SizeY`），生成时随机开启 `0 ~ CurAocaoCount` 个插槽，可通过开凿石（`Stone_KZ`）打孔扩充至上限；插槽直接影响装备估价（由 [`AocaoPrice`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/AocaoPrice.cs) 提供加价梯度）。镶嵌流程通过 [`WeaponBaoshiApplyUtil`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs) 分流处理：普通宝石依颜色与装备部位（武器/头/胸/手/腿）分化为 26 种具体属性（Type 0~25），支持玩家属性加成联动公式 `FloorToInt((Base + BS_Add) * (1 + BS_Multi / 100))`；精华系统支持元素注入（单件限 12 次）与基础属性融合（单件限 8 次）；符文系统支持技能等级提升（`WPSkill.Number2`）、第 2 特效槽位赋予（`SPC[1]`）与专属装备底缀附着（`FW_Base`）。此外，游戏通过 [`BaoshiManager`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Managers/BaoshiManager.cs) 提供 5 合 1 宝石升级合成，以及针对宝石、天赋符文、特效符文、基础属性符文的 4 轨无损拆卸返还机制。数据表中共定义了 **137 项** 独立条目。

---

## 1. 普通彩色宝石（Standard Socketed Gems，UseType = 0，共 48 条）

普通彩色宝石镶嵌到装备的空插槽（`WPAocao.HasAocao == true && HasBaoshi == false`）中。不同颜色镶嵌在不同装备部位时，会由 [`WeaponBaoshiApplyUtil.GetSocketType`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L71-L205) 分配唯一的 `Type` 标识（0~25），并在装备时由 [`WeaponClass.ApplySocketedGemStats`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2847-L2943) 转化为玩家对应属性加成。最终数值受玩家天赋/词缀加成：`NumberLast = FloorToInt((Number + BS_Add) * (1 + BS_Multi / 100))`。
合成与拆卸价格由 [`BaoshiSettings`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Core.Settings/BaoshiSettings.cs) 与品质（0~7）决定。

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 传奇的红宝石<br>(Legendary Ruby) | 红宝石 (red)<br>7 级 (传奇) | 基础值: 10%<br>• 武器: 火焰伤害 +10%<br>• 头部/腿部: 最大生命 +10%<br>• 胸甲: 火焰抗性 +10%<br>• 手部: 火焰穿透 +10% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=2) / 5个6级合成 (50,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50001 | 拆卸费: 80,000金<br>售价: 80,000金 |
| 史诗的红宝石<br>(Epic Ruby) | 红宝石 (red)<br>6 级 (史诗) | 基础值: 8%<br>• 武器: 火焰伤害 +8%<br>• 头部/腿部: 最大生命 +8%<br>• 胸甲: 火焰抗性 +8%<br>• 手部: 火焰穿透 +8% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=1) / 5个5级合成 (20,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50002 | 拆卸费: 30,000金<br>售价: 30,000金 |
| 完美的红宝石<br>(Perfect Ruby) | 红宝石 (red)<br>5 级 (完美) | 基础值: 6%<br>• 武器: 火焰伤害 +6%<br>• 头部/腿部: 最大生命 +6%<br>• 胸甲: 火焰抗性 +6%<br>• 手部: 火焰穿透 +6% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个4级合成 (8,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50003 | 拆卸费: 15,000金<br>售价: 12,000金 |
| 无暇的红宝石<br>(Flawless Ruby) | 红宝石 (red)<br>4 级 (无瑕) | 基础值: 5%<br>• 武器: 火焰伤害 +5%<br>• 头部/腿部: 最大生命 +5%<br>• 胸甲: 火焰抗性 +5%<br>• 手部: 火焰穿透 +5% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个3级合成 (3,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50004 | 拆卸费: 8,000金<br>售价: 5,000金 |
| 卓越的红宝石<br>(Superior Ruby) | 红宝石 (red)<br>3 级 (卓越) | 基础值: 4%<br>• 武器: 火焰伤害 +4%<br>• 头部/腿部: 最大生命 +4%<br>• 胸甲: 火焰抗性 +4%<br>• 手部: 火焰穿透 +4% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个2级合成 (1,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50005 | 拆卸费: 3,000金<br>售价: 2,000金 |
| 精致的红宝石<br>(Exquisite Ruby) | 红宝石 (red)<br>2 级 (精致) | 基础值: 3%<br>• 武器: 火焰伤害 +3%<br>• 头部/腿部: 最大生命 +3%<br>• 胸甲: 火焰抗性 +3%<br>• 手部: 火焰穿透 +3% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个1级合成 (500金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50006 | 拆卸费: 1,000金<br>售价: 800金 |
| 标准的红宝石<br>(Standard Ruby) | 红宝石 (red)<br>1 级 (标准) | 基础值: 2%<br>• 武器: 火焰伤害 +2%<br>• 头部/腿部: 最大生命 +2%<br>• 胸甲: 火焰抗性 +2%<br>• 手部: 火焰穿透 +2% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个0级合成 (100金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50007 | 拆卸费: 500金<br>售价: 300金 |
| 裂开的红宝石<br>(Chipped Ruby) | 红宝石 (red)<br>0 级 (裂开) | 基础值: 1%<br>• 武器: 火焰伤害 +1%<br>• 头部/腿部: 最大生命 +1%<br>• 胸甲: 火焰抗性 +1%<br>• 手部: 火焰穿透 +1% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢基础掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50008 | 拆卸费: 100金<br>售价: 120金 |
| 传奇的蓝宝石<br>(Legendary Sapphire) | 蓝宝石 (blue)<br>7 级 (传奇) | 基础值: 10%<br>• 武器: 冰霜伤害 +10%<br>• 头部/腿部: 最大法力 +10%<br>• 胸甲: 冰霜抗性 +10%<br>• 手部: 冰霜穿透 +10% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=2) / 5个6级合成 (50,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50009 | 拆卸费: 80,000金<br>售价: 80,000金 |
| 史诗的蓝宝石<br>(Epic Sapphire) | 蓝宝石 (blue)<br>6 级 (史诗) | 基础值: 8%<br>• 武器: 冰霜伤害 +8%<br>• 头部/腿部: 最大法力 +8%<br>• 胸甲: 冰霜抗性 +8%<br>• 手部: 冰霜穿透 +8% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=1) / 5个5级合成 (20,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50010 | 拆卸费: 30,000金<br>售价: 30,000金 |
| 完美的蓝宝石<br>(Perfect Sapphire) | 蓝宝石 (blue)<br>5 级 (完美) | 基础值: 6%<br>• 武器: 冰霜伤害 +6%<br>• 头部/腿部: 最大法力 +6%<br>• 胸甲: 冰霜抗性 +6%<br>• 手部: 冰霜穿透 +6% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个4级合成 (8,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50011 | 拆卸费: 15,000金<br>售价: 12,000金 |
| 无暇的蓝宝石<br>(Flawless Sapphire) | 蓝宝石 (blue)<br>4 级 (无瑕) | 基础值: 5%<br>• 武器: 冰霜伤害 +5%<br>• 头部/腿部: 最大法力 +5%<br>• 胸甲: 冰霜抗性 +5%<br>• 手部: 冰霜穿透 +5% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个3级合成 (3,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50012 | 拆卸费: 8,000金<br>售价: 5,000金 |
| 卓越的蓝宝石<br>(Superior Sapphire) | 蓝宝石 (blue)<br>3 级 (卓越) | 基础值: 4%<br>• 武器: 冰霜伤害 +4%<br>• 头部/腿部: 最大法力 +4%<br>• 胸甲: 冰霜抗性 +4%<br>• 手部: 冰霜穿透 +4% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个2级合成 (1,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50013 | 拆卸费: 3,000金<br>售价: 2,000金 |
| 精致的蓝宝石<br>(Exquisite Sapphire) | 蓝宝石 (blue)<br>2 级 (精致) | 基础值: 3%<br>• 武器: 冰霜伤害 +3%<br>• 头部/腿部: 最大法力 +3%<br>• 胸甲: 冰霜抗性 +3%<br>• 手部: 冰霜穿透 +3% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个1级合成 (500金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50014 | 拆卸费: 1,000金<br>售价: 800金 |
| 标准的蓝宝石<br>(Standard Sapphire) | 蓝宝石 (blue)<br>1 级 (标准) | 基础值: 2%<br>• 武器: 冰霜伤害 +2%<br>• 头部/腿部: 最大法力 +2%<br>• 胸甲: 冰霜抗性 +2%<br>• 手部: 冰霜穿透 +2% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个0级合成 (100金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50015 | 拆卸费: 500金<br>售价: 300金 |
| 裂开的蓝宝石<br>(Chipped Sapphire) | 蓝宝石 (blue)<br>0 级 (裂开) | 基础值: 1%<br>• 武器: 冰霜伤害 +1%<br>• 头部/腿部: 最大法力 +1%<br>• 胸甲: 冰霜抗性 +1%<br>• 手部: 冰霜穿透 +1% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢基础掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50016 | 拆卸费: 100金<br>售价: 120金 |
| 传奇的黄宝石<br>(Legendary Topaz) | 黄宝石 (yellow)<br>7 级 (传奇) | 基础值: 10%<br>• 武器: 闪电伤害 +10%<br>• 头部/腿部: 掉落率 +10%<br>• 胸甲: 闪电抗性 +10%<br>• 手部: 闪电穿透 +10% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=2) / 5个6级合成 (50,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50017 | 拆卸费: 80,000金<br>售价: 80,000金 |
| 史诗的黄宝石<br>(Epic Topaz) | 黄宝石 (yellow)<br>6 级 (史诗) | 基础值: 8%<br>• 武器: 闪电伤害 +8%<br>• 头部/腿部: 掉落率 +8%<br>• 胸甲: 闪电抗性 +8%<br>• 手部: 闪电穿透 +8% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=1) / 5个5级合成 (20,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50018 | 拆卸费: 30,000金<br>售价: 30,000金 |
| 完美的黄宝石<br>(Perfect Topaz) | 黄宝石 (yellow)<br>5 级 (完美) | 基础值: 6%<br>• 武器: 闪电伤害 +6%<br>• 头部/腿部: 掉落率 +6%<br>• 胸甲: 闪电抗性 +6%<br>• 手部: 闪电穿透 +6% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个4级合成 (8,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50019 | 拆卸费: 15,000金<br>售价: 12,000金 |
| 无暇的黄宝石<br>(Flawless Topaz) | 黄宝石 (yellow)<br>4 级 (无瑕) | 基础值: 5%<br>• 武器: 闪电伤害 +5%<br>• 头部/腿部: 掉落率 +5%<br>• 胸甲: 闪电抗性 +5%<br>• 手部: 闪电穿透 +5% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个3级合成 (3,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50020 | 拆卸费: 8,000金<br>售价: 5,000金 |
| 卓越的黄宝石<br>(Superior Topaz) | 黄宝石 (yellow)<br>3 级 (卓越) | 基础值: 4%<br>• 武器: 闪电伤害 +4%<br>• 头部/腿部: 掉落率 +4%<br>• 胸甲: 闪电抗性 +4%<br>• 手部: 闪电穿透 +4% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个2级合成 (1,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50021 | 拆卸费: 3,000金<br>售价: 2,000金 |
| 精致的黄宝石<br>(Exquisite Topaz) | 黄宝石 (yellow)<br>2 级 (精致) | 基础值: 3%<br>• 武器: 闪电伤害 +3%<br>• 头部/腿部: 掉落率 +3%<br>• 胸甲: 闪电抗性 +3%<br>• 手部: 闪电穿透 +3% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个1级合成 (500金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50022 | 拆卸费: 1,000金<br>售价: 800金 |
| 标准的黄宝石<br>(Standard Topaz) | 黄宝石 (yellow)<br>1 级 (标准) | 基础值: 2%<br>• 武器: 闪电伤害 +2%<br>• 头部/腿部: 掉落率 +2%<br>• 胸甲: 闪电抗性 +2%<br>• 手部: 闪电穿透 +2% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个0级合成 (100金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50023 | 拆卸费: 500金<br>售价: 300金 |
| 裂开的黄宝石<br>(Chipped Topaz) | 黄宝石 (yellow)<br>0 级 (裂开) | 基础值: 1%<br>• 武器: 闪电伤害 +1%<br>• 头部/腿部: 掉落率 +1%<br>• 胸甲: 闪电抗性 +1%<br>• 手部: 闪电穿透 +1% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢基础掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50024 | 拆卸费: 100金<br>售价: 120金 |
| 传奇的绿宝石<br>(Legendary Emerald) | 绿宝石 (green)<br>7 级 (传奇) | 基础值: 10%<br>• 武器: 毒素伤害 +10%<br>• 头部: 同伴最大生命 +10%<br>• 胸甲: 毒素抗性 +10%<br>• 手部: 毒素穿透 +10%<br>• 腿部: 同伴攻击速度 +10% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=2) / 5个6级合成 (50,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50025 | 拆卸费: 80,000金<br>售价: 80,000金 |
| 史诗的绿宝石<br>(Epic Emerald) | 绿宝石 (green)<br>6 级 (史诗) | 基础值: 8%<br>• 武器: 毒素伤害 +8%<br>• 头部: 同伴最大生命 +8%<br>• 胸甲: 毒素抗性 +8%<br>• 手部: 毒素穿透 +8%<br>• 腿部: 同伴攻击速度 +8% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=1) / 5个5级合成 (20,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50026 | 拆卸费: 30,000金<br>售价: 30,000金 |
| 完美的绿宝石<br>(Perfect Emerald) | 绿宝石 (green)<br>5 级 (完美) | 基础值: 6%<br>• 武器: 毒素伤害 +6%<br>• 头部: 同伴最大生命 +6%<br>• 胸甲: 毒素抗性 +6%<br>• 手部: 毒素穿透 +6%<br>• 腿部: 同伴攻击速度 +6% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个4级合成 (8,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50027 | 拆卸费: 15,000金<br>售价: 12,000金 |
| 无暇的绿宝石<br>(Flawless Emerald) | 绿宝石 (green)<br>4 级 (无瑕) | 基础值: 5%<br>• 武器: 毒素伤害 +5%<br>• 头部: 同伴最大生命 +5%<br>• 胸甲: 毒素抗性 +5%<br>• 手部: 毒素穿透 +5%<br>• 腿部: 同伴攻击速度 +5% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个3级合成 (3,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50028 | 拆卸费: 8,000金<br>售价: 5,000金 |
| 卓越的绿宝石<br>(Superior Emerald) | 绿宝石 (green)<br>3 级 (卓越) | 基础值: 4%<br>• 武器: 毒素伤害 +4%<br>• 头部: 同伴最大生命 +4%<br>• 胸甲: 毒素抗性 +4%<br>• 手部: 毒素穿透 +4%<br>• 腿部: 同伴攻击速度 +4% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个2级合成 (1,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50029 | 拆卸费: 3,000金<br>售价: 2,000金 |
| 精致的绿宝石<br>(Exquisite Emerald) | 绿宝石 (green)<br>2 级 (精致) | 基础值: 3%<br>• 武器: 毒素伤害 +3%<br>• 头部: 同伴最大生命 +3%<br>• 胸甲: 毒素抗性 +3%<br>• 手部: 毒素穿透 +3%<br>• 腿部: 同伴攻击速度 +3% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个1级合成 (500金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50030 | 拆卸费: 1,000金<br>售价: 800金 |
| 标准的绿宝石<br>(Standard Emerald) | 绿宝石 (green)<br>1 级 (标准) | 基础值: 2%<br>• 武器: 毒素伤害 +2%<br>• 头部: 同伴最大生命 +2%<br>• 胸甲: 毒素抗性 +2%<br>• 手部: 毒素穿透 +2%<br>• 腿部: 同伴攻击速度 +2% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个0级合成 (100金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50031 | 拆卸费: 500金<br>售价: 300金 |
| 裂开的绿宝石<br>(Chipped Emerald) | 绿宝石 (green)<br>0 级 (裂开) | 基础值: 1%<br>• 武器: 毒素伤害 +1%<br>• 头部: 同伴最大生命 +1%<br>• 胸甲: 毒素抗性 +1%<br>• 手部: 毒素穿透 +1%<br>• 腿部: 同伴攻击速度 +1% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢基础掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50032 | 拆卸费: 100金<br>售价: 120金 |
| 传奇的钻石<br>(Legendary Diamond) | 钻石/白宝石 (white)<br>7 级 (传奇) | 基础值: 10%<br>• 武器: 物理伤害 +10%<br>• 头部/腿部: 攻击速度 +10%<br>• 胸甲: 物理抗性 +10%<br>• 手部: 物理穿透 +10% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=2) / 5个6级合成 (50,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50033 | 拆卸费: 80,000金<br>售价: 80,000金 |
| 史诗的钻石<br>(Epic Diamond) | 钻石/白宝石 (white)<br>6 级 (史诗) | 基础值: 8%<br>• 武器: 物理伤害 +8%<br>• 头部/腿部: 攻击速度 +8%<br>• 胸甲: 物理抗性 +8%<br>• 手部: 物理穿透 +8% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=1) / 5个5级合成 (20,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50034 | 拆卸费: 30,000金<br>售价: 30,000金 |
| 完美的钻石<br>(Perfect Diamond) | 钻石/白宝石 (white)<br>5 级 (完美) | 基础值: 6%<br>• 武器: 物理伤害 +6%<br>• 头部/腿部: 攻击速度 +6%<br>• 胸甲: 物理抗性 +6%<br>• 手部: 物理穿透 +6% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个4级合成 (8,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50035 | 拆卸费: 15,000金<br>售价: 12,000金 |
| 无暇的钻石<br>(Flawless Diamond) | 钻石/白宝石 (white)<br>4 级 (无瑕) | 基础值: 5%<br>• 武器: 物理伤害 +5%<br>• 头部/腿部: 攻击速度 +5%<br>• 胸甲: 物理抗性 +5%<br>• 手部: 物理穿透 +5% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个3级合成 (3,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50036 | 拆卸费: 8,000金<br>售价: 5,000金 |
| 卓越的钻石<br>(Superior Diamond) | 钻石/白宝石 (white)<br>3 级 (卓越) | 基础值: 4%<br>• 武器: 物理伤害 +4%<br>• 头部/腿部: 攻击速度 +4%<br>• 胸甲: 物理抗性 +4%<br>• 手部: 物理穿透 +4% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个2级合成 (1,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50037 | 拆卸费: 3,000金<br>售价: 2,000金 |
| 精致的钻石<br>(Exquisite Diamond) | 钻石/白宝石 (white)<br>2 级 (精致) | 基础值: 3%<br>• 武器: 物理伤害 +3%<br>• 头部/腿部: 攻击速度 +3%<br>• 胸甲: 物理抗性 +3%<br>• 手部: 物理穿透 +3% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个1级合成 (500金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50038 | 拆卸费: 1,000金<br>售价: 800金 |
| 标准的钻石<br>(Standard Diamond) | 钻石/白宝石 (white)<br>1 级 (标准) | 基础值: 2%<br>• 武器: 物理伤害 +2%<br>• 头部/腿部: 攻击速度 +2%<br>• 胸甲: 物理抗性 +2%<br>• 手部: 物理穿透 +2% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个0级合成 (100金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50039 | 拆卸费: 500金<br>售价: 300金 |
| 裂开的钻石<br>(Chipped Diamond) | 钻石/白宝石 (white)<br>0 级 (裂开) | 基础值: 1%<br>• 武器: 物理伤害 +1%<br>• 头部/腿部: 攻击速度 +1%<br>• 胸甲: 物理抗性 +1%<br>• 手部: 物理穿透 +1% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢基础掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50040 | 拆卸费: 100金<br>售价: 120金 |
| 传奇的紫宝石<br>(Legendary Amethyst) | 紫宝石 (purple)<br>7 级 (传奇) | 基础值: 10%<br>• 武器: 暗影伤害 +10%<br>• 头部: 同伴伤害 +10%<br>• 胸甲: 暗影抗性 +10%<br>• 手部: 暗影穿透 +10%<br>• 腿部: 移动速度 +10% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=2) / 5个6级合成 (50,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50041 | 拆卸费: 80,000金<br>售价: 80,000金 |
| 史诗的紫宝石<br>(Epic Amethyst) | 紫宝石 (purple)<br>6 级 (史诗) | 基础值: 8%<br>• 武器: 暗影伤害 +8%<br>• 头部: 同伴伤害 +8%<br>• 胸甲: 暗影抗性 +8%<br>• 手部: 暗影穿透 +8%<br>• 腿部: 移动速度 +8% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=1) / 5个5级合成 (20,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50042 | 拆卸费: 30,000金<br>售价: 30,000金 |
| 完美的紫宝石<br>(Perfect Amethyst) | 紫宝石 (purple)<br>5 级 (完美) | 基础值: 6%<br>• 武器: 暗影伤害 +6%<br>• 头部: 同伴伤害 +6%<br>• 胸甲: 暗影抗性 +6%<br>• 手部: 暗影穿透 +6%<br>• 腿部: 移动速度 +6% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个4级合成 (8,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50043 | 拆卸费: 15,000金<br>售价: 12,000金 |
| 无暇的紫宝石<br>(Flawless Amethyst) | 紫宝石 (purple)<br>4 级 (无瑕) | 基础值: 5%<br>• 武器: 暗影伤害 +5%<br>• 头部: 同伴伤害 +5%<br>• 胸甲: 暗影抗性 +5%<br>• 手部: 暗影穿透 +5%<br>• 腿部: 移动速度 +5% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个3级合成 (3,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50044 | 拆卸费: 8,000金<br>售价: 5,000金 |
| 卓越的紫宝石<br>(Superior Amethyst) | 紫宝石 (purple)<br>3 级 (卓越) | 基础值: 4%<br>• 武器: 暗影伤害 +4%<br>• 头部: 同伴伤害 +4%<br>• 胸甲: 暗影抗性 +4%<br>• 手部: 暗影穿透 +4% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个2级合成 (1,000金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50045 | 拆卸费: 3,000金<br>售价: 2,000金 |
| 精致的紫宝石<br>(Exquisite Amethyst) | 紫宝石 (purple)<br>2 级 (精致) | 基础值: 3%<br>• 武器: 暗影伤害 +3%<br>• 头部: 同伴伤害 +3%<br>• 胸甲: 暗影抗性 +3%<br>• 手部: 暗影穿透 +3% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个1级合成 (500金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50046 | 拆卸费: 1,000金<br>售价: 800金 |
| 标准的紫宝石<br>(Standard Amethyst) | 紫宝石 (purple)<br>1 级 (标准) | 基础值: 2%<br>• 武器: 暗影伤害 +2%<br>• 头部: 同伴伤害 +2%<br>• 胸甲: 暗影抗性 +2%<br>• 手部: 暗影穿透 +2% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢掉落 (DropScene=0) / 5个0级合成 (100金) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50047 | 拆卸费: 500金<br>售价: 300金 |
| 裂开的紫宝石<br>(Chipped Amethyst) | 紫宝石 (purple)<br>0 级 (裂开) | 基础值: 1%<br>• 武器: 暗影伤害 +1%<br>• 头部: 同伴伤害 +1%<br>• 胸甲: 暗影抗性 +1%<br>• 手部: 暗影穿透 +1% | 武器、头部、胸甲、手部、腿部的宝石插槽 | 地牢基础掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50048 | 拆卸费: 100金<br>售价: 120金 |

---

## 2. 融合精华（Essences，UseType = 1，共 12 条）

精华直接作用于装备本身（无须占用宝石插槽）。
- **元素精华 (JHEL0~JHEL5)**：单件装备最多融合 12 次（`JHEL_Count < 12`）。数值取决于部位：主手武器 +4% 对应元素伤害，副手武器/戒指 +1% 对应元素穿透，护甲/护符 +1% 对应元素抗性，法球/首饰 +3% 对应元素伤害。
- **属性精华 (JH_*)**：单件装备最多融合 8 次（`JH_Count < 8`）。直接添加到装备的 `Main` 词缀中。

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 火胆精华<br>(Ember Essence) | 元素精华<br>(JHEL0) | 注入火焰元素数值：<br>• 主手武器: 火焰伤害 +4%<br>• 副手武器/戒指: 火焰穿透 +1%<br>• 护甲/护符: 火焰抗性 +1%<br>• 法球/首饰: 火焰伤害 +3% | 全部装备部位 (单件限 12 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50049 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)<br>售出单价: 200,000金 |
| 冰封精魄<br>(Frost Essence) | 元素精华<br>(JHEL1) | 注入冰霜元素数值：<br>• 主手武器: 冰霜伤害 +4%<br>• 副手武器/戒指: 冰霜穿透 +1%<br>• 护甲/护符: 冰霜抗性 +1%<br>• 法球/首饰: 冰霜伤害 +3% | 全部装备部位 (单件限 12 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50050 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)<br>售出单价: 200,000金 |
| 御电精华<br>(Storm Essence) | 元素精华<br>(JHEL2) | 注入闪电元素数值：<br>• 主手武器: 闪电伤害 +4%<br>• 副手武器/戒指: 闪电穿透 +1%<br>• 护甲/护符: 闪电抗性 +1%<br>• 法球/首饰: 闪电伤害 +3% | 全部装备部位 (单件限 12 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50051 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)<br>售出单价: 200,000金 |
| 淬毒精华<br>(Venom Essence) | 元素精华<br>(JHEL3) | 注入毒素元素数值：<br>• 主手武器: 毒素伤害 +4%<br>• 副手武器/戒指: 毒素穿透 +1%<br>• 护甲/护符: 毒素抗性 +1%<br>• 法球/首饰: 毒素伤害 +3% | 全部装备部位 (单件限 12 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50052 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)<br>售出单价: 200,000金 |
| 穿刺精华<br>(Piercing Essence) | 元素精华<br>(JHEL4) | 注入物理元素数值：<br>• 主手武器: 物理伤害 +4%<br>• 副手武器/戒指: 物理穿透 +1%<br>• 护甲/护符: 物理抗性 +1%<br>• 法球/首饰: 物理伤害 +3% | 全部装备部位 (单件限 12 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50053 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)<br>售出单价: 200,000金 |
| 诅咒精华<br>(Curse Essence) | 元素精华<br>(JHEL5) | 注入暗影元素数值：<br>• 主手武器: 暗影伤害 +4%<br>• 副手武器/戒指: 暗影穿透 +1%<br>• 护甲/护符: 暗影抗性 +1%<br>• 法球/首饰: 暗影伤害 +3% | 全部装备部位 (单件限 12 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50054 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L214-L232)<br>售出单价: 200,000金 |
| 伤害精华<br>(Damage Essence) | 属性精华<br>(JH_damage) | 融合进入武器主属性：<br>• 基础伤害 (Damage_Bei) +3% | 武器 (单件限 8 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50055 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L240-L242)<br>主词缀 Index: 10 |
| 迅捷精华<br>(Swiftness Essence) | 属性精华<br>(JH_ats) | 融合进入武器主属性：<br>• 攻击速度 (ATSpeed_Bei) +3% | 武器 (单件限 8 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50056 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L249-L251)<br>主词缀 Index: 11 |
| 恶魔精华<br>(Demonic Essence) | 属性精华<br>(JH_CPdamage) | 融合进入武器主属性：<br>• 同伴伤害 (C_Damage) +5% | 武器 (单件限 8 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50057 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L252-L254)<br>主词缀 Index: 101 |
| 生命精华<br>(Vitality Essence) | 属性精华<br>(JH_heal) | 融合进入防具主属性：<br>• 最大生命 (Health_Bei) +3% | 防具 (单件限 8 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50058 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L243-L245)<br>主词缀 Index: 1 |
| 智力精华<br>(Wisdom Essence) | 属性精华<br>(JH_mana) | 融合进入防具主属性：<br>• 最大法力 (Mana_Bei) +3% | 防具 (单件限 8 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50059 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L246-L248)<br>主词缀 Index: 2 |
| 傀儡精华<br>(Golem Essence) | 属性精华<br>(JH_CPheal) | 融合进入防具主属性：<br>• 同伴最大生命 (C_Health) +5% | 防具 (单件限 8 次) | 地牢掉落 (DropScene=1) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50060 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L255-L257)<br>主词缀 Index: 100 |

---

## 3. 功能与铸造石（Crafting & Modification Stones，UseType = 2，共 11 条）

功能石用于对装备进行扩展打孔、数值翻倍、词缀洗炼或升华重铸。

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 开凿石<br>(Chiseling Stone) | 功能打孔石<br>(Stone_KZ) | 为装备增加 1 个可用宝石插槽（`AocaoCount++`，`HasAocao=true`） | 武器、防具（未达插槽上限 `MaxAocaoCount` 时） | 地牢掉落 (DropScene=0) / 商店 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50061 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L408-L426)<br>售出单价: 2,000金 |
| 飞升石<br>(Ascension Stone) | 升华重铸石<br>(Stone_FS) | 按照玩家当前等级重新从模板生成装备基础数值与词缀 | 未镶嵌宝石与符文的武器/防具/饰品 | 地牢掉落 (DropScene=0) / 商店 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50062 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L288-L293)<br>售出单价: 200,000金 |
| 幻化石<br>(Transmutation Stone) | 幻化强化石<br>(Stone_HH) | 提升装备特效伤害倍率：<br>`SPC_DMG_Bei += 5`（单件最多强化 10 次） | 武器/装备 | 秘境高层掉落 (DropScene=2) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50063 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L443-L458)<br>售出单价: 500,000金 |
| 奥秘石<br>(Arcane Stone) | 技能孔石<br>(Stone_AM) | 提升武器天赋技能符文插槽上限：<br>`TryAddSkillFWCountMax()` | 武器 | 秘境高层掉落 (DropScene=2) / 宝箱 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50064 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L306-L308)<br>售出单价: 500,000金 |
| 毁灭石<br>(Annihilation Stone) | 翻倍石<br>(Stone_HM) | 武器基础伤害/属性数值永久翻倍（每件武器仅限使用 1 次） | 武器 | 秘境高层掉落 (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50065 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L300-L305)<br>售出单价: 3,000,000金 |
| 崇高石<br>(Exalted Stone) | 翻倍石<br>(Stone_CG) | 防具基础护甲/生命数值永久翻倍（每件防具仅限使用 1 次） | 防具 (头部/胸甲/手套/鞋子) | 秘境高层掉落 (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50066 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L280-L285)<br>售出单价: 3,000,000金 |
| 棱彩石<br>(Prismatic Stone) | 翻倍石<br>(Stone_LC) | 饰品基础属性数值永久翻倍（每件饰品仅限使用 1 次） | 饰品 (护符/戒指/法球/首饰) | 秘境高层掉落 (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50067 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L294-L299)<br>售出单价: 3,000,000金 |
| 附魔石<br>(Enchanting Stone) | 锻造石<br>(Stone_FM) | 装备附魔功能材料（在武器锻造面板用于附加属性） | 装备锻造面板 | 地牢掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50068 | 售出单价: 500,000金 |
| 混乱石<br>(Chaos Stone) | 锻造石<br>(Stone_HD) | 重洗装备所有随机附加词缀（在武器锻造面板使用） | 装备锻造面板 | 地牢掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50069 | 售出单价: 500,000金 |
| 洗炼石<br>(Reforging Stone) | 锻造石<br>(Stone_XL) | 重洗装备词缀数值区间（在武器锻造面板使用） | 装备锻造面板 | 地牢掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50070 | 售出单价: 500,000金 |
| 淬炼石<br>(Tempering Stone) | 锻造石<br>(Stone_CL) | 提升装备词缀品阶与上限（在武器锻造面板使用） | 装备锻造面板 | 地牢掉落 (DropScene=0) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 50071 | 售出单价: 500,000金 |

---

## 4. 技能与特效符文模版（Skill & SPC Rune Templates，UseType = 3 & 4，共 2 条）

天赋技能符文与装备特效符文在数据表中以母版条目存在，运行时根据具体技能/特效动态派生实例。

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 天赋技能符文<br>(Talent Skill Rune) | 天赋符文<br>(UseType = 3, FW_SK) | 指定天赋技能等级 +1<br>（`WPSkill.Number2++`，提升技能伤害与特效） | 武器的天赋技能槽（受武器 `SKCountMax` 限制） | 拆卸武器已有技能点，或秘境高级掉落 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 51000 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L313-L354)<br>售价由技能品阶决定 |
| 装备技能符文<br>(Equipment Skill Rune) | 特效符文<br>(UseType = 4, FW_SPC) | 赋予装备对应特效（`SPC[1]`）：包括投射物分裂、触发施法、击中斩杀等 169+ 种特效机制 | 由符文 `FWtype` 决定：<br>0: 武器, 1: 头/胸, 2: 手/鞋, 3: 护符/戒指, 4: 法球/饰品 | 拆卸装备已有第2特效，或秘境高级掉落 | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 51001 | [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L356-L385)<br>需符合职业技能类型限制 |

---

## 5. 装备基础属性符文（Equipment Base Attribute Runes，UseType = 5，共 64 条）

装备基础属性符文（`BaseFW`）用于为装备赋予底缀属性（`weapon.FW_Base`）。共 16 种属性类型，每种分为 4 阶品质（T1 顶级、T2 高阶、T3 中阶、T4 基础），对应不同的秘境品质掉落门槛（`DropScene` 1~4）。镶嵌后可通过 [`BaoshiManager.EnterSplitEquipmentAttributeRune`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Managers/BaoshiManager.cs#L450-L453) 拆卸返还。

### 5.1 武器专属属性符文（FWtype = 0，共 28 条）

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 毁灭的符文<br>(Rune of Destruction) | 伤害符文 (DMG)<br>T1 (传奇) | 基础伤害加成 (Damage_Bei) +100% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59000 | 售出单价: 3,000,000金 |
| 致命的符文<br>(Rune of Lethality) | 伤害符文 (DMG)<br>T2 (高阶) | 基础伤害加成 (Damage_Bei) +70% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59001 | 售出单价: 1,200,000金 |
| 威猛的符文<br>(Rune of Might) | 伤害符文 (DMG)<br>T3 (中阶) | 基础伤害加成 (Damage_Bei) +40% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59002 | 售出单价: 500,000金 |
| 增伤的符文<br>(Rune of Damage) | 伤害符文 (DMG)<br>T4 (基础) | 基础伤害加成 (Damage_Bei) +20% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59003 | 售出单价: 200,000金 |
| 全能的符文<br>(Rune of Mastery) | 攻速符文 (ATS)<br>T1 (传奇) | 攻击速度加成 (ATSpeed_Bei) +150% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59004 | 售出单价: 3,000,000金 |
| 狂暴的符文<br>(Rune of Berserking) | 攻速符文 (ATS)<br>T2 (高阶) | 攻击速度加成 (ATSpeed_Bei) +100% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59005 | 售出单价: 1,200,000金 |
| 残暴的符文<br>(Rune of Brutality) | 攻速符文 (ATS)<br>T3 (中阶) | 攻击速度加成 (ATSpeed_Bei) +60% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59006 | 售出单价: 500,000金 |
| 暴力的符文<br>(Rune of Violence) | 攻速符文 (ATS)<br>T4 (基础) | 攻击速度加成 (ATSpeed_Bei) +30% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59007 | 售出单价: 200,000金 |
| 爆裂的符文<br>(Rune of Explosion) | 暴伤符文 (BJD)<br>T1 (传奇) | 暴击伤害 (BJDamage) +50% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59008 | 售出单价: 3,000,000金 |
| 狂怒的符文<br>(Rune of Fury) | 暴伤符文 (BJD)<br>T2 (高阶) | 暴击伤害 (BJDamage) +36% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59009 | 售出单价: 1,200,000金 |
| 狙击的符文<br>(Rune of Sniping) | 暴伤符文 (BJD)<br>T3 (中阶) | 暴击伤害 (BJDamage) +20% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59010 | 售出单价: 500,000金 |
| 精准的符文<br>(Rune of Precision) | 暴伤符文 (BJD)<br>T4 (基础) | 暴击伤害 (BJDamage) +10% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59011 | 售出单价: 200,000金 |
| 灭世的符文<br>(Rune of Annihilation) | 全穿透符文 (ALLC)<br>T1 (传奇) | 全元素穿透 (AllChuan) +40% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59012 | 售出单价: 3,000,000金 |
| 斩首的符文<br>(Rune of Decapitation) | 全穿透符文 (ALLC)<br>T2 (高阶) | 全元素穿透 (AllChuan) +30% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59013 | 售出单价: 1,200,000金 |
| 穿刺的符文<br>(Rune of Piercing) | 全穿透符文 (ALLC)<br>T3 (中阶) | 全元素穿透 (AllChuan) +20% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59014 | 售出单价: 500,000金 |
| 锐利的符文<br>(Rune of Sharpness) | 全穿透符文 (ALLC)<br>T4 (基础) | 全元素穿透 (AllChuan) +10% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59015 | 售出单价: 200,000金 |
| 噩梦的符文<br>(Rune of Nightmare) | 持续伤害符文 (DOT)<br>T1 (传奇) | 持续伤害加成 (AllDot_DMG) +200% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59016 | 售出单价: 3,000,000金 |
| 恐怖的符文<br>(Rune of Terror) | 持续伤害符文 (DOT)<br>T2 (高阶) | 持续伤害加成 (AllDot_DMG) +150% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59017 | 售出单价: 1,200,000金 |
| 诅咒的符文<br>(Rune of Cursing) | 持续伤害符文 (DOT)<br>T3 (中阶) | 持续伤害加成 (AllDot_DMG) +100% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59018 | 售出单价: 500,000金 |
| 不洁的符文<br>(Rune of Defilement) | 持续伤害符文 (DOT)<br>T4 (基础) | 持续伤害加成 (AllDot_DMG) +50% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59019 | 售出单价: 200,000金 |
| 恶魔的符文<br>(Rune of Demons) | 同伴伤害符文 (C_DMG)<br>T1 (传奇) | 同伴伤害加成 (C_Damage) +150% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59020 | 售出单价: 3,000,000金 |
| 险恶的符文<br>(Rune of Malice) | 同伴伤害符文 (C_DMG)<br>T2 (高阶) | 同伴伤害加成 (C_Damage) +100% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59021 | 售出单价: 1,200,000金 |
| 无情的符文<br>(Rune of Ruthlessness) | 同伴伤害符文 (C_DMG)<br>T3 (中阶) | 同伴伤害加成 (C_Damage) +60% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59022 | 售出单价: 500,000金 |
| 锯齿的符文<br>(Rune of Serration) | 同伴伤害符文 (C_DMG)<br>T4 (基础) | 同伴伤害加成 (C_Damage) +30% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59023 | 售出单价: 200,000金 |
| 愤怒的符文<br>(Rune of Wrath) | 同伴攻速符文 (C_ATS)<br>T1 (传奇) | 同伴攻击速度 (C_ATSpeed) +150% | 武器 (FW_Base 插槽) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59024 | 售出单价: 3,000,000金 |
| 超强的符文<br>(Rune of Power) | 同伴攻速符文 (C_ATS)<br>T2 (高阶) | 同伴攻击速度 (C_ATSpeed) +100% | 武器 (FW_Base 插槽) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59025 | 售出单价: 1,200,000金 |
| 禁忌的符文<br>(Rune of Forbidden Power) | 同伴攻速符文 (C_ATS)<br>T3 (中阶) | 同伴攻击速度 (C_ATSpeed) +60% | 武器 (FW_Base 插槽) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59026 | 售出单价: 500,000金 |
| 灵巧的符文<br>(Rune of Dexterity) | 同伴攻速符文 (C_ATS)<br>T4 (基础) | 同伴攻击速度 (C_ATSpeed) +30% | 武器 (FW_Base 插槽) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59027 | 售出单价: 200,000金 |

### 5.2 防具专属属性符文（FWtype = 1，共 24 条）

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 护佑的符文<br>(Rune of Protection) | 生命符文 (Heal)<br>T1 (传奇) | 最大生命加成 (Health_Bei) +60% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59028 | 售出单价: 3,000,000金 |
| 装甲的符文<br>(Rune of Armor) | 生命符文 (Heal)<br>T2 (高阶) | 最大生命加成 (Health_Bei) +36% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59029 | 售出单价: 1,200,000金 |
| 强壮的符文<br>(Rune of Strength) | 生命符文 (Heal)<br>T3 (中阶) | 最大生命加成 (Health_Bei) +20% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59030 | 售出单价: 500,000金 |
| 坚硬的符文<br>(Rune of Hardness) | 生命符文 (Heal)<br>T4 (基础) | 最大生命加成 (Health_Bei) +10% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59031 | 售出单价: 200,000金 |
| 天界的符文<br>(Rune of the Heavens) | 法力符文 (Mana)<br>T1 (传奇) | 最大法力加成 (Mana_Bei) +60% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59032 | 售出单价: 3,000,000金 |
| 奥秘的符文<br>(Rune of Mystery) | 法力符文 (Mana)<br>T2 (高阶) | 最大法力加成 (Mana_Bei) +36% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59033 | 售出单价: 1,200,000金 |
| 精湛的符文<br>(Rune of Expertise) | 法力符文 (Mana)<br>T3 (中阶) | 最大法力加成 (Mana_Bei) +20% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59034 | 售出单价: 500,000金 |
| 清澈的符文<br>(Rune of Clarity) | 法力符文 (Mana)<br>T4 (基础) | 最大法力加成 (Mana_Bei) +10% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59035 | 售出单价: 200,000金 |
| 隐形的符文<br>(Rune of Invisibility) | 全抗性符文 (Anti)<br>T1 (传奇) | 全抗性加成 (AllAnti) +20% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59036 | 售出单价: 3,000,000金 |
| 吸收的符文<br>(Rune of Absorption) | 全抗性符文 (Anti)<br>T2 (高阶) | 全抗性加成 (AllAnti) +15% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59037 | 售出单价: 1,200,000金 |
| 不屈的符文<br>(Rune of Resolve) | 全抗性符文 (Anti)<br>T3 (中阶) | 全抗性加成 (AllAnti) +10% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59038 | 售出单价: 500,000金 |
| 坚韧的符文<br>(Rune of Fortitude) | 全抗性符文 (Anti)<br>T4 (基础) | 全抗性加成 (AllAnti) +5% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59039 | 售出单价: 200,000金 |
| 狂野的符文<br>(Rune of the Wild) | 移速符文 (MVS)<br>T1 (传奇) | 移动速度加成 (MVSpeed_Bei) +20% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59040 | 售出单价: 3,000,000金 |
| 极速的符文<br>(Rune of Swiftness) | 移速符文 (MVS)<br>T2 (高阶) | 移动速度加成 (MVSpeed_Bei) +15% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59041 | 售出单价: 1,200,000金 |
| 迅捷的符文<br>(Rune of Haste) | 移速符文 (MVS)<br>T3 (中阶) | 移动速度加成 (MVSpeed_Bei) +10% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59042 | 售出单价: 500,000金 |
| 轻盈的符文<br>(Rune of Lightness) | 移速符文 (MVS)<br>T4 (基础) | 移动速度加成 (MVSpeed_Bei) +5% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59043 | 售出单价: 200,000金 |
| 复仇的符文<br>(Rune of Vengeance) | 同伴生命符文 (C_Heal)<br>T1 (传奇) | 同伴最大生命 (C_Health) +80% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59044 | 售出单价: 3,000,000金 |
| 召魂的符文<br>(Rune of Soulcalling) | 同伴生命符文 (C_Heal)<br>T2 (高阶) | 同伴最大生命 (C_Health) +50% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59045 | 售出单价: 1,200,000金 |
| 掌控的符文<br>(Rune of Control) | 同伴生命符文 (C_Heal)<br>T3 (中阶) | 同伴最大生命 (C_Health) +30% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59046 | 售出单价: 500,000金 |
| 复苏的符文<br>(Rune of Revival) | 同伴生命符文 (C_Heal)<br>T4 (基础) | 同伴最大生命 (C_Health) +15% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59047 | 售出单价: 200,000金 |
| 无形的符文<br>(Rune of Intangibility) | 同伴全抗符文 (C_Anti)<br>T1 (传奇) | 同伴全抗性 (C_AllAnti) +20% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59048 | 售出单价: 3,000,000金 |
| 阴影的符文<br>(Rune of Shadows) | 同伴全抗符文 (C_Anti)<br>T2 (高阶) | 同伴全抗性 (C_AllAnti) +15% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59049 | 售出单价: 1,200,000金 |
| 闪避的符文<br>(Rune of Evasion) | 同伴全抗符文 (C_Anti)<br>T3 (中阶) | 同伴全抗性 (C_AllAnti) +10% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59050 | 售出单价: 500,000金 |
| 荆棘的符文<br>(Rune of Thorns) | 同伴全抗符文 (C_Anti)<br>T4 (基础) | 同伴全抗性 (C_AllAnti) +5% | 防具 (头部/胸甲/手套/鞋子) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59051 | 售出单价: 200,000金 |

### 5.3 饰品专属属性符文（FWtype = 2，共 12 条）

| 镶嵌物名称 | 类型/等级 | 效果与数值 | 适用部位/插槽 | 获取方式 | 代码位置 | 备注 |
|---|---|---|---|---|---|---|
| 百变的符文<br>(Rune of Versatility) | 法球伤害符文 (ORB_DMG)<br>T1 (传奇) | 附加法球伤害 (WPSPC_DMG) +60 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59052 | 售出单价: 3,000,000金 |
| 幸运的符文<br>(Rune of Fortune) | 法球伤害符文 (ORB_DMG)<br>T2 (高阶) | 附加法球伤害 (WPSPC_DMG) +45 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59053 | 售出单价: 1,200,000金 |
| 贵族的符文<br>(Rune of Nobility) | 法球伤害符文 (ORB_DMG)<br>T3 (中阶) | 附加法球伤害 (WPSPC_DMG) +30 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59054 | 售出单价: 500,000金 |
| 附魔的符文<br>(Rune of Enchantment) | 法球伤害符文 (ORB_DMG)<br>T4 (基础) | 附加法球伤害 (WPSPC_DMG) +15 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59055 | 售出单价: 200,000金 |
| 神级的符文<br>(Rune of Divinity) | 陷阱伤害符文 (XJ_DMG)<br>T1 (传奇) | 陷阱技能伤害 (XJ_DMG) +60 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59056 | 售出单价: 3,000,000金 |
| 大师的符文<br>(Rune of the Master) | 陷阱伤害符文 (XJ_DMG)<br>T2 (高阶) | 陷阱技能伤害 (XJ_DMG) +45 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59057 | 售出单价: 1,200,000金 |
| 战术的符文<br>(Rune of Tactics) | 陷阱伤害符文 (XJ_DMG)<br>T3 (中阶) | 陷阱技能伤害 (XJ_DMG) +30 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59058 | 售出单价: 500,000金 |
| 伏击的符文<br>(Rune of Ambush) | 陷阱伤害符文 (XJ_DMG)<br>T4 (基础) | 陷阱技能伤害 (XJ_DMG) +15 | 饰品 (护符/戒指/法球/首饰) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59059 | 售出单价: 200,000金 |
| 五彩的符文<br>(Rune of Prismatic) | 掉落率符文 (Drop)<br>T1 (传奇) | 物品掉落率 (ItemDrop_Rate) +12% | 饰品 (护符/戒指/法球/首饰) | 秘境层数 4+ (DropScene=4) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59060 | 售出单价: 3,000,000金 |
| 辉煌的符文<br>(Rune of Glory) | 掉落率符文 (Drop)<br>T2 (高阶) | 物品掉落率 (ItemDrop_Rate) +9% | 饰品 (护符/戒指/法球/首饰) | 秘境层数 3+ (DropScene=3) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59061 | 售出单价: 1,200,000金 |
| 华贵的符文<br>(Rune of Splendor) | 掉落率符文 (Drop)<br>T3 (中阶) | 物品掉落率 (ItemDrop_Rate) +6% | 饰品 (护符/戒指/法球/首饰) | 秘境层数 2+ (DropScene=2) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59062 | 售出单价: 500,000金 |
| 精华的符文<br>(Rune of Essence) | 掉落率符文 (Drop)<br>T4 (基础) | 物品掉落率 (ItemDrop_Rate) +3% | 饰品 (护符/戒指/法球/首饰) | 秘境层数 1+ (DropScene=1) | [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)<br>GlobalID: 59063 | 售出单价: 200,000金 |

---

## 6. 数据表与资源结构说明

游戏在运行时通过 [`ItemManager.LoadData_BS`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L5147-L5216) 解析嵌入在 Unity 资源包（`sharedassets1.assets`）中的 TextAsset 数据表（标识为 `0 2 Baoshi`）。

### 6.1 CSV 数据表字段定义（`0 2 Baoshi.csv`）

| 字段序号 | 字段名 | 类型 | 说明 |
|---|---|---|---|
| 0 | 备注/名称 | string | 策划中文备注 |
| 1 | `GlobalID` | int | 物品全局唯一 ID（宝石 50001~50048，精华 50049~50060，功能石 50061~50071，符文模版 51000~51001，属性符文 59000~59063） |
| 2 | `ItemName` | string | 物品英文标识键（对应本地化主键） |
| 3 | `Price` | int | 价格品阶档位（索引对应 [`BaoshiPrice.Price`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiPrice.cs)） |
| 4 | `Quality` | int | 物品稀有度/框色品质（0: 普通, 1: 魔法, 2: 稀有, 3: 史诗, 4: 传奇, 5: 精华, 6: 符文, 8: 宝石） |
| 5 | `Icon` | int | 图标资源索引（对应 `IconBaoshi.icon`） |
| 6 | `Level` | int | 物品使用等级门槛 |
| 7 | `UseType` | int | 镶嵌物功能大类（0: 普通宝石, 1: 融合精华, 2: 功能石, 3: 天赋技能符文, 4: 装备特效符文, 5: 基础属性符文） |
| 8 | `BS_Quality` | int | 宝石专属品质等级（0: 裂开, 1: 标准, 2: 精致, 3: 卓越, 4: 无暇, 5: 完美, 6: 史诗, 7: 传奇） |
| 9 | `SoundDrop` | int | 掉落音效索引 |
| 10 | `SoundUse` | int | 使用/镶嵌音效索引 |
| 11 | `RotateType`| int | 背包旋转模式 |
| 12 | `BStype` | string | 属性子类型分类（宝石颜色 red/blue/yellow/green/white/purple；精华类型 JHEL0~JHEL5/JH_*；符文属性 DMG/ATS/BJD/ALLC/DOT/C_DMG/C_ATS/Heal/Mana/Anti/MVS/C_Heal/C_Anti/ORB_DMG/XJ_DMG/Drop） |
| 13 | `Number` | int | 词缀基础数值（百分比或固定数值） |
| 14 | `MstackSize` | int | 最大堆叠数量（普通宝石/功能石为 999，符文为 1） |
| 15 | `CstackSize` | int | 初始掉落堆叠数量 |
| 16 | `DropSpriteSize` | int | 地面掉落物图标缩放比 |
| 17 | `FWType` | int | 适用装备类型（0: 武器, 1: 防具, 2: 饰品） |
| 18 | `DropScene` | int | 掉落层数门槛（0: 普通关卡, 1~4: 秘境对应品质深度） |

---

## 7. 说明

### 7.1 数据来源（核心类清单）

- [`BaoshiClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs)：镶嵌物统一数据类，实现 `ItemClass` 与 `IDropItemData`，封装了 `NumberLast` 数值公式与 `GetMain()` 提示文本生成。
- [`WPAocao.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WPAocao.cs)：装备单个凹槽（Socket）运行时数据结构。
- [`WeaponBaoshiApplyUtil.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs)：镶嵌核心分流器，提供 `TryApply`，集中处理 6 类镶嵌物对装备数据结构的注入与校验逻辑。
- [`WeaponClass.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs)：装备主体类，管理插槽列表 `Aocao`、插槽计数 `AocaoCount`、底缀符文 `FW_Base` 与特效列表 `SPC`，负责在穿戴时将属性应用至玩家。
- [`UI.Managers/BaoshiManager.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/UI.Managers/BaoshiManager.cs)：宝石加工管理中枢，控制 5 合 1 升级合成以及 4 类物品（宝石/天赋符文/特效符文/基础属性符文）的拆卸提取流程。
- [`Core.Settings/BaoshiSettings.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Core.Settings/BaoshiSettings.cs)：宝石合成消耗数量（`needCount = 5`）、各品质合成价格与拆卸价格的全局 ScriptableObject 配置。
- [`BaoshiPrice.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiPrice.cs)：各品阶（0~12）物品基础价值映射字典。
- [`AocaoPrice.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/AocaoPrice.cs)：装备插槽数量（1~6 孔）带来的附加价值字典。
- [`Data.SaveData/SaveDataEquipmentSanitizer.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Data.SaveData/SaveDataEquipmentSanitizer.cs)：存档装备数据校验器，包含插槽属性字段映射表 `GemFloatFields`、`FwBaseFloatFields` 与 `FwBaseIntFields`。

### 7.2 表格列含义

- **镶嵌物名称**：中英文双语显示，中文取自官方本地化映射表 `Item_FY`，英文为 CSV 原始 `ItemName`。
- **类型/等级**：分类标签及品阶（如普通宝石 0~7 级、符文 T1~T4 等级）。
- **效果与数值**：生效的具体属性字段及加成数值，普通宝石详列全部 5 种装备部位的差异化效果。
- **适用部位/插槽**：该物品可作用的装备部位（武器、头、胸、手、腿、饰品等）及槽位类型。
- **获取方式**：掉落场景条件（普通地牢或秘境层数 `DropScene`）、合成配方及金币消耗。
- **代码位置**：核心数据定义、应用逻辑及 GlobalID。
- **备注**：拆卸费用、基础售价、主词缀映射索引或特殊规则。

### 7.3 修改注意事项

1. **数值修改入口**：
   - 宝石/符文基础数值直接修改 TextAsset 数据表 `0 2 Baoshi.txt` 中的 `Number` 列。
   - 宝石合成消耗比例修改 [`BaoshiSettings.needCount`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Core.Settings/BaoshiSettings.cs#L9)；合成/拆卸金币消耗修改 `createPrice1~7` 和 `splitPrice0~7`。
   - 玩家宝石增幅公式修改 [`BaoshiClass.NumberLast`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/BaoshiClass.cs#L41) 与 [`WeaponClass.GetSocketedGemNumber`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponClass.cs#L2837-L2845)。
2. **本地化同步（Localization）**：
   - 若在 `0 2 Baoshi.txt` 中添加或重命名 `ItemName`，必须在 `resources/res://Localization/Item_FY.json` 中添加对应的多语言条目（尤其是 `ChineseS` 与 `English`），否则 UI 显示将回退为原始 Key。
3. **存档清理器同步（SaveDataEquipmentSanitizer）**：
   - 若新增或修改符文属性类型（`FW_Base.type`）或宝石属性映射（`WPAocao.Type` 0~25），**必须**同步更新 [`SaveDataEquipmentSanitizer.cs`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/Data.SaveData/SaveDataEquipmentSanitizer.cs) 中的 `GemFloatFields`、`FwBaseFloatFields` 与 `FwBaseIntFields`，否则在读写存档时新增/修改的词缀会被判定为非法数据并被自动剔除（Pruned）。
4. **插槽上限与打孔规则**：
   - 装备最大插槽数 `MaxAocaoCount` 受物品尺寸（`SizeX * SizeY`）限制。若要支持更大插槽数，需同步调整道具的占用格子或修改 `TryAddSocket` 中的尺寸约束逻辑。

### 7.4 未覆盖与存疑项说明

1. **附魔与洗炼石（Stone_FM / Stone_HD / Stone_XL / Stone_CL）**：这 4 种石头虽在 `0 2 Baoshi.txt` 中归类为 `UseType = 2`（功能石），但在代码中它们不由 [`WeaponBaoshiApplyUtil.TryApplyStone`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/WeaponBaoshiApplyUtil.cs#L269-L311) 直接拖拽到装备上消耗，而是作为消耗品在单独的锻造重铸/附魔面板中作为材料扣除。
2. **动态生成符文的属性浮动**：属性符文在实际掉落时（[`ItemManager.DropBaseFW`](file:///C:/GAME-AnYingDiLao/MODworkv2/decompiled/ItemManager.cs#L6823-L6856)），其数值会在基础 `Number` 的 `70% ~ 110%` 范围内浮动（`Mathf.RoundToInt(Number * Random.Range(0.7f, 1.1f))`），因此游戏内实际拾取到的符文数值可能略有差异。
