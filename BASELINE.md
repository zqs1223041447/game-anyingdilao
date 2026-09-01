# Baseline — Game-Later 新原版唯一基线（回归用）

> 此文件记录重做前的基线与清单，push 到 github 供随时回归。

- **Game-Later vanilla DLL**：`Game-Later/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll` `92E0120F...2D52` 2352640B
- **Game-Later 资产**：`level1 67d86d7b...dedbb908` / `resources d9948ac3...` / `globalgamemanagers 33d0679f...`
- **纯净反编译基线**：`_archive/DELETE-2026-09-01/decompiled_fresh_new` 895 .cs（已修复 ILSpy array[^1] + RefSafetyRules）
- **当前 MOD 源码**：`MODworkv2/decompiled` 69C0D965（待按清单逐行重做）
- **V1.32 含 MOD 七轮快照**：`_archive/.../v32_decompile` 0C779D0E（P0 六项来源）
- **清单**：见 `docs/mod-feature-inventory.md`（全量 6 增量 37）
- **重做规则**：以 `decompiled_fresh_new` 为唯一基线，逐个最小补丁重放，每类跑 `full_serialize_scan.py 895 全部一致` + `dotnet build 0 error`，`Game-Later` 全程只读
