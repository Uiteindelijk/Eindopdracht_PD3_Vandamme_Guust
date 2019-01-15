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

    //player sees npc
    public bool NpcIsSpotted { get; set; }

    //npc is running away
    public Transform[] Rooms;
    private int _counter;
    private float _distance;
    private bool _runningAway = false;
    private bool _runAnim = false;

    //is player in same room as npc
    public bool PlayerInRoom { get; set; }

    //moving to player
    private Transform _player;

    //running away from player
    private bool _escape = false;
    public bool StillInSameRoom { get; set; }

    //player attack
    [SerializeField] private int _stabDamage;
    private float _attackTimer;
    public bool StabAnimIsPlaying { get; set; }

    //check room on npc
    public bool AlreadyNpcInRoom { get; set; }

    //check if room has been patrolled
    public bool RoomChecked { get; set; }

    //patrolling room
    public Transform NextPatrolPoint { get; set; }

    //npc is moving rooms
    public bool NpcIsMovingRoom { get; set; }

    //npc lives
    [SerializeField] private int _npcLives = 0;
    private bool _npcIsDead = false;
    
    void Start()
    {
        SetPublicVars();
        ToAssignComponents();
        
#if DEBUG

        Assert.IsNotNull(_player, "The player doesn't contain the tag player");
        Assert.IsNotNull(_agent, "The npc: " + transform.name + " doesn't have a NavMeshAgent");

#endif
        
        NpcBehaviourTree();
        StartCoroutine(RunTree());
    }

    IEnumerator RunTree()
    {
        while (Application.isPlaying)
        {
            yield return _rootNode.Tick();
        }
    }
    
    //to set every public item
    private void SetPublicVars()
    {
        NpcIsSpotted = false;
        PlayerInRoom = false;
        StillInSameRoom = false;
        AlreadyNpcInRoom = false;
        StabAnimIsPlaying = false;
        RoomChecked = false;
        NpcIsMovingRoom = false;
        _attackTimer = 0.5f;
    }
    
    //to find gameobjects for the enemy
    private void ToAssignComponents()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = transform.GetComponent<NavMeshAgent>();
    }
    
    //the enemy behaviour tree
    private void NpcBehaviourTree()
    {
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
    }

    //npc spotted
    public bool NpcSpottedByPlayer()
    {
        if(_agent.remainingDistance < 0.5f)
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
            _agent.SetDestination(new Vector3(_player.position.x + 1f, _player.position.y, _player.position.z));
            transform.LookAt(new Vector3(_player.position.x, _player.position.y, _player.position.z));
            

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
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.forward * 1f, Color.blue);
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.forward, out hit, 1f))
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
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0)
        {
            //Debug.Log("Player got stabbed");
            _player.GetComponent<PlayerMovement>().RecivingTheStab(_stabDamage);
            
            _escape = true;
            StillInSameRoom = true;
        }
        if (_attackTimer > 0.45f)
        {
            transform.GetComponent<EnemyAnimations>().NpcStabsPlayer();
        }

        yield return NodeResult.Running;
        
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
            //Debug.Log("room has been checked");
            return false;
        }
        else
        {
            //Debug.Log("room hasn't been checked yet");
            return true;
        }
    }
    
    //patrolling the room
    IEnumerator<NodeResult> PatrollingRoom()
    {
        if(NextPatrolPoint != null && !RoomChecked && !PlayerInRoom && !_runningAway)
        {
            //Debug.Log("Moving to patrolpoint: " + NextPatrolPoint.name);
            _agent.SetDestination(NextPatrolPoint.position);

            yield return NodeResult.Succes;
        }
        else
        {
            //Debug.Log("No next patrol point");
            yield return NodeResult.Failure;
        }
        
    }

    //moves room if the room already has been patrolled
    IEnumerator<NodeResult> NpcMovesRoom()
    {
        if (!PlayerInRoom)
        {
            NpcIsMovingRoom = true;
            _agent.SetDestination(Rooms[_counter].position);
            //Debug.Log("Moving Room" + _agent.SetDestination(Rooms[_counter].position));

            yield return NodeResult.Succes;
        }
        else
        {
            yield return NodeResult.Failure;
        }


    }
    
    //if the npc dies
    public void NpcRecivesDamage(int Damage)
    {
        _npcLives -= Damage;

        if(_npcLives <= 0)
        {
            _agent.Stop();
            transform.GetComponent<EnemyAnimations>().NpcDies();
        }

    }
    
    //if the player dies
    public void PlayerDied()
    {
        Debug.Log("The Player is Gone!");
        _agent.Stop();
        transform.GetComponent<EnemyAnimations>().PlayerIsDead();
    }
    
}
