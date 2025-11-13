using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngineInternal;
using UnityEngine.Events;

namespace Game {

	public class WeaponBaseScript : MonoBehaviour
	{
		[SerializeField] private PlayerMovement_CC _playerMovement;
		[SerializeField] private Transform _gunModel;
		[SerializeField] private RuntimeAnimatorController _animController;

		[Space(10)]

		public Transform firePoint;

		[Space(15)]

		public string weaponName;

		[Space(10)]

		public LayerMask hittableLayers;
		public LayerMask enemyLayers;

		[Space(15)]

		public UnityEvents unityEvents;

		[Serializable]
		public class UnityEvents
		{
			public UnityEvent OnFire;
			public UnityEvent OnLateFire;
		}

		[Space(10)]
		[Header("Weapon Modes")]
		public bool semiAuto = false;
		public bool fullAuto = false;
		public bool burstFire = false;
		public bool shotgun = false;
		public bool melee = false;
		[Space(10)]
		[Header("Basic Variables")]
		public int damage = 30;
		public float fireDelay = 0.2f;
		public int range = 2000;
		[Space(10)]
		[Header("Equip Variables")]
		public float unholsterDelay = 0.2f;
		public bool afterUnholsterEnableCanFire = true;
		[Space(10)]
		[Header("Burst Fire Variables")]
		public bool autoBurst = false;
		public float burstFireDelay = 0.1f;
		public int burstSize = 3;
		[Space(10)]
		[Header("Shotgun Fire Variables")]
		public int pelletAmount;
		public bool fullAutoShawty;
		[Header("Melee Attack Variables")]
		public bool autoMeleeAttack = false;
		public bool canCookMelee = false;
		public bool areaAttack = false;
		public float maxAttackAngle;
		[Space(10)]
		[Header("Reticle Variables")]
		public float reticleDefaultSpread = 1f;
		public float reticleSpreadSize = 1.5f;
		public float reticleResetSpeed = 1f;

		private Transform _playerController;
		private Camera _camera;
		private Animator _animator;

		private PlayerInputListener _playerInputListener;
		private EnemyChecker _enemyChecker;
		private WeaponAmmoScript _weaponAmmoScript;
		private Transform _reticle;

		private List<Transform> _enemies = new List<Transform>();
		private List<Transform> _areaAttackAbleEnemies = new List<Transform>();

		private float _fireInput;
		private Vector2 _lookInput;

		private bool _canFire = false;
		internal bool _canSwitch = false;
		private bool _isBursting = false;
		private bool _canHitEnemyWithAreaAttack = false;
		private bool _isUnholstering = false;

		private float _fireTime = 0f;
		private float _burstTime = 0f;
		private float _unholsterTime = 0f;

		private int _LDSemiFire = 0;
		private int _LDShotgunFire = 0;
		private int _LDMeleeAttack = 0;
		private int _LDUnholster = 0;

		private int _currentBurstIndex = 0;

		internal Vector3 _fireDirection;
		internal RaycastHit _hit;
		internal Vector3 _hitPoint;
		internal bool _hitSomething = false;
		internal string _hitName = "--";

		private Image _testFireFlashImage;
		private Transform _impactSphere;

		[Serializable]
		public class DebugVariables
		{
			public bool canFire;
			public bool canSwitch;
			public bool isBursting;
			public bool canHitEnemyWithAreaAttack;
			[Space(15)]
			public float fireTime;
			public float burstTime;
			[Space(15)]
			public int LDSemiFire;
			public int LDCrouchAnim;
			[Space(15)]
			public int currentBurstIndex;
			[Space(15)]
			public Vector3 targetWeapRotation;
			public Vector3 currenWeapRotation;
			[Space(15)]
			public List<Transform> impactSparkPoolList = new List<Transform>();
			public List<Transform> impactHolePoolList = new List<Transform>();
			[Space(15)]
			public Image FireFlashImage;
			public Transform ImpactSphere;
		}

		[Space(20)]

		public bool debug = true;

		[Space(20)]

		public DebugVariables debugVariables;

