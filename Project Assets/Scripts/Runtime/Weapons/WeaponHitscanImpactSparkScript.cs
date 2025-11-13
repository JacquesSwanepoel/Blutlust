using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponHitscanImpactSparkScript : MonoBehaviour
    {
		public float impactSparkParticleSize = 0.04f;
		public float impactSparkLifeTime = 0.4f;

		private WeaponBaseScript _weaponBaseScript;
		private WeaponEffectPoolScript _weaponEffectPoolScript;
		private WeaponEffectPoolScript.Effect _impactSparkEffect;

		private Vector3 _hitPoint;
		private RaycastHit _hit;
		private int _currentImpactSparkIndex = 0;

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
					if (_effect.effectType == WeaponEffectPoolScript.Effect.EffectType.ImpactSpark)
					{
						_impactSparkEffect = _effect;
					}
				}
			}
		}

		public void PlaceImpactSpark()
		{
			if (_impactSparkEffect == null || _weaponBaseScript == null) { return; }

			if (_impactSparkEffect.weaponEffectPrefab == null || _impactSparkEffect.weaponEffectPoolParent == null || _impactSparkEffect.weaponEffectPoolList.Count < 1)
			{
				print("Something Wrong with impactSparkEffect.");
				return;
			}

			if (_impactSparkEffect.weaponEffectPoolList.Count < 1) { return; }

			_hitPoint = _weaponBaseScript._hitPoint;
			_hit = _weaponBaseScript._hit;

			Transform impactEffect = _impactSparkEffect.weaponEffectPoolList[_currentImpactSparkIndex];

			Quaternion stayRotation = Quaternion.FromToRotation(transform.up, _hit.normal);

			impactEffect.gameObject.SetActive(true);
			impactEffect.position = _hitPoint;
			impactEffect.rotation = stayRotation;


			ImpactEffectScript impactEffectScript = impactEffect.GetComponent<ImpactEffectScript>();
			impactEffectScript.lifeTime = impactSparkLifeTime;
			impactEffectScript.stayScale = Vector3.one * impactSparkParticleSize;
			impactEffectScript.Initialize();


			if (_currentImpactSparkIndex < _impactSparkEffect.weaponEffectPoolList.Count - 1)
			{
				_currentImpactSparkIndex++;
			}
			else
			{
				_currentImpactSparkIndex = 0;
			}
		}
	}
}