using Interact;
using UnityEngine;

public class OpenDoor : InteractableBase
{
	private static readonly int _liang = Shader.PropertyToID("_Liang");

	public GameObject off;

	public GameObject on;

	public SpriteRenderer renderA;

	public SpriteRenderer renderB;

	public bool Opened;

	public LevelPoint point;

	public int soundA;

	public override InteractionType Type => InteractionType.Portal;

	private void Start()
	{
		off = base.transform.Find("of").gameObject;
		on = base.transform.Find("on").gameObject;
		renderA = base.transform.Find("of").gameObject.GetComponent<SpriteRenderer>();
		renderB = base.transform.Find("on").gameObject.GetComponent<SpriteRenderer>();
		point = base.transform.parent.GetComponent<LevelPoint>();
		renderA.material.SetFloat(_liang, 1f);
		renderB.material.SetFloat(_liang, 1f);
		Opened = false;
		off.SetActive(value: true);
		on.SetActive(value: false);
	}

	public override bool CanInteract()
	{
		return false;
	}

	public override void Interact()
	{
		Opened = true;
		off.SetActive(value: false);
		on.SetActive(value: true);
	}

	protected override void OnHover(bool isHovering)
	{
		if (isHovering)
		{
			renderA.material.SetFloat(_liang, 0f);
			renderB.material.SetFloat(_liang, 0f);
		}
		else
		{
			renderA.material.SetFloat(_liang, 1f);
			renderB.material.SetFloat(_liang, 1f);
		}
	}
}
