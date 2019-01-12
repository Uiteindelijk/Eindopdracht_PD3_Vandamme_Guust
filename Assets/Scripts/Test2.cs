using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test2 : MonoBehaviour
{
    //to move charlie with nav mesh
    public Camera Cam;
    public NavMeshAgent Agent;
    private bool _npcIsMoving = false;
    private Animator _anim;
    
    private void Start()
    {
        _anim = transform.GetComponent<Animator>();
        
    }

    void Update()
    {
        MovingCharlie();
    }

    void MovingCharlie()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Debug.Log("Charlie has new order");

            if (Physics.Raycast(ray, out hit))
            {
                //move charlie
                Agent.SetDestination(hit.point);
                //Debug.Log("Charlie is moving");
            }

        }
    }
    
}
