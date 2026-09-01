using System;
using System.Collections.Generic;

namespace Dialog;

[Serializable]
public class DialogCandidateData
{
	public string DialogId;

	public int Priority;

	public bool IsNewContent = true;

	public bool RemoveAfterComplete = true;

	public List<DialogConditionData> Conditions = new List<DialogConditionData>();
}
