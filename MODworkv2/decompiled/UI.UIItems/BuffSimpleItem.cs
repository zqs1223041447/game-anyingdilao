using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

namespace UI.UIItems;

public class BuffSimpleItem : MonoBehaviour
{
	public float CDTime;

	public float JStimeA;

	public string UseType;

	public ACTbar act;

	public float Fill => (CDTime - JStimeA) / CDTime;

	private void Awake()
	{
		JStimeA = 0f;
	}

	private void Start()
	{
		act = SingletonMonoScope<ACTbar>.Instance;
	}

	private void OnEnable()
	{
		JStimeA = 0f;
	}

	private void Update()
	{
		JStimeA += Time.deltaTime;
		if (JStimeA >= CDTime)
		{
			Del();
			JStimeA = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void Del()
	{
		ACT_UseBT[] useBT = act.useBT;
		foreach (ACT_UseBT aCT_UseBT in useBT)
		{
			if (aCT_UseBT.Type == UseType)
			{
				aCT_UseBT.IsCD = false;
				aCT_UseBT.slot = null;
			}
		}
		if (SingletonMonoScope<SimplePotionManager>.HasInstance)
		{
			SingletonMonoScope<SimplePotionManager>.Instance.SimpleList.Remove(this);
		}
	}
}
