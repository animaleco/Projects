using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Trunk : Enemy
{

    [Header("Trunk details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 7;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastTimeAttacked;

    // Poniendo override y el metodo se pone solo
    protected override void Update()
    {
        base.Update(); // base.Metodo para tener el funcionamiento de awake del script enemy


        if (isDead)
        {
            return;
        }

        bool canAttack = Time.time > lastTimeAttacked + attackCooldown;

        if (isPlayerDetected && canAttack)
        {
            Attack();
        }

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void Attack()
    {
        idleTimer = idleDuration + attackCooldown;
        lastTimeAttacked = Time.time;
        animator.SetTrigger("attack");
    }

    private void CreateBullet()
    {
        Enemy_Bullet newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);

        Vector2 bulletVelocity = new Vector2(facingDirection * bulletSpeed, 0);
        newBullet.SetVelocity(bulletVelocity);


        if(facingDirection == 1)
        {
            newBullet.FlipSprite();
        }

        Destroy(newBullet.gameObject, 10);
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
        if (idleTimer > 0)
        {
            return;
        }

        rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);


    }

}
