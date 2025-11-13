using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandToCrouchBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Finished Crouch To Stand", false);
        animator.SetBool("Finished Stand To Crouch", false);

        animator.SetBool("Stand To Crouching", true);
        animator.SetBool("Is Stand Crouch Transitioning", true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Finished Crouch To Stand", false);
        animator.SetBool("Finished Stand To Crouch", true);

        animator.SetBool("Stand To Crouching", false);
        if (!animator.GetBool("Crouch To Standing"))
        {
            animator.SetBool("Is Stand Crouch Transitioning", false);
        }
    }
}
