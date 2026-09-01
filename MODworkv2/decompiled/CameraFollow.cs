using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform trans;

	public float moveSpeed = 1f;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
	}

	private void LateUpdate()
	{
		base.transform.position = Vector2.Lerp(base.transform.position, trans.transform.position, moveSpeed);
	}
}
