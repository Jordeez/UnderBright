using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    private KnockbackHandler knock;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Horizontal movement")]
    public float topSpeed = 8f;   
    public float acceleration = 80f;    
    public float deceleration = 60f;   
    public float velPower = 1f;  
    public float frictionAmount = 0.2f;   

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpCutMultiplier = 0.5f;  


    private float moveInput;              
    private bool isGrounded;
    private bool isJumping;
    private float lastGroundedTime;        
    private const float groundedTolerance = 0.1f;


    void Awake()
    {
        knock = GetComponent<KnockbackHandler>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        moveInput = 0;
        if (Input.GetKey(KeyCode.A)) moveInput = -1;
        if (Input.GetKey(KeyCode.D)) moveInput = 1;


        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !knock.IsKnockedBack)
            Jump();
        if (Input.GetKeyUp(KeyCode.W))
            OnJumpUp();

        if (moveInput < 0) spriteRenderer.flipX = true;
        if (moveInput > 0) spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyFriction();
        anim.SetBool("isRunning", Mathf.Abs(moveInput) > 0.01f && isGrounded);
    }




    void ApplyHorizontalMovement()
    {
        if (knock.IsKnockedBack) return;


        float targetSpeed = moveInput * topSpeed;


        float speedDif = targetSpeed - rb.linearVelocity.x;


        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;


        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
    }



    void ApplyFriction()
    {

        if (isGrounded && Mathf.Abs(moveInput) < 0.01f && Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {

            float amount = Mathf.Min(Mathf.Abs(rb.linearVelocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.linearVelocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }



    void Jump()
    {
        isGrounded = false;
        isJumping = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("Jump");
        anim.SetBool("isGrounded", false);
    }


    void OnJumpUp()
    {
        if (rb.linearVelocity.y > 0 && isJumping)
        {

            float cut = rb.linearVelocity.y * (1 - jumpCutMultiplier);
            rb.AddForce(Vector2.down * cut, ForceMode2D.Impulse);
        }
        isJumping = false;
    }


    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
        {

            isGrounded = true;
            isJumping = false;
            lastGroundedTime = groundedTolerance;
            anim.SetBool("isGrounded", true);
        }
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
            lastGroundedTime = groundedTolerance;
    }

    void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
            isGrounded = false;
    }
}
