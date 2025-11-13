using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class standCrouchUI : MonoBehaviour
    {
        [SerializeField] private PlayerMovement_CC _playerMovement;
		[SerializeField] private Transform standImage;
		[SerializeField] private Transform crouchImage;

		int _LDStandCrouch = 0;

		private void Update()
		{
			if (!_playerMovement) { return; }
		
			if (!_playerMovement._isCrouching && _LDStandCrouch == 0)
			{
				if (standImage) { standImage.gameObject.SetActive(true); }
				if (crouchImage) { crouchImage.gameObject.SetActive(false); }

				_LDStandCrouch = 1;
			}
			else if (_playerMovement._isCrouching && _LDStandCrouch == 1)
			{
				if (standImage) { standImage.gameObject.SetActive(false); }
				if (crouchImage) { crouchImage.gameObject.SetActive(true); }

				_LDStandCrouch = 0;
			}
		}
	}
}