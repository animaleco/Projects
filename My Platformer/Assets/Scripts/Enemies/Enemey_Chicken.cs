using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemey_Chicken : Enemy
{
    [Header("Chicken details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;

    private float aggroTimer;
    private bool playerDetected;
    private bool canFlip = true;

    // Poniendo override y el metodo se pone solo
    protected override void Update()
    {
        base.Update(); // base.Metodo para tener el funcionamiento de awake del script enemy

        
        aggroTimer -= Time.deltaTime;


        if (isDead)
        {
            return;
        }

        if (playerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if(aggroTimer < 0)
        {
            canMove = false;
        }

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {
            Flip();
            canMove = false;
           
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
        {
            return;
        }

        float xValue = player.transform.position.x;

        HandleFlip(player.transform.position.x);

         rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && isFacingRight || xValue > transform.position.x && !isFacingRight)
        {
            if (canFlip)
            {
                canFlip = false;

                Invoke(nameof(Flip), .3f);
            }
        }
    }

    protected override void Flip()
    {
        base.Flip();

        canFlip = true;
    }

}
