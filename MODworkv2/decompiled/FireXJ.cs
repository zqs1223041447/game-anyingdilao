using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class FireXJ : MonoBehaviour
{
	public int Size;

	public Transform[] trans;

	public float ATtime;

	private float ATtimeTmp;

	private float timeA;

	private float timeB;

	private int type;

	private void OnEnable()
	{
		ATtimeTmp = Random.Range(ATtime + 2f, ATtime - 2f);
		type = Random.Range(0, 6);
		timeA = 0f;
		Transform[] array = trans;
		foreach (Transform transform in array)
		{
			if (SingletonMonoScope<GameDataManager>.HasInstance)
			{
				switch (Size)
				{
				case 0:
					LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.XJ_Pen_1[type], transform.position, Quaternion.identity, transform);
					break;
				case 1:
					LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.XJ_Pen_2[type], transform.position, Quaternion.identity, transform);
					break;
				}
			}
		}
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (!(timeA >= ATtimeTmp))
		{
			return;
		}
		timeA = 0f;
		Transform[] array = trans;
		foreach (Transform transform in array)
		{
			if (SingletonMonoScope<GameDataManager>.HasInstance)
			{
				switch (Size)
				{
				case 0:
					LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.XJ_Pen_1[type], transform.position, Quaternion.identity, transform);
					break;
				case 1:
					LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.XJ_Pen_2[type], transform.position, Quaternion.identity, transform);
					break;
				}
			}
		}
	}
}
