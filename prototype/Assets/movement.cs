using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    private bool isGrounded;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Combo system
    private bool isAttacking = false;
    private bool attack1Done = false;
    private bool canChainAttack = false;

    private float comboWindowTimer = 0f;
    public float comboWindowTime = 0.4f;

    // Input buffer
    private bool bufferedAttack = false;
    private float inputBufferTimer = 0f;
    public float inputBufferTime = 0.25f;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttackCombo();
        HandleInputBufferTimer();
    }

    void HandleMovement()
    {
        Vector2 velocity = myRigidbody.velocity;
        anim.SetBool("isRunning", Mathf.Abs(velocity.x) > 0.1f);

        if (Input.GetKey(KeyCode.A))
        {
            velocity.x = -moveSpeed;
            FlipSprite(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            velocity.x = moveSpeed;
            FlipSprite(false);
        }
        else
        {
            velocity.x = 0f;
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
            anim.SetTrigger("Jump");
            anim.SetBool("isGrounded", false);
        }

        myRigidbody.velocity = velocity;
    }

    void HandleAttackCombo()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isAttacking)
            {
                // Only trigger attack1 if not already attacking
                anim.SetTrigger("attack1");
                isAttacking = true;
                ConsumeBufferedInput();
            }
            else
            {
                // Buffer input for attack2
                BufferAttackInput();
            }
        }

        if (attack1Done)
        {
            comboWindowTimer -= Time.deltaTime;

            if (canChainAttack && bufferedAttack)
            {
                anim.SetTrigger("attack2");
                ResetCombo();
                return;
            }

            if (comboWindowTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    void BufferAttackInput()
    {
        bufferedAttack = true;
        inputBufferTimer = inputBufferTime;
    }

    void ConsumeBufferedInput()
    {
        bufferedAttack = false;
        inputBufferTimer = 0f;
    }

    void HandleInputBufferTimer()
    {
        if (bufferedAttack)
        {
            inputBufferTimer -= Time.deltaTime;
            if (inputBufferTimer <= 0f)
            {
                bufferedAttack = false;
            }
        }
    }

    // Animation Event: Call this at the END of Attack1
    public void OnAttack1End()
    {
        isAttacking = false;
        attack1Done = true;
        canChainAttack = true;
        comboWindowTimer = comboWindowTime;
    }

    void ResetCombo()
    {
        isAttacking = false;
        attack1Done = false;
        canChainAttack = false;
        comboWindowTimer = 0f;
        ConsumeBufferedInput();
    }

    void FlipSprite(bool faceLeft)
    {
        Vector3 scale = transform.localScale;
        if ((faceLeft && scale.x > 0) || (!faceLeft && scale.x < 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
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
