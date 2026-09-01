using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
	[SerializeField]
	private Text versionText;

	private void Awake()
	{
		versionText = GetComponent<Text>();
		versionText.text = "Version: " + Application.version;
	}
}
