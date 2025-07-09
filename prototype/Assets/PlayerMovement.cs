using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    private KnockbackHandler knock;
    private bool isGrounded;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private bool isAttacking = false;
    private float inputBufferTimer = 0f;
    private bool attack1Buffered = false;
    private bool attack2Buffered = false;
    public float inputBufferTime = 0.25f;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private float moveInputX;

    void Start()
    {
        knock = GetComponent<KnockbackHandler>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInputX = 0f;
        if (Input.GetKey(KeyCode.A)) moveInputX = -1f;
        if (Input.GetKey(KeyCode.D)) moveInputX = 1f;

        if (moveInputX < 0f) FlipSprite(true);
        if (moveInputX > 0f) FlipSprite(false);

        HandleAttackCombo();
        HandleJumpInput();
        TickInputBuffer();
    }

    void FixedUpdate()
    {
        Vector2 vel = myRigidbody.linearVelocity;

        if (!knock.IsKnockedBack)
        {
            vel.x = moveInputX * moveSpeed;
        }

        vel += knock.CurrentForce;

        myRigidbody.linearVelocity = vel;

        anim.SetBool("isRunning", Mathf.Abs(moveInputX) > 0.1f && isGrounded);
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !knock.IsKnockedBack)
        {
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetTrigger("Jump");
            anim.SetBool("isGrounded", false);
        }
    }

    void HandleAttackCombo()
    {
        if (inputBufferTimer <= 0f)
        {
            attack1Buffered = false;
            attack2Buffered = false;
        }

        if (Input.GetKeyDown(KeyCode.J) && isGrounded)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            if (!isAttacking)
            {
                if (!attack1Buffered)
                {
                    attack1Buffered = true;
                    anim.SetTrigger("attack1");
                    foreach (Collider2D enemy in hitEnemies)
                    {
                        Debug.Log("enemy hit");
                    }
                    inputBufferTimer = inputBufferTime;
                }
                else if (attack1Buffered && !attack2Buffered)
                {
                    attack2Buffered = true;
                    anim.SetTrigger("attack2");
                    ResetAttackBuffer();
                }
            }
        }
    }

    void TickInputBuffer()
    {
        if (inputBufferTimer > 0f)
        {
            inputBufferTimer -= Time.deltaTime;
        }
    }

    void ResetAttackBuffer()
    {
        attack1Buffered = false;
        attack2Buffered = false;
        inputBufferTimer = 0f;
    }

    void FlipSprite(bool faceLeft)
    {
        Vector3 scale = transform.localScale;
        if ((faceLeft && scale.x > 0f) || (!faceLeft && scale.x < 0f))
        {
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }
}