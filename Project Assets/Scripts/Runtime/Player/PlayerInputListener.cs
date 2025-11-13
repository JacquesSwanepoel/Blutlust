using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlayerInputListener : MonoBehaviour
	{
		public PlayerInputObject _playerInput;

		public float lookXInput;
		public float lookYInput;
		public Vector2 lookInput;

		public float fireInput;

		private void Update()
		{
			Input();
		}

		private void Input()
		{
			if (_playerInput == null) { return; }

			fireInput = _playerInput._inputActions.Gameplay.Fire.ReadValue<float>() > 0.01f ? 1f : 0f;

			lookXInput = _playerInput._inputActions.Gameplay.LookX.ReadValue<float>();
			lookYInput = _playerInput._inputActions.Gameplay.LookY.ReadValue<float>();
			lookInput = new Vector2(lookXInput, lookYInput);
		}
	}
}