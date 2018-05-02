using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{

	public static void SetLayerRecursively(this GameObject obj, int newLayer)
	{
		if (null == obj)
		{
			return;
		}

		obj.layer = newLayer;

		foreach (Transform child in obj.transform)
		{
			if (null == child)
			{
				continue;
			}
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}
	public static void SetActiveRecursive(this GameObject obj, bool active)
	{
		if (null == obj)
			return;

		obj.SetActive(active);
		foreach (Transform child in obj.transform)
		{
			if (null == child)
			{
				continue;
			}
			SetActiveRecursive(child.gameObject, active);
		}
	}
}
