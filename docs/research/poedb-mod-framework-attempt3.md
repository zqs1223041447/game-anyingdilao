# POEDB 融合 MOD 框架 — Attempt3 (V1.9 游侠双技能实装)

> 日期：2026-08-27 | 版本：V1.9 | 执行：Fixer (muse-spark-1.2) | 任务：为游侠新增一级可学“龙卷射击(Tornado Shot)”与“旋风斩(Cyclone)”

---

## 1. 调研结论

### 1.1 游侠 Xi 与系映射
- `SkillTagSystem.cs:17-21` XiIndexNames: `[0]Hell Messenger ... [6]Windwalker/风之游侠 [7]Doomsday Disciple [8]High Elf ...`
- `game-systems-ref/05-skills.md` 实证：Xi6 为风之游侠，含 Razor Arrow (Unlock 0) / Lethal Dart / Barrage / Gale Arrow 等，已验证 PLType=2 时开放 Xi6/7/8 三页 (`TalentManager.SetStart` 按 `PL.PLType*3` 范围)。
- 选择 Xi=6 作为两技能容身系：弓系主系、技能数相对最少、与模板 Razor Arrow 同系易复用图标与 OBJ；注释中已说明“若页拥挤可迁移至 Xi=7 同游侠系”以满足后续扩展。

### 1.2 一级可学判定字段
- `LoadData_SampleF` 71 行结构： `IndexName(1) / icon(2+5) / Price(6) / UnLock_Point(7) / Xi(8) / Level_Max(9) / Info(10) / SonA-C / ... / FStype(UseAni后) / damageType / Damage_Base / Damage_Level / ManaCost_Base / CoolDown_Base / ... / CountMulti / ... / Size / AngleA / Follow_F / AllChuan_F 等`
- 一级可学关键：`UnLock_Point=0`（系内0点门槛）+ `Price=0`（无需技能点价格）+ `Level_Max=4`（与本系 Razor Arrow 一致）+ `Xi=6`。`XiData[xi].Level_Base` 初始 0 时 `Refresh(xi)` 判定 `UnLock_Point <= Level_Base` 即 Unlock。
- `Skill_FY` 本地化键：`Info` 列如 `info_Tornado Shot` → `LOC.MM.GetSkill(Info)` 读 `resources.assets` Skill_FY JSON；缺键时 `LOC.Get` 回退返回 key 本身，保证不空白。

### 1.3 模板与飞行
- Tornado Shot：模板 `Razor Arrow` (Xi6/OBJ38)，FStype=7 环绕/散射，CountMulti=6 二次6弹，AllChuan_F=0 穿透。复用 `SK_FlyA` 投射物；二次散射 Tier2 可在 `SK_FlyA.TimeStop`/`OnTriggerEnter` 中二次 Spawn。
- Cyclone：模板 `Cleave` (刃系，Xi3 常见)，若 Xi6 内无 Cleave 则回退首个 Sample_F（Razor Arrow），override 为 FStype=7 环绕/附着玩家（`Gun.ARCattack case7` 中 `SetParent(pl.transform)`），Size 1.2 / Range1 2.5 模拟近战环绕范围；Tier2 可复用 `SK_Round`/`SK_Sword` 持续光环或 `SK_FlyA` 定时伤害，注释已在 `PoedbSkillInjector` 顶部说明。

---

## 2. 数据：data/poedb/skills.json 双条目

```json
{
  "id": "tornado-shot",
  "name": "Tornado Shot", "name_zh": "龙卷射击",
  "tags": ["Attack","Projectile","Bow"],
  "shadow_dungeon_mapping": {
    "template_index_name": "Razor Arrow",
    "index_name": "Tornado Shot",
    "info_key": "info_Tornado Shot",
    "column_overrides": { "FStype":"7","CountMulti":"6","Damage_Base":"100","Damage_Level":"3","ManaCost_Base":"8","CoolDown_Base":"1.2","AllChuan_F":"0" }
  }
},
{
  "id": "cyclone",
  "name": "Cyclone", "name_zh": "旋风斩",
  "tags": ["Attack","Area","Melee"],
  "shadow_dungeon_mapping": {
    "template_index_name": "Cleave",
    "index_name": "Cyclone",
    "info_key": "info_Cyclone",
    "column_overrides": {
      "Xi":"6","Price":"0","UnLock_Point":"0","Level_Max":"4",
      "FStype":"7","CountMulti":"1","Damage_Base":"80","Damage_Level":"5",
      "ManaCost_Base":"6","CoolDown_Base":"0.5","AllChuan_F":"1","Follow_F":"1","Size":"1.2","Range1":"2.5"
    }
  }
}
```
- Xi=6，InfoKey 分别 `info_Tornado Shot` / `info_Cyclone`，一级可学（Price 0 / Unlock 0）。
- `tools/poedb-pipeline/seed_data.py` 同步新增 `CYCLONE` 常量，`build_all()` 现输出 2 条，`manifest.json` item_count=2。
- 校验：`python schema.py validate` → PASS，tornado-shot 完整示例仍可查，skills 2 items valid。

