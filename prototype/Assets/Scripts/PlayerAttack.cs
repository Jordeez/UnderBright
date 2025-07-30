using System.Collections;
using UnityEngine;

public class PlayerAttackProjectile : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 2f;

    private Animator anim;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        anim.SetTrigger("attack");
    }


    public void ShootProjectile()
    {
        float direction = spriteRenderer.flipX ? 1f : -1f; 

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);


        Collider2D projCol = proj.GetComponent<Collider2D>();
        if (projCol != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(projCol, playerCollider);
        }


        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * projectileSpeed, 0f);
        }


        SpriteRenderer projSprite = proj.GetComponent<SpriteRenderer>();
        if (projSprite != null)
            projSprite.flipX = spriteRenderer.flipX;

        Destroy(proj, projectileLifetime);
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
}
