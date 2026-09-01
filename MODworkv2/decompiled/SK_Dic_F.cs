using System;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Dic_F : MonoBehaviour
{
	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float range;

	private int IndexA;

	private int IndexB;

	private int type;

	private int Count;

	private bool CanAT;

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
		CanAT = false;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA > sp.BuffTime)
			{
				timeA = 0f;
				LeanPool.Despawn(this);
			}
		}
	}

	public void FaShe()
	{
		CanAT = true;
		range = sp.JG;
		IndexA = sp.Dic_F;
		IndexB = sp.MainEL;
		if (sp.CF_Rate > 0f)
		{
			if ((float)UnityEngine.Random.Range(0, 101) < sp.CF_Rate)
			{
				type = sp.CF_Type;
				Count = sp.CF_Count;
			}
			else
			{
				type = sp.TypeDIC_F;
				Count = sp.Count_F;
			}
		}
		else
		{
			type = sp.TypeDIC_F;
			Count = sp.Count_F;
		}
		switch (type)
		{
		case 0:
		{
			Vector3 vector = sp.TargetPos - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (Count % 2 == 1)
			{
				Vector3 position = AnglePOS(base.transform.position, range, num);
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
				component2.dic = vector;
				if (Count > 1)
				{
					for (int j = 0; j < Count / 2; j++)
					{
						float num2 = (float)(j + 1) * sp.AngleA;
						Vector3 vector2 = AnglePOS(base.transform.position, range, num + num2);
						Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector2, Quaternion.identity).GetComponent<Dicform>();
						component3.sp = sp;
						component3.SetCount(sp.ZY);
						component3.SubType = 0;
						component3.Index = 0;
						component3.dic = vector2 - base.transform.position;
						float num3 = 360f - (float)(j + 1) * sp.AngleA;
						Vector3 vector3 = AnglePOS(base.transform.position, range, num + num3);
						Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector3, Quaternion.identity).GetComponent<Dicform>();
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
				for (int k = 0; k < Count / 2; k++)
				{
					float num4 = (float)(k + 1) * sp.AngleA - sp.AngleA / 2f;
					Vector3 vector4 = AnglePOS(base.transform.position, range, num + num4);
					Dicform component5 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector4, Quaternion.identity).GetComponent<Dicform>();
					component5.sp = sp;
					component5.SetCount(sp.ZY);
					component5.SubType = 0;
					component5.Index = 0;
					component5.dic = vector4 - base.transform.position;
					float num5 = 360f - (float)(k + 1) * sp.AngleA + sp.AngleA / 2f;
					Vector3 vector5 = AnglePOS(base.transform.position, range, num + num5);
					Dicform component6 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector5, Quaternion.identity).GetComponent<Dicform>();
					component6.sp = sp;
					component6.SetCount(sp.ZY);
					component6.SubType = 0;
					component6.Index = 0;
					component6.dic = vector5 - base.transform.position;
				}
			}
			if ((bool)_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL])
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num));
			}
			break;
		}
		case 1:
		{
			for (int l = 0; l < Count; l++)
			{
				float angle = (float)l / (float)Count * 360f;
				Vector3 vector6 = AnglePOS(base.transform.position, range, angle);
				Dicform component7 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector6, Quaternion.identity).GetComponent<Dicform>();
				component7.sp = sp;
				component7.SetCount(sp.ZY);
				component7.SubType = 0;
				component7.Index = 0;
				component7.dic = vector6 - base.transform.position;
			}
			if ((bool)_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL])
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[sp.FX_F].OBJ[sp.MainEL], base.transform.position, Quaternion.identity);
			}
			break;
		}
		case 2:
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
				component.dic = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
			}
			break;
		}
		}
		if (_gameDataManager.SKPB.Dic[IndexA].ST[IndexB] != null)
		{
			RuntimeManager.PlayOneShot(_gameDataManager.SKPB.Dic[IndexA].ST[IndexB], base.transform.position);
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