---

## 3. 框架/代码

### 3.1 运行时注入（选项A 代码注入优先）
- 新文件 `MODworkv2/decompiled/PoedbMod/PoedbSkillInjector.cs`（280+行）：
  - `InjectIfNeeded(TalentManager)` 幂等，克隆模板 `SkillData_Sample_Father`（反射拷贝全部 public 字段+属性，skillbt 置空），override 指定列，写入 `XiData[6].Sample_F`，登记 `SKI` 与 `FW`（`SKFW_Group`）。
  - 模板查找：优先 Xi6 内 `Razor Arrow`/`Cleave`，回退任意 Xi6 首项，再跨 Xi 搜索，防模板缺失。
  - 本地化 fallback：反射 `LOC._table` 注入 `Skill_FY.Tornado Shot/Cyclone` 与 `info_Tornado Shot/info_Cyclone` 的中英（`LanguageType.English/ChineseS/ChineseT`），保证 tooltip 不空白；并说明正式合并到 `resources.assets path_id=433` 的流程（见下）。
- 挂钩：`TalentManager.LoadTalentTables()` 尾部（`_talentTablesLoaded=true` 后）直接 `PoedbSkillInjector.InjectIfNeeded(this)`，已加载场景二次调用亦补注；`SetStart` 的 `Refresh` 天然刷新解锁。

### 3.2 标签
- `SkillTagSystem.cs`：BoomerangWhitelist 保留 `Ice Crystal`，注释新增 Tornado/Cyclone 非回旋形态由 FStype7=环绕推导；标签结果：Tornado → ◆风之游侠 + ◇环绕·穿透·多弹，Cyclone → ◆风之游侠 + ◇环绕（Size 1.2 辅助）。
- `PoedbMod/Registry.cs`：`CollectPoedbTags` 重写为双技能分流——Tornado (CountMulti>1 & FStype7/8) 显示 `POEDB: Attack, Projectile, Bow` + 中文描述；Cyclone (FStype7 & CountMulti==1) 显示 `POEDB: Attack, Area, Melee` + 中文描述；避免污染普通弓箭。

### 3.3 飞行
- Tornado：沿用 `SK_FlyA` 回旋白名单扩展点 `Registry.IsBoomerangSkill`（当前 Tags 无 Returning 故不回旋，保留接口），散射为列覆盖 CountMulti 6，Tier2 可扩展二次 360° 散射。
- Cyclone：FStype 7 附着玩家（`Gun case7 SetParent`），环绕持续伤害；代码注释指明 Tier2 可复用 `SK_Round`/`SK_Sword` 或 `SK_FlyA` 定时伤害实现真·旋风光环。

### 3.4 本地化
- **运行时 fallback**：`PoedbSkillInjector.InjectLocalizationFallback()` 已保证游戏内 tooltip 有中文（见 LOC._table 注入）。
- **资源正式合并流程（说明）**：`localization.json`（`info_Tornado Shot`/`info_Cyclone`）→ 用 `AssetTools.NET` 或 `PocCsvRow` 类工具导出 `resources.assets` 的 `Skill_FY` TextAsset (path_id=433)，合并键后写回，备份 `resources.assets` 后部署；本版为免改 resources.assets 的可玩 DLL 实装，已用代码侧 fallback 兜底，校验细则见 `docs/skill-spec.md 描述同步要求`。

---

## 4. 工具链

