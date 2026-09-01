# POE 投射物技能数据调研（POEDB.TW → 暗影地牢移植基线）

> 日期：2026-08-24 ｜ 车道：lib-1（@librarian）｜ 状态：**定稿**
> 用途：loop-mt6011d6-xq9b51「外部技能描述添加投射物技能」能力测试的数据规格来源。

## 数据基准

PoE 1 **v3.29.0**（POEDB.TW 实时镜像游戏文件）。

## 获取途径

| 优先级 | 来源 | URL | 格式 |
|---|---|---|---|
| ★ 数值权威 | RePoE (repoe-fork) | github.com/repoe-fork/repoe · repoe-fork.github.io/poe1.html | JSON `gems.json` |
| ★ 繁中文本 | POEDB.TW 页面 | `poedb.tw/tw/{英文名下划线}` | HTML |
| ○ 交叉验证 | PoB 社区版 / pob-data JSON | github.com/PathOfBuildingCommunity/PathOfBuilding · repoe-fork.github.io/pob-data/poe1/Skills/*.json | Lua/JSON |

RePoE 字段要点：`damage_effectiveness` 0=100%（存增量）；`crit_chance` ÷100；`cast_time`/`cooldown` 毫秒；繁中文本需从 POEDB.TW 抓取后按内部 ID 对齐。

## 三技能数值表（Lv1→Lv20）

### 1. 火球 Fireball（`SkillGemFireball`）
- 描述：「釋放一顆球型火焰向前飛射，接觸到怪物時會爆炸並對周圍敵人造成傷害」
- 标签：投射物/法术/范围效果/火焰
- 魔耗 5→20；施放 0.75s；暴击 5%；伤害效能 300%→480%
- 弹速 10.4 m/s；基础伤害 9–14 → 1883–2825 火焰
- **AoE 半径 1.0→1.8 m，命中即爆炸，无穿透**
- **点燃：25% 机率，按技能基础火焰伤害持续 4 秒**

### 2. 冰矛 Ice Spear（`SkillGemIceSpear`）
- 描述：「快速連續地發射冰之碎片。碎片在飛行一小段距離後會轉化為第二型態，此階段碎片飛行速度更快且能穿透敵人。」
- 标签：暴击/法术/投射物/冰冷
- 魔耗 8→23；施放 0.70s；暴击 7.5%；效能 130% 固定
- 投射物 ×2；弹速 10 m/s → 第二形态 ≈40 m/s（+300% more）
- 基础伤害 22–33 → 568–852 冰冷；无 AoE
- **形态机制：飞行一段距离转二形态：+600% 暴击率、+(30→49)% 暴击加成、无限穿透**
- 无内建冰缓词缀（冰冷击中按通用规则天然冰缓）

### 3. 闪电箭矢 Lightning Arrow（`SkillGemLightningArrow`）
- 描述：「射出一發充滿閃電能量的箭矢，箭矢擊中敵人時會造成電擊，同時電擊附近一群敵人。」
- 标签：攻击/范围效果/投射物/闪电/弓箭
- 魔耗 6→10；攻速随弓；效能 149.5%→177.1%（纯 % 武器伤害，无固定点伤）
- 弹速 32.6 m/s；AoE 半径 18（内部单位溅射圈）
- **溅射：击中目标附近最多 3 个额外敌人；50% 物理转闪电**
- **感电：对被感电敌人伤害如同 (100→290)% more**

## 移植映射备忘（结合本游戏）

| POE 机制 | 本游戏落点（待 exp-1 锚点确认） |
|---|---|
| 直飞弹体+命中爆炸 AoE | SK_Fly* 命中 EXP/SubA/SubB 溅射链 |
| 点燃 DOT | SetDot → DOT_MG.AddDot → TakeBoomDie |
| 冰缓减速 | Buff 系统（入口类待确认） |
| 穿透/多段命中 | SK_Fly* 命中列表 em / 不终止分支 |
| 音效 | FMOD RuntimeManager.PlayOneShot("event:/...") 字符串 |
| 特效索引 | SKPB.Skill[OBJ].OBJ[MainEL] / HitFX / ATFX |

## 来源清单

1. https://poedb.tw/tw/Fireball ｜ https://poedb.tw/tw/Ice_Spear ｜ https://poedb.tw/tw/Lightning_Arrow
2. https://github.com/repoe-fork/repoe （docs/gems.md 字段定义已核对）
3. https://github.com/PathOfBuildingCommunity/PathOfBuilding （dev 分支 act_int/act_dex.lua）
4. https://github.com/aianlinb/LibGGPK3 （台服翻译表解包，备选）

精确度：全部数值来自 POEDB.TW 当前线上数据（=游戏文件 v3.29.0），非社区估算。
