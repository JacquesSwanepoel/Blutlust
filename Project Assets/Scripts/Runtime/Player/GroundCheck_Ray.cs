using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GroundCheck_Ray : MonoBehaviour
    {
        private CapsuleCollider _capsuleCollider;

        public LayerMask groundLayers;
        [Space(15)]
        [Range(0f, 90f)] public float maxGroundedAngle = 80f;
		[Min(0f)] public float raycastLength = 0.25f;
        [Space(15)]
        public bool rayGrounded = false;
		public bool colGrounded = false;
        [Space(15)]
        public Vector3 surfNorm = Vector3.zero;
        public float surfNormAngle = 0f;

		private RaycastHit _hit;

		private void Start()
		{
			Initialize();
		}

		private void OnValidate()
		{
			if (transform.TryGetComponent(out CapsuleCollider capsuleCollider) && !_capsuleCollider)
			{
				_capsuleCollider = capsuleCollider;
			}
		}

		public void Initialize()
		{
			if (transform.TryGetComponent(out CapsuleCollider capsuleCollider))
			{
				_capsuleCollider = capsuleCollider;
			}
		}

		public void FixedUpdate()
		{
			if (Physics.Raycast(_raycastStartPos, -transform.up, out _hit, raycastLength, groundLayers))
			{
				surfNorm = _hit.normal;
				surfNormAngle = Vector3.Angle(transform.up, _hit.normal);

				if (surfNormAngle < maxGroundedAngle)
				{
					rayGrounded = true;
				}
				else
				{
					rayGrounded = false;
				}
			}
			else
			{
				rayGrounded = false;
			}
		}
		private void OnCollisionStay(Collision collision)
		{
			bool cancellingGrounded = true;

			if (1 << collision.transform.gameObject.layer == groundLayers)
			{
				colGrounded = true;
				cancellingGrounded = false;
				CancelInvoke(nameof(StopColGrounded));
			}

			if (!cancellingGrounded)
			{
				cancellingGrounded = true;
				Invoke(nameof(StopColGrounded), Time.deltaTime * 3f);//Running the invoke for 3 frames in advance to stop grounded incase the method doesnt run in the future;
			}
		}
		private void StopColGrounded()
		{
			colGrounded = false;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = rayGrounded ? Color.red : Color.white;
			
			Gizmos.DrawRay(_raycastStartPos, -transform.up * raycastLength);
		}

		private Vector3 _raycastStartPos
		{
			get
			{
				if (_capsuleCollider)
				{
					return transform.position + _capsuleCollider.center - transform.up * (_capsuleCollider.height / 2f - _capsuleCollider.radius);
				}
				else
				{
					return transform.position;
				}
			}
		}
	}
}
