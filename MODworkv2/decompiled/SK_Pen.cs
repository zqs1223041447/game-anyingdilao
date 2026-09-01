using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Pen : MonoBehaviour
{
	public ParticleSystem[] par;

	public GameObject FX;

	public float LifeTime;

	public float ATtime;

	public int Type;

	public bool Body;

	public float size;

	public float ColTime;

	public float DotMulti;

	public bool CanMV;

	public float MoveSpeed;

	[HideInInspector]
	public Dicform dic;

	private bool canAT;

	private float timeA;

	private float timeB;

	private float timeC;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void OnEnable()
	{
		canAT = true;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		ParticleSystem[] array = par;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		array = par;
		ParticleSystem[] array2 = array;
		foreach (ParticleSystem obj in array2)
		{
			obj.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ParticleSystem.MainModule main = obj.main;
			main.duration = LifeTime;
			obj.Play();
		}
	}

	public void InitPen(float lifeTime)
	{
		LifeTime = lifeTime;
		ParticleSystem[] array = par;
		foreach (ParticleSystem obj in array)
		{
			obj.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ParticleSystem.MainModule main = obj.main;
			main.duration = LifeTime;
			obj.Play();
		}
	}

	private void Update()
	{
		if (canAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= 0.33f)
			{
				timeA = 0f;
				Vector3 right = base.transform.right;
				float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
				switch (Type)
				{
				case 0:
					SpawnEmptyCol(num);
					break;
				case 1:
					SpawnEmptyCol(num);
					SpawnEmptyCol(num + 180f);
					break;
				case 2:
					SpawnEmptyCol(num);
					SpawnEmptyCol(num - 17f);
					SpawnEmptyCol(num + 17f);
					break;
				}
			}
			timeB += Time.deltaTime;
			if (timeB >= LifeTime)
			{
				timeB = 0f;
				stop();
			}
		}
		else
		{
			timeC += Time.deltaTime;
			if (timeC >= 1.2f)
			{
				timeC = 0f;
				LeanPool.Despawn(this);
			}
		}
	}

	public void stop()
	{
		canAT = false;
	}

	private void SpawnEmptyCol(float angle)
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.Euler(0f, 0f, angle)).GetComponent<EmptyCOL>();
			Dicform component2 = component.GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component2.SubType = dic.SubType;
			component2.Index = dic.Index;
			component.size = size;
			component.Body = Body;
			component.DotMulti = DotMulti;
			component.CanMV = CanMV;
			component.MoveSpeed = MoveSpeed;
			component.lifeTime = ColTime;
			component.FX = FX;
		}
	}
}
