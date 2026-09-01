using FinkFramework.Runtime.Singleton;
using Inputs.Gamepad;
using SK.Framework;
using Spine;
using Spine.Unity;
using UnityEngine;

public class DEAD : MonoBehaviour
{
	public PlayerManager PLY;

	public SkeletonAnimation ani;

	public Spine.AnimationState stat;

	private AnimationStateData data;

	[SpineEvent("", "", true, false, false)]
	public string[] SPevent;

	public bool ISright;

	public bool Zheng;

	public StateMachine STA;

	public GameObject tar;

	public GameObject tar2;

	[SpineAnimation("", "", true, false)]
	public string AttackA;

	[SpineAnimation("", "", true, false)]
	public string AttackB;

	[SpineAnimation("", "", true, false)]
	public string SkillA;

	[SpineAnimation("", "", true, false)]
	public string die;

	public float JStimeA;

	public bool ISwalk
	{
		get
		{
			if (PLY.IsAlive && PLY.IsMoving)
			{
				return Zheng;
			}
			return false;
		}
	}

	public bool ISwalkB
	{
		get
		{
			if (PLY.IsAlive && PLY.IsMoving)
			{
				return !Zheng;
			}
			return false;
		}
	}

	public bool ISidle
	{
		get
		{
			if (!PLY.IsMoving)
			{
				return PLY.IsAlive;
			}
			return false;
		}
	}

	private void Awake()
	{
		PLY = base.transform.parent.parent.GetComponent<PlayerManager>();
		tar = base.transform.Find("SkeletonUtility-SkeletonRoot/root/tar").gameObject;
		tar2 = base.transform.Find("SkeletonUtility-SkeletonRoot/root/tar2").gameObject;
		ani = GetComponent<SkeletonAnimation>();
		stat = ani.AnimationState;
		data = stat.Data;
		stat.Event += OnUserDefinedEvent;
		data.SetMix("idle", "walk", 0f);
		data.SetMix("idle", "walkB", 0f);
	}

	private void Start()
	{
		ISright = true;
		statMS();
	}

	private void Update()
	{
		ChangeAni();
	}

	public void SetStart()
	{
	}

