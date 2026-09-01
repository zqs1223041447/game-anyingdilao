# -*- coding: utf-8 -*-
"""
schema.py — POEDB MOD 数据管线统一 Schema 定义
================================================

本模块定义 Shadow Dungeon MOD 从 POEDB 抓取/转换数据的统一 JSON Schema。
覆盖六类机制：
  1. equipment_effects  装备特殊效果（含唯一装备的隐式/显式词缀）
  2. support_gems       辅助技能宝石效果及限制（tags / 支持规则 / 效果）
  3. talent_tree        天赋与天赋珠宝插槽（节点 / 珠宝槽 / 大点）
  4. crafting           装备制作工艺（Crafting Bench：Mod / Require / ItemClasses / Unlock）
  5. enemy_mods         敌人词缀（Monster mods）
  6. map_mods           地图词缀（Map mods）

持久化格式：本地 JSON（data/poedb/*.json）+ 清单 data/poedb/manifest.json。
本模块只定义结构与校验，不负责抓取（见 fetch_poedb.py）与种子数据（见 seed_data.py）。
"""

from __future__ import annotations

import json
import os as _os_sys
import sys
from typing import Any, Dict, List, Optional

# 确保控制台 UTF-8（与 nl-pack.py 一致）
try:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    if hasattr(sys.stderr, "reconfigure"):
        sys.stderr.reconfigure(encoding="utf-8", errors="replace")
except Exception:
    pass

# ---------------------------------------------------------------------------
# Schema 版本
# ---------------------------------------------------------------------------
SCHEMA_VERSION = "1.0.0"

# ---------------------------------------------------------------------------
# 六类数据的统一顶层结构
# ---------------------------------------------------------------------------
# 每个 data/poedb/<category>.json 文件形如：
# {
#   "schema_version": "1.0.0",
#   "category": "support_gems",
#   "source": "poedb.tw",
#   "fetched_at": "2026-08-27T00:00:00Z",
#   "items": [ ... ]
# }

# ---------------------------------------------------------------------------
# 1) equipment_effects —— 装备特殊效果
# ---------------------------------------------------------------------------
EQUIPMENT_EFFECT_ITEM = {
    "id": "str, 唯一键（如 'headhunter'）",
    "name": "str, 装备名（如 'Headhunter'）",
    "base_type": "str, 基底类型（如 'Leather Belt'）",
    "rarity": "str, unique/magic/rare/normal",
    "implicit_mods": ["str, 隐式词缀描述"],
    "explicit_mods": ["str, 显式词缀描述"],
    "flavour_text": "str, 风味文本（可选）",
    "tags": ["str, 装备标签（如 Belt, Strength）"],
    "source_url": "str, poedb 页面 URL",
}

# ---------------------------------------------------------------------------
# 2) support_gems —— 辅助技能宝石效果及限制
# ---------------------------------------------------------------------------
SUPPORT_GEM_ITEM = {
    "id": "str, 唯一键（如 'added-fire-damage-support'）",
    "name": "str, 宝石名（如 'Added Fire Damage Support'）",
    "tags": ["str, 宝石标签（如 Fire, Physical, Support）"],
    "support_type": "str, normal/awakened/exceptional",
    "description": "str, 效果描述（含 # 占位符）",
    "supported_tags": ["str, 可支持的技能标签（如 Attack, Spell）"],
    "restrictions": ["str, 限制（如 'Cannot support skills that don't come from gems'）"],
    "cost_multiplier": "float, 法力消耗倍率（可选）",
    "level_scaling": {
        "levels": [{"level": 1, "value": "效果数值", "mana_mult": 1.0}],
    },
    "source_url": "str",
}

# ---------------------------------------------------------------------------
# 3) talent_tree —— 天赋与天赋珠宝插槽
# ---------------------------------------------------------------------------
TALENT_NODE_ITEM = {
    "id": "str, 节点 hash（如 12345）",
    "name": "str, 节点名",
    "type": "str, normal/notable/keystone/mastery",
    "stats": ["str, 节点提供的词缀"],
    "is_jewel_socket": "bool, 是否为珠宝插槽",
    "jewel_radius": "int, 珠宝影响半径（可选）",
    "connected_to": ["int, 相连节点 hash"],
    "class_restriction": "str, 职业限制（可选）",
    "source_url": "str",
}

