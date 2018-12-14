using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour {

    protected Animator _anim;

    public bool IkActive = false;
    public Transform RightHandObj = null;
    public Transform LookObj = null;

	void Start ()
    {
        _anim = GetComponent<Animator>();
	}
	
	void OnAnimatorIk()
    {
		
        if(_anim)
        {

            //if the IK is active, set the position and rotation directly to the goal.
            if(IkActive)
            {
                //set the look target position, if one has been assigned.
                if(LookObj != null)
                {
                    _anim.SetLookAtWeight(1);
                    _anim.SetLookAtPosition(LookObj.position);
                }

                //set the right hand target position and rotation, if one has been assigned.
                if(RightHandObj != null)
                {
                    _anim.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    _anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                    _anim.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
                    
                }

            }

            //if the ik is not active, set the position and rotation of the hand and head back to og position.
            else
            {
                _anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _anim.SetLookAtWeight(0);
            }

        }

	}



}
