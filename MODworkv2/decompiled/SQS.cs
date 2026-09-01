using FinkFramework.Runtime.Singleton;
using Inputs.Gamepad;
using SK.Framework;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SQS : MonoBehaviour
{
	private static readonly int flip = Shader.PropertyToID("_Flip");

	private static readonly int qiangDu = Shader.PropertyToID("_QiangDu");

	private static readonly int change = Shader.PropertyToID("_Change");

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
	public string AttackAA;

	[SpineAnimation("", "", true, false)]
	public string AttackAB;

	[SpineAnimation("", "", true, false)]
	public string AttackB;

	[SpineAnimation("", "", true, false)]
	public string AttackC;

	[SpineAnimation("", "", true, false)]
	public string AttackDUN;

	[SpineAnimation("", "", true, false)]
	public string ChongDUN;

	[SpineAnimation("", "", true, false)]
	public string ChongSS;

	[SpineAnimation("", "", true, false)]
	public string skillA;

	[SpineAnimation("", "", true, false)]
	public string skillB;

	[SpineAnimation("", "", true, false)]
	public string skillC;

	[SpineAnimation("", "", true, false)]
	public string die;

	public float JStimeA;

	public Skin newSK;

	public int SKindex;

	public Material mat;

	[SerializeField]
	private MeshRenderer render;

	private MaterialPropertyBlock mpb;

	public GameObject[] FXOBJ;

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
		render = GetComponent<MeshRenderer>();
		stat = ani.AnimationState;
		data = stat.Data;
		stat.Event += OnUserDefinedEvent;
		mat = ani.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
		mpb = new MaterialPropertyBlock();
		newSK = new Skin("skin");
		SKindex = 0;
		ChangeSkin(SKindex);
		data.SetMix("idleA", "walk", 0f);
		data.SetMix("idleA", "walkB", 0f);
		data.SetMix("idleB", "walk", 0f);
		data.SetMix("idleB", "walkB", 0f);
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
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[1];
		_ = e.Data.Name == SPevent[2];
		if (e.Data.Name == SPevent[3])
		{
			stat.SetEmptyAnimation(1, 0.2f);
			PLY.IsAttack = false;
			if (PLY.IsAttackAnimationSkill)
			{
				PLY.IsSkill = false;
				PLY.IsAttackAnimationSkill = false;
			}
		}
		if (e.Data.Name == SPevent[4])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[5];
		if (e.Data.Name == SPevent[6])
		{
			stat.SetEmptyAnimation(1, 0.2f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		if (e.Data.Name == SPevent[7])
		{
			stat.SetEmptyAnimation(1, 0.2f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		if (e.Data.Name == SPevent[8])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[9];
		if (e.Data.Name == SPevent[10])
		{
			stat.SetEmptyAnimation(1, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		if (e.Data.Name == SPevent[11])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[12];
		if (e.Data.Name == SPevent[13])
		{
			PLY.CanMove = true;
			stat.SetEmptyAnimation(2, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
			PLY.IsChong = false;
			PLY.rigBD.velocity = Vector2.zero;
			PLY.Direction = Vector2.zero;
		}
		if (e.Data.Name == SPevent[14])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[15];
		if (e.Data.Name == SPevent[16])
		{
			PLY.CanMove = true;
			stat.SetEmptyAnimation(2, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
			PLY.IsChong = false;
			PLY.rigBD.velocity = Vector2.zero;
			PLY.Direction = Vector2.zero;
		}
		if (e.Data.Name == SPevent[17])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[18];
		_ = e.Data.Name == SPevent[19];
		if (e.Data.Name == SPevent[20])
		{
			PLY.CanMove = true;
			stat.SetEmptyAnimation(2, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		if (e.Data.Name == SPevent[21])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[22];
		_ = e.Data.Name == SPevent[23];
		if (e.Data.Name == SPevent[24])
		{
			PLY.CanMove = true;
			stat.SetEmptyAnimation(2, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		if (e.Data.Name == SPevent[25])
		{
			if (PLY.IScomp)
			{
				SingletonMonoScope<Gun>.Instance.Summon();
			}
			else
			{
				SingletonMonoScope<Gun>.Instance.SQSattack();
			}
		}
		_ = e.Data.Name == SPevent[26];
		_ = e.Data.Name == SPevent[27];
		if (e.Data.Name == SPevent[28])
		{
			PLY.CanMove = true;
			stat.SetEmptyAnimation(2, 0.1f);
			PLY.IsSkill = false;
			PLY.IsAttack = false;
		}
		_ = e.Data.Name == SPevent[29];
		_ = e.Data.Name == SPevent[30];
		_ = e.Data.Name == SPevent[31];
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
		switch (SKindex)
		{
		case 0:
		case 1:
			PLY.PlayMoveAnimationIfNeeded(stat, "idleB", loop: true, 1f);
			break;
		case 2:
			PLY.PlayMoveAnimationIfNeeded(stat, "idleA", loop: true, 1f);
			break;
		}
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
		if (FSM.Instance.GetMachine<StateMachine>("SQS") != null)
		{
			StateMachine.Destroy("SQS");
		}
		STA = StateMachine.Create("SQS").Build<State>("idle").OnEnter(delegate
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
			switch (SKindex)
			{
			case 0:
			case 1:
				PLY.AttackTrack = stat.SetAnimation(1, AttackAA, loop: true);
				break;
			case 2:
				PLY.AttackTrack = stat.SetAnimation(1, AttackC, loop: true);
				break;
			}
			PLY.AttackTrack.TimeScale = PLY.AttackAnimationTimeScale;
			break;
		case 1:
			PLY.IsSkill = true;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.AttackTrack = stat.SetAnimation(1, AttackB, loop: false);
			PLY.AttackTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 2:
			PLY.IsSkill = true;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.AttackTrack = stat.SetAnimation(1, AttackDUN, loop: false);
			PLY.AttackTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 3:
			PLY.IsSkill = true;
			PLY.IsChong = true;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.AttackTrack = stat.SetAnimation(2, ChongDUN, loop: false);
			PLY.AttackTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 4:
			PLY.IsSkill = true;
			PLY.IsChong = true;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.SkillTrack = stat.SetAnimation(2, ChongSS, loop: false);
			PLY.SkillTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 5:
			PLY.IsSkill = true;
			PLY.CanMove = false;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.SkillTrack = stat.SetAnimation(2, skillA, loop: false);
			PLY.SkillTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 6:
			PLY.IsSkill = true;
			PLY.CanMove = false;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.SkillTrack = stat.SetAnimation(2, skillB, loop: false);
			PLY.SkillTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 7:
			PLY.IsSkill = true;
			PLY.CanMove = false;
			stat.AddEmptyAnimation(1, 0f, 0f);
			PLY.SkillTrack = stat.SetAnimation(2, skillC, loop: false);
			PLY.SkillTrack.TimeScale = PLY.SkillAnimationTimeScale;
			break;
		case 8:
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

	public void ChangeSkin(int index)
	{
		newSK.Clear();
		switch (index)
		{
		case 0:
			newSK.AddSkin(ani.Skeleton.Data.FindSkin("AAA"));
			newSK.AddSkin(ani.Skeleton.Data.FindSkin("dunA"));
			mpb.SetInt(flip, 0);
			mpb.SetFloat(qiangDu, 0f);
			mpb.SetFloat(change, 0f);
			render.SetPropertyBlock(mpb);
			break;
		case 1:
			newSK.AddSkin(ani.Skeleton.Data.FindSkin("BBB"));
			newSK.AddSkin(ani.Skeleton.Data.FindSkin("dunB"));
			mpb.SetInt(flip, 1);
			mpb.SetFloat(qiangDu, 0f);
			mpb.SetFloat(change, 0f);
			render.SetPropertyBlock(mpb);
			break;
		case 2:
			newSK.AddSkin(ani.Skeleton.Data.FindSkin("AAA"));
			newSK.AddSkin(ani.Skeleton.Data.FindSkin("sword"));
			mpb.SetInt(flip, 2);
			mpb.SetFloat(qiangDu, 0.8f);
			mpb.SetFloat(change, 1.4f);
			render.SetPropertyBlock(mpb);
			break;
		}
		SKindex = index;
		ani.Skeleton.SetSkin(newSK);
		ani.Skeleton.SetSlotsToSetupPose();
	}

	public void FXon()
	{
		for (int i = 0; i < FXOBJ.Length; i++)
		{
			FXOBJ[i].SetActive(value: true);
		}
	}

	public void FXoff()
	{
		for (int i = 0; i < FXOBJ.Length; i++)
		{
			FXOBJ[i].SetActive(value: false);
		}
	}

	public void Nothing()
	{
	}
}