		private void Debug()
		{
			if (!debug) { return; }

			debugVariables.canFire = _canFire;
			debugVariables.isBursting = _isBursting;
			debugVariables.canHitEnemyWithAreaAttack = _canHitEnemyWithAreaAttack;

			debugVariables.fireTime = _fireTime;
			debugVariables.burstTime = _burstTime;

			debugVariables.LDSemiFire = _LDSemiFire;

			debugVariables.currentBurstIndex = _currentBurstIndex;

			_testFireFlashImage = debugVariables.FireFlashImage;
			_impactSphere = debugVariables.ImpactSphere;
		}

		private void Start()
		{
			Initialize();
		}
		public void Initialize()
		{
			if (_gunModel)
				_animator = _gunModel.GetComponent<Animator>();
			else
			{
				print("Assign gunModel on " + transform.name);
			}

			if (_animator == null && _gunModel)
				_gunModel.gameObject.AddComponent<Animator>();

			if (_animController && _animator)
				_animator.runtimeAnimatorController = _animController;

			_playerController = GameObject.FindGameObjectWithTag("Player Controller").transform;

			if (_playerController)
			{
				if (_playerController.TryGetComponent(out PlayerInputListener playerInputListener)) { _playerInputListener = playerInputListener; }

				if (_playerController.TryGetComponent(out EnemyChecker enemCheckScript)) { _enemyChecker = enemCheckScript; }
			}

			_reticle = GameObject.FindGameObjectWithTag("Reticle").transform;

			if (_reticle.TryGetComponent(out Reticle _reticleScript))
			{
				_reticleScript.defaultSize = reticleDefaultSpread;
				_reticleScript.spreadSize = reticleSpreadSize;
				_reticleScript.resetSpeed = reticleResetSpeed;
			}

			_camera = Camera.main;

			if (_camera)
			{
				_fireDirection = _camera.transform.forward;
			}

			_weaponAmmoScript = transform.GetComponent<WeaponAmmoScript>();

			CheckForEnemies();
		}

		private void Update()
		{
			Debug();

			Input();

			SemiFiringManager();

			FullAutoFiringManager();

			BurstFiringManager();

			ShotgunFiringManager();

			MeleeFiringManager();

			CheckForMeleeableEnemies();
		}
		private void LateUpdate()
		{
			
		}

		private void FixedUpdate()
		{
			FireDelayManager();

			BurstFireDelayManager();

			UnholsterDelayManager();
		}

		private void Input()
		{
			if (!_playerInputListener) { return; }

			_fireInput = _playerInputListener.fireInput;
			_lookInput = _playerInputListener.lookInput;
		}

		private void SemiFiringManager()
		{
			if (_animator == null || _animController == null || fullAuto || burstFire || shotgun || melee) { return; }

			if (_weaponAmmoScript && _weaponAmmoScript.ammo < 1) { return; }

			if (semiAuto && _LDSemiFire == 0 && _fireInput == 1f)
			{
				Fire();

				_canFire = false;
				_canSwitch = false;

				_LDSemiFire++;
			}
			else if (_LDSemiFire == 1 && _fireInput == 0f && _canFire)
			{
				_LDSemiFire--;
			}
		}
		private void FullAutoFiringManager()
		{
			if (_animator == null || _animController == null || semiAuto || burstFire || shotgun || melee) { return; }

			if (_weaponAmmoScript && _weaponAmmoScript.ammo < 1) { return; }

			if (fullAuto && _fireInput == 1f && _canFire)
			{
				Fire();

				_canFire = false;

				_canSwitch = false;
			}
		}
		private void BurstFiringManager()
		{
			if (_animator == null || _animController == null || semiAuto || fullAuto || shotgun || melee) { return; }

			if (_weaponAmmoScript && _weaponAmmoScript.ammo < 1) { return; }

			if (!autoBurst)
			{
				if (burstFire && _LDSemiFire == 0 && _fireInput == 1f && !_isBursting)
				{
					_isBursting = true;

					_canFire = false;

					_canSwitch = false;

					_LDSemiFire++;
				}
				else if (_LDSemiFire == 1 && _fireInput == 0f && _canFire)
				{
					_LDSemiFire--;
				}
			}
			else
			{
				if (burstFire && _fireInput == 1f && _canFire && !_isBursting)
				{
					_isBursting = true;

					_canFire = false;

					_canSwitch = false;
				}
			}
		}
		private void ShotgunFiringManager()
		{
			if (_animator == null || _animController == null || semiAuto || fullAuto || burstFire || melee) { return; }

			if (_weaponAmmoScript && _weaponAmmoScript.ammo < 1) { return; }

			if (fullAutoShawty)
			{
				if (shotgun && _LDShotgunFire == 0 && _fireInput == 1f && _canFire)
				{
					ShotgunFire();

					_canFire = false;

					_canSwitch = false;

					_LDShotgunFire++;
				}
				else if (_LDShotgunFire == 1 && _fireInput == 0f)
				{
					_LDShotgunFire--;
				}
			}
			else
			{
				if (_fireInput == 1f && _canFire)
				{
					ShotgunFire();

					_canFire = false;

					_canSwitch = false;
				}
			}
		}
		private void MeleeFiringManager()
		{
			if (_animator == null || _animController == null || semiAuto || fullAuto || burstFire) { return; }

			if (_weaponAmmoScript && _weaponAmmoScript.ammo < 1) { return; }

			if (melee && _LDMeleeAttack == 0 && _fireInput == 1f && _canFire)
			{
				if (areaAttack)
				{
					AreaMeleeAttack();
				}
				else
				{
					Fire();
				}

				_canFire = false;

				_canSwitch = false;

				_LDMeleeAttack++;
			}
			else if (melee && _LDMeleeAttack == 0 && _fireInput == 1f && !_canFire && !canCookMelee)
			{
				_LDMeleeAttack++;
			}
			else if (_fireInput == 0f && _LDMeleeAttack == 1)
			{
				_LDMeleeAttack--;
			}
		}


