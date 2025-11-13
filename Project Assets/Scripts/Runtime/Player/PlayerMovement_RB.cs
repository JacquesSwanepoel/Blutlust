using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlayerMovement_RB : MonoBehaviour
	{
		[SerializeField] private PlayerInputObject _playerInput = null;

		public Transform cameraRollParent = null;

		private Transform _playerCollider = null;

		private GroundCheck_Ray _groundCheck;

		private Rigidbody _rigidbody;

		private CapsuleCollider _capsuleCollider;

		[Header("Acceleration")]
		public bool simultaneousAccDec = true;
		public float groundAcceleration = 100f;
		public float airAcceleration = 65f;
		[Header("Deceleration")]
		public bool autoMinSpeed = true;
		public float minSpeed = 1f;
		public float groundDeceleration = 50f;
		public float airDeceleration = 5f;
		[Header("Terminal Speeds")]
		public float groundTerminalSpeed = 20f;
		public float airTerminalSpeed = 20f;
		public float crouchTerminalSpeed = 10f;
		[Header("Jump")]
		public int jumpAmount = 1;
		public float jumpForce = 4f;
		public float airJumpForce = 4f;
		public float airJumpInfluenceAmount = 4f;
		public bool bunnyHop = false;
		public bool resetVelocityOnAirJump = true;
		[Header("Crouch")]
		public float crouchSpeed = 10f;
		public float crouchCameraPos = 1f;
		public float crouchColliderHeight = 1f;
		public bool toggleCrouch = true;
		[Space(10)]
		public bool useUnCrouchCheck = true;
		public LayerMask unCrouchBlockingLayers;
		[Header("Gravity")]
		public float groundedGravity = 1f;
		public float airborneGravity = 1f;
		public float gravityAcceleration = 5f;
		public float gravityTerminalSpeed = 15f;
		[Header("Slide")]
		public float slideSpeed = 40f;
		public float slideResetDelay = 1f;

		private Vector2 _moveInput = Vector2.zero;
		private float _jumpInput = 0f;
		private float _crouchInput = 0f;

		private Vector3 _lossyMoveInput;
		private Vector3 _lossyMoveInput_right;
		private float _relativeSurfaceAngle;
		private Vector3 _moveDir;

		private float _crouchTime = 0f;

		private bool _isInputtingMovement = false;
		internal bool _isCrouching = false;

		private bool _canJump = true;
		private bool _groundJumped = false;

		private int _LDJump = 0;
		private int _LDCrouchToggleSub = 0;
		private int _LDCrouchToggleMaster = 0;
		private int _LDCrouchForce = 0;
		private int _LDSlide = 0;
		private int _LDGravity = 0;
		private int _LDGrip = 0;
		private int _LDNoGrip = 0;
		private int _LDGripDisabled = 0;

		private int _currentJumpIndex;
		private Vector3 _initialCamLocalPos = Vector3.zero;
		private Vector3 _primeColliderCenter = Vector3.zero;
		private float _primeColliderHeight = 2f;

		internal Vector3 _lossyVelocity;
		internal Vector3 _localVelocity;

		private Vector3 _lateralLocVelocity;
		private Vector3 _verticleLocVelocity;

		private void OnValidate()
		{
			if (cameraRollParent)
			{
				_initialCamLocalPos = cameraRollParent.localPosition;
			}

			if (GameObject.FindGameObjectWithTag("Player Collider") && !_playerCollider)
			{
				_playerCollider = GameObject.FindGameObjectWithTag("Player Collider").transform;
			}

			if (_playerCollider.TryGetComponent(out Rigidbody rigidbody) && !_rigidbody)
			{
				_rigidbody = rigidbody;
			}

			if (_playerCollider.TryGetComponent(out CapsuleCollider capsuleCollider) && !_capsuleCollider)
			{
				_capsuleCollider = capsuleCollider;
			}

			GeneralValidate();
		}

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (GameObject.FindGameObjectWithTag("Player Collider"))
			{
				_playerCollider = GameObject.FindGameObjectWithTag("Player Collider").transform;
			}

			if (_playerCollider)
			{
				if (_playerCollider.TryGetComponent(out Rigidbody rigidbody))
				{
					_rigidbody = rigidbody;
				}
				if (_playerCollider.TryGetComponent(out GroundCheck_Ray groundCheckRay))
				{
					_groundCheck = groundCheckRay;
				}
				if (_playerCollider.TryGetComponent(out CapsuleCollider capsuleCollider))
				{
					_capsuleCollider = capsuleCollider;
				}
			}

			if (_capsuleCollider)
			{
				_primeColliderCenter = _capsuleCollider.center;
				_primeColliderHeight = _capsuleCollider.height;
			}

			if (cameraRollParent)
			{
				_initialCamLocalPos = cameraRollParent.localPosition;
			}

			_currentJumpIndex = jumpAmount;
		}

		public void GeneralValidate()
		{
			if (autoMinSpeed)
			{
				float highest = groundAcceleration > airAcceleration ? groundAcceleration : airAcceleration;

				minSpeed = highest / 100f;
				minSpeed *= 1.1f;
			}
		}

		private void Update()
		{
			Input();

			if (_rigidbody)
			{
				_lossyVelocity = _rigidbody.linearVelocity;
				_localVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);

				_lateralLocVelocity = new Vector3(_localVelocity.x, 0f, _localVelocity.z);
				_verticleLocVelocity = new Vector3(0f, _localVelocity.y, 0f);
			}

			GeneralValidate();
		}

		private void FixedUpdate()
		{
			Gravity();

			Move();

			Jump();

			CrouchManager();
		}

		private void Input()
		{
			if (_playerInput == null) { return; }

			float xInput = _playerInput._inputActions.Gameplay.Right.ReadValue<float>() - _playerInput._inputActions.Gameplay.Left.ReadValue<float>();
			float yInput = _playerInput._inputActions.Gameplay.Forward.ReadValue<float>() - _playerInput._inputActions.Gameplay.Backward.ReadValue<float>();

			_moveInput = new Vector2(xInput, yInput);

			if (_moveInput.magnitude > 1f)
			{
				_moveInput /= _moveInput.magnitude;
			}

			_lossyMoveInput = transform.TransformDirection(new Vector3(_moveInput.x, 0f, _moveInput.y));

			_isInputtingMovement = _moveInput == Vector2.zero ? false : true;
			////
			_jumpInput = _playerInput._inputActions.Gameplay.Jump.ReadValue<float>();
			////
			_crouchInput = _playerInput._inputActions.Gameplay.Crouch.ReadValue<float>();
		}

		private void Move()
		{
			if (_rigidbody == null || _playerInput == null) { return; }

			Vector3 MoveY = transform.forward * _moveInput.y;
			Vector3 MoveX = transform.right * _moveInput.x;

			bool applyDeceleration = false;

			if (!simultaneousAccDec)//Works out when to apply Deceleration
			{
				if (!_isInputtingMovement)
				{
					applyDeceleration = true;
				}
			}
			else
			{
				applyDeceleration = true;
			}

			if (_rayGrounded)//Applies Acc & Dec if grounded
			{
				if (_groundCheck)//Fancy Pants calculations, look at the gizmos and youll understand
				{
					_lossyMoveInput_right = Quaternion.Euler(0f, 90f, 0f) * _lossyMoveInput;
					_moveDir = Vector3.ProjectOnPlane(_groundCheck.surfNorm, _lossyMoveInput_right).normalized;
					_moveDir = Quaternion.AngleAxis(90f, _lossyMoveInput_right) * _moveDir;

					if (_isInputtingMovement)
					{
						_rigidbody.linearVelocity += _moveDir * groundAcceleration / 100f;

						if (_rigidbody.linearVelocity.magnitude > groundTerminalSpeed)//Capping speed to terminal speed
						{
							Vector3 OGVelocity = _rigidbody.linearVelocity;

							_rigidbody.linearVelocity = Vector3.zero;
							_rigidbody.linearVelocity = OGVelocity.normalized * groundTerminalSpeed;
						}
					}
					if (applyDeceleration)
					{
						_rigidbody.linearVelocity -= _rigidbody.linearVelocity.normalized * groundDeceleration / 100f;
					}

					_rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, _groundCheck.surfNorm);
				}
			}
			else//Applies Acc & Dec if airborne
			{
				
			}

			if (applyDeceleration && _rigidbody.linearVelocity.magnitude < minSpeed)//Removes any tiny movements generated by Dec
			{
				Vector3 clampedVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);//A velocity clamping system that actually works
				clampedVelocity.x = 0f;
				clampedVelocity.z = 0f;

				_rigidbody.linearVelocity = transform.TransformDirection(clampedVelocity);
			}
		}

		private void Jump()
		{
			if (_rigidbody == null || _playerInput == null || !_canJump) { return; }

			if (_rayGrounded && _jumpInput == 1f && _LDJump == 0 && !bunnyHop && _currentJumpIndex > 0)
			{
				_rigidbody.linearVelocity -= transform.TransformDirection(_verticleLocVelocity);//Absolute verticle velocity is set to Vector3.zero and jump force is added
				_rigidbody.linearVelocity += transform.TransformDirection(Vector3.up * jumpForce);

				_canJump = false;
				_groundJumped = true;

				Invoke(nameof(EnableCanJump), Time.deltaTime * 5f);

				_currentJumpIndex = 0;
				_LDCrouchToggleMaster = 0;
				_LDCrouchToggleSub = 0;
				_LDJump = 1;
			}
			else if (_rayGrounded && _jumpInput == 0f && _LDJump == 1)
			{
				_LDJump = 0;
				_currentJumpIndex = jumpAmount;
			}

			if (_rayGrounded && _jumpInput == 1f && bunnyHop && _currentJumpIndex > 0)
			{
				if (_LDJump == 0) { _LDJump++; }

				_rigidbody.linearVelocity -= transform.TransformDirection(_verticleLocVelocity);//Absolute verticle velocity is set to Vector3.zero and jump force is added
				_rigidbody.linearVelocity += transform.TransformDirection(Vector3.up * jumpForce);

				_canJump = false;
				_groundJumped = true;

				Invoke(nameof(EnableCanJump), Time.deltaTime * 10f);

				_currentJumpIndex = jumpAmount - 1;
				_LDCrouchToggleMaster = 0;
				_LDCrouchToggleSub = 0;
			}
		}

		private void EnableCanJump()
		{
			_canJump = true;
		}

		private void CrouchManager()
		{
			if (_playerInput == null || cameraRollParent == null) { return; }

			if (!toggleCrouch)
			{
				if (_crouchInput == 1f)
				{
					CrouchDown();
				}
				else if (_crouchInput == 0f)
				{
					CrouchUp();
				}
			}
			else
			{
				if (_crouchInput == 1f && _LDCrouchToggleSub == 0 && _LDCrouchToggleMaster == 0 && _rayGrounded)
				{
					_LDCrouchToggleSub = 1;
					_LDCrouchToggleMaster = 1;
				}
				else if (_crouchInput == 0f && _LDCrouchToggleSub == 1 && _LDCrouchToggleMaster == 1)
				{
					_LDCrouchToggleSub = 0;
				}
				else if (_crouchInput == 1f && _LDCrouchToggleSub == 0 && _LDCrouchToggleMaster == 1)
				{
					_LDCrouchToggleSub = 1;
					_LDCrouchToggleMaster = 0;
				}
				else if (_crouchInput == 0f && _LDCrouchToggleSub == 1 && _LDCrouchToggleMaster == 0)
				{
					_LDCrouchToggleSub = 0;
				}

				if (_LDCrouchToggleMaster == 1f)
				{
					CrouchDown();
				}
				else if (_LDCrouchToggleMaster == 0f)
				{
					CrouchUp();
				}
			}

			if (_rigidbody == null) { return; }

			if (_crouchInput == 1f && _LDCrouchForce == 0)
			{
				//_move += -transform.up;

				_LDCrouchForce++;
			}
			else if (_crouchInput == 0f && _LDCrouchForce == 1)
			{
				_LDCrouchForce--;
			}
		}

		private void CrouchDown()
		{
			if (!_capsuleCollider) { return; }

			_capsuleCollider.height = Mathf.Lerp(_primeColliderHeight, crouchColliderHeight, _crouchTime);
			_capsuleCollider.center = Vector3.Lerp(_primeColliderCenter, _crouchColliderCenter, _crouchTime);
			cameraRollParent.localPosition = Vector3.Lerp(_initialCamLocalPos, _initialCamLocalPos - (Vector3.up * crouchCameraPos), _crouchTime);//Using Vector3.up because it's in local space

			if (_crouchTime < 1f)
			{
				_crouchTime += Time.deltaTime * crouchSpeed;
			}
			else
			{
				_crouchTime = 1f;
			}

			_isCrouching = true;
		}

		private void CrouchUp()
		{
			if (!_capsuleCollider || !_canUnCrouch) { return; }

			_capsuleCollider.height = Mathf.Lerp(_primeColliderHeight, crouchColliderHeight, _crouchTime);
			_capsuleCollider.center = Vector3.Lerp(_primeColliderCenter, _crouchColliderCenter, _crouchTime);
			cameraRollParent.localPosition = Vector3.Lerp(_initialCamLocalPos, _initialCamLocalPos - (Vector3.up * crouchCameraPos), _crouchTime);//Using Vector3.up because it's in local space

			if (_crouchTime > 0f)
			{
				_crouchTime -= Time.deltaTime * crouchSpeed;
			}
			else
			{
				_crouchTime = 0f;
			}

			_isCrouching = false;
		}

		private void Gravity()
		{
			if (_rigidbody == null) { return; }

			if (_rayGrounded && _canJump && _groundJumped)
			{
				_groundJumped = false;
			}

			if (_rayGrounded && !_groundJumped && !_colGrounded)//Just on the ground
			{
				_LDGravity = 0;

				//_rigidbody.AddForce(-transform.up * groundedGravity, ForceMode.Acceleration);
				_rigidbody.MovePosition(transform.position - transform.up * groundedGravity / 100f);
			}
			else if (!_rayGrounded)//Airborne, by falling or jumping
			{
				if (_LDGravity == 0 && !_groundJumped)//Faling, set gravity to falling gravity
				{
					_LDGravity++;

					_rigidbody.AddForce(-transform.up * airborneGravity, ForceMode.Acceleration);
				}

				_rigidbody.linearVelocity -= transform.up * gravityAcceleration * Time.deltaTime;
			}

			Vector3 clampedVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);//Takes CURRENT velocity, makes it local, messes with it, and changes it back to global
			clampedVelocity.y = Mathf.Clamp(clampedVelocity.y, -gravityTerminalSpeed, gravityTerminalSpeed);
			_rigidbody.linearVelocity = transform.TransformDirection(clampedVelocity);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;

			if (_capsuleCollider && _playerCollider)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(_playerCollider.position + transform.TransformDirection(_crouchColliderCenter) + (transform.up * (crouchColliderHeight / 2f - _capsuleCollider.radius)), _capsuleCollider.radius);//Sphere indicating where the collider's top will be when crouched
				//  /\ Uses TransformDirection because the CapColl center is in local space

				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(_initialCamLocalPos) - (transform.up * crouchCameraPos), _capsuleCollider.radius * 0.8f);//Sphere indicating where the camera will be when crouched
				//  /\ Uses TransformDirection because the _initialCamLocalPos is in local space

				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(_unCrouchRayStartPos, _unCrouchRayEndPos);//Line to indicate the uncrouch raycast

				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(_capsuleColliderTopSpherePos, _capsuleCollider.radius);//Draw top sphere of collider
				Gizmos.DrawWireSphere(_capsuleColliderBottomSpherePos, _capsuleCollider.radius);//Draw bottom sphere of collider
				Gizmos.DrawLine(_capsuleColliderTopSpherePos + _playerCollider.right * _capsuleCollider.radius, _capsuleColliderBottomSpherePos + _playerCollider.right * _capsuleCollider.radius);
				Gizmos.DrawLine(_capsuleColliderTopSpherePos - _playerCollider.right * _capsuleCollider.radius, _capsuleColliderBottomSpherePos - _playerCollider.right * _capsuleCollider.radius);
				Gizmos.DrawLine(_capsuleColliderTopSpherePos + _playerCollider.forward * _capsuleCollider.radius, _capsuleColliderBottomSpherePos + _playerCollider.forward * _capsuleCollider.radius);
				Gizmos.DrawLine(_capsuleColliderTopSpherePos - _playerCollider.forward * _capsuleCollider.radius, _capsuleColliderBottomSpherePos - _playerCollider.forward * _capsuleCollider.radius);
			}

			if (_capsuleCollider)
			{
				Vector3 pos = transform.position;

				//Movement Debug
				Gizmos.color = Color.green;
				Gizmos.DrawRay(pos, _lossyMoveInput * 3f);
				Gizmos.color = Color.red;
				Gizmos.DrawRay(pos - _lossyMoveInput_right * 1.5f, _lossyMoveInput_right * 3f);
				Gizmos.color = Color.white;
				Gizmos.DrawRay(pos, _moveDir * 3f);
				Gizmos.DrawRay(pos, -_moveDir * 3f);

				if (_groundCheck)
				{
					Gizmos.color = Color.black;
					Vector3 test = Vector3.ProjectOnPlane(_groundCheck.surfNorm, _lossyMoveInput_right).normalized;
					Gizmos.DrawRay(pos, test * 3f);
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(pos, _groundCheck.surfNorm * 3f);
				}
			}
		}

		private bool _rayGrounded
		{
			get
			{
				if (_groundCheck)
				{
					if (_groundCheck.rayGrounded)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return true;
				}
			}
		}

		private bool _colGrounded
		{
			get
			{
				if (_groundCheck)
				{
					if (_groundCheck.colGrounded)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return true;
				}
			}
		}
		private bool _canUnCrouch
		{
			get
			{
				return Physics.Linecast(_unCrouchRayStartPos, _unCrouchRayEndPos, unCrouchBlockingLayers) ? false : true;
			}
		}
		private Vector3 _crouchColliderCenter
		{
			get
			{
				if (_capsuleCollider)
				{
					crouchColliderHeight = Mathf.Clamp(crouchColliderHeight, _capsuleCollider.radius * 2f, Mathf.Infinity);

					return _primeColliderCenter - (Vector3.up * _primeColliderHeight / 2f) + (Vector3.up * crouchColliderHeight / 2f);//Use Vector3.up because the CapColl center is in local space
				}
				else
				{
					return Vector3.zero;
				}
			}
		}
		private Vector3 _capsuleColliderTopSpherePos
		{
			get
			{
				if (_capsuleCollider)
				{
					return _playerCollider.position + transform.TransformDirection(_capsuleCollider.center) + transform.up * (_capsuleCollider.height / 2f - _capsuleCollider.radius);//Use TransformDirection because the CapColl center is in local space
				}
				else
				{
					return Vector3.zero;
				}
			}
		}
		private Vector3 _capsuleColliderBottomSpherePos
		{
			get
			{
				if (_capsuleCollider)
				{
					return _playerCollider.position + transform.TransformDirection(_capsuleCollider.center) - transform.up * (_capsuleCollider.height / 2f - _capsuleCollider.radius);//Use TransformDirection because the CapColl center is in local space
				}
				else
				{
					return Vector3.zero;
				}
			}
		}
		private Vector3 _unCrouchRayStartPos
		{
			get
			{
				if (_capsuleCollider && _playerCollider)
				{
					return _playerCollider.position + transform.TransformDirection(_crouchColliderCenter) + transform.up * (crouchColliderHeight / 2f - _capsuleCollider.radius);
					//  /\ Uses TransformDirection because the CapColl center is in local space
				}
				else
				{
					return Vector3.zero;
				}
			}
		}
		private Vector3 _unCrouchRayEndPos
		{
			get
			{
				if (_capsuleCollider && _playerCollider)
				{
					return _playerCollider.position + transform.TransformDirection(_primeColliderCenter) + (transform.up * (_primeColliderHeight / 2f));
					//  /\ Uses TransformDirection because the CapColl center is in local space
				}
				else
				{
					return Vector3.zero;
				}
			}
		}
	}
}