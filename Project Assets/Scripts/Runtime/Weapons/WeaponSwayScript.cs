using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class WeaponSwayScript : MonoBehaviour
    {
		[SerializeField] private Transform _pivotPoint;
		[Space(15)]
		public float rotatePointOffset = 1.5f;
		[Space(10)]
		public bool predictiveSway = true;
		[Space(10)]
		public float gunSwayAmountX = 12f;
		public float gunSwayAmountY = 12f;
		public float gunSwayResetSmoothingX = 25f;
		public float gunSwayResetSmoothingY = 25f;
		public float gunSwaySmoothing = 40f;
		public float gunSwayClampX = 10f;
		public float gunSwayClampY = 10f;

		private Transform _playerController;
		private PlayerInputListener _playerInputListener;

		private Vector2 _lookInput;

		private Vector3 _initialPivotPointRot;
		private Vector3 _targetWeaponRotation;
		private Vector3 _newWeaponRotation;

		private int _reactivePredictive = 1;

		private void Start()
		{
			Initialize();
		}

		private void OnValidate()
		{
			if (_pivotPoint)
			{
				_pivotPoint.localPosition = new Vector3(0f, 0f, -rotatePointOffset);
				_pivotPoint.GetChild(0).localPosition = new Vector3(0f, 0f, rotatePointOffset);
			}
		}

		public void Initialize()
		{
			_playerController = GameObject.FindGameObjectWithTag("Player Controller").transform;

			if (_playerController)
			{
				if (_playerController.TryGetComponent(out PlayerInputListener playerInputListener)) { _playerInputListener = playerInputListener;  }
			}

			if (_pivotPoint)
			{
				_initialPivotPointRot = _pivotPoint.localRotation.eulerAngles;

				_pivotPoint.localPosition = new Vector3(0f, 0f, -rotatePointOffset);
				_pivotPoint.GetChild(0).localPosition = new Vector3(0f, 0f, rotatePointOffset);
			}
		}

		private void Update()
		{
			Input();

			RotateWeapon();
		}

		private void Input()
		{
			if (!_playerInputListener) { return; }

			_lookInput = _playerInputListener.lookInput;
		}

		private void RotateWeapon()
		{
			_reactivePredictive = predictiveSway ? -1 : 1;

			float desiredGunXRotation = _lookInput.x * gunSwayAmountX / 30f * Time.deltaTime * _reactivePredictive;
			float desiredGunYRotation = _lookInput.y * gunSwayAmountY / 30f * Time.deltaTime * _reactivePredictive;

			_targetWeaponRotation.x += -desiredGunYRotation;
			_targetWeaponRotation.y += -desiredGunXRotation;

			_targetWeaponRotation.x = Mathf.Clamp(_targetWeaponRotation.x, -gunSwayClampX, gunSwayClampX);
			_targetWeaponRotation.y = Mathf.Clamp(_targetWeaponRotation.y, -gunSwayClampY, gunSwayClampY);

			float _V1ref = 0f;
			Vector3 _V3ref = Vector3.zero;

			_targetWeaponRotation.x = Mathf.SmoothDamp(_targetWeaponRotation.x, _initialPivotPointRot.x , ref _V1ref, 1f / gunSwayResetSmoothingX);
			_targetWeaponRotation.y = Mathf.SmoothDamp(_targetWeaponRotation.y, _initialPivotPointRot.y , ref _V1ref, 1f / gunSwayResetSmoothingY);

			_newWeaponRotation = Vector3.SmoothDamp(_newWeaponRotation, _targetWeaponRotation, ref _V3ref, 1f / gunSwaySmoothing);

			if (_pivotPoint)
			{
				if (_lookInput.magnitude != 0f || _pivotPoint.localRotation.eulerAngles != _initialPivotPointRot)
				{
					_pivotPoint.localRotation = Quaternion.Euler(_newWeaponRotation);
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (_pivotPoint)
			{
				Gizmos.DrawWireSphere(_pivotPoint.position, 0.1f);
			}
		}
	}
}