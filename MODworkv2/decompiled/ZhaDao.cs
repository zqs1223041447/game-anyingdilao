using System.Collections.Generic;
using FMODUnity;
using Lean.Pool;
using MEC;
using UnityEngine;

public class ZhaDao : MonoBehaviour
{
	private static readonly int attack = Animator.StringToHash("attack");

	public GameObject EXP;

	public GameObject obj;

	private float timeA;

	public float ATtimeALL;

	public bool isAttack;

	public float Atime;

	public float Stime;

	public bool rand;

	public Animator dao;

	public string AudioAT;

	public string AudioRestore;

	private CoroutineHandle loopHandle;

	private void Awake()
	{
		dao = base.transform.Find("dao").GetComponent<Animator>();
	}

	private void OnEnable()
	{
		isAttack = false;
		timeA = 0f;
		isAttack = false;
		if (rand)
		{
			loopHandle = Timing.RunCoroutine(stop(Atime));
		}
		dao.SetBool(attack, value: false);
	}

	private void Update()
	{
		if (!rand && isAttack)
		{
			timeA += Time.deltaTime;
			if (timeA >= ATtimeALL)
			{
				timeA = 0f;
				isAttack = false;
			}
		}
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
		if (isAttack || rand || !collision.CompareTag("FootCOL"))
		{
			return;
		}
		switch (collision.GetComponent<FootCOL>().peo.CharacterType)
		{
		case 0:
			dao.SetBool(attack, value: true);
			if (AudioAT != null)
			{
				RuntimeManager.PlayOneShot(AudioAT, base.transform.position);
			}
			isAttack = true;
			break;
		case 1:
			dao.SetBool(attack, value: true);
			if (AudioAT != null)
			{
				RuntimeManager.PlayOneShot(AudioAT, base.transform.position);
			}
			isAttack = true;
			break;
		}
	}

	private IEnumerator<float> start(float time)
	{
		yield return Timing.WaitForSeconds(time);
		dao.SetBool(attack, value: true);
		if (AudioAT != null)
		{
			RuntimeManager.PlayOneShot(AudioAT, base.transform.position);
		}
		loopHandle = Timing.RunCoroutine(stop(Random.Range(Atime - 1f, Atime + 2f)));
	}

	private IEnumerator<float> stop(float time)
	{
		yield return Timing.WaitForSeconds(time);
		dao.SetBool(attack, value: false);
		loopHandle = Timing.RunCoroutine(start(Stime));
	}

	public void Zha()
	{
		LeanPool.Spawn(obj, base.transform.position, Quaternion.identity);
		LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity);
		this.wait(1f, Shou);
	}

	public void Shou()
	{
		dao.SetBool(attack, value: false);
		if (AudioRestore != null)
		{
			RuntimeManager.PlayOneShot(AudioRestore, base.transform.position);
		}
	}
}
