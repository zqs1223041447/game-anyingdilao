using FinkFramework.Runtime.Singleton;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class CustomScrollView : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerUpHandler
{
	[Header("References")]
	public RectTransform track;

	public RectTransform handle;

	[Header("Content")]
	public RectTransform content;

	[Header("View")]
	public RectTransform viewport;

	[Header("Options")]
	public bool reverse;

	[Header("Mouse Wheel")]
	public bool enableMouseWheel = true;

	public float wheelSensitivity = 0.05f;

	[Header("Value")]
	[Range(0f, 1f)]
	public float value;

	public FloatEvent onValueChanged = new FloatEvent();

	[Header("Auto Hide")]
	public bool autoHideScrollbar = true;

	[Header("Gamepad Auto Focus Scroll")]
	public bool enableGamepadAutoScrollToSelection = true;

	public float selectionPadding = 12f;

	public bool smoothAutoScroll;

	public float smoothScrollSpeed = 12f;

	private float minY;

	private float maxY;

	private float contentMinY;

	private float contentMaxY;

	private float _lastContentHeight = -1f;

	private float _lastViewportHeight = -1f;

	private RectTransform _lastSelectedRect;

	private float _targetContentY;

	private bool _isDraggingScrollbar;

	private void Start()
	{
		Canvas.ForceUpdateCanvases();
		CalculateRange();
		CalculateContentRange();
		SetValue(value, silent: true);
	}

	private void Update()
	{
		CheckAndRefresh();
		HandleMouseWheel();
		HandleGamepadAutoScroll();
		if (smoothAutoScroll)
		{
			UpdateSmoothScroll();
		}
	}

	private void HandleMouseWheel()
	{
		if (enableMouseWheel && IsMouseInsideViewport())
		{
			float y = Input.mouseScrollDelta.y;
			if (!Mathf.Approximately(y, 0f))
			{
				SetValue(value - y * wheelSensitivity);
			}
		}
	}

	private void HandleGamepadAutoScroll()
	{
		if (SingletonMonoGlobal<CurrentInputManager>.HasInstance && !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
		{
			_lastSelectedRect = null;
		}
		else
		{
			if (!enableGamepadAutoScrollToSelection || !Application.isPlaying || !content || !viewport || !EventSystem.current || !SingletonMonoGlobal<CurrentInputManager>.HasInstance || !SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent())
			{
				return;
			}
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if ((bool)currentSelectedGameObject)
			{
				RectTransform component = currentSelectedGameObject.GetComponent<RectTransform>();
				if ((bool)component && component.IsChildOf(content) && component != _lastSelectedRect)
				{
					_lastSelectedRect = component;
					ScrollToMakeVisible(component, selectionPadding);
				}
			}
		}
	}

	private void UpdateSmoothScroll()
	{
		if ((bool)content)
		{
			Vector2 anchoredPosition = content.anchoredPosition;
			if (!(Mathf.Abs(anchoredPosition.y - _targetContentY) < 0.01f))
			{
				anchoredPosition.y = Mathf.Lerp(anchoredPosition.y, _targetContentY, smoothScrollSpeed * Time.unscaledDeltaTime);
				anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, contentMinY, contentMaxY);
				content.anchoredPosition = anchoredPosition;
				SyncValueFromContentPosition(silent: true);
			}
		}
	}

	private void CheckAndRefresh()
	{
		if ((bool)content && (bool)viewport)
		{
			Canvas.ForceUpdateCanvases();
			float height = content.rect.height;
			float height2 = viewport.rect.height;
			if (!Mathf.Approximately(height, _lastContentHeight) || !Mathf.Approximately(height2, _lastViewportHeight))
			{
				_lastContentHeight = height;
				_lastViewportHeight = height2;
				CalculateRange();
				CalculateContentRange();
				SetValue(value, silent: true);
				_targetContentY = content.anchoredPosition.y;
			}
			if ((bool)_lastSelectedRect && !_lastSelectedRect.gameObject.activeInHierarchy)
			{
				_lastSelectedRect = null;
			}
		}
	}

	private void OnRectTransformDimensionsChange()
	{
		CheckAndRefresh();
	}

	private float GetVisualValue(float v)
	{
		if (!reverse)
		{
			return v;
		}
		return 1f - v;
	}

	private void CalculateContentRange()
	{
		if ((bool)content && (bool)viewport)
		{
			float height = viewport.rect.height;
			float num = content.rect.height - height;
			bool flag = num > 0f;
			if (autoHideScrollbar && (bool)track)
			{
				track.gameObject.SetActive(flag);
			}
			if (!flag)
			{
				value = 0f;
				contentMinY = 0f;
				contentMaxY = 0f;
			}
			else
			{
				contentMinY = 0f;
				contentMaxY = num;
			}
		}
	}

	private void CalculateRange()
	{
		if ((bool)track && (bool)handle)
		{
			float height = track.rect.height;
			float height2 = handle.rect.height;
			minY = (0f - height) * 0.5f + height2 * 0.5f;
			maxY = height * 0.5f - height2 * 0.5f;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (IsPointerInsideTrack(eventData))
		{
			_isDraggingScrollbar = true;
			UpdateValue(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (_isDraggingScrollbar)
		{
			UpdateValue(eventData);
		}
	}

	public void AddValue(float delta, bool silent = false)
	{
		SetValue(value + delta, silent);
	}

	private void UpdateValue(PointerEventData eventData)
	{
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(track, eventData.position, eventData.pressEventCamera, out var localPoint))
		{
			float num = Mathf.Clamp(localPoint.y, minY, maxY);
			float num2 = Mathf.InverseLerp(minY, maxY, num);
			value = (reverse ? (1f - num2) : num2);
			SetHandlePosition(value);
			SetContentPosition(value);
			_targetContentY = (content ? content.anchoredPosition.y : 0f);
			onValueChanged?.Invoke(value);
		}
	}

	public void SetValue(float v, bool silent = false)
	{
		value = Mathf.Clamp01(v);
		SetHandlePosition(value);
		SetContentPosition(value);
		_targetContentY = (content ? content.anchoredPosition.y : 0f);
		if (!silent)
		{
			onValueChanged?.Invoke(value);
		}
	}

	private void SetContentPosition(float v)
	{
		if ((bool)content)
		{
			float y = Mathf.Lerp(contentMinY, contentMaxY, v);
			Vector2 anchoredPosition = content.anchoredPosition;
			anchoredPosition.y = y;
			content.anchoredPosition = anchoredPosition;
		}
	}

	private void SetHandlePosition(float v)
	{
		if ((bool)handle)
		{
			float visualValue = GetVisualValue(v);
			float y = Mathf.Lerp(minY, maxY, visualValue);
			Vector2 anchoredPosition = handle.anchoredPosition;
			anchoredPosition.y = y;
			handle.anchoredPosition = anchoredPosition;
		}
	}

	private void EnsureContentRectTransform()
	{
		if ((bool)content)
		{
			content.anchorMin = new Vector2(0f, 1f);
			content.anchorMax = new Vector2(1f, 1f);
			content.pivot = new Vector2(0.5f, 1f);
		}
	}

	private void OnValidate()
	{
		EnsureContentRectTransform();
		CalculateRange();
		CalculateContentRange();
		SetValue(value, silent: true);
		if ((bool)content)
		{
			_targetContentY = content.anchoredPosition.y;
		}
	}

	private bool IsMouseInsideViewport()
	{
		if (!viewport)
		{
			return false;
		}
		return RectTransformUtility.RectangleContainsScreenPoint(viewport, Input.mousePosition, null);
	}

	private bool IsPointerInsideTrack(PointerEventData eventData)
	{
		if (!track)
		{
			return false;
		}
		return RectTransformUtility.RectangleContainsScreenPoint(track, eventData.position, eventData.pressEventCamera);
	}

	private void ScrollToMakeVisible(RectTransform target, float padding)
	{
		if (!target || !content || !viewport || contentMaxY <= contentMinY)
		{
			return;
		}
		GetRectInViewportLocal(target, out var top, out var bottom);
		float num = viewport.rect.yMax - padding;
		float num2 = viewport.rect.yMin + padding;
		float y = content.anchoredPosition.y;
		if (top > num)
		{
			float num3 = top - num;
			y -= num3;
		}
		else
		{
			if (!(bottom < num2))
			{
				return;
			}
			float num4 = num2 - bottom;
			y += num4;
		}
		y = Mathf.Clamp(y, contentMinY, contentMaxY);
		if (smoothAutoScroll)
		{
			_targetContentY = y;
			return;
		}
		Vector2 anchoredPosition = content.anchoredPosition;
		anchoredPosition.y = y;
		content.anchoredPosition = anchoredPosition;
		_targetContentY = y;
		SyncValueFromContentPosition(silent: false);
	}

	private void SyncValueFromContentPosition(bool silent)
	{
		if (!content)
		{
			return;
		}
		if (Mathf.Approximately(contentMaxY, contentMinY))
		{
			value = 0f;
			SetHandlePosition(value);
			if (!silent)
			{
				onValueChanged?.Invoke(value);
			}
			return;
		}
		float num = Mathf.InverseLerp(contentMinY, contentMaxY, content.anchoredPosition.y);
		value = Mathf.Clamp01(num);
		SetHandlePosition(value);
		if (!silent)
		{
			onValueChanged?.Invoke(value);
		}
	}

	private void GetRectInViewportLocal(RectTransform target, out float top, out float bottom)
	{
		Vector3[] array = new Vector3[4];
		target.GetWorldCorners(array);
		Vector3 vector = viewport.InverseTransformPoint(array[0]);
		Vector3 vector2 = viewport.InverseTransformPoint(array[1]);
		Vector3 vector3 = viewport.InverseTransformPoint(array[2]);
		Vector3 vector4 = viewport.InverseTransformPoint(array[3]);
		top = Mathf.Max(vector2.y, vector3.y, vector.y, vector4.y);
		bottom = Mathf.Min(vector2.y, vector3.y, vector.y, vector4.y);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isDraggingScrollbar = false;
	}

	private void OnDisable()
	{
		_isDraggingScrollbar = false;
	}
}
