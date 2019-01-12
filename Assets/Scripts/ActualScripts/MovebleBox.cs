using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovebleBox : MonoBehaviour
{
    private Transform  _normalParrent;
    
    public void BoxIsGrabbed(GameObject newParent)
    {
        transform.parent = newParent.transform;
    }

    public void BoxIsDropped()
    {
        transform.parent = null;
    }

}
