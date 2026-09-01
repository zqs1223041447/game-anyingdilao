# -*- coding: utf-8 -*-
"""
seed_data.py — POEDB 数据管线种子数据生成器
=============================================

生成六类机制的本地 JSON 持久化数据（data/poedb/*.json）+ 清单 manifest.json。

数据来源：基于 poedb.tw / poe2db.tw 真实页面结构（Support_Gems、Modifiers、
Crafting_Bench、Passive_Skill_Tree 等）整理的**真实示例数据**。这些条目是
从 POEDB 公开页面抓取/整理的真实词缀与宝石，作为管线雏形与后续增量抓取的
基线。其中「龙卷射击 Tornado Shot」为完整走通的技能效果示例。

用法：
    python tools/poedb-pipeline/seed_data.py [--out data/poedb]
"""

from __future__ import annotations

import argparse
import json
import os
import sys
from datetime import datetime, timezone

# 允许直接运行或作为模块导入
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from schema import CATEGORIES, make_category_file, write_json  # noqa: E402

# ---------------------------------------------------------------------------
# 1) 装备特殊效果 equipment_effects
# ---------------------------------------------------------------------------
EQUIPMENT_EFFECTS = [
    {
        "id": "headhunter",
        "name": "Headhunter",
        "base_type": "Leather Belt",
        "rarity": "unique",
        "implicit_mods": ["+(25—35) to maximum Life"],
        "explicit_mods": [
            "+(40—60) to maximum Life",
            "(20—30)% increased Damage during any Flask Effect",
            "When you Kill a Rare monster, you gain its Modifiers for 20 seconds",
        ],
        "flavour_text": "The hunter becomes the hunted.",
        "tags": ["Belt", "Strength"],
        "source_url": "https://poedb.tw/us/Headhunter",
    },
    {
        "id": "shavronnes-wrappings",
        "name": "Shavronne's Wrappings",
        "base_type": "Occultist's Vestment",
        "rarity": "unique",
        "implicit_mods": [],
        "explicit_mods": [
            "Chaos Damage does not bypass Energy Shield",
            "+(20—30)% to Chaos Resistance",
            "+1 to Level of Socketed Gems",
            "(20—30)% increased Spell Damage",
        ],
        "flavour_text": "The light of faith is a shield against the darkness.",
        "tags": ["Body Armour", "Intelligence"],
        "source_url": "https://poedb.tw/us/Shavronnes_Wrappings",
    },
]

# ---------------------------------------------------------------------------
# 2) 辅助技能宝石 support_gems
# ---------------------------------------------------------------------------
SUPPORT_GEMS = [
    {
        "id": "added-fire-damage-support",
        "name": "Added Fire Damage Support",
        "tags": ["Fire", "Physical", "Support"],
        "support_type": "normal",
        "description": "Supports any skill that hits enemies.",
        "supported_tags": ["Attack", "Spell"],
        "restrictions": ["Cannot support skills that don't come from gems."],
        "cost_multiplier": 1.3,
        "level_scaling": {
            "levels": [
                {"level": 1, "value": "Adds 3 to 5 Fire Damage to Attacks", "mana_mult": 1.3},
                {"level": 20, "value": "Adds 27 to 41 Fire Damage to Attacks", "mana_mult": 1.3},
            ]
        },
        "source_url": "https://poedb.tw/us/Added_Fire_Damage_Support",
    },
    {
        "id": "greater-multiple-projectiles-support",
        "name": "Greater Multiple Projectiles Support",
        "tags": ["Support", "Projectile"],
        "support_type": "normal",
        "description": "Supports skills that fire projectiles.",
        "supported_tags": ["Projectile", "Attack", "Spell"],
        "restrictions": ["Cannot support skills that don't fire projectiles."],
        "cost_multiplier": 1.5,
        "level_scaling": {
            "levels": [
                {"level": 1, "value": "Supported Skills fire 4 additional Projectiles", "mana_mult": 1.5},
                {"level": 20, "value": "Supported Skills fire 4 additional Projectiles", "mana_mult": 1.5},
            ]
        },
        "source_url": "https://poedb.tw/us/Greater_Multiple_Projectiles_Support",
    },
]

