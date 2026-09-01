# MOD 功能清单（以 Game-Later 新原版为唯一基线，逐行重做前快照）

> 基线：`_archive/DELETE-2026-09-01/decompiled_fresh_new`（Game-Later vanilla 92E0120F，895 .cs，未含 MOD）
> 现状：`MODworkv2/decompiled`（69C0D965，含 V1.32 六项 + 品质 bg 部分缺失）
> 快照：`_archive/.../v32_decompile`（0C779D0E，V1.32 七轮）+ `CHANGELOG.md` V1.32 条目

## 全量新增文件（仅 MOD 有，原版无）

| 文件 | 作用 |
|---|---|
| `MODworkv2/decompiled/FxSpriteFactory.cs` | 特效工厂（新增） |
| `MODworkv2/decompiled/InventorySortBar.cs` + `InventorySortMode.cs` | 背包排序条（新增） |
| `MODworkv2/decompiled/PoeItemMod.cs` | POE 道具与关卡注入主模块（新增） |
| `MODworkv2/decompiled/SkillTagSystem.cs` | 技能标签（新增） |
| `MODworkv2/decompiled/PoedbMod/`（22 文件，`AffixTierDisplay/PoedbMod/CraftBenchOps/CraftBenchUI` 等） | POE 模块 |

小计 6 项（+ PoedbMod 目录 22 文件）。

## 增量补丁（在原版 .cs 上打补丁，需保留原版功能）

> 按 `diff decompiled_fresh_new vs 69C0D965` 中含 MOD 标记的 37 增量文件归类（去 ILSpy 噪音后）：

- 缀模组：`WeaponClass.cs`（词条档位显示等）+ `BaoshiClass.cs`/`UseItemClass.cs`（PoeItemMod 注入）
- 发射与命中：`Gun.cs`（星环总出口等）、`SK_Angle_F.cs`/`SK_FlyA.cs`/`SK_FlyBall.cs`/`SK_FlyFollow.cs`
- 背包与商店：`InventoryManager.cs`、`ShopManager.cs`、`TalentManager.cs`、`GameUIManager.cs`
- 存档：`Data.SaveData/WeaponSaveData.cs` + `SaveDataEquipmentSanitizer.cs`、`ItemManager.cs`（掉落与缀池）
- 系统：`PlayerManager.cs`（额外弹幕计数 `BS_ExtraProjectiles`）、`ACTbar.cs`（Count_F 增量）、`UI.Panels/WeaponManager.cs`

以上清单即“以 fresh_new 为唯一基线，逐行重做”时需逐个最小补丁重放并每类跑 `full_serialize_scan.py` + `dotnet build 0 error` 的边界。

