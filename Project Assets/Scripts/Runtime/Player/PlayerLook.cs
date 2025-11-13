using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
	public class PlayerLook : MonoBehaviour
	{
		[SerializeField] private PlayerInputObject _playerInput;

		[SerializeField] private Transform _rollParent;
		[SerializeField] private Transform _cameraParent;

		public float sensitivity;
		[Space(15)]
		public bool globalXAxisRot = false;
		[Space(15)]
		public bool lean = true;
		public float leanAmount;
		public float leanSpeed;

		internal Vector2 _lookInput;
		private Vector2 _moveInput;

		private float rotateY = 0f;
		private float rotateX = 0f;

		private float rotateZ = 0f;

		private void Start()
		{
			Application.targetFrameRate = 144;

			Initialize();
		}

		private void Update()
		{
			Input();

			Look();
		}

		private void FixedUpdate()
		{
			MoveCameraTilt();
		}

		private void Initialize()
		{

		}

		private void Input()
		{
			if (_playerInput == null) { return; }

			_lookInput = new Vector2(_playerInput._inputActions.Gameplay.LookX.ReadValue<float>(), _playerInput._inputActions.Gameplay.LookY.ReadValue<float>());

			float xInput = _playerInput._inputActions.Gameplay.Right.ReadValue<float>() - _playerInput._inputActions.Gameplay.Left.ReadValue<float>();
			float yInput = _playerInput._inputActions.Gameplay.Forward.ReadValue<float>() - _playerInput._inputActions.Gameplay.Backward.ReadValue<float>();

			_moveInput = new Vector2(xInput, yInput);

			if (_moveInput.magnitude > 1f)
			{
				_moveInput /= _moveInput.magnitude;
			}
		}

		private void Look()
		{
			if (_rollParent == null) { return; }

			rotateY += _lookInput.x * sensitivity / 45.45723f;
			rotateX += _lookInput.y * sensitivity / 45.45723f;

			rotateX = Mathf.Clamp(rotateX, -89f, 89f);

			transform.localRotation = Quaternion.Euler(transform.localRotation.x, rotateY, transform.localRotation.z);

			//_cameraParent.localRotation = Quaternion.Euler(-rotateY, _cameraParent.localRotation.y, -rotateZ);

			if (lean)
			{
				transform.localRotation = Quaternion.Euler(new Vector3(0f, rotateY, 0f));
				_rollParent.localRotation = Quaternion.Euler(new Vector3(!globalXAxisRot ? 0f : -rotateX, 0f, -rotateZ));
				_cameraParent.localRotation = Quaternion.Euler(new Vector3(globalXAxisRot ? 0f : -rotateX, 0f, 0f));
			}
			else
			{
				transform.localRotation = Quaternion.Euler(new Vector3(0f, rotateY, 0f));
				_cameraParent.localRotation = Quaternion.Euler(new Vector3(-rotateX, 0f, 0f));
			}
		}

		private void MoveCameraTilt()
		{
			if (!_rollParent) { return; }

			float _ref = 0f;
			rotateZ = Mathf.SmoothDamp(rotateZ, leanAmount * _moveInput.x, ref _ref, 1f / leanSpeed / 5f);
		}
	}
}