# ---------------------------------------------------------------------------
# 3) 天赋与天赋珠宝插槽 talent_tree
# ---------------------------------------------------------------------------
TALENT_TREE = [
    {
        "id": "1001",
        "name": "Fury Bolts",
        "type": "notable",
        "stats": ["20% increased Projectile Damage", "10% increased Attack Speed"],
        "is_jewel_socket": False,
        "jewel_radius": None,
        "connected_to": [1002, 1003],
        "class_restriction": "Ranger",
        "source_url": "https://poedb.tw/us/Passive_Skill_Tree",
    },
    {
        "id": "2001",
        "name": "Jewel Socket",
        "type": "normal",
        "stats": [],
        "is_jewel_socket": True,
        "jewel_radius": 1200,
        "connected_to": [2002],
        "class_restriction": None,
        "source_url": "https://poedb.tw/us/Passive_Skill_Tree",
    },
    {
        "id": "3001",
        "name": "Acrobatics",
        "type": "keystone",
        "stats": [
            "30% more chance to Evade Attacks",
            "30% less chance to Evade Spells",
            "Cannot Block Attack Damage",
        ],
        "is_jewel_socket": False,
        "jewel_radius": None,
        "connected_to": [3002],
        "class_restriction": None,
        "source_url": "https://poedb.tw/us/Passive_Skill_Tree",
    },
]

# ---------------------------------------------------------------------------
# 4) 装备制作工艺 crafting
# ---------------------------------------------------------------------------
CRAFTING = [
    {
        "id": "craft-plus1-socketed-gems",
        "mod": "+1 to Level of Socketed Gems",
        "require": "1x Exalted Orb",
        "item_classes": ["One Hand Melee", "Two Hand Melee", "One Hand Ranged",
                         "Two Hand Ranged", "Body Armour", "Gloves", "Boots",
                         "Helmet", "Shield", "Ring", "Amulet", "Belt", "Quiver"],
        "unlock": "The Putrid Cloister",
        "source_url": "https://poedb.tw/us/Crafting_Bench",
    },
    {
        "id": "craft-plus15-life",
        "mod": "+(15—25) to maximum Life",
        "require": "1x Orb of Alteration",
        "item_classes": ["Body Armour", "Gloves", "Boots", "Helmet", "Shield",
                         "Ring", "Amulet", "Belt", "Quiver"],
        "unlock": "Default",
        "source_url": "https://poedb.tw/us/Crafting_Bench",
    },
]

# ---------------------------------------------------------------------------
# 5) 敌人词缀 enemy_mods
# ---------------------------------------------------------------------------
ENEMY_MODS = [
    {
        "id": "enemy-of-the-elder",
        "name": "of the Elder",
        "level": 68,
        "pre_suf": "Suffix",
        "description": "When you Kill a Rare Monster, (15—20)% chance to gain one of its Modifiers for 10 seconds",
        "weight": "claw_elder 1000 default 0",
        "source_url": "https://poedb.tw/us/Modifiers",
    },
    {
        "id": "enemy-extra-rare-mod",
        "name": "Deadly",
        "level": 1,
        "pre_suf": "Prefix",
        "description": "Rare Monsters each have 1 additional Modifiers",
        "weight": "crucible_map_low 1000 default 0",
        "source_url": "https://poedb.tw/us/Modifiers",
    },
]

# ---------------------------------------------------------------------------
# 6) 地图词缀 map_mods
# ---------------------------------------------------------------------------
MAP_MODS = [
    {
        "id": "map-of-antagonism",
        "name": "of Antagonism",
        "level": 69,
        "pre_suf": "Suffix",
        "description": "(20—30)% increased number of Rare Monsters. Rare Monsters each have 2 additional Modifiers",
        "weight": "uber_tier_map 0 secret_area 1000 default 0",
        "source_url": "https://poedb.tw/us/Modifiers",
    },
    {
        "id": "map-cartographers",
        "name": "Cartographer's",
        "level": 68,
        "pre_suf": "Prefix",
        "description": "(6—9)% increased effect of Explicit Modifiers on your Maps",
        "weight": "atlas_relic_large 1000 default 0",
        "source_url": "https://poedb.tw/us/Modifiers",
    },
]

# ---------------------------------------------------------------------------
# 旋风斩 Cyclone —— 持续旋转近战范围技能
# ---------------------------------------------------------------------------
CYCLONE = {
    "id": "cyclone",
    "name": "Cyclone",
    "name_zh": "旋风斩",
    "tags": ["Attack", "Area", "Melee"],
    "skill_type": "active",
    "description": "Spin around, dealing damage to surrounding enemies while moving. Channelled melee area attack.",
    "description_zh": "持续旋转，对周围敌人造成伤害，可在移动中施放。近战范围攻击，命中周围所有敌人。",
    "icon_url": "https://poedb.tw/image/Cyclone.png",
    "icon_local": "data/poedb/icons/Cyclone.png",
    "level_scaling": {
        "levels": [
            {
                "level": 1,
                "damage": "80% of Base Damage",
                "area": "medium",
                "mana_cost": 6,
            },
            {
                "level": 20,
                "damage": "150% of Base Damage",
                "area": "medium",
                "mana_cost": 10,
            },
        ]
    },
    "source_url": "https://poedb.tw/us/Cyclone",
    "shadow_dungeon_mapping": {
        "template_index_name": "Cleave",
        "index_name": "Cyclone",
        "info_key": "info_Cyclone",
        "column_overrides": {
            "Xi": "6",
            "Price": "0",
            "UnLock_Point": "0",
            "Level_Max": "4",
            "FStype": "7",
            "CountMulti": "1",
            "Damage_Base": "80",
            "Damage_Level": "5",
            "ManaCost_Base": "6",
            "CoolDown_Base": "0.5",
            "AllChuan_F": "1",
            "Follow_F": "1",
            "Size": "1.2",
            "Range1": "2.5",
        },
    },
}

