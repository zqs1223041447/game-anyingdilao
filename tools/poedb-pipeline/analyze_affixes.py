# -*- coding: utf-8 -*-
"""
analyze_affixes.py — 任务 A：POE 装备词缀 → 《暗影地牢》物品分类映射 + 适配性分级
================================================================================
输入：data/poedb/affixes/affixes_all.json（112 页，含基底详情页词缀池）
输出：
  data/poedb/affixes/affix_by_slot.json        按暗影地牢槽位归并的词缀清单（含映射/分级）
  data/poedb/affixes/affix_mapping_stats.json  分级统计
适配分级：A=直接映射（游戏已有同义字段/机制） B=类似机制可改写（Tier2 补丁有先例）
          C=需新系统/大改  D=依赖 POE 专有系统（无对应，不建议实现）
暗影地牢词缀域依据 docs/game-systems-ref/01-equipment-affixes.md 的 Index 全表。
"""
import json, os, re, sys
from collections import Counter, defaultdict

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", ".."))
SRC = os.path.join(ROOT, "data", "poedb", "affixes", "affixes_all.json")
OUT = os.path.join(ROOT, "data", "poedb", "affixes")

# ---------- POE 页 → 暗影地牢槽位 ----------
def slot_of(page):
    if page in ("One_Hand_Axes", "One_Hand_Maces", "One_Hand_Swords", "Thrusting_One_Hand_Swords",
                "Two_Hand_Axes", "Two_Hand_Maces", "Two_Hand_Swords", "Bows", "Claws", "Daggers",
                "Rune_Daggers", "Staves", "Sceptres", "Wands", "Warstaves", "Eternal_Sword",
                "Decimation_Bow"):
        return "主手武器"
    for fam, slot in [("Helmets", "头盔"), ("Body_Armours", "胸甲"), ("Gloves", "手套"), ("Boots", "鞋子")]:
        if page == fam or page.startswith(fam + "_") or page in ("Ezomyte_Burgonet", "Astral_Plate",
                                                                "Iron_Gauntlets", "Titan_Greaves"):
            return slot
    if page in ("Amulets", "Onyx_Amulet"):
        return "护符"
    if page in ("Rings", "Bone_Ring", "Unset_Ring", "Two-Stone_Ring", "Burst_Band"):
        return "戒指"
    if page in ("Belts", "Vanguard_Belt", "Leather_Belt"):
        return "腰带→首饰(游戏无腰带)"
    if page in ("Bone_Spirit_Shield", "Tioco_Spirit_Shield", "Blunt_Arrow_Quiver", "Two-Point_Arrow_Quiver",
                "Quivers"):
        return "副手"
    if "Jewel" in page or page in ("Timeless_Jewel",):
        return "镶嵌珠宝"
    if "Flask" in page:
        return "药剂"
    if page in ("Blueprints", "Burial_Idol", "Conqueror_Idol", "Kamasan_Idol", "Foliate_Brooch",
                "Corvine_Charm", "Ursine_Charm", "Whisper-woven_Cloak", "Grandmaster_Keyring", "Contracts"):
        return "游戏无对应(异类)"
    return "其他"


