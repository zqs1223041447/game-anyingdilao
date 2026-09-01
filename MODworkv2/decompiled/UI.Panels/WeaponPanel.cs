using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.UI;
using Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels;

public class WeaponPanel : GamepadSelectablePanel
{
	[SerializeField]
	private string audio_weapon_success;

	private Button confirmButton;

	private Text titleText;

	[HideInInspector]
	public CanvasGroup WeaponCavOld;

	[HideInInspector]
	public Text WP_titleO;

	[HideInInspector]
	public Text WP_typeO;

	[HideInInspector]
	public Text WP_levelO;

	[HideInInspector]
	public Text WP_mainO;

	[HideInInspector]
	public Text WP_specialO;

	[HideInInspector]
	public GameObject[] skillOBJO;

	[HideInInspector]
	public Text[] skillTextO;

	[HideInInspector]
	public GameObject WP_lineO_A;

	[HideInInspector]
	public GameObject WP_lineO_B;

	[HideInInspector]
	public GameObject WP_lineO_C;

	[HideInInspector]
	public GameObject WP_lineO_D;

	[HideInInspector]
	public GameObject WP_lineO_E;

	[HideInInspector]
	public GameObject WP_lineO_F;

	[HideInInspector]
	public GameObject[] WP_baoshiOBJO;

	[HideInInspector]
	public Text[] WP_baoshiO;

	[HideInInspector]
	public Image[] pic_aocaoO;

	[HideInInspector]
	public Image[] pic_baoshiO;

	[HideInInspector]
	public GameObject priceO;

	[HideInInspector]
	public Text WP_priceO;

	[HideInInspector]
	public CanvasGroup WeaponCavNew;

	[HideInInspector]
	public Text WP_titleN;

	[HideInInspector]
	public Text WP_typeN;

	[HideInInspector]
	public Text WP_levelN;

	[HideInInspector]
	public Text WP_mainN;

	[HideInInspector]
	public Text WP_specialN;

	[HideInInspector]
	public GameObject[] skillOBJN;

	[HideInInspector]
	public Text[] skillTextN;

	[HideInInspector]
	public GameObject WP_lineN_A;

	[HideInInspector]
	public GameObject WP_lineN_B;

	[HideInInspector]
	public GameObject WP_lineN_C;

	[HideInInspector]
	public GameObject WP_lineN_D;

	[HideInInspector]
	public GameObject WP_lineN_E;

	[HideInInspector]
	public GameObject WP_lineN_F;

	[HideInInspector]
	public GameObject[] WP_baoshiOBJN;

	[HideInInspector]
	public Text[] WP_baoshiN;

	[HideInInspector]
	public Image[] pic_aocaoN;

	[HideInInspector]
	public Image[] pic_baoshiN;

	[HideInInspector]
	public GameObject priceN;

	[HideInInspector]
	public Text WP_priceN;

	protected override void Awake()
	{
		base.Awake();
		titleText = GetControl<Text>("WeaponTitleText");
		confirmButton = GetControl<Button>("Cover");
		confirmButton.onClick.AddListener(delegate
		{
			Singleton<UIManager>.Instance.HidePanel<WeaponPanel>();
		});
		titleText.text = LOC.MM.GetMain("weapon_panel_title");
		BindOldTip();
		BindNewTip();
	}

