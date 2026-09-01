using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_TimeDel : MonoBehaviour
{
	public string SoundA;

	public string SoundB;

	public float SoDelay;

	public int SoundRate;

	public float LifeTime;

	public float DelDelay;

	private float timeA;

	private float timeB;

	private float timeE;

	private bool SoundOK;

	public ParticleSystem[] parLoop;

	private int RD;

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeE = 0f;
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				main.loop = true;
			}
		}
		SoundOK = false;
		if (SoundA != null)
		{
			RD = Random.Range(0, 101);
			if (RD < SoundRate)
			{
				RuntimeManager.PlayOneShot(SoundA, base.transform.position);
			}
		}
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			if (parLoop.Length != 0)
			{
				ParticleSystem[] array = parLoop;
				for (int i = 0; i < array.Length; i++)
				{
					ParticleSystem.MainModule main = array[i].main;
					main.loop = false;
				}
			}
		}
		timeB += Time.deltaTime;
		if (timeB > LifeTime + DelDelay)
		{
			timeB = 0f;
			LeanPool.SafeDespawn(this);
		}
		if (SoundOK)
		{
			return;
		}
		timeE += Time.deltaTime;
		if (!(timeE >= SoDelay))
		{
			return;
		}
		if (SoundB != null)
		{
			RD = Random.Range(0, 101);
			if (RD < SoundRate)
			{
				RuntimeManager.PlayOneShot(SoundB, base.transform.position);
			}
		}
		timeE = 0f;
		SoundOK = true;
	}
}
