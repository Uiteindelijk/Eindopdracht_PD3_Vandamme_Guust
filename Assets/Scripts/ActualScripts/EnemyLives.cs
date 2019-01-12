using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyLives : MonoBehaviour
{
    [SerializeField] private float _lives;
    private bool _enemyDead;
    private Animator _anim;

    private void Start()
    {
        //_anim = transform.GetChild(0).GetComponent<Animator>();

#if DEBUG
        //Assert.IsNotNull(_anim, "Enemy " + transform.name + " doesn't have an animator");
#endif

    }

    private void Update()
    {
        //_anim.SetBool("IsDead", _enemyDead);
    }

    public void TakeDamage(float amount)
    {
        _lives -= amount;
        if(_lives <= 0)
        {
            Die();    
        }
    }

    void Die()
    {
        Debug.Log("Charlie is fucking dead");
        _enemyDead = true;
        Destroy(gameObject);
    }

}
