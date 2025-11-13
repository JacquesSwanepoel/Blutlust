using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
	[CreateAssetMenu]

	public class PlayerInputObject : ScriptableObject
	{
		internal InputActions _inputActions;

		private void OnEnable()
		{
			_inputActions = new InputActions();

			MovementInput();

			LookInput();

			Other();

			UI();
		}

		private void MovementInput()
		{
			_inputActions.Gameplay.Forward.Enable();

			_inputActions.Gameplay.Backward.Enable();

			_inputActions.Gameplay.Left.Enable();

			_inputActions.Gameplay.Right.Enable();

			_inputActions.Gameplay.Jump.Enable();

			_inputActions.Gameplay.Crouch.Enable();

			_inputActions.Gameplay.Fire.Enable();

			_inputActions.Gameplay.Reload.Enable();
		}

		private void LookInput()
		{
			_inputActions.Gameplay.LookX.Enable();

			_inputActions.Gameplay.LookY.Enable();
		}

		private void Other()
		{
			_inputActions.Gameplay.PreviousWeapon.Enable();
			_inputActions.Gameplay.NextWeapon.Enable();
		}

		private void UI()
		{
			_inputActions.Gameplay.Pause.Enable();

			_inputActions.Menu.UnPause.Enable();
		}

		private void OnDisable()
		{
			_inputActions.Gameplay.Forward.Disable();
			_inputActions.Gameplay.Backward.Disable();
			_inputActions.Gameplay.Left.Disable();
			_inputActions.Gameplay.Right.Disable();

			_inputActions.Gameplay.Jump.Disable();
			_inputActions.Gameplay.Crouch.Disable();
			_inputActions.Gameplay.Fire.Disable();
			_inputActions.Gameplay.Reload.Disable();

			_inputActions.Gameplay.LookX.Disable();
			_inputActions.Gameplay.LookY.Disable();

			_inputActions.Gameplay.PreviousWeapon.Disable();
			_inputActions.Gameplay.NextWeapon.Disable();


			_inputActions.Gameplay.Pause.Disable();

			_inputActions.Menu.UnPause.Disable();
		}
	}
}