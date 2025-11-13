using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCC : MonoBehaviour
{
	[Min(0)] public float height = 2f;
	public Vector3 center = Vector3.zero;
	[Range(0f, 1f)] public float stepToHeightRatio = 0.8f;

	private CharacterController _characterController;

	private void OnValidate()
	{
		if (transform.TryGetComponent(out CharacterController _char) && !_characterController)
		{
			_characterController = _char;
		}

		EasyCCSetup();
	}

	private void Update()
	{
		EasyCCSetup();
	}

	internal Vector3 _ccBottomSpherePos
	{
		get
		{
			if (_characterController)
			{
				return transform.position + _characterController.center - transform.up * (_characterController.height / 2f - _characterController.radius);
			}
			else
			{
				return transform.position;
			}
		}
	}

	internal Vector3 _ccTopSpherePos
	{
		get
		{
			if (_characterController)
			{
				return transform.position + _characterController.center + transform.up * (_characterController.height / 2f - _characterController.radius + _characterController.stepOffset);
			}
			else
			{
				return transform.position;
			}
		}
	}

	internal float radius
	{
		get
		{
			if (_characterController)
			{
				return _characterController.radius + _characterController.skinWidth;
			}
			else
			{
				return 0f;
			}
		}
	}

	internal void EasyCCSetup()
	{
		if (_characterController)
		{
			_characterController.height = (stepToHeightRatio + 1f) / 2f * height;
			_characterController.stepOffset = (1f - (stepToHeightRatio + 1f) / 2f) * height;

			_characterController.center = new Vector3(0f, (height / 4f * stepToHeightRatio) - height / 4f, 0f) + center;
		}
	}

	private void OnDrawGizmos()
	{
		if (_characterController)
		{
			Gizmos.color = Color.magenta;

			float radius = _characterController.radius + _characterController.skinWidth;

			Gizmos.DrawWireSphere(_ccTopSpherePos, radius);
			Gizmos.DrawLine(_ccTopSpherePos + transform.forward * radius, _ccBottomSpherePos + transform.forward * radius);
			Gizmos.DrawLine(_ccTopSpherePos - transform.forward * radius, _ccBottomSpherePos - transform.forward * radius);
			Gizmos.DrawLine(_ccTopSpherePos + transform.right * radius, _ccBottomSpherePos + transform.right * radius);
			Gizmos.DrawLine(_ccTopSpherePos - transform.right * radius, _ccBottomSpherePos - transform.right * radius);
			Gizmos.DrawWireSphere(_ccBottomSpherePos, radius);

			Gizmos.color = Color.cyan;

			Vector3 ccBottomBoundsPos = _ccBottomSpherePos - transform.up * this.radius;

			Vector3 stepOffsetDebugCenter = Vector3.Lerp(ccBottomBoundsPos, ccBottomBoundsPos + transform.up * _characterController.stepOffset, 0.5f);

			Gizmos.DrawWireCube(stepOffsetDebugCenter, new Vector3(this.radius * 2f, _characterController.stepOffset, this.radius * 2f));
		}
	}
}