# ---------- 词缀语义规则表：(正则, 游戏词缀域, Index, 分级) ----------
# Index 依据 game-systems-ref/01-equipment-affixes.md
RULES = [
    (r"最大生命|生命上限", "最大生命%", 1, "A"),
    (r"魔力上限|最大魔力|最大法力", "最大法力%", 2, "A"),
    (r"每秒(生命|魔力)?再生|生命回复速度|(生命|魔力)再生", "生命/法力秒回", 3, "A"),
    (r"攻击击中.*回复|击中回复", "击中回复", 5, "A"),
    (r"伤害提高|增加.*%伤害|总体伤害|攻击伤害|法术伤害", "基础伤害%", 10, "A"),
    (r"攻击速度|施法速度", "攻击速度%", 11, "A"),
    (r"移动速度", "移动速度%", 12, "A"),
    (r"暴击几率|暴击率|暴击产生几率", "暴击几率%", 13, "A"),
    (r"暴击伤害", "暴击伤害%", 14, "A"),
    (r"冷却回复速度", "冷却缩减%", 15, "A"),
    (r"魔力(消耗|保留)", "法力消耗降低%", 16, "A"),
    (r"格挡几率|格挡率", "格挡几率%", 17, "A"),
    (r"承受.*伤害.*降低|伤害减免", "伤害减免%", 18, "A"),
    (r"持续伤害.*降低", "持续伤害减免%", 19, "A"),
    (r"减速", "减速抗性%", 20, "A"),
    (r"穿透.*抗性|元素抗性穿透|抗性穿透", "全元素穿透%", 21, "A"),
    (r"全部元素抗性|元素抗性", "全元素抗性%", 22, "A"),
    (r"稀有怪物|魔法怪物|首领|传奇怪物", "精英/首领增减伤", 31, "A"),
    (r"物品稀有度|掉落数量|掉落率", "掉落几率%", 50, "A"),
    (r"投射物速度|箭矢速度", "投射物速度%", 51, "A"),
    (r"法球伤害", "法球技能伤害%", 52, "A"),
    (r"击晕|击退几率", "击晕几率%", 53, "A"),
    (r"投射物穿透", "投射物穿透率%", 54, "A"),
    (r"生命偷取|法力偷取", "伤害百分比吸收", 62, "A"),
    (r"宝石.*提高|镶嵌.*宝石", "宝石数值/效果加成", 80, "B"),
    (r"召唤生物.*伤害|魔宠.*伤害|图腾伤害", "同伴/召唤增伤", 103, "A"),
    (r"召唤生物.*生命|魔宠.*生命", "同伴生命", 100, "A"),
    (r"召唤生物.*速度|召唤生物.*攻击", "同伴攻速/移速", 102, "A"),
    (r"召唤生物.*抗性", "同伴全抗", 104, "A"),
    (r"灼烧|点燃|燃烧", "DOT域(火)", 2000, "A"),
    (r"中毒|毒素", "DOT域(毒)", 2000, "A"),
    (r"流血", "DOT域(物理)", 2000, "A"),
    (r"冰缓|冰冻|冻结", "DOT域(冰)", 2600, "A"),
    (r"感电", "DOT域(电)", 2000, "A"),
    (r"持续伤害.*(提高|加成)|(异常状态)?效果持续", "DOT强化/持续", 300, "B"),
    (r"光环|诅咒|烙印|捷|纪律|坚定|愤怒", "技能特化域(SK)", 3000, "C"),
    (r"图腾放置|放置图腾", "技能特化域(SK·图腾)", 3000, "C"),
    (r"击败敌人时|击杀时|击杀.*获得", "击杀叠层/触发", 1250, "B"),
    (r"攻击命中时|施法时|命中时.*获得", "施法叠层", 800, "B"),
    (r"低血|满血|生命.*低于|生命.*高于", "血量阈值触发", 500, "B"),
    (r"低魔力|满魔力|法力.*低于", "蓝量阈值触发", 509, "B"),
    (r"移动时|静止时|站立|冲刺", "移动状态触发", 550, "B"),
    (r"转化为|承受的.*转为|转为承受", "属性/元素转化", 600, "C"),
    (r"偷取时|溢出.*暴击", "双向机制转化", 650, "B"),
    (r"耐力球|狂怒球|暴击球|充能球|魔力球", "球系统", 0, "C"),
    (r"范围内天赋|附近.*天赋|周围.*天赋|neighbourhood", "POE天赋树半径", 0, "D"),
    (r"天赋.* Small (Passive )?Skills|小天赋点", "POE天赋树改写", 0, "D"),
    (r"辅助宝石|技能宝石等级|gem level", "宝石等级", 0, "B"),
    (r"战吼|嘲讽", "战吼系统", 0, "C"),
    (r"闪避值|闪避几率", "闪避(游戏无护甲类,近似减伤)", 18, "B"),
    (r"护甲|护体", "护甲(游戏无护甲类,近似减伤)", 18, "B"),
    (r"能量护盾", "能量护盾(游戏无ES,近似法力/护盾)", 2, "C"),
    (r"压制|法术压制", "法术压制(游戏无,近似减伤)", 18, "B"),
    (r"恐惧|威吓|震慑敌人", "恐惧(无直接对应)", 0, "C"),
    (r"陷阱|地雷", "陷阱/地雷域", 0, "B"),
    (r"额外投射物|投射物数量", "全局投射物数量", 0, "B"),
    (r"连锁|分裂|弹射", "投射物弹射/分裂", 0, "B"),
    (r"不朽|瓦尔技能", "瓦尔技能(游戏无)", 0, "D"),
    (r" amalgam|深渊|裂隙|五军|军团", "POE联赛机制", 0, "D"),
    (r"祭坛|圣物|圣油|涂膏", "POE涂膏(近似天赋被动)", 0, "C"),
    (r" specimen|蓝图|契约|走私|蓝图复制", "POE盗贼工会(无对应)", 0, "D"),
    (r"尸体|挖掘|探矿", "POE探矿(无对应)", 0, "D"),
    (r"药剂.*充能|充能上限|充能获取", "药剂充能(游戏药剂无充能,近似冷却)", 0, "B"),
    (r"药剂效果", "药剂持续时间/效果", 171, "A"),
    (r"避免.*异常|异常状态.*几率", "异常免疫(近似DOT减免)", 19, "B"),
    (r"移动技能|瞬移", "位移技能(游戏有FStype3)", 0, "B"),
    (r"复活时间|死亡时", "死亡机制(游戏无)", 0, "D"),
    (r"众神|女神|女神眷顾", "众神系统(无对应)", 0, "D"),
    (r"(火焰|冰霜|闪电)抗性", "单系元素抗性", 1, "A"),
    (r"附加.*基础混沌伤害", "混沌附加(游戏6系外,并入物理白字)", 3, "B"),
    (r"魔像", "魔像(=同伴系统)", 100, "B"),
    (r"药剂给予的", "药剂效果强化", 171, "A"),
    (r"格挡时", "格挡触发", 17, "B"),
    (r"偷取.*转为|转为治疗", "偷取转化", 650, "B"),
    (r"已损失.*生命|未保留生命", "生命阈值触发", 500, "B"),
    (r"智慧|敏捷|力量", "三围属性(游戏无三围系统)", 0, "D"),
    (r"近期|迷踪|属性需求", "POE专有状态(无对应)", 0, "D"),
    (r"^\(", "括号说明文本(不映射)", 0, "D"),
    (r"附加.*基础(火焰|冰霜|闪电|物理)伤害", "元素白字附加伤害", 3, "A"),
    (r"混沌抗性", "混沌抗性(游戏6系外,并入全抗)", 22, "B"),
    (r"(火焰|冰霜|闪电)与(火焰|冰霜|闪电)抗性|双抗", "双系抗性(并入全抗)", 22, "A"),
    (r"命中值|命中几率", "命中(游戏攻击必中,无对应)", 0, "D"),
    (r"技能石等级|弓技能石|法杖技能石", "技能等级加成(WPSkill域)", 0, "A"),
    (r"击败.*获得.*魔力|击败.*回复", "击杀回复/叠层", 1250, "B"),
    (r"结界|充能次数", "药剂充能(近似冷却)", 0, "B"),
    (r"钓鱼|鱼饵|史实", "POE钓鱼彩蛋(无对应)", 0, "D"),
    (r"插槽", "物品插槽数(不映射)", 0, "D"),
    (r"外延.*词缀|附魔：", "附魔域(游戏无附魔,近似符文FW)", 0, "B"),
    (r"既有的|影响效果|塑界者|征服者|裂界者", "POE影响底材(无对应)", 0, "D"),
    (r"袭击者|深渊珠宝|放逐|裂变", "POE联赛词缀(无对应)", 0, "D"),
    (r"战利品|盗贼|逃犯|赏金", "POE赏金(无对应)", 0, "D"),
]


