using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class XJ_DamageCol : MonoBehaviour
{
	public int Damage;

	public DamageType DMtype;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		this.wait(0.2f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void Update()
	{
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("FootCOL"))
		{
			FootCOL component = collision.GetComponent<FootCOL>();
			if (component.peo.CharacterType == 0)
			{
				component.peo.pl.TakeDamage(SingletonMonoScope<PlayerManager>.Instance.HealStat.Max * (float)Damage / 100f, 40f, 0f, 0f, DMtype, null);
			}
			if (component.peo.CharacterType == 1)
			{
				component.peo.cp.TakeDamage(component.peo.cp.HealthStat.MaxValue * (float)Damage / 100f, 40f, 0f, 0f, 0f, DMtype, null);
			}
		}
	}
}
