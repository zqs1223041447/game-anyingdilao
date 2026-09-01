using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_TestA : MonoBehaviour
{
	public GameObject[] OBJ;

	public Transform trans;

	private float timeA;

	public float LifeTime;

	public int FasheType;

	public int FSnumber;

	public float FStime;

	public float angleRange;

	public SkillOBJ_DT_SP sp;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
	}

	private void OnEnable()
	{
		timeA = 0f;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		switch (FasheType)
		{
		case 0:
		{
			Vector3 vector = trans.position - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (FSnumber % 2 == 1)
			{
				Dicform component2 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, num)).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				if (FSnumber > 1)
				{
					for (int j = 0; j < FSnumber / 2; j++)
					{
						GameObject gameObject = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, num + angleRange * (float)(j + 1)));
						GameObject obj = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, num - angleRange * (float)(j + 1)));
						Dicform component3 = gameObject.GetComponent<Dicform>();
						component3.sp = sp;
						component3.SetCount(sp.ZY);
						component3.SubType = 0;
						Dicform component4 = obj.GetComponent<Dicform>();
						component4.sp = sp;
						component4.SetCount(sp.ZY);
						component4.SubType = 0;
					}
				}
			}
			else
			{
				for (int k = 0; k < FSnumber / 2; k++)
				{
					GameObject gameObject2 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, num + angleRange * (float)(k + 1) - angleRange / 2f));
					GameObject obj2 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, num - angleRange * (float)(k + 1) + angleRange / 2f));
					Dicform component5 = gameObject2.GetComponent<Dicform>();
					component5.sp = sp;
					component5.SetCount(sp.ZY);
					component5.SubType = 0;
					Dicform component6 = obj2.GetComponent<Dicform>();
					component6.sp = sp;
					component6.SetCount(sp.ZY);
					component6.SubType = 0;
				}
			}
			break;
		}
		case 1:
		{
			Vector3 vector2 = trans.position - base.transform.position;
			float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			if (FSnumber % 2 == 1)
			{
				Dicform component8 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
				component8.sp = sp;
				component8.SetCount(sp.ZY);
				component8.SubType = 0;
				if (FSnumber > 1)
				{
					for (int m = 0; m < FSnumber / 2; m++)
					{
						Vector3 position = new Vector3(base.transform.position.x + vector2.y * (float)m, base.transform.position.y - vector2.x * (float)m, 0f);
						Vector3 position2 = new Vector3(base.transform.position.x - vector2.y * (float)m, base.transform.position.y + vector2.x * (float)m, 0f);
						GameObject obj3 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], position, Quaternion.Euler(0f, 0f, z));
						GameObject gameObject3 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], position2, Quaternion.Euler(0f, 0f, z));
						Dicform component9 = obj3.GetComponent<Dicform>();
						component9.sp = sp;
						component9.SetCount(sp.ZY);
						component9.SubType = 0;
						Dicform component10 = gameObject3.GetComponent<Dicform>();
						component10.sp = sp;
						component10.SetCount(sp.ZY);
						component10.SubType = 0;
					}
				}
			}
			else
			{
				for (int n = 0; n < FSnumber / 2; n++)
				{
					Vector3 position3 = new Vector3(base.transform.position.x + vector2.y / 2f + vector2.y * (float)n, base.transform.position.y - vector2.x / 2f - vector2.x * (float)n, 0f);
					Vector3 position4 = new Vector3(base.transform.position.x - vector2.y / 2f - vector2.y * (float)n, base.transform.position.y + vector2.x / 2f + vector2.x * (float)n, 0f);
					GameObject obj4 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], position3, Quaternion.Euler(0f, 0f, z));
					GameObject gameObject4 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], position4, Quaternion.Euler(0f, 0f, z));
					Dicform component11 = obj4.GetComponent<Dicform>();
					component11.sp = sp;
					component11.SetCount(sp.ZY);
					component11.SubType = 0;
					Dicform component12 = gameObject4.GetComponent<Dicform>();
					component12.sp = sp;
					component12.SetCount(sp.ZY);
					component12.SubType = 0;
				}
			}
			break;
		}
		case 2:
		{
			Vector3 vector3 = trans.position - base.transform.position;
			float num2 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			for (int num3 = 0; num3 < FSnumber; num3++)
			{
				Dicform component13 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, num2 + Random.Range(0f - angleRange, angleRange))).GetComponent<Dicform>();
				component13.sp = sp;
				component13.SetCount(sp.ZY);
				component13.SubType = 0;
			}
			break;
		}
		case 3:
		{
			for (int l = 0; l < FSnumber; l++)
			{
				Dicform component7 = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, 360 / FSnumber * (l + 1))).GetComponent<Dicform>();
				component7.sp = sp;
				component7.SetCount(sp.ZY);
				component7.SubType = 0;
			}
			break;
		}
		case 4:
		{
			for (int i = 0; i < FSnumber; i++)
			{
				Dicform component = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
			}
			break;
		}
		}
	}
}
