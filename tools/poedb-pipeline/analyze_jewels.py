# -*- coding: utf-8 -*-
"""
analyze_jewels.py — 任务 B：POE 传奇珠宝 × 《暗影地牢》适配性分级（逐颗人工判读）
==================================================================================
输入：data/poedb/unique_jewels/unique_jewels.json（97 颗真实抓取）
输出：data/poedb/unique_jewels/jewel_adaptation.json
分级：A=现有字段/机制直接落地  B=现有机制改写（Tier2 补丁有先例）
      C=需新系统/大改  D=依赖 POE 专有体系（无对应，不建议实现）
判读基准：game-systems-ref/01-equipment-affixes.md 词缀域全表 + V1.23 投射物机制先例
"""
import json, os, sys
from collections import Counter

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", ".."))
SRC = os.path.join(ROOT, "data", "poedb", "unique_jewels", "unique_jewels.json")

# name_list(英文名) → (分级, 游戏落点, 实现要点)
VERDICT = {
    "Rain of Splinters": ("A", "投射物+1 / 图腾类战地实体", "图腾技能发射额外投射物：复用 V1.23 Count_F 增幅管线；图腾增伤走战地实体域 Index 1100-1146"),
    "Hidden Potential": ("A", "每件魔法装备增伤 → 装备共鸣域", "游戏已有 Index 400-464「每件强化/带技能/幻化武器共鸣」词缀族，加一条 per-magic-item 共鸣即可"),
    "Primordial Eminence": ("A", "同伴攻速 C_ATSpeed / 同伴增益", "同伴攻速游戏直接有（宝石 Type 11 同款）；「魔像增益」做成召唤时临时 Buff"),
    "To Dust": ("A", "同伴增伤 C_Damage / 同伴移速", "两个字段游戏原生存在（Index 103 + 同伴移速）"),
    "Fortress Covenant": ("A", "同伴增伤 + 格挡几率", "C_Damage 直接映射；副手格挡走 Index 17"),
    "Emperors Mastery": ("A", "生命上限 + 体型视觉", "生命上限 Index 1 直接映射；体型=玩家模型 scale 缩放（纯视觉，B 级工作量但无机制风险，归 A 组）"),
    "Inspired Learning": ("B", "击败精英获得随机增益", "游戏击杀触发域 Index 1250-1276 + Buff_PL_Layer 叠层 Buff 可复刻「击败稀有怪偷 buff 20s」"),
    "Emperors Might": ("B", "体型视觉 + 任意数值挂载", "体型缩放可做；力量属性游戏无，可改挂伤害%"),
    "Grand Spectrum": ("B", "同类珠宝计数叠层", "镶嵌珠宝系统（V1.23 Type26 先例）可在装备时统计同类 GlobalID 数量 ×7% 全抗"),
    "Primordial Might": ("B", "召唤时临时增伤", "召唤链 CompanionRuntimeData 有挂点；「召唤后 8s 增伤」走同伴召唤触发 Buff"),
    "Replica Primordial Might": ("B", "同伴上限-1 + 召唤增伤", "Summon_count_Other 字段原生存在（Index 4100）；负面面+增伤面均可落地"),
    "Firesong": ("B", "点燃时间 → 全异常持续", "游戏 DOT 六系持续时间域（Index 301）改写为全 DOT 共享"),
    "The Golden Rule": ("B", "流血自反 + 条件护甲", "DOT 自反=新触发路径但走 DOT_MG 管线；护甲近似减伤 Index 18"),
    "Seething Fury": ("B", "持盾条件增伤", "CharType 判断副手类型 + 伤害%；ES 转暴击面可砍"),
    "Quickening Covenant": ("B", "同伴攻速/施速", "同伴攻速直接映射；法术压制面砍掉"),
    "Stormshroud": ("B", "异常避免近似", "游戏无异常几率字段，近似映射到 DOT 减免/减速抗"),
    "Ancestral Vision": ("B", "异常避免近似", "同上，压制→异常避免的桥接可在 DOT 结算入口做"),
    "Fevered Mind": ("B", "权衡类词缀", "游戏原生有 Index 750「伤害大幅提升但耗蓝加倍」权衡域，直接改数值落地"),
    "Healthy Mind": ("B", "属性转化域", "游戏转化域 Index 600-604 有生命/法力转伤害先例，加一条生命→魔力转化"),
    "Dead Reckoning": ("B", "同伴附加盾条件伤害", "同伴伤害 + 盾牌 ES 条件（CharType 判断），混沌伤并入暗影白字"),
    "Reckless Defence": ("B", "格挡拆分", "游戏格挡单字段（Index 17），拆攻击/法术两类需小改结算"),
    "Replica Reckless Defence": ("B", "格挡拆分", "同上"),
    "Emperors Wit": ("B", "暴击率", "暴击率 30% 直接映射（Index 13）；智慧面砍掉"),
    "Unending Hunger": ("B", "召唤击杀触发增益", "同伴击杀玩家回血（Index 4307）先例 → 改成「召唤击杀得噬魂 buff」"),
    "Primordial Harmony": ("B", "同伴技能冷却", "同伴技能 CD 字段存在（CompSkill CoolDown）"),
    "The Anima Stone": ("B", "同类珠宝计数条件", "同 Grand Spectrum 计数逻辑，条件成立+1 魔像"),
    "Energised Armour": ("C", "护甲/ES 体系缺失", "游戏无护甲与 ES，只能近似减伤（Index 18），转换关系需重新设计"),
    "The Red Dream": ("C", "伤害转化 + 树半径联动", "火伤→混沌转化可并入元素转化域，但「天赋联动」依赖 POE 树，需砍半实现"),
    "The Red Nightmare": ("C", "同上 + 格挡", "同上"),
    "The Green Dream": ("C", "同 Red Dream", "同上"),
    "The Green Nightmare": ("C", "同上", "同上"),
    "The Blue Dream": ("C", "同 Red Dream", "同上"),
    "The Blue Nightmare": ("C", "同上", "同上"),
    "Split Personality": ("C", "树距离统计", "游戏天赋树有图结构可算出发点距离，但需新增距离统计系统"),
    "Dissolution of the Flesh": ("C", "生命保留机制", "游戏无 ES/保留机制，需新状态机"),
    "Forbidden Flame": ("C", "双珠宝配对", "需两颗珠宝互检词缀 + 指定天赋解锁，配对系统需新做"),
    "Forbidden Flesh": ("C", "双珠宝配对", "同上"),
    "The Adorned": ("C", "腐化珠宝体系", "游戏无珠宝腐化，需先建腐化系统"),
    "Lioneyes Fall": ("C", "词缀类转换", "近战→弓转换需重写词缀归属判定"),
    "Self-Flagellation": ("C", "诅咒计数", "游戏无诅咒系统，可近似「每个负面 Debuff 增伤」走 DOT 层数统计"),
    "Apex Mode": ("C", "凝聚层数系统", "游戏无凝聚层数，需新叠层（可用 Buff_PL_Layer 近似但语义不符）"),
    "Nadir Mode": ("C", "凝聚层数系统", "同上"),
    "Melding of the Flesh": ("C", "抗性上限封顶", "游戏抗性上限词缀（Index 200-202）存在但封顶规则不同，需改结算"),
    "The Balance of Terror": ("C", "词缀池随机珠宝", "新界恐惧珠宝=18 条词缀池随机，需随机抽取系统"),
    "Watchers Eye": ("C", "光环词缀池", "核心=光环获得词缀，游戏无光环；仅生命上限+面可单独摘出做 A 级"),
    "Inertia": ("D", "POE 树半径 + 三围", "依赖范围内天赋与三围体系，游戏均无"),
    "Efficient Training": ("D", "三围转换", "游戏无三围"),
    "Fragility": ("D", "球系统", "游戏无耐力球"),
    "Replica Fragility": ("D", "球系统", "同上"),
    "Atziris Reign": ("D", "瓦尔技能", "游戏无瓦尔"),
    "Chill of Corruption": ("D", "瓦尔灵魂", "游戏无瓦尔"),
    "Might of the Meek": ("D", "树半径小天赋", "依赖 POE 树"),
    "Tempered Flesh": ("D", "三围计数", "游戏无三围"),
    "Combat Focus": ("D", "虹光技能", "POE 专有技能形态"),
    "Transcendent Flesh": ("D", "三围计数", "游戏无三围"),
    "Thread of Hope": ("D", "树半径穿透", "依赖 POE 树"),
    "Intuitive Leap": ("D", "核心天赋免连接", "依赖 POE 树"),
    "Pure Talent": ("D", "出发点连接判定", "依赖 POE 树"),
    "Fluid Motion": ("D", "三围转换", "游戏无三围"),
    "Careful Planning": ("D", "三围转换", "游戏无三围"),
    "Fertile Mind": ("D", "三围转换", "游戏无三围"),
    "Brute Force Solution": ("D", "三围转换", "游戏无三围"),
    "Hidden Potential(仿品)": ("D", "—", "—"),
    "Tempered Spirit": ("D", "三围计数", "游戏无三围"),
    "Transcendent Spirit": ("D", "三围计数", "游戏无三围"),
    "Unnatural Instinct": ("D", "树半径小天赋", "依赖 POE 树"),
    "Pacifism": ("D", "球系统", "游戏无狂怒球"),
    "Replica Pacifism": ("D", "球系统", "同上"),
    "Powerlessness": ("D", "球系统", "游戏无暴击球"),
    "Replica Powerlessness": ("D", "球系统", "同上"),
    "Immutable Force": ("D", "晕眩/格挡回复", "游戏无晕眩机制"),
    "Bloodnotch": ("D", "眩晕伤害转生命", "游戏无眩晕"),
    "Warriors Tale": ("D", "文身", "POE 专有"),
    "Energy From Within": ("D", "ES/生命转换", "游戏无 ES"),
    "Witchbane": ("D", "诅咒体系", "游戏无诅咒"),
    "Rational Doctrine": ("D", "最高属性 + 地面", "游戏无三围与地面系统"),
    "Sublime Vision": ("D", "永恒珠宝类", "依赖 POE 瓦尔永恒珠宝体系"),
    "The Light of Meaning": ("D", "树半径", "依赖 POE 树"),
    "Bound By Destiny": ("D", "永恒珠宝类", "依赖 POE 树改写"),
    "Glorious Vanity": ("D", "永恒珠宝", "瓦尔信仰转化"),
    "Militant Faith": ("D", "永恒珠宝", "圣堂转化"),
    "Brutal Restraint": ("D", "永恒珠宝", "马拉克斯转化"),
    "Elegant Hubris": ("D", "永恒珠宝", "永恒帝国纪念"),
    "Lethal Pride": ("D", "永恒珠宝", "卡鲁领导权"),
    "Heroic Tragedy": ("D", "永恒珠宝", "卡古兰纪念"),
    "Voices": ("D", "珠宝插槽天赋", "依赖 POE 树"),
    "Megalomaniac": ("D", "随机天赋", "依赖 POE 树"),
    "One With Nothing": ("D", "空明之掌天赋", "依赖 POE 树"),
    "Kitavas Teachings": ("D", "奇塔弗门徒天赋", "依赖 POE 树"),
    "The Interrogation": ("D", "苦难秘辛天赋", "依赖 POE 树"),
    "Calamitous Visions": ("D", "独行使者天赋", "依赖 POE 树"),
    "Natural Affinity": ("D", "造化自然天赋", "依赖 POE 树"),
    "The Siege": ("D", "万物皆动天赋", "依赖 POE 树"),
    "The Front Line": ("D", "百战老兵天赋", "依赖 POE 树"),
    "Emperors Cunning": ("B", "体型视觉 + 暴击率近似", "体型缩放可做；敏捷面砍掉，可改挂移速%"),
    "Impossible Escape": ("D", "核心天赋免连接", "依赖 POE 树"),
    "Tempered Mind": ("D", "三围计数", "游戏无三围"),
    "Transcendent Mind": ("D", "三围计数", "游戏无三围"),
}
# 别名兜底（抓取名与 POE 常用名差异）
ALIAS = {"Hidden Potential(仿品)": []}


def main():
    data = json.load(open(SRC, encoding="utf-8"))
    out = []
    missing = []
    for j in data["jewels"]:
        key = j["name_list"]
        v = VERDICT.get(key)
        if v is None:
            missing.append(key)
            v = ("?", "待判读", "")
        out.append({
            "name_cn": j.get("name_cn"), "name_en": key, "base": (j.get("title") or "").replace(j.get("name_cn") or "", "").strip(),
            "effects": j["effects"][:4],
            "grade": v[0], "game_target": v[1], "how": v[2],
        })
    stats = Counter(x["grade"] for x in out)
    json.dump({"schema_version": "1.0", "source": "poedb.tw/cn/Jewels（97 颗真实抓取）",
               "grade_meaning": {"A": "现有字段/机制直接落地", "B": "现有机制改写（Tier2 补丁有先例）",
                                 "C": "需新系统/大改", "D": "依赖 POE 专有体系（不建议实现）", "?": "待判读"},
               "stats": dict(stats), "jewels": out},
              open(os.path.join(ROOT, "data", "poedb", "unique_jewels", "jewel_adaptation.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    print("分级:", dict(stats))
    if missing:
        print("未判读:", missing)


if __name__ == "__main__":
    main()
