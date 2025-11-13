using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using TMPro;

namespace Game
{
	public class InputRebinder : MonoBehaviour
	{
		[SerializeField] private PlayerSettingSaveObject _saveObject;
		[SerializeField] private PlayerInputObject _playerInput;
		[SerializeField] private List<InputAction> actions = new List<InputAction>();
		[Space(15)]
		[SerializeField] private List<Transform> _KMStartRebindObjectList = new List<Transform>();
		[SerializeField] private List<Transform> _KMWaitingForInputObjectList = new List<Transform>();
		[Space(15)]
		[SerializeField] private List<Transform> _GPStartRebindObjectList = new List<Transform>();
		[SerializeField] private List<Transform> _GPWaitingForInputObjectList = new List<Transform>();

		private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
		internal bool _isRebinding = false;

		private void Start()
		{
			Initialize();
		}
		private void Initialize()
		{
			actions.Clear();
			actions.Add(_playerInput._inputActions.Gameplay.Forward);
			actions.Add(_playerInput._inputActions.Gameplay.Backward);
			actions.Add(_playerInput._inputActions.Gameplay.Left);
			actions.Add(_playerInput._inputActions.Gameplay.Right);
			actions.Add(_playerInput._inputActions.Gameplay.Jump);
			actions.Add(_playerInput._inputActions.Gameplay.Crouch);
			actions.Add(_playerInput._inputActions.Gameplay.Fire);
			actions.Add(_playerInput._inputActions.Gameplay.NextWeapon);
			actions.Add(_playerInput._inputActions.Gameplay.PreviousWeapon);

			LoadNewBindings();

			for (int i = 0; i < _KMStartRebindObjectList.Count; i++)
			{
				UpdateButtonText(i, PlayerInputFunctions.KeyboardMouseBinding(_playerInput._inputActions), _KMStartRebindObjectList);
			}

			for (int i = 0; i < _GPStartRebindObjectList.Count; i++)
			{
				UpdateButtonText(i, PlayerInputFunctions.GamepadBinding(_playerInput._inputActions), _GPStartRebindObjectList);
			}
		}




		public void RebindGamepadBinding(int index)
		{
			if (_GPStartRebindObjectList.Count <= 0 && _GPWaitingForInputObjectList.Count <= 0 && actions.Count <= 0) { return; }

			if (_GPStartRebindObjectList[index]) { _GPStartRebindObjectList[index].gameObject.SetActive(false); }
			if (_GPWaitingForInputObjectList[index]) { _GPWaitingForInputObjectList[index].gameObject.SetActive(true); }

			actions[index].Disable();

			rebindingOperation = actions[index].PerformInteractiveRebinding()
				.WithControlsExcluding("Mouse")
				.WithCancelingThrough("<Gamepad>/start")
				.WithCancelingThrough("<Keyboard>/escape")
				.OnMatchWaitForAnother(0.1f)
				.WithControlsHavingToMatchPath("<Gamepad>/dpad/up")
				.WithControlsHavingToMatchPath("<Gamepad>/dpad/down")
				.WithControlsHavingToMatchPath("<Gamepad>/dpad/left")
				.WithControlsHavingToMatchPath("<Gamepad>/dpad/right")
				.WithControlsHavingToMatchPath("<Gamepad>/leftStick/up")
				.WithControlsHavingToMatchPath("<Gamepad>/leftStick/down")
				.WithControlsHavingToMatchPath("<Gamepad>/leftStick/left")
				.WithControlsHavingToMatchPath("<Gamepad>/leftStick/right")
				.WithControlsHavingToMatchPath("<Gamepad>/leftStickPress")
				.WithControlsHavingToMatchPath("<Gamepad>/rightStickPress")
				.WithControlsHavingToMatchPath("<Gamepad>/rightStick/up")
				.WithControlsHavingToMatchPath("<Gamepad>/rightStick/down")
				.WithControlsHavingToMatchPath("<Gamepad>/rightStick/left")
				.WithControlsHavingToMatchPath("<Gamepad>/rightStick/right")
				.WithControlsHavingToMatchPath("<Gamepad>/buttonNorth")
				.WithControlsHavingToMatchPath("<Gamepad>/buttonSouth")
				.WithControlsHavingToMatchPath("<Gamepad>/buttonWest")
				.WithControlsHavingToMatchPath("<Gamepad>/buttonEast")
				.WithControlsHavingToMatchPath("<Gamepad>/leftShoulder")
				.WithControlsHavingToMatchPath("<Gamepad>/leftTrigger")
				.WithControlsHavingToMatchPath("<Gamepad>/rightShoulder")
				.WithControlsHavingToMatchPath("<Gamepad>/rightTrigger")
				.WithControlsHavingToMatchPath("<DualShock4GamepadHID>/touchpadButton")
				.WithBindingMask(PlayerInputFunctions.GamepadBinding(_playerInput._inputActions))
				.OnComplete(operation => OnRebindComplete(index, PlayerInputFunctions.GamepadBinding(_playerInput._inputActions), _GPStartRebindObjectList, _GPWaitingForInputObjectList))
				.OnCancel(operation => OnRebindComplete(index, PlayerInputFunctions.GamepadBinding(_playerInput._inputActions), _GPStartRebindObjectList, _GPWaitingForInputObjectList))
				.Start();

			_isRebinding = true;
		}

