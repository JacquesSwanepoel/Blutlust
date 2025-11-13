using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game
{
    public class WeaponEffectPoolScript : MonoBehaviour
    {
		[Serializable]
		public class Effect
		{
			public enum EffectType
			{
				LineRenderer,
				ImpactSpark,
				ImpactHole
			}

			public EffectType effectType = new EffectType();

			public Transform weaponEffectPrefab;
			public int weaponEffectPoolSize;
			public Transform weaponEffectPoolParent;
			public List<Transform> weaponEffectPoolList = new List<Transform>();
		}

		public List<Effect> effects = new List<Effect>();

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			foreach (Effect _effect in effects)
			{
				PopulateImpactEffectPool(_effect.weaponEffectPrefab, _effect.weaponEffectPoolList, _effect.weaponEffectPoolSize, _effect.weaponEffectPoolParent);
			}
		}

		private void PopulateImpactEffectPool(Transform _weaponEffectPrefab, List<Transform> _weaponEffectPoolList, int _weaponEffectPoolSize, Transform _weaponEffectPoolParent)
		{
			if (!_weaponEffectPrefab || !_weaponEffectPoolParent) { return; }

			for (int i = 0; i < _weaponEffectPoolSize; i++)
			{
				Transform child = Instantiate(_weaponEffectPrefab, transform.position, Quaternion.identity, _weaponEffectPoolParent);
				child.gameObject.SetActive(false);
			}

			_weaponEffectPoolList.Clear();

			foreach (Transform impactEffect in _weaponEffectPoolParent)
			{
				_weaponEffectPoolList.Add(impactEffect);
			}
		}
	}
}