using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonIK : StateMachineBehaviour {

    public Transform ButtonTarget;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //for more information check document

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, ButtonTarget.position);
        
    }
}
