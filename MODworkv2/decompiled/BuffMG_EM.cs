using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class BuffMG_EM : MonoBehaviour
{
	public Enemy em;

	public Buffer_Enemy[] buffEM;

	public List<Buffer_Enemy> list = new List<Buffer_Enemy>();

	private void Awake()
	{
		em = base.transform.parent.GetComponent<Enemy>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
	}

	public void AddBuff(Buff_Enemy buff)
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		Buffer_Enemy component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.buffer_em, base.transform.position, Quaternion.identity, base.transform).GetComponent<Buffer_Enemy>();
		list.Add(component);
		component.AddBuff(buff, em.DotTimeCut);
		switch (buff.type)
		{
		case 0:
			em.AttackSpeed_Cut += buff.ATSpeedCut;
			em.MoveSpeed_Cut += buff.MVSpeedCut;
			em.H_HurtDMG_Buff += buff.HurtDamageAdd;
			switch (buff.damageType)
			{
			case DamageType.fire:
				em.FireAntiCut_Simple += buff.AntiCut;
				break;
			case DamageType.frozen:
				em.FrozenAntiCut_Simple += buff.AntiCut;
				break;
			case DamageType.thunder:
				em.ThunderAntiCut_Simple += buff.AntiCut;
				break;
			case DamageType.poison:
				em.PoisonAntiCut_Simple += buff.AntiCut;
				break;
			case DamageType.physics:
				em.PhysicsAntiCut_Simple += buff.AntiCut;
				break;
			case DamageType.shadow:
				em.ShadowAntiCut_Simple += buff.AntiCut;
				break;
			}
			break;
		case 1:
			em.Damage_Bei += buff.Damage;
			em.Chuan += buff.Chuan;
			em.Through += buff.Through;
			em.BJRate += buff.BJrate;
			em.GeDang += buff.GeDang;
			em.AttackSpeed_Bei += buff.AttackSpeed;
			em.MoveSpeed_Bei += buff.MoveSpeed;
			em.DamageAnti += buff.DamageAnti;
			em.Health_Prc += buff.Health_Prc;
			break;
		}
		em?.RefreshSpeedAndSetAni();
	}

	public void DelBuff(Buff_Enemy enemy, Buffer_Enemy buffer)
	{
		GameObject clone = buffer.gameObject;
		list.Remove(buffer);
		LeanPool.Despawn(clone);
		switch (enemy.type)
		{
		case 0:
			em.AttackSpeed_Cut -= enemy.ATSpeedCut;
			em.MoveSpeed_Cut -= enemy.MVSpeedCut;
			em.H_HurtDMG_Buff -= enemy.HurtDamageAdd;
			switch (enemy.damageType)
			{
			case DamageType.fire:
				em.FireAntiCut_Simple -= enemy.AntiCut;
				break;
			case DamageType.frozen:
				em.FrozenAntiCut_Simple -= enemy.AntiCut;
				break;
			case DamageType.thunder:
				em.ThunderAntiCut_Simple -= enemy.AntiCut;
				break;
			case DamageType.poison:
				em.PoisonAntiCut_Simple -= enemy.AntiCut;
				break;
			case DamageType.physics:
				em.PhysicsAntiCut_Simple -= enemy.AntiCut;
				break;
			case DamageType.shadow:
				em.ShadowAntiCut_Simple -= enemy.AntiCut;
				break;
			}
			break;
		case 1:
			em.Damage_Bei -= enemy.Damage;
			em.Chuan -= enemy.Chuan;
			em.Through -= enemy.Through;
			em.BJRate -= enemy.BJrate;
			em.GeDang -= enemy.GeDang;
			em.AttackSpeed_Bei -= enemy.AttackSpeed;
			em.MoveSpeed_Bei -= enemy.MoveSpeed;
			em.DamageAnti -= enemy.DamageAnti;
			em.Health_Prc -= enemy.Health_Prc;
			break;
		}
		em?.RefreshSpeedAndSetAni();
	}

	public void DelAll()
	{
		if (list.Count > 0)
		{
			int num;
			for (num = 0; num < list.Count; num++)
			{
				list[num].DelBuff();
				num--;
			}
		}
	}

	public void DelAllDebuff()
	{
		for (int num = list.Count - 1; num > -1; num--)
		{
			if (list[num].buff.type == 0)
			{
				list[num].DelBuff();
			}
		}
	}

	public void SetAni()
	{
		em?.SetAni();
	}

	public int GetDebuffCount()
	{
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].buff.type == 0)
			{
				num++;
			}
		}
		return num;
	}
}
