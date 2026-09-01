using System.Globalization;
using FinkFramework.Runtime.Singleton;
using Localization;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
	private static Character instance;

	public CanvasGroup cav;

	private float time;

	public Text[] Main;

	public Text[] Heal1;

	public Text[] Heal2;

	public Text[] Mana1;

	public Text[] Mana2;

	public Text[] EL;

	public Text[] EL0;

	public Text[] EL1;

	public Text[] EL2;

	public Text[] EL3;

	public Text[] EL4;

	public Text[] EL5;

	public Text[] DMG1;

	public Text[] DMG2;

	public Text[] Speed1;

	public Text[] Speed2;

	public Text[] Crit1;

	public Text[] Crit2;

	public Text[] EXP1;

	public Text[] EXP2;

	public Text[] Strong1;

	public Text[] Strong2;

	public Text[] Drop1;

	public Text[] Drop2;

	public Text[] CP1;

	public Text[] CP2;

	private PlayerManager PL;

	public static Character Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<Character>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		cav = GetComponent<CanvasGroup>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
		time = 0f;
		RefreshUI();
	}

	private void Update()
	{
		Desplay();
	}

	private void OnEnable()
	{
		LOC.MM.OnLanguageChanged += OnLanguageChanged;
	}

	private void OnDisable()
	{
		LOC.MM.OnLanguageChanged -= OnLanguageChanged;
	}

	private void OnLanguageChanged(LanguageType lang)
	{
		RefreshUI();
	}

	public void Desplay()
	{
		time += Time.deltaTime;
		if (time >= 0.1f)
		{
			if (Mathf.Approximately(cav.alpha, 1f))
			{
				CanShu();
			}
			time = 0f;
		}
	}

	public void RefreshUI()
	{
		RefreshNewLabels();
		CanShu();
	}

	public void CanShu()
	{
		if (TryRefreshPlayer())
		{
			SetTexts(Main, EmptyIfNull(PL.PlayerName), LabelValue("Character_Role", GetRoleText()), LabelValue("Character_Level", PL.Level.ToString()), LabelValue("Character_ParagonLevel", PL.DFLevel.ToString()));
			SetTexts(Heal2, FormatNumber(GetStatMax(PL.HealStat)), FormatPercent(PL.Health_Bei_Last), FormatNumber(PL.Health_R_Base), FormatPercent(PL.Health_Percent_Last), FormatNumber(PL.Attack_R_health_Max));
			SetTexts(Mana2, FormatNumber(GetStatMax(PL.ManaStat)), FormatPercent(PL.Mana_Bei_Last), FormatNumber(PL.Mana_R_Base), FormatPercent(PL.Mana_Percent_Last), FormatNumber(PL.Attack_R_mana_Max));
			RefreshElementValues();
			SetTexts(DMG2, FormatNumber(PL.Damage_Last), FormatPercent(PL.Damage_Bei_Last + PL.Damage_Cut), FormatPercent(PL.BJrate_Last), FormatPercent(PL.BJDamage_Last));
			SetTexts(Speed2, FormatPercent(GetAttackSpeedIncrease()), FormatPercent(GetMoveSpeedIncrease()), FormatPercent(PL.CoolDown_Max), FormatPercent(PL.ManaXH));
			SetTexts(Crit2, FormatPercent(PL.ORB_Damage_Last), FormatPercent(GetDotDamageBonus()), FormatPercent(GetTrapDamageBonus()), FormatPercent(GetTotemEffectIncrease()));
			SetTexts(EXP2, FormatPercent(PL.FlySpeed), FormatPercent(PL.ThroughRate), FormatPercent(PL.EXP_Range), FormatPercent(PL.JYrate_Last));
			SetTexts(Strong2, FormatPercent(PL.GeDang_Max), FormatPercent(PL.Damage_Anti_Max), FormatPercent(PL.DOTcut_Last), FormatPercent(PL.AntiSlow_Max));
			SetTexts(Drop2, FormatPercent(PL.ItemDrop_Rate_Last), FormatPercent(PL.Xp_Bei_Tmp), FormatDecimal(PL.Pick_PL_Max), FormatDecimal(GetFairyPickupRange()));
			SetTexts(CP2, FormatPercent(PL.C_Damage_Last), FormatPercent(PL.C_Health_Last), FormatPercent(PL.C_ATSpeed_Last), FormatPercent(PL.C_MVSpeed_Last), FormatPercent(PL.C_AllAnti_Last));
		}
	}

	private void RefreshNewLabels()
	{
		SetTexts(Heal1, LOC.MM.GetMain("Character_Health"), LOC.MM.GetMain("Character_HealthMax"), LOC.MM.GetMain("Character_HealthRegen"), LOC.MM.GetMain("Character_PercentRecoveryPerSecond"), LOC.MM.GetMain("Character_KillHealth"));
		SetTexts(Mana1, LOC.MM.GetMain("Character_Mana"), LOC.MM.GetMain("Character_ManaMax"), LOC.MM.GetMain("Character_ManaRegen"), LOC.MM.GetMain("Character_PercentRecoveryPerSecond"), LOC.MM.GetMain("Character_KillMana"));
		SetTexts(EL, LOC.MM.GetMain("Character_ElementDamage"), LOC.MM.GetMain("Character_ElementDamageIncrease"), LOC.MM.GetMain("Character_ElementPenetration"), LOC.MM.GetMain("Character_ElementResistance"), LOC.MM.GetMain("Character_ElementDamageReduction"));
		SetTexts(DMG1, LOC.MM.GetMain("Character_Damage"), LOC.MM.GetMain("Character_DamageIncrease"), LOC.MM.GetMain("Character_CritRate"), LOC.MM.GetMain("Character_CritDamage"));
		SetTexts(Speed1, LOC.MM.GetMain("Character_AttackSpeed"), LOC.MM.GetMain("Character_MoveSpeed"), LOC.MM.GetMain("Character_CooldownReduction"), LOC.MM.GetMain("Character_ManaCostReduction"));
		SetTexts(Crit1, LOC.MM.GetMain("Character_AccessoryDamage"), LOC.MM.GetMain("Character_DotDamage"), LOC.MM.GetMain("Character_TrapDamage"), LOC.MM.GetMain("Character_TotemEffect"));
		SetTexts(EXP1, LOC.MM.GetMain("Character_ProjectileSpeed"), LOC.MM.GetMain("Character_PierceRate"), LOC.MM.GetMain("Character_ExplosionSkillRange"), LOC.MM.GetMain("Character_StunRate"));
		SetTexts(Strong1, LOC.MM.GetMain("Character_BlockRate"), LOC.MM.GetMain("Character_DamageReduction"), LOC.MM.GetMain("Character_DebuffDurationReduction"), LOC.MM.GetMain("Character_SlowResistance"));
		SetTexts(Drop1, LOC.MM.GetMain("Character_DropRate"), LOC.MM.GetMain("Character_ExperienceGain"), LOC.MM.GetMain("Character_AutoPickupRange"), LOC.MM.GetMain("Character_FairyPickupRange"));
		SetTexts(CP1, LOC.MM.GetMain("Character_CompDamage"), LOC.MM.GetMain("Character_CompHealth"), LOC.MM.GetMain("Character_CompAttackSpeed"), LOC.MM.GetMain("Character_CompMoveSpeed"), LOC.MM.GetMain("Character_CompAllResistance"));
	}

	private void RefreshElementValues()
	{
		SetElementTexts(EL0, DamageType.fire, PL.FireDamage, PL.FireDamage_Bei_Last + PL.FireDamageXi, PL.FireChuan_Last, PL.FireAnti_Last, PL.FireCut);
		SetElementTexts(EL1, DamageType.frozen, PL.FrozenDamage, PL.FrozenDamage_Bei_Last + PL.FrozenDamageXi, PL.FrozenChuan_Last, PL.FrozenAnti_Last, PL.FrozenCut);
		SetElementTexts(EL2, DamageType.thunder, PL.ThunderDamage, PL.ThunderDamage_Bei_Last + PL.ThunderDamageXi, PL.ThunderChuan_Last, PL.ThunderAnti_Last, PL.ThunderCut);
		SetElementTexts(EL3, DamageType.poison, PL.PoisonDamage, PL.PoisonDamage_Bei_Last + PL.PoisonDamageXi, PL.PoisonChuan_Last, PL.PoisonAnti_Last, PL.PoisonCut);
		SetElementTexts(EL4, DamageType.physics, PL.PhysicsDamage, PL.PhysicsDamage_Bei_Last + PL.PhysicsDamageXi, PL.PhysicsChuan_Last, PL.PhysicsAnti_Last, PL.PhysicsCut);
		SetElementTexts(EL5, DamageType.shadow, PL.ShadowDamage, PL.ShadowDamage_Bei_Last + PL.ShadowDamageXi, PL.ShadowChuan_Last, PL.ShadowAnti_Last, PL.ShadowCut);
	}

	private void SetElementTexts(Text[] row, DamageType type, float damage, float damageIncrease, float chuan, float anti, float damageAnti)
	{
		SetTexts(row, ElementColor(type, FormatNumber(damage)), ElementColor(type, FormatPercent(damageIncrease)), ElementColor(type, FormatPercent(chuan)), ElementColor(type, FormatPercent(anti)), ElementColor(type, FormatPercent(damageAnti)));
	}

	private bool TryRefreshPlayer()
	{
		if (PL == null && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
		return PL != null;
	}

	private string GetRoleText()
	{
		return PL.PLType switch
		{
			0 => LOC.MM.GetMain("Character_Class_MGC"), 
			1 => LOC.MM.GetMain("Character_Class_SQS"), 
			2 => LOC.MM.GetMain("Character_Class_ARC"), 
			3 => LOC.MM.GetMain("Character_Class_DEAD"), 
			_ => string.Empty, 
		};
	}

	private float GetDotDamageBonus()
	{
		return PL.AllDot_DMG + PL.BE_ZQ_Dot * (float)PL.BE_ZQ_Count + PL.BE_SPC_Dot * (float)PL.BE_SPC_Count + PL.BE_HH_Dot * (float)PL.BE_HH_Count + PL.Runtime_DotDamage_Tmp;
	}

	private float GetTrapDamageBonus()
	{
		return (float)PL.XJ_DMG + PL.Runtime_XJ_DMG_Tmp + (SingletonMonoScope<ACTbar>.HasInstance ? ((float)SingletonMonoScope<ACTbar>.Instance.GetEveryCompXJ_DMG()) : 0f) + PL.BE_ZQ_XJ_DMG * (float)PL.BE_ZQ_Count + PL.BE_SPC_XJ_DMG * (float)PL.BE_SPC_Count + PL.BE_HH_XJ_DMG * (float)PL.BE_HH_Count + PL.BE_SK_XJ_DMG * (float)PL.BE_SK_Count + PL.BE_BS_XJ_DMG * (float)PL.BE_BS_Count;
	}

	private float GetTotemEffectIncrease()
	{
		return PL.TuT_Buff;
	}

	private float GetAttackSpeedIncrease()
	{
		return GetPercentIncrease(PL.ATSpeed_Max, PL.ATSpeed_Base);
	}

	private float GetMoveSpeedIncrease()
	{
		return GetPercentIncrease(PL.MVSpeed_Max, PL.MVSpeed_Base);
	}

	private float GetFairyPickupRange()
	{
		return PL.Pick_XJL_Base + PL.Pick_XJL_Base * PL.Pick_XJL_Bei / 100f;
	}

	private static float GetPercentIncrease(float currentValue, float baseValue)
	{
		if (Mathf.Approximately(baseValue, 0f))
		{
			return 0f;
		}
		return (currentValue / baseValue - 1f) * 100f;
	}

	private static float GetStatCur(Stat stat)
	{
		if (!(stat != null))
		{
			return 0f;
		}
		return stat.Cur;
	}

	private static float GetStatMax(Stat stat)
	{
		if (!(stat != null))
		{
			return 0f;
		}
		return stat.Max;
	}

	private static void SetTexts(Text[] texts, params string[] values)
	{
		if (texts == null || values == null)
		{
			return;
		}
		int num = Mathf.Min(texts.Length, values.Length);
		for (int i = 0; i < num; i++)
		{
			if (texts[i] != null)
			{
				texts[i].text = values[i];
			}
		}
	}

	private static string LabelValue(string labelKey, string value)
	{
		return LOC.MM.GetMain(labelKey) + ": " + value;
	}

	private static string ElementColor(DamageType type, string value)
	{
		return "<color=" + DamageColor.Colors[type] + ">" + value + "</color>";
	}

	private static string EmptyIfNull(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return value;
		}
		return string.Empty;
	}

	private static string FormatNumber(float value)
	{
		if (value < 0f)
		{
			return "-" + DamgeTextManager.FormatDamageNumber(0f - value);
		}
		return DamgeTextManager.FormatDamageNumber(value);
	}

	private static string FormatPercent(float value)
	{
		return FormatDecimal(value) + "%";
	}

	private static string FormatDecimal(float value)
	{
		if (Mathf.Approximately(value, Mathf.Round(value)))
		{
			return Mathf.RoundToInt(value).ToString();
		}
		return value.ToString("0.#", CultureInfo.InvariantCulture);
	}
}
