using System;
using System.Collections.Generic;

namespace Data.SaveData;

[Serializable]
public class DialogSaveData
{
	public List<string> TriggeredEvents = new List<string>();

	public List<string> CompletedDialogs = new List<string>();

	public static DialogSaveData CreateDefault()
	{
		return new DialogSaveData
		{
			TriggeredEvents = new List<string>(),
			CompletedDialogs = new List<string>()
		};
	}
}
