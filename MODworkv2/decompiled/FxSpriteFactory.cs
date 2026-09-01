using System;
using UnityEngine;

public static class FxSpriteFactory
{
	private const int FrameCount = 8;

	private const int FrameSize = 64;

	private static Texture2D _soft;

	private static Texture2D _shard;

	private static Texture2D _fireSheet;

	private static Sprite[] _fireFrames;

	private static Material _particleMatTemplate;

	private static readonly Color FireTipColor = new Color(0.851f, 0.659f, 1f, 1f);

	private static readonly Color FireBodyColor = new Color(0.659f, 0.333f, 0.969f, 1f);

	private static readonly Color FireCoreColor = new Color(0.357f, 0.122f, 0.659f, 1f);

	private static readonly Color FireDarkColor = new Color(0.141f, 0.063f, 0.251f, 1f);

	public static Texture2D SoftTex
	{
		get
		{
			EnsureBase();
			return _soft;
		}
	}

	public static Texture2D ShardTex
	{
		get
		{
			EnsureBase();
			return _shard;
		}
	}

	public static Sprite FireFrame(int i)
	{
		EnsureFire();
		if (_fireFrames == null || _fireFrames.Length == 0)
		{
			return null;
		}
		return _fireFrames[Mathf.Clamp(i, 0, _fireFrames.Length - 1)];
	}

	private static void EnsureBase()
	{
		if (!(_soft != null))
		{
			_soft = MakeSoft();
			_shard = MakeShard();
		}
	}

	private static Texture2D NewTex(int w, int h)
	{
		return new Texture2D(w, h, TextureFormat.RGBA32, mipChain: false)
		{
			filterMode = FilterMode.Bilinear,
			wrapMode = TextureWrapMode.Clamp
		};
	}

