using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamDriver.Decal;

public class ImpactEffectScript : MonoBehaviour
{
    internal float lifeTime = 1f;

	internal Vector3 stayScale = Vector3.one;

	internal bool randomRotation = false;
	internal bool setFakeParent = false;
	private bool _hasSetParent = false;

	private Transform OGParent;
    public Transform fakeParent;
	private DecalMesh decalMeshScript;

	private float _random;
	private Vector3 _positionOffset;
	private Quaternion _rotationOffset;

	private void Awake()
	{
		decalMeshScript = transform.GetComponent<DecalMesh>();

		if (transform.parent)
		{
			OGParent = transform.parent;
		}
	}

	private void Update()
	{
		TrackFakeParent();
	}

	public void Initialize()
	{
		if (IsInvoking(nameof(DeactivateSelf))) { CancelInvoke(nameof(DeactivateSelf)); }

		Invoke(nameof(DeactivateSelf), lifeTime);

		SetImpactEffectStartTransforms();

		if (setFakeParent)
		{
			SetFakeParent();
		}
	}

	private void SetImpactEffectStartTransforms()
	{
		transform.SetGlobalScale(stayScale);

		_random = Random.Range(0f, 360f);

		if (randomRotation) { transform.localRotation *= Quaternion.Euler(0f, 180f, _random); }
	}

	public void DeactivateSelf()
	{
		if (OGParent)
		{
			transform.SetParent(OGParent);
		}

        fakeParent = null;

		_hasSetParent = false;

		CancelInvoke(nameof(DeactivateSelf));

		transform.localPosition = Vector3.zero;

		gameObject.SetActive(false);
	}

	public void SetFakeParent()
	{
		if (!fakeParent) { return; }

		_positionOffset = fakeParent.position - transform.position;

		_hasSetParent = true;
	}

	private void TrackFakeParent()
	{
		if (!fakeParent || !_hasSetParent) { return; }

		if (!fakeParent.gameObject.activeInHierarchy)
		{
			CancelInvoke(nameof(DeactivateSelf));

			DeactivateSelf();
		}

		transform.position = fakeParent.position - _positionOffset;
	}
}
