using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerInRoom : MonoBehaviour
{
    //vars to check if there is an npc or player
    private bool _playerIsInRoom = false, _npcIsInRoom = false;
    private Transform _player, _npc;

    //vars for room patroll
    private bool _isRoomChecked = false;
    private int _patrollCounter = 0;
    public Transform[] PatrollPoints;
    
    private void Update()
    {
        CheckRoomForPlayerAndNpc();
        PatrolStatus();
    }
    
    //to check if the player and npc are toghetter in room
    private void CheckRoomForPlayerAndNpc()
    {
        if (_playerIsInRoom && _npcIsInRoom)
        {
            //Debug.Log("enemy en player are in room: " + gameObject.name);
            _npc.transform.GetComponent<EnemyAi>().PlayerInRoom = true;
        }
        else if (!_playerIsInRoom && _npcIsInRoom)
        {
            _npc.transform.GetComponent<EnemyAi>().PlayerInRoom = false;
        }
        else if (!_npcIsInRoom)
        {
            _npc = null;
        }
    }

    //to check if the npc has to patrol the room
    private void PatrolStatus()
    {
        if (_npcIsInRoom && !_isRoomChecked && !_playerIsInRoom)
        {
            PatrolRoom();
        }

        if (_isRoomChecked && _npcIsInRoom)
        {
            _npc.GetComponent<EnemyAi>().RoomChecked = true;
            //_isRoomChecked = false; //remove later
        }

        if (!_npcIsInRoom)
        {
            _isRoomChecked = false;
        }
    }

    //if npc still needs to patroll the room
    private void PatrolRoom()
    {
        //the npc know now that the room isn't checked
        _npc.GetComponent<EnemyAi>().RoomChecked = false;

        //we give the npc next point to move to
        _npc.GetComponent<EnemyAi>().NextPatrolPoint = PatrollPoints[_patrollCounter];
        float distance = Vector3.Distance(_npc.position, PatrollPoints[_patrollCounter].position);
        
        if (distance < 0.5f)
        {
            _patrollCounter++;
        }

        if(_patrollCounter >= PatrollPoints.Length)
        {
            _isRoomChecked = true;
            _patrollCounter = 0;
        }
        
    }
    
    //checks if there is an npc or player in the room
    void OnTriggerStay(Collider other)
    {
        //to check for player
        if (other.tag == "Player")
        {
            _playerIsInRoom = true;
        }

        //to check for npc and not a second npc
        if (other.tag == "Enemy" && !_npcIsInRoom)
        {
            //Debug.Log(_npc + "Enemy is in room: " + gameObject.name);
            _npcIsInRoom = true;
            _npc = other.transform;
            other.GetComponent<EnemyAi>().AlreadyNpcInRoom = false;
            
        }
        
    }
    
    //if a second npc enters the room
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && _npcIsInRoom)
        {
            other.GetComponent<EnemyAi>().AlreadyNpcInRoom = true;
            //Debug.Log(other.name + " Npc should leave this room there is already an npc here");
        }
        
    }

    //if the player or npc leave the room
    private void OnTriggerExit(Collider other)
    {
        //to check who is leaving the room
        if (other.tag == "Enemy")
        {
            _npc.transform.GetComponent<EnemyAi>().PlayerInRoom = false;
            _npc.transform.GetComponent<EnemyAi>().StillInSameRoom = false;
            _npc.transform.GetComponent<EnemyAi>().NpcIsMovingRoom = false;
            _npcIsInRoom = false;
        }

        if (other.tag == "Player")
        {
            _playerIsInRoom = false;
        }

    }
    
}
