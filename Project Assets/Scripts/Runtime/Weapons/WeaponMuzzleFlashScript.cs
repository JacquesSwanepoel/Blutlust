using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WeaponMuzzleFlashScript : MonoBehaviour
    {
		[SerializeField] private ParticleSystem _muzzleFlash;
		[Header("MuzzleFlash Variables")]
		public float muzzleFlashEnableTime = 0.05f;

		public void PlayMuzzleFlash()
		{
			if (_muzzleFlash)
			{
				_muzzleFlash.Play();

				Invoke(nameof(DisableMuzzleFlash), muzzleFlashEnableTime);
			}
		}
		private void DisableMuzzleFlash()
		{
			_muzzleFlash.Clear();
		}
	}
}