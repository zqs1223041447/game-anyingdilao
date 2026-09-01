using FMOD.Studio;
using FMODUnity;
using Inputs.Gamepad;
using UnityEngine;

public class SWW : MonoBehaviour
{
	[HideInInspector]
	public Camera cam;

	public LayerMask layerMask;

	public int AAAAA;

	public string soundA;

	public EventReference soundB;

	private void Start()
	{
		_ = AAAAA;
		_ = AAAAA;
		_ = AAAAA;
		_ = AAAAA;
		_ = Physics2D.Raycast(AimProvider.GetAimWorldPos(), Vector2.zero, float.PositiveInfinity, layerMask).collider != null;
	}

	public void FmodLean()
	{
		EventInstance eventInstance = RuntimeManager.CreateInstance(soundB);
		eventInstance.setParameterByName("Intesity", 10f);
		eventInstance.start();
	}

	public void CCC()
	{
	}
}
