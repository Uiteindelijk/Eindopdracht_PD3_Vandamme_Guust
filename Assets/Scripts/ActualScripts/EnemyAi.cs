using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class EnemyAi : MonoBehaviour
{
    private INode _rootNode;

    //to move charlie with nav mesh
    private NavMeshAgent _agent;
    private bool _npcIsMoving = false;
    private Animator _anim;

    //player sees npc
    [HideInInspector] public bool NpcIsSpotted = false;

    //npc is running away
    public Transform[] Rooms;
    private int _counter;
    private float _distance;
    private bool _runningAway = false;

    //is player in same room as npc
    [HideInInspector] public bool PlayerInRoom = false;

    //moving to player
    private Transform _player;

    //running away from player
    private bool _escape = false;
    [HideInInspector] public bool StillInSameRoom = false;

    //player attack
    [SerializeField] private int _stabDamage;

    //check room on npc
    [HideInInspector] public bool AlreadyNpcInRoom = false;

    //check if room has been patrolled
    [HideInInspector] public bool RoomChecked = false;

    //patrolling room
    [HideInInspector] public Transform NextPatrolPoint;

    //npc is moving rooms
    [HideInInspector] public bool NpcIsMovingRoom = false;
    

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = transform.GetComponent<NavMeshAgent>();
        _anim = transform.GetComponent<Animator>();

#if DEBUG

        Assert.IsNotNull(_player, "The player doesn't contain the tag player");
        Assert.IsNotNull(_agent, "The npc: " + transform.name + " doesn't have a NavMeshAgent");
        Assert.IsNotNull(_anim, "The npc: " + transform.name + " doesn't have a Animator");

#endif

        //the behaviour tree
        _rootNode =


                new SelectorNode(
                    new SequenceNode(
                        new ConditionNode(NpcSpottedByPlayer),
                        new ActionNode(RunAway)),
                    new SequenceNode(
                        new ConditionNode(IsInSameRoomAsPlayer),
                        new SequenceNode(
                            new ActionNode(NpcMovesToPlayer),
                            new ConditionNode(PlayerInRangeToAttack),
                            new SequenceNode(
                                new ActionNode(NpcAttacksPlayer),
                                new ActionNode(NpcRetreats)))),
                    new SelectorNode(
                        new SequenceNode(
                            new ConditionNode(NpcRoomCheck),
                            new ConditionNode(CheckIfRoomHasBeenPatrolled),
                            new ActionNode(PatrollingRoom)),
                        new ActionNode(NpcMovesRoom)
                        ));
                


        StartCoroutine(RunTree());
    }

    IEnumerator RunTree()
    {
        while (Application.isPlaying)
        {
            yield return _rootNode.Tick();
        }
    }

    //npc spotted
    public bool NpcSpottedByPlayer()
    {
        if(_agent.remainingDistance < 3)
        {
            _counter =  Random.Range(0, Rooms.Length);
            _runningAway = false;
        }
        
        if (NpcIsSpotted)
        {
            //Debug.Log("Spotted");
            return true;
        }
        else
        {
            //Debug.Log("Npc Not spotted");
            return false;
        }
        

    }

    //npc is running away
    IEnumerator<NodeResult> RunAway()
    {
        Debug.Log("Npc is runnning away");
        _distance = Vector3.Distance(_agent.transform.position, Rooms[_counter].position);
        _agent.SetDestination(Rooms[_counter].position);
        _runningAway = true;
        

        yield return NodeResult.Succes;
    }

    //To check if the player is in the same room
    public bool IsInSameRoomAsPlayer()
    {
        if (PlayerInRoom && !StillInSameRoom)
        {
            //Debug.Log("Player in room by: " + gameObject.name);
            return true;
        }
        else
        {
            //Debug.Log("No Player in room by: " + gameObject.name);
            if(_escape)
            {
                _escape = false;
                //Debug.Log("has escaped");
            }

            return false;
        }

    }

    //npc moves to player
    IEnumerator<NodeResult> NpcMovesToPlayer()
    {
        
        if(!_runningAway && !_escape)
        {
            //Debug.Log("Moving to player");
            _agent.SetDestination(new Vector3(_player.position.x - 0.5f, _player.position.y, _player.position.z ));
            transform.LookAt(new Vector3(_player.position.x, transform.localRotation.y, _player.position.z));
            

            yield return NodeResult.Succes;
        }
        
    }
    
    //check if player is in enemy range
    public bool PlayerInRangeToAttack()
    {

        Vector3 direction = _player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < 180 * 0.5f)
        {
            //kijken of er een object tussen de speler en enemy zit
            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.forward * 1f, Color.blue);
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {
                //Debug.Log("raycast is hitting: " + hit.transform.name);
                if (hit.transform.tag == "Player" && !_escape)
                {
                    //Debug.Log("Player in range to get stabbed");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
            return false;
        
    }

    //attacks the player
    IEnumerator<NodeResult> NpcAttacksPlayer()
    {
        //Debug.Log("Player got stabbed");
        _player.GetComponent<New>().RecivingTheStab(_stabDamage);
        _escape = true;
        StillInSameRoom = true;


        yield return NodeResult.Succes;
    }

    //runs away after attacking the player
    IEnumerator<NodeResult> NpcRetreats()
    {
        //Debug.Log("Retreating from player");
        _agent.SetDestination(Rooms[_counter].position);

        _distance = Vector3.Distance(_agent.transform.position, Rooms[_counter].position);
        

        yield return NodeResult.Succes;
        
    }

    //check if there is already an npc in the room
    public bool NpcRoomCheck()
    {
        //Debug.Log("Checking room if there is an npc");

        if (AlreadyNpcInRoom || StillInSameRoom)
        {
            //Debug.Log("there is an npc in the room/ already was here");     //terug wisselen de false en treu
            return false;
        }
        else
        {
            //Debug.Log(" is allone in room");
            return true;
        }
        
    }

    //check if the rool already has been patrolled
    public bool CheckIfRoomHasBeenPatrolled()
    {
        if(RoomChecked)
        {
            Debug.Log("room has been checked");
            return false;
        }
        else
        {
            Debug.Log("room hasn't been checked yet");
            return true;
        }
    }
    
    //patrolling the room
    IEnumerator<NodeResult> PatrollingRoom()
    {
        if(NextPatrolPoint != null && !RoomChecked && !PlayerInRoom && !_runningAway)
        {
            Debug.Log("Patrolling room");
            _agent.SetDestination(NextPatrolPoint.position);

            yield return NodeResult.Succes;
        }
        else
        {
            Debug.Log("No next patrol point");
            yield return NodeResult.Failure;
        }
        
    }

    //moves room if the room already has been patrolled
    IEnumerator<NodeResult> NpcMovesRoom()
    {
        if (!PlayerInRoom)
        {
            Debug.Log("Moving Room");
            NpcIsMovingRoom = true;
            _agent.SetDestination(Rooms[_counter].position);

            yield return NodeResult.Succes;
        }
        else
        {
            yield return NodeResult.Failure;
        }


    }

}
