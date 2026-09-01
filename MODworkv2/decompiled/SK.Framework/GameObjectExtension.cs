using System;
using UnityEngine;

namespace SK.Framework;

public static class GameObjectExtension
{
	public static GameObject Activate(this GameObject self)
	{
		self.SetActive(value: true);
		return self;
	}

	public static GameObject Deactivate(this GameObject self)
	{
		self.SetActive(value: false);
		return self;
	}

	public static GameObject ActiveInvert(this GameObject self)
	{
		self.SetActive(!self.activeSelf);
		return self;
	}

	public static void ActiveReplace(this GameObject self, GameObject target, bool destroy)
	{
		if (destroy)
		{
			UnityEngine.Object.Destroy(self);
		}
		else
		{
			self.SetActive(value: false);
		}
		target.SetActive(value: true);
	}

	public static GameObject SetName(this GameObject self, string name)
	{
		self.name = name;
		return self;
	}

	public static GameObject SetLayer(this GameObject self, int layer)
	{
		self.layer = layer;
		return self;
	}

	public static GameObject SetLayer(this GameObject self, string layer)
	{
		self.layer = LayerMask.NameToLayer(layer);
		return self;
	}

	public static GameObject SetTag(this GameObject self, string tag)
	{
		self.tag = tag;
		return self;
	}

	public static GameObject RemoveComponent<T>(this GameObject self) where T : Component
	{
		T component = self.GetComponent<T>();
		if (null == component)
		{
			UnityEngine.Object.Destroy(component);
		}
		return self;
	}

	public static void Destroy(this GameObject self)
	{
		if (null != self)
		{
			UnityEngine.Object.Destroy(self);
		}
	}

	public static void Destroy(this GameObject self, float delay)
	{
		if (null != self)
		{
			UnityEngine.Object.Destroy(self, delay);
		}
	}

	public static T GetOrAddComponent<T>(this GameObject self) where T : Component
	{
		return self.GetComponent<T>() ?? self.AddComponent<T>();
	}

	public static bool IsActiveSelf<T>(this T self) where T : Component
	{
		return self.gameObject.activeSelf;
	}

	public static T Activate<T>(this T self) where T : Component
	{
		self.gameObject.SetActive(value: true);
		return self;
	}

	public static T Deactivate<T>(this T self) where T : Component
	{
		self.gameObject.SetActive(value: false);
		return self;
	}

	public static T ActiveInvert<T>(this T self) where T : Component
	{
		self.gameObject.SetActive(!self.gameObject.activeSelf);
		return self;
	}

	public static T SetName<T>(this T self, string name) where T : Component
	{
		self.gameObject.name = name;
		return self;
	}

	public static T SetLayer<T>(this T self, int layer) where T : Component
	{
		self.gameObject.layer = layer;
		return self;
	}

	public static T SetLayer<T>(this T self, string layer) where T : Component
	{
		self.gameObject.layer = LayerMask.NameToLayer(layer);
		return self;
	}

	public static T SetTag<T>(this T self, string tag) where T : Component
	{
		self.gameObject.tag = tag;
		return self;
	}

	public static void DestroyGameObject<T>(this T self) where T : Component
	{
		if ((bool)self && (bool)self.gameObject)
		{
			UnityEngine.Object.Destroy(self.gameObject);
		}
	}

	public static void DestroyGameObject<T>(this T self, float delay) where T : Component
	{
		if ((bool)self && (bool)self.gameObject)
		{
			UnityEngine.Object.Destroy(self.gameObject, delay);
		}
	}

	public static Component GetOrAddComponent<T>(this T self, Type type) where T : Component
	{
		Component component = self.gameObject.GetComponent(type);
		if (!(component != null))
		{
			return self.gameObject.AddComponent(type);
		}
		return component;
	}

	public static Mesh GetMeshFromMeshFilter(this GameObject self)
	{
		MeshFilter component = self.GetComponent<MeshFilter>();
		if (!component || !component.sharedMesh)
		{
			return null;
		}
		return component.sharedMesh;
	}

	public static Mesh GetMeshFromSkinnedMeshRenderer(this GameObject self)
	{
		SkinnedMeshRenderer component = self.GetComponent<SkinnedMeshRenderer>();
		if (!component || !component.sharedMesh)
		{
			return null;
		}
		return component.sharedMesh;
	}

	public static Material GetMaterial(this GameObject self)
	{
		MeshRenderer component = self.GetComponent<MeshRenderer>();
		if (!component)
		{
			return null;
		}
		return component.material;
	}

	public static bool IsVisible(this GameObject self, Camera camera)
	{
		Collider component = self.GetComponent<Collider>();
		if (null == component)
		{
			return false;
		}
		return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), component.bounds);
	}
}
