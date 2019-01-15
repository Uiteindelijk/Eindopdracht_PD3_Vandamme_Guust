using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBehaviour : MonoBehaviour {

    public Transform Target;

    private void Update()
    {
        //so the cam follows the position but not the rotation
        this.transform.position = Target.position;
    }

}
