using UnityEngine;
using TMPro;

namespace Game {
    public class TextDebug : MonoBehaviour
    {
        [SerializeField] private WeaponSwitcher _weaponSwitcher;
        [Space(10)]
        [SerializeField] private TMP_Text _console;

        private string _currentFPS = "Place Holder Text";
        private string _averageFPS = "Place Holder Text";
        private string _timer = "Place Holder Text";
        private string _fixedDeltaTime = "Place Holder Text";
        private string _deltaTime = "Place Holder Text";

        private string _playerWorldPos = "Place Holder Text";
        private string _playerWorldRot = "Place Holder Text";

        private string _playerMoveInput = "Place Holder Text";
        private string _playerLookInput = "Place Holder Text";

        private string _velocity = "Place Holder Text";
        private string _velocityMagnitude = "Place Holder Text";

        private string _isGrounded = "Place Holder Text";

        private string _lastObjectHit = "Place Holder Text";
        private string _currentWeaponSpread = "Place Holder Text";

        private string _surfaceNormalAngle = "Place Holder Text";

        [SerializeField] private float _hudRefreshRate = 1f;

        private Transform _playerController = null;
        private Transform _playerCollider = null;
        private Transform _cameraParent = null;

        private float _fpstimer;

        private float _timerTime = 0f;

        private int _frameTotal = 0;
        private int _cycleCount = 1;

        ////////////////////////////
        public int NPR = 18;
        public int VPR = 6;
        ////////////////////////////

        private void Awake()
		{
            _playerController = GameObject.FindWithTag("Player Controller").transform;
            _playerCollider = GameObject.FindWithTag("Player Collider").transform;

            _cameraParent = Camera.main.transform.parent;
        }

		private void Update()
        {
            if (Time.unscaledTime > _fpstimer)
            {
                CustomUpdate();
            }

            UpdateConsole();
        }

		private void OnValidate()
		{
            UpdateConsole();
        }

		private void CustomUpdate()
		{
            _fpstimer = Time.unscaledTime + _hudRefreshRate;

            int fps = (int)(1f / Time.unscaledDeltaTime);

            _frameTotal += fps;

            _currentFPS = GenerateValueString("FPS: ", fps, NPR, "F0");
            ////
            _cycleCount++;

            _averageFPS = GenerateValueString("Average: ", (_frameTotal / _cycleCount), NPR, "F0");
            ////
            if (_frameTotal > 100000f)
            {
                _frameTotal = 0;
                _cycleCount = 1;
            }

            _timerTime += Time.deltaTime;

            _timer = GenerateValueString("Time: ", _timerTime, NPR, "F4");
            ////
            _fixedDeltaTime = GenerateValueString("Fixed deltaTime: ", Time.fixedDeltaTime, NPR, "F4");
            ////
            _deltaTime = GenerateValueString("deltaTime: ", Time.deltaTime, NPR, "F4");
            ////
            if (_playerCollider)
			{
                _playerWorldPos = GenerateVector3String("World Pos: ", _playerCollider.position, NPR, VPR, "F1");
            ////
                _playerWorldRot = GenerateVector3String("World Rot: ", _cameraParent.rotation.eulerAngles, NPR, VPR, "F1");
            }
            ////
            if (_playerController.TryGetComponent(out PlayerInputListener playerInput))
			{
                float xInput = playerInput._playerInput._inputActions.Gameplay.Right.ReadValue<float>() - playerInput._playerInput._inputActions.Gameplay.Left.ReadValue<float>();
                float yInput = playerInput._playerInput._inputActions.Gameplay.Forward.ReadValue<float>() - playerInput._playerInput._inputActions.Gameplay.Backward.ReadValue<float>();

                Vector2 moveInput = new Vector2(xInput, yInput);

                _playerMoveInput = GenerateVector3String("Move Input: ", moveInput, NPR, VPR, "F0");
            ////
                float lookXInput = playerInput._playerInput._inputActions.Gameplay.LookX.ReadValue<float>();
                float lookYInput = playerInput._playerInput._inputActions.Gameplay.LookY.ReadValue<float>();

                Vector2 lookInput = new Vector2(lookXInput, lookYInput);

                _playerLookInput = GenerateVector3String("Look Input: ", lookInput, NPR, VPR, "F0");
            }
            ////
            if (_playerController.TryGetComponent(out PlayerMovement_RB playerMovement))
            {
                _velocity = GenerateVector3String("Velocity: ", playerMovement._localVelocity, NPR, VPR, "F3");
            ////     
                _velocityMagnitude = GenerateValueString("Velocity Mag: ", playerMovement._localVelocity.magnitude, NPR, "F3");
            }
            ////
            if (_weaponSwitcher.childCount > 0)
            {
                Transform currentWeapon = _weaponSwitcher.transform.GetChild(_weaponSwitcher._weaponIndex);

                if (currentWeapon.TryGetComponent(out WeaponBaseScript weaponScript))
                {
                    _lastObjectHit = "Last Object Hit: ".PadRight(NPR, ' ') + weaponScript._hitName;
                    _lastObjectHit = GenerateString("Last Object Hit: ", weaponScript._hitName, NPR);
            ////
                    if (weaponScript.transform.TryGetComponent(out WeaponSpreadScript weaponSpreadScript))
                    {
                        _currentWeaponSpread = GenerateValueString("Current Weapon Spread: ", weaponSpreadScript._currentSpreadAmount, NPR, "F2");
                    }
                }
            }
            ////
            if (_playerCollider && _playerCollider.TryGetComponent(out GroundCheck_Ray groundCheck))
            {
                _isGrounded = GenerateString("Is Grounded: ", groundCheck.rayGrounded.ToString(), NPR);

            ////
                _surfaceNormalAngle = GenerateValueString("Surface Normal Angle: ", groundCheck.surfNormAngle, NPR, "F2");
            }
        }

        private string GenerateString(string name, string value, int namePad)
        {
            return name.PadRight(namePad, ' ') + value;
        }

        private string GenerateValueString(string name, float value, int namePad, string precision)
        {
            return name.PadRight(namePad, ' ') + value.ToString(precision);
        }

        private string GenerateVector3String(string name, Vector3 vector3, int namePad, int valuePad, string precision)
		{
            return name.PadRight(namePad, ' ') + vector3.x.ToString(precision).PadRight(valuePad, ' ') + "|" +
                       vector3.y.ToString(precision).PadRight(valuePad, ' ') + "|" +
                       vector3.z.ToString(precision).PadRight(valuePad, ' ');
		}

        private void UpdateConsole()
		{
            if (!_console) { return; }

            _console.text =
            _currentFPS + "\n"
            + _averageFPS + "\n"
            + _timer + "\n"
            + _fixedDeltaTime + "\n"
            + _deltaTime + "\n"
            + "\n"
            + _playerWorldPos + "\n"
            + _playerWorldRot + "\n"
            + "\n"
            + _playerMoveInput + "\n"
            + _playerLookInput + "\n"
            + "\n"
            + _velocity + "\n"
            + _velocityMagnitude + "\n"
            + "\n"
            + _isGrounded + "\n"
            + "\n"
            + _lastObjectHit + "\n"
            + _currentWeaponSpread + "\n"
            + "\n"
            + _surfaceNormalAngle;
		}
    }
}