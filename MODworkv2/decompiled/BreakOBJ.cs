using FinkFramework.Runtime.Singleton;
using Interact;
using Lean.Pool;
using UnityEngine;

public class BreakOBJ : InteractableBase
{
	private static readonly int liang = Shader.PropertyToID("_Liang");

	[SerializeField]
	private SpriteRenderer render;

	public int type;

	public int Sound;

	public int sp;

	public int index;

	public override InteractionType Type => InteractionType.Breakable;

	private void Start()
	{
		render = GetComponent<SpriteRenderer>();
	}

	public void Break()
	{
		SingletonMonoScope<LevelManager>.Instance.BreakSP(type, sp, base.transform);
		SingletonMonoScope<ItemManager>.Instance.BreakDrop(base.transform, type);
		SingletonMonoGlobal<AudioManager>.Instance.SceneBreakOBJ(base.transform, Sound);
		LeanPool.Despawn(this);
	}

	protected override void OnHover(bool isHovering)
	{
		if (isHovering)
		{
			render.material.SetFloat(liang, 0f);
		}
		else
		{
			render.material.SetFloat(liang, 1f);
		}
	}

	public override void Interact()
	{
	}

	public override bool CanInteract()
	{
		return false;
	}
}
