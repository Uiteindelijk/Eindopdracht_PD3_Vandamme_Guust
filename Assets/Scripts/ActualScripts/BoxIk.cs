using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxIk : StateMachineBehaviour
{

    //public Transform BoxTargetRight, BoxTargetLeft;
    public float RayRange { get; set; }
    public Transform _leftRay, _rightRay;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckRay(_leftRay, AvatarIKGoal.LeftHand, animator);
        CheckRay(_rightRay, AvatarIKGoal.RightHand, animator);
    }

    void CheckRay(Transform startpos, AvatarIKGoal IkGoal, Animator anim)
    {
        RaycastHit hit;
        if(Physics.Raycast(startpos.position, startpos.forward, out hit, RayRange))
        {
            Vector3 targetPos = hit.point;
            SetIk(anim ,IkGoal, targetPos);
        }


    }

    void SetIk(Animator anim, AvatarIKGoal IkGoal, Vector3 targetpos)
    {
        anim.SetIKPositionWeight(IkGoal, 1.0f);
        anim.SetIKPosition(IkGoal, targetpos);
    }


}
