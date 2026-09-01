using Lean.Pool;
using UnityEngine;

public class SK_DICIspine : MonoBehaviour
{
	public Animator ani;

	private float time;

	private float ATtime;

	public float JStime;

	public float LifeTime;

	[ColorUsage(true, true)]
	public Color[] MainColor;

	[HideInInspector]
	public MeshRenderer SpineRender;

	private MaterialPropertyBlock mpb;

	private void Awake()
	{
		SpineRender = GetComponent<MeshRenderer>();
		ani = GetComponent<Animator>();
		time = 0f;
		ATtime = 0f;
		ani.SetBool("attack", value: true);
		mpb = new MaterialPropertyBlock();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		time = 0f;
		ATtime = 0f;
		ani.SetBool("attack", value: true);
		mpb.SetInt("_MainAlpha", 0);
		SpineRender.SetPropertyBlock(mpb);
	}

	private void Update()
	{
		ATtime += Time.deltaTime;
		if (ATtime >= JStime)
		{
			ATtime = 0f;
			ani.SetBool("attack", value: false);
		}
		time += Time.deltaTime;
		if (time >= LifeTime)
		{
			time = 0f;
			LeanPool.Despawn(this);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		collision.CompareTag("BodyCOL");
	}

	public void SetColor(DamageType type)
	{
		switch (type)
		{
		case DamageType.fire:
			mpb.SetColor("_MainColor", MainColor[0]);
			break;
		case DamageType.frozen:
			mpb.SetColor("_MainColor", MainColor[1]);
			break;
		case DamageType.thunder:
			mpb.SetColor("_MainColor", MainColor[2]);
			break;
		case DamageType.poison:
			mpb.SetColor("_MainColor", MainColor[3]);
			break;
		case DamageType.physics:
			mpb.SetColor("_MainColor", MainColor[4]);
			break;
		case DamageType.shadow:
			mpb.SetColor("_MainColor", MainColor[5]);
			break;
		}
		mpb.SetInt("_MainAlpha", 1);
		SpineRender.SetPropertyBlock(mpb);
	}
}
