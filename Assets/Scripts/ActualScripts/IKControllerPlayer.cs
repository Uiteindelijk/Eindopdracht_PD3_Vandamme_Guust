using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControllerPlayer : StateMachineBehaviour
{

    private float IkWeight = 0.7f;
    public Transform IkTargetLeftHand, IkTargetRightHand;

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //for more information check document

        //to set how much the limb is following to the target
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IkWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IkWeight);

        //to let the limb go to the position you want
        animator.SetIKPosition(AvatarIKGoal.LeftHand, IkTargetLeftHand.position);
        animator.SetIKPosition(AvatarIKGoal.RightHand, IkTargetRightHand.position);
    }
    
}
