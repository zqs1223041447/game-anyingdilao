using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Doom_Ball : MonoBehaviour
{
	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanFX;

	private bool CanSound;

	public SK_Doom father;

	private GameDataManager _gameDataManager;

	private void Awake()
	{
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanFX = true;
		CanSound = true;
	}

	private void Update()
	{
		_ = CanFX;
		timeA += Time.deltaTime;
		if (timeA > 0.2f)
		{
			CanFX = true;
			CanSound = true;
			timeA = 0f;
		}
	}

	public void SetHit(Dicform dic, Vector3 angle)
	{
		if (father.sp.TypeDIC_S > 0 && dic.UPDamage == 0f && dic.sp.damageType == father.sp.damageType)
		{
			if (dic.sp.skillName == father.sp.ZQName)
			{
				dic.UPDamage = father.sp.TypeDIC_S * 2;
			}
			else
			{
				dic.UPDamage = father.sp.TypeDIC_S;
			}
		}
		if (!(dic.sp.skillName == father.sp.ZQName))
		{
			return;
		}
		if (father.sp.TypeORB == 0 && !dic.ChangeFL && dic.Index == 0)
		{
			dic.ChangeFL = true;
		}
		if (!(father.sp.Damage > 0f) || dic.Index != 0)
		{
			return;
		}
		FaShe(angle);
		if (CanFX)
		{
			if (_gameDataManager.SKPB.FX_quan[7].OBJ[father.sp.MainEL] != null)
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[7].OBJ[father.sp.MainEL], base.transform.position, Quaternion.identity);
			}
			CanFX = false;
		}
	}

	public void FaShe(Vector3 angle)
	{
		for (int i = 0; i < father.sp.CountMulti; i++)
		{
			Vector3 vector = angle;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[father.sp.ZD_F].OBJ[father.sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - father.sp.AngleA, father.sp.AngleA))).GetComponent<Dicform>();
			component.sp = father.sp;
			component.SetCount(father.sp.ZY);
			component.SubType = 0;
			component.Index = 0;
		}
		if (CanSound)
		{
			if (_gameDataManager.SKPB.Angle[father.sp.ZD_F].ST[father.sp.MainEL] != null)
			{
				RuntimeManager.PlayOneShot(_gameDataManager.SKPB.Angle[father.sp.ZD_F].ST[father.sp.MainEL], base.transform.position);
			}
			CanSound = false;
		}
	}
}
