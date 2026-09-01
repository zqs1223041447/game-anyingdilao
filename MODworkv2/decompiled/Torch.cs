using SK.Framework;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Torch : MonoBehaviour
{
	private Light2D lit;

	private void Awake()
	{
		lit = base.transform.Find("main/light").GetComponent<Light2D>();
	}

	private void Start()
	{
		Messenger.ADD<float>("Light2d", liang);
		Messenger.ADD<float>(1, AAA);
		Messenger.ADD<int, float, SkillData>(1, BBB);
		Messenger.DEL<int, float, SkillData>(1, BBB);
		Messenger.Send("XXX", 1);
	}

	private void liang(float a)
	{
		lit.intensity = a;
	}

	private void AAA(float a)
	{
		lit.intensity = a;
	}

	private void BBB(int a, float b, SkillData obj)
	{
		lit.intensity = a;
	}
}
