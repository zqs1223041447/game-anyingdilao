using FMODUnity;
using FinkFramework.Runtime.Singleton;
using UI.CustomHandler;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ACT_skillBT : MonoBehaviour
{
	public Image image;

	public Image imageCD;

	public Image LowMana;

	[FormerlySerializedAs("Aura")]
	public Image AutoAT;

	public Sprite backGround;

	public bool IsCD;

	public float CDTime;

	public int index;

	private float JStime;

	public bool YanShi;

	public bool Opened;

	public bool AutoAttackEnabled = true;

	public string IndexName;

	public int Xi;

	public int SkillType;

	public ACTListSkillBT actL;

	public bool HasMana;

	public SkillOBJ_DT_SP SP;

	public SK_BuffA bf;

	private float timeA;

	private PlayerManager PL;

	private Button button;

	private UIButtonState buttonState;

	private ACTbar actbar;

	private GameUIManager _gameUIManager;

	private AudioManager _audioManager;

	private void Awake()
	{
		imageCD = base.transform.Find("CD").GetComponent<Image>();
		LowMana = base.transform.Find("LowMana").GetComponent<Image>();
		button = GetComponent<Button>();
		image = GetComponent<Image>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		actbar = SingletonMonoScope<ACTbar>.Instance;
		_gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		buttonState = GetComponent<UIButtonState>();
		button.onClick.AddListener(ToggleActListSkill);
		buttonState.onHoverEnter.AddListener(HoverEnter);
		buttonState.onHoverExit.AddListener(HoverExit);
	}

	private void OnDestroy()
	{
		buttonState.onHoverEnter.RemoveListener(HoverEnter);
		buttonState.onHoverExit.RemoveListener(HoverExit);
	}

	private void Start()
	{
		timeA = 0f;
		JStime = 0f;
		YanShi = false;
		Opened = false;
		IsCD = false;
		LowMana.fillAmount = 0f;
		HasMana = true;
	}

	private void Update()
	{
		if ((bool)actL)
		{
			if (HasMana)
			{
				if (actL.DT.ManaCost > PL.ManaStat.Cur)
				{
					LowMana.fillAmount = 1f;
					HasMana = false;
				}
			}
			else if (actL.DT.ManaCost <= PL.ManaStat.Cur)
			{
				LowMana.fillAmount = 0f;
				HasMana = true;
			}
			if (actL.IsCD)
			{
				imageCD.fillAmount = actL.Fill;
			}
			else
			{
				imageCD.fillAmount = 0f;
			}
			if (YanShi)
			{
				JStime += Time.deltaTime;
				if (JStime >= 0.2f)
				{
					_gameUIManager.ShowACTListSkillTip(Xi, SkillType, IndexName, base.transform, 0);
					JStime = 0f;
				}
			}
		}
		else
		{
			ClearRuntimeState();
		}
	}

	public void ClearRuntimeState()
	{
		if ((bool)imageCD)
		{
			imageCD.fillAmount = 0f;
		}
		if ((bool)LowMana)
		{
			LowMana.fillAmount = 0f;
		}
		HasMana = true;
		IsCD = false;
		YanShi = false;
		JStime = 0f;
	}

	public void HoverEnter()
	{
		if (Opened)
		{
			YanShi = true;
		}
		else if (!actbar.OpendSkillList)
		{
			_gameUIManager.ShowEmptySkillTip(base.transform);
		}
	}

	public void HoverExit()
	{
		YanShi = false;
		JStime = 0f;
		_gameUIManager.HideSkillTip();
		_gameUIManager.HideEmptyTip();
	}

	public void ToggleActListSkill()
	{
		if (actbar.actListSkill.Count > 0 && PL.IsAlive)
		{
			if (!actbar.OpendSkillList)
			{
				actbar.ShowACTListSkill(index, base.transform);
				YanShi = false;
				JStime = 0f;
				_gameUIManager.HideSkillTip();
				_gameUIManager.HideEmptyTip();
				RuntimeManager.PlayOneShot(_audioManager.audioData.Add_Point_1);
			}
			else
			{
				actbar.CloseSkillListUI();
				YanShi = true;
				JStime = 0f;
			}
		}
	}
}
