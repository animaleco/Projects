using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;
    [SerializeField] private DifficultyType gameDifficulty;
    private GameManager gameManager;

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D cd;

    private bool canBeControlled = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultGravityScale;
    private bool canDoubleJump;

    [Header("Buffer & Coyote jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1f; // soluciona que el personaje salte nada mas empezar
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1f;

    [Header("Wall interactions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;
    private bool canflip;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private float xInput;
    private float yInput;

    private bool isFacingRight = true;
    private int facingDirection = 1;

    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private int skinId;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();


    }

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        gameManager = GameManager.instance;

        UpdateGameDifficulty();
        RespawnFinished(false);
        UpdateSkin();
    }

   
    private void Update()
    {


        UpdateAirborneStatus();

        if (canBeControlled == false)
        {
            HandleCollision();
            HandleAnimations();
            return;
        }

        if (isKnocked)
        {
            return;
        }

        HandleEnemeyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision(); // Poner este metodo cerca de las animaciones hace que se actualice antes para solucionar bugs visuales
        HandleAnimations();
    }

    public void Damage()
    {
        if(gameDifficulty == DifficultyType.Normal)
        {

            if(gameManager.FruitsCollected() <= 0)
            {
                Die();
                gameManager.RestartLevel();
            }
            else
            {
                ObjectCreator.instance.CreateObject(fruitDrop, transform, true);
                gameManager.RemoveFruit();
            }

            return;
        }

        if(gameDifficulty == DifficultyType.Hard)
        {
            Die();
            gameManager.RestartLevel();    
        }
    }

    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null)
        {
            gameDifficulty = difficultyManager.difficulty;
        }
    }
    public void UpdateSkin()
    {
        SkinManager skinManager = SkinManager.instance;

        if (skinManager == null )
        {
            return;
        }

        animator.runtimeAnimatorController = animators[skinManager.choosenSkinId];
    }

    private void HandleEnemeyDetection()
    {
        if(rb.velocity.y >= 0)
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach (var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();

            if (newEnemy != null)
            {
                AudioManager.instance.PlaySFX(1);
                newEnemy.Die();
                Jump();
                
            }
        }
    }

    public void RespawnFinished(bool finished)
    {
        float gravityScale = rb.gravityScale;

        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

            AudioManager.instance.PlaySFX(11);
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
    }

    public void Knockback(float sourceDamageXposition)
    {
        float knockbackDir = 1;

        if (transform.position.x < sourceDamageXposition)
        {
            knockbackDir = -1;
        }

        if (isKnocked)
        {
            return;
        }

        AudioManager.instance.PlaySFX(9);
        CameraManager.instance.ScreenShake(knockbackDir);
        StartCoroutine(KnockbackRoutine());

        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        animator.SetBool("isKnocked", true);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        animator.SetBool("isKnocked", false);
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX(0);

        GameObject newDeathVfx = Instantiate(deathVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration)
    {
        StartCoroutine(PushCourutine(direction, duration));
    }

    private IEnumerator PushCourutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
    
    }

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
        {
            HandleLanding();
        }

        if (!isGrounded && !isAirborne)
        {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne()
    {
        isAirborne = true;

        if(rb.velocity.y < 0)
        {
          ActivateCoyoteJump();
            
        }

    }

    private void HandleLanding()
    {
        dustFx.Play();

        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");


        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    #region Buffer & Coyote Jump
    private void RequestBufferJump()
    {
        if(isAirborne)
        {
            bufferJumpActivated = Time.time;
        }
    }

    private void AttemptBufferJump()
    {
        if(Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCoyoteJump()
    {
        
        coyoteJumpActivated = Time.time;
    }

    private void CancelCoyoteJump()
    {
        
        coyoteJumpActivated = Time.time - 1;
    }
    #endregion

    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;
        
        if (isGrounded || coyoteJumpAvailable)
        {
            Debug.Log(Time.time + " :: " + (coyoteJumpActivated + coyoteJumpWindow) + " :: " + coyoteJumpAvailable);
            Jump();
            

        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
            
        }
        else if (canDoubleJump)
        {
            DoubleJump();
            
        }
        CancelCoyoteJump();
    }

    private void Jump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void DoubleJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);

        StopCoroutine(WallJumpRoutine());
        isWallJumping = false;
        canDoubleJump = false;
        rb.velocity = new Vector2 (rb.velocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(12);

        canDoubleJump = true;
        rb.velocity = new Vector2(wallJumpForce.x * -facingDirection, wallJumpForce.y);

        Flip();       

        StopAllCoroutines(); // Parar la "Coroutine" aquí hace que no se corte el WallJump si lo haces muy rápido
        StartCoroutine(WallJumpRoutine());

    }
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yModifer = yInput < 0 ? 1 : .05f; // VARIABLE = CONDICION ? SECUMPLE : NOSECUMPLE 

        if (canWallSlide == false)
        {
            return;
        }

        
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifer);

    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

       // canflip = false;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;

        //canflip = true;
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {        
        animator.SetFloat("xVelocity", rb.velocity.x);
        animator.SetFloat("yVelocity", rb .velocity.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if (isWallDetected)
        {
            return; // Interrumpe la función
        }
        if (isWallJumping)
        {
            return;
        }

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleFlip()
    {
        if(xInput < 0 && isFacingRight || xInput > 0 && !isFacingRight)
        {
            Flip();
           // dustFx.Play();
        
        }
    }

    private void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;
         
    }

    private void OnDrawGizmos()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        Gizmos.color = hit.collider == null ? Color.red : Color.green;
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);   
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDirection), transform.position.y));
    }
}