def classify(text):
    for pat, domain, idx, grade in RULES:
        if re.search(pat, text):
            return domain, idx, grade
    return "未分类", 0, "?"


def main():
    data = json.load(open(SRC, encoding="utf-8"))
    pages = data["pages"]
    by_slot = defaultdict(lambda: defaultdict(list))
    stats = Counter()
    domain_counter = defaultdict(Counter)
    for page, pdata in pages.items():
        slot = slot_of(page)
        for sec in pdata["sections"]:
            title = sec["title"]
            # 跳过页脚/装备配方等噪音区
            if re.search(r"Sites|News|About|Community|Recipe|Acquisition|Alternate|导入|瓦尔宝珠|虚空忆境",
                         title):
                continue
            for m in sec["mods"]:
                domain, idx, grade = classify(m["text"])
                stats[grade] += 1
                domain_counter[slot][domain] += 1
                by_slot[slot][domain if domain != "未分类" else "未分类"].append({
                    "text": m["text"], "values": m.get("values", []),
                    "kind": m["kind"], "src_page": page, "src_section": title[:24],
                    "game_index": idx, "grade": grade,
                })
    out = {"schema_version": "1.0", "source": "poedb.tw/cn（112 页，基底详情页含词缀池）",
           "grade_meaning": {"A": "直接映射：游戏已有同义字段/机制", "B": "类似机制可改写（Tier2 补丁有先例）",
                             "C": "需新系统/较大改造", "D": "依赖 POE 专有系统，不建议实现"},
           "slots": {k: dict(v) for k, v in by_slot.items()}}
    json.dump(out, open(os.path.join(OUT, "affix_by_slot.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    stats_out = {"grade_total": dict(stats),
                 "slot_top_domains": {s: c.most_common(12) for s, c in domain_counter.items()}}
    json.dump(stats_out, open(os.path.join(OUT, "affix_mapping_stats.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    print("分级统计:", dict(stats))
    for s in by_slot:
        print("[%-14s] 词缀 %4d" % (s, sum(len(v) for v in by_slot[s].values())))


if __name__ == "__main__":
    main()
