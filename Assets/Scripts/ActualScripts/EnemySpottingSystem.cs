using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpottingSystem : MonoBehaviour
{
    private Transform _npc;
    [SerializeField] private float _fieldOfView = 0, _range;
    private bool _enemyInSight = false;

    private void Update()
    {
        //Debug.Log("enemy in sight: " + _enemyInSight);
        if (_enemyInSight)
        {
            _npc.GetComponent<EnemyAi>().NpcIsSpotted = true;
        }
        else if (!_enemyInSight && _npc != null)
        {
            _npc.GetComponent<EnemyAi>().NpcIsSpotted = false;
            _npc = null;
        }

    }

    //to check if the enemy is in eye sight
    private void OnTriggerStay(Collider other)
    {
        //for more information check document
        if (other.tag == "Enemy")
        {
            _enemyInSight = false;

            //calculation of enemy is in field of view
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < _fieldOfView * 0.5f)
            {
                //kijken of er een object tussen de speler en enemy zit
                RaycastHit hit;
                Debug.DrawRay(transform.position, new Vector3(direction.x, direction.y +0.5f, direction.z) * _range, Color.blue);
                if (Physics.Raycast(transform.position, new Vector3(direction.x, direction.y + 0.5f, direction.z), out hit, _range))
                {
                    //Debug.Log("raycast is hitting: " + hit.transform.name);
                    if (hit.transform.tag == "Enemy")
                    {
                        //Debug.Log("Enemy is spotted");
                        _enemyInSight = true;
                        _npc = hit.transform;
                    }
                }
                
            }
        }
    }
}