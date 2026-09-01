using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_fatherA : MonoBehaviour
{
	public string SoundA;

	public GameObject OBJ;

	[HideInInspector]
	public float Angle;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private int FScountTMP;

	[HideInInspector]
	public int Count;

	[HideInInspector]
	public int type;

	private PlayerManager _playerManager;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		FScountTMP = 0;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
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
						Dicform component2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).GetComponent<Dicform>();
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
						Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - Angle, Angle))).GetComponent<Dicform>();
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
		if (timeA > 1f)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		Angle = sp.AngleA;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
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
		switch (type)
		{
		case 0:
		{
			Vector3 right = base.transform.right;
			float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
			if (Count % 2 == 1)
			{
				Dicform component2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num)).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
				if (Count > 1)
				{
					for (int j = 0; j < Count / 2; j++)
					{
						GameObject obj = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + Angle * (float)(j + 1)));
						GameObject gameObject = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num - Angle * (float)(j + 1)));
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
					GameObject obj2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + Angle * (float)(k + 1) - Angle / 2f));
					GameObject gameObject2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num - Angle * (float)(k + 1) + Angle / 2f));
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
			break;
		}
		case 1:
		{
			Vector3 vector = base.transform.right * sp.JG;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (Count % 2 == 1)
			{
				Dicform component8 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
				component8.sp = sp;
				component8.SetCount(sp.ZY);
				component8.SubType = 0;
				component8.Index = 0;
				if (Count > 1)
				{
					for (int m = 0; m < Count / 2; m++)
					{
						Vector3 position = new Vector3(base.transform.position.x + vector.y * (float)(m + 1), base.transform.position.y - vector.x * (float)(m + 1), 0f);
						Vector3 position2 = new Vector3(base.transform.position.x - vector.y * (float)(m + 1), base.transform.position.y + vector.x * (float)(m + 1), 0f);
						GameObject obj3 = LeanPool.Spawn(OBJ, position, Quaternion.Euler(0f, 0f, z));
						GameObject gameObject3 = LeanPool.Spawn(OBJ, position2, Quaternion.Euler(0f, 0f, z));
						Dicform component9 = obj3.GetComponent<Dicform>();
						component9.sp = sp;
						component9.SetCount(sp.ZY);
						component9.SubType = 0;
						component9.Index = 0;
						Dicform component10 = gameObject3.GetComponent<Dicform>();
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
					Vector3 position3 = new Vector3(base.transform.position.x + vector.y / 2f + vector.y * (float)n, base.transform.position.y - vector.x / 2f - vector.x * (float)n, 0f);
					Vector3 position4 = new Vector3(base.transform.position.x - vector.y / 2f - vector.y * (float)n, base.transform.position.y + vector.x / 2f + vector.x * (float)n, 0f);
					GameObject obj4 = LeanPool.Spawn(OBJ, position3, Quaternion.Euler(0f, 0f, z));
					GameObject gameObject4 = LeanPool.Spawn(OBJ, position4, Quaternion.Euler(0f, 0f, z));
					Dicform component11 = obj4.GetComponent<Dicform>();
					component11.sp = sp;
					component11.SetCount(sp.ZY);
					component11.SubType = 0;
					component11.Index = 0;
					Dicform component12 = gameObject4.GetComponent<Dicform>();
					component12.sp = sp;
					component12.SetCount(sp.ZY);
					component12.SubType = 0;
					component12.Index = 0;
				}
			}
			break;
		}
		case 2:
		{
			Vector3 right2 = base.transform.right;
			float num2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
			for (int num3 = 0; num3 < Count; num3++)
			{
				Dicform component13 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num2 + Random.Range(0f - Angle, Angle))).GetComponent<Dicform>();
				component13.sp = sp;
				component13.SetCount(sp.ZY);
				component13.SubType = 0;
				component13.Index = 0;
			}
			break;
		}
		case 3:
		{
			for (int l = 0; l < Count; l++)
			{
				Dicform component7 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, 360 / Count * (l + 1))).GetComponent<Dicform>();
				component7.sp = sp;
				component7.SetCount(sp.ZY);
				component7.SubType = 0;
				component7.Index = 0;
			}
			break;
		}
		case 4:
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
			}
			break;
		}
		}
		if (sp.Reborn > 0)
		{
			int indexType = sp.indexType;
			if (indexType != 0)
			{
				_ = indexType - 1;
				_ = 1;
			}
			else
			{
				_playerManager.HealStat.Cur += _playerManager.HealStat.Max * (float)sp.Reborn / 100f;
			}
		}
	}
}
