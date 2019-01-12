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
    public Transform[] _patrollPoints;
    
    private void Update()
    {
        //to check if the player or other npc is in the room
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

        //patroll stuf
        if (_npcIsInRoom && !_isRoomChecked && !_playerIsInRoom)
        {
            PatrolRoom();
        }

        if(_isRoomChecked && _npcIsInRoom)
        {
            _npc.GetComponent<EnemyAi>().RoomChecked = true;
            //_isRoomChecked = false; //remove later
        }

        if(!_npcIsInRoom)
        {
            _isRoomChecked = false;
        }

    }
    
    void PatrolRoom()
    {
        //the npc know now that the room isn't checked
        _npc.GetComponent<EnemyAi>().RoomChecked = false;

        //we give the npc next point to move to
        _npc.GetComponent<EnemyAi>().NextPatrolPoint = _patrollPoints[_patrollCounter];
        float distance = Vector3.Distance(_npc.position, _patrollPoints[_patrollCounter].position);
        
        if (distance < 0.5f)
        {
            _patrollCounter++;
        }

        if(_patrollCounter >= _patrollPoints.Length)
        {
            _isRoomChecked = true;
            _patrollCounter = 0;
        }
        
    }
    
    void OnTriggerStay(Collider other)
    {
        //to check who is in the room
        if (other.tag == "Player")
        {
            _playerIsInRoom = true;
        }

        if (other.tag == "Enemy" && !_npcIsInRoom)
        {
            _npcIsInRoom = true;
            _npc = other.transform;
            other.GetComponent<EnemyAi>().AlreadyNpcInRoom = false;
            //Debug.Log(_npc + "Enemy is in room: " + gameObject.name);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && _npcIsInRoom)
        {
            other.GetComponent<EnemyAi>().AlreadyNpcInRoom = true;
            //Debug.Log(other.name + " Npc should leave this room there is already an npc here");
        }
        
    }

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
