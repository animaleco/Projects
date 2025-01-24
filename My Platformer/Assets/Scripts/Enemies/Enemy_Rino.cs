using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Rino : Enemy
{

    [Header("Rino details")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedUpRate = .6f;
    private float defaultSpeed;
    [SerializeField] private Vector2 impactPower;

    [Header("Effects")]
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private Vector2 cameraImpulseDir;
    private CinemachineImpulseSource impulseSource;

    private float moveSpeeed;


    protected override void Start()
    {
        base.Start();

        canMove = false;
        defaultSpeed = base.moveSpeed;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        
    }
    protected override void Update()
    {
        base.Update();
        HandleCharge();    

    }

    private void HitWallImpact()
    {
        dustFx.Play();
        impulseSource.m_DefaultVelocity = new Vector2(cameraImpulseDir.x * facingDirection, cameraImpulseDir.y);
        impulseSource.GenerateImpulse();
    }

    private void HandleCharge()
    {
        if (canMove == false)
        {
            return;
        }

        HandleSpeedUp();

        rb.velocity = new Vector2(base.moveSpeed * facingDirection, rb.velocity.y);

        if (isWallDetected)
        {
            wallHit();
            
        }

        if (!isGroundInfrontDetected)
        {
            TurnAround();
        }


    }

    private void TurnAround()
    {
        SpeedReset();
        canMove = false;
        rb.velocity = Vector2.zero;
        Flip();
        
    }

   

    private void HandleSpeedUp()
    {
        moveSpeeed = base.moveSpeed + (Time.deltaTime * speedUpRate);

        if (base.moveSpeed >= maxSpeed)
        {
            maxSpeed = base.moveSpeed;
        }
    }


    private void wallHit()
    {
        canMove = false; 

        HitWallImpact();
        SpeedReset();


        animator.SetBool("hitWall", true);
        rb.velocity = new Vector2(impactPower.x * -facingDirection, impactPower.y);
    }

    private void SpeedReset()
    {
        base.moveSpeed = defaultSpeed;
    }

    private void ChargeIsOver()
    {
        
        animator.SetBool("hitWall", false);
        Invoke(nameof(Flip),1);
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();

       
        if(isPlayerDetected && isGrounded)
        {
            canMove = true;
            
        }
    }

}
