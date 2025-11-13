using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponHitscanImpactHoleScript : MonoBehaviour
    {
		public float impactHoleDecalSize = 0.1f;
		public float impactHoleLifeTime = 10f;

		private WeaponBaseScript _weaponBaseScript;
		private WeaponEffectPoolScript _weaponEffectPoolScript;
		private WeaponEffectPoolScript.Effect _impactHoleEffect;

		private Vector3 _hitPoint;
		private RaycastHit _hit;
		private int _currentImpactHoleIndex = 0;

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			_weaponBaseScript = transform.GetComponent<WeaponBaseScript>();

			_weaponEffectPoolScript = transform.GetComponent<WeaponEffectPoolScript>();

			if (_weaponEffectPoolScript)
			{
				foreach (WeaponEffectPoolScript.Effect _effect in _weaponEffectPoolScript.effects)
				{
					if (_effect.effectType == WeaponEffectPoolScript.Effect.EffectType.ImpactHole)
					{
						_impactHoleEffect = _effect;
					}
				}
			}
		}

		public void PlaceImpactHole()
		{
			if (_impactHoleEffect == null || _weaponBaseScript == null) { return; }

			if (!_weaponBaseScript._hitSomething) { return; }

			if (_impactHoleEffect.weaponEffectPrefab == null || _impactHoleEffect.weaponEffectPoolParent == null || _impactHoleEffect.weaponEffectPoolList.Count < 1)
			{
				print("Something Wrong with impactSparkEffect.");
				return;
			}

			if (_impactHoleEffect.weaponEffectPoolList.Count < 1) { return; }

			_hitPoint = _weaponBaseScript._hitPoint;
			_hit = _weaponBaseScript._hit;

			Transform impactEffect = _impactHoleEffect.weaponEffectPoolList[_currentImpactHoleIndex];

			Quaternion stayRotation = Quaternion.LookRotation(_hit.normal);

			Transform fakeParent = _hit.transform;

			impactEffect.gameObject.SetActive(true);
			impactEffect.position = _hitPoint + _hit.normal * (_currentImpactHoleIndex * 0.00001f + 0.00001f);
			impactEffect.rotation = stayRotation;


			ImpactEffectScript impactEffectScript = impactEffect.GetComponent<ImpactEffectScript>();

			impactEffectScript.randomRotation = true;
			impactEffectScript.fakeParent = fakeParent;
			impactEffectScript.setFakeParent = true;
			impactEffectScript.lifeTime = impactHoleLifeTime;
			impactEffectScript.stayScale = Vector3.one * impactHoleDecalSize;
			impactEffectScript.Initialize();


			if (_currentImpactHoleIndex < _impactHoleEffect.weaponEffectPoolList.Count - 1)
			{
				_currentImpactHoleIndex++;
			}
			else
			{
				_currentImpactHoleIndex = 0;
			}
		}
	}
}