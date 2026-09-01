using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class FireXJ_Fire : MonoBehaviour
{
	public DamageType DMtype;

	public string SoundA;

	public ParticleSystem[] parLoop;

	public GameObject EXP;

	public float Lifetime;

	public float ATtime;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanAT = true;
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= ATtime)
			{
				timeA = 0f;
				LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity).GetComponent<XJ_DamageCol>().DMtype = DMtype;
			}
			timeB += Time.deltaTime;
			if (!(timeB >= Lifetime))
			{
				return;
			}
			timeB = 0f;
			if (parLoop.Length != 0)
			{
				for (int i = 0; i < parLoop.Length; i++)
				{
					ParticleSystem.MainModule main = parLoop[i].main;
					main.loop = false;
				}
			}
			CanAT = false;
		}
		else
		{
			timeC += Time.deltaTime;
			if (timeC >= 2f)
			{
				timeC = 0f;
				LeanPool.Despawn(this);
			}
		}
	}
}
