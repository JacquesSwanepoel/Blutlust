using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponHitscanStaticLineRendererScript : MonoBehaviour
    {
		[Header("Bullet Line Variables")]
		public Gradient LineRendererGradient;
		public float lineRendererEnabledTime = 0.1f;
		public AnimationCurve LineRendererFadeCurve;

		private WeaponBaseScript _weaponBaseScript;
		private WeaponEffectPoolScript _weaponEffectPoolScript;
		private WeaponEffectPoolScript.Effect _lineRendererEffect;

		private Transform _firePoint;
		private Vector3 _LRhitPoint;
		private int _currentLineRendererIndex = 0;

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			_weaponBaseScript = transform.GetComponent<WeaponBaseScript>();

			if (_weaponBaseScript)
			{
				_firePoint = _weaponBaseScript.firePoint;
			}

			_weaponEffectPoolScript = transform.GetComponent<WeaponEffectPoolScript>();

			if (_weaponEffectPoolScript)
			{
				foreach (WeaponEffectPoolScript.Effect _effect in _weaponEffectPoolScript.effects)
				{
					if (_effect.effectType == WeaponEffectPoolScript.Effect.EffectType.LineRenderer)
					{
						_lineRendererEffect = _effect;
					}
				}
			}
		}

		public void ShootLineRenderer()
		{
			if (_firePoint == null || _lineRendererEffect == null || _weaponBaseScript == null) { return; }

			if (_lineRendererEffect.weaponEffectPrefab == null || _lineRendererEffect.weaponEffectPoolParent == null || _lineRendererEffect.weaponEffectPoolList.Count < 1)
			{
				print("Something Wrong with linerendererEffect.");
				return;
			}

			LineRenderer lineRenderer = _lineRendererEffect.weaponEffectPoolList[_currentLineRendererIndex].GetComponent<LineRenderer>();

			_LRhitPoint = _weaponBaseScript._hitPoint;

			lineRenderer.colorGradient = LineRendererGradient;
			lineRenderer.gameObject.SetActive(true);

			StaticLineRenderer LRScript = lineRenderer.GetComponent<StaticLineRenderer>();

			LRScript.PlaceLine(_firePoint.position, _LRhitPoint);

			LRScript._lifeTime = lineRendererEnabledTime;
			LRScript._curve = LineRendererFadeCurve;

			if (_currentLineRendererIndex < _lineRendererEffect.weaponEffectPoolList.Count - 1)
			{
				_currentLineRendererIndex++;
			}
			else
			{
				_currentLineRendererIndex = 0;
			}
		}
	}
}