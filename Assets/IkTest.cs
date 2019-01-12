using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkTest : StateMachineBehaviour {

    public Transform IkTarget, IkTarget2;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.2f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.2f);

        animator.SetIKPosition(AvatarIKGoal.LeftFoot, IkTarget.position);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, IkTarget2.position);

    }

}
