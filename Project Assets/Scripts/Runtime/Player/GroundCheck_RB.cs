using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GroundCheck_RB : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public LayerMask groundLayers;
        [Space(15)]
        [Range(0f, 90f)] public float maxGroundedAngle = 80f;
        [Space(15)]
        public bool isGrounded;
        [Space(15)]
        public Vector3 surfNorm = Vector3.zero;
        public float surfNormAngle = 0f;

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (transform.TryGetComponent(out Rigidbody rigidbody))
			{
				_rigidbody = rigidbody;
			}
		}

		private void OnCollisionStay(Collision collision)
        {
            if (1 << collision.gameObject.layer != groundLayers || !_rigidbody) { return; }

            bool cancellingGrounded = true;

            foreach (ContactPoint contactPoint in collision.contacts)
			{
                surfNorm = contactPoint.normal;
                surfNormAngle = Vector3.Angle(surfNorm, transform.up);

                if (surfNormAngle < maxGroundedAngle)
				{
                    isGrounded = true;
                    cancellingGrounded = false;
                    CancelInvoke(nameof(StopGrounded));
                }
			}

            if (!cancellingGrounded)
            {
                cancellingGrounded = true;
                Invoke(nameof(StopGrounded), Time.deltaTime * 3f);//Running the invoke for 3 frames in advance to stop grounded incase the method doesnt run in the future;
            }
        }

        private void StopGrounded()
        {
            isGrounded = false;
        }
	}
}