		private void CheckForEnemies()
		{
			if (!_enemyChecker) { return; }

			_enemies = _enemyChecker.enemies;
		}
		private void CheckForMeleeableEnemies()
		{
			if (!_camera || !melee || !areaAttack) { return; }

			foreach (Transform enemy in _enemies)
			{
				float distance = Vector3.Distance(enemy.position, _camera.transform.position);

				if (distance < range && Vector3.Angle(_camera.transform.forward, (enemy.position - _camera.transform.position).normalized) < maxAttackAngle)
				{
					if (Physics.Raycast(_camera.transform.position, (enemy.position - _camera.transform.position).normalized, out RaycastHit meleeLineHit, distance))
					{
						if (1 << meleeLineHit.transform.gameObject.layer == enemyLayers)
						{
							_canHitEnemyWithAreaAttack = true;

							if (!_areaAttackAbleEnemies.Contains(enemy)) { _areaAttackAbleEnemies.Add(enemy); }
						}
						else
						{
							_canHitEnemyWithAreaAttack = false;

							if (_areaAttackAbleEnemies.Contains(enemy)) { _areaAttackAbleEnemies.Remove(enemy); }
						}
					}
					else
					{
						_canHitEnemyWithAreaAttack = false;

						if (_areaAttackAbleEnemies.Contains(enemy)) { _areaAttackAbleEnemies.Remove(enemy); }
					}
				}
				else
				{
					_canHitEnemyWithAreaAttack = false;

					if (_areaAttackAbleEnemies.Contains(enemy)) { _areaAttackAbleEnemies.Remove(enemy); }
				}
			}
		}

		private void FireDelayManager()
		{
			if (_canFire || _isUnholstering || _isBursting) { return; }

			_fireTime += Time.deltaTime;

			if (_fireTime > fireDelay)
			{
				_canFire = true;

				_canSwitch = true;

				_fireTime = 0f;
			}
		}
		private void BurstFireDelayManager()
		{
			if (_canFire || _isUnholstering || !_isBursting) { return; }

			if (_burstTime == 0f && _currentBurstIndex < burstSize)
			{
				if (_weaponAmmoScript && _weaponAmmoScript.ammo < 1) { return; }

				Fire();

				_currentBurstIndex++;
			}

			_burstTime += Time.deltaTime;

			if (_currentBurstIndex >= burstSize && _burstTime > burstFireDelay || _weaponAmmoScript && _weaponAmmoScript.ammo <= 0 && _burstTime > burstFireDelay)
			{
				_fireTime += Time.deltaTime;
			}
			else if (_currentBurstIndex >= burstSize && _burstTime > burstFireDelay && !_weaponAmmoScript)
			{
				_fireTime += Time.deltaTime;
			}

			if (_burstTime > burstFireDelay && _currentBurstIndex < burstSize)
			{
				if (_weaponAmmoScript && _weaponAmmoScript.ammo > 0)
				{
					_burstTime = 0f;
				}
				else if (!_weaponAmmoScript)
				{
					_burstTime = 0f;
				}
			}

			if (_fireTime > fireDelay)
			{
				_canFire = true;

				_canSwitch = true;

				_isBursting = false;

				_currentBurstIndex = 0;

				_fireTime = 0f;

				_burstTime = 0f;
			}
		}


