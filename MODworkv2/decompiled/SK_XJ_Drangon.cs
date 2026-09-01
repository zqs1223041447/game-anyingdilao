using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SK_XJ_Drangon : MonoBehaviour
{
	public string SoundA;

	public string SoundAT;

	public Skill_PB_List[] OBJ;

	public EnemyColorGroup[] CLG;

	public GameObject[] DZ;

	[HideInInspector]
	public SK_DZ Keng;

	[SpineEvent("", "", true, false, false)]
	public string[] SPevent;

	[SpineAnimation("", "", true, false)]
	public string born;

	[SpineAnimation("", "", true, false)]
	public string idle;

	[SpineAnimation("", "", true, false)]
	public string attackA;

	[SpineAnimation("", "", true, false)]
	public string die;

	public Transform ATpoint;

	public float ATtime;

	public float DelDelay;

	public List<Enemy> em = new List<Enemy>();

	public List<Companion> cp = new List<Companion>();

	public List<PlayerManager> pl = new List<PlayerManager>();

	public Collider2D[] hitEM = new Collider2D[6];

	public Collider2D[] hitCP = new Collider2D[3];

	public Collider2D[] hitPL = new Collider2D[1];

	[HideInInspector]
	public GameObject tar;

	[HideInInspector]
	public GameObject tar2;

	[HideInInspector]
	public SkeletonAnimation ani;

	private Spine.AnimationState stat;

	private AnimationStateData data;

	private MaterialPropertyBlock mpb;

	[HideInInspector]
	public Transform target;

	[HideInInspector]
	public MeshRenderer render;

	[HideInInspector]
	public Dicform dic;

	private bool ISattack;

	private bool CanAT;

	private float timeB;

	private float timeC;

	private float timeD;

	[HideInInspector]
	public Vector3 pos;

	public TrackEntry MoveTrack;

	private int MainEL;

	private int RD;

	private int IndexA;

	private float range;

	private bool initialized;

	public bool nearPlayer => Vector2.Distance(base.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < Vector2.Distance(base.transform.position, cp[0].transform.position);

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		ani = base.transform.Find("main/Spine").gameObject.GetComponent<SkeletonAnimation>();
		render = base.transform.Find("main/Spine").GetComponent<MeshRenderer>();
		tar = base.transform.Find("main/Spine/SkeletonUtility-SkeletonRoot/tar").gameObject;
		tar2 = base.transform.Find("main/Spine/SkeletonUtility-SkeletonRoot/tar2").gameObject;
		ATpoint = base.transform.Find("main/Spine/AT");
		stat = ani.AnimationState;
		data = stat.Data;
		data.SetMix("idle", "attackA", 0.2f);
		data.SetMix("attackA", "idle", 0.2f);
		stat.Event += OnUserDefinedEvent;
		MoveTrack = new TrackEntry();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		CanAT = false;
		target = null;
		ISattack = false;
		em.Clear();
		cp.Clear();
		pl.Clear();
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		for (int j = 0; j < hitCP.Length; j++)
		{
			hitCP[j] = null;
		}
		hitPL[0] = null;
		mpb = new MaterialPropertyBlock();
		mpb.SetFloat("_Dislove", 0f);
		mpb.SetFloat("_MainAlpha", 0f);
		render.SetPropertyBlock(mpb);
		initialized = false;
	}

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		MainEL = dic.sp.MainEL;
		Keng = LeanPool.Spawn(DZ[MainEL], new Vector3(base.transform.position.x, base.transform.position.y - 0.02f, base.transform.position.z), Quaternion.identity).GetComponent<SK_DZ>();
		Skin skin = new Skin("skin");
		skin.Clear();
		skin.AddSkin(ani.Skeleton.Data.FindSkin(CLG[0].XI[0].CL[MainEL].SkinName));
		ani.Skeleton.SetSkin(skin);
		ani.Skeleton.SetSlotsToSetupPose();
		mpb.SetFloat("_Dislove", 1f);
		mpb.SetFloat("_MainAlpha", 1f);
		mpb.SetInt("_Flip", CLG[0].XI[0].CL[MainEL].Flip);
		mpb.SetVector("_MainMix", CLG[0].XI[0].CL[MainEL].MainMix);
		mpb.SetInt("_MainHue", CLG[0].XI[0].CL[MainEL].MainHue);
		mpb.SetFloat("_MainSat", CLG[0].XI[0].CL[MainEL].MainSat);
		mpb.SetColor("_MainColor", CLG[0].XI[0].CL[MainEL].MainColor);
		mpb.SetColor("_DisloveColor", CLG[0].XI[0].CL[MainEL].DisloveColor);
		mpb.SetColor("_AlphaColor", CLG[0].XI[0].CL[MainEL].AlphaColor);
		render.SetPropertyBlock(mpb);
		stat.SetAnimation(0, born, loop: false);
		if (dic.sp.ZY)
		{
			pos = base.transform.position - SingletonMonoScope<PlayerManager>.Instance.transform.position;
			if (pos.x >= 0f)
			{
				ani.skeleton.ScaleX = 1f;
			}
			if (pos.x < 0f)
			{
				ani.skeleton.ScaleX = -1f;
			}
		}
		else
		{
			pos = base.transform.position - dic.sp.em.transform.position;
			if (pos.x >= 0f)
			{
				ani.skeleton.ScaleX = 1f;
			}
			if (pos.x < 0f)
			{
				ani.skeleton.ScaleX = -1f;
			}
		}
		range = dic.sp.Range_AT;
		IndexA = dic.sp.ZD_F;
		CanAT = true;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= dic.sp.BuffTime)
		{
			MoveTrack = ani.AnimationState.SetAnimation(0, die, loop: false);
			MoveTrack.TimeScale = 1f;
			CanAT = false;
			timeB = 0f;
		}
		CheckPOS();
		if (target != null && !ISattack)
		{
			timeC += Time.deltaTime;
			if (timeC >= ATtime)
			{
				timeC = 0f;
				ISattack = true;
				MoveTrack = ani.AnimationState.SetAnimation(0, attackA, loop: false);
				MoveTrack.Complete += OnSpineAnimationComplete;
				MoveTrack.TimeScale = 1f;
			}
		}
		timeD += Time.deltaTime;
		if (timeD >= 0.15f)
		{
			timeD = 0f;
			Refresh();
		}
	}

	public void CheckPOS()
	{
		if (dic.sp.ZY)
		{
			if (em.Count > 0)
			{
				pos = em[0].peo.em.transform.position - base.transform.position;
				if (pos.x >= 0f)
				{
					ani.skeleton.ScaleX = 1f;
				}
				if (pos.x < 0f)
				{
					ani.skeleton.ScaleX = -1f;
				}
				tar.transform.position = em[0].peo.em.yao.transform.position;
			}
			else
			{
				tar.transform.position = Vector2.Lerp(tar.transform.position, tar2.transform.position, 0.05f);
			}
		}
		else if (cp.Count > 0 || pl.Count > 0)
		{
			if (cp.Count > 0 && pl.Count > 0)
			{
				if (nearPlayer)
				{
					pos = pl[0].peo.pl.transform.position - base.transform.position;
					if (pos.x >= 0f)
					{
						ani.skeleton.ScaleX = 1f;
					}
					if (pos.x < 0f)
					{
						ani.skeleton.ScaleX = -1f;
					}
					tar.transform.position = pl[0].peo.pl.yao.transform.position;
				}
				else
				{
					pos = cp[0].peo.cp.transform.position - base.transform.position;
					if (pos.x >= 0f)
					{
						ani.skeleton.ScaleX = 1f;
					}
					if (pos.x < 0f)
					{
						ani.skeleton.ScaleX = -1f;
					}
					tar.transform.position = cp[0].peo.cp.yao.transform.position;
				}
			}
			else if (cp.Count > 0 && pl.Count < 0)
			{
				pos = cp[0].peo.cp.transform.position - base.transform.position;
				if (pos.x >= 0f)
				{
					ani.skeleton.ScaleX = 1f;
				}
				if (pos.x < 0f)
				{
					ani.skeleton.ScaleX = -1f;
				}
				tar.transform.position = cp[0].peo.cp.yao.transform.position;
			}
			else
			{
				pos = pl[0].peo.pl.transform.position - base.transform.position;
				if (pos.x >= 0f)
				{
					ani.skeleton.ScaleX = 1f;
				}
				if (pos.x < 0f)
				{
					ani.skeleton.ScaleX = -1f;
				}
				tar.transform.position = pl[0].peo.pl.yao.transform.position;
			}
		}
		else
		{
			tar.transform.position = Vector2.Lerp(tar.transform.position, tar2.transform.position, 0.05f);
		}
	}

	public void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e)
	{
		if (e.Data.Name == SPevent[0])
		{
			CanAT = true;
			MoveTrack = ani.AnimationState.SetAnimation(0, idle, loop: true);
			MoveTrack.TimeScale = 1f;
		}
		_ = e.Data.Name == SPevent[1];
		if (e.Data.Name == SPevent[2] && target != null)
		{
			if (dic.sp.ZY)
			{
				if (em.Count > 0)
				{
					target = em[0].peo.em.yao.transform;
				}
				else
				{
					target = null;
				}
			}
			else if (cp.Count > 0)
			{
				if (pl.Count > 0)
				{
					if (Vector2.Distance(base.transform.position, pl[0].yao.transform.position) < Vector2.Distance(base.transform.position, cp[0].peo.cp.yao.transform.position))
					{
						target = pl[0].yao.transform;
					}
					else
					{
						target = cp[0].peo.cp.yao.transform;
					}
				}
				else
				{
					target = cp[0].peo.cp.yao.transform;
				}
			}
			else if (pl.Count > 0)
			{
				target = pl[0].yao.transform;
			}
			else
			{
				target = null;
			}
			if (target != null)
			{
				RD = Random.Range(0, 101);
				if (RD < 30 && SoundAT != null)
				{
					RuntimeManager.PlayOneShot(SoundAT, base.transform.position);
				}
				Vector3 vector = target.position - ATpoint.transform.position;
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (dic.sp.Count_F % 2 == 1)
				{
					Dicform component = LeanPool.Spawn(OBJ[MainEL].PB[IndexA], ATpoint.transform.position, Quaternion.Euler(0f, 0f, num)).GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = 0;
					if (dic.sp.Count_F > 1)
					{
						for (int i = 0; i < dic.sp.Count_F / 2; i++)
						{
							GameObject gameObject = LeanPool.Spawn(OBJ[MainEL].PB[IndexA], ATpoint.transform.position, Quaternion.Euler(0f, 0f, num + dic.sp.AngleA * (float)(i + 1)));
							GameObject obj = LeanPool.Spawn(OBJ[MainEL].PB[IndexA], ATpoint.transform.position, Quaternion.Euler(0f, 0f, num - dic.sp.AngleA * (float)(i + 1)));
							Dicform component2 = gameObject.GetComponent<Dicform>();
							component2.sp = dic.sp;
							component2.SetCount(dic.sp.ZY);
							component2.SubType = 0;
							Dicform component3 = obj.GetComponent<Dicform>();
							component3.sp = dic.sp;
							component3.SetCount(dic.sp.ZY);
							component3.SubType = 0;
						}
					}
				}
				else
				{
					for (int j = 0; j < dic.sp.Count_F / 2; j++)
					{
						GameObject gameObject2 = LeanPool.Spawn(OBJ[MainEL].PB[IndexA], ATpoint.transform.position, Quaternion.Euler(0f, 0f, num + dic.sp.AngleA * (float)(j + 1) - dic.sp.AngleA / 2f));
						GameObject obj2 = LeanPool.Spawn(OBJ[MainEL].PB[IndexA], ATpoint.transform.position, Quaternion.Euler(0f, 0f, num - dic.sp.AngleA * (float)(j + 1) + dic.sp.AngleA / 2f));
						Dicform component4 = gameObject2.GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.SubType = 0;
						Dicform component5 = obj2.GetComponent<Dicform>();
						component5.sp = dic.sp;
						component5.SetCount(dic.sp.ZY);
						component5.SubType = 0;
					}
				}
			}
		}
		if (e.Data.Name == SPevent[3])
		{
			Stop();
		}
	}

	public void OnSpineAnimationComplete(TrackEntry trackEntry)
	{
		if (ISattack)
		{
			ISattack = false;
			stat.SetAnimation(0, idle, loop: true).TimeScale = 1f;
		}
	}

	public void Stop()
	{
		mpb.SetFloat("_Dislove", 0f);
		mpb.SetFloat("_MainAlpha", 0f);
		render.SetPropertyBlock(mpb);
		Keng.Stop();
		Keng = null;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void Refresh()
	{
		if (dic.sp.ZY)
		{
			if (em.Count > 0)
			{
				target = em[0].peo.em.yao.transform;
			}
			else
			{
				target = null;
			}
		}
		else if (cp.Count > 0)
		{
			if (pl.Count > 0)
			{
				if (Vector2.Distance(base.transform.position, pl[0].yao.transform.position) < Vector2.Distance(base.transform.position, cp[0].peo.cp.yao.transform.position))
				{
					target = pl[0].yao.transform;
				}
				else
				{
					target = cp[0].peo.cp.yao.transform;
				}
			}
			else
			{
				target = cp[0].peo.cp.yao.transform;
			}
		}
		else if (pl.Count > 0)
		{
			target = pl[0].yao.transform;
		}
		else
		{
			target = null;
		}
		if (dic.sp.ZY)
		{
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("BodyCOLem"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					BodyCOL component = hitEM[i].GetComponent<BodyCOL>();
					if ((bool)component)
					{
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
						{
							em.Add(component.peo.em);
						}
						hitEM[i] = null;
					}
				}
			}
		}
		else
		{
			int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitCP, LayerMask.GetMask("BodyCOLcp"));
			if (num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					BodyCOL component2 = hitCP[j].GetComponent<BodyCOL>();
					if ((bool)component2)
					{
						if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive && !cp.Contains(component2.peo.cp))
						{
							cp.Add(component2.peo.cp);
						}
						hitCP[j] = null;
					}
				}
			}
			int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitPL, LayerMask.GetMask("BodyCOLpl"));
			if (num3 > 0)
			{
				for (int k = 0; k < num3; k++)
				{
					BodyCOL component3 = hitPL[k].GetComponent<BodyCOL>();
					if ((bool)component3)
					{
						if (component3.peo.CharacterType == 0 && component3.peo.pl.IsAlive && !pl.Contains(component3.peo.pl))
						{
							pl.Add(component3.peo.pl);
						}
						hitPL[k] = null;
					}
				}
			}
		}
		if (dic.sp.ZY)
		{
			if (em.Count <= 0)
			{
				return;
			}
			for (int l = 0; l < em.Count; l++)
			{
				if (!em[l].IsAlive || em[l].IsYS || em[l].IsJump || Vector3.Distance(em[l].transform.position, base.transform.position) > range)
				{
					em.Remove(em[l]);
					l--;
				}
			}
			em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
			return;
		}
		if (cp.Count > 0)
		{
			for (int m = 0; m < cp.Count; m++)
			{
				if (!cp[m].IsAlive || Vector3.Distance(cp[m].transform.position, base.transform.position) > range)
				{
					cp.Remove(cp[m]);
					m--;
				}
			}
			cp.Sort((Companion t1, Companion t2) => Vector3.Distance(t1.yao.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.yao.transform.position, base.transform.position)));
		}
		if (pl.Count > 0 && (!pl[0].IsAlive || Vector3.Distance(pl[0].transform.position, base.transform.position) > range))
		{
			pl.Remove(pl[0]);
		}
	}
}
