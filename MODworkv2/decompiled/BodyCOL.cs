using FinkFramework.Runtime.Singleton;
using Interact;
using UnityEngine;

public class BodyCOL : InteractableBase
{
	[HideInInspector]
	public People peo;

	public override InteractionType Type => InteractionType.Enemy;

	private Enemy CachedEnemy
	{
		get
		{
			if (!peo)
			{
				return null;
			}
			return peo.em;
		}
	}

	private void Awake()
	{
		CachePeople();
	}

	private void OnEnable()
	{
		CachePeople();
	}

	private void CachePeople()
	{
		Transform transform = (base.transform.parent ? base.transform.parent.Find("People") : null);
		if ((bool)transform)
		{
			peo = transform.GetComponent<People>();
		}
	}

	public override bool CanInteract()
	{
		return false;
	}

	public override void Interact()
	{
	}

	protected override void OnHover(bool isHovering)
	{
		Enemy cachedEnemy = CachedEnemy;
		if ((bool)cachedEnemy && (bool)peo && peo.CharacterType == 2 && !cachedEnemy.IS_Boss && cachedEnemy.IsAlive && !cachedEnemy.IsJump && !cachedEnemy.IsYS)
		{
			if (isHovering)
			{
				SingletonMonoScope<UI_EnemyTip>.Instance.NotifyMouseHoverEnter(cachedEnemy);
			}
			else
			{
				SingletonMonoScope<UI_EnemyTip>.Instance.NotifyMouseHoverExit(cachedEnemy);
			}
		}
	}
}