	private void BindOldTip()
	{
		WeaponCavOld = base.transform.Find("Content/TipGroup/WeaponTipOld").GetComponent<CanvasGroup>();
		WP_titleO = WeaponCavOld.transform.Find("title").GetComponent<Text>();
		WP_typeO = WeaponCavOld.transform.Find("zhiye/type").GetComponent<Text>();
		WP_levelO = WeaponCavOld.transform.Find("zhiye/level").GetComponent<Text>();
		WP_mainO = WeaponCavOld.transform.Find("main").GetComponent<Text>();
		WP_specialO = WeaponCavOld.transform.Find("special").GetComponent<Text>();
		WP_lineO_A = WeaponCavOld.transform.Find("lineA").gameObject;
		WP_lineO_B = WeaponCavOld.transform.Find("lineB").gameObject;
		WP_lineO_C = WeaponCavOld.transform.Find("lineC").gameObject;
		WP_lineO_D = WeaponCavOld.transform.Find("lineD").gameObject;
		WP_lineO_E = WeaponCavOld.transform.Find("lineE").gameObject;
		WP_lineO_F = WeaponCavOld.transform.Find("lineF").gameObject;
		skillOBJO = new GameObject[6];
		skillOBJO[0] = WeaponCavOld.transform.Find("skillA").gameObject;
		skillOBJO[1] = WeaponCavOld.transform.Find("skillB").gameObject;
		skillOBJO[2] = WeaponCavOld.transform.Find("skillC").gameObject;
		skillOBJO[3] = WeaponCavOld.transform.Find("skillD").gameObject;
		skillOBJO[4] = WeaponCavOld.transform.Find("skillE").gameObject;
		skillOBJO[5] = WeaponCavOld.transform.Find("skillF").gameObject;
		skillTextO = new Text[6];
		skillTextO[0] = skillOBJO[0].transform.Find("Text").GetComponent<Text>();
		skillTextO[1] = skillOBJO[1].transform.Find("Text").GetComponent<Text>();
		skillTextO[2] = skillOBJO[2].transform.Find("Text").GetComponent<Text>();
		skillTextO[3] = skillOBJO[3].transform.Find("Text").GetComponent<Text>();
		skillTextO[4] = skillOBJO[4].transform.Find("Text").GetComponent<Text>();
		skillTextO[5] = skillOBJO[5].transform.Find("Text").GetComponent<Text>();
		WP_baoshiOBJO = new GameObject[6];
		WP_baoshiOBJO[0] = WeaponCavOld.transform.Find("AoCaoA").gameObject;
		WP_baoshiOBJO[1] = WeaponCavOld.transform.Find("AoCaoB").gameObject;
		WP_baoshiOBJO[2] = WeaponCavOld.transform.Find("AoCaoC").gameObject;
		WP_baoshiOBJO[3] = WeaponCavOld.transform.Find("AoCaoD").gameObject;
		WP_baoshiOBJO[4] = WeaponCavOld.transform.Find("AoCaoE").gameObject;
		WP_baoshiOBJO[5] = WeaponCavOld.transform.Find("AoCaoF").gameObject;
		WP_baoshiO = new Text[6];
		WP_baoshiO[0] = WP_baoshiOBJO[0].transform.Find("Text").GetComponent<Text>();
		WP_baoshiO[1] = WP_baoshiOBJO[1].transform.Find("Text").GetComponent<Text>();
		WP_baoshiO[2] = WP_baoshiOBJO[2].transform.Find("Text").GetComponent<Text>();
		WP_baoshiO[3] = WP_baoshiOBJO[3].transform.Find("Text").GetComponent<Text>();
		WP_baoshiO[4] = WP_baoshiOBJO[4].transform.Find("Text").GetComponent<Text>();
		WP_baoshiO[5] = WP_baoshiOBJO[5].transform.Find("Text").GetComponent<Text>();
		pic_aocaoO = new Image[6];
		pic_aocaoO[0] = WP_baoshiOBJO[0].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoO[1] = WP_baoshiOBJO[1].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoO[2] = WP_baoshiOBJO[2].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoO[3] = WP_baoshiOBJO[3].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoO[4] = WP_baoshiOBJO[4].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoO[5] = WP_baoshiOBJO[5].transform.Find("aocao").GetComponent<Image>();
		pic_baoshiO = new Image[6];
		pic_baoshiO[0] = WP_baoshiOBJO[0].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiO[1] = WP_baoshiOBJO[1].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiO[2] = WP_baoshiOBJO[2].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiO[3] = WP_baoshiOBJO[3].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiO[4] = WP_baoshiOBJO[4].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiO[5] = WP_baoshiOBJO[5].transform.Find("aocao/baoshi").GetComponent<Image>();
		priceO = WeaponCavOld.transform.Find("price").gameObject;
		WP_priceO = WeaponCavOld.transform.Find("price/price").GetComponent<Text>();
	}

