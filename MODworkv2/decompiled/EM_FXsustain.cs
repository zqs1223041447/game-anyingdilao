using UnityEngine;

public class EM_FXsustain : MonoBehaviour
{
	public GameObject[] OBJ;

	public ParticleSystem[] parLoop;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
	}

	private void Update()
	{
	}

	public void SetColor(int A)
	{
		if (OBJ.Length == 0)
		{
			return;
		}
		for (int i = 0; i < OBJ.Length; i++)
		{
			if (OBJ[i] != null)
			{
				OBJ[i].SetActive(value: false);
			}
		}
		OBJ[A].SetActive(value: true);
	}

	public void StopFX()
	{
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
	}
}
