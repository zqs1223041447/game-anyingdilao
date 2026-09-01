using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using MEC;
using Spine.Unity;
using UnityEngine;

public class DICI_col : MonoBehaviour
{
	private static readonly int attack = Animator.StringToHash("attack");

	public GameObject EXP;

	private bool open;

	private bool isAttack;

	public float Atime;

	public float Stime;

	public float damage;

	private GameObject spineOBJ;

	public Collider2D col;

	public Animator[] ci;

	public SkeletonMecanim mecanim;

	public string AudioAT;

	public string AudioRestore;

	private CoroutineHandle loopHandle;

	private void Awake()
	{
		mecanim = base.transform.Find("A").gameObject.GetComponent<SkeletonMecanim>();
		spineOBJ = base.transform.Find("A").gameObject;
		col = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		isAttack = true;
		if (isAttack)
		{
			Animator[] array = ci;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetBool(attack, value: true);
			}
		}
		loopHandle = Timing.RunCoroutine(stop(Atime));
	}

	private IEnumerator<float> start(float time)
	{
		yield return Timing.WaitForSeconds(time);
		AT();
		loopHandle = Timing.RunCoroutine(stop(Atime));
	}

	private IEnumerator<float> stop(float time)
	{
		yield return Timing.WaitForSeconds(time);
		Restore();
		loopHandle = Timing.RunCoroutine(start(Random.Range(Stime - 1f, Stime + 1f)));
	}

	private void OnDisable()
	{
		Timing.KillCoroutines(loopHandle);
	}

	private void OnDestroy()
	{
		Timing.KillCoroutines(loopHandle);
	}

	public void AT()
	{
		Animator[] array = ci;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetBool(attack, value: true);
		}
		SingletonMonoGlobal<AudioManager>.Instance.SceneSFX(base.transform, AudioAT);
	}

	public void Restore()
	{
		Animator[] array = ci;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetBool(attack, value: false);
		}
		if (AudioRestore != null)
		{
			SingletonMonoGlobal<AudioManager>.Instance.SceneSFX(base.transform, AudioRestore);
		}
	}

	public void Damage()
	{
		LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
	}
}
