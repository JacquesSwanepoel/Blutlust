using System;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCameraScript : MonoBehaviour
{
    private Camera _mainCamera;

	Vector3 _finalTarget;

	public enum ForwardAxis
	{
		forward,
		right,
		up
	};

	public ForwardAxis forwardAxis = new ForwardAxis();

	public enum Direction
	{
		normal,
		flipped
	};

	public Direction direction = new Direction();

	private void OnValidate()
	{
		if (Camera.main && !_mainCamera)
		{
			_mainCamera = Camera.main;
		}

		RotateObject();
	}

	void Update()
    {
		RotateObject();
	}

	private void RotateObject()
	{
		if (_mainCamera)
		{
			Vector3 target = transform.position -_mainCamera.transform.position;

			transform.rotation = Quaternion.LookRotation((target).normalized);

			switch (forwardAxis)
			{
				case ForwardAxis.forward:
					_finalTarget = transform.forward;
					break;
				case ForwardAxis.right:
					_finalTarget = transform.right;
					break;
				case ForwardAxis.up:
					_finalTarget = transform.up;
					break;
			}

			switch (direction)
			{
				case Direction.flipped:
					_finalTarget = Vector3.Scale(_finalTarget, -Vector3.one);
					break;
			}

			transform.rotation = Quaternion.LookRotation(_finalTarget);
		}
	}
}
