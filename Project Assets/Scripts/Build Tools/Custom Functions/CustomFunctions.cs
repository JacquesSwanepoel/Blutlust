using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CustomFunctions 
{
    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    public static List<GameObject> ObjectsWithLayer(LayerMask layer)
    {
        List<GameObject> ObjectsWithLayer = new List<GameObject>();

        foreach (GameObject obj in AllGameObjects)
        {
            if (1 << obj.gameObject.layer == layer)
            {
                ObjectsWithLayer.Add(obj);
            }
        }
        return ObjectsWithLayer;
    }

    public static GameObject[] AllGameObjects
	{
		get
		{
            return Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        }
	}
}