	public void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e)
	{
		SingletonMonoScope<Gun>.Instance.SetAnimationEventTrack(trackEntry);
		if (e.Data.Name == SPevent[0])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.DEADattack();
			}
		}
		_ = e.Data.Name == SPevent[1];
		if (e.Data.Name == SPevent[2])
		{
			stat.SetEmptyAnimation(1, 0.1f);
			PLY.IsAttack = false;
			if (PLY.IsAttackAnimationSkill)
			{
				PLY.IsSkill = false;
				PLY.IsAttackAnimationSkill = false;
			}
		}
		if (e.Data.Name == SPevent[3])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.DEADattack();
			}
		}
		_ = e.Data.Name == SPevent[4];
		if (e.Data.Name == SPevent[5])
		{
			stat.SetEmptyAnimation(1, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		if (e.Data.Name == SPevent[6])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.DEADattack();
			}
		}
		_ = e.Data.Name == SPevent[7];
		_ = e.Data.Name == SPevent[8];
		if (e.Data.Name == SPevent[9])
		{
			PLY.CanMove = true;
			stat.SetEmptyAnimation(2, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		_ = e.Data.Name == SPevent[10];
		_ = e.Data.Name == SPevent[11];
		_ = e.Data.Name == SPevent[12];
	}

	public void ChangeAni()
	{
		AimContext currentAimContext = AimProvider.GetCurrentAimContext();
		Vector3 fallbackAim = new Vector3(currentAimContext.WorldPoint.x, currentAimContext.WorldPoint.y, 0f);
		Vector3 battleAimWorldPosition = PLY.GetBattleAimWorldPosition(fallbackAim, tar.transform, tar2.transform);
		Vector3 vector = battleAimWorldPosition - PLY.transform.position;
		if (PLY.IsAlive)
		{
			if (PLY.IsBattle)
			{
				tar.transform.position = battleAimWorldPosition;
				if (vector.x >= 0f)
				{
					ISright = true;
					ani.skeleton.ScaleX = 1f;
				}
				if (vector.x < 0f)
				{
					ISright = false;
					ani.skeleton.ScaleX = -1f;
				}
				if (ani.skeleton.ScaleX * PLY.Direction.x < 0f)
				{
					Zheng = false;
				}
				if (ani.skeleton.ScaleX * PLY.Direction.x > 0f)
				{
					Zheng = true;
				}
			}
			else
			{
				tar.transform.position = Vector2.Lerp(tar.transform.position, tar2.transform.position, 0.3f);
				Zheng = true;
				if (PLY.Direction.x > 0f)
				{
					ISright = true;
					ani.skeleton.ScaleX = 1f;
				}
				if (PLY.Direction.x < 0f)
				{
					ISright = false;
					ani.skeleton.ScaleX = -1f;
				}
			}
		}
		if (PLY.IsBattle && !PLY.IsAttack)
		{
			JStimeA += Time.deltaTime;
			if (JStimeA >= 2f)
			{
				PLY.IsBattle = false;
				JStimeA = 0f;
			}
		}
	}

	public void idleON()
	{
		PLY.PlayMoveAnimationIfNeeded(stat, "idle", loop: true, 1f);
	}

	public void walkON()
	{
		PLY.PlayMoveAnimationIfNeeded(stat, "walk", loop: true, PLY.MoveAnimationTimeScale);
	}

	public void walkBON()
	{
		PLY.PlayMoveAnimationIfNeeded(stat, "walkB", loop: true, PLY.MoveAnimationTimeScale);
	}

	public void Die()
	{
		stat.SetEmptyAnimation(0, 0f);
		stat.SetEmptyAnimation(1, 0f);
		stat.SetEmptyAnimation(2, 0f);
		stat.SetAnimation(0, string.IsNullOrEmpty(die) ? "die" : die, loop: false).TimeScale = 1f;
	}

	public void statMS()
	{
		if (FSM.Instance.GetMachine<StateMachine>("DEAD") != null)
		{
			StateMachine.Destroy("DEAD");
		}
		STA = StateMachine.Create("DEAD").Build<State>("idle").OnEnter(delegate
		{
			idleON();
		})
			.OnStay(delegate
			{
				Nothing();
			})
			.Complete()
			.Build<State>("walk")
			.OnEnter(delegate
			{
				walkON();
			})
			.OnStay(delegate
			{
				Nothing();
			})
			.Complete()
			.Build<State>("walkB")
			.OnEnter(delegate
			{
				walkBON();
			})
			.OnStay(delegate
			{
				Nothing();
			})
			.Complete()
			.Build<State>("Die")
			.OnEnter(delegate
			{
				Die();
			})
			.OnStay(delegate
			{
				Nothing();
			})
			.Complete()
			.SwitchWhen(() => !PLY.IsAlive, "Die")
			.SwitchWhen(() => ISidle, "idle")
			.SwitchWhen(() => ISwalk, "walk")
			.SwitchWhen(() => ISwalkB, "walkB");
	}

	public void ACT(int anim)
	{
		PLY.IsAttack = true;
		PLY.IsBattle = true;
		switch (anim)
		{
		case 0:
			PLY.AttackTrack = stat.SetAnimation(1, AttackA, loop: true);
			PLY.AttackTrack.TimeScale = PLY.AttackAnimationTimeScale;
			break;
		case 1:
			PLY.IsSkill = true;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.AttackTrack = stat.SetAnimation(1, AttackB, loop: false);
			PLY.AttackTrack.TimeScale = PLY.AttackAnimationTimeScale;
			break;
		case 2:
			PLY.IsSkill = true;
			PLY.CanMove = false;
			stat.AddEmptyAnimation(1, 0f, 0f);
			stat.AddEmptyAnimation(2, 0f, 0f);
			PLY.SkillTrack = stat.SetAnimation(2, SkillA, loop: false);
			PLY.SkillTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		}
	}

	public bool ReturnAni()
	{
		if (ISwalk || ISwalkB)
		{
			return false;
		}
		return true;
	}

	public void Nothing()
	{
	}
}
