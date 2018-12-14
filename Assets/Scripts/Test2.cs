using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{

    public Transform Target;
    public Vector3 Offset;
    public Animator Anim;

    private Transform _chest; 

	void Start ()
    {
        _chest = Anim.GetBoneTransform(HumanBodyBones.Chest);
	}
	
	void LateUpdate ()
    {

        _chest.LookAt(Target.position);
        _chest.rotation = _chest.rotation * Quaternion.Euler(Offset);

	}
}
