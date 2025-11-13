using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLineRenderer : MonoBehaviour
{
	public LineRenderer lr;

	internal AnimationCurve _curve;

	internal float _lifeTime;
	private float _time;

	private GradientAlphaKey[] _alphaKeys;
	private GradientAlphaKey[] _initialAlphaKeys;
	private int _LDAlphaKeys = 0;

	private void Update()
	{
		FadeLine();

		_time += Time.deltaTime;

		if (_time > _lifeTime)
		{
			_time = 0;

			transform.gameObject.SetActive(false);
		}
	}

	internal void PlaceLine(Vector3 point1, Vector3 point2)
	{
		if (transform.TryGetComponent(out LineRenderer _lr))
		{
			lr = _lr;
		}

		lr.SetPositions(new Vector3[2] { point1, point2 });

		_LDAlphaKeys = 0;
	}

	private void FadeLine()
	{
		if (lr)
		{
			if (lr.colorGradient.alphaKeys.Length > 0 && _curve != null)
			{
				if (_LDAlphaKeys == 0)
				{
					_initialAlphaKeys = lr.colorGradient.alphaKeys;
					_alphaKeys = lr.colorGradient.alphaKeys;
				}

				for (int i = 0; i < _alphaKeys.Length; i++)
				{
					_alphaKeys[i] = new GradientAlphaKey(_curve.Evaluate(1f - _time / _lifeTime) * _initialAlphaKeys[i].alpha, _initialAlphaKeys[i].time);
				}

				_LDAlphaKeys++;
			}

			Gradient gr = new Gradient();

			gr.SetKeys(lr.colorGradient.colorKeys, _alphaKeys);

			lr.colorGradient = gr;
		}
	}
}
