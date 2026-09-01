using System;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class DOT_MG : MonoBehaviour
{
	public Enemy em;

	private float timeA;

	public float[] JStime;

	public GameObject[] DotFX;

	public DOT_Enemy[] dt;

	public bool DOTover;

	public PlayerManager PL;

	public ACTbar ACT;

	private const float BossFrozenRateMulti = 0.5f;

	private const float BossFrozenTimeMulti = 0.5f;

	private void Awake()
	{
		em = base.transform.parent.GetComponent<Enemy>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		PL.EnsurePlayerDotData();
		ACT = SingletonMonoScope<ACTbar>.Instance;
		Initialize();
	}

	private void OnEnable()
	{
		EnsureRuntimeArrays();
		timeA = 0f;
		DOTover = false;
	}

	private void OnDisable()
	{
		ForceReleaseAllDotFx();
	}

	private void Update()
	{
		if (!em || !em.IsAlive)
		{
			if (!DOTover)
			{
				ClearDot();
			}
		}
		else
		{
			TakeDot();
			TakeDebuff();
		}
	}

	public void AddDot(DamageType type, ACT_DOT dot, int layer)
	{
		if (dot == null || !CanAddPlayerDot() || layer <= 0)
		{
			return;
		}
		int runtimeDotCount = GetRuntimeDotCount();
		for (int i = 0; i < runtimeDotCount; i++)
		{
			if (dt[i].damageType != type)
			{
				continue;
			}
			ACT_DOT aCT_DOT = ((ACT != null && ACT.DOT != null && i < ACT.DOT.Length) ? ACT.DOT[i] : null);
			if (aCT_DOT == null)
			{
				continue;
			}
			int layer2 = Mathf.Clamp(dt[i].Layer + layer, 0, aCT_DOT.Layer_Max);
			if (!dt[i].IsDot)
			{
				dt[i].IsDot = true;
				dt[i].Layer = layer2;
				JStime[i] = GetDotDuration(i, dot);
				em.TakeDotDebuff(add: true, aCT_DOT.ATSpeedCut, aCT_DOT.MVSpeedCut, aCT_DOT.DamageLow);
				em.TakeDotDebuffLayer(add: true, aCT_DOT.ELAntiCut, aCT_DOT.YunCut, dt[i].Layer, dt[i].damageType);
				if (!DotFX[i] || !DotFX[i].activeInHierarchy)
				{
					DotFX[i] = ACTbar.TakeDotFX(i, em.body.transform, em.size);
				}
			}
			else
			{
				dt[i].IsDot = true;
				dt[i].Layer = layer2;
				JStime[i] = GetDotDuration(i, dot);
				em.TakeDotDebuffLayer(add: true, aCT_DOT.ELAntiCut, aCT_DOT.YunCut, dt[i].Layer, dt[i].damageType);
			}
		}
		em?.RefreshSpeedAndSetAni();
	}

	public void DelDot(int i)
	{
		ClearDotSlot(i, forceReleaseFx: false);
		em?.RefreshSpeedAndSetAni();
	}

	private float GetDotDuration(int index, ACT_DOT dot)
	{
		float num = Mathf.Max(0.1f, dot.lifeTime * (1f - em.DotTimeCut / 100f));
		if (ShouldKeepFrozenForeverDotTime(index))
		{
			num = Mathf.Max(num, GetFrozenRemainTime());
		}
		return num;
	}

	private bool ShouldKeepFrozenForeverDotTime(int index)
	{
		if ((bool)em && !em.IS_Boss && em.IS_Frozen && em.FrozenTime > 20f && PL != null && PL.DOT != null && index >= 0 && index < PL.DOT.Length && PL.DOT[index] != null)
		{
			return PL.DOT[index].FrozenForeverDot;
		}
		return false;
	}

	private int GetFrozenJumpRate(int index, ACT_DOT dot)
	{
		int num = ((em.HealthStat.CurrentValue < em.HealthStat.MaxValue * 0.3f) ? PL.DOT[index].Frozen30 : 0);
		float num2 = dot.FrozenJump_Rate + (float)num;
		if (em.IS_Boss)
		{
			num2 *= 0.5f;
		}
		return Mathf.Clamp(Mathf.FloorToInt(num2), 0, 100);
	}

	private float GetFrozenDuration(ACT_DOT dot)
	{
		float num = dot.FrozenJump_Time * (1f - em.DotTimeCut / 100f);
		if (em.IS_Boss)
		{
			num *= 0.5f;
		}
		return Mathf.Max(0.1f, num);
	}

	private float GetFrozenRemainTime()
	{
		if (!em)
		{
			return 0f;
		}
		return Mathf.Max(0.1f, em.FrozenTime - em.FrozenJSTime);
	}

	public void ReleaseFrozenDotFxIfIdle()
	{
		EnsureRuntimeArrays();
		if (dt != null && DotFX != null && dt.Length > 1 && DotFX.Length > 1 && !dt[1].IsDot && (bool)em && !em.IS_Frozen && (bool)DotFX[1])
		{
			ReleaseDotFx(1, forceReleaseFx: true);
		}
	}

	public void ClearDot()
	{
		int runtimeDotCount = GetRuntimeDotCount();
		for (int i = 0; i < runtimeDotCount; i++)
		{
			if (dt[i].IsDot)
			{
				ACT_DOT aCT_DOT = ((ACT != null && ACT.DOT != null && i < ACT.DOT.Length) ? ACT.DOT[i] : null);
				if (aCT_DOT != null && aCT_DOT.BoomDie_Rate > 0f && (float)UnityEngine.Random.Range(0, 100) <= aCT_DOT.BoomDie_Rate)
				{
					ACT.TakeBoomDie(i, em, dt[i].Layer);
				}
			}
			ClearDotSlot(i, forceReleaseFx: true);
		}
		ForceReleaseAllDotFx();
		if ((bool)em)
		{
			em.IS_Frozen = false;
			em.FrozenJSTime = 0f;
			em.RefreshSpeedAndSetAni();
		}
		DOTover = true;
	}

	private void ClearDotSlot(int i, bool forceReleaseFx)
	{
		if (IsDotIndexValid(i))
		{
			bool isDot = dt[i].IsDot;
			DamageType damageType = dt[i].damageType;
			dt[i].IsDot = false;
			dt[i].Layer = 0;
			if (JStime != null && i < JStime.Length)
			{
				JStime[i] = 0f;
			}
			ACT_DOT aCT_DOT = ((ACT != null && ACT.DOT != null && i < ACT.DOT.Length) ? ACT.DOT[i] : null);
			if (isDot && (bool)em && aCT_DOT != null)
			{
				em.TakeDotDebuff(add: false, aCT_DOT.ATSpeedCut, aCT_DOT.MVSpeedCut, aCT_DOT.DamageLow);
				em.TakeDotDebuffLayer(add: false, aCT_DOT.ELAntiCut, aCT_DOT.YunCut, 0, damageType);
			}
			ReleaseDotFx(i, forceReleaseFx);
		}
	}

	private void ReleaseDotFx(int i, bool forceReleaseFx)
	{
		if (DotFX != null && i >= 0 && i < DotFX.Length)
		{
			if (!DotFX[i])
			{
				DotFX[i] = null;
			}
			else if (forceReleaseFx || !ShouldKeepFrozenDotFx(i))
			{
				GameObject clone = DotFX[i];
				DotFX[i] = null;
				LeanPool.Despawn(clone);
			}
		}
	}

	private void ForceReleaseAllDotFx()
	{
		if (DotFX != null)
		{
			for (int i = 0; i < DotFX.Length; i++)
			{
				ReleaseDotFx(i, forceReleaseFx: true);
			}
		}
	}

	private bool ShouldKeepFrozenDotFx(int i)
	{
		if (IsDotIndexValid(i) && dt[i].damageType == DamageType.frozen && (bool)em && em.IsAlive)
		{
			return em.IS_Frozen;
		}
		return false;
	}

	private bool IsDotIndexValid(int i)
	{
		if (dt != null && i >= 0 && i < dt.Length)
		{
			return dt[i] != null;
		}
		return false;
	}

	private int GetRuntimeDotCount()
	{
		EnsureRuntimeArrays();
		int num = ((dt != null) ? dt.Length : 0);
		if (JStime != null)
		{
			num = Math.Min(num, JStime.Length);
		}
		if (ACT != null && ACT.DOT != null)
		{
			num = Math.Min(num, ACT.DOT.Length);
		}
		return num;
	}

	private void EnsureRuntimeArrays()
	{
		if (dt == null || dt.Length < 6)
		{
			Array.Resize(ref dt, 6);
		}
		if (JStime == null || JStime.Length < 6)
		{
			Array.Resize(ref JStime, 6);
		}
		if (DotFX == null || DotFX.Length < 6)
		{
			Array.Resize(ref DotFX, 6);
		}
		for (int i = 0; i < 6; i++)
		{
			if (dt[i] == null)
			{
				dt[i] = new DOT_Enemy();
			}
		}
	}

	public void TakeDot()
	{
		timeA += Time.deltaTime;
		if (!(timeA >= 0.5f))
		{
			return;
		}
		timeA = 0f;
		int runtimeDotCount = GetRuntimeDotCount();
		for (int i = 0; i < runtimeDotCount; i++)
		{
			if (!dt[i].IsDot || ((ACT != null && ACT.DOT != null && i < ACT.DOT.Length) ? ACT.DOT[i] : null) == null)
			{
				continue;
			}
			em.TakeDotDamage(ACT.DOT[i].damageType, ACT.DOT[i].Damage * PL.GiveDamage(i) / 100f * (float)dt[i].Layer / 2f, PL.GiveChuan(i));
			if (UnityEngine.Random.Range(0, 101) < PL.DOT[i].DMG_AddOne && dt[i].Layer < ACT.DOT[i].Layer_Max)
			{
				dt[i].Layer++;
			}
			if (ACT.DOT[i].BoomJump_Rate > 0f && (float)UnityEngine.Random.Range(0, 101) < ACT.DOT[i].BoomJump_Rate)
			{
				ACT.TakeBoomJump(i, em);
			}
			if (ACT.DOT[i].CutJump_Rate > 0f && (float)UnityEngine.Random.Range(0, 101) < ACT.DOT[i].CutJump_Rate)
			{
				em.TakeCutJumpDamage(ACT.DOT[i].damageType, ACT.DOT[i].CutJump_Damage);
				ACT.TakeCutJump(i, em);
			}
			if (PL.DOT[i].LayerPRC > 0 && dt[i].Layer > 7)
			{
				em.TakeCutJumpDamage(ACT.DOT[i].damageType, PL.DOT[i].LayerPRC);
			}
			if (em.IS_Frozen || !(ACT.DOT[i].FrozenJump_Rate > 0f))
			{
				continue;
			}
			int num = UnityEngine.Random.Range(0, 101);
			int frozenJumpRate = GetFrozenJumpRate(i, ACT.DOT[i]);
			if (num >= frozenJumpRate)
			{
				continue;
			}
			em.FrozenTime = GetFrozenDuration(ACT.DOT[i]);
			em.IS_Frozen = true;
			em.RefreshSpeedAndSetAni();
			ACT.TakeFrozen(em.body.transform, em.size);
			if (PL.DOT[i].FrozenCut > 0 && UnityEngine.Random.Range(0, 100) < PL.DOT[i].FrozenCut)
			{
				em.TakeCutJumpDamage(ACT.DOT[i].damageType, 8f);
			}
			if (!em.IS_Boss && PL.DOT[i].FrozenFoever > 0 && em.Quality < 3 && UnityEngine.Random.Range(0, 100) < PL.DOT[i].FrozenFoever)
			{
				em.FrozenTime += 100f;
				if (PL.DOT[i].FrozenForeverDot)
				{
					JStime[i] = Mathf.Max(JStime[i], GetFrozenRemainTime());
				}
			}
		}
	}

	public void TakeDebuff()
	{
		int runtimeDotCount = GetRuntimeDotCount();
		for (int i = 0; i < runtimeDotCount; i++)
		{
			if (!dt[i].IsDot)
			{
				continue;
			}
			if (ShouldKeepFrozenForeverDotTime(i))
			{
				JStime[i] = Mathf.Max(JStime[i], GetFrozenRemainTime());
			}
			if (JStime[i] > 0f)
			{
				JStime[i] -= Time.deltaTime;
				if (JStime[i] <= 0f)
				{
					DelDot(i);
				}
			}
		}
	}

	public void DotYB()
	{
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && dt[i].Layer >= 3 && PL.DOT[i].YB)
			{
				LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.HitFX[10].OBJ[i], em.yao.transform.position, Quaternion.identity, em.yao.transform);
				em.TakeDotDamage(ACT.DOT[i].damageType, ACT.DOT[i].Damage * PL.GiveDamage(i) / 100f * (float)dt[i].Layer * 3f, PL.GiveChuan(i));
				if (!em.IsDpsTarget && PL.DOT[i].YB_MS > 0 && dt[i].Layer > 6 && UnityEngine.Random.Range(0, 101) < PL.DOT[i].YB_MS)
				{
					em.HealthStat.CurrentValue -= em.HealthStat.MaxValue;
				}
				dt[i].Layer = ((!PL.DOT[i].YB_half) ? 1 : Mathf.Max(1, Mathf.CeilToInt((float)dt[i].Layer * 0.5f)));
				if (PL.DOT[i].YB_Add > 0)
				{
					dt[i].Layer = Mathf.Clamp(dt[i].Layer + PL.DOT[i].YB_Add, 1, ACT.DOT[i].Layer_Max);
				}
				em.TakeDotDebuffLayer(add: true, ACT.DOT[i].ELAntiCut, ACT.DOT[i].YunCut, dt[i].Layer, dt[i].damageType);
			}
		}
	}

	public void SetAni()
	{
		em?.SetAni();
	}

	public void Initialize()
	{
		EnsureRuntimeArrays();
		dt[0].damageType = DamageType.fire;
		dt[1].damageType = DamageType.frozen;
		dt[2].damageType = DamageType.thunder;
		dt[3].damageType = DamageType.poison;
		dt[4].damageType = DamageType.physics;
		dt[5].damageType = DamageType.shadow;
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i] != null)
			{
				dt[i].IsDot = false;
				dt[i].Layer = 0;
			}
			if (JStime != null && i < JStime.Length)
			{
				JStime[i] = 0f;
			}
		}
		ForceReleaseAllDotFx();
		timeA = 0f;
		DOTover = false;
	}

	public int GetDotCount()
	{
		int num = 0;
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot)
			{
				num++;
			}
		}
		return num;
	}

	public int GerDotYS()
	{
		int num = 0;
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot)
			{
				num += PL.DOT[i].YS;
			}
		}
		return num;
	}

	public int GerDotFrozenHurtDMG()
	{
		if (!em || !em.IS_Frozen || PL == null || PL.DOT == null)
		{
			return 0;
		}
		int num = 0;
		int num2 = Mathf.Min(dt.Length, PL.DOT.Length);
		for (int i = 0; i < num2; i++)
		{
			if (dt[i].IsDot && PL.DOT[i] != null)
			{
				num += PL.DOT[i].FrozenHurtDMG;
			}
		}
		return num;
	}

	public int GerDotBE_CP()
	{
		int num = 0;
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot)
			{
				num += PL.DOT[i].BE_CP;
			}
		}
		return num;
	}

	public int GerDotBF_DMG()
	{
		int num = 0;
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot)
			{
				num += PL.DOT[i].BF_DMG;
			}
		}
		return num;
	}

	public bool GerDotSL()
	{
		int num = 0;
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && PL.DOT[i].SL)
			{
				num++;
			}
		}
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public bool GerDotCM()
	{
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && PL.DOT[i].CM)
			{
				return true;
			}
		}
		return false;
	}

	public bool GerDotMH()
	{
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && RollPercent(PL.DOT[i].MH))
			{
				return true;
			}
		}
		return false;
	}

	public bool GerDotZZ()
	{
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && PL.DOT[i].ZZ)
			{
				return true;
			}
		}
		return false;
	}

	public bool GerDotDead()
	{
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && RollPercent(PL.DOT[i].Dead))
			{
				return true;
			}
		}
		return false;
	}

	public void TryDotJYOnStun()
	{
		if (!CanAddPlayerDot())
		{
			return;
		}
		for (int i = 0; i < dt.Length; i++)
		{
			if (dt[i].IsDot && dt[i].Layer < ACT.DOT[i].Layer_Max && RollPercent(PL.DOT[i].JY))
			{
				AddDot(dt[i].damageType, ACT.DOT[i], 1);
			}
		}
	}

	private bool CanAddPlayerDot()
	{
		if (!(PL == null))
		{
			return PL.NoDot_BJD <= 0;
		}
		return true;
	}

	private bool RollPercent(int rate)
	{
		if (rate <= 0)
		{
			return false;
		}
		if (rate >= 100)
		{
			return true;
		}
		return UnityEngine.Random.Range(0, 100) < rate;
	}
}
