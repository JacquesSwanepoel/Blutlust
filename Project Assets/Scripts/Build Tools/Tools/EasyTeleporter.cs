using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyTeleporter : MonoBehaviour
{
    [SerializeField] private Transform _otherTeleporter;

	[SerializeField] private float reTeleportTime = 1f;

	private Transform _player;

	internal bool canTeleport = true;

	private float canTeleportTime = 0f;

	private void Start()
	{
		if (GameObject.FindWithTag("Player"))
		{
			_player = GameObject.FindWithTag("Player").transform;
		}
	}

	private void Update()
	{
		canTeleportDelayManager();
	}

	private void OnCollisionEnter()
	{
		Teleport();
	}

	private void Teleport()
	{
		if (_otherTeleporter == null || _player == null || !canTeleport) { return; }

		if (_otherTeleporter.TryGetComponent(out EasyTeleporter easyTP))
		{
			easyTP.canTeleport = false;

			_player.position = _otherTeleporter.position;
		}
	}

	private void canTeleportDelayManager()
	{
		if (canTeleport) { return; }

		canTeleportTime += Time.deltaTime;

		Physics.IgnoreCollision(transform.GetComponent<Collider>(), _player.GetComponent<Collider>(), true);

		if (canTeleportTime > reTeleportTime)
		{
			Physics.IgnoreCollision(transform.GetComponent<Collider>(), _player.GetComponent<Collider>(), false);

			canTeleport = true;

			canTeleportTime = 0f;
		}
	}
}
