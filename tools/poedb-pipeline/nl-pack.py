# -*- coding: utf-8 -*-
"""
nl-pack.py — 自然语言快速制作更新包 CLI
=========================================

输入自然语言指令（如「参考POEDB增加龙卷射击技能」），自动：
  1. 读取本地持久化数据（data/poedb/*.json）
  2. 解析指令 → 匹配技能/词缀/天赋
  3. 生成技能定义 / CSV 行 / 资源补丁（本地化键）
  4. 输出到 builds/packs/<name>/ 的更新包雏形

用法：
    python tools/poedb-pipeline/nl-pack.py "参考POEDB增加龙卷射击技能"
    python tools/poedb-pipeline/nl-pack.py --list
    python tools/poedb-pipeline/nl-pack.py --skill tornado-shot

输出结构（builds/packs/<name>/）：
    pack.json              更新包元数据（含 SHA256，UTF-8 BOM 以兼容 PowerShell）
    skill_definition.json  技能定义（映射到 Shadow Dungeon 数据模型）
    samplef_row.csv        SampleF CSV 追加行（列覆盖后的完整行）
    localization.json      Skill_FY 本地化键（info_<name> 中英）
    README.md              更新包说明

编码说明（本次修复）：
    - 全部 JSON/MD/CSV 写入使用 utf-8-sig（带 BOM），确保 PowerShell / 记事本自动识别为 UTF-8，
      彻底解决 PowerShell GBK 控制台下 Get-Content 显示为 ?? 的问题。
    - 读取全部使用 utf-8-sig（兼容有/无 BOM）。
    - 控制台输出强制 utf-8 reconfigure，中文在 chcp 936/65001 均正常。
    - pack.json 的 command 字段保留原始指令原文（显式 utf-8-sig 落盘，不经 ascii 转义）。
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import sys
from datetime import datetime, timezone

# 允许直接运行或作为模块导入
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

# Windows 控制台默认 cp1252/gbk 会导致中文打印异常，强制 UTF-8
try:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    if hasattr(sys.stderr, "reconfigure"):
        sys.stderr.reconfigure(encoding="utf-8", errors="replace")
    # stdin 也尝试 utf-8，确保管道中文参数不丢
    if hasattr(sys.stdin, "reconfigure"):
        sys.stdin.reconfigure(encoding="utf-8", errors="replace")
except Exception:
    pass

# 工作区根目录（本文件位于 tools/poedb-pipeline/ 下）
ROOT = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
DATA_DIR = os.path.join(ROOT, "data", "poedb")
PACKS_DIR = os.path.join(ROOT, "builds", "packs")

# 写入编码：utf-8-sig = 带 BOM 的 UTF-8，读取自动去 BOM
WRITE_ENCODING = "utf-8-sig"
READ_ENCODING = "utf-8-sig"

# ---------------------------------------------------------------------------
# 指令解析
# ---------------------------------------------------------------------------
# 关键词 → 动作类型
ACTION_KEYWORDS = {
    "技能": "skill",
    "skill": "skill",
    "词缀": "affix",
    "mod": "affix",
    "天赋": "talent",
    "passive": "talent",
    "工艺": "crafting",
    "craft": "crafting",
    "辅助": "support",
    "support": "support",
    "地图": "map",
    "map": "map",
    "敌人": "enemy",
    "monster": "enemy",
    "装备": "equipment",
    "equipment": "equipment",
}


def _fix_mojibake(text: str) -> str:
    """尝试修复 PowerShell GBK → Python 误解码导致的乱码（防御性）。

    场景：用户在 chcp 936 终端输入中文，Python 若以 cp1252 解码会得到 mojibake。
    本函数检测常见乱码特征并尝试通过 gbk/utf-8 往返修复；失败则原样返回。
    """
    if not text:
        return text
    # 若已包含正常中文，直接返回
    if any("\u4e00" <= ch <= "\u9fff" for ch in text):
        # 检测是否同时含有 replacement 或 ?? 片段，尝试轻量修复
        if "??" not in text and "\ufffd" not in text:
            return text
    # 常见 mojibake 修复尝试：先按 latin1 取 bytes，再按 gbk/utf-8 解码
    for enc in ("gbk", "utf-8", "cp936"):
        try:
            raw = text.encode("latin1", errors="ignore")
            cand = raw.decode(enc)
            if any("\u4e00" <= ch <= "\u9fff" for ch in cand):
                return cand
        except Exception:
            continue
    return text


def parse_command(text: str) -> dict:
    """解析自然语言指令，返回 {action, target, raw}。"""
    raw = text
    text = _fix_mojibake(text)
    text_lower = text.lower()
    action = None
    for kw, act in ACTION_KEYWORDS.items():
        if kw.lower() in text_lower:
            action = act
            break
    if action is None:
        action = "skill"  # 默认按技能处理

    # 提取目标名：优先匹配本地数据中的已知技能名
    target = _match_known_name(text)
    return {"action": action, "target": target, "raw": raw}


def _match_known_name(text: str) -> str | None:
    """在本地数据中匹配指令提到的技能/词缀名。"""
    text_fixed = _fix_mojibake(text)
    skills = _load_category("skills")
    for item in skills:
        for key in ("name", "name_zh", "id"):
            val = item.get(key)
            if val and (val.lower() in text_fixed.lower() or text_fixed.lower() in str(val).lower()):
                return item["id"]
    # 兜底：含 龙卷/tornado 则匹配 tornado-shot；旋风/cyclone 则匹配 cyclone
    if "龙卷" in text_fixed or "tornado" in text_fixed.lower():
        return "tornado-shot"
    if "旋风" in text_fixed or "cyclone" in text_fixed.lower():
        return "cyclone"
    return None


# ---------------------------------------------------------------------------
# 数据加载
# ---------------------------------------------------------------------------
def _load_category(category: str) -> list:
    path = os.path.join(DATA_DIR, f"{category}.json")
    if not os.path.exists(path):
        return []
    with open(path, "r", encoding=READ_ENCODING) as f:
        data = json.load(f)
    return data.get("items", [])


def load_skill(skill_id: str) -> dict | None:
    for item in _load_category("skills"):
        if item.get("id") == skill_id:
            return item
    return None


# ---------------------------------------------------------------------------
# 更新包生成
# ---------------------------------------------------------------------------
def _sha256(path: str) -> str:
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()


def _write_json_bom(path: str, obj) -> None:
    with open(path, "w", encoding=WRITE_ENCODING) as f:
        json.dump(obj, f, ensure_ascii=False, indent=2)
        f.write("\n")


def _write_text_bom(path: str, text: str) -> None:
    with open(path, "w", encoding=WRITE_ENCODING, newline="\n") as f:
        f.write(text)


def _build_samplef_row(skill: dict) -> str:
    """根据技能定义的 shadow_dungeon_mapping 生成 SampleF CSV 追加行。

    说明：SampleF 表头约 150 列。此处生成「列覆盖 + 模板继承」的完整行，
    未覆盖列以模板值占位（实际部署时由 SkillForge 从模板行克隆补齐）。
    """
    mapping = skill.get("shadow_dungeon_mapping", {})
    overrides = mapping.get("column_overrides", {})
    # 关键列顺序（与 SkillData_Sample_Father 字段对应）
    columns = [
        "IndexName", "Info", "Xi", "Price", "UnLock_Point", "Level_Max",
        "UseAni", "FStype", "damageType", "Damage_Base", "Damage_Level",
        "ManaCost_Base", "CoolDown_Base", "CountMulti", "AllChuan_F",
        "Follow_F", "FlySpeed_Base", "Size", "AngleA",
    ]
    defaults = {
        "IndexName": mapping.get("index_name", skill.get("name", "")),
        "Info": mapping.get("info_key", f"info_{skill.get('name', '')}"),
        "Xi": "6",            # 弓系
        "Price": "0",
        "UnLock_Point": "0",
        "Level_Max": "20",
        "UseAni": "0",
        "FStype": "7",
        "damageType": "physics",
        "Damage_Base": "100",
        "Damage_Level": "3",
        "ManaCost_Base": "8",
        "CoolDown_Base": "1.2",
        "CountMulti": "1",
        "AllChuan_F": "0",
        "Follow_F": "1",
        "FlySpeed_Base": "10",
        "Size": "1",
        "AngleA": "0",
    }
    defaults.update(overrides)
    return ",".join(defaults.get(col, "") for col in columns)


def generate_pack(skill_id: str, out_dir: str | None = None, raw_command: str | None = None) -> str:
    """为指定技能生成更新包，返回包目录路径。"""
    skill = load_skill(skill_id)
    if skill is None:
        raise ValueError(f"本地数据中未找到技能: {skill_id}")

    name = skill.get("name", skill_id)
    pack_name = re.sub(r"[^A-Za-z0-9_-]", "-", name).strip("-").lower()
    pack_dir = out_dir or os.path.join(PACKS_DIR, pack_name)
    os.makedirs(pack_dir, exist_ok=True)

    mapping = skill.get("shadow_dungeon_mapping", {})
    info_key = mapping.get("info_key", f"info_{name}")

    # 1) skill_definition.json
    skill_def = {
        "schema_version": "1.0.0",
        "pack_name": pack_name,
        "source": "poedb.tw",
        "skill": {
            "id": skill.get("id"),
            "name": name,
            "name_zh": skill.get("name_zh"),
            "tags": skill.get("tags", []),
            "description": skill.get("description"),
            "description_zh": skill.get("description_zh"),
            "level_scaling": skill.get("level_scaling"),
        },
        "shadow_dungeon_mapping": mapping,
    }
    skill_def_path = os.path.join(pack_dir, "skill_definition.json")
    _write_json_bom(skill_def_path, skill_def)

    # 2) samplef_row.csv
    csv_row = _build_samplef_row(skill)
    csv_path = os.path.join(pack_dir, "samplef_row.csv")
    _write_text_bom(csv_path, csv_row + "\n")

    # manifest 列头（便于审计）
    csv_header_path = os.path.join(pack_dir, "samplef_row_header.csv")
    _write_text_bom(csv_header_path, ",".join([
        "IndexName", "Info", "Xi", "Price", "UnLock_Point", "Level_Max",
        "UseAni", "FStype", "damageType", "Damage_Base", "Damage_Level",
        "ManaCost_Base", "CoolDown_Base", "CountMulti", "AllChuan_F",
        "Follow_F", "FlySpeed_Base", "Size", "AngleA"]) + "\n")

    # 3) localization.json（Skill_FY 本地化键，满足描述同步要求）
    loc = {
        "info_key": info_key,
        "localizations": {
            "English": skill.get("description", ""),
            "ChineseS": skill.get("description_zh", ""),
            "ChineseT": skill.get("description_zh", ""),
        },
    }
    loc_path = os.path.join(pack_dir, "localization.json")
    _write_json_bom(loc_path, loc)

    # 4) pack.json（元数据 + SHA256）— command 字段保留原始指令原文，utf-8-sig 落盘
    # 优先使用调用方传入的 raw_command，否则回退为按技能名构造
    command_text = raw_command if raw_command else f"参考POEDB增加{skill.get('name_zh', name)}技能"
    # 确保 command_text 为正常中文（修复可能的 mojibake）
    command_text = _fix_mojibake(command_text)
    pack_meta = {
        "pack_name": pack_name,
        "created_at": datetime.now(timezone.utc).isoformat(),
        "command": command_text,
        "command_raw": raw_command or command_text,
        "skill_id": skill_id,
        "files": {
            "skill_definition.json": _sha256(skill_def_path),
            "samplef_row.csv": _sha256(csv_path),
            "localization.json": _sha256(loc_path),
        },
        "deploy_notes": (
            "1. 用 SkillForge 将 samplef_row.csv 追加到 SampleF（模板继承未覆盖列）\n"
            "2. 将 localization.json 的 info_key 写入 resources.assets Skill_FY\n"
            "3. 重编译部署 Assembly-CSharp.dll（若需新行为走 Tier 2 代码补丁）"
        ),
    }
    pack_path = os.path.join(pack_dir, "pack.json")
    _write_json_bom(pack_path, pack_meta)

    # 5) README.md
    readme = (
        f"# 更新包：{skill.get('name_zh', name)}（{name}）\n\n"
        f"- 来源：POEDB（{skill.get('source_url', '')}）\n"
        f"- 生成时间：{pack_meta['created_at']}\n"
        f"- 技能标签：{', '.join(skill.get('tags', []))}\n\n"
        f"## 描述\n{skill.get('description_zh', '')}\n\n"
        f"## 部署步骤\n{pack_meta['deploy_notes']}\n"
    )
    _write_text_bom(os.path.join(pack_dir, "README.md"), readme)

    # 6) manifest.json（包内清单，聚合校验用）
    manifest_path = os.path.join(pack_dir, "manifest.json")
    _write_json_bom(manifest_path, {
        "pack_name": pack_name,
        "skill_id": skill_id,
        "generated_at": pack_meta["created_at"],
        "files": pack_meta["files"],
        "info_key": info_key,
    })

    return pack_dir


def list_skills() -> None:
    print("Local persisted skill data:")
    for item in _load_category("skills"):
        print(f"  - {item.get('id')}: {item.get('name_zh', item.get('name'))} ({item.get('name')})")


def main() -> int:
    parser = argparse.ArgumentParser(description="自然语言快速制作更新包")
    parser.add_argument("command", nargs="?", help="自然语言指令，如「参考POEDB增加龙卷射击技能」")
    parser.add_argument("--list", action="store_true", help="列出本地技能数据")
    parser.add_argument("--skill", help="直接指定技能 id 生成更新包")
    parser.add_argument("--out", help="输出目录（默认 builds/packs/<name>）")
    args = parser.parse_args()

    if args.list:
        list_skills()
        return 0

    skill_id = args.skill
    raw_cmd = args.command
    if args.command:
        parsed = parse_command(args.command)
        if parsed["action"] != "skill":
            print(f"[WARN] 指令动作「{parsed['action']}」暂未实现完整生成，按技能处理。")
        skill_id = parsed["target"] or skill_id
        raw_cmd = parsed["raw"]

    if not skill_id:
        print("No skill recognized. Available:")
        list_skills()
        return 1

    try:
        pack_dir = generate_pack(skill_id, args.out, raw_command=raw_cmd)
        print(f"[OK] Pack generated: {pack_dir}")
        for f in os.listdir(pack_dir):
            print(f"     - {f}")
        # 校验 pack.json 中文不乱码
        pj = os.path.join(pack_dir, "pack.json")
        with open(pj, "r", encoding=READ_ENCODING) as rf:
            meta = json.load(rf)
            cmd = meta.get("command", "")
            if "??" in cmd or "\ufffd" in cmd:
                print(f"[WARN] pack.json command 字段可能存在编码问题: {cmd!r}")
            else:
                print(f"[OK] pack.json command 正常: {cmd}")
        return 0
    except ValueError as e:
        print(f"[ERR] {e}")
        return 1


if __name__ == "__main__":
    sys.exit(main())
