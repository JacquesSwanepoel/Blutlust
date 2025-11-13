using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponKickBackScript : MonoBehaviour
    {
		[SerializeField] private Transform _pivotPoint;

		[Space(15)]

		[Header("Weapon Kickback Variables")]
		public int gunKickAmount = 6;
		public float gunKickRecoverySpeed = 4f;
		public float gunKickLimit = 20f;

		private Transform _playerController;
		private PlayerInputListener _playerInputListener;
		private WeaponSwayScript _weaponSwayScript;

		private Vector3 _initialPivotPointPos;

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			_playerController = GameObject.FindGameObjectWithTag("Player Controller").transform;

			if (_playerController)
			{
				if (_playerController.TryGetComponent(out PlayerInputListener playerInputListener)) { _playerInputListener = playerInputListener; }
			}

			if (transform.TryGetComponent(out WeaponSwayScript weaponSwayScript))
			{
				_weaponSwayScript = weaponSwayScript;
			}

			if (_pivotPoint && _weaponSwayScript)
			{
				_initialPivotPointPos = new Vector3(0f, 0f, -_weaponSwayScript.rotatePointOffset);
			}
		}

		private void Update()
		{
			Input();

			WeaponKickManager();
		}

		private void Input()
		{
			if (!_playerInputListener) { return; }
		}

		private void WeaponKickManager()
		{
			Vector3 _ref = Vector3.zero;
			if (_pivotPoint)
			{
				if (Vector3.Distance(_pivotPoint.localPosition, _initialPivotPointPos) > 0.001f)
				{
					_pivotPoint.localPosition = Vector3.SmoothDamp(_pivotPoint.localPosition, _initialPivotPointPos, ref _ref, 1f / gunKickRecoverySpeed / 5f);
				}
				else
				{
					_pivotPoint.localPosition = _initialPivotPointPos;
				}
			}
		}
		public void WeaponBackwardKick()
		{
			if (_pivotPoint)
			{
				if (_pivotPoint.localPosition.z > _initialPivotPointPos.z - gunKickLimit / 50f)
					_pivotPoint.localPosition = new Vector3(_pivotPoint.localPosition.x, _pivotPoint.localPosition.y, _pivotPoint.localPosition.z + gunKickAmount / 100f);
			}
		}
	}
}