- `tools/poedb-pipeline/nl-pack.py`：`_match_known_name` 新增旋风/cyclone 兜底，`_build_samplef_row` 已支持 Xi/Price/UnLock 等覆盖。
- 执行：
  ```
  python tools/poedb-pipeline/seed_data.py --out data/poedb  # 重新物化，全部带 BOM
  python tools/poedb-pipeline/schema.py validate            # [PASS] 2 items valid
  python tools/poedb-pipeline/nl-pack.py --list             # tornado-shot, cyclone 均可见
  python tools/poedb-pipeline/nl-pack.py --skill tornado-shot
  python tools/poedb-pipeline/nl-pack.py --skill cyclone
  python tools/poedb-pipeline/nl-pack.py "参考POEDB增加龙卷射击技能"
  python tools/poedb-pipeline/nl-pack.py "参考POEDB增加旋风斩技能"
  ```
- 产物：
  - `builds/packs/tornado-shot/` 7 文件（含 `samplef_row.csv`: `Tornado Shot,info_Tornado Shot,6,0,0,20,0,7,...`）
  - `builds/packs/cyclone/` 7 文件（含 `samplef_row.csv`: `Cyclone,info_Cyclone,6,0,0,4,0,7,physics,80,5,6,0.5,1,1,10,1.2,0` + `localization.json` info_Cyclone）

---

## 5. 构建与部署

- `dotnet build MODworkv2/decompiled/Assembly-CSharp.csproj -c Release` → **0 error, 123 warnings**（存量）。
- 备份确认：`MODworkv2/backup/Assembly-CSharp-original.dll` (2,313,728 B) 为原版。
- 部署：`bin/Release/netstandard2.0/Assembly-CSharp.dll` → `ShadowDungeon/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll`
  - 构建 SHA256：`F14927CEBFA13BCC2F6705F6C5F775D62392ABAE731FC63B9EA19CCB738EDC6E`
  - 目标 SHA256（覆盖后）：`F14927CEBFA13BCC2F6705F6C5F775D62392ABAE731FC63B9EA19CCB738EDC6E` 一致。
- 启动验证：进程存活 **≥35秒 PASS**，`%USERPROFILE%\AppData\LocalLow\OO Cat\Shadow Dungeon\Player.log` 中 `Exception=0 Crash=0 TypeLoad=0 NullReference=0` PASS。
- 资源：`resources.assets` 未改动（原版），本地化走代码 fallback；若需正式合并需备份该文件。

---

## 6. 版本登记

- `CHANGELOG.md` V1.9 条目已登记（日期 2026-08-27，涉及文件、SHA、验证与部署状态）。

---

## 7. 游戏内学习路径

1. 新建角色选 **游侠系**（任意开局 PLType=2 对应 Xi6-8），进入游戏后按天赋快捷（默认 `P` 或底部 `天赋`）。
2. 天赋页签切换至 **风之游侠 (Windwalker, Xi6)**——首行即显示 **龙卷射击 (Tornado Shot)** 与 **旋风斩 (Cyclone)**，图标复用 Razor Arrow/Cleave，Info tooltip 为注入的中英文（FStype 7 环绕等）。
3. 两技能 `UnLock_Point=0` 且 `Price=0`，**1级即可加点**（有剩余技能点时点击图标 +1，`P_Used` 增加，按钮解锁为彩色，可施放）。
4. 加点后自动进入快捷栏（`ACTbar.AddSkillListSlotSP`），或打开技能列表拖至 1-8 快捷键，`Gun.CreatSP` 按 FStype 7 环绕形态施放。

---

## 8. 后续扩展建议

- 在 `SK_FlyA` 中实现 Tornado 二次散射（`CountMulti 6` → 末段 `TimeStop` 时向 360° Spawn 6 次级弹）。
- 为 Cyclone 增加持续伤害 Tick（参考 `SK_Round` 持续光环，每 0.2s `EM_Set` 范围伤害），并在 `Gun` 中为 FStype7 增加范围碰撞。
- 将 `LOC` 注入改为 `AssetTools.NET` 合并 `resources.assets` path_id=433 的持久化流程，出正式资产包。
- 扩展 `data/poedb/skills.json` 至 POEDB 全量弓/近战系，复用同 Injector 模板。
