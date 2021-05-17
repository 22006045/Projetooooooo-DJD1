using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum Faction { Friend, Enemy };

    [SerializeField]
    protected Faction     faction;
    [SerializeField]
    protected int         maxHealth = 100;
    [SerializeField]
    protected Transform   groundCheckObject;
    [SerializeField]
    protected float       groundCheckRadius = 3.0f;
    [SerializeField]
    protected LayerMask   groundCheckLayer;
    [SerializeField]
    protected float       invulnerabilityDuration = 2.0f;
    [SerializeField]
    protected float       blinkDuration = 0.2f;
    [SerializeField]
    protected float       knockbackVelocity = 30.0f;
    [SerializeField]
    protected float       knockbackDuration = 0.5f;
    [SerializeField]
    protected float       hitKnockbackDuration = 0.2f;
    [SerializeField]
    protected GameObject  deathPrefab;

    protected Rigidbody2D       rb;
    protected SpriteRenderer    spriteRenderer;
    protected Animator          animator;
    protected int               health;
    protected float             invulnerabilityTimer = 0;
    protected float             blinkTimer;
    protected float             knockbackTimer;

    public bool isDead => health <= 0;
    protected bool isInvulnerable { get { return (invulnerabilityTimer > 0); } }
    protected bool canHit { get { return (!isInvulnerable) && (!isDead); } }
    protected bool canMove { get { return (knockbackTimer <= 0) && (!isDead); } }

    protected virtual bool knockbackOnHit => true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        health = maxHealth;
    }

    protected virtual void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer = invulnerabilityTimer - Time.deltaTime;

            blinkTimer = blinkTimer - Time.deltaTime;
            if (blinkTimer <= 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;

                blinkTimer = blinkDuration;
            }
        }
        else
        {
            spriteRenderer.enabled = true;
        }

        if (knockbackTimer > 0)
        {
            knockbackTimer = knockbackTimer - Time.deltaTime;
        }
    }

    public void DealDamage(int damage, Vector2 hitDirection)
    {
        if (!canHit) return;

        health = health - damage;

        if (health == 0)
        {
            if (deathPrefab)
            {
                GameObject explosionObject = Instantiate(deathPrefab, transform.position, transform.rotation);
                explosionObject.transform.localScale = transform.localScale;

                Destroy(gameObject, 0.100f);
            }
            else
            {
                rb.velocity = Vector2.up * 300;
                knockbackTimer = 2.0f;
                Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false;
                }

                Destroy(gameObject, 2.0f);
            }

           
        }
        else
        {
            invulnerabilityTimer = invulnerabilityDuration;
            blinkTimer = blinkDuration;

            if (knockbackOnHit)
            {
                Vector2 knockback = hitDirection.normalized * knockbackVelocity + Vector2.up * knockbackVelocity * 7.0f;
                rb.velocity = knockback;

                knockbackTimer = knockbackDuration;
            }
        }
    }

    protected bool IsGround()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheckObject.position, groundCheckRadius, groundCheckLayer);

        return (collider != null);
    }

    protected void TurnTo(float dirX)
    {
        if (dirX < -0.1)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            currentRotation.y = 0;
            transform.rotation = Quaternion.Euler(currentRotation);
        }
        else if (dirX > 0.1)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            currentRotation.y = 180;
            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    

    public bool IsHostile(Faction faction)
    {
        if (faction != this.faction) return true;

        return false;
    }
}
