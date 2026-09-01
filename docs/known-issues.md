# 已知问题 (Known Issues)

> 已确认、暂缓处理的问题登记。修复时迁移到 worklog 并从此处删除。

## KI-001 ArcBoomerang 返回点与射出点偏差约 45°
- **现象**：箭矢回旋时不是回到发射点，收回点与射出点相差约 45°
- **根因推断**：`StartReturn()` 的追踪目标是 `PlayerManager.yao`（角色视觉锚点 "main/FX yao"），而非武器发射点 `Gun.ARCpointA`；两者天然存在角度差
- **影响**：纯观感，不影响功能与平衡
- **修复方向**：StartReturn 的 target 改为记录发射时的 `ARCpointA.position`（生成时缓存），或直接引用 Gun 发射点 Transform
- **优先级**：低（用户确认不急）

## KI-002 ArcBoomerang 缺本地化键
- **状态**：处理中（2026-08-24 desc-sync 车道：Skill_FY 补 `info_ArcBoomerang` 键 + 剃刀箭文案同步，staging 产出见 `modwork/asset-inventory/desc-sync/`，待部署）
- **现象**：tooltip 显示英文原文 `ArcBoomerang` 并伴随 Warn 日志
- **修复方向**：向 resources.assets 的 Skill_FY JSON（path_id=433）追加 `info_ArcBoomerang` 键；或后续 UABEA 编辑
- **优先级**：低

## KI-003 开发 VM HomeScene 原生崩溃（环境限制）
- **现象**：本 VM（Hyper-V/WARP 软渲染/8GB）加载 HomeScene 时 UnityPlayer 原生崩溃（0x80000003）
- **定界结论**：预存问题——最小无注入构建亦复现；真实机器（RTX 5070）同文件正常进 Play（V3 验收已证）
- **影响**：本 VM 只能做菜单态验证；场景级验收必须在真实机器执行
- **优先级**：不再追修（有真实机器验收通道）

## KI-004 游戏资产源头中文损坏
- **现象**：Xi/Baoshi/UseItem 等 CSV 的中文列在资产内部已是 U+FFFD（开发者导入期 GBK→UTF-8 有损转换）
- **修复方向**：不可逆；显示层恢复走 Skill_FY 本地化 JSON
- **优先级**：无（历史数据，绕过即可）