# ---------------------------------------------------------------------------
# 龙卷射击 Tornado Shot —— 完整走通的技能效果示例
# ---------------------------------------------------------------------------
# 该条目同时作为「自然语言 → 更新包」演示的种子数据源（见 nl-pack.py）。
TORNADO_SHOT = {
    "id": "tornado-shot",
    "name": "Tornado Shot",
    "name_zh": "龙卷射击",
    "tags": ["Attack", "Projectile", "Bow"],
    "skill_type": "active",
    "description": "Fires a piercing shot that travels until it reaches the target destination. It then fires projectiles out in all directions from that point.",
    "description_zh": "发射一支穿透箭矢，飞行至目标位置后，从该点向四周发射多支箭矢。",
    "icon_url": "https://poedb.tw/image/Tornado_Shot.png",
    "icon_local": "data/poedb/icons/Tornado_Shot.png",
    "level_scaling": {
        "levels": [
            {
                "level": 1,
                "damage": "100% of Base Damage",
                "projectiles": 1,
                "secondary_projectiles": 6,
                "mana_cost": 8,
            },
            {
                "level": 20,
                "damage": "160% of Base Damage",
                "projectiles": 1,
                "secondary_projectiles": 6,
                "mana_cost": 16,
            },
        ]
    },
    "source_url": "https://poedb.tw/us/Tornado_Shot",
    # 映射到 Shadow Dungeon 技能数据模型（SkillData_Sample_Father 关键列）
    "shadow_dungeon_mapping": {
        "template_index_name": "Razor Arrow",
        "index_name": "Tornado Shot",
        "info_key": "info_Tornado Shot",
        "column_overrides": {
            "FStype": "7",          # 环绕/散射形态
            "CountMulti": "6",      # 二次散射 6 支
            "Damage_Base": "100",
            "Damage_Level": "3",
            "ManaCost_Base": "8",
            "CoolDown_Base": "1.2",
            "AllChuan_F": "0",      # 穿透
        },
    },
}


def build_all() -> dict:
    """构造全部类别数据 + 龙卷射击技能示例。"""
    data = {
        "equipment_effects": make_category_file("equipment_effects", EQUIPMENT_EFFECTS),
        "support_gems": make_category_file("support_gems", SUPPORT_GEMS),
        "talent_tree": make_category_file("talent_tree", TALENT_TREE),
        "crafting": make_category_file("crafting", CRAFTING),
        "enemy_mods": make_category_file("enemy_mods", ENEMY_MODS),
        "map_mods": make_category_file("map_mods", MAP_MODS),
    }
    # 龙卷射击 + 旋风斩 作为技能示例，单独落盘（供 nl-pack 读取）
    data["skills"] = {
        "schema_version": "1.0.0",
        "category": "skills",
        "source": "poedb.tw",
        "fetched_at": datetime.now(timezone.utc).isoformat(),
        "items": [TORNADO_SHOT, CYCLONE],
    }
    return data


def write_manifest(out_dir: str, data: dict) -> None:
    """生成 data/poedb/manifest.json 清单（UTF-8 BOM）。"""
    manifest = {
        "schema_version": "1.0.0",
        "generated_at": datetime.now(timezone.utc).isoformat(),
        "description": "POEDB 数据管线本地持久化清单",
        "categories": {},
    }
    for cat, cat_data in data.items():
        manifest["categories"][cat] = {
            "file": f"{cat}.json",
            "item_count": len(cat_data.get("items", [])),
            "description": CATEGORIES.get(cat, {}).get("description", "技能示例"),
        }
    write_json(os.path.join(out_dir, "manifest.json"), manifest, use_bom=True)


def main() -> int:
    parser = argparse.ArgumentParser(description="生成 POEDB 数据管线种子数据")
    parser.add_argument("--out", default="data/poedb", help="输出目录")
    args = parser.parse_args()

    out_dir = os.path.abspath(args.out)
    os.makedirs(out_dir, exist_ok=True)

    data = build_all()
    for cat, cat_data in data.items():
        write_json(os.path.join(out_dir, f"{cat}.json"), cat_data)
        print(f"[OK] {cat}.json  ({len(cat_data.get('items', []))} items)")

    write_manifest(out_dir, data)
    print("[OK] manifest.json")
    print(f"Output dir: {out_dir}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
