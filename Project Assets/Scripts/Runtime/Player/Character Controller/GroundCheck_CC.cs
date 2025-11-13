using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
	public class SurfaceRay
	{
		public Ray ray = new Ray();
		public Color color = Color.white;

		public bool hitSomething = false;
		public string layerHit = "";
		public Vector3 surfNorm = Vector3.zero;
		public float surfNormAngle = 0f;
	}

	public class GroundCheck_CC : MonoBehaviour
	{
		public LayerMask groundLayers;
		public LayerMask movingLayers;
		[Space(15)]
		[Min(1)] public int groupCount = 1;
		[Min(1)] public int radialCount = 1;
		[Space(15)]
		[Range(0f, 0.05f)] public float offset = 0.01f;
		[Range(0f, 90f)] public float coneAngle = 90f;
		[Range(0f, 90f)] public float maxGroundedAngle = 90f;
		[Space(15)]
        public bool _isGrounded = false;
		public bool _isOnSlope = false;

		internal Vector3 _surfNorm = Vector3.zero;
		public float _surfNormAngle = 0f;

		private CharacterController _characterController;
		private CustomCC _customCC;

		private Ray _ray;
		private RaycastHit _hit;

		public List<SurfaceRay> surfaceRayList = new List<SurfaceRay>();

		private int _raysHitCount = 0;
		private int _groundDetectCount;

		internal int _debugRaysHitCount = 0;

		private void Awake()
		{
			_characterController = transform.GetComponent<CharacterController>();
		}

		private void Update()
		{
			DrawRays();
		}

		private void OnValidate()
		{
			if (transform.TryGetComponent(out CharacterController _char) && !_characterController)
			{
				_characterController = _char;
			}
			if (transform.TryGetComponent(out CustomCC _custom) && !_customCC)
			{
				_customCC = _custom;
			}

			DrawRays();
		}

		private void DrawRays()
		{
			if (_characterController)
			{
				groupCount = Mathf.Clamp(groupCount, 1, groupCount * radialCount);

				surfaceRayList.Clear();

				if (_collidersInBoundingBox != 0)
				{
					if (groupCount != 1)
					{
						for (int i = 0; i < groupCount; i++)
						{
							float tickAngle = coneAngle / groupCount;

							Vector3 groupDir = Vector3.RotateTowards(-transform.up, transform.forward, (tickAngle + tickAngle / (groupCount - 1)) * i * Mathf.Deg2Rad, 0f);

							groupDir.Normalize();

							if (i != 0)
							{
								for (int r = 0; r < Mathf.Floor(radialCount); r++)
								{
									Vector3 radialDir = Quaternion.AngleAxis(360f / radialCount * r, transform.up) * groupDir;

									_ray = new Ray(_ccBottomSpherePos, radialDir);

									GenerateSurfaceRay(_ray);

									SurfaceDetextion();
								}
							}
							else
							{
								_ray = new Ray(_ccBottomSpherePos, groupDir);

								GenerateSurfaceRay(_ray);

								SurfaceDetextion();
							}
						}
					}
					else
					{
						_ray = new Ray(_ccBottomSpherePos, -transform.up);

						GenerateSurfaceRay(_ray);

						SurfaceDetextion();
					}
				}

				CalculateSurfaces();
			}
		}

		private void GenerateSurfaceRay(Ray ray)
		{
			SurfaceRay surfaceRay = new SurfaceRay();
			surfaceRay.ray = ray;
			surfaceRayList.Add(surfaceRay);
		}

		private Vector3 _ccBottomSpherePos
		{
			get
			{
				if (_customCC)
				{
					return _customCC._ccBottomSpherePos;
				}
				else if (_characterController)
				{
					return transform.position + _characterController.center - transform.up * (_characterController.height / 2f - _characterController.radius);
				}
				else
				{
					return transform.position;
				}
			}
		}

		private float _rayLength
		{
			get
			{
				if (_characterController)
				{
					return _characterController.radius + _characterController.skinWidth + offset;
				}
				else
				{
					return 1f;
				}
			}
		}

		private int _collidersInBoundingBox
		{
			get
			{
				Collider[] colliders = Physics.OverlapSphere(_ccBottomSpherePos, _rayLength * 1.1f);

				int x = 0;

				foreach (Collider collider in colliders)
				{
					x++;
				}

				return 1;
			}
		}

		private void SurfaceDetextion()
		{
			SurfaceRay currentSurfaceRay = surfaceRayList[surfaceRayList.Count - 1];

			if (Physics.Raycast(currentSurfaceRay.ray, out _hit, _rayLength))
			{
				_raysHitCount++;

				int hitLayer = 1 << _hit.collider.gameObject.layer;
				string hitLayerName = LayerMask.LayerToName(_hit.collider.gameObject.layer);

				currentSurfaceRay.color = Color.red;        //Set all the variables for the current SurfaceRay
				currentSurfaceRay.hitSomething = true;
				currentSurfaceRay.layerHit = hitLayerName;
				currentSurfaceRay.surfNorm = _hit.normal;
				currentSurfaceRay.surfNormAngle = Vector3.Angle(_hit.normal, transform.up);

				if (hitLayer == groundLayers || hitLayer == movingLayers)
				{
					currentSurfaceRay.color = Color.cyan;

					_groundDetectCount++;
				}

				surfaceRayList[surfaceRayList.Count - 1] = currentSurfaceRay;
			}
		}

		private void CalculateSurfaces()
		{
			_surfNorm = Vector3.zero;
			_surfNormAngle = 0f;

			foreach (SurfaceRay surfaceRay in surfaceRayList)
			{
				if (surfaceRay.surfNormAngle > _surfNormAngle && surfaceRay.surfNorm != Vector3.zero && surfaceRay.hitSomething)//Chooses the largest surfNormAngle for the Final Normal
				{
					_surfNorm = surfaceRay.surfNorm;
					_surfNormAngle = surfaceRay.surfNormAngle;
				}
			}

			foreach (SurfaceRay surfaceRay in surfaceRayList)
			{
				if (surfaceRay.surfNormAngle < _surfNormAngle && surfaceRay.surfNorm != Vector3.zero && surfaceRay.hitSomething)//Chooses the smallest surfNormAngle for the Final Normal
				{
					_surfNorm = surfaceRay.surfNorm;
					_surfNormAngle = surfaceRay.surfNormAngle;
				}
			}

			_debugRaysHitCount = _raysHitCount;

			if (_groundDetectCount > 0)
			{
				if (_surfNormAngle < maxGroundedAngle)
				{
					_isGrounded = true;
				}
				else
				{
					_isGrounded = false;
				}
			}
			else
			{
				_isGrounded = false;
			}

			_groundDetectCount = 0;
			_raysHitCount = 0;
		}

		private void OnDrawGizmos()
		{
			foreach (SurfaceRay surfaceRay in surfaceRayList)
			{
				Gizmos.color = surfaceRay.color;
				Gizmos.DrawRay(surfaceRay.ray.origin, surfaceRay.ray.direction * _rayLength);
			}
		}
	}
}