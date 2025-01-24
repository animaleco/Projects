using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mushroom : Enemy
{

    // Poniendo override y el metodo se pone solo
    protected override void Update()
    {
        base.Update(); // base.Metodo para tener el funcionamiento de awake del script enemy

       
        if (isDead)
        {
            return;
        }

        HandleMovement();

        if(isGrounded)
           HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {


            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if(idleTimer > 0)
        {
            return;
        }

         rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);
        

    }



}
