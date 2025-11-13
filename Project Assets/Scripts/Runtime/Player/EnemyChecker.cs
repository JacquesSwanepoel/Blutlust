using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
	public List<Transform> enemies = new List<Transform>();
	public LayerMask enemyLayers;

	private void Start()
	{
		FindEnemies();
	}

	public void FindEnemies()
	{
		enemies.Clear();

		foreach (GameObject obj in CustomFunctions.ObjectsWithLayer(enemyLayers))
		{
			enemies.Add(obj.transform);
		}
	}
}
