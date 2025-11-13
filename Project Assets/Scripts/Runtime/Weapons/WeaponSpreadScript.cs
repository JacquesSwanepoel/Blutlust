using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponSpreadScript : MonoBehaviour
    {
		[Header("Spread")]
		public float horizontalSpread;
		public float verticalSpread;
		public bool dynamicSpread = true;
		public bool firstAccurate = false;
		[Range(0f, 1f)] public float baseSpreadAmount = 0f;
		public float dynamicSpreadExpandAmount = 0.1f;
		public float dynamicSpreadRecoverSpeed = 0.1f;

		private Camera _camera;
		private WeaponBaseScript _weaponBaseScript;

		internal float _currentSpreadAmount = 0f;

		private Vector3 _horizontalDirection;
		private Vector3 _verticalDirection;

		private float _horAngle;
		private float _verAngle;

		private List<Vector2> textureCoord = new List<Vector2>();

		private void Start()
		{
			Initialize();
		}

		private void Update()
		{
			SpreadRecoveryManager();
		}

		public void Initialize()
		{
			_camera = Camera.main;

			_weaponBaseScript = transform.GetComponent<WeaponBaseScript>();

			_currentSpreadAmount = baseSpreadAmount;
		}

		public void CalculateSpread()
		{
			if (_weaponBaseScript == null) { return; }

			if (dynamicSpread)
			{
				if (_currentSpreadAmount == baseSpreadAmount && firstAccurate)
				{
					_currentSpreadAmount = 0f;
				}
			}

			Vector2 spread = new Vector2(horizontalSpread, verticalSpread);

			if (spread.magnitude > 1f)
			{
				spread /= spread.magnitude;
			}

			_horAngle = UnityEngine.Random.Range(0f, spread.x * horizontalSpread) * Mathf.Deg2Rad * _currentSpreadAmount;
			_verAngle = UnityEngine.Random.Range(0f, spread.y * verticalSpread) * Mathf.Deg2Rad * _currentSpreadAmount;

			int leftRight = UnityEngine.Random.Range(0, 2);
			int upDown = UnityEngine.Random.Range(0, 2);

			_horizontalDirection = leftRight == 0 ? -_camera.transform.right : _camera.transform.right;
			_verticalDirection = upDown == 0 ? -_camera.transform.up : _camera.transform.up;

			Vector3 FireDirection = Vector3.RotateTowards(_camera.transform.forward, _horizontalDirection, _horAngle, 0f);
			FireDirection = Vector3.RotateTowards(FireDirection, _verticalDirection, _verAngle, 0f);

			_weaponBaseScript._fireDirection = FireDirection;

			if (dynamicSpread)
			{
				_currentSpreadAmount += dynamicSpreadExpandAmount;
			}

			if (_currentSpreadAmount > 1f)
			{
				_currentSpreadAmount = 1f;
			}
		}

		private void SpreadRecoveryManager()
		{
			if (!dynamicSpread) { return; }

			if (_currentSpreadAmount > baseSpreadAmount)
			{
				_currentSpreadAmount -= Time.deltaTime * dynamicSpreadRecoverSpeed;
			}
			else if (_currentSpreadAmount < baseSpreadAmount)
			{
				_currentSpreadAmount = baseSpreadAmount;
			}
		}

		private void OnDrawGizmos()
		{
			if (_weaponBaseScript == null) { return; }

			//Gizmos.draw
		}
	}
}