	private void BindNewTip()
	{
		WeaponCavNew = base.transform.Find("Content/TipGroup/WeaponTipNew").GetComponent<CanvasGroup>();
		WP_titleN = WeaponCavNew.transform.Find("title").GetComponent<Text>();
		WP_typeN = WeaponCavNew.transform.Find("zhiye/type").GetComponent<Text>();
		WP_levelN = WeaponCavNew.transform.Find("zhiye/level").GetComponent<Text>();
		WP_mainN = WeaponCavNew.transform.Find("main").GetComponent<Text>();
		WP_specialN = WeaponCavNew.transform.Find("special").GetComponent<Text>();
		WP_lineN_A = WeaponCavNew.transform.Find("lineA").gameObject;
		WP_lineN_B = WeaponCavNew.transform.Find("lineB").gameObject;
		WP_lineN_C = WeaponCavNew.transform.Find("lineC").gameObject;
		WP_lineN_D = WeaponCavNew.transform.Find("lineD").gameObject;
		WP_lineN_E = WeaponCavNew.transform.Find("lineE").gameObject;
		WP_lineN_F = WeaponCavNew.transform.Find("lineF").gameObject;
		skillOBJN = new GameObject[6];
		skillOBJN[0] = WeaponCavNew.transform.Find("skillA").gameObject;
		skillOBJN[1] = WeaponCavNew.transform.Find("skillB").gameObject;
		skillOBJN[2] = WeaponCavNew.transform.Find("skillC").gameObject;
		skillOBJN[3] = WeaponCavNew.transform.Find("skillD").gameObject;
		skillOBJN[4] = WeaponCavNew.transform.Find("skillE").gameObject;
		skillOBJN[5] = WeaponCavNew.transform.Find("skillF").gameObject;
		skillTextN = new Text[6];
		skillTextN[0] = skillOBJN[0].transform.Find("Text").GetComponent<Text>();
		skillTextN[1] = skillOBJN[1].transform.Find("Text").GetComponent<Text>();
		skillTextN[2] = skillOBJN[2].transform.Find("Text").GetComponent<Text>();
		skillTextN[3] = skillOBJN[3].transform.Find("Text").GetComponent<Text>();
		skillTextN[4] = skillOBJN[4].transform.Find("Text").GetComponent<Text>();
		skillTextN[5] = skillOBJN[5].transform.Find("Text").GetComponent<Text>();
		WP_baoshiOBJN = new GameObject[6];
		WP_baoshiOBJN[0] = WeaponCavNew.transform.Find("AoCaoA").gameObject;
		WP_baoshiOBJN[1] = WeaponCavNew.transform.Find("AoCaoB").gameObject;
		WP_baoshiOBJN[2] = WeaponCavNew.transform.Find("AoCaoC").gameObject;
		WP_baoshiOBJN[3] = WeaponCavNew.transform.Find("AoCaoD").gameObject;
		WP_baoshiOBJN[4] = WeaponCavNew.transform.Find("AoCaoE").gameObject;
		WP_baoshiOBJN[5] = WeaponCavNew.transform.Find("AoCaoF").gameObject;
		WP_baoshiN = new Text[6];
		WP_baoshiN[0] = WP_baoshiOBJN[0].transform.Find("Text").GetComponent<Text>();
		WP_baoshiN[1] = WP_baoshiOBJN[1].transform.Find("Text").GetComponent<Text>();
		WP_baoshiN[2] = WP_baoshiOBJN[2].transform.Find("Text").GetComponent<Text>();
		WP_baoshiN[3] = WP_baoshiOBJN[3].transform.Find("Text").GetComponent<Text>();
		WP_baoshiN[4] = WP_baoshiOBJN[4].transform.Find("Text").GetComponent<Text>();
		WP_baoshiN[5] = WP_baoshiOBJN[5].transform.Find("Text").GetComponent<Text>();
		pic_aocaoN = new Image[6];
		pic_aocaoN[0] = WP_baoshiOBJN[0].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoN[1] = WP_baoshiOBJN[1].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoN[2] = WP_baoshiOBJN[2].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoN[3] = WP_baoshiOBJN[3].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoN[4] = WP_baoshiOBJN[4].transform.Find("aocao").GetComponent<Image>();
		pic_aocaoN[5] = WP_baoshiOBJN[5].transform.Find("aocao").GetComponent<Image>();
		pic_baoshiN = new Image[6];
		pic_baoshiN[0] = WP_baoshiOBJN[0].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiN[1] = WP_baoshiOBJN[1].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiN[2] = WP_baoshiOBJN[2].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiN[3] = WP_baoshiOBJN[3].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiN[4] = WP_baoshiOBJN[4].transform.Find("aocao/baoshi").GetComponent<Image>();
		pic_baoshiN[5] = WP_baoshiOBJN[5].transform.Find("aocao/baoshi").GetComponent<Image>();
		priceN = WeaponCavNew.transform.Find("price").gameObject;
		WP_priceN = WeaponCavNew.transform.Find("price/price").GetComponent<Text>();
	}