# ---------------------------------------------------------------------------
# 4) crafting —— 装备制作工艺
# ---------------------------------------------------------------------------
CRAFTING_RECIPE_ITEM = {
    "id": "str, 唯一键",
    "mod": "str, 工艺词缀（如 '+1 to Level of Socketed Gems'）",
    "require": "str, 消耗材料（如 '1x Exalted Orb'）",
    "item_classes": ["str, 适用装备类别"],
    "unlock": "str, 解锁条件（如 'The Grand Arena' / 'Default'）",
    "source_url": "str",
}

# ---------------------------------------------------------------------------
# 5) enemy_mods —— 敌人词缀
# ---------------------------------------------------------------------------
ENEMY_MOD_ITEM = {
    "id": "str, 唯一键",
    "name": "str, 词缀名（如 'of the Elder'）",
    "level": "int, 词缀等级",
    "pre_suf": "str, prefix/suffix",
    "description": "str, 词缀效果描述",
    "weight": "str, 权重（如 'claw_elder 1000 default 0'）",
    "source_url": "str",
}

# ---------------------------------------------------------------------------
# 6) map_mods —— 地图词缀
# ---------------------------------------------------------------------------
MAP_MOD_ITEM = {
    "id": "str, 唯一键",
    "name": "str, 词缀名",
    "level": "int, 词缀等级",
    "pre_suf": "str, prefix/suffix",
    "description": "str, 词缀效果描述",
    "weight": "str, 权重",
    "source_url": "str",
}

# ---------------------------------------------------------------------------
# 类别注册表
# ---------------------------------------------------------------------------
CATEGORIES: Dict[str, Dict[str, Any]] = {
    "equipment_effects": {
        "file": "equipment_effects.json",
        "item_schema": EQUIPMENT_EFFECT_ITEM,
        "description": "装备特殊效果",
    },
    "support_gems": {
        "file": "support_gems.json",
        "item_schema": SUPPORT_GEM_ITEM,
        "description": "辅助技能宝石效果及限制",
    },
    "talent_tree": {
        "file": "talent_tree.json",
        "item_schema": TALENT_NODE_ITEM,
        "description": "天赋与天赋珠宝插槽",
    },
    "crafting": {
        "file": "crafting.json",
        "item_schema": CRAFTING_RECIPE_ITEM,
        "description": "装备制作工艺",
    },
    "enemy_mods": {
        "file": "enemy_mods.json",
        "item_schema": ENEMY_MOD_ITEM,
        "description": "敌人词缀",
    },
    "map_mods": {
        "file": "map_mods.json",
        "item_schema": MAP_MOD_ITEM,
        "description": "地图词缀",
    },
}


def make_category_file(category: str, items: List[Dict[str, Any]],
                       fetched_at: str = "2026-08-27T00:00:00Z",
                       source: str = "poedb.tw") -> Dict[str, Any]:
    """构造一个类别数据文件的顶层结构。"""
    if category not in CATEGORIES:
        raise ValueError(f"未知类别: {category}，可选 {list(CATEGORIES)}")
    return {
        "schema_version": SCHEMA_VERSION,
        "category": category,
        "source": source,
        "fetched_at": fetched_at,
        "items": items,
    }


def validate_item(category: str, item: Dict[str, Any]) -> List[str]:
    """校验单个 item 是否满足该类别 schema 的必填键。返回缺失键列表。"""
    schema = CATEGORIES[category]["item_schema"]
    missing = [k for k in schema if k not in item]
    return missing


def write_json(path: str, data: Dict[str, Any], use_bom: bool = True) -> None:
    """以 UTF-8 缩进 JSON 落盘（默认带 BOM，确保 PowerShell/记事本自动识别为 UTF-8）。"""
    enc = "utf-8-sig" if use_bom else "utf-8"
    with open(path, "w", encoding=enc) as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
        f.write("\n")


