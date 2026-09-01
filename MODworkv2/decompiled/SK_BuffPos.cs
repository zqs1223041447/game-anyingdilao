using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_BuffPos : MonoBehaviour
{
	public GameObject OBJ;

	public float LifeTime;

	public float DelDelay;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private bool CanAT;

	private PlayerManager _playerManager;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		CanAT = false;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA > LifeTime)
			{
				timeA = 0f;
				Stop();
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
	}

	public void FaShe()
	{
		LifeTime = sp.BuffTime;
		CanAT = true;
		Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = sp;
		component.SetCount(sp.ZY);
		component.SubType = 0;
		component.Index = 0;
		if (sp.Reborn > 0 && sp.indexType == 0)
		{
			_playerManager.HealStat.Cur += _playerManager.HealStat.Max * (float)sp.Reborn / 100f;
		}
		base.transform.SetParent(SingletonMonoScope<PlayerManager>.Instance.transform);
	}

	public void Stop()
	{
		CanAT = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
