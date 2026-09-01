using System;
using Interact;

namespace Data.AutoGen.DataClass.Settings;

[Serializable]
public class GameSettingData
{
	public int language;

	public bool auto_save;

	public int auto_save_time;

	public bool left_invert_x;

	public bool left_invert_y;

	public bool right_invert_x;

	public bool right_invert_y;

	public bool autoChangeUseToggle;

	public PcPickupMode pcPickupMode;

	public bool mouse_move;

	public bool QZ_Move;

	public bool auto_lock1;

	public bool auto_lock2;

	public bool auto_attack;

	public int Dis_Skill1;

	public int Dis_Skill2;

	public int Dis_Skill3;

	public int Dis_Skill4;

	public int Dis_Skill5;

	public int Dis_Skill6;

	public int Dis_Skill7;

	public int Dis_Skill8;

	public bool AutoStop;
}
