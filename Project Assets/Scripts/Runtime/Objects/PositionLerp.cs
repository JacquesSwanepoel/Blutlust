using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLerp : MonoBehaviour
{
	[SerializeField] private Transform _object;
    [SerializeField] private Transform _position1;
    [SerializeField] private Transform _position2;
	[Space(10)]
	[SerializeField] private float _speed;
    [SerializeField][Range(0f, 1f)] private float _lerpTime;
    [SerializeField] private AnimationCurve _lerpCurve;

	private bool add = true;

	private void OnValidate()
	{
		if (_object && _position1 && _position2)
		{
			_object.position = Vector3.Lerp(_position1.position, _position2.position, _lerpCurve.Evaluate(_lerpTime));
		}
	}

	private void Update()
	{
		if (add)
		{
			_lerpTime += Time.deltaTime * _speed;

			if (_lerpTime > 1f)
			{
				add = false;
			}
		}
		else
		{
			_lerpTime -= Time.deltaTime * _speed;

			if (_lerpTime < 0f)
			{
				add = true;
			}
		}

		_lerpTime = Mathf.Clamp01(_lerpTime);

		if (_object && _position1 && _position2)
		{
			_object.position = Vector3.Lerp(_position1.position, _position2.position, _lerpCurve.Evaluate(_lerpTime));
		}
	}

	private void OnDrawGizmos()
	{
		if (_position1 && _position2)
		{
			Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(_position1.position, 1f);
			Gizmos.DrawWireSphere(_position2.position, 1f);
		}
	}
}
