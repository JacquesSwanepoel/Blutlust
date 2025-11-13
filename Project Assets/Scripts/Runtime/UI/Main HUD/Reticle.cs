using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
	[SerializeField] private Transform _reticle;
	[Space(15)]
	[SerializeField] private Color _color;
	[Space(10)]
	public int rReticle = 255;
	public int gReticle = 255;
	public int bReticle = 255;
	[Space(20)]
	public float defaultSize = 1f;
	[Space(20)]
	public float spreadSize = 1f;
	public float resetSpeed = 1f;

	private void Start()
	{
		Initialize();
	}
	private void Update()
	{
		ResetReticle();
	}

	private void OnValidate()
	{
		_color.r = rReticle / 255;
		_color.g = gReticle / 255;
		_color.b = bReticle / 255;

		if (_reticle)
		{
			if (_reticle.TryGetComponent(out SpriteRenderer sRenderer))
			{
				sRenderer.color = _color;
			}
		}
	}

	private void Initialize()
	{
		
	}

	public void ChangeReticleTransform()
	{
		if (_reticle)
		{
			_reticle.localScale = Vector3.one * defaultSize;
		}
	}

	public void SpreadReticle()
	{
		_reticle.localScale = Vector3.one * defaultSize * spreadSize;
	}

	private void ResetReticle()
	{
		if (_reticle.localScale.x >  defaultSize)
		{
			_reticle.localScale -= Vector3.one * Time.deltaTime * resetSpeed;
		}
		else
		{
			_reticle.localScale = Vector3.one * defaultSize;
		}
	}
}
