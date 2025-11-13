using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureColorRampGenerator : MonoBehaviour
{
	[SerializeField] private Shader _debugMatShader;
    [SerializeField] private Transform _debugPlane;
	[Space(15)]
	[SerializeField] private string _exportPath = "";
	[SerializeField] private bool _exportTexture = false;
	[Space(15)]
	[SerializeField] private Texture2D _forgedTexture;
	[SerializeField] private bool safetySwitch = false;
	public bool applyForgedTextureALL = false;
	public bool applyForgedTextureLIST = false;
	[Space(15)]
	[SerializeField] private bool _safetySwitch = false;
	public bool useCustomGradientInShaderALL = false;
	[Space(10)]
	public bool resetShaderGradientsToPrevious = false;
	[Space(10)]
	public bool useCustomGradientInShaderLIST = false;
	[SerializeField] private List<Material> _pliableMaterials = new List<Material>();
	[Space(15)]
	[SerializeField] private bool _copyDiffuseToSpecular = false;
	[SerializeField] private Gradient _specularGradient;
	[SerializeField] private Gradient _diffuseGradient;
	[Min(0)] [SerializeField] private int _resolution;
	[Space(15)]
	public Texture2D customTextureColorRamp;
	[Space(15)]
	[SerializeField] private bool updateTextureOnResolutionChange = true;
	[SerializeField] private bool updateTextureOnGradientChange = true;
	[SerializeField] private bool updateTexture = false;

    private int _oldResolution;

	private List<GradientColorKey> _oldDiffuseGradientKeys = new List<GradientColorKey>();
	private GradientMode _oldDiffuseGradientMode;
	private List<GradientColorKey> _oldSpecularGradientKeys = new List<GradientColorKey>();
	private GradientMode _oldSpecularGradientMode;

	private List<Material> _oldMaterials = new List<Material>();
	private List<Texture2D> _oldTextureColorRamps = new List<Texture2D>();

	private Material _debugMat;

    private void OnResolutionChange()
	{
        Initialize();

		if (updateTextureOnResolutionChange)
		{
			GenerateTexture();
		}
	}

	private void OnGradientChange()
	{
		GenerateTexture();
	}

    private void Initialize()
	{
        
	}

    private void GenerateTexture()
	{
        customTextureColorRamp = new Texture2D(_resolution, _resolution);

		for (int x = 0; x < _resolution; x++)
		{
            for (int y = 0; y < _resolution; y++)
			{
				if (y < (float)_resolution / 2f)
				{
					customTextureColorRamp.SetPixel(x, y, _diffuseGradient.Evaluate((float)x / (float)_resolution));
				}
				else
				{
					customTextureColorRamp.SetPixel(x, y, _specularGradient.Evaluate((float)x / (float)_resolution));
				}
			}
		}

		customTextureColorRamp.Apply();

		if (_debugPlane)
		{
			if (!_debugMatShader) { return; }

			_debugMat = new Material(_debugMatShader)
			{
				mainTexture = customTextureColorRamp,
				color = Color.white
			};

			if (_debugPlane.TryGetComponent(out MeshRenderer mRenderer))
			{
				mRenderer.material = _debugMat;
			}
		}
	}

	private void OnValidate()
	{
		if (_resolution != _oldResolution)
		{
            OnResolutionChange();

            _oldResolution = _resolution;
		}

		if (updateTextureOnGradientChange)
		{
			if (_diffuseGradient.mode != _oldDiffuseGradientMode || _specularGradient.mode != _oldSpecularGradientMode)
			{
				OnGradientChange();

				_oldDiffuseGradientMode = _diffuseGradient.mode;
				_oldSpecularGradientMode = _specularGradient.mode;
			}

			for (int i = 0; i < _diffuseGradient.colorKeys.Length; i++)
			{
				if (_diffuseGradient.colorKeys.Length != _oldDiffuseGradientKeys.Count)
				{
					_oldDiffuseGradientKeys.Clear();

					foreach (GradientColorKey GCK in _diffuseGradient.colorKeys)
					{
						_oldDiffuseGradientKeys.Add(GCK);
					}
				}
				else
				{
					if (_diffuseGradient.colorKeys[i].color != _oldDiffuseGradientKeys[i].color || _diffuseGradient.colorKeys[i].time != _oldDiffuseGradientKeys[i].time)
					{
						_oldDiffuseGradientKeys[i] = _diffuseGradient.colorKeys[i];

						OnGradientChange();

						break;
					}
				}
			}

			for (int i = 0; i < _specularGradient.colorKeys.Length; i++)
			{
				if (_specularGradient.colorKeys.Length != _oldSpecularGradientKeys.Count)
				{
					_oldSpecularGradientKeys.Clear();

					foreach (GradientColorKey GCK in _specularGradient.colorKeys)
					{
						_oldSpecularGradientKeys.Add(GCK);
					}
				}
				else
				{
					if (_specularGradient.colorKeys[i].color != _oldSpecularGradientKeys[i].color || _specularGradient.colorKeys[i].time != _oldSpecularGradientKeys[i].time)
					{
						_oldSpecularGradientKeys[i] = _specularGradient.colorKeys[i];

						OnGradientChange();

						break;
					}
				}
			}
		}

		ApplyManager();
	}

	private void ApplyManager()
	{
		if (_copyDiffuseToSpecular)
		{
			_specularGradient.colorKeys = _oldDiffuseGradientKeys.ToArray();

			GenerateTexture();

			_copyDiffuseToSpecular = false;
		}

		if (updateTexture)
		{
			GenerateTexture();

			updateTexture = false;
		}

		if (useCustomGradientInShaderALL && _safetySwitch)
		{
			_oldMaterials.Clear();
			_oldTextureColorRamps.Clear();

			foreach (Transform _transform in FindObjectsOfType<Transform>())
			{
				if (_transform.TryGetComponent(out Renderer _renderer))
				{
					if (_renderer.sharedMaterial)
					{
						if (_renderer.sharedMaterial.HasProperty("Texture2D_34D208B2"))
						{
							_oldMaterials.Add(_renderer.sharedMaterial);
							_oldTextureColorRamps.Add((Texture2D)_renderer.sharedMaterial.GetTexture("Texture2D_34D208B2"));

							_renderer.sharedMaterial.SetTexture("Texture2D_34D208B2", customTextureColorRamp);
						}
					}
				}
			}

			useCustomGradientInShaderALL = false;
			_safetySwitch = false;
		}

		if (resetShaderGradientsToPrevious)
		{
			for (int i = 0; i < _oldMaterials.Count; i++)
			{
				_oldMaterials[i].SetTexture("Texture2D_34D208B2", _oldTextureColorRamps[i]);
			}

			resetShaderGradientsToPrevious = false;
		}

		if (useCustomGradientInShaderLIST)
		{
			foreach (Material _material in _pliableMaterials)
			{
				if (_material.HasProperty("Texture2D_34D208B2"))
				{
					_material.SetTexture("Texture2D_34D208B2", customTextureColorRamp);

					print(_material.name);
				}
			}

			useCustomGradientInShaderLIST = false;
		}

		if (_exportTexture)
		{
			if (customTextureColorRamp)
			{
				byte[] bytes = customTextureColorRamp.EncodeToPNG();
				string _systemPath = Application.dataPath + "/" + _exportPath.Replace("Assets/", string.Empty);
				string _path = Application.dataPath + "/" + _exportPath.Replace("Assets/", string.Empty) + "/" + "CustomTextureID" + Random.Range(0f, 1000f).ToString() + ".png";
				if (Directory.Exists(_systemPath))
				{
					File.WriteAllBytes(_path, bytes);
				}

				AssetDatabase.Refresh();
			}

			_exportTexture = false;
		}

		if (applyForgedTextureALL && safetySwitch && _forgedTexture)
		{
			_oldMaterials.Clear();
			_oldTextureColorRamps.Clear();

			foreach (Transform _transform in FindObjectsOfType<Transform>())
			{
				if (_transform.TryGetComponent(out Renderer _renderer))
				{
					if (_renderer.sharedMaterial)
					{
						if (_renderer.sharedMaterial.HasProperty("Texture2D_34D208B2"))
						{
							_oldMaterials.Add(_renderer.sharedMaterial);
							_oldTextureColorRamps.Add((Texture2D)_renderer.sharedMaterial.GetTexture("Texture2D_34D208B2"));

							_renderer.sharedMaterial.SetTexture("Texture2D_34D208B2", _forgedTexture);
						}
					}
				}
			}

			applyForgedTextureALL = false;
			safetySwitch = false;
		}

		if (applyForgedTextureLIST && _forgedTexture)
		{
			foreach (Material _material in _pliableMaterials)
			{
				if (_material.HasProperty("Texture2D_34D208B2"))
				{
					_material.SetTexture("Texture2D_34D208B2", _forgedTexture);

					print(_material.name);
				}
			}

			applyForgedTextureLIST = false;
		}
	}
}
