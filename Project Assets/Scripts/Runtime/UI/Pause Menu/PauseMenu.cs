using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;

namespace Game
{
	public class PauseMenu : MonoBehaviour
	{
		[SerializeField] private PlayerInputObject _playerInput;
		[SerializeField] private InputRebinder _inputRebinder;

		public bool freezeWorldOnPause = true;

		public Transform pauseButtons;
		public Transform settingsButtons;
		public Transform keyboardRebindButtons;
		public Transform gamepadRebindButtons;
		[Space(20)]
		[SerializeField] private Slider _sensitivitySlider;
		public float sensMin;
		public float sensMax;
		[Space(20)]
		[SerializeField] private TMP_InputField _sensitivityInputField;
		[Space(20)]
		public bool isInPauseMenu = false;
		public bool isInSettingsMenu = false;
		public bool isInKeyboardRebindMenu = false;
		public bool isInGamepadRebindMenu = false;
		[Space(20)]
		public Transform disableKeyboardMouseUIToggle;
		private Animator _disableKeyboardMouseAnimator;
		[Space(10)]
		[SerializeField] private Transform inputDeviceDisplayParent;
		private Transform gamepadImage;
		private Transform keyboardMouseImage;

		private Transform _playerController;
		private PlayerLook _playerLook;

		private Transform _mainHUD = null;
		private Transform _pauseMenu = null;

		private float _pauseInput = 0f;
		private float _unPauseInput = 0f;

		private float _currentSensitivity = 0f;
		
		private int _LDPauseMenu = 0;
		private int _LDSettingsMenu = 0;
		private int _LDKeyboardRebindMenu = 0;
		private int _LDGamepadRebindMenu = 0;

		private int _commaChecker = 0;

		private bool _useKeyboardMouse = true;
		private bool _useGamepad = false;

		private bool _isRebinding = false;

		private void Start()
		{
			Inititalize();
		}

		private void Inititalize()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_playerController = GameObject.FindWithTag("Player Controller").transform;
			
			if (_playerController.TryGetComponent<PlayerLook>(out PlayerLook playerLook)) { _playerLook = playerLook; }
			if (_playerLook) { _currentSensitivity = _playerLook.sensitivity; }

			_pauseMenu = transform.GetChild(0).transform;
			_mainHUD = GameObject.FindWithTag("Main HUD").transform;

			if (_sensitivitySlider) 
			{ 
				_sensitivitySlider.value = _currentSensitivity;
				_sensitivitySlider.minValue = sensMin;
				_sensitivitySlider.maxValue = sensMax;

				_sensitivityInputField.text = _currentSensitivity.ToString();
			}

			if (_playerInput)
			{
				OnChangeInputDevice();
			}

			if (disableKeyboardMouseUIToggle)
			{
				disableKeyboardMouseUIToggle.TryGetComponent(out _disableKeyboardMouseAnimator);
			}

			if (inputDeviceDisplayParent)
			{
				if (inputDeviceDisplayParent.childCount >= 2)
				{
					gamepadImage = inputDeviceDisplayParent.GetChild(0);
					keyboardMouseImage = inputDeviceDisplayParent.GetChild(1);
				}
			}
		}

		private void Update()
		{
			Input();

			OpenPauseMenu();

			InPauseMenu();

			InSettingsMenu();

			InKeyboardRebind();

			InGamepadRebind();

			if (_inputRebinder)
			{
				_isRebinding = _inputRebinder._isRebinding;
			}
		}

		private void Input()
		{
			if (_playerInput == null) { return; }

			_pauseInput = _playerInput._inputActions.Gameplay.Pause.ReadValue<float>();

			_unPauseInput = _playerInput._inputActions.Menu.UnPause.ReadValue<float>();
		}


		private void OpenPauseMenu()
		{
			if (isInPauseMenu) { return; }

			if (_pauseInput == 1f && _LDPauseMenu == 0)
			{
				_LDPauseMenu++;

				OnOpenPauseMenu();
			}
			else if (_pauseInput == 0f && _LDPauseMenu == 1)
			{
				_LDPauseMenu--;
			}
		}

