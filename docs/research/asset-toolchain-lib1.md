# 调研记录：Unity 资产修改工具链（lib-1）

> 日期：2026-08-23 ｜ 调研人：librarian (lib-1) ｜ 状态：已完成
> 原始任务：为「新增技能树节点」与「新视觉/动画/音效」评估可行工具链

## 关键前提（已核实）

- 传统非 Addressables 布局：`resources.assets` + `sharedassets0-2.assets` + `level0-2`，无 `StreamingAssets/aa`
- FMOD **2.2.24**（fmodstudio.dll FileVersion），bank 位于 `StreamingAssets/Desktop/*.bank`，含 `Skill.bank`
- spine-csharp **4.0.30**（spine-unity 4.0 运行时，官方支持 Unity 2019.4）
- Managed 全套 DLL 在位（Mono 游戏资产编辑最大利好：type tree 可从 DLL 反推）

## 1. UABEA / UABE

- 原 UABE 已归档（2025-10）；活跃线为 **UABEANext**
- TextAsset 编辑：✅ 最成熟用法，Export Dump → 改文本 → Import Dump，**长度可变**
- 新增资产条目：✅ 可行但繁琐（File->Add 或 AssetsTools.NET 脚本）；Resources.Load 资产须同步登记 ResourceManager.m_Container
- Unity 2019.4：✅ classdata.tpk 覆盖
- 坑：type tree 缺失靠 DLL 反推（我们有全套 DLL ✅）；CRC 仅影响 Addressables（本游戏无 ✅）；跨文件 PPtr 手工重指

## 2. AssetRipper

- 活跃维护，支持 2019.4；**单向工具（成品→工程），不能回写**
- 定位：只读检查器/素材导出器；整包重建不推荐（FMOD bank 不导出、shader 易损、差异多）

## 3. 纹理/Sprite 替换

- 正确姿势：编辑 Sprite 底层 **Texture2D**（Sprite 本体不可直接导入）
- **同尺寸替换最稳**：改尺寸破坏 rect/UV/mipmap；改名断引用
- 散装 Sprite 换像素不断引用；SpriteAtlas 换页内容可行但不改尺寸
- 注意同一贴图多文件副本问题

## 4. FMOD 音频

- bank 内只能**等长/更短替换**现有音效（社区工具：IZH318 FSB-BANK-Extractor-Rebuilder 最新最全）
- 无原工程不能新增事件
- 绕行路线一（推荐）：ModAudioManager + `UnityWebRequestMultimedia.GetAudioClip("file://...")` 播外部 ogg/wav（游戏带 UnityWebRequestAudioModule ✅）
- 绕行路线二：免费版 FMOD Studio 2.2.x 自建 Mod.bank + 代码 `RuntimeManager.LoadBank("Mod")`（初始化时序需实测）

## 5. Spine 动画

- 运行时可加载新骨架（CreateRuntimeInstance 公开 API）；atlas 是纯文本
- 但 `.skel.bytes` 二进制严格绑版本、无成熟手写工具；手写 JSON 动画工作量不现实
- **实践结论：只能复用现有动画**（AnimationState 混合/排队/换肤零门槛；程序化拼简单 Timeline 中等门槛）
- 全新角色动画需要 Spine Editor 授权（付费）→ 判定为墙

## 6. BepInEx / Harmony

- **BepInEx 5.4.23.x（LTS）** + HarmonyX，Unity 2019.4 Mono 完全成熟（win_x64 包）
- 坑：emit 报错时设 `[Preloader] HarmonyBackend = cecil`
- 能力天花板：不需要新序列化资产的都能做——新 MonoBehaviour/AddComponent、UI 克隆伪造列表、Icon 复用/运行时 Sprite.Create
- 与重编译 Assembly-CSharp 路线**不互斥**，可共存

## 7. 推荐路线

### (A) 新增技能树节点
| 推荐度 | 路线 | 工作量 |
|---|---|---|
| ★★★ | A1：UABEA 直接改 CSV TextAsset（加行） | 小（小时级） |
| ★★★ | A2：BepInEx 运行时追加节点+克隆 UI 行 | 中（数天） |
| ★★ | A3：AssetsTools.NET 脚本新增 TextAsset+容器登记 | 中 |

建议 A1 起步验证数据链路，复杂行为叠加 A2。不上 AssetRipper 整包重建。

### (B) 新增视觉/动画/音效
| 推荐度 | 路线 | 工作量 |
|---|---|---|
| ★★★ | B1：UABEA 同尺寸纹理替换 | 小 |
| ★★★ | B2：代码绕过 FMOD 播外部音频 | 小-中 |
| ★★☆ | B3：自建 Mod.bank 加载 | 中 |
| ★★☆ | B4：Spine 复用+程序化混合 | 小-中 |
| ★ | B5：全新 Spine 动画/bank 结构修改 | 大/不可行 |

## 不确定项

1. FMODUnity 运行时追加加载自定义 bank 的时序兼容性未实测
2. 游戏 Spine 骨架是 .json 还是 .skel.bytes 未逐一核对
3. 社区 FMOD 工具对 2.2.24 Vorbis bank 重建成功率因工具而异
