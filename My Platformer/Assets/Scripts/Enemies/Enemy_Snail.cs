using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Enemy_Snail : Enemy
{
    [Header("Snail details")]
    [SerializeField] private Enemy_SnailBody bodyPrefab;
    [SerializeField] private float maxSpeed = 10;
    private bool hasBody = true;

    // Poniendo override y el metodo se pone solo
    protected override void Update()
    {
        base.Update(); // base.Metodo para tener el funcionamiento de awake del script enemy


        if (isDead)
        {
            return;
        }

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    public override void Die()
    {
        if (hasBody)
        {
            canMove = false;
            hasBody = false;
            animator.SetTrigger("hit");

            rb.velocity = Vector2.zero;
            idleDuration = 0;
        }
        else if (canMove == false && hasBody == false)
        {
            animator.SetTrigger("hit");
            canMove = true;
            moveSpeed = maxSpeed;
        }
        else
        {    
            base.Die();
        }

    }

    private void HandleTurnAround()
    {
        bool canFlipFromLedge = !isGroundInfrontDetected && hasBody;

        if (canFlipFromLedge || isWallDetected)
        {


            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
        {
            return;
        }

        if(canMove == false)
        {
            return;
        }

        rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);


    }

    private void CreateBody()
    {
        Enemy_SnailBody newBody = Instantiate(bodyPrefab, transform.position, Quaternion.identity);

        if (Random.Range(0, 100) < 50)
        {
            deathRotacionDirection = deathRotacionDirection * -1;
        }

        newBody.SetupBody(deathImpactSpeed, deathRotationSpeed * deathRotacionDirection, facingDirection);

        Destroy(newBody.gameObject, 10);
    }

    protected override void Flip()
    {
        base.Flip();

        if(hasBody == false)
        {
            animator.SetTrigger("wallHit");
        }
    }
}
