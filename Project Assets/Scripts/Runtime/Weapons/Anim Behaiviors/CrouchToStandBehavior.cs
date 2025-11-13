using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchToStandBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Finished Stand To Crouch", false);
        animator.SetBool("Finished Crouch To Stand", false);

        animator.SetBool("Crouch To Standing", true);
        animator.SetBool("Is Stand Crouch Transitioning", true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        animator.SetBool("Finished Stand To Crouch", false);
        animator.SetBool("Finished Crouch To Stand", true);

        animator.SetBool("Crouch To Standing", false);
        if (!animator.GetBool("Stand To Crouching"))
        {
            animator.SetBool("Is Stand Crouch Transitioning", false);
        }
    }
}
