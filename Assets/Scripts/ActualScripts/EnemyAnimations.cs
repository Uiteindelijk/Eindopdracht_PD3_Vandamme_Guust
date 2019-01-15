using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

public class EnemyAnimations : MonoBehaviour
{

    private Animator _anim;
    private bool _npcIsDead = false, _npcStab = false, _npcIsMoving = false, _playerIsDead = false;
    private Vector3 _npcVelocity = Vector3.zero;
    private NavMeshAgent _npcAgent;

    private void Start()
    {
        //to set the animator and transform
        _npcAgent = transform.GetComponent<NavMeshAgent>();
        _anim = transform.GetComponent<Animator>();

#if DEBUG

        Assert.IsNotNull(_npcAgent, "Npc: " + transform.name + " doesn't contain a NavMeshAgent");
        Assert.IsNotNull(_anim, "Npc: " + transform.name + " doesn't contain an Animator");
        
#endif
        
    }

    private void Update()
    {
        //to calculate the npc velocity
        NpcVelocity();
        
        //to update the animations of the npc
        UpdateAnimations();
    }

    //were the animation gets updated
    private void UpdateAnimations()
    {
        //walkanimations
        _anim.SetFloat("ForBackMoving", _npcVelocity.z);
        _anim.SetFloat("LeftRightMoving", _npcVelocity.x);
        _anim.SetBool("IsMoving", _npcIsMoving);

        //stab animation
        _anim.SetBool("StabsPlayer", _npcStab);

        //dyinganimation
        _anim.SetBool("IsDead", _npcIsDead);

        //player dead winning animation
        _anim.SetBool("PlayerIsDead", _playerIsDead);

    }

    //npc is moving
    private void NpcVelocity()
    {
        //to get the velocity of the npc
        Vector3 velocity = Vector3.Scale(_npcAgent.velocity, new Vector3(1, 0, 1));
        _npcVelocity = _npcAgent.transform.InverseTransformDirection(velocity);
        _npcVelocity.Normalize();

        //to let the animator know npc is moving
        if(_npcVelocity.z > 0.1 || _npcVelocity.x > 0.1)
        {
            _npcIsMoving = true;
        }
        else
        {
            _npcIsMoving = false;
        }

    }

    //when the npc stabs the player
    public void NpcStabsPlayer()
    {
        _npcStab = true;
    }

    //when the stab ends
    private void StabAnimationEnds()
    {
        _npcStab = false;
        transform.GetComponent<EnemyAi>().StabAnimIsPlaying = _npcStab;
    }

    //when the npc dies
    public void NpcDies()
    {
        _npcIsDead = true;
    }

    //if the player is dead
    public void PlayerIsDead()
    {
        _playerIsDead = true;
    }

}
