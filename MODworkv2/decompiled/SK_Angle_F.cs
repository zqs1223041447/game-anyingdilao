using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Angle_F : MonoBehaviour
{
	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private int FScountTMP;

	private bool CanAT;

	private int IndexA;

	private int IndexB;

	private int IndexFX;

	private int type;

	private int Count;

	private GameDataManager _gameDataManager;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanAT = false;
		FScountTMP = 0;
		this.wait(0.0001f, delegate
		{
			FaShe();
		});
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		if (type > 4 && FScountTMP < Count)
		{
			timeB += Time.deltaTime;
			if (timeB > sp.FStime1)
			{
				switch (type)
				{
				case 5:
				{
					for (int j = 0; j < sp.CountMulti; j++)
					{
						Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).GetComponent<Dicform>();
						component2.sp = sp;
						component2.SetCount(sp.ZY);
						component2.SubType = 0;
						component2.Index = 0;
					}
					break;
				}
				case 6:
				{
					for (int i = 0; i < sp.CountMulti; i++)
					{
						Vector3 right = base.transform.right;
						float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
						Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - sp.AngleA, sp.AngleA))).GetComponent<Dicform>();
						component.sp = sp;
						component.SetCount(sp.ZY);
						component.SubType = 0;
						component.Index = 0;
					}
					break;
				}
				}
				timeB = 0f;
				FScountTMP++;
			}
		}
		timeA += Time.deltaTime;
		if (timeA > (float)Count * sp.FStime1 + 0.2f)
		{
			timeA = 0f;
			CanAT = false;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		CanAT = true;
		bool ringEquipped = PoeItemMod.RingEquipped;
		if (sp.CF_Rate > 0f)
		{
			if ((float)Random.Range(0, 101) < sp.CF_Rate)
			{
				type = sp.CF_Type;
				Count = sp.CF_Count;
			}
			else
			{
				type = sp.Type_F;
				Count = sp.Count_F;
			}
		}
		else
		{
			type = sp.Type_F;
			Count = sp.Count_F;
		}
		if (type < 5)
		{
			switch (type)
			{
			case 0:
			{
				Vector3 right = base.transform.right;
				float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
				if (ringEquipped)
				{
					SpawnEvenRing(Count + 4);
					if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
					{
						LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num));
					}
					break;
				}
				if (Count % 2 == 1)
				{
					Dicform component8 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num)).GetComponent<Dicform>();
					component8.sp = sp;
					component8.SetCount(sp.ZY);
					component8.SubType = 0;
					component8.Index = 0;
					if (Count > 1)
					{
						for (int m = 0; m < Count / 2; m++)
						{
							GameObject gameObject3 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num + sp.AngleA * (float)(m + 1)));
							GameObject obj3 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num - sp.AngleA * (float)(m + 1)));
							Dicform component9 = gameObject3.GetComponent<Dicform>();
							component9.sp = sp;
							component9.SetCount(sp.ZY);
							component9.SubType = 0;
							component9.Index = 0;
							Dicform component10 = obj3.GetComponent<Dicform>();
							component10.sp = sp;
							component10.SetCount(sp.ZY);
							component10.SubType = 0;
							component10.Index = 0;
						}
					}
				}
				else
				{
					for (int n = 0; n < Count / 2; n++)
					{
						GameObject gameObject4 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num + sp.AngleA * (float)(n + 1) - sp.AngleA / 2f));
						GameObject obj4 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num - sp.AngleA * (float)(n + 1) + sp.AngleA / 2f));
						Dicform component11 = gameObject4.GetComponent<Dicform>();
						component11.sp = sp;
						component11.SetCount(sp.ZY);
						component11.SubType = 0;
						component11.Index = 0;
						Dicform component12 = obj4.GetComponent<Dicform>();
						component12.sp = sp;
						component12.SetCount(sp.ZY);
						component12.SubType = 0;
						component12.Index = 0;
					}
				}
				if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num));
				}
				break;
			}
			case 1:
			{
				if (ringEquipped)
				{
					SpawnEvenRing(Count + 4);
					if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
					{
						LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, Mathf.Atan2(base.transform.right.y, base.transform.right.x) * 57.29578f));
					}
					break;
				}
				Vector3 vector = base.transform.right * sp.JG;
				float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (Count % 2 == 1)
				{
					Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
					component2.sp = sp;
					component2.SetCount(sp.ZY);
					component2.SubType = 0;
					component2.Index = 0;
					if (Count > 1)
					{
						for (int j = 0; j < Count / 2; j++)
						{
							Vector3 position = new Vector3(base.transform.position.x + vector.y * (float)(j + 1), base.transform.position.y - vector.x * (float)(j + 1), 0f);
							Vector3 position2 = new Vector3(base.transform.position.x - vector.y * (float)(j + 1), base.transform.position.y + vector.x * (float)(j + 1), 0f);
							GameObject obj = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], position, Quaternion.Euler(0f, 0f, z));
							GameObject gameObject = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], position2, Quaternion.Euler(0f, 0f, z));
							Dicform component3 = obj.GetComponent<Dicform>();
							component3.sp = sp;
							component3.SetCount(sp.ZY);
							component3.SubType = 0;
							component3.Index = 0;
							Dicform component4 = gameObject.GetComponent<Dicform>();
							component4.sp = sp;
							component4.SetCount(sp.ZY);
							component4.SubType = 0;
							component4.Index = 0;
						}
					}
				}
				else
				{
					for (int k = 0; k < Count / 2; k++)
					{
						Vector3 position3 = new Vector3(base.transform.position.x + vector.y / 2f + vector.y * (float)k, base.transform.position.y - vector.x / 2f - vector.x * (float)k, 0f);
						Vector3 position4 = new Vector3(base.transform.position.x - vector.y / 2f - vector.y * (float)k, base.transform.position.y + vector.x / 2f + vector.x * (float)k, 0f);
						GameObject obj2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], position3, Quaternion.Euler(0f, 0f, z));
						GameObject gameObject2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], position4, Quaternion.Euler(0f, 0f, z));
						Dicform component5 = obj2.GetComponent<Dicform>();
						component5.sp = sp;
						component5.SetCount(sp.ZY);
						component5.SubType = 0;
						component5.Index = 0;
						Dicform component6 = gameObject2.GetComponent<Dicform>();
						component6.sp = sp;
						component6.SetCount(sp.ZY);
						component6.SubType = 0;
						component6.Index = 0;
					}
				}
				if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, z));
				}
				break;
			}
			case 2:
			{
				Vector3 right2 = base.transform.right;
				float num2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
				if (ringEquipped)
				{
					SpawnEvenRing(Count + 4);
					if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
					{
						LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num2));
					}
					break;
				}
				for (int num3 = 0; num3 < Count; num3++)
				{
					Dicform component13 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num2 + Random.Range(0f - sp.AngleA, sp.AngleA))).GetComponent<Dicform>();
					component13.sp = sp;
					component13.SetCount(sp.ZY);
					component13.SubType = 0;
					component13.Index = 0;
				}
				if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num2));
				}
				break;
			}
			case 3:
			{
				if (ringEquipped)
				{
					SpawnEvenRing(Count + 4);
					if (_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL] != null)
					{
						LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.identity);
					}
					break;
				}
				for (int l = 0; l < Count; l++)
				{
					Dicform component7 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, 360 / Count * (l + 1))).GetComponent<Dicform>();
					component7.sp = sp;
					component7.SetCount(sp.ZY);
					component7.SubType = 0;
					component7.Index = 0;
				}
				if (_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.identity);
				}
				break;
			}
			case 4:
			{
				if (ringEquipped)
				{
					SpawnEvenRing(Count + 4);
					if (_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL] != null)
					{
						LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.identity);
					}
					break;
				}
				for (int i = 0; i < Count; i++)
				{
					Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
				}
				if (_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.identity);
				}
				break;
			}
			}
		}
		else if (type == 5)
		{
			if (_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL] != null)
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.identity);
			}
		}
		else if (_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL] != null)
		{
			Vector3 right3 = base.transform.right;
			float z2 = Mathf.Atan2(right3.y, right3.x) * 57.29578f;
			LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, z2));
		}
		if (_gameDataManager.SKPB.Angle[sp.ZD_F].ST[sp.MainEL] != null)
		{
			RuntimeManager.PlayOneShot(_gameDataManager.SKPB.Angle[sp.ZD_F].ST[sp.MainEL], base.transform.position);
		}
	}

	private void SpawnEvenRing(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, 360f / (float)count * (float)(i + 1))).GetComponent<Dicform>();
			component.sp = sp;
			component.SetCount(sp.ZY);
			component.SubType = 0;
			component.Index = 0;
		}
	}
}
