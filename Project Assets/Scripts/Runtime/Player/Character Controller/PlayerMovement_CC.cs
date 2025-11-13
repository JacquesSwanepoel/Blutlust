using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlayerMovement_CC : MonoBehaviour
	{
		[SerializeField] private PlayerInputObject _playerInput = null;

		public Transform cameraRollParent = null;

		private Transform _parentCollider = null;

		private GroundCheck_CC _groundCheck = null;

		private CharacterController _charController;

		public CustomCC _customCC;

		[SerializeField] private LayerMask _groundLayers;

		[Header("Acceleration")]
		public bool simultaneousAccDec = true;
		public float groundAcceleration = 5f;
		public float airAcceleration = 5f;
		[Header("Deceleration")]
		public float minSpeed = 0.01f;
		public float groundDeceleration = 5f;
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
		public float crouchCameraPos = 0f;
		public float crouchColSize = 1f;
		public bool toggleCrouch = true;
		[Space(10)]
		public bool useUnCrouchCheck = true;
		public LayerMask unCrouchBlockingLayers;
		[Header("Gravity")]		
		[Range(0f, 90f)] public float autoGroundedGravityAngle = 80f;
		public float groundedGravity = 20f;
		public float airborneGravity = 20f;
		public float jumpedGravity = 10f;
		public float gravityAcceleration = 5f;
		public float gravityTerminalSpeed = 15f;
		[Header("Slopes")]
		public float maxSlopeGripAngle = 40f;
		public float maxSlopeAngle = 89f;
		[Header("Slide Boost")]
		public float slideSpeed = 40f;
		public float slideResetDelay = 1f;
		[Header("Other Variables")]
		public bool noInputGrip = true;
		public float noInputGripAmount = 1f;
		[Space(15)]
		public bool debug = true;
		[Space(15)]

		internal Vector3 _moveXZ;
		internal Vector3 _moveY;

		private Vector2 _moveInput = Vector2.zero;
		private float _jumpInput = 0f;
		private float _crouchInput = 0f;

		private float _crouchTime = 0f;

		private float _currentGroundDrag;
		private float _currentAirDrag;
		private Vector3 _gravityDir = new Vector3(0f, -2f, 0f);

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
		private Vector3 _initialCamPos = Vector3.zero;
		private Vector3 _initialccCenter = Vector3.zero;
		private float _initialccHeight = 2f;

		internal Vector3 _lossyVelocity;
		internal Vector3 _localVelocity;
		internal Vector3 _currentLateralDrag;
		internal Vector3 _currentVerticalDrag;

		[Serializable]
		private class DebugVariables
		{
			public bool isGrounded = false;
			public bool isCrouching = false;
			public bool isOnSlope = false;
			public bool isInputtingMovement = false;
			public bool groundJumped = false;
			public bool canSlide = true;
			public bool slideGrounded = false;

			[Space(20)]

			public bool canUncrouch = false;

			[Space(20)]

			public int currentJumpIndex;

			[Space(20)]

			public int LDJump;
			public int LDCrouchToggleSub;
			public int LDCrouchToggleMaster;
			public int LDCrouchForce;
			public int LDSlide;
			public int LDGrip;
			public int LDNoGrip;
			public int LDGripDisabled;

			[Space(20)]

			public Vector2 moveInput;

			[Space(20)]

			public Vector3 localVelocity;
			public Vector3 gravityDir;
			public float groundNormalDifference;
		}

		[SerializeField] private DebugVariables debugVariables;

		private void OnValidate()
		{
			if (cameraRollParent)
				_initialCamPos = cameraRollParent.localPosition;

			if (transform.parent.TryGetComponent(out CustomCC _custom) && !_customCC)
			{
				_customCC = _custom;
			}

			if (transform.parent.TryGetComponent(out CharacterController _char) && !_charController)
			{
				_charController = _char;
			}
		}

		private void Debug()
		{
			if (!debug) { return; }

			debugVariables.isGrounded = _isGrounded;
			debugVariables.isCrouching = _isCrouching;
			debugVariables.isOnSlope = _isOnSlope;
			debugVariables.isInputtingMovement = _isInputtingMovement;
			debugVariables.groundJumped = _groundJumped;

			debugVariables.currentJumpIndex = _currentJumpIndex;

			debugVariables.LDJump = _LDJump;
			debugVariables.LDCrouchToggleSub = _LDCrouchToggleSub;
			debugVariables.LDCrouchToggleMaster = _LDCrouchToggleMaster;
			debugVariables.LDCrouchForce = _LDCrouchForce;
			debugVariables.LDSlide = _LDSlide;
			debugVariables.LDGrip = _LDGrip;
			debugVariables.LDNoGrip = _LDNoGrip;
			debugVariables.LDGripDisabled = _LDGripDisabled;

			debugVariables.moveInput = _moveInput;

			debugVariables.localVelocity = _moveXZ;
			debugVariables.gravityDir = _gravityDir;
			debugVariables.groundNormalDifference = _groundNormalDifference;
		}

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (transform.parent)
			{
				_parentCollider = transform.parent;
			}

			if (_parentCollider)
			{
				_charController = _parentCollider.GetComponent<CharacterController>();
				_groundCheck = _parentCollider.GetComponent<GroundCheck_CC>();
				_customCC = _parentCollider.GetComponent<CustomCC>();
			}

			if (_customCC)
			{
				_initialccCenter = _customCC.center;
				_initialccHeight = _customCC.height;
			}

			if (cameraRollParent)
				_initialCamPos = cameraRollParent.localPosition;

			_currentJumpIndex = jumpAmount;
		}

		private void Update()
		{
			Debug();

			Input();
		}

		private void FixedUpdate()
		{
			_lossyVelocity = _lossyVelocityCalc;
			_localVelocity = _localVelocityCalc;

			Move();

			Gravity();

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

			_jumpInput = _playerInput._inputActions.Gameplay.Jump.ReadValue<float>();

			_crouchInput = _playerInput._inputActions.Gameplay.Crouch.ReadValue<float>();
		}

		private void Move()
		{
			if (_charController == null || _playerInput == null) { return; }

			Vector3 MasterMove = Vector3.Scale(_moveXZ, _lossyVelocity) + _moveY;

			print(MasterMove);

			_charController.Move(MasterMove * Time.deltaTime);

			_isInputtingMovement = _moveInput == Vector2.zero ? false : true;

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

			if (_isGrounded)//Applies Acc & Dec if grounded
			{
				if (applyDeceleration)
				{
					_moveXZ -= _moveXZ.normalized * groundDeceleration / 100f;
				}

				if (_isInputtingMovement)
				{
					_moveXZ += (MoveX + MoveY) * groundAcceleration / 100f;
				}

				_moveXZ = Vector3.ClampMagnitude(_moveXZ, groundTerminalSpeed);
			}
			else//Applies Acc & Dec if airborne
			{
				if (applyDeceleration)
				{
					_moveXZ -= _moveXZ.normalized * airDeceleration / 100f;
				}

				if (_isInputtingMovement)
				{
					_moveXZ += (MoveX + MoveY) * airAcceleration / 100f;
				}

				_moveXZ = Vector3.ClampMagnitude(_moveXZ, airTerminalSpeed);
			}

			if (applyDeceleration && _moveXZ.magnitude < minSpeed)//Removes any tiny movements generated by Dec
			{
				_moveXZ = Vector3.zero;
			}
		}

		private void Jump()
		{
			if (_charController == null || _playerInput == null || !_canJump) { return; }

			if (_isGrounded && _jumpInput == 1f && _LDJump == 0 && !bunnyHop && _currentJumpIndex > 0)
			{
				_moveY += transform.up * jumpForce;

				_canJump = false;
				_groundJumped = true;

				Invoke(nameof(EnableCanJump), Time.deltaTime * 0.1f);

				_currentJumpIndex = 0;
				_LDCrouchToggleMaster = 0;
				_LDCrouchToggleSub = 0;
				_LDJump = 1;
			}
			else if (_isGrounded && _jumpInput == 0f && _LDJump == 1)
			{
				_LDJump = 0;
			}

			if (_isGrounded && _jumpInput == 1f && bunnyHop && _currentJumpIndex > 0)
			{
				if (_LDJump == 0) { _LDJump++; }

				_moveY = -transform.up * jumpedGravity;
				_moveY += transform.up * jumpForce;

				_canJump = false;
				_groundJumped = true;

				Invoke(nameof(EnableCanJump), 0.1f);

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
				if (_crouchInput == 1f && _LDCrouchToggleSub == 0 && _LDCrouchToggleMaster == 0 && _isGrounded)
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

			if (_charController == null) { return; }

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
			if (!_customCC) { return; }

			_customCC.height = Mathf.Lerp(_initialccHeight, crouchColSize, _crouchTime);
			_customCC.center = Vector3.Lerp(_initialccCenter, _crouchColliderPos, _crouchTime);
			cameraRollParent.localPosition = Vector3.Lerp(_initialCamPos, _initialCamPos - (transform.up * crouchCameraPos), _crouchTime);

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
			if (!_customCC || !_canUnCrouch) { return; }

			_customCC.height = Mathf.Lerp(_initialccHeight, crouchColSize, _crouchTime);
			_customCC.center = Vector3.Lerp(_initialccCenter, _crouchColliderPos, _crouchTime);
			cameraRollParent.localPosition = Vector3.Lerp(_initialCamPos, _initialCamPos - (transform.up * crouchCameraPos), _crouchTime);

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
			if (_charController == null) { return; }

			if (_isGrounded && _canJump && _groundJumped)
			{
				_groundJumped = false;
			}

			if (_isGrounded && !_groundJumped)
			{
				_LDGravity = 0;

				if (_groundCheck)
				{
					//Vector3 lossyLatVel = transform.forward * 20f;           //Here just to test things
					Vector3 lossyLatVel = new Vector3(_lossyVelocity.x, 0f, _lossyVelocity.z);
					float latVelNormalAngle = Vector3.Angle(_groundCheck._surfNorm, lossyLatVel);
					latVelNormalAngle = 90f - latVelNormalAngle;

					if (latVelNormalAngle > 0f && latVelNormalAngle < 90f)
					{
						float rad = latVelNormalAngle * Mathf.Deg2Rad;

						float force = groundTerminalSpeed * Mathf.Tan(rad);		//Using the terminal speed intead of the Vel Magnitude rather than increasing groundcheck raylength offset

						if (force > groundedGravity)
						{
							_moveY = -transform.up * force * 1.5f; //Multiplying because the defualt amount isnt enough
						}
						else
						{
							_moveY = -transform.up * groundedGravity;
						}
					}
					else
					{
						_moveY = -transform.up * groundedGravity;
					}
				}
				else
				{
					_moveY = -transform.up * groundedGravity;
				}
			}
			else
			{
				if (_LDGravity == 0 && !_groundJumped)
				{
					_LDGravity++;

					_moveY = -transform.up * airborneGravity;
				}

				_moveY += -transform.up * Time.deltaTime * gravityAcceleration;
			}

			if (_moveY.magnitude > gravityTerminalSpeed && !_isGrounded)//Caps moveY to angular velocity
			{
				_moveY /= _moveY.magnitude;
				_moveY *= gravityTerminalSpeed;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;

			if (_customCC)
			{
				if (_charController)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawWireSphere(transform.parent.position + _crouchColliderPos + (transform.up * (crouchColSize / 2f - _charController.radius)), _customCC.radius);
				}

				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(transform.position + _initialCamPos - (transform.up * crouchCameraPos), _customCC.radius * 0.8f);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(_unCrouchRayStartPos, _unCrouchRayEndPos);
			}
		}

		private bool _isGrounded
		{
			get
			{
				if (_groundCheck)
				{
					return _groundCheck._isGrounded;
				}
				else
				{
					return true;
				}
			}
		}
		private bool _isOnSlope
		{
			get
			{
				if (_groundCheck)
				{
					return _groundCheck._isOnSlope;
				}
				else
				{
					return false;
				}
			}
		}
		private float _groundNormalDifference
		{
			get
			{
				if (_groundCheck)
				{
					return _groundCheck._surfNormAngle;
				}
				else
				{
					return 0f;
				}
			}
		}

		private Vector3 _crouchColliderPos
		{
			get
			{
				if (_customCC)
				{
					return _initialccCenter - (transform.up * crouchColSize / 2f);
				}
				else
				{
					return Vector3.zero;
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
		private Vector3 _unCrouchRayStartPos
		{
			get
			{
				if (_customCC)
				{
					//return transform.parent.position + _crouchColliderPos + (transform.up * (crouchColSize / 2f));
					return transform.parent.position + _crouchColliderPos + (transform.up * (crouchColSize / 2f)) - transform.up * _customCC.radius;
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
				if (_customCC)
				{
					return transform.parent.position + _initialccCenter + (transform.up * (_initialccHeight / 2f));
				}
				else
				{
					return Vector3.zero;
				}
			}
		}

		private Vector3 oldPos = Vector3.zero;
		private Vector3 _lossyVelocityCalc
		{
			get
			{
				if (_parentCollider)
				{
					Vector3 oldoldPos = oldPos;

					oldPos = _parentCollider.position;

					return (_parentCollider.position - oldoldPos) / Time.deltaTime;
				}
				else
				{
					return Vector3.zero;
				}
			}
		}

		public Vector3 _localVelocityCalc
		{
			get
			{
				return transform.InverseTransformDirection(_lossyVelocity);
			}
		}
	}
}