		public void OnOpenPauseMenu()
		{
			/////////////////////
			if (freezeWorldOnPause) { Time.timeScale = 0f; }

			if (_pauseMenu)
			{
				_pauseMenu.gameObject.SetActive(true);
			}

			if (_mainHUD)
			{
				_mainHUD.gameObject.SetActive(false);
			}

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

			if (_playerInput)
			{
				_playerInput._inputActions.Gameplay.Disable();
				_playerInput._inputActions.Menu.Enable();
			}
			/////////////////////

			pauseButtons.gameObject.SetActive(true);
			settingsButtons.gameObject.SetActive(false);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(false);

			isInPauseMenu = true;
			isInSettingsMenu = false;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = false;
		}
		private void InPauseMenu()
		{
			if (!isInPauseMenu) { return; }

			if (_LDPauseMenu == 0 && _unPauseInput == 1f)
			{
				_LDPauseMenu++;

				OnClosePauseMenu();
			}
			else if (_LDPauseMenu == 1 && _pauseInput == 0f && _unPauseInput == 0f)
			{
				_LDPauseMenu--;
			}
		}
		public void OnClosePauseMenu()
		{
			/////////////////////
			if (freezeWorldOnPause) { Time.timeScale = 1f; }

			if (_pauseMenu)
			{
				_pauseMenu.gameObject.SetActive(false);
			}

			if (_mainHUD)
			{
				_mainHUD.gameObject.SetActive(true);
			}

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			if (_playerInput)
			{
				_playerInput._inputActions.Gameplay.Enable();
				_playerInput._inputActions.Menu.Disable();
			}
			/////////////////////

			isInPauseMenu = false;
			isInSettingsMenu = false;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = false;

			pauseButtons.gameObject.SetActive(false);
			settingsButtons.gameObject.SetActive(false);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(false);
		}




		public void OnOpenSettings()
		{
			isInPauseMenu = false;
			isInSettingsMenu = true;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = false;

			pauseButtons.gameObject.SetActive(false);
			settingsButtons.gameObject.SetActive(true);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(false);

		}
		private void InSettingsMenu()
		{
			if (!isInSettingsMenu) { return; }

			if (_unPauseInput == 1f && _LDSettingsMenu == 0)
			{
				_LDSettingsMenu++;
				_LDPauseMenu++;

				OnCloseSettings();
			}
			else if (_unPauseInput == 0f && _LDSettingsMenu == 1)
			{
				_LDSettingsMenu--;
			}
		}
		public void OnCloseSettings()
		{
			isInPauseMenu = true;
			isInSettingsMenu = false;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = false;

			pauseButtons.gameObject.SetActive(true);
			settingsButtons.gameObject.SetActive(false);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(false);
		}

		public void OnSensSliderChange(Slider slider)
		{
			if (_sensitivitySlider)
			{
				if (_playerLook)
				{
					float sens = (float)System.Math.Round(slider.value, 2);
					_playerLook.sensitivity = sens;
					
					if (_sensitivityInputField)
					{
						_sensitivityInputField.text = sens.ToString();
						_currentSensitivity = sens;
					}
				}
			}
		}
		public void OnSensEditBoxChange(TMP_InputField inputField)
		{
			if (_playerLook)
			{
				foreach (char _char in inputField.text)
				{
					if (!char.IsNumber(_char) && _char != " "[0] && _char != ","[0] && _char != "."[0])
					{
						inputField.text = _currentSensitivity.ToString();
						return;
					}
				}

				string text = inputField.text;

				text = text.Replace(" ", "");
				text = text.Replace(",", ".");

				foreach (char _char in text)
				{
					if (_char == "."[0] && _commaChecker == 1)
					{
						inputField.text = _currentSensitivity.ToString();
						return;
					}

					if (_char == "."[0] && _commaChecker == 0)
					{
						_commaChecker++;
					}
				}

				_commaChecker = 0;

				if (float.Parse(text) >= 0f && float.Parse(text) > sensMin && float.Parse(text) < sensMax)
				{
					_playerLook.sensitivity = float.Parse(text);
					_sensitivitySlider.value = float.Parse(text);
					_currentSensitivity = _playerLook.sensitivity;
				}
				else
				{
					inputField.text = _currentSensitivity.ToString();
				}
			}
		}

