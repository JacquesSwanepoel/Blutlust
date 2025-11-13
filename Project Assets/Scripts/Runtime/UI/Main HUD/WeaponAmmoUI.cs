using UnityEngine;
using TMPro;

namespace Game
{
	public class WeaponAmmoUI : MonoBehaviour
	{
		[SerializeField] private TMP_Text _ammo;

		private Transform _playerWeaponContainer = null;
		private Transform _currentWeapon = null;

		private void Awake()
		{
			Initialize();
		}

		public void Initialize()
		{
			_playerWeaponContainer = GameObject.FindWithTag("Weapon Parent").transform;
		}

		private void Update()
		{
			AmmoUIManager();
		}

		private void AmmoUIManager()
		{
			if (_playerWeaponContainer == null) { return; }

			if (_playerWeaponContainer.TryGetComponent<WeaponSwitcher>(out WeaponSwitcher _weaponSwitcher))
			{
				_currentWeapon = _playerWeaponContainer.GetChild(_weaponSwitcher._weaponIndex);
			}

			if (_currentWeapon == null) { return; }

			if (_currentWeapon.TryGetComponent<WeaponAmmoScript>(out WeaponAmmoScript _weaponAmmoScript))
			{
				_ammo.text = _weaponAmmoScript.ammo.ToString();
			}
		}
	}
}