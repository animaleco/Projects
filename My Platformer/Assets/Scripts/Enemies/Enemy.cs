using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;

    protected Transform player;
    

    [Header("General info")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float idleDuration = 1.5f;
    protected bool canMove = true;
    protected float idleTimer;


    [Header("Death details")]
    [SerializeField] protected float deathImpactSpeed = 5;
    [SerializeField] protected float deathRotationSpeed = 150;
    protected int deathRotacionDirection = 1;
    protected bool isDead;

    [Header("Basic collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = .7f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float playerDetectionDistance;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected Transform groundCheck;
    protected bool isPlayerDetected;
    protected bool isGrounded;
    protected bool isWallDetected;
    protected bool isGroundInfrontDetected;   

    protected int facingDirection = -1;
    protected bool isFacingRight = false;

    protected virtual void Awake() //Para poder Sobreescribir el metodo en otro script
    {
        animator = GetComponent<Animator>();    
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        InvokeRepeating(nameof(UpdatePlayerReference), 0, 1);

        if(sr.flipX == true && !isFacingRight)
        {
            sr.flipX = false;
            Flip();
        }

        PlayerManager.OnPlayerRespawn += UpdatePlayerReference;
    }

    private void UpdatePlayerReference()
    {
        if(player != null)
        {
            player = PlayerManager.instance.player.transform;

        }
    }

    protected virtual void Update()
    {
        HandleAnimator();
        HandleCollision();

        idleTimer -= Time.deltaTime;

        if (isDead)
        {
          HandleDeathRotation();

        }
    }

    public virtual void Die()
    {
        foreach(var collider in colliders)
        {

            collider.enabled = false;

        }
       
        animator.SetTrigger("hit");
        rb.velocity = new Vector2(rb.velocity.x, deathImpactSpeed);
        isDead = true;

        if(Random.Range(0,100) < 50)
        {
            deathRotacionDirection = deathRotacionDirection * -1;
        }

        PlayerManager.OnPlayerRespawn -= UpdatePlayerReference;
        Destroy(gameObject, 10);
    }

    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotacionDirection) * Time.deltaTime);
        
    }

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && isFacingRight || xValue > transform.position.x && !isFacingRight)
        {
            Flip();
        }
    }

    protected virtual void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;

    }

    [ContextMenu("Change Facing Direction")]
    public void FlipDefaultFacingDirection()
    {
        sr.flipX = !sr.flipX;
    }

    protected virtual void HandleAnimator()
    {
        animator.SetFloat("xVelocity", rb.velocity.x);
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        isPlayerDetected = isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, playerDetectionDistance, whatIsPlayer);

    }

    protected virtual void OnDrawGizmos()
    {
       
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDirection), transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (playerDetectionDistance * facingDirection), transform.position.y));
    }
}
