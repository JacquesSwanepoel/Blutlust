using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnviromentLightingScript : MonoBehaviour
{
	[SerializeField] private Light _mainLight;
	[Space(15)]
	[SerializeField] [Min(0f)] private float _intensity = 1f;
	[SerializeField] private float _multiplier = 1f;
	[Space(15)]
	[SerializeField] private Gradient _skyColorShift;
	[SerializeField] private Gradient _equatorColorShift;
	[SerializeField] private Gradient _groundColorShift;

	private void OnValidate()
	{
		_mainLight = RenderSettings.sun;

		if (_mainLight)
		{
			_mainLight.intensity = _intensity;

			RenderSettings.ambientSkyColor = _skyColorShift.Evaluate(_mainLight.intensity * _multiplier);
			RenderSettings.ambientEquatorColor = _equatorColorShift.Evaluate(_mainLight.intensity * _multiplier);
			RenderSettings.ambientGroundColor = _groundColorShift.Evaluate(_mainLight.intensity * _multiplier);
		}
	}
}
