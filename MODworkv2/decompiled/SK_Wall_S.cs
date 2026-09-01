using System;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Wall_S : MonoBehaviour
{
	private float timeA;

	private bool CanAT;

	[HideInInspector]
	public Dicform dic;

	private int IndexA;

	private int IndexB;

	private float range;

	private int Count;

	private int type;

	private GameDataManager _gameDataManager;

	private bool initialized;

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
		initialized = false;
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

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		CanAT = true;
		range = dic.sp.JG;
		Count = dic.sp.Count_ORB;
		IndexA = dic.sp.ORB;
		IndexB = dic.sp.MainEL;
		type = dic.sp.TypeORB;
		if (type == 0)
		{
			Vector3 vector = base.transform.right * range;
			if (Count % 2 == 1)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				if (Count > 1)
				{
					for (int i = 0; i < Count / 2; i++)
					{
						Vector3 position = new Vector3(base.transform.position.x + vector.y * (float)(i + 1), base.transform.position.y - vector.x * (float)(i + 1), 0f);
						Vector3 position2 = new Vector3(base.transform.position.x - vector.y * (float)(i + 1), base.transform.position.y + vector.x * (float)(i + 1), 0f);
						GameObject obj = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position, Quaternion.identity);
						GameObject gameObject = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position2, Quaternion.identity);
						Dicform component2 = obj.GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.SubType = dic.SubType;
						component2.Index = dic.Index;
						Dicform component3 = gameObject.GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = dic.SubType;
						component3.Index = dic.Index;
					}
				}
			}
			else
			{
				for (int j = 0; j < Count / 2; j++)
				{
					Vector3 position3 = new Vector3(base.transform.position.x + vector.y / 2f + vector.y * (float)j, base.transform.position.y - vector.x / 2f - vector.x * (float)j, 0f);
					Vector3 position4 = new Vector3(base.transform.position.x - vector.y / 2f - vector.y * (float)j, base.transform.position.y + vector.x / 2f + vector.x * (float)j, 0f);
					GameObject obj2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position3, Quaternion.identity);
					GameObject gameObject2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position4, Quaternion.identity);
					Dicform component4 = obj2.GetComponent<Dicform>();
					component4.sp = dic.sp;
					component4.SetCount(dic.sp.ZY);
					component4.SubType = dic.SubType;
					component4.Index = dic.Index;
					Dicform component5 = gameObject2.GetComponent<Dicform>();
					component5.sp = dic.sp;
					component5.SetCount(dic.sp.ZY);
					component5.SubType = dic.SubType;
					component5.Index = dic.Index;
				}
			}
		}
		else if (type == 1)
		{
			Vector3 vector2 = base.transform.up * range;
			if (Count % 2 == 1)
			{
				Dicform component6 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component6.sp = dic.sp;
				component6.SubType = dic.SubType;
				component6.Index = dic.Index;
				if (Count > 1)
				{
					for (int k = 0; k < Count / 2; k++)
					{
						Vector3 position5 = new Vector3(base.transform.position.x + vector2.y * (float)(k + 1), base.transform.position.y - vector2.x * (float)(k + 1), 0f);
						Vector3 position6 = new Vector3(base.transform.position.x - vector2.y * (float)(k + 1), base.transform.position.y + vector2.x * (float)(k + 1), 0f);
						GameObject obj3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position5, Quaternion.identity);
						GameObject gameObject3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position6, Quaternion.identity);
						Dicform component7 = obj3.GetComponent<Dicform>();
						component7.sp = dic.sp;
						component7.SetCount(dic.sp.ZY);
						component7.SubType = dic.SubType;
						component7.Index = dic.Index;
						Dicform component8 = gameObject3.GetComponent<Dicform>();
						component8.sp = dic.sp;
						component8.SetCount(dic.sp.ZY);
						component8.SubType = dic.SubType;
						component8.Index = dic.Index;
					}
				}
			}
			else
			{
				for (int l = 0; l < Count / 2; l++)
				{
					Vector3 position7 = new Vector3(base.transform.position.x + vector2.y / 2f + vector2.y * (float)l, base.transform.position.y - vector2.x / 2f - vector2.x * (float)l, 0f);
					Vector3 position8 = new Vector3(base.transform.position.x - vector2.y / 2f - vector2.y * (float)l, base.transform.position.y + vector2.x / 2f + vector2.x * (float)l, 0f);
					GameObject obj4 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position7, Quaternion.identity);
					GameObject gameObject4 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position8, Quaternion.identity);
					Dicform component9 = obj4.GetComponent<Dicform>();
					component9.sp = dic.sp;
					component9.SetCount(dic.sp.ZY);
					component9.SubType = dic.SubType;
					component9.Index = dic.Index;
					Dicform component10 = gameObject4.GetComponent<Dicform>();
					component10.sp = dic.sp;
					component10.SetCount(dic.sp.ZY);
					component10.SubType = dic.SubType;
					component10.Index = dic.Index;
				}
			}
		}
		else if (type == 2)
		{
			Vector3 vector3 = base.transform.right * range;
			if (Count % 2 == 1)
			{
				Dicform component11 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component11.sp = dic.sp;
				component11.SetCount(dic.sp.ZY);
				component11.SubType = dic.SubType;
				component11.Index = dic.Index;
				if (Count > 1)
				{
					for (int m = 0; m < Count / 2; m++)
					{
						Vector3 position9 = new Vector3(base.transform.position.x + vector3.y * (float)(m + 1), base.transform.position.y - vector3.x * (float)(m + 1), 0f);
						Vector3 position10 = new Vector3(base.transform.position.x - vector3.y * (float)(m + 1), base.transform.position.y + vector3.x * (float)(m + 1), 0f);
						GameObject obj5 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position9, Quaternion.identity);
						GameObject gameObject5 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position10, Quaternion.identity);
						Dicform component12 = obj5.GetComponent<Dicform>();
						component12.sp = dic.sp;
						component12.SetCount(dic.sp.ZY);
						component12.SubType = dic.SubType;
						component12.Index = dic.Index;
						Dicform component13 = gameObject5.GetComponent<Dicform>();
						component13.sp = dic.sp;
						component13.SetCount(dic.sp.ZY);
						component13.SubType = dic.SubType;
						component13.Index = dic.Index;
					}
				}
			}
			else
			{
				for (int n = 0; n < Count / 2; n++)
				{
					Vector3 position11 = new Vector3(base.transform.position.x + vector3.y / 2f + vector3.y * (float)n, base.transform.position.y - vector3.x / 2f - vector3.x * (float)n, 0f);
					Vector3 position12 = new Vector3(base.transform.position.x - vector3.y / 2f - vector3.y * (float)n, base.transform.position.y + vector3.x / 2f + vector3.x * (float)n, 0f);
					GameObject obj6 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position11, Quaternion.identity);
					GameObject gameObject6 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position12, Quaternion.identity);
					Dicform component14 = obj6.GetComponent<Dicform>();
					component14.sp = dic.sp;
					component14.SetCount(dic.sp.ZY);
					component14.SubType = dic.SubType;
					component14.Index = dic.Index;
					Dicform component15 = gameObject6.GetComponent<Dicform>();
					component15.sp = dic.sp;
					component15.SetCount(dic.sp.ZY);
					component15.SubType = dic.SubType;
					component15.Index = dic.Index;
				}
			}
			Vector3 vector4 = base.transform.up * range;
			if (Count % 2 == 1)
			{
				Dicform component16 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component16.sp = dic.sp;
				component16.SubType = dic.SubType;
				component16.Index = dic.Index;
				if (Count > 1)
				{
					for (int num = 0; num < Count / 2; num++)
					{
						Vector3 position13 = new Vector3(base.transform.position.x + vector4.y * (float)(num + 1), base.transform.position.y - vector4.x * (float)(num + 1), 0f);
						Vector3 position14 = new Vector3(base.transform.position.x - vector4.y * (float)(num + 1), base.transform.position.y + vector4.x * (float)(num + 1), 0f);
						GameObject obj7 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position13, Quaternion.identity);
						GameObject gameObject7 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position14, Quaternion.identity);
						Dicform component17 = obj7.GetComponent<Dicform>();
						component17.sp = dic.sp;
						component17.SetCount(dic.sp.ZY);
						component17.SubType = dic.SubType;
						component17.Index = dic.Index;
						Dicform component18 = gameObject7.GetComponent<Dicform>();
						component18.sp = dic.sp;
						component18.SetCount(dic.sp.ZY);
						component18.SubType = dic.SubType;
						component18.Index = dic.Index;
					}
				}
			}
			else
			{
				for (int num2 = 0; num2 < Count / 2; num2++)
				{
					Vector3 position15 = new Vector3(base.transform.position.x + vector4.y / 2f + vector4.y * (float)num2, base.transform.position.y - vector4.x / 2f - vector4.x * (float)num2, 0f);
					Vector3 position16 = new Vector3(base.transform.position.x - vector4.y / 2f - vector4.y * (float)num2, base.transform.position.y + vector4.x / 2f + vector4.x * (float)num2, 0f);
					GameObject obj8 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position15, Quaternion.identity);
					GameObject gameObject8 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position16, Quaternion.identity);
					Dicform component19 = obj8.GetComponent<Dicform>();
					component19.sp = dic.sp;
					component19.SetCount(dic.sp.ZY);
					component19.SubType = dic.SubType;
					component19.Index = dic.Index;
					Dicform component20 = gameObject8.GetComponent<Dicform>();
					component20.sp = dic.sp;
					component20.SetCount(dic.sp.ZY);
					component20.SubType = dic.SubType;
					component20.Index = dic.Index;
				}
			}
		}
		else
		{
			for (int num3 = 0; num3 < Count; num3++)
			{
				float angle = (float)num3 / (float)Count * 360f;
				Vector3 position17 = AnglePOS(base.transform.position, range, angle);
				Dicform component21 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], position17, Quaternion.identity).GetComponent<Dicform>();
				component21.sp = dic.sp;
				component21.SetCount(dic.sp.ZY);
				component21.SubType = dic.SubType;
				component21.Index = dic.Index;
			}
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
