using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLineRenderer : MonoBehaviour
{
	internal Vector3 point1;
	internal Vector3 point2;

	internal Vector3 target;

	internal float speed;

	internal float _lifeTime;

	private float _time;

	private void Update()
	{
		MoveLRPoints();

		_time += Time.deltaTime;

		if (_time > _lifeTime)
		{
			_time = 0;

			transform.gameObject.SetActive(false);
		}
	}

	private void MoveLRPoints()
	{
		if (Vector3.Distance(point1, target) > 0.01f)
		{
			point1 = Vector3.MoveTowards(point1, target, speed * Time.deltaTime);
		}
		if (Vector3.Distance(point2, target) > 0.01f)
		{
			point2 = Vector3.MoveTowards(point2, target, speed * Time.deltaTime);
		}

		transform.GetComponent<LineRenderer>().SetPositions(new Vector3[2] { point1, point2 });
	}
}
