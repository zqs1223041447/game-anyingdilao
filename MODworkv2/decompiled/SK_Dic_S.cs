using System;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Dic_S : MonoBehaviour
{
	private int IndexA;

	private int IndexB;

	private int IndexFX;

	private int Count;

	[HideInInspector]
	public Dicform dic;

	private float timeA;

	private float Range;

	private int type;

	public bool CanAT;

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
		CanAT = false;
		this.wait(0.0001f, delegate
		{
			FaShe();
		});
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA > dic.sp.BuffTime)
			{
				timeA = 0f;
				LeanPool.Despawn(this);
			}
		}
	}

	public void FaShe()
	{
		CanAT = true;
		IndexFX = dic.sp.FX_S;
		type = dic.sp.TypeDIC_S;
		IndexA = dic.sp.Dic_S;
		IndexB = dic.sp.MainEL;
		switch (dic.SubType)
		{
		case 0:
			Count = dic.sp.Count_S;
			break;
		case 1:
			Count = dic.sp.Count_AB;
			break;
		case 2:
			Count = dic.sp.Count_AB;
			break;
		}
		Range = dic.sp.JG;
		switch (type)
		{
		case 0:
		{
			Vector3 vector = dic.sp.TargetPos - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (Count % 2 == 1)
			{
				Vector3 position = AnglePOS(base.transform.position, Range, num);
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component2.Index = dic.Index;
				component2.dic = vector;
				if (Count > 1)
				{
					for (int j = 0; j < Count / 2; j++)
					{
						float num2 = (float)(j + 1) * dic.sp.AngleB;
						Vector3 vector2 = AnglePOS(base.transform.position, Range, num + num2);
						Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector2, Quaternion.identity).GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = dic.SubType;
						component3.Index = dic.Index;
						component3.dic = vector2 - base.transform.position;
						float num3 = 360f - (float)(j + 1) * dic.sp.AngleB;
						Vector3 vector3 = AnglePOS(base.transform.position, Range, num + num3);
						Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector3, Quaternion.identity).GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.SubType = dic.SubType;
						component4.Index = dic.Index;
						component4.dic = vector3 - base.transform.position;
					}
				}
			}
			else
			{
				for (int k = 0; k < Count / 2; k++)
				{
					float num4 = (float)(k + 1) * dic.sp.AngleB - dic.sp.AngleB / 2f;
					Vector3 vector4 = AnglePOS(base.transform.position, Range, num + num4);
					Dicform component5 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector4, Quaternion.identity).GetComponent<Dicform>();
					component5.sp = dic.sp;
					component5.SetCount(dic.sp.ZY);
					component5.SubType = dic.SubType;
					component5.Index = dic.Index;
					component5.dic = vector4 - base.transform.position;
					float num5 = 360f - (float)(k + 1) * dic.sp.AngleB + dic.sp.AngleB / 2f;
					Vector3 vector5 = AnglePOS(base.transform.position, Range, num + num5);
					Dicform component6 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector5, Quaternion.identity).GetComponent<Dicform>();
					component6.sp = dic.sp;
					component6.SetCount(dic.sp.ZY);
					component6.SubType = dic.SubType;
					component6.Index = dic.Index;
					component6.dic = vector5 - base.transform.position;
				}
			}
			if (_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL] != null)
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_shan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num));
			}
			break;
		}
		case 1:
		{
			for (int l = 0; l < Count; l++)
			{
				float angle = (float)l / (float)Count * 360f;
				Vector3 vector6 = AnglePOS(base.transform.position, Range, angle);
				Dicform component7 = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], vector6, Quaternion.identity).GetComponent<Dicform>();
				component7.sp = dic.sp;
				component7.SetCount(dic.sp.ZY);
				component7.SubType = dic.SubType;
				component7.Index = dic.Index;
				component7.dic = vector6 - base.transform.position;
			}
			if (_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL] != null)
			{
				LeanPool.Spawn(_gameDataManager.SKPB.FX_quan[IndexFX].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.identity);
			}
			break;
		}
		case 2:
		{
			for (int i = 0; i < Count; i++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Dic[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
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
