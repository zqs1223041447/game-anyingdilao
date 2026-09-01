using FinkFramework.Runtime.Singleton;
using Localization;
using UnityEngine;

public class FilterManager : SingletonMonoGlobal<FilterManager>
{
	private bool _inited;

	public QulityType PLPick { get; private set; }

	public QulityType SPPick { get; private set; }

	public QulityType SPFJ { get; private set; }

	public void Init()
	{
		if (!_inited)
		{
			PLPick = (QulityType)Singleton<SettingDataManager>.Instance.Filter.Player_Auto_Pickup;
			SPPick = (QulityType)Singleton<SettingDataManager>.Instance.Filter.Sprite_Auto_Pickup;
			SPFJ = (QulityType)Singleton<SettingDataManager>.Instance.Filter.Sprite_Automatically_Salvages;
			_inited = true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Init();
	}

	public void SetFilterPL(QulityType qulity)
	{
		if (PLPick != qulity)
		{
			PLPick = qulity;
		}
	}

	public void SetFilterXJL(QulityType qulity)
	{
		if (SPPick != qulity)
		{
			SPPick = qulity;
		}
	}

	public void SetFilterXJL_FJ(QulityType qulity)
	{
		if (SPFJ != qulity)
		{
			SPFJ = qulity;
		}
	}

	public static bool CanPickByQuality(QulityType qulity, int itemQuality)
	{
		if (qulity == QulityType.Filter_Null)
		{
			return false;
		}
		return itemQuality >= GetFilterQuality(qulity);
	}

	public static bool CanSalvageByQuality(QulityType qulity, int itemQuality)
	{
		return qulity switch
		{
			QulityType.Filter_Null => false, 
			QulityType.Filter_Mythical => true, 
			_ => itemQuality <= GetFilterQuality(qulity), 
		};
	}

	private static int GetFilterQuality(QulityType qulity)
	{
		return Mathf.Clamp((int)(qulity - 1), 0, 6);
	}

	public static string GetPickDisplayName(QulityType qulity)
	{
		return qulity switch
		{
			QulityType.Filter_Null => "pc_pickup_mode_off", 
			QulityType.Filter_Normal => "Filter_Normal", 
			QulityType.Filter_Magic => "Filter_Magic", 
			QulityType.Filter_Rare => "Filter_Rare", 
			QulityType.Filter_Exquisite => "Filter_Exquisite", 
			QulityType.Filter_Epic => "Filter_Epic", 
			QulityType.Filter_Legendary => "Filter_Legendary", 
			QulityType.Filter_Mythical => "Filter_Mythical", 
			_ => "pc_pickup_mode_off", 
		};
	}

	public static string GetFJDisplayName(QulityType qulity)
	{
		return qulity switch
		{
			QulityType.Filter_Null => "pc_pickup_mode_off", 
			QulityType.Filter_Normal => "FJ_Normal", 
			QulityType.Filter_Magic => "FJ_Magic", 
			QulityType.Filter_Rare => "FJ_Rare", 
			QulityType.Filter_Exquisite => "FJ_Exquisite", 
			QulityType.Filter_Epic => "FJ_Epic", 
			QulityType.Filter_Legendary => "FJ_Legendary", 
			QulityType.Filter_Mythical => "FJ_Mythical", 
			_ => "pc_pickup_mode_off", 
		};
	}
}
