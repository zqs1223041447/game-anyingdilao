using Lean.Pool;
using UnityEngine;

public class SK_Universe_Aixs : MonoBehaviour
{
	public bool CanZhuan;

	public float speed;

	public SK_Universe father;

	public SK_Universe_Ball ball;

	public Transform point;

	private bool _released;

	private void Awake()
	{
		point = base.transform.Find("point");
	}

	private void OnEnable()
	{
		CanZhuan = false;
		ball = null;
		father = null;
		_released = false;
	}

	private void Update()
	{
		if (CanZhuan)
		{
			base.transform.Rotate(0f, 0f, speed * Time.deltaTime);
		}
		if (!_released && (bool)ball && ball.StartFollow)
		{
			ReleaseBecauseBallLeft();
		}
	}

	public void BindBall(SK_Universe_Ball newBall, SK_Universe newFather)
	{
		ball = newBall;
		father = newFather;
		CanZhuan = true;
		_released = false;
	}

	private void ReleaseBecauseBallLeft()
	{
		_released = true;
		ball = null;
		CanZhuan = false;
		if ((bool)father)
		{
			father.NotifyAxisReleased(this);
		}
		LeanPool.Despawn(this);
	}

	public void Stop()
	{
		ball = null;
		CanZhuan = false;
		if ((bool)father)
		{
			father.NotifyAxisReleased(this);
		}
		LeanPool.Despawn(this);
	}
}
