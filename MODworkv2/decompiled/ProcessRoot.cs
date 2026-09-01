using UnityEngine;

public class ProcessRoot : MonoBehaviour
{
	public ProcessScope Scope { get; private set; }

	public void Init(ProcessScope scope)
	{
		Scope = scope;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