	public void Init(WeaponClass oldData, WeaponClass newData, long price)
	{
		FillWeaponTipOld(oldData);
		FillWeaponTipNew(oldData, newData);
		titleText.text = LOC.MM.GetMainFormat("weapon_panel_tip", price);
		RuntimeManager.PlayOneShot(audio_weapon_success, base.transform.position);
	}

	public override void OnShow()
	{
		base.OnShow();
		SetFirstSelected(confirmButton);
	}

	public override bool OnCancel()
	{
		Singleton<UIManager>.Instance.HidePanel<WeaponPanel>();
		return true;
	}

	private void FillWeaponTipOld(WeaponClass wp)
	{
		FillWeaponTip(wp, WP_titleO, WP_typeO, WP_levelO, WP_mainO, WP_specialO, skillOBJO, skillTextO, WP_lineO_A, WP_lineO_B, WP_lineO_C, WP_lineO_D, WP_lineO_E, WP_lineO_F, WP_baoshiOBJO, WP_baoshiO, pic_aocaoO, pic_baoshiO, priceO, WP_priceO);
	}

	private void FillWeaponTipNew(WeaponClass oldWp, WeaponClass newWp)
	{
		FillWeaponTip(newWp, WP_titleN, WP_typeN, WP_levelN, WP_mainN, WP_specialN, skillOBJN, skillTextN, WP_lineN_A, WP_lineN_B, WP_lineN_C, WP_lineN_D, WP_lineN_E, WP_lineN_F, WP_baoshiOBJN, WP_baoshiN, pic_aocaoN, pic_baoshiN, priceN, WP_priceN, oldWp);
	}

