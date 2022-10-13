﻿ using UnityEngine;
 using UnityEngine.UI;
 using System.Collections.Generic;
 using UnityEngine.SceneManagement;
 using System.Collections;
 
 static class UtilityMethods {
    /// <summary>
    /// Rounds a Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    /// <summary>
    /// Floors a Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Floor(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Floor(vector3.x * multiplier) / multiplier,
            Mathf.Floor(vector3.y * multiplier) / multiplier,
            Mathf.Floor(vector3.z * multiplier) / multiplier);
    }

    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition   = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        Canvas.ForceUpdateCanvases();
        return result;
    }

    public static int Sign(bool b) {
        return b ? 1 : -1;
    }

    public static string GetHierarchicalName (this GameObject go) {
		string name = go.name;
		while (go.transform.parent != null) {

			go = go.transform.parent.gameObject;
			name = go.name + "/" + name;
		}
		return name;
	}

    public static void DrawBox(Bounds bounds, Color color) {
        Vector2 corner1;
        corner1.x = bounds.min.x;
        corner1.y = bounds.max.y;
        Vector2 corner2 = corner1;
        corner2.x = bounds.max.x;
        Debug.DrawLine(corner1, corner2, color);

        corner1 = corner2;
        corner2.y = bounds.min.y;
        Debug.DrawLine(corner1, corner2, color);

        corner1 = corner2;
        corner2.x = bounds.min.x;
        corner2.y = bounds.min.y;
        Debug.DrawLine(corner1, corner2, color);

        corner1 = corner2;
        corner2.y = bounds.max.y;
        Debug.DrawLine(corner1, corner2, color);
    }

    public static List<T> Find<T>(bool includeInactive=false)
         {
             List<T> interfaces = new List<T>();
             GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
             foreach( var rootGameObject in rootGameObjects )
             {
                 T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>(includeInactive: includeInactive);
                 foreach( var childInterface in childrenInterfaces )
                 {
                     interfaces.Add(childInterface);
                 }
             }
             return interfaces;
         }
 }