def read_json(path: str) -> Dict[str, Any]:
    """以 UTF-8-SIG 读取（兼容带/不带 BOM），返回解析后的 dict。"""
    # utf-8-sig 会自动剥离 BOM，若文件无 BOM 则等价于 utf-8
    with open(path, "r", encoding="utf-8-sig") as f:
        return json.load(f)


def validate_all(data_dir: str = "data/poedb") -> int:
    """校验 data_dir 下全部类别文件的完整性。返回错误数（0=全部通过）。"""
    import os as _os
    errors = 0
    # 检查 manifest
    manifest_path = _os.path.join(data_dir, "manifest.json")
    if not _os.path.exists(manifest_path):
        print(f"[ERR] manifest.json 缺失: {manifest_path}")
        errors += 1
    else:
        try:
            m = read_json(manifest_path)
            cats = m.get("categories", {})
            for cat, meta in cats.items():
                fp = _os.path.join(data_dir, meta.get("file", f"{cat}.json"))
                if not _os.path.exists(fp):
                    print(f"[ERR] {cat}: 文件缺失 {fp}")
                    errors += 1
                    continue
                jf = read_json(fp)
                items = jf.get("items", [])
                expected = meta.get("item_count", -1)
                if expected != -1 and expected != len(items):
                    print(f"[WARN] {cat}: manifest item_count={expected} 实际 {len(items)}")
                # 校验必填字段
                for idx, it in enumerate(items):
                    miss = validate_item(cat, it) if cat in CATEGORIES else []
                    # skills 类别不走 CATEGORIES，用单独校验
                    if cat == "skills":
                        for k in ("id", "name", "name_zh", "tags", "skill_type", "shadow_dungeon_mapping"):
                            if k not in it:
                                miss.append(k)
                    if miss:
                        print(f"[ERR] {cat}[{idx}] 缺失字段: {miss}")
                        errors += 1
                if errors == 0:
                    print(f"[OK] {cat}: {len(items)} items valid")
        except Exception as ex:
            print(f"[ERR] 校验异常: {ex}")
            errors += 1
    # 校验 tornado-shot 完整性
    try:
        sj = read_json(_os.path.join(data_dir, "skills.json"))
        found = any(it.get("id") == "tornado-shot" for it in sj.get("items", []))
        if not found:
            print("[ERR] skills.json 缺少 tornado-shot 完整示例")
            errors += 1
        else:
            # 检查映射完整性
            ts = next(it for it in sj["items"] if it["id"] == "tornado-shot")
            mp = ts.get("shadow_dungeon_mapping", {})
            for k in ("template_index_name", "index_name", "info_key", "column_overrides"):
                if k not in mp:
                    print(f"[ERR] tornado-shot mapping 缺失 {k}")
                    errors += 1
            print("[OK] tornado-shot 完整示例可查")
    except Exception as ex:
        print(f"[ERR] tornado-shot 检查异常: {ex}")
        errors += 1
    return errors


if __name__ == "__main__":
    import argparse as _argparse
    _p = _argparse.ArgumentParser(description="POEDB Schema 校验")
    _p.add_argument("cmd", nargs="?", default="validate", help="validate | list")
    _p.add_argument("--data-dir", default="data/poedb", help="数据目录")
    _a = _p.parse_args()
    if _a.cmd in ("validate", "check"):
        # 解析 --data-dir 若以不同形式传入
        dd = _a.data_dir
        # 允许 python schema.py validate --data-dir X
        # 若第一个参数是路径且包含 poedb，则视为 data-dir
        import os as _os2
        if not _os2.path.exists(dd):
            # 尝试相对 ROOT
            _root = _os2.path.abspath(_os2.path.join(_os2.path.dirname(__file__), "..", ".."))
            cand = _os2.path.join(_root, dd)
            if _os2.path.exists(cand):
                dd = cand
        errs = validate_all(dd)
        if errs == 0:
            print("[PASS] 全部校验通过")
        else:
            print(f"[FAIL] 校验失败 {errs} 个错误")
        sys.exit(1 if errs else 0)
    elif _a.cmd == "list":
        print("Categories:", ", ".join(CATEGORIES.keys()) + ", skills")
        sys.exit(0)
    else:
        _p.print_help()
        sys.exit(1)
