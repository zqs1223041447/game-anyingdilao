using FinkFramework.Runtime.Singleton;
using Interact;
using UnityEngine;

public class BSFlower : InteractableBase
{
	private static readonly int liang = Shader.PropertyToID("_Liang");

	public Sprite[] flower;

	public GameObject baseR;

	public GameObject baoshiR;

	public GameObject point;

	public Collider2D col;

	public int type;

	public SpriteRenderer renderA;

	public SpriteRenderer renderB;

	private void Awake()
	{
		baseR = base.transform.Find("main/base").gameObject;
		baoshiR = base.transform.Find("main/bs").gameObject;
		point = base.transform.Find("point").gameObject;
		col = GetComponent<Collider2D>();
		renderA = base.transform.Find("main/base").gameObject.GetComponent<SpriteRenderer>();
		renderB = base.transform.Find("main/bs").gameObject.GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		type = Random.Range(0, 9);
		renderB.sprite = flower[type];
		setColor(type);
	}

	public void UseFlower()
	{
		baoshiR.SetActive(value: false);
		SingletonMonoScope<LevelManager>.Instance.BreakFlower(type, base.transform);
	}

	protected override void OnHover(bool isHovering)
	{
	}

	public override void Interact()
	{
	}

	public override bool CanInteract()
	{
		return (base.transform.position - SingletonMonoScope<PlayerManager>.Instance.transform.position).sqrMagnitude < 3f;
	}

	public void setColor(int a)
	{
	}
}
