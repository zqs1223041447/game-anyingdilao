using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using MEC;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
	public Transform[] trans;

	public List<GameObject> obj = new List<GameObject>();

	public bool isAttack;

	public int type;

	public bool rand;

	public float Damage;

	private float Atime;

	private float Stime;

	public float attackSpeed;

	[SerializeField]
	private Collider2D col;

	public int AudioAT;

	private CoroutineHandle loopHandle;

	private void Start()
	{
		isAttack = true;
		type = Random.Range(0, 6);
		Atime = 6f;
		Stime = 2f;
		Transform[] array = trans;
		foreach (Transform transform in array)
		{
			if (SingletonMonoScope<GameDataManager>.HasInstance)
			{
				GameObject item = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.XJ_Pen_1[type], transform.position, Quaternion.identity, transform);
				obj.Add(item);
				continue;
			}
			break;
		}
	}

	public void OnEnable()
	{
		loopHandle = Timing.RunCoroutine(stop(Atime));
	}

	private void OnDisable()
	{
		Timing.KillCoroutines(loopHandle);
	}

	private void OnDestroy()
	{
		Timing.KillCoroutines(loopHandle);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isAttack)
		{
			collision.CompareTag("Player");
		}
	}

	private IEnumerator<float> start(float time)
	{
		yield return Timing.WaitForSeconds(time);
		foreach (GameObject item in obj)
		{
			ParticleSystem.EmissionModule emission = item.transform.Find("huo").GetComponent<ParticleSystem>().emission;
			ParticleSystem.EmissionModule emission2 = item.transform.Find("yan").GetComponent<ParticleSystem>().emission;
			emission.rateOverTime = 30f;
			emission2.rateOverTime = 30f;
		}
		isAttack = true;
		loopHandle = Timing.RunCoroutine(stop(Atime));
	}

	private IEnumerator<float> stop(float time)
	{
		yield return Timing.WaitForSeconds(time);
		foreach (GameObject item in obj)
		{
			ParticleSystem.EmissionModule emission = item.transform.Find("huo").GetComponent<ParticleSystem>().emission;
			ParticleSystem.EmissionModule emission2 = item.transform.Find("yan").GetComponent<ParticleSystem>().emission;
			emission.rateOverTime = 0f;
			emission2.rateOverTime = 0f;
		}
		isAttack = false;
		loopHandle = Timing.RunCoroutine(start(Stime));
	}
}
