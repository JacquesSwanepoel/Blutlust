using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponHitscanLineRendererScript : MonoBehaviour
    {
		[Header("Bullet Line Variables")]
		public float lineRendererLength = 5f;
		public float lineRendererSpeed = 10f;
		[Range(0f, 1f)] public float lineRendererStartKeyOpacity = 0f;
		[Range(0f, 1f)] public float lineRendererMidKeyOpacity = 1f;
		[Range(0f, 1f)] public float lineRendererEndKeyOpacity = 1f;
		[Range(0f, 1f)] public float lineRendererMidKeyPos = 0.3f;
		public float lineRendererEnabledTime = 0.1f;

		private WeaponBaseScript _weaponBaseScript;
		private WeaponEffectPoolScript _weaponEffectPoolScript;
		private WeaponEffectPoolScript.Effect _lineRendererEffect;

		private Camera _camera;
		private Transform _firePoint;
		private Vector3 _LRhitPoint;
		private int _currentLineRendererIndex = 0;

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			_camera = Camera.main;

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

			if (Vector3.Distance(_camera.transform.position, _LRhitPoint) <= lineRendererLength) { return; }

			lineRenderer.gameObject.SetActive(true);

			MovingLineRenderer LRScript = lineRenderer.GetComponent<MovingLineRenderer>();
			LRScript._lifeTime = lineRendererEnabledTime;
			LRScript.point1 = _firePoint.position;
			LRScript.point2 = lineRendererLength * Vector3.Normalize(_LRhitPoint - _firePoint.position) + _firePoint.position;
			LRScript.target = _LRhitPoint;
			LRScript.speed = lineRendererSpeed;

			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(lineRenderer.colorGradient.colorKeys[0].color, 0.0f), new GradientColorKey(lineRenderer.colorGradient.colorKeys[1].color, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(lineRendererStartKeyOpacity, 0.0f), new GradientAlphaKey(lineRendererMidKeyOpacity, lineRendererMidKeyPos), new GradientAlphaKey(lineRendererEndKeyOpacity, 1.0f) }
			);

			lineRenderer.colorGradient = gradient;

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