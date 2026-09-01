using UnityEngine;

public class SK_FlyBall_Lerp : MonoBehaviour
{
	public float speed;

	public float lerpSpeed;

	private float MoveSpeedTmp;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		MoveSpeedTmp = speed;
	}

	private void Update()
	{
		base.transform.position += base.transform.right * MoveSpeedTmp * Time.deltaTime;
		MoveSpeedTmp = Mathf.Lerp(MoveSpeedTmp, 0f, Time.deltaTime * lerpSpeed);
	}
}