	private static Texture2D MakeSoft()
	{
		Texture2D texture2D = NewTex(64, 64);
		Color[] array = new Color[4096];
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				float num = ((float)j - 31.5f) / 31.5f;
				float num2 = ((float)i - 31.5f) / 31.5f;
				float num3 = Mathf.Sqrt(num * num + num2 * num2);
				float num4 = Mathf.Pow(Mathf.Clamp01(1f - num3), 2.1f);
				array[i * 64 + j] = new Color(1f, 1f, 1f, Mathf.Clamp01(num4 * 1.12f));
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	private static Texture2D MakeShard()
	{
		Texture2D texture2D = NewTex(64, 64);
		Color[] array = new Color[4096];
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				float num = ((float)j - 31.5f) / 31.5f;
				float num2 = ((float)i - 31.5f) / 31.5f;
				float num3 = Mathf.Clamp01(1f - (Mathf.Abs(num) / 0.24f + Mathf.Abs(num2)));
				num3 = num3 * num3 * 1.1f;
				float num4 = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Sqrt(num * num + num2 * num2)), 3f) * 0.5f;
				array[i * 64 + j] = new Color(1f, 1f, 1f, Mathf.Clamp01(num3 + num4));
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	private static void EnsureFire()
	{
		if (_fireSheet != null)
		{
			return;
		}
		Color fireTipColor = FireTipColor;
		Color fireBodyColor = FireBodyColor;
		Color fireCoreColor = FireCoreColor;
		Color fireDarkColor = FireDarkColor;
		Texture2D texture2D = NewTex(512, 64);
		Color[] array = new Color[32768];
		Color[] array2 = new Color[4096];
		float[] array3 = new float[4096];
		System.Random random = new System.Random(20260828);
		for (int i = 0; i < 8; i++)
		{
			float num = (float)i / 7f;
			float num2 = 7f + num * 28f;
			float num3 = 1f - Mathf.Max(0f, num - 0.5f) / 0.5f;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = new Color(0f, 0f, 0f, 0f);
				array3[j] = 0f;
			}
			for (int k = 0; k < 22; k++)
			{
				float num4 = (float)(random.NextDouble() * Math.PI * 2.0);
				float num5 = num2 * (0.15f + (float)random.NextDouble() * 0.85f) * (0.35f + 0.65f * num);
				float num6 = (5f + num * 11f) * (0.4f + (float)random.NextDouble() * 0.9f);
				float t = (float)random.NextDouble();
				Color color = ((num < 0.3f) ? Color.Lerp(fireTipColor, fireBodyColor, t) : ((!(num < 0.7f)) ? Color.Lerp(fireCoreColor, fireDarkColor, t) : Color.Lerp(fireBodyColor, fireCoreColor, t)));
				float num7 = (0.55f + (float)random.NextDouble() * 0.45f) * num3;
				float num8 = 31.5f + (float)Math.Cos(num4) * num5;
				float num9 = 31.5f + (float)Math.Sin(num4) * num5;
				int num10 = Mathf.Max(0, (int)(num8 - num6));
				int num11 = Mathf.Min(63, (int)(num8 + num6));
				int num12 = Mathf.Max(0, (int)(num9 - num6));
				int num13 = Mathf.Min(63, (int)(num9 + num6));
				for (int l = num12; l <= num13; l++)
				{
					for (int m = num10; m <= num11; m++)
					{
						float num14 = ((float)m - num8) / num6;
						float num15 = ((float)l - num9) / num6;
						float num16 = Mathf.Sqrt(num14 * num14 + num15 * num15);
						if (!(num16 >= 1f))
						{
							float num17 = Mathf.Pow(1f - num16, 1.7f) * num7;
							int num18 = l * 64 + m;
							float num19 = num17 + array3[num18] * (1f - num17);
							if (!(num19 <= 0.0001f))
							{
								array2[num18].r = (color.r * num17 + array2[num18].r * array3[num18] * (1f - num17)) / num19;
								array2[num18].g = (color.g * num17 + array2[num18].g * array3[num18] * (1f - num17)) / num19;
								array2[num18].b = (color.b * num17 + array2[num18].b * array3[num18] * (1f - num17)) / num19;
								array2[num18].a = num19;
								array3[num18] = num19;
							}
						}
					}
				}
			}
			float num20 = num2 * 0.75f;
			for (int n = 0; n < 64; n++)
			{
				for (int num21 = 0; num21 < 64; num21++)
				{
					float num22 = ((float)num21 - 31.5f) / num20;
					float num23 = ((float)n - 31.5f) / num20;
					float num24 = Mathf.Sqrt(num22 * num22 + num23 * num23);
					if (!(num24 >= 1f))
					{
						float num25 = Mathf.Pow(1f - num24, 1.6f) * num3 * 0.95f;
						Color color2 = Color.Lerp(fireTipColor, fireBodyColor, Mathf.Clamp01(num24 * 1.4f));
						int num26 = n * 64 + num21;
						float num27 = num25 + array3[num26] * (1f - num25);
						if (!(num27 <= 0.0001f))
						{
							array2[num26].r = (color2.r * num25 + array2[num26].r * array3[num26] * (1f - num25)) / num27;
							array2[num26].g = (color2.g * num25 + array2[num26].g * array3[num26] * (1f - num25)) / num27;
							array2[num26].b = (color2.b * num25 + array2[num26].b * array3[num26] * (1f - num25)) / num27;
							array2[num26].a = num27;
							array3[num26] = num27;
						}
					}
				}
			}
			for (int num28 = 0; num28 < 64; num28++)
			{
				for (int num29 = 0; num29 < 64; num29++)
				{
					array[num28 * 512 + i * 64 + num29] = array2[num28 * 64 + num29];
				}
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		_fireSheet = texture2D;
		_fireFrames = new Sprite[8];
		for (int num30 = 0; num30 < 8; num30++)
		{
			_fireFrames[num30] = Sprite.Create(texture2D, new Rect(num30 * 64, 0f, 64f, 64f), new Vector2(0.5f, 0.5f), 100f);
			try
			{
				_fireFrames[num30].name = "FxFireFrame" + num30;
			}
			catch
			{
			}
		}
	}

	public static Material MakeParticleMaterial(Texture2D tex, Color tint, Material srcHint)
	{
		try
		{
			if (_particleMatTemplate == null && srcHint != null)
			{
				_particleMatTemplate = srcHint;
			}
			Material material;
			if (_particleMatTemplate != null)
			{
				material = new Material(_particleMatTemplate);
				if (material.HasProperty("_MainTex") && tex != null)
				{
					material.mainTexture = tex;
				}
				if (material.HasProperty("_TintColor"))
				{
					material.SetColor("_TintColor", tint);
				}
				if (material.HasProperty("_Color"))
				{
					material.SetColor("_Color", tint);
				}
			}
			else
			{
				Shader shader = Shader.Find("Sprites/Default");
				if (shader == null)
				{
					return null;
				}
				material = new Material(shader);
				if (material.HasProperty("_MainTex") && tex != null)
				{
					material.mainTexture = tex;
				}
				material.color = tint;
			}
			return material;
		}
		catch
		{
			return null;
		}
	}

	public static void SpawnFlipbookBurst(Vector3 pos, Material srcHint)
	{
		try
		{
			EnsureFire();
			if (_fireSheet == null)
			{
				return;
			}
			GameObject gameObject = new GameObject("FxFireFlipbookBurst");
			gameObject.transform.position = pos;
			ParticleSystem particleSystem = gameObject.AddComponent<ParticleSystem>();
			ParticleSystem.MainModule main = particleSystem.main;
			main.loop = false;
			main.playOnAwake = true;
			main.duration = 0.62f;
			main.startLifetime = 0.62f;
			main.startSpeed = 0f;
			main.startSize = 1.7f;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.maxParticles = 2;
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.rateOverTime = 0f;
			emission.SetBursts(new ParticleSystem.Burst[1]
			{
				new ParticleSystem.Burst(0f, 1)
			});
			try
			{
				ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = particleSystem.textureSheetAnimation;
				textureSheetAnimation.enabled = true;
				textureSheetAnimation.mode = ParticleSystemAnimationMode.Sprites;
				for (int i = 0; i < _fireFrames.Length; i++)
				{
					textureSheetAnimation.AddSprite(_fireFrames[i]);
				}
				textureSheetAnimation.cycleCount = 1;
				textureSheetAnimation.frameOverTime = new ParticleSystem.MinMaxCurve(0f, new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 7f)));
			}
			catch
			{
			}
			ParticleSystemRenderer component = particleSystem.GetComponent<ParticleSystemRenderer>();
			if (component != null)
			{
				Material material = MakeParticleMaterial(_fireSheet, Color.white, srcHint);
				if (material != null)
				{
					component.material = material;
					component.sortingOrder = 5;
				}
			}
			particleSystem.Play();
			UnityEngine.Object.Destroy(gameObject, 1f);
		}
		catch
		{
		}
	}

	public static void SpawnIceBurst(Vector3 pos, Color coreColor, Color mainColor, Color deepColor, Material srcHint)
	{
		try
		{
			GameObject gameObject = new GameObject("FxIceBurst");
			gameObject.transform.position = pos;
			ParticleSystem particleSystem = gameObject.AddComponent<ParticleSystem>();
			ParticleSystem.MainModule main = particleSystem.main;
			main.loop = false;
			main.playOnAwake = true;
			main.duration = 0.9f;
			main.startLifetime = 0.5f;
			main.startSpeed = 0f;
			main.startSize = 0.34f;
			main.startColor = new ParticleSystem.MinMaxGradient(coreColor);
			main.gravityModifier = 0.25f;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.maxParticles = 32;
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.SetBursts(new ParticleSystem.Burst[1]
			{
				new ParticleSystem.Burst(0f, 26)
			});
			emission.rateOverTime = 0f;
			ParticleSystem.ShapeModule shape = particleSystem.shape;
			shape.enabled = true;
			shape.shapeType = ParticleSystemShapeType.Circle;
			shape.radius = 0.05f;
			shape.rotation = new Vector3(0f, 0f, -90f);
			try
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
				velocityOverLifetime.enabled = true;
				velocityOverLifetime.speedModifier = new ParticleSystem.MinMaxCurve(1.4f, 4.2f);
			}
			catch
			{
			}
			try
			{
				ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particleSystem.sizeOverLifetime;
				sizeOverLifetime.enabled = true;
				sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.35f, 0.85f), new Keyframe(1f, 0.15f)));
			}
			catch
			{
			}
			try
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
				colorOverLifetime.enabled = true;
				Gradient gradient = new Gradient();
				gradient.SetKeys(new GradientColorKey[2]
				{
					new GradientColorKey(coreColor, 0f),
					new GradientColorKey(deepColor, 1f)
				}, new GradientAlphaKey[3]
				{
					new GradientAlphaKey(0.95f, 0f),
					new GradientAlphaKey(0.6f, 0.4f),
					new GradientAlphaKey(0f, 1f)
				});
				colorOverLifetime.color = gradient;
			}
			catch
			{
			}
			ParticleSystemRenderer component = particleSystem.GetComponent<ParticleSystemRenderer>();
			if (component != null)
			{
				Material material = MakeParticleMaterial(SoftTex, Color.white, srcHint);
				if (material != null)
				{
					component.material = material;
					component.sortingOrder = 5;
				}
			}
			particleSystem.Play();
			GameObject gameObject2 = new GameObject("FxIceShards");
			gameObject2.transform.position = pos;
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: true);
			ParticleSystem particleSystem2 = gameObject2.AddComponent<ParticleSystem>();
			ParticleSystem.MainModule main2 = particleSystem2.main;
			main2.loop = false;
			main2.playOnAwake = true;
			main2.duration = 0.9f;
			main2.startLifetime = 0.65f;
			main2.startSpeed = 0f;
			main2.startSize = 0.3f;
			main2.startRotation = new ParticleSystem.MinMaxCurve(0f, (float)Math.PI * 2f);
			main2.gravityModifier = 0.45f;
			main2.simulationSpace = ParticleSystemSimulationSpace.World;
			main2.maxParticles = 14;
			ParticleSystem.EmissionModule emission2 = particleSystem2.emission;
			emission2.SetBursts(new ParticleSystem.Burst[1]
			{
				new ParticleSystem.Burst(0f, 10)
			});
			emission2.rateOverTime = 0f;
			try
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime2 = particleSystem2.velocityOverLifetime;
				velocityOverLifetime2.enabled = true;
				velocityOverLifetime2.speedModifier = new ParticleSystem.MinMaxCurve(0.9f, 2.6f);
			}
			catch
			{
			}
			try
			{
				ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = particleSystem2.rotationOverLifetime;
				rotationOverLifetime.enabled = true;
				rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-4.712389f, 4.712389f);
			}
			catch
			{
			}
			try
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem2.colorOverLifetime;
				colorOverLifetime2.enabled = true;
				Gradient gradient2 = new Gradient();
				gradient2.SetKeys(new GradientColorKey[2]
				{
					new GradientColorKey(mainColor, 0f),
					new GradientColorKey(deepColor, 1f)
				}, new GradientAlphaKey[2]
				{
					new GradientAlphaKey(0.9f, 0f),
					new GradientAlphaKey(0f, 1f)
				});
				colorOverLifetime2.color = gradient2;
			}
			catch
			{
			}
			ParticleSystemRenderer component2 = particleSystem2.GetComponent<ParticleSystemRenderer>();
			if (component2 != null)
			{
				Material material2 = MakeParticleMaterial(ShardTex, Color.white, srcHint);
				if (material2 != null)
				{
					component2.material = material2;
					component2.sortingOrder = 5;
				}
			}
			particleSystem2.Play();
			GameObject gameObject3 = new GameObject("FxIceFlash");
			gameObject3.transform.position = pos;
			gameObject3.transform.SetParent(gameObject.transform, worldPositionStays: true);
			ParticleSystem particleSystem3 = gameObject3.AddComponent<ParticleSystem>();
			ParticleSystem.MainModule main3 = particleSystem3.main;
			main3.loop = false;
			main3.playOnAwake = true;
			main3.duration = 0.28f;
			main3.startLifetime = 0.28f;
			main3.startSpeed = 0f;
			main3.startSize = 0.9f;
			main3.startColor = new ParticleSystem.MinMaxGradient(coreColor);
			main3.simulationSpace = ParticleSystemSimulationSpace.World;
			main3.maxParticles = 1;
			ParticleSystem.EmissionModule emission3 = particleSystem3.emission;
			emission3.SetBursts(new ParticleSystem.Burst[1]
			{
				new ParticleSystem.Burst(0f, 1)
			});
			emission3.rateOverTime = 0f;
			try
			{
				ParticleSystem.SizeOverLifetimeModule sizeOverLifetime2 = particleSystem3.sizeOverLifetime;
				sizeOverLifetime2.enabled = true;
				sizeOverLifetime2.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 0.35f), new Keyframe(1f, 1.6f)));
			}
			catch
			{
			}
			try
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime3 = particleSystem3.colorOverLifetime;
				colorOverLifetime3.enabled = true;
				Gradient gradient3 = new Gradient();
				gradient3.SetKeys(new GradientColorKey[1]
				{
					new GradientColorKey(coreColor, 0f)
				}, new GradientAlphaKey[2]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(0f, 1f)
				});
				colorOverLifetime3.color = gradient3;
			}
			catch
			{
			}
			ParticleSystemRenderer component3 = particleSystem3.GetComponent<ParticleSystemRenderer>();
			if (component3 != null)
			{
				Material material3 = MakeParticleMaterial(SoftTex, Color.white, srcHint);
				if (material3 != null)
				{
					component3.material = material3;
					component3.sortingOrder = 6;
				}
			}
			particleSystem3.Play();
			UnityEngine.Object.Destroy(gameObject, 1.5f);
		}
		catch
		{
		}
	}
}
