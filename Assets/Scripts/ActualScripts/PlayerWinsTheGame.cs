using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWinsTheGame : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerMovement>().PlayerWon();
        }
    }

}
