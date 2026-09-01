using FMODUnity;
using FinkFramework.Runtime.Singleton;
using UI.CustomHandler;
using UI.UIItems;
using UnityEngine;
using UnityEngine.UI;

public class ACT_UseBT : MonoBehaviour
{
	public Image image;

	public Image imageCD;

	public Sprite backGround;

	public bool IsCD;

	public int index;

	private float JStime;

	public bool YanShi;

	public bool Opend;

	public string IndexName;

	public string Type;

	public int stackSize;

	public Text stackText;

	[HideInInspector]
	public BuffSimpleItem slot;

	private ACTbar actbar;

	private GameUIManager _gameUIManager;

	private AudioManager _audioManager;

	private PlayerManager PL;

	private Button button;

	private UIButtonState buttonState;

	private void Awake()
	{
		imageCD = base.transform.Find("CD").GetComponent<Image>();
		stackText = base.transform.Find("stackSize").GetComponent<Text>();
		stackText.gameObject.SetActive(value: false);
		image = GetComponent<Image>();
		actbar = SingletonMonoScope<ACTbar>.Instance;
		_gameUIManager = SingletonMonoScope<GameUIManager>.Instance;
		_audioManager = SingletonMonoGlobal<AudioManager>.Instance;
		PL = SingletonMonoScope<PlayerManager>.Instance;
		button = GetComponent<Button>();
		buttonState = GetComponent<UIButtonState>();
		button.onClick.AddListener(ToggleActListUse);
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
		JStime = 0f;
		YanShi = false;
		Opend = false;
	}

	private void Update()
	{
		if (Opend && IsCD && (bool)slot)
		{
			imageCD.fillAmount = slot.Fill;
		}
		if (!YanShi)
		{
			return;
		}
		JStime += Time.deltaTime;
		if (JStime >= 0.2f)
		{
			SlotData slotData = SingletonMonoScope<InventoryManager>.Instance.ReturnSameUse(IndexName);
			if (slotData != null)
			{
				_gameUIManager.ShowACTUseTip(base.transform.position, slotData.useitem);
			}
			JStime = 0f;
		}
	}

	public void HoverEnter()
	{
		if (Opend)
		{
			YanShi = true;
		}
		else if (!actbar.OpendUseList)
		{
			_gameUIManager.ShowEmptyUseTip(base.transform);
		}
	}

	public void HoverExit()
	{
		YanShi = false;
		JStime = 0f;
		_gameUIManager.HideUseTip();
		_gameUIManager.HideEmptyTip();
	}

	public void ToggleActListUse()
	{
		if (actbar.UseListCount > 0 && PL.IsAlive)
		{
			if (!actbar.OpendUseList)
			{
				actbar.PruneInvalidUseList();
				actbar.ShowACTListUse(index, base.transform);
				YanShi = false;
				JStime = 0f;
				_gameUIManager.HideUseTip();
				_gameUIManager.HideEmptyTip();
				RuntimeManager.PlayOneShot(_audioManager.audioData.Quick_SK_Open);
			}
			else
			{
				actbar.CloseUseListUI();
				YanShi = false;
				JStime = 0f;
			}
		}
	}

	public void RefreshStack(int a)
	{
		stackSize = a;
		if (stackSize > 999)
		{
			stackText.text = "999+";
		}
		else
		{
			stackText.text = stackSize.ToString();
		}
	}
}
