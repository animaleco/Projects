using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_FireButton : MonoBehaviour
{
    private Animator animator;
    private Trap_Fire trapFire;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        trapFire = GetComponentInParent<Trap_Fire>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();    

        if (player != null)
        {
            animator.SetTrigger("activate");
            trapFire.SwitchOffFire();
        }
    }
}
