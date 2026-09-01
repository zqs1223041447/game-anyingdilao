using Lean.Pool;
using UnityEngine;

public class SK_DropArrowLit : MonoBehaviour
{
	public TrailRenderer[] trail;

	public float[] trTime;

	public SpriteRenderer render;

	public Sprite on;

	public Sprite off;

	public float lifeTime;

	public float DropDownTime;

	private float timeA;

	private float timeB;

	public GameObject obj;

	private void Start()
	{
		timeA = 0f;
		timeB = 0f;
		render = base.transform.Find("main/qiu").GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		timeB = 0f;
		timeA = 0f;
		render.sprite = on;
		if (trail.Length != 0)
		{
			for (int i = 0; i < trail.Length; i++)
			{
				trail[i].emitting = true;
				trail[i].time = trTime[i];
			}
		}
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA >= DropDownTime)
		{
			render.sprite = off;
			timeA = 0f;
		}
		timeB += Time.deltaTime;
		if (timeB >= lifeTime)
		{
			timeB = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void zha()
	{
		if (trail.Length != 0)
		{
			for (int i = 0; i < trail.Length; i++)
			{
				trail[i].emitting = false;
			}
		}
		LeanPool.Spawn(obj, base.transform.position, Quaternion.identity);
	}
}
