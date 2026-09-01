using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Angle_S : MonoBehaviour
{
	[HideInInspector]
	public Dicform dic;

	private int IndexA;

	private int IndexB;

	private int IndexFX;

	private int Count;

	private int CountMulti;

	private float FStime;

	private int type;

	private bool CanAT;

	private float timeA;

	private float timeB;

	private int FScountTMP;

	private GameDataManager _gameDataManager;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		FScountTMP = 0;
		CanAT = false;
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
			if (timeB > FStime)
			{
				switch (type)
				{
				case 5:
				{
					for (int j = 0; j < CountMulti; j++)
					{
						Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.SubType = dic.SubType;
						component2.Index = dic.Index;
					}
					break;
				}
				case 6:
				{
					Vector3 right = base.transform.right;
					float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
					for (int i = 0; i < CountMulti; i++)
					{
						Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - dic.sp.AngleB, dic.sp.AngleB))).GetComponent<Dicform>();
						component.sp = dic.sp;
						component.SetCount(dic.sp.ZY);
						component.SubType = dic.SubType;
						component.Index = dic.Index;
					}
					break;
				}
				}
				timeB = 0f;
				FScountTMP++;
			}
		}
		timeA += Time.deltaTime;
		if (timeA > (float)Count * FStime + 0.2f)
		{
			timeA = 0f;
			CanAT = false;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		CanAT = true;
		IndexFX = dic.sp.FX_S;
		CountMulti = dic.sp.CountMulti;
		FStime = dic.sp.FStime2;
		switch (dic.SubType)
		{
		case 0:
			IndexA = dic.sp.ZD_S;
			IndexB = dic.sp.MainEL;
			Count = dic.sp.Count_S;
			type = dic.sp.Type_S;
			break;
		case 1:
		case 2:
			IndexA = dic.sp.ZD_AB;
			IndexB = dic.sp.MainEL;
			Count = dic.sp.Count_AB;
			type = dic.sp.Type_AB;
			break;
		}
		if (type < 5)
		{
			switch (type)
			{
			case 0:
			{
				Vector3 right2 = base.transform.right;
				float num2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
				if (Count % 2 == 1)
				{
					Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num2)).GetComponent<Dicform>();
					component3.sp = dic.sp;
					component3.SetCount(dic.sp.ZY);
					component3.SubType = dic.SubType;
					component3.Index = dic.Index;
					if (Count > 1)
					{
						for (int k = 0; k < Count / 2; k++)
						{
							GameObject obj = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num2 + dic.sp.AngleB * (float)(k + 1)));
							GameObject gameObject = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num2 - dic.sp.AngleB * (float)(k + 1)));
							Dicform component4 = obj.GetComponent<Dicform>();
							component4.sp = dic.sp;
							component4.SetCount(dic.sp.ZY);
							component4.SubType = dic.SubType;
							component4.Index = dic.Index;
							Dicform component5 = gameObject.GetComponent<Dicform>();
							component5.sp = dic.sp;
							component5.SetCount(dic.sp.ZY);
							component5.SubType = dic.SubType;
							component5.Index = dic.Index;
						}
					}
				}
				else
				{
					for (int l = 0; l < Count / 2; l++)
					{
						GameObject gameObject2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num2 + dic.sp.AngleB * (float)(l + 1) - dic.sp.AngleB / 2f));
						GameObject obj2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num2 - dic.sp.AngleB * (float)(l + 1) + dic.sp.AngleB / 2f));
						Dicform component6 = gameObject2.GetComponent<Dicform>();
						component6.sp = dic.sp;
						component6.SetCount(dic.sp.ZY);
						component6.SubType = dic.SubType;
						component6.Index = dic.Index;
						Dicform component7 = obj2.GetComponent<Dicform>();
						component7.sp = dic.sp;
						component7.SetCount(dic.sp.ZY);
						component7.SubType = dic.SubType;
						component7.Index = dic.Index;
					}
				}
				if (_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num2));
				}
				break;
			}
			case 1:
			{
				Vector3 vector = base.transform.right * dic.sp.JG;
				float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (Count % 2 == 1)
				{
					Dicform component8 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
					component8.sp = dic.sp;
					component8.SetCount(dic.sp.ZY);
					component8.SubType = 0;
					component8.Index = dic.Index;
					if (Count > 1)
					{
						for (int m = 0; m < Count / 2; m++)
						{
							Vector3 position = new Vector3(base.transform.position.x + vector.y * (float)(m + 1), base.transform.position.y - vector.x * (float)(m + 1), 0f);
							Vector3 position2 = new Vector3(base.transform.position.x - vector.y * (float)(m + 1), base.transform.position.y + vector.x * (float)(m + 1), 0f);
							GameObject obj3 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], position, Quaternion.Euler(0f, 0f, z));
							GameObject gameObject3 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], position2, Quaternion.Euler(0f, 0f, z));
							Dicform component9 = obj3.GetComponent<Dicform>();
							component9.sp = dic.sp;
							component9.SetCount(dic.sp.ZY);
							component9.SubType = dic.SubType;
							component9.Index = dic.Index;
							Dicform component10 = gameObject3.GetComponent<Dicform>();
							component10.sp = dic.sp;
							component10.SetCount(dic.sp.ZY);
							component10.SubType = dic.SubType;
							component10.Index = dic.Index;
						}
					}
				}
				else
				{
					for (int n = 0; n < Count / 2; n++)
					{
						Vector3 position3 = new Vector3(base.transform.position.x + vector.y / 2f + vector.y * (float)n, base.transform.position.y - vector.x / 2f - vector.x * (float)n, 0f);
						Vector3 position4 = new Vector3(base.transform.position.x - vector.y / 2f - vector.y * (float)n, base.transform.position.y + vector.x / 2f + vector.x * (float)n, 0f);
						GameObject obj4 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], position3, Quaternion.Euler(0f, 0f, z));
						GameObject gameObject4 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], position4, Quaternion.Euler(0f, 0f, z));
						Dicform component11 = obj4.GetComponent<Dicform>();
						component11.sp = dic.sp;
						component11.SetCount(dic.sp.ZY);
						component11.SubType = dic.SubType;
						component11.Index = dic.Index;
						Dicform component12 = gameObject4.GetComponent<Dicform>();
						component12.sp = dic.sp;
						component12.SetCount(dic.sp.ZY);
						component12.SubType = dic.SubType;
						component12.Index = dic.Index;
					}
				}
				if (_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, z));
				}
				break;
			}
			case 2:
			{
				Vector3 right = base.transform.right;
				float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
				for (int j = 0; j < Count; j++)
				{
					Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - dic.sp.AngleB, dic.sp.AngleB))).GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = dic.SubType;
					component2.Index = dic.Index;
				}
				if (_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num));
				}
				break;
			}
			case 3:
			{
				for (int num3 = 0; num3 < Count; num3++)
				{
					Dicform component13 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, 360 / Count * (num3 + 1))).GetComponent<Dicform>();
					component13.sp = dic.sp;
					component13.SetCount(dic.sp.ZY);
					component13.SubType = dic.SubType;
					component13.Index = dic.Index;
				}
				if (_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.identity);
				}
				break;
			}
			case 4:
			{
				for (int i = 0; i < Count; i++)
				{
					Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = dic.SubType;
					component.Index = dic.Index;
				}
				if (_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL] != null)
				{
					LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.identity);
				}
				break;
			}
			}
		}
		else if (type == 5)
		{
			if (_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL] != null)
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.identity);
			}
		}
		else if (_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL] != null)
		{
			Vector3 right3 = base.transform.right;
			float z2 = Mathf.Atan2(right3.y, right3.x) * 57.29578f;
			LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, z2));
		}
		if (_gameDataManager.SKPB.Angle[IndexA].ST[IndexB] != null)
		{
			RuntimeManager.PlayOneShot(_gameDataManager.SKPB.Angle[IndexA].ST[IndexB], base.transform.position);
		}
	}
}
