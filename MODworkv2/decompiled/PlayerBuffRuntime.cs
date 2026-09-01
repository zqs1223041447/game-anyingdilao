using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class PlayerBuffRuntime : MonoBehaviour
{
	private sealed class TimedStackBuffInstance
	{
		public string Key;

		public PlayerRuntimeBuffSource Source;

		public int SourceId;

		public float Duration;

		public float RemainTime;

		public int Stack;

		public int MaxStack;

		public bool ClearOnExpire;

		public PlayerRuntimeBuffModifier PerStackModifier;
	}

	private sealed class PlayerLayerBuffInstance
	{
		public Buff_PL_Layer Config;

		public PlayerRuntimeBuffSource Source;

		public int SourceId;

		public float RemainTime;

		public bool FullModifierApplied;
	}

	public PlayerManager pl;

	private readonly List<TimedStackBuffInstance> timedStackBuffs = new List<TimedStackBuffInstance>();

	private readonly List<PlayerLayerBuffInstance> layerBuffs = new List<PlayerLayerBuffInstance>();

	private readonly Dictionary<int, Buff_PL_Layer> setLayerBuffConfigs = new Dictionary<int, Buff_PL_Layer>();

	private readonly Dictionary<ACTListSkillBT, List<SkillOBJ_DT_SP>> activeSkillInstances = new Dictionary<ACTListSkillBT, List<SkillOBJ_DT_SP>>();

	private float clear1Timer;

	private float clear2Timer;

	private float deadWdTimer;

	private float deadRageWdTimer;

	private float deadStealthWdTimer;

	private float wuDiTimer;

	private float moveChargeTimer;

	private float moveDecayTimer;

	private float noUseSkillTimer;

	private int noUseSkill1Stack;

	private int noUseSkill2Stack;

	private PlayerRuntimeBuffModifier dynamicModifier = new PlayerRuntimeBuffModifier();

	private PlayerRuntimeBuffModifier appliedDynamicModifier = new PlayerRuntimeBuffModifier();

	private bool dynamicApplied;

	private void Awake()
	{
		pl = (base.transform.parent ? base.transform.parent.GetComponent<PlayerManager>() : GetComponent<PlayerManager>());
	}

	private void OnDisable()
	{
		ClearAllRuntimeBuffs();
		RemoveDynamicModifier();
	}

	private void Update()
	{
		if ((bool)pl && pl.IsAlive)
		{
			TickTimedStackBuffs();
			TickLayerBuffs();
			TickUtilityTimers();
			RefreshDynamicModifier();
		}
	}

	public void ClearAllRuntimeBuffs()
	{
		for (int num = timedStackBuffs.Count - 1; num >= 0; num--)
		{
			RemoveTimedStackBuffAt(num);
		}
		for (int num2 = layerBuffs.Count - 1; num2 >= 0; num2--)
		{
			RemoveLayerBuffAt(num2);
		}
		noUseSkill1Stack = 0;
		noUseSkill2Stack = 0;
		moveChargeTimer = 0f;
		moveDecayTimer = 0f;
		noUseSkillTimer = 0f;
		wuDiTimer = 0f;
		deadWdTimer = 0f;
		deadRageWdTimer = 0f;
		deadStealthWdTimer = 0f;
		activeSkillInstances.Clear();
		RemoveDynamicModifier();
	}

	public void ClearBySource(PlayerRuntimeBuffSource source, int sourceId)
	{
		for (int num = timedStackBuffs.Count - 1; num >= 0; num--)
		{
			TimedStackBuffInstance timedStackBuffInstance = timedStackBuffs[num];
			if (timedStackBuffInstance.Source == source && timedStackBuffInstance.SourceId == sourceId)
			{
				RemoveTimedStackBuffAt(num);
			}
		}
		for (int num2 = layerBuffs.Count - 1; num2 >= 0; num2--)
		{
			PlayerLayerBuffInstance playerLayerBuffInstance = layerBuffs[num2];
			if (playerLayerBuffInstance.Source == source && playerLayerBuffInstance.SourceId == sourceId)
			{
				RemoveLayerBuffAt(num2);
			}
		}
	}

	public void OnSkillUsed(ACTListSkillBT skill)
	{
		if (CanApplyRuntimeBuff() && (bool)skill && skill.DT != null && skill.DT.type == 0 && skill.DT.simple != null)
		{
			ACT_skillSample simple = skill.DT.simple;
			simple.EnsureRuntimeBuffDefaults();
			string skillName = (string.IsNullOrEmpty(skill.IndexName) ? skill.DT.IndexName : skill.IndexName);
			AddSkillUseBuffs(skillName, simple);
			AddAttackBuffs(simple);
			TriggerSetLayerBuff(PlayerRuntimeBuffSource.Attack);
			AddNoUseSkillAttackClear();
			RemoveTimedStackBuff("MV_DMG");
			moveChargeTimer = 0f;
			moveDecayTimer = 0f;
			if (simple.WD > 0f)
			{
				AddWuDi(simple.WD);
			}
			if (simple.HasAnyRuntimePresenceBuff())
			{
				AddPresenceBuff(skillName, simple);
			}
		}
	}

	public void OnSkillCrit(ACTListSkillBT skill)
	{
		if (CanApplyRuntimeBuff() && (bool)skill && skill.DT != null && skill.DT.type == 0 && skill.DT.simple != null)
		{
			ACT_skillSample simple = skill.DT.simple;
			simple.EnsureRuntimeBuffDefaults();
			ApplySkillCritBonuses(skill, simple.Crit_Time, simple.Crit_CD, null);
		}
	}

	public void OnSkillCrit(SkillOBJ_DT_SP skillSource)
	{
		if (!CanApplyRuntimeBuff() || !skillSource)
		{
			return;
		}
		ACTListSkillBT aCTListSkillBT = FindRegisteredSkillOwner(skillSource);
		if (!aCTListSkillBT && SingletonMonoScope<ACTbar>.HasInstance)
		{
			aCTListSkillBT = SingletonMonoScope<ACTbar>.Instance.FindSampleSkillByName(skillSource.skillName);
		}
		int crit_Time = skillSource.Crit_Time;
		int crit_CD = skillSource.Crit_CD;
		if ((bool)aCTListSkillBT && aCTListSkillBT.DT != null && aCTListSkillBT.DT.type == 0 && aCTListSkillBT.DT.simple != null)
		{
			ACT_skillSample simple = aCTListSkillBT.DT.simple;
			simple.EnsureRuntimeBuffDefaults();
			if (simple.Crit_Time > 0)
			{
				crit_Time = simple.Crit_Time;
			}
			if (simple.Crit_CD > 0)
			{
				crit_CD = simple.Crit_CD;
			}
		}
		ApplySkillCritBonuses(aCTListSkillBT, crit_Time, crit_CD, skillSource);
	}

	public void OnEnemyKilled(Enemy enemy)
	{
		if (CanApplyRuntimeBuff())
		{
			AddKillBuffs(enemy);
			TriggerSetLayerBuff(PlayerRuntimeBuffSource.Kill);
			if (pl.Kem_Refresh > 0 && Random.value < 0.01f)
			{
				RefreshAllSkillCooldowns();
				AddTimedStackBuff("Kem_Refresh", PlayerRuntimeBuffSource.Kill, 0, 3f, 1, 1, MakeModifier(pl.Kem_Refresh), clearOnExpire: true);
			}
		}
	}

	public void OnBlock()
	{
		if (CanApplyRuntimeBuff() && pl.GD_DMG > 0)
		{
			AddTimedStackBuff("GD_DMG", PlayerRuntimeBuffSource.Block, 0, 6f, 6, 1, MakeModifier(pl.GD_DMG), clearOnExpire: false);
		}
	}

	public void OnPlayerHit()
	{
	}

	public void OnPickGem()
	{
		if (CanApplyRuntimeBuff() && pl.PickBS_MVS > 0)
		{
			AddTimedStackBuff("PickBS_MVS", PlayerRuntimeBuffSource.System, 0, 3f, 1, 1, MakeModifier(0f, 0f, pl.PickBS_MVS), clearOnExpire: true);
		}
	}

	public void AddLayerBuff(Buff_PL_Layer config, PlayerRuntimeBuffSource source, int sourceId)
	{
		if (CanApplyRuntimeBuff() && config != null && !string.IsNullOrEmpty(config.BuffName) && config.LayerMax > 0)
		{
			PlayerLayerBuffInstance playerLayerBuffInstance = FindLayerBuff(config.BuffName);
			if (playerLayerBuffInstance == null)
			{
				playerLayerBuffInstance = new PlayerLayerBuffInstance
				{
					Config = CloneLayerBuff(config),
					Source = source,
					SourceId = sourceId
				};
				layerBuffs.Add(playerLayerBuffInstance);
			}
			playerLayerBuffInstance.RemainTime = Mathf.Max(0f, playerLayerBuffInstance.Config.BuffTime);
			AddLayer(playerLayerBuffInstance, 1);
		}
	}

	public void RegisterSetLayerBuff(int setId, Buff_PL_Layer config)
	{
		if (setId > 0 && config != null && !string.IsNullOrEmpty(config.BuffName) && config.LayerMax > 0)
		{
			ClearBySource(PlayerRuntimeBuffSource.Set, setId);
			setLayerBuffConfigs[setId] = CloneLayerBuff(config);
		}
	}

	public void UnregisterSetLayerBuff(int setId)
	{
		if (setId > 0)
		{
			setLayerBuffConfigs.Remove(setId);
			ClearBySource(PlayerRuntimeBuffSource.Set, setId);
		}
	}

	public void ClearSetLayerBuffs()
	{
		setLayerBuffConfigs.Clear();
		for (int num = layerBuffs.Count - 1; num >= 0; num--)
		{
			if (layerBuffs[num].Source == PlayerRuntimeBuffSource.Set)
			{
				RemoveLayerBuffAt(num);
			}
		}
	}

	public void TriggerSetLayerBuff(int buffName)
	{
		TriggerSetLayerBuff(buffName.ToString());
	}

	public void TriggerSetLayerBuff(string buffName)
	{
		if (!CanApplyRuntimeBuff() || string.IsNullOrEmpty(buffName))
		{
			return;
		}
		foreach (KeyValuePair<int, Buff_PL_Layer> setLayerBuffConfig in setLayerBuffConfigs)
		{
			if (setLayerBuffConfig.Value != null && setLayerBuffConfig.Value.BuffName == buffName)
			{
				AddLayerBuff(setLayerBuffConfig.Value, PlayerRuntimeBuffSource.Set, setLayerBuffConfig.Key);
			}
		}
	}

	public void TriggerSetLayerBuff(PlayerRuntimeBuffSource source)
	{
		if (!CanApplyRuntimeBuff() || (source != PlayerRuntimeBuffSource.Attack && source != PlayerRuntimeBuffSource.Kill))
		{
			return;
		}
		foreach (KeyValuePair<int, Buff_PL_Layer> setLayerBuffConfig in setLayerBuffConfigs)
		{
			Buff_PL_Layer value = setLayerBuffConfig.Value;
			if (value != null && !string.IsNullOrEmpty(value.BuffName) && value.LayerMax > 0)
			{
				int num = value.BuffType;
				if (num != 0 && num != 1)
				{
					num = 0;
				}
				if ((num != 0 || source == PlayerRuntimeBuffSource.Attack) && (num != 1 || source == PlayerRuntimeBuffSource.Kill))
				{
					AddLayerBuff(value, PlayerRuntimeBuffSource.Set, setLayerBuffConfig.Key);
				}
			}
		}
	}

	public void RegisterSkillInstance(ACTListSkillBT skill, SkillOBJ_DT_SP instance)
	{
		if (CanApplyRuntimeBuff() && (bool)skill && (bool)instance && instance.indexType == 0 && !(instance.BuffTime <= 0f) && (bool)instance.GetComponent<SK_BuffA>())
		{
			if (!activeSkillInstances.TryGetValue(skill, out var value))
			{
				value = new List<SkillOBJ_DT_SP>();
				activeSkillInstances.Add(skill, value);
			}
			if (!value.Contains(instance))
			{
				value.Add(instance);
			}
		}
	}

	public void UnregisterSkillInstance(SkillOBJ_DT_SP instance, bool naturalEnd)
	{
		if (!instance)
		{
			return;
		}
		ACTListSkillBT aCTListSkillBT = null;
		bool flag = false;
		foreach (KeyValuePair<ACTListSkillBT, List<SkillOBJ_DT_SP>> activeSkillInstance in activeSkillInstances)
		{
			if (activeSkillInstance.Value.Remove(instance))
			{
				aCTListSkillBT = activeSkillInstance.Key;
				flag = true;
				break;
			}
		}
		if (aCTListSkillBT != null && activeSkillInstances.TryGetValue(aCTListSkillBT, out var value) && value.Count == 0)
		{
			activeSkillInstances.Remove(aCTListSkillBT);
		}
		if (flag && naturalEnd && instance.Over_Prc > 0 && (bool)pl && pl.HealStat != null && pl.IsAlive)
		{
			HealPlayerByPercent(instance.Over_Prc);
		}
	}

	private ACTListSkillBT FindRegisteredSkillOwner(SkillOBJ_DT_SP instance)
	{
		if (!instance)
		{
			return null;
		}
		foreach (KeyValuePair<ACTListSkillBT, List<SkillOBJ_DT_SP>> activeSkillInstance in activeSkillInstances)
		{
			if (activeSkillInstance.Value != null && activeSkillInstance.Value.Contains(instance))
			{
				return activeSkillInstance.Key;
			}
		}
		return null;
	}

	private void ApplySkillCritBonuses(ACTListSkillBT skill, int critTime, int critCD, SkillOBJ_DT_SP skillSource)
	{
		string skillName = GetSkillName(skill, skillSource);
		if (critTime > 0 && Random.value < (float)critTime * 0.01f)
		{
			if ((!skill || !ExtendActiveSkillInstances(skill, 1f)) && (bool)skillSource)
			{
				skillSource.BuffTime += 1f;
			}
			if (!string.IsNullOrEmpty(skillName))
			{
				ExtendTimedStackBuffDuration("Has:" + skillName, 1f);
			}
		}
		if ((bool)skill && critCD > 0 && Random.value < (float)critCD * 0.01f)
		{
			skill.JStimeA += 1f;
			if (skill.JStimeA >= skill.CDTime)
			{
				skill.ResetCD();
			}
		}
	}

	private string GetSkillName(ACTListSkillBT skill, SkillOBJ_DT_SP skillSource)
	{
		if (skill != null && skill.DT != null)
		{
			if (!string.IsNullOrEmpty(skill.IndexName))
			{
				return skill.IndexName;
			}
			return skill.DT.IndexName;
		}
		if (!skillSource)
		{
			return string.Empty;
		}
		return skillSource.skillName;
	}

	private void AddSkillUseBuffs(string skillName, ACT_skillSample simple)
	{
		AddTimedStackBuffIfValue("UseDMG:" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, simple.UseDMG, 4f, 5, MakeModifier(simple.UseDMG));
		AddTimedStackBuffIfValue("UseATS:" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, simple.UseATS, 3f, 4, MakeModifier(0f, simple.UseATS));
		AddTimedStackBuffIfValue("UseMVS:" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, simple.UseMVS, 2f, 3, MakeModifier(0f, 0f, simple.UseMVS));
		for (int i = 0; i < 6; i++)
		{
			int num = simple.UseDMG_EL[i];
			if (num > 0)
			{
				PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier();
				playerRuntimeBuffModifier.ElementDamage[i] = num;
				AddTimedStackBuff("UseDMG_EL" + i + ":" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, 3f, 4, 1, playerRuntimeBuffModifier, clearOnExpire: false);
			}
			int num2 = simple.UseChuan[i];
			if (num2 > 0)
			{
				PlayerRuntimeBuffModifier playerRuntimeBuffModifier2 = new PlayerRuntimeBuffModifier();
				playerRuntimeBuffModifier2.ElementChuan[i] = num2;
				AddTimedStackBuff("UseChuan" + i + ":" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, 2f, 3, 1, playerRuntimeBuffModifier2, clearOnExpire: false);
			}
		}
		AddTimedStackBuffIfValue("UseCP_DMG:" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, simple.UseCP_DMG, 3f, 4, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, simple.UseCP_DMG));
		AddTimedStackBuffIfValue("UseCP_ATS:" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, simple.UseCP_ATS, 2f, 4, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, simple.UseCP_ATS));
	}

	private void AddAttackBuffs(ACT_skillSample simple)
	{
		AddTimedStackBuffIfValue("Attack_DMG1", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_DMG1, 4f, 5, MakeModifier(pl.Attack_DMG1));
		AddTimedStackBuffIfValue("Attack_DMG2", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_DMG2, 3f, 6, MakeModifier(pl.Attack_DMG2));
		AddTimedStackBuffIfValue("Attack_ATS1", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_ATS1, 3f, 6, MakeModifier(0f, pl.Attack_ATS1));
		AddTimedStackBuffIfValue("Attack_ATS2", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_ATS2, 3f, 8, MakeModifier(0f, pl.Attack_ATS2));
		AddTimedStackBuffIfValue("Attack_Chuan", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_Chuan, 3f, 6, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Attack_Chuan));
		AddTimedStackBuffIfValue("Attack_BJR", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_BJR, 3f, 10, MakeModifier(0f, 0f, 0f, pl.Attack_BJR));
		AddTimedStackBuffIfValue("Attack_BJD", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_BJD, 3f, 12, MakeModifier(0f, 0f, 0f, 0f, pl.Attack_BJD));
		AddTimedStackBuffIfValue("Attack_DotDMG1", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_DotDMG1, 3f, 6, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Attack_DotDMG1));
		AddTimedStackBuffIfValue("Attack_DotDMG2", PlayerRuntimeBuffSource.Attack, 0, pl.Attack_DotDMG2, 4f, 8, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Attack_DotDMG2));
		if (pl.TP_DMG > 0 && simple != null && (simple.DashSkill || simple.TPSkill))
		{
			AddTimedStackBuff("TP_DMG", PlayerRuntimeBuffSource.Attack, 0, 3f, 10, 1, MakeModifier(pl.TP_DMG), clearOnExpire: false);
		}
		if (pl.Final_Diff_DMG <= 0 || simple == null || !simple.LastSkill)
		{
			return;
		}
		PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier();
		int num = Mathf.Clamp(simple.MainEL, 0, 5);
		for (int i = 0; i < 6; i++)
		{
			if (i != num)
			{
				playerRuntimeBuffModifier.ElementDamage[i] = pl.Final_Diff_DMG;
			}
		}
		AddTimedStackBuff("Final_Diff_DMG:" + num, PlayerRuntimeBuffSource.Attack, 0, 3f, 99, 1, playerRuntimeBuffModifier, clearOnExpire: false);
	}

	private void AddPresenceBuff(string skillName, ACT_skillSample simple)
	{
		PlayerRuntimeBuffModifier perStackModifier = MakeModifier(simple.Has_DMG, simple.Has_ATS, simple.Has_MVS, simple.Has_BJR, simple.Has_BJD, simple.Has_DMG_Cut, 0f, dotTimeCut: simple.Has_DotTimeCut, dotDamage: simple.Has_Dot_DMG, companionDamage: simple.Has_CP_DMG, companionAttackSpeed: 0f, allChuan: 0f, elementAnti: 0f, geDang: simple.Has_GD, orbDamage: simple.Has_ORB_DMG, trapDamage: simple.Has_XJ_DMG);
		float duration = Mathf.Max(0.1f, simple.BuffTime);
		AddTimedStackBuff("Has:" + skillName, PlayerRuntimeBuffSource.SkillUse, 0, duration, 1, 1, perStackModifier, clearOnExpire: true);
	}

	private void AddKillBuffs(Enemy enemy)
	{
		AddTimedStackBuffIfValue("Kem_DMG1", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_DMG1, 3f, 8, MakeModifier(pl.Kem_DMG1));
		AddTimedStackBuffIfValue("Kem_DMG2", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_DMG2, 5f, 10, MakeModifier(pl.Kem_DMG2));
		AddTimedStackBuffIfValue("Kem_ATS1", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_ATS1, 3f, 5, MakeModifier(0f, pl.Kem_ATS1));
		AddTimedStackBuffIfValue("Kem_ATS2", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_ATS2, 5f, 8, MakeModifier(0f, pl.Kem_ATS2));
		AddKillElementBuff(0, pl.Kem_EL0, 6f, 8);
		AddKillElementBuff(1, pl.Kem_EL1, 5f, 6);
		AddKillElementBuff(2, pl.Kem_EL2, 6f, 8);
		AddKillElementBuff(3, pl.Kem_EL3, 5f, 6);
		AddKillElementBuff(4, pl.Kem_EL4, 3f, 4);
		AddKillElementBuff(5, pl.Kem_EL5, 6f, 8);
		AddTimedStackBuffIfValue("Kem_CP_DMG1", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_CP_DMG1, 2f, 4, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Kem_CP_DMG1));
		AddTimedStackBuffIfValue("Kem_CP_DMG2", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_CP_DMG2, 3f, 6, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Kem_CP_DMG2));
		AddTimedStackBuffIfValue("Kem_CP_ATS1", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_CP_ATS1, 2f, 5, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Kem_CP_ATS1));
		AddTimedStackBuffIfValue("Kem_CP_ATS2", PlayerRuntimeBuffSource.Kill, 0, pl.Kem_CP_ATS2, 3f, 6, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Kem_CP_ATS2));
		if (enemy != null && enemy.Quality > 2)
		{
			AddTimedStackBuffIfValue("Kjy_DMG", PlayerRuntimeBuffSource.Kill, 0, pl.Kjy_DMG, 5f, 3, MakeModifier(pl.Kjy_DMG));
			AddTimedStackBuffIfValue("Kjy_AllAnti", PlayerRuntimeBuffSource.Kill, 0, pl.Kjy_AllAnti, 3f, 3, MakeModifier(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, pl.Kjy_AllAnti));
		}
	}

	private void AddKillElementBuff(int element, int value, float duration, int maxStack)
	{
		if (value > 0)
		{
			PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier();
			playerRuntimeBuffModifier.ElementDamage[element] = value;
			AddTimedStackBuff("Kem_EL" + element, PlayerRuntimeBuffSource.Kill, 0, duration, maxStack, 1, playerRuntimeBuffModifier, clearOnExpire: false);
		}
	}

	private void AddTimedStackBuffIfValue(string key, PlayerRuntimeBuffSource source, int sourceId, int value, float duration, int maxStack, PlayerRuntimeBuffModifier modifier)
	{
		if (value > 0)
		{
			AddTimedStackBuff(key, source, sourceId, duration, maxStack, 1, modifier, clearOnExpire: false);
		}
	}

	private void AddTimedStackBuff(string key, PlayerRuntimeBuffSource source, int sourceId, float duration, int maxStack, int addStack, PlayerRuntimeBuffModifier perStackModifier, bool clearOnExpire)
	{
		if (CanApplyRuntimeBuff() && !string.IsNullOrEmpty(key) && perStackModifier != null && !perStackModifier.IsEmpty())
		{
			TimedStackBuffInstance timedStackBuffInstance = FindTimedStackBuff(key);
			if (timedStackBuffInstance == null)
			{
				timedStackBuffInstance = new TimedStackBuffInstance
				{
					Key = key,
					Source = source,
					SourceId = sourceId,
					Duration = Mathf.Max(0.01f, duration),
					RemainTime = Mathf.Max(0.01f, duration),
					MaxStack = Mathf.Max(1, maxStack),
					Stack = 0,
					ClearOnExpire = clearOnExpire,
					PerStackModifier = perStackModifier.Clone()
				};
				timedStackBuffs.Add(timedStackBuffInstance);
			}
			else
			{
				RemoveModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
				timedStackBuffInstance.Duration = Mathf.Max(0.01f, duration);
				timedStackBuffInstance.RemainTime = timedStackBuffInstance.Duration;
				timedStackBuffInstance.MaxStack = Mathf.Max(1, maxStack);
				timedStackBuffInstance.ClearOnExpire = clearOnExpire;
				timedStackBuffInstance.PerStackModifier = perStackModifier.Clone();
			}
			timedStackBuffInstance.Stack = Mathf.Clamp(timedStackBuffInstance.Stack + Mathf.Max(1, addStack), 1, timedStackBuffInstance.MaxStack);
			timedStackBuffInstance.RemainTime = timedStackBuffInstance.Duration;
			ApplyModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
			RefreshRuntimeDerivedStats();
		}
	}

	private void TickTimedStackBuffs()
	{
		float deltaTime = Time.deltaTime;
		for (int num = timedStackBuffs.Count - 1; num >= 0; num--)
		{
			TimedStackBuffInstance timedStackBuffInstance = timedStackBuffs[num];
			timedStackBuffInstance.RemainTime -= deltaTime;
			if (!(timedStackBuffInstance.RemainTime > 0f))
			{
				if (timedStackBuffInstance.Source == PlayerRuntimeBuffSource.Set && !timedStackBuffInstance.ClearOnExpire && timedStackBuffInstance.Stack > 1)
				{
					RemoveModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
					timedStackBuffInstance.Stack--;
					timedStackBuffInstance.RemainTime = timedStackBuffInstance.Duration;
					ApplyModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
					RefreshRuntimeDerivedStats();
				}
				else
				{
					RemoveTimedStackBuffAt(num);
				}
			}
		}
	}

	private void TickLayerBuffs()
	{
		float deltaTime = Time.deltaTime;
		for (int num = layerBuffs.Count - 1; num >= 0; num--)
		{
			PlayerLayerBuffInstance playerLayerBuffInstance = layerBuffs[num];
			if (playerLayerBuffInstance.Config.BuffTime <= 0f)
			{
				RemoveLayerBuffAt(num);
			}
			else
			{
				playerLayerBuffInstance.RemainTime -= deltaTime;
				if (playerLayerBuffInstance.RemainTime <= 0f)
				{
					AddLayer(playerLayerBuffInstance, -1);
					if (playerLayerBuffInstance.Config.LayerCur <= 0)
					{
						RemoveLayerBuffAt(num);
					}
					else
					{
						playerLayerBuffInstance.RemainTime = Mathf.Max(0f, playerLayerBuffInstance.Config.BuffTime);
					}
				}
			}
		}
	}

	private void AddLayer(PlayerLayerBuffInstance instance, int delta)
	{
		if (instance != null && instance.Config != null)
		{
			Buff_PL_Layer config = instance.Config;
			int num = Mathf.Clamp(config.LayerCur, 0, Mathf.Max(0, config.LayerMax));
			int num2 = Mathf.Clamp(num + delta, 0, Mathf.Max(0, config.LayerMax));
			if (num != num2)
			{
				PlayerRuntimeBuffModifier modifier = MakeModifierFromLayer(config, full: false);
				RemoveModifier(modifier, num);
				config.LayerCur = num2;
				ApplyModifier(modifier, num2);
				RefreshLayerFullModifier(instance, num, num2);
				RefreshRuntimeDerivedStats();
			}
		}
	}

	private void RefreshLayerFullModifier(PlayerLayerBuffInstance instance, int oldLayer, int newLayer)
	{
		bool flag = oldLayer >= instance.Config.LayerMax && instance.Config.LayerMax > 0;
		bool flag2 = newLayer >= instance.Config.LayerMax && instance.Config.LayerMax > 0;
		if (flag != flag2)
		{
			PlayerRuntimeBuffModifier modifier = MakeModifierFromLayer(instance.Config, full: true);
			if (flag)
			{
				RemoveModifier(modifier, 1);
				instance.FullModifierApplied = false;
			}
			if (flag2)
			{
				ApplyModifier(modifier, 1);
				instance.FullModifierApplied = true;
			}
		}
	}

	private void TickUtilityTimers()
	{
		float deltaTime = Time.deltaTime;
		if (pl.Clear1 > 0)
		{
			clear1Timer += deltaTime;
			if (clear1Timer >= 5f)
			{
				clear1Timer = 0f;
				ClearDebuffAndHeal(pl.Clear1);
			}
		}
		if (pl.Clear2 > 0)
		{
			clear2Timer += deltaTime;
			if (clear2Timer >= 3f)
			{
				clear2Timer = 0f;
				ClearDebuffAndHeal(pl.Clear2);
			}
		}
		if (pl.DeadWD)
		{
			if (deadWdTimer < 30f)
			{
				deadWdTimer += deltaTime;
			}
		}
		else
		{
			deadWdTimer = 0f;
		}
		if (pl.DeadRageWD)
		{
			if (deadRageWdTimer < 24f)
			{
				deadRageWdTimer += deltaTime;
			}
		}
		else
		{
			deadRageWdTimer = 0f;
		}
		if (pl.DeadStealthWD)
		{
			if (deadStealthWdTimer < 25f)
			{
				deadStealthWdTimer += deltaTime;
			}
		}
		else
		{
			deadStealthWdTimer = 0f;
		}
		if (wuDiTimer > 0f)
		{
			wuDiTimer -= deltaTime;
		}
		TickMoveDamage(deltaTime);
		TickNoUseSkill(deltaTime);
	}

	private void TickMoveDamage(float dt)
	{
		if (pl.MV_DMG <= 0)
		{
			return;
		}
		if (pl.IsMoving)
		{
			moveDecayTimer = 0f;
			moveChargeTimer += dt;
			if (moveChargeTimer >= 1f)
			{
				moveChargeTimer = 0f;
				AddTimedStackBuff("MV_DMG", PlayerRuntimeBuffSource.Move, 0, 9999f, 4, 1, MakeModifier(pl.MV_DMG), clearOnExpire: false);
			}
		}
		else
		{
			moveChargeTimer = 0f;
			moveDecayTimer += dt;
			if (moveDecayTimer >= 2f)
			{
				moveDecayTimer = 0f;
				ReduceTimedStackBuff("MV_DMG", 1);
			}
		}
	}

	private void TickNoUseSkill(float dt)
	{
		if (pl.NoUseSK_DMG1 > 0 || pl.NoUseSK_DMG2 > 0)
		{
			noUseSkillTimer += dt;
			if (!(noUseSkillTimer < 1f))
			{
				noUseSkillTimer = 0f;
				noUseSkill1Stack = AddManualStack("NoUseSK_DMG1", noUseSkill1Stack, 5, pl.NoUseSK_DMG1);
				noUseSkill2Stack = AddManualStack("NoUseSK_DMG2", noUseSkill2Stack, 8, pl.NoUseSK_DMG2);
			}
		}
	}

	private int AddManualStack(string key, int currentStack, int maxStack, int value)
	{
		if (value <= 0 || currentStack >= maxStack)
		{
			return currentStack;
		}
		AddTimedStackBuff(key, PlayerRuntimeBuffSource.Attack, 0, 9999f, maxStack, 1, MakeModifier(value), clearOnExpire: false);
		return currentStack + 1;
	}

	private void AddNoUseSkillAttackClear()
	{
		RemoveTimedStackBuff("NoUseSK_DMG1");
		RemoveTimedStackBuff("NoUseSK_DMG2");
		noUseSkill1Stack = 0;
		noUseSkill2Stack = 0;
		noUseSkillTimer = 0f;
	}

	private void ClearDebuffAndHeal(int healPercent)
	{
		if ((bool)pl.BuffMG && pl.BuffMG.HasDebuff())
		{
			pl.BuffMG.DelOneDebuff();
			HealPlayerByPercent(healPercent);
		}
	}

	private void HealPlayerByPercent(float percent)
	{
		if (CanApplyRuntimeBuff() && !(pl.HealStat == null) && !(percent <= 0f))
		{
			pl.HealStat.Cur = Mathf.Min(pl.HealStat.Cur + pl.HealStat.Max * percent / 100f, pl.HealStat.Max);
		}
	}

	private void RefreshDynamicModifier()
	{
		PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier();
		if (pl.BuffEvery_CP > 0 && (bool)pl.BuffMG)
		{
			playerRuntimeBuffModifier.CompanionDamage += pl.BuffEvery_CP * pl.BuffMG.GetBuffKindCount();
		}
		int playerDebuffDotKindCount = GetPlayerDebuffDotKindCount();
		if (playerDebuffDotKindCount > 0)
		{
			playerRuntimeBuffModifier.MoveSpeed += pl.Z_Dot_MV * playerDebuffDotKindCount;
			for (int i = 0; i < 6; i++)
			{
				playerRuntimeBuffModifier.ElementDamage[i] += pl.Z_Dot_EL * playerDebuffDotKindCount;
			}
		}
		RemoveDynamicModifier();
		dynamicModifier = playerRuntimeBuffModifier;
		ApplyModifier(dynamicModifier, 1);
		appliedDynamicModifier = dynamicModifier.Clone();
		dynamicApplied = true;
	}

	private int GetPlayerDebuffDotKindCount()
	{
		if (!pl.BuffMG)
		{
			return 0;
		}
		bool[] array = new bool[6];
		for (int i = 0; i < pl.BuffMG.list.Count; i++)
		{
			Buffer_PL buffer_PL = pl.BuffMG.list[i];
			if ((bool)buffer_PL && buffer_PL.buff != null && buffer_PL.buff.type == 0 && !(buffer_PL.buff.DotDamage <= 0f))
			{
				int num = pl.GiveInt(buffer_PL.buff.damageType);
				array[num] = true;
			}
		}
		int num2 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j])
			{
				num2++;
			}
		}
		return num2;
	}

	private void RemoveDynamicModifier()
	{
		if (dynamicApplied)
		{
			RemoveModifier(appliedDynamicModifier, 1);
			dynamicApplied = false;
		}
	}

	public bool TryPreventFatalDamage()
	{
		if (!CanApplyRuntimeBuff() || pl.HealStat == null)
		{
			return false;
		}
		if (pl.DeadWD && deadWdTimer >= 30f)
		{
			deadWdTimer = 0f;
			AddWuDi(1f);
			AddTimedStackBuff("DeadWD_ATS", PlayerRuntimeBuffSource.System, 0, 1f, 1, 1, MakeModifier(0f, 100f), clearOnExpire: true);
			return true;
		}
		if (pl.DeadRageWD && deadRageWdTimer >= 24f)
		{
			deadRageWdTimer = 0f;
			AddWuDi(1f);
			AddTimedStackBuff("DeadRageWD_DMG", PlayerRuntimeBuffSource.System, 0, 1f, 1, 1, MakeModifier(60f), clearOnExpire: true);
			return true;
		}
		if (pl.DeadStealthWD && deadStealthWdTimer >= 25f)
		{
			deadStealthWdTimer = 0f;
			AddWuDi(1f);
			AddTimedStackBuff("DeadStealthWD_MVS", PlayerRuntimeBuffSource.System, 0, 1f, 1, 1, MakeModifier(0f, 0f, 80f), clearOnExpire: true);
			return true;
		}
		return false;
	}

	public bool IsWuDi()
	{
		if (!pl || !pl.IsAlive)
		{
			return false;
		}
		return wuDiTimer > 0f;
	}

	private bool CanApplyRuntimeBuff()
	{
		if ((bool)pl)
		{
			return pl.IsAlive;
		}
		return false;
	}

	private void RefreshRuntimeDerivedStats()
	{
		if ((bool)pl)
		{
			pl.RefreshRuntimeDerivedStats();
		}
	}

	private void AddWuDi(float duration)
	{
		wuDiTimer = Mathf.Max(wuDiTimer, duration);
	}

	private void RefreshAllSkillCooldowns()
	{
		if (!SingletonMonoScope<ACTbar>.HasInstance)
		{
			return;
		}
		for (int i = 0; i < SingletonMonoScope<ACTbar>.Instance.actListSkill.Count; i++)
		{
			ACTListSkillBT aCTListSkillBT = SingletonMonoScope<ACTbar>.Instance.actListSkill[i];
			if ((bool)aCTListSkillBT)
			{
				aCTListSkillBT.ResetCD();
			}
		}
	}

	private bool ExtendActiveSkillInstances(ACTListSkillBT skill, float seconds)
	{
		if (!activeSkillInstances.TryGetValue(skill, out var value))
		{
			return false;
		}
		bool result = false;
		for (int num = value.Count - 1; num >= 0; num--)
		{
			SkillOBJ_DT_SP skillOBJ_DT_SP = value[num];
			if (!skillOBJ_DT_SP)
			{
				value.RemoveAt(num);
			}
			else
			{
				skillOBJ_DT_SP.BuffTime += seconds;
				result = true;
			}
		}
		return result;
	}

	private void ReduceTimedStackBuff(string key, int count)
	{
		TimedStackBuffInstance timedStackBuffInstance = FindTimedStackBuff(key);
		if (timedStackBuffInstance != null)
		{
			RemoveModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
			timedStackBuffInstance.Stack -= Mathf.Max(1, count);
			if (timedStackBuffInstance.Stack <= 0)
			{
				timedStackBuffs.Remove(timedStackBuffInstance);
				RefreshRuntimeDerivedStats();
			}
			else
			{
				ApplyModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
				RefreshRuntimeDerivedStats();
			}
		}
	}

	private void ExtendTimedStackBuffDuration(string key, float seconds)
	{
		TimedStackBuffInstance timedStackBuffInstance = FindTimedStackBuff(key);
		if (timedStackBuffInstance != null)
		{
			timedStackBuffInstance.Duration += seconds;
			timedStackBuffInstance.RemainTime += seconds;
		}
	}

	private void RemoveTimedStackBuff(string key)
	{
		for (int num = timedStackBuffs.Count - 1; num >= 0; num--)
		{
			if (timedStackBuffs[num].Key == key)
			{
				RemoveTimedStackBuffAt(num);
			}
		}
	}

	private void RemoveTimedStackBuffAt(int index)
	{
		TimedStackBuffInstance timedStackBuffInstance = timedStackBuffs[index];
		RemoveModifier(timedStackBuffInstance.PerStackModifier, timedStackBuffInstance.Stack);
		timedStackBuffs.RemoveAt(index);
		RefreshRuntimeDerivedStats();
	}

	private void RemoveLayerBuffAt(int index)
	{
		PlayerLayerBuffInstance playerLayerBuffInstance = layerBuffs[index];
		PlayerRuntimeBuffModifier modifier = MakeModifierFromLayer(playerLayerBuffInstance.Config, full: false);
		RemoveModifier(modifier, playerLayerBuffInstance.Config.LayerCur);
		if (playerLayerBuffInstance.FullModifierApplied)
		{
			PlayerRuntimeBuffModifier modifier2 = MakeModifierFromLayer(playerLayerBuffInstance.Config, full: true);
			RemoveModifier(modifier2, 1);
		}
		layerBuffs.RemoveAt(index);
		RefreshRuntimeDerivedStats();
	}

	private TimedStackBuffInstance FindTimedStackBuff(string key)
	{
		for (int i = 0; i < timedStackBuffs.Count; i++)
		{
			if (timedStackBuffs[i].Key == key)
			{
				return timedStackBuffs[i];
			}
		}
		return null;
	}

	private PlayerLayerBuffInstance FindLayerBuff(string indexName)
	{
		for (int i = 0; i < layerBuffs.Count; i++)
		{
			if (layerBuffs[i].Config != null && layerBuffs[i].Config.BuffName == indexName)
			{
				return layerBuffs[i];
			}
		}
		return null;
	}

	private PlayerRuntimeBuffModifier MakeModifier(float damage = 0f, float attackSpeed = 0f, float moveSpeed = 0f, float bjRate = 0f, float bjDamage = 0f, float damageAnti = 0f, float healthPercent = 0f, float dotDamage = 0f, float companionDamage = 0f, float companionAttackSpeed = 0f, float allChuan = 0f, float elementAnti = 0f, float dotTimeCut = 0f, float geDang = 0f, float orbDamage = 0f, float trapDamage = 0f)
	{
		PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier
		{
			Damage = damage,
			AttackSpeed = attackSpeed,
			MoveSpeed = moveSpeed,
			BJrate = bjRate,
			BJDamage = bjDamage,
			DamageAnti = damageAnti,
			HealthPercent = healthPercent,
			DotDamage = dotDamage,
			DotTimeCut = dotTimeCut,
			CompanionDamage = companionDamage,
			CompanionAttackSpeed = companionAttackSpeed,
			GeDang = geDang,
			OrbDamage = orbDamage,
			TrapDamage = trapDamage,
			AllChuan = allChuan
		};
		if (elementAnti != 0f)
		{
			for (int i = 0; i < 6; i++)
			{
				playerRuntimeBuffModifier.ElementAnti[i] = elementAnti;
			}
		}
		return playerRuntimeBuffModifier;
	}

	private PlayerRuntimeBuffModifier MakeModifierFromLayer(Buff_PL_Layer layer, bool full)
	{
		PlayerRuntimeBuffModifier playerRuntimeBuffModifier = new PlayerRuntimeBuffModifier();
		if (layer == null)
		{
			return playerRuntimeBuffModifier;
		}
		int num = pl.GiveInt(layer.damageType);
		if (!full)
		{
			switch (layer.Type_Layer)
			{
			case 1:
				playerRuntimeBuffModifier.Damage = layer.Number_Layer;
				break;
			case 2:
				playerRuntimeBuffModifier.ElementDamage[num] = layer.Number_Layer;
				break;
			case 3:
				playerRuntimeBuffModifier.ElementChuan[num] = layer.Number_Layer;
				break;
			case 4:
				playerRuntimeBuffModifier.BJrate = layer.Number_Layer;
				break;
			case 5:
				playerRuntimeBuffModifier.AttackSpeed = layer.Number_Layer;
				break;
			case 6:
				playerRuntimeBuffModifier.MoveSpeed = layer.Number_Layer;
				break;
			}
		}
		else
		{
			switch (layer.Type_Max)
			{
			case 1:
				playerRuntimeBuffModifier.DamageAnti = layer.Number_Max;
				break;
			case 2:
				playerRuntimeBuffModifier.HealthPercent = layer.Number_Max;
				break;
			case 3:
				playerRuntimeBuffModifier.DotDamage = layer.Number_Max;
				break;
			case 4:
				playerRuntimeBuffModifier.CompanionDamage = layer.Number_Max;
				break;
			case 5:
				playerRuntimeBuffModifier.BJDamage = layer.Number_Max;
				break;
			case 6:
				playerRuntimeBuffModifier.MoveSpeed = layer.Number_Max;
				break;
			case 7:
				playerRuntimeBuffModifier.ElementChuan[num] = layer.Number_Max;
				break;
			}
		}
		return playerRuntimeBuffModifier;
	}

	private Buff_PL_Layer CloneLayerBuff(Buff_PL_Layer source)
	{
		return new Buff_PL_Layer
		{
			BuffName = (source.BuffName ?? string.Empty),
			BuffType = source.BuffType,
			Type_Layer = source.Type_Layer,
			Type_Max = source.Type_Max,
			BuffTime = Mathf.Max(0f, source.BuffTime),
			LayerMax = Mathf.Max(0, source.LayerMax),
			LayerCur = Mathf.Clamp(source.LayerCur, 0, Mathf.Max(0, source.LayerMax)),
			damageType = source.damageType,
			Number_Layer = source.Number_Layer,
			Number_Max = source.Number_Max
		};
	}

	private void ApplyModifier(PlayerRuntimeBuffModifier modifier, int stack)
	{
		ApplyModifierSign(modifier, stack, 1f);
	}

	private void RemoveModifier(PlayerRuntimeBuffModifier modifier, int stack)
	{
		ApplyModifierSign(modifier, stack, -1f);
	}

	private void ApplyModifierSign(PlayerRuntimeBuffModifier modifier, int stack, float sign)
	{
		if (modifier != null && stack != 0 && (bool)pl)
		{
			float num = (float)stack * sign;
			pl.Damage_Bei_Tmp += modifier.Damage * num;
			pl.ATSpeed_Tmp += modifier.AttackSpeed * num;
			pl.MVSpeed_Tmp += modifier.MoveSpeed * num;
			pl.BJrate_Tmp += modifier.BJrate * num;
			pl.BJDamage_Tmp += modifier.BJDamage * num;
			pl.Damage_Anti_Tmp += modifier.DamageAnti * num;
			pl.Health_Percent_Tmp += modifier.HealthPercent * num;
			pl.Runtime_DotDamage_Tmp += modifier.DotDamage * num;
			pl.Runtime_DotTimeCut_Tmp += modifier.DotTimeCut * num;
			pl.C_Damage_Tmp += modifier.CompanionDamage * num;
			pl.C_ATSpeed_Tmp += modifier.CompanionAttackSpeed * num;
			pl.GeDang_Tmp += modifier.GeDang * num;
			pl.Runtime_ORB_Damage_Tmp += modifier.OrbDamage * num;
			pl.Runtime_XJ_DMG_Tmp += modifier.TrapDamage * num;
			for (int i = 0; i < 6; i++)
			{
				AddElementDamage(i, modifier.ElementDamage[i] * num);
				AddElementChuan(i, modifier.ElementChuan[i] * num + modifier.AllChuan * num);
				AddElementAnti(i, modifier.ElementAnti[i] * num);
			}
		}
	}

	private void AddElementDamage(int index, float value)
	{
		switch (index)
		{
		case 0:
			pl.FireDamage_Bei_Tmp += value;
			break;
		case 1:
			pl.FrozenDamage_Bei_Tmp += value;
			break;
		case 2:
			pl.ThunderDamage_Bei_Tmp += value;
			break;
		case 3:
			pl.PoisonDamage_Bei_Tmp += value;
			break;
		case 4:
			pl.PhysicsDamage_Bei_Tmp += value;
			break;
		case 5:
			pl.ShadowDamage_Bei_Tmp += value;
			break;
		}
	}

	private void AddElementChuan(int index, float value)
	{
		switch (index)
		{
		case 0:
			pl.FireChuan_Tmp += value;
			break;
		case 1:
			pl.FrozenChuan_Tmp += value;
			break;
		case 2:
			pl.ThunderChuan_Tmp += value;
			break;
		case 3:
			pl.PoisonChuan_Tmp += value;
			break;
		case 4:
			pl.PhysicsChuan_Tmp += value;
			break;
		case 5:
			pl.ShadowChuan_Tmp += value;
			break;
		}
	}

	private void AddElementAnti(int index, float value)
	{
		switch (index)
		{
		case 0:
			pl.FireAnti_Tmp += value;
			break;
		case 1:
			pl.FrozenAnti_Tmp += value;
			break;
		case 2:
			pl.ThunderAnti_Tmp += value;
			break;
		case 3:
			pl.PoisonAnti_Tmp += value;
			break;
		case 4:
			pl.PhysicsAnti_Tmp += value;
			break;
		case 5:
			pl.ShadowAnti_Tmp += value;
			break;
		}
	}
}