		private void Fire()
		{
			unityEvents.OnFire.Invoke();

			if (_weaponAmmoScript)
			{
				if (!_weaponAmmoScript.infiniteAmmo && !melee && !shotgun) { _weaponAmmoScript.ammo--; }

				if (_weaponAmmoScript.ammo == 1 && !melee)
				{
					_animator.SetTrigger("Last Bullet");
				}
				else
				{
					int fireType = UnityEngine.Random.Range(0, 2);

					_animator.SetTrigger("Fire");
				}
			}
			else
			{
				int fireType = UnityEngine.Random.Range(0, 2);

				_animator.SetTrigger("Fire");
			}
			
			//Reticle
			if (_reticle)
			{
				_reticle.GetComponent<Reticle>().SpreadReticle();
			}
			//

			_hitSomething = Physics.Raycast(_camera.transform.position, _fireDirection, out RaycastHit hit, range, hittableLayers);
			_hit = hit;

			if (_hitSomething)
			{
				_hitName = _hit.transform.name;
				_hitPoint = hit.point;

				if (1 << hit.transform.gameObject.layer == enemyLayers)
				{
					hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
				}
			}
			else
			{
				_hitName = "Sky";
				_hitPoint = _camera.transform.position + _fireDirection * range;
			}

			TurnOnFireFlash();

			if (_impactSphere) { _impactSphere.position = _hitPoint; }

			unityEvents.OnLateFire.Invoke();
		}
		private void ShotgunFire()
		{
			for (int i = 0; i < pelletAmount; i++)
			{
				Fire();
			}

			_weaponAmmoScript.ammo--;
		}
		private void AreaMeleeAttack()
		{
			if (_weaponAmmoScript.ammo == 1 && !melee)
			{
				_animator.SetTrigger("Last Bullet");
			}
			else
			{
				int fireType = UnityEngine.Random.Range(0, 2);

				_animator.SetTrigger("Fire");
			}

			foreach (Transform enemy in _areaAttackAbleEnemies)
			{
				if (enemy.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
				{
					enemyHealth.TakeDamage(damage);
				}
			}

			if (!_weaponAmmoScript.infiniteAmmo) { _weaponAmmoScript.ammo--; }

			TurnOnFireFlash();
		}


		private void TurnOnFireFlash()
		{
			if (_testFireFlashImage == null) { return; }

			_testFireFlashImage.enabled = true;

			Invoke(nameof(TurnOffFireFlash), burstFire ? burstFireDelay * 0.9f : fireDelay * 0.9f);
		}
		private void TurnOffFireFlash()
		{
			_testFireFlashImage.enabled = false;
		}

		private void OnDisable()
		{
			_fireTime = 0f;
			_burstTime = 0f;
			_currentBurstIndex = 0;
			_isBursting = false;

			_LDSemiFire = 0;
		}

		private void OnEnable()
		{
			_isUnholstering = true;
			_canFire = false;
			_canSwitch = false;
			_unholsterTime = 0f;
			_LDUnholster = 0;

			if (_weaponAmmoScript)
			{
				if (_weaponAmmoScript.ammo == 0 && _animator)
				{
					_animator.SetTrigger("OnEnableMagEmptyCheck");

					_animator.SetBool("Mag Is Empty", true);
				}
			}
		}

		private void UnholsterDelayManager()
		{
			_unholsterTime += Time.deltaTime;

			if (_unholsterTime > unholsterDelay && _LDUnholster == 0 && afterUnholsterEnableCanFire)
			{
				_isUnholstering = false;
				_canFire = true;
				_canSwitch = true;
				_LDUnholster++;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(_hitPoint, 0.5f);

			if (_camera && _enemies.Count > 0 && melee && areaAttack)
			{
				foreach (Transform enemy in _enemies)
				{
					Gizmos.DrawRay(_camera.transform.position, (enemy.position - _camera.transform.position).normalized * Vector3.Distance(_camera.transform.position, enemy.position));
				}
			}
		}
	}
}