		public void OnUseGamepadToggle()
		{
			if (!_playerInput) { return; }

			if (_useGamepad)
			{
				_useGamepad = false;

				if (disableKeyboardMouseUIToggle)
				{
					StartCoroutine(nameof(DisableDisableKeyboardMouseToggleDelay));

					disableKeyboardMouseUIToggle.GetComponent<Toggle>().enabled = false;
					disableKeyboardMouseUIToggle.GetComponent<Toggle>().isOn = false;

					if (_disableKeyboardMouseAnimator) { _disableKeyboardMouseAnimator.SetTrigger("Disable"); }
				}

				if (!_useKeyboardMouse)
				{
					_useKeyboardMouse = true;
				}
			}
			else
			{
				StopCoroutine(nameof(DisableDisableKeyboardMouseToggleDelay));

				_useGamepad = true;

				if (disableKeyboardMouseUIToggle)
				{
					disableKeyboardMouseUIToggle.gameObject.SetActive(true);

					disableKeyboardMouseUIToggle.GetComponent<Toggle>().enabled = true;

					if (_disableKeyboardMouseAnimator) { _disableKeyboardMouseAnimator.SetTrigger("Enable"); }
				}
			}

			OnChangeInputDevice();
		}
		public void OnDisableKeyboardMouseToggle()
		{
			if (!_playerInput) { return; }

			if (_useKeyboardMouse)
			{
				_useKeyboardMouse = false;
			}
			else
			{
				_useKeyboardMouse = true;
			}

			OnChangeInputDevice();
		}
		private IEnumerator DisableDisableKeyboardMouseToggleDelay()
		{
			yield return new WaitForSecondsRealtime(0.25f);

			disableKeyboardMouseUIToggle.gameObject.SetActive(false);
		}
		private void OnChangeInputDevice()
		{
			if (_useKeyboardMouse && !_useGamepad)
			{
				PlayerInputFunctions.ChangeControlScheme(_playerInput._inputActions, _playerInput._inputActions.Mouse_KeyboardScheme);
				print("KeyboardMouse Active");

				if (keyboardMouseImage) { keyboardMouseImage.gameObject.SetActive(true); }
				if (gamepadImage) { gamepadImage.gameObject.SetActive(false); }
			}
			else if (!_useKeyboardMouse && _useGamepad)
			{
				PlayerInputFunctions.ChangeControlScheme(_playerInput._inputActions, _playerInput._inputActions.GamepadScheme);
				print("Gamepad Active");

				if (keyboardMouseImage) { keyboardMouseImage.gameObject.SetActive(false); }
				if (gamepadImage) { gamepadImage.gameObject.SetActive(true); }
			}
			else
			{
				PlayerInputFunctions.RemoveBindingMask(_playerInput._inputActions);
				print("All Devices Active");

				if (keyboardMouseImage) { keyboardMouseImage.gameObject.SetActive(true); }
				if (gamepadImage) { gamepadImage.gameObject.SetActive(true); }
			}
		}




		public void OnOpenKeyboardRebind()
		{
			isInPauseMenu = false;
			isInSettingsMenu = false;
			isInKeyboardRebindMenu = true;
			isInGamepadRebindMenu = false;

			pauseButtons.gameObject.SetActive(false);
			settingsButtons.gameObject.SetActive(false);
			keyboardRebindButtons.gameObject.SetActive(true);
			gamepadRebindButtons.gameObject.SetActive(false);
		}
		private void InKeyboardRebind()
		{
			if (!isInKeyboardRebindMenu) { return; }

			if (_unPauseInput == 1f && _LDKeyboardRebindMenu == 0 && !_isRebinding)
			{
				_LDKeyboardRebindMenu++;
				_LDSettingsMenu++;

				OnCloseKeyboardRebind();
			}
			else if (_unPauseInput == 0f && _LDKeyboardRebindMenu == 1)
			{
				_LDKeyboardRebindMenu--;
			}
		}
		public void OnCloseKeyboardRebind()
		{
			isInPauseMenu = false;
			isInSettingsMenu = true;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = false;

			pauseButtons.gameObject.SetActive(false);
			settingsButtons.gameObject.SetActive(true);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(false);
		}


		public void OnOpenGamepadRebind()
		{
			isInPauseMenu = false;
			isInSettingsMenu = false;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = true;

			pauseButtons.gameObject.SetActive(false);
			settingsButtons.gameObject.SetActive(false);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(true);
		}
		private void InGamepadRebind()
		{
			if (!isInGamepadRebindMenu) { return; }

			if (_unPauseInput == 1f && _LDGamepadRebindMenu == 0 && !_isRebinding)
			{
				_LDGamepadRebindMenu++;
				_LDSettingsMenu++;

				OnCloseGamepadRebind();
			}
			else if (_unPauseInput == 0f && _LDGamepadRebindMenu == 1)
			{
				_LDGamepadRebindMenu--;
			}
		}
		public void OnCloseGamepadRebind()
		{
			isInPauseMenu = false;
			isInSettingsMenu = true;
			isInKeyboardRebindMenu = false;
			isInGamepadRebindMenu = false;

			pauseButtons.gameObject.SetActive(false);
			settingsButtons.gameObject.SetActive(true);
			keyboardRebindButtons.gameObject.SetActive(false);
			gamepadRebindButtons.gameObject.SetActive(false);
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}