using System;
using UI.Map;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class InterfaceSettingData
{
	public bool map_toggle;

	public MapDisplayMode map_mode;

	public float map_scale;

	public float map_view_range;

	public float map_global_alpha;

	public float map_border_alpha;

	public bool damage_text;

	public float damage_scale;

	public int cursor;

	public bool display_item;

	public float cursor_speed;

	public bool aim_point;

	public bool item_tip;
}
