using Lean.Pool;
using UnityEngine;

public class SK_DZ : MonoBehaviour
{
	public bool MV;

	public ParticleSystem[] parLoop;

	private bool canStop;

	private float timeA;

	private void OnEnable()
	{
		canStop = false;
		timeA = 0f;
		if (parLoop == null)
		{
			return;
		}
		for (int i = 0; i < parLoop.Length; i++)
		{
			ParticleSystem particleSystem = parLoop[i];
			if (!(particleSystem == null))
			{
				ParticleSystem.MainModule main = particleSystem.main;
				main.loop = true;
				particleSystem.Play(withChildren: true);
			}
		}
	}

	private void Update()
	{
		if (canStop)
		{
			timeA += Time.deltaTime;
			float num = (MV ? 1.2f : 3.5f);
			if (timeA >= num)
			{
				timeA = 0f;
				LeanPool.Despawn(base.gameObject);
			}
		}
	}

	public void Stop()
	{
		canStop = true;
		if (parLoop == null)
		{
			return;
		}
		for (int i = 0; i < parLoop.Length; i++)
		{
			ParticleSystem particleSystem = parLoop[i];
			if ((bool)particleSystem)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				main.loop = false;
			}
		}
	}
}