		public void RebindKeyboardMouseBinding(int index)
		{
			if (_KMStartRebindObjectList.Count <= 0 && _KMWaitingForInputObjectList.Count <= 0 && actions.Count <= 0) { return; }

			if (_KMStartRebindObjectList[index]) { _KMStartRebindObjectList[index].gameObject.SetActive(false); }
			if (_KMWaitingForInputObjectList[index]) { _KMWaitingForInputObjectList[index].gameObject.SetActive(true); }

			actions[index].Disable();

			rebindingOperation = actions[index].PerformInteractiveRebinding()
				.OnMatchWaitForAnother(0.1f)
				.WithControlsHavingToMatchPath("<Keyboard>/q")
				.WithControlsHavingToMatchPath("<Keyboard>/w")
				.WithControlsHavingToMatchPath("<Keyboard>/e")
				.WithControlsHavingToMatchPath("<Keyboard>/r")
				.WithControlsHavingToMatchPath("<Keyboard>/t")
				.WithControlsHavingToMatchPath("<Keyboard>/y")
				.WithControlsHavingToMatchPath("<Keyboard>/u")
				.WithControlsHavingToMatchPath("<Keyboard>/i")
				.WithControlsHavingToMatchPath("<Keyboard>/o")
				.WithControlsHavingToMatchPath("<Keyboard>/p")
				.WithControlsHavingToMatchPath("<Keyboard>/a")
				.WithControlsHavingToMatchPath("<Keyboard>/s")
				.WithControlsHavingToMatchPath("<Keyboard>/d")
				.WithControlsHavingToMatchPath("<Keyboard>/f")
				.WithControlsHavingToMatchPath("<Keyboard>/g")
				.WithControlsHavingToMatchPath("<Keyboard>/h")
				.WithControlsHavingToMatchPath("<Keyboard>/j")
				.WithControlsHavingToMatchPath("<Keyboard>/k")
				.WithControlsHavingToMatchPath("<Keyboard>/l")
				.WithControlsHavingToMatchPath("<Keyboard>/z")
				.WithControlsHavingToMatchPath("<Keyboard>/x")
				.WithControlsHavingToMatchPath("<Keyboard>/c")
				.WithControlsHavingToMatchPath("<Keyboard>/v")
				.WithControlsHavingToMatchPath("<Keyboard>/b")
				.WithControlsHavingToMatchPath("<Keyboard>/n")
				.WithControlsHavingToMatchPath("<Keyboard>/m")
				.WithControlsHavingToMatchPath("<Keyboard>/0")
				.WithControlsHavingToMatchPath("<Keyboard>/1")
				.WithControlsHavingToMatchPath("<Keyboard>/2")
				.WithControlsHavingToMatchPath("<Keyboard>/3")
				.WithControlsHavingToMatchPath("<Keyboard>/4")
				.WithControlsHavingToMatchPath("<Keyboard>/5")
				.WithControlsHavingToMatchPath("<Keyboard>/6")
				.WithControlsHavingToMatchPath("<Keyboard>/7")
				.WithControlsHavingToMatchPath("<Keyboard>/8")
				.WithControlsHavingToMatchPath("<Keyboard>/9")
				.WithControlsHavingToMatchPath("<Keyboard>/upArrow")
				.WithControlsHavingToMatchPath("<Keyboard>/downArrow")
				.WithControlsHavingToMatchPath("<Keyboard>/leftArrow")
				.WithControlsHavingToMatchPath("<Keyboard>/rightArrow")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad0")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad1")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad2")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad3")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad4")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad5")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad6")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad7")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad8")
				.WithControlsHavingToMatchPath("<Keyboard>/numpad9")
				.WithControlsHavingToMatchPath("<Keyboard>/f1")
				.WithControlsHavingToMatchPath("<Keyboard>/f2")
				.WithControlsHavingToMatchPath("<Keyboard>/f3")
				.WithControlsHavingToMatchPath("<Keyboard>/f4")
				.WithControlsHavingToMatchPath("<Keyboard>/f5")
				.WithControlsHavingToMatchPath("<Keyboard>/f6")
				.WithControlsHavingToMatchPath("<Keyboard>/f7")
				.WithControlsHavingToMatchPath("<Keyboard>/f8")
				.WithControlsHavingToMatchPath("<Keyboard>/f9")
				.WithControlsHavingToMatchPath("<Keyboard>/f10")
				.WithControlsHavingToMatchPath("<Keyboard>/f11")
				.WithControlsHavingToMatchPath("<Keyboard>/f12")
				.WithControlsHavingToMatchPath("<Keyboard>/minus")
				.WithControlsHavingToMatchPath("<Keyboard>/equals")
				.WithControlsHavingToMatchPath("<Keyboard>/backquote")
				.WithControlsHavingToMatchPath("<Keyboard>/leftBracket")
				.WithControlsHavingToMatchPath("<Keyboard>/rightBracket")
				.WithControlsHavingToMatchPath("<Keyboard>/backslash")
				.WithControlsHavingToMatchPath("<Keyboard>/semicolon")
				.WithControlsHavingToMatchPath("<Keyboard>/quote")
				.WithControlsHavingToMatchPath("<Keyboard>/comma")
				.WithControlsHavingToMatchPath("<Keyboard>/period")
				.WithControlsHavingToMatchPath("<Keyboard>/slash")
				.WithControlsHavingToMatchPath("<Keyboard>/tab")
				.WithControlsHavingToMatchPath("<Keyboard>/leftShift")
				.WithControlsHavingToMatchPath("<Keyboard>/leftCtrl")
				.WithControlsHavingToMatchPath("<Keyboard>/leftAlt")
				.WithControlsHavingToMatchPath("<Keyboard>/space")
				.WithControlsHavingToMatchPath("<Keyboard>/rightShift")
				.WithControlsHavingToMatchPath("<Keyboard>/rightCtrl")
				.WithControlsHavingToMatchPath("<Keyboard>/rightAlt")
				.WithControlsHavingToMatchPath("<Keyboard>/enter")
				.WithControlsHavingToMatchPath("<Mouse>/leftButton")
				.WithControlsHavingToMatchPath("<Mouse>/middleButton")
				.WithControlsHavingToMatchPath("<Mouse>/rightButton")
				.WithControlsHavingToMatchPath("<Mouse>/forward")
				.WithControlsHavingToMatchPath("<Mouse>/backward")
				.WithControlsHavingToMatchPath("<Mouse>/scroll/y")
				.WithBindingMask(PlayerInputFunctions.KeyboardMouseBinding(_playerInput._inputActions))
				.OnComplete(operation => OnRebindComplete(index, PlayerInputFunctions.KeyboardMouseBinding(_playerInput._inputActions), _KMStartRebindObjectList, _KMWaitingForInputObjectList))
				.OnCancel(operation => OnRebindComplete(index, PlayerInputFunctions.KeyboardMouseBinding(_playerInput._inputActions), _KMStartRebindObjectList, _KMWaitingForInputObjectList))
				.Start();

			_isRebinding = true;
		}


		private void OnRebindComplete(int index, InputBinding inputBinding, List<Transform> _startRebindList, List<Transform> _waitingInputList)
		{
			print(rebindingOperation.candidates);
			rebindingOperation.Dispose();

			_isRebinding = false;

			if (_startRebindList[index])
			{
				_startRebindList[index].gameObject.SetActive(true);

				UpdateButtonText(index, inputBinding, _startRebindList);

				actions[index].Enable();
			}
			if (_waitingInputList[index]) { _waitingInputList[index].gameObject.SetActive(false); }

			SaveBindings();
		}
		private void UpdateButtonText(int index, InputBinding inputBinding, List<Transform> _startRebindList)
		{
			int bindingIndex = actions[index].GetBindingIndex(inputBinding);

			if (_startRebindList[index].GetChild(0).TryGetComponent(out TMP_Text tmp_text))
			{
				tmp_text.text = InputControlPath.ToHumanReadableString(
			actions[index].bindings[bindingIndex].effectivePath,
			InputControlPath.HumanReadableStringOptions.OmitDevice);
			}
		}


		public void SaveBindings()
		{
			if (!_playerInput || !_saveObject) { return; }

			_saveObject.savedBindings = _playerInput._inputActions.asset.SaveBindingOverridesAsJson();
		}

		private void LoadNewBindings()
		{
			if (!_playerInput || !_saveObject) { return; }

			if (_saveObject.savedBindings != string.Empty)
			{
				_playerInput._inputActions.asset.LoadBindingOverridesFromJson(_saveObject.savedBindings);
			}
		}


		public void LoadKeyboardDefaultBindings()
		{
			if (!_playerInput || !_saveObject) { return; }

			foreach (InputAction action in actions)
			{
				InputActionRebindingExtensions.RemoveBindingOverride(action, PlayerInputFunctions.KeyboardMouseBinding(_playerInput._inputActions));
			}

			_saveObject.savedBindings = _playerInput._inputActions.asset.SaveBindingOverridesAsJson();

			//Text Update
			for (int i = 0; i < _KMStartRebindObjectList.Count; i++)
			{
				UpdateButtonText(i, PlayerInputFunctions.KeyboardMouseBinding(_playerInput._inputActions), _KMStartRebindObjectList);
			}
		}
		public void LoadGamepadDefaultBindings()
		{
			if (!_playerInput || !_saveObject) { return; }

			foreach (InputAction action in actions)
			{
				InputActionRebindingExtensions.RemoveBindingOverride(action, PlayerInputFunctions.GamepadBinding(_playerInput._inputActions));
			}

			_saveObject.savedBindings = _playerInput._inputActions.asset.SaveBindingOverridesAsJson();

			//Text Update
			for (int i = 0; i < _GPStartRebindObjectList.Count; i++)
			{
				UpdateButtonText(i, PlayerInputFunctions.GamepadBinding(_playerInput._inputActions), _GPStartRebindObjectList);
			}
		}

		public void CancelRebinding()
		{
			rebindingOperation.Cancel();

			_isRebinding = false;
		}
	}
}