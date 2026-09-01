using System;
using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_fatherDIC : MonoBehaviour
{
	public string SoundA;

	public GameObject OBJ;

	public float LifeTime;

	public float FStime;

	public int FasheType;

	public bool UseDicCount;

	public int FSnumber;

	public float range;

	public float Angle;

	[Header("=========")]
	public bool CanChangeForm;

	public int FasheTypeA;

	public int FasheTypeB;

	public int FasheCountA;

	public int FasheCountB;

	[Header("=========")]
	public float SpeedMin;

	public float SpeedMax;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private int FScountTMP;

	private bool CanAT;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		FScountTMP = 0;
		CanAT = false;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		if (CanAT && FasheType > 9 && FScountTMP < FSnumber)
		{
			timeB += Time.deltaTime;
			if (timeB > FStime)
			{
				if (FasheType == 10)
				{
					Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
					component.dic = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
					component.speed = UnityEngine.Random.Range(SpeedMin, SpeedMax);
				}
				timeB = 0f;
				FScountTMP++;
			}
		}
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		CanAT = true;
		Angle = sp.AngleA;
		if (CanChangeForm)
		{
			if (sp.CF_Rate > 0f)
			{
				if ((float)UnityEngine.Random.Range(0, 101) < sp.CF_Rate)
				{
					FSnumber = FasheCountB;
				}
				else
				{
					FSnumber = FasheCountA;
				}
			}
			else
			{
				FSnumber = FasheCountA;
			}
		}
		else if (UseDicCount)
		{
			FSnumber = sp.Count_F;
		}
		switch (FasheType)
		{
		case 0:
		{
			Vector3 vector = sp.TargetPos - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (FSnumber % 2 == 1)
			{
				Vector3 position = AnglePOS(base.transform.position, range, num);
				Dicform component2 = LeanPool.Spawn(OBJ, position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
				component2.dic = vector;
				if (FSnumber > 1)
				{
					for (int j = 0; j < FSnumber / 2; j++)
					{
						float num2 = (float)(j + 1) * Angle;
						Vector3 vector2 = AnglePOS(base.transform.position, range, num + num2);
						Dicform component3 = LeanPool.Spawn(OBJ, vector2, Quaternion.identity).GetComponent<Dicform>();
						component3.sp = sp;
						component3.SetCount(sp.ZY);
						component3.SubType = 0;
						component3.Index = 0;
						component3.dic = vector2 - base.transform.position;
						float num3 = 360f - (float)(j + 1) * Angle;
						Vector3 vector3 = AnglePOS(base.transform.position, range, num + num3);
						Dicform component4 = LeanPool.Spawn(OBJ, vector3, Quaternion.identity).GetComponent<Dicform>();
						component4.sp = sp;
						component4.SetCount(sp.ZY);
						component4.SubType = 0;
						component4.Index = 0;
						component4.dic = vector3 - base.transform.position;
					}
				}
			}
			else
			{
				for (int k = 0; k < FSnumber / 2; k++)
				{
					float num4 = (float)(k + 1) * Angle - Angle / 2f;
					Vector3 vector4 = AnglePOS(base.transform.position, range, num + num4);
					Dicform component5 = LeanPool.Spawn(OBJ, vector4, Quaternion.identity).GetComponent<Dicform>();
					component5.sp = sp;
					component5.SetCount(sp.ZY);
					component5.SubType = 0;
					component5.Index = 0;
					component5.dic = vector4 - base.transform.position;
					float num5 = 360f - (float)(k + 1) * Angle + Angle / 2f;
					Vector3 vector5 = AnglePOS(base.transform.position, range, num + num5);
					Dicform component6 = LeanPool.Spawn(OBJ, vector5, Quaternion.identity).GetComponent<Dicform>();
					component6.sp = sp;
					component6.SetCount(sp.ZY);
					component6.SubType = 0;
					component6.Index = 0;
					component6.dic = vector5 - base.transform.position;
				}
			}
			break;
		}
		case 1:
		{
			for (int i = 0; i < FSnumber; i++)
			{
				Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SubType = 0;
				component.Index = 0;
			}
			break;
		}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private static Vector3 AnglePOS(Vector3 center, float radius, float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float x = center.x + radius * Mathf.Cos(f);
		float y = center.y + radius * Mathf.Sin(f);
		return new Vector3(x, y, 0f);
	}
}
