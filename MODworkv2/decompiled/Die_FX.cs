using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class Die_FX : MonoBehaviour
{
	public string SoundA;

	public string SoundB;

	public string SoundC;

	public string SoundD;

	public int SoundRate;

	public float SkyT;

	public float GroundT;

	private float timeA;

	private float LifeTmp;

	[HideInInspector]
	public int type;

	[HideInInspector]
	public int SPtype;

	public GameObject[] A;

	public GameObject[] B;

	public GameObject[] C;

	public GameObject[] D;

	public GameObject Ground;

	public bool Use;

	private int RD;

	private bool initialized;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		LifeTmp = 1f;
		timeA = 0f;
		if (A.Length != 0)
		{
			for (int i = 0; i < A.Length; i++)
			{
				A[i].SetActive(value: false);
			}
		}
		if (B.Length != 0)
		{
			for (int j = 0; j < B.Length; j++)
			{
				B[j].SetActive(value: false);
			}
		}
		if (C.Length != 0)
		{
			for (int k = 0; k < C.Length; k++)
			{
				C[k].SetActive(value: false);
			}
		}
		if (D.Length != 0)
		{
			for (int l = 0; l < D.Length; l++)
			{
				D[l].SetActive(value: false);
			}
		}
		initialized = !Use;
	}

	private void Update()
	{
		if (Use)
		{
			timeA += Time.deltaTime;
			if (timeA > LifeTmp)
			{
				timeA = 0f;
				Stop();
			}
		}
	}

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		if (type == 0)
		{
			if (Ground != null)
			{
				Ground.SetActive(value: true);
			}
			LifeTmp = GroundT;
		}
		else
		{
			if (Ground != null)
			{
				Ground.SetActive(value: false);
			}
			LifeTmp = SkyT;
		}
		switch (SPtype)
		{
		case 0:
			if (A.Length != 0)
			{
				for (int j = 0; j < A.Length; j++)
				{
					A[j].SetActive(value: true);
				}
			}
			if (SoundA != null)
			{
				RD = Random.Range(0, 101);
				if (RD < SoundRate)
				{
					RuntimeManager.PlayOneShot(SoundA, base.transform.position);
				}
			}
			break;
		case 1:
			if (B.Length != 0)
			{
				for (int l = 0; l < B.Length; l++)
				{
					B[l].SetActive(value: true);
				}
			}
			if (SoundB != null)
			{
				RD = Random.Range(0, 101);
				if (RD < SoundRate)
				{
					RuntimeManager.PlayOneShot(SoundB, base.transform.position);
				}
			}
			break;
		case 2:
			if (C.Length != 0)
			{
				for (int k = 0; k < C.Length; k++)
				{
					C[k].SetActive(value: true);
				}
			}
			if (SoundC != null)
			{
				RD = Random.Range(0, 101);
				if (RD < SoundRate)
				{
					RuntimeManager.PlayOneShot(SoundC, base.transform.position);
				}
			}
			break;
		case 3:
			if (D.Length != 0)
			{
				for (int i = 0; i < D.Length; i++)
				{
					D[i].SetActive(value: true);
				}
			}
			if (SoundD != null)
			{
				RD = Random.Range(0, 101);
				if (RD < SoundRate)
				{
					RuntimeManager.PlayOneShot(SoundD, base.transform.position);
				}
			}
			break;
		}
	}

	public void Stop()
	{
		if (A.Length != 0)
		{
			for (int i = 0; i < A.Length; i++)
			{
				A[i].SetActive(value: false);
			}
		}
		if (B.Length != 0)
		{
			for (int j = 0; j < B.Length; j++)
			{
				B[j].SetActive(value: false);
			}
		}
		if (C.Length != 0)
		{
			for (int k = 0; k < C.Length; k++)
			{
				C[k].SetActive(value: false);
			}
		}
		if (D.Length != 0)
		{
			for (int l = 0; l < D.Length; l++)
			{
				D[l].SetActive(value: false);
			}
		}
		LeanPool.Despawn(this);
	}
}
