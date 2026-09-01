#!/usr/bin/env python3
"""Unity 序列化字段审计：对比两棵反编译树中同一类的序列化面。

用法:
    python serialize_audit.py <vanilla_file.cs> <mod_file.cs> [--verbose]

输出两侧参与 Unity 序列化的字段列表（按声明顺序）及差异。
判定规则（Unity Mono 后端）:
    序列化   = (public 且 非 const/static/readonly 且 无 NonSerialized) 或 (带 SerializeField)
    不序列化 = const / static / readonly / [NonSerialized] / 自动属性 / 方法
"""
import re
import sys

FIELD_RE = re.compile(
    r'^\s*(?P<access>public|private|protected|internal)\s+'
    r'(?P<mods>(?:(?:static|readonly|const|new|volatile)\s+)*)'
    r'(?P<type>[A-Za-z_][\w\.\<\>\[\],\s]*?)\s+'
    r'(?P<name>\w+)\s*(?:=[^;]*)?;\s*$'
)
ATTR_RE = re.compile(r'^\s*\[(?P<attr>[^\]]+)\]')
NONSERIAL_ATTRS = {'NonSerialized', 'System.NonSerialized', 'NonSerializedAttribute'}

# Unity 序列化对齐后的字节宽度（保守估计，用于大小推算）
WIDTH = {'bool': 4, 'char': 4, 'int': 4, 'float': 4, 'long': 8, 'double': 8,
         'short': 4, 'byte': 4, 'string': 4, 'Vector2': 8, 'Vector3': 12,
         'Vector4': 16, 'Color': 16, 'Color32': 4, 'Quaternion': 16}


TYPE_DECL_RE = re.compile(
    r'^\s*(?:\[[^\]]+\]\s*)*(?:public|private|protected|internal|static|abstract|sealed|partial|)*\s*'
    r'(?:partial\s+)?(?:class|struct|interface|enum)\s+\w+'
)


def parse(path):
    """返回 [(name, type, line_no)]，仅含参与序列化的字段。

    只采集"类成员层"（紧跟 class/struct 声明块内一层）的字段，
    方法体、属性访问器内的局部变量一律忽略。
    """
    out, pending = [], []
    with open(path, encoding='utf-8-sig', errors='replace') as fh:
        lines = fh.read().splitlines()
    depth = 0
    member_depth = None
    expect_brace = False
    for i, raw in enumerate(lines, 1):
        line = raw.strip()
        depth_before = depth
        depth += line.count('{') - line.count('}')
        if not line:
            continue
        m = ATTR_RE.match(line)
        if m:
            pending.append(m.group('attr').strip())
            continue
        if TYPE_DECL_RE.match(line):
            if member_depth is None:
                if '{' in line:
                    member_depth = depth
                    expect_brace = False
                else:
                    expect_brace = True
            pending = []
            continue
        if expect_brace and line.startswith('{'):
            if member_depth is None:
                member_depth = depth
            expect_brace = False
            continue
        if member_depth is None or depth_before != member_depth:
            pending = []
            continue
        fm = FIELD_RE.match(line)
        if fm and '=>' not in line:
            access = fm.group('access')
            mods = fm.group('mods').split()
            if 'const' in mods or 'static' in mods or 'readonly' in mods:
                pending = []
                continue
            has_serialize = any('SerializeField' in a for a in pending)
            has_nonserial = any(a.split('.')[-1] in NONSERIAL_ATTRS for a in pending)
            if (access == 'public' and not has_nonserial) or has_serialize:
                out.append((fm.group('name'), fm.group('type').strip(), i))
        pending = []
    return out


def width(ftype):
    base = ftype.replace('[]', '').strip()
    return WIDTH.get(base, 4)


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        return 2
    va, mo = sys.argv[1], sys.argv[2]
    verbose = '--verbose' in sys.argv
    vf, mf = parse(va), parse(mo)
    vn = [f[0] for f in vf]
    mn = [f[0] for f in mf]

    print(f"vanilla 序列化字段 {len(vf)} 个 | mod 序列化字段 {len(mf)} 个")
    vbytes = sum(width(t) for _, t, _ in vf)
    mbytes = sum(width(t) for _, t, _ in mf)
    print(f"估算序列化大小: vanilla ~{vbytes} B | mod ~{mbytes} B | 差 {mbytes - vbytes:+d} B")
    print()

    only_v = [f for f in vf if f[0] not in mn]
    only_m = [f for f in mf if f[0] not in vn]
    if only_v:
        print(">>> 仅 vanilla 有（mod 缺失，会导致 Read 偏小）:")
        for n, t, l in only_v:
            print(f"      L{l:<6} {t:<28} {n}   (~{width(t)} B)")
    if only_m:
        print(">>> 仅 mod 有（mod 多出，会导致 Read 偏大）:")
        for n, t, l in only_m:
            print(f"      L{l:<6} {t:<28} {n}   (~{width(t)} B)")
    if not only_v and not only_m:
        print(">>> 字段集合一致，检查声明顺序:")
        if vn != mn:
            for i, (a, b) in enumerate(zip(vn, mn)):
                if a != b:
                    print(f"      位置 {i}: vanilla={a}  mod={b}")
        else:
            print("      字段集合与顺序均一致。")

    if verbose:
        print("\n--- vanilla 全量 ---")
        for n, t, l in vf:
            print(f"  L{l:<6} {t:<28} {n}")
    return 0


if __name__ == '__main__':
    sys.exit(main())
