using UnityEngine;

public class SK_Snake : MonoBehaviour
{
	public float speed;

	public Transform Tar;

	private void Update()
	{
		Vector3 vector = Tar.position - base.transform.position;
		Vector3 upwards = Quaternion.Euler(0f, 0f, 90f) * vector;
		base.transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
		if ((base.transform.position - Tar.position).magnitude > 0.3f)
		{
			base.transform.Translate(Vector3.right * (speed * Time.deltaTime));
		}
	}
}
