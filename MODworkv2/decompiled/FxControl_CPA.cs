using Lean.Pool;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FxControl_CPA : MonoBehaviour
{
	private static readonly int dislove = Shader.PropertyToID("_Dislove");

	private static readonly int flip = Shader.PropertyToID("_Flip");

	private static readonly int mainMix = Shader.PropertyToID("_MainMix");

	private static readonly int mainHue = Shader.PropertyToID("_MainHue");

	private static readonly int mainSat = Shader.PropertyToID("_MainSat");

	private static readonly int mainColor = Shader.PropertyToID("_MainColor");

	private static readonly int disloveColor = Shader.PropertyToID("_DisloveColor");

	private static readonly int alphaColor = Shader.PropertyToID("_AlphaColor");

	private static readonly int mainAlpha = Shader.PropertyToID("_MainAlpha");

	private static readonly int fxColor = Shader.PropertyToID("_FXColor");

	private static readonly int fxSat = Shader.PropertyToID("_FXSat");

	public Light2D lit;

	public AnimationCurve DeadCurve;

	public CompColorData[] CLDT;

	public int Flip;

	public Vector4 MainMix;

	public int MainHue;

	public float MainSat;

	[ColorUsage(true, true)]
	public Color MainColor;

	[ColorUsage(true, true)]
	public Color DisloveColor;

	[ColorUsage(true, true)]
	public Color AlphaColor;

	public float DieFX_TimeDelay;

	private bool isDead;

	private float Deadtime;

	private byte SDalpha;

	private float JStime;

	[HideInInspector]
	public Companion cp;

	[HideInInspector]
	public SpriteRenderer SD;

	private void Awake()
	{
		cp = GetComponent<Companion>();
		SD = base.transform.Find("shadow").gameObject.GetComponent<SpriteRenderer>();
		isDead = false;
	}

	private void OnEnable()
	{
		SetStart();
	}

	private void Update()
	{
		if (!isDead)
		{
			if (cp.IsDead)
			{
				DieFX(cp.DieType);
			}
		}
		else if (cp.DieType == 2)
		{
			Deadtime += Time.deltaTime;
			cp.mpb?.SetFloat(dislove, DeadCurve.Evaluate(Deadtime));
			if ((bool)lit)
			{
				lit.intensity = DeadCurve.Evaluate(Deadtime);
			}
			cp.SpineRender.SetPropertyBlock(cp.mpb);
			JStime += Time.deltaTime;
			if (SDalpha != 0 && JStime > 0.15f)
			{
				SDalpha -= 15;
				SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, SDalpha);
				JStime = 0f;
			}
		}
	}

	public void SetStart()
	{
		JStime = 0f;
		SDalpha = 150;
		SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, SDalpha);
		Deadtime = 0f;
		isDead = false;
		if ((bool)cp.SpineRender)
		{
			if (cp.mpb == null)
			{
				cp.mpb = new MaterialPropertyBlock();
			}
			cp.mpb.SetFloat(dislove, 1f);
			ApplyColorData(applyPropertyBlock: false);
			cp.mpb.SetFloat(mainAlpha, 1f);
			cp.mpb.SetFloat(fxSat, 1f);
			cp.mpb.SetColor(fxColor, Color.white);
			cp.SpineRender.SetPropertyBlock(cp.mpb);
		}
	}

	public void ApplyColorData()
	{
		ApplyColorData(applyPropertyBlock: true);
	}

	private void ApplyColorData(bool applyPropertyBlock)
	{
		if ((bool)cp && (bool)cp.SpineRender)
		{
			if (cp.mpb == null)
			{
				cp.mpb = new MaterialPropertyBlock();
			}
			CompColorData colorData = GetColorData();
			if (colorData != null)
			{
				cp.mpb.SetInt(flip, colorData.Flip);
				cp.mpb.SetVector(mainMix, colorData.MainMix);
				cp.mpb.SetInt(mainHue, colorData.MainHue);
				cp.mpb.SetFloat(mainSat, colorData.MainSat);
				cp.mpb.SetColor(mainColor, colorData.MainColor);
				cp.mpb.SetColor(disloveColor, colorData.DisloveColor);
				cp.mpb.SetColor(alphaColor, colorData.AlphaColor);
			}
			else
			{
				cp.mpb.SetInt(flip, Flip);
				cp.mpb.SetVector(mainMix, MainMix);
				cp.mpb.SetInt(mainHue, MainHue);
				cp.mpb.SetFloat(mainSat, MainSat);
				cp.mpb.SetColor(mainColor, MainColor);
				cp.mpb.SetColor(disloveColor, DisloveColor);
				cp.mpb.SetColor(alphaColor, AlphaColor);
			}
			if (applyPropertyBlock)
			{
				cp.SpineRender.SetPropertyBlock(cp.mpb);
			}
		}
	}

	private CompColorData GetColorData()
	{
		if (CLDT == null || CLDT.Length == 0)
		{
			return null;
		}
		int num = (cp ? cp.BStype : 0);
		if (num < 0 || num >= CLDT.Length)
		{
			num = 0;
		}
		return CLDT[num];
	}

	public void DieFX(int A)
	{
		switch (A)
		{
		case 1:
			switch (cp.DiePos)
			{
			case 0:
				LeanPool.Spawn(cp.Die_OBJ, cp.transform.position, Quaternion.identity);
				break;
			case 1:
				LeanPool.Spawn(cp.Die_OBJ, cp.body.transform.position, Quaternion.identity);
				break;
			}
			break;
		case 2:
			this.wait(DieFX_TimeDelay, delegate
			{
				LeanPool.Spawn(cp.Die_OBJ, cp.body.transform.position, Quaternion.identity, cp.body.transform);
			});
			break;
		}
		cp.DeleteSelf();
		LeanPool.Despawn(cp.gameObject);
		isDead = true;
	}
}
