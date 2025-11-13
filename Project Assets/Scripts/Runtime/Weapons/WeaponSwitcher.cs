using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
	public class WeaponSwitcher : MonoBehaviour
	{
		[SerializeField] private PlayerInputObject _playerInput = null;

		private Transform currentActiveWeapon;

		private float _previousWeaponInput;
		private float _nextWeaponInput;

		public int _weaponIndex = 0;

		public float childCount = 0f;

		public bool onlySwitchOnCanFire = true;

		private void Start()
		{
			Initialize();
		}

		private void Initialize() 
		{
			childCount = transform.childCount;

			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(true);
			}

			Invoke(nameof(OnWeaponSwitch), 0.01f);
		}

		private void Update()
		{
			Input();
		}

		private void LateUpdate()
		{
			SwitchWeaponManager();
		}

		private void Input()
		{
			if (_playerInput == null) { return; }

			_previousWeaponInput = _playerInput._inputActions.Gameplay.PreviousWeapon.ReadValue<float>() > 0f ? Mathf.Abs(_playerInput._inputActions.Gameplay.PreviousWeapon.ReadValue<float>()) / 120f : 0f;

			_nextWeaponInput = _playerInput._inputActions.Gameplay.NextWeapon.ReadValue<float>() < 0f ? Mathf.Abs(_playerInput._inputActions.Gameplay.NextWeapon.ReadValue<float>()) / 120f : 0f;
		}

		private void SwitchWeaponManager()
		{
			if (onlySwitchOnCanFire && currentActiveWeapon)
			{
				if (currentActiveWeapon.TryGetComponent<WeaponBaseScript>(out WeaponBaseScript wScript))
				{
					if (!wScript._canSwitch) { return; }
				}
			}

			if (_previousWeaponInput > 0f)
			{
				_weaponIndex--;
			}
			else if (_nextWeaponInput > 0f)
			{
				_weaponIndex++;
			}

			if (_weaponIndex < 0)
			{
				_weaponIndex = (int) childCount - 1;
			}
			else if (_weaponIndex > childCount - 1)
			{
				_weaponIndex = 0;
			}

			if (_previousWeaponInput > 0f)
			{
				OnWeaponSwitch();
			}
			else if (_nextWeaponInput > 0f)
			{
				OnWeaponSwitch();
			}
		}

		private void OnWeaponSwitch()
		{
			foreach (Transform child in transform)
			{
				if (child == transform.GetChild(_weaponIndex))
				{
					child.gameObject.SetActive(true);
					currentActiveWeapon = child;
				}
				else
				{
					child.gameObject.SetActive(false);
				}
			}
		}
	}
}