	private static void FillWeaponTip(WeaponClass wp, Text titleText, Text typeText, Text levelText, Text mainText, Text specialText, GameObject[] skillObjs, Text[] skillTexts, GameObject lineA, GameObject lineB, GameObject lineC, GameObject lineD, GameObject lineE, GameObject lineF, GameObject[] baoshiObjs, Text[] baoshiTexts, Image[] aocaoImages, Image[] baoshiImages, GameObject priceObj, Text priceText, WeaponClass oldWp = null)
	{
		if (wp == null || !SingletonMonoScope<PlayerManager>.HasInstance || !SingletonMonoScope<ItemManager>.HasInstance)
		{
			return;
		}
		PlayerManager instance = SingletonMonoScope<PlayerManager>.Instance;
		lineA.SetActive(value: true);
		lineB.SetActive(value: true);
		titleText.text = wp.GetTitle();
		switch (wp.PLtype)
		{
		case 0:
			typeText.text = ((instance.PLType == 0) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("MGC Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("MGC Item") + "</color>"));
			break;
		case 1:
			typeText.text = ((instance.PLType == 1) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("SQS Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("SQS Item") + "</color>"));
			break;
		case 2:
			typeText.text = ((instance.PLType == 2) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("ARC Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("ARC Item") + "</color>"));
			break;
		case 3:
			typeText.text = ((instance.PLType == 3) ? ("<color=#BAFDFF>" + LOC.MM.GetMain("DEAD Item") + "</color>") : ("<color=#FF1F1F>" + LOC.MM.GetMain("DEAD Item") + "</color>"));
			break;
		case 1000:
			typeText.text = string.Empty;
			break;
		}
		if (wp.Level > instance.Level)
		{
			levelText.text = string.Format("<color=#FF1F1F>{0} : {1}</color>", LOC.MM.GetMain("Level"), wp.Level);
		}
		else
		{
			levelText.text = string.Format("<color=#BAFDFF>{0} : {1}</color>", LOC.MM.GetMain("Level"), wp.Level);
		}
		if (oldWp != null)
		{
			mainText.text = wp.GetMain(oldWp);
		}
		else
		{
			mainText.text = wp.GetMain();
		}
		wp.TryGetSPCTemplate(0, out var _, out var mb);
		if (mb != null && mb.SPCtype > 0)
		{
			lineC.SetActive(value: true);
			specialText.gameObject.SetActive(value: true);
			specialText.text = wp.GetSpecial(0);
		}
		else
		{
			lineC.SetActive(value: false);
			specialText.gameObject.SetActive(value: false);
		}
		GameObject[] array = skillObjs;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: false);
			}
		}
		if (wp.WP_SkillCount > 0)
		{
			lineD.SetActive(value: true);
			int num = Mathf.Min(wp.WP_SkillCount, Mathf.Min(skillObjs.Length, skillTexts.Length));
			for (int j = 0; j < num; j++)
			{
				if ((bool)skillObjs[j])
				{
					skillObjs[j].SetActive(value: true);
				}
				if ((bool)skillTexts[j])
				{
					skillTexts[j].text = $"{LOC.MM.GetSkill(wp.WPSK[j].IndexName)} + {wp.WPSK[j].Number}";
				}
			}
		}
		else
		{
			lineD.SetActive(value: false);
		}
		if (wp.AocaoCount > 0)
		{
			lineE.SetActive(value: true);
			array = baoshiObjs;
			foreach (GameObject gameObject2 in array)
			{
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(value: false);
				}
			}
			int num2 = Mathf.Min(wp.AocaoCount, Mathf.Min(baoshiObjs.Length, Mathf.Min(baoshiTexts.Length, Mathf.Min(aocaoImages.Length, baoshiImages.Length))));
			for (int k = 0; k < num2; k++)
			{
				if ((bool)baoshiObjs[k])
				{
					baoshiObjs[k].SetActive(value: true);
				}
				if (wp.Aocao[k].HasBaoshi)
				{
					if ((bool)baoshiTexts[k])
					{
						baoshiTexts[k].text = wp.GetBaoshi(k);
					}
					if ((bool)aocaoImages[k])
					{
						aocaoImages[k].color = new Color32(0, 0, 0, 0);
					}
					if ((bool)baoshiImages[k])
					{
						baoshiImages[k].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
						baoshiImages[k].sprite = wp.Aocao[k].Icon;
					}
				}
				else
				{
					if ((bool)baoshiTexts[k])
					{
						baoshiTexts[k].text = LOC.MM.GetMain("Empty Slot");
					}
					if ((bool)aocaoImages[k])
					{
						aocaoImages[k].color = new Color32(197, 197, 197, byte.MaxValue);
					}
					if ((bool)baoshiImages[k])
					{
						baoshiImages[k].color = new Color32(0, 0, 0, 0);
					}
				}
			}
		}
		else
		{
			lineE.SetActive(value: false);
			array = baoshiObjs;
			foreach (GameObject gameObject3 in array)
			{
				if ((bool)gameObject3)
				{
					gameObject3.SetActive(value: false);
				}
			}
		}
		lineF.SetActive(value: true);
		priceObj.SetActive(value: true);
		priceText.text = wp.Price.ToString();
	}
}
