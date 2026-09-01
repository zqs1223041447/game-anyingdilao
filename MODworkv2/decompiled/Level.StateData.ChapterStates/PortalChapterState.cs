using System.Collections.Generic;
using Core.Teleport;

namespace Level.StateData.ChapterStates;

public class PortalChapterState
{
	private readonly Dictionary<PortalType, PortalData> _portals = new Dictionary<PortalType, PortalData>();

	public bool HasPortal(PortalType type)
	{
		return _portals.ContainsKey(type);
	}

	public PortalData GetPortal(PortalType type)
	{
		return _portals[type];
	}

	public void SetPortal(PortalType type, PortalData data)
	{
		_portals[type] = data;
	}

	public void MarkConsumed(PortalType type)
	{
		if (_portals.TryGetValue(type, out var value))
		{
			value.IsConsumed = true;
			_portals[type] = value;
		}
	}

	public void RemovePortal(PortalType type)
	{
		_portals.Remove(type);
	}

	public void Clear()
	{
		_portals.Clear();
	}
}
