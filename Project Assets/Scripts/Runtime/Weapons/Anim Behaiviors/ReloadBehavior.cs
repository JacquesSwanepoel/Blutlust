using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadBehavior : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Is Reloading", true);

        if (animator.GetBool("Is Crouching"))
        {
           animator.SetTrigger("Stand To Crouch");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Is Reloading", false);

        animator.SetBool("Finished Crouch To Stand", true);

        if (animator.GetBool("Is Crouching"))
		{
            animator.SetTrigger("Stand To Crouch");
        }

        Debug.Log("doesit");
    }
}
