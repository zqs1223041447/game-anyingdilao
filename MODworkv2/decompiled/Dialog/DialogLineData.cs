using System;

namespace Dialog;

[Serializable]
public class DialogLineData
{
	public string Content;

	public string ContentKey;

	public object[] FormatArgs;

	public Action OnLineFinished;

	public string EventKey;

	public bool TriggerOnce = true;
}
