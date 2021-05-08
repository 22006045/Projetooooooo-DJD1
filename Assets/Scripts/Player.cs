using UnityEngine;

public class Player : Character
{
    [SerializeField]
    private float       moveSpeed = 20.0f;
    [SerializeField]
    private float       jumpSpeed = 100.0f;
    [SerializeField]
    private float       maxJumpTime = 0.1f;
    [SerializeField]
    private int         maxJumps = 1;
    [SerializeField]
    private int         jumpGravityStart = 1;
    [SerializeField]
    Collider2D          groundCollider;
    [SerializeField]
    Collider2D          airCollider;

    private float           hAxis;
    private int             nJumps;
    private float           timeOfJump;

    void FixedUpdate()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(hAxis * moveSpeed, rb.velocity.y);
        }
    }

    protected override void Update()
    {
        hAxis = Input.GetAxis("Horizontal");

        bool isGround = IsGround();

        if ((isGround) && (Mathf.Abs(rb.velocity.y) < 0.1f))
        {
            nJumps = maxJumps;
        }

        Vector2 currentVelocity = rb.velocity;

        if ((Input.GetButtonDown("Jump")) && (nJumps > 0))
        {
            currentVelocity.y = jumpSpeed;

            rb.velocity = currentVelocity;

            nJumps--;

            rb.gravityScale = jumpGravityStart;

            timeOfJump = Time.time;
        }
        else 
        {
            float elapsedTimeSinceJump = Time.time - timeOfJump;
            if ((Input.GetButton("Jump")) && (elapsedTimeSinceJump < maxJumpTime))
            {
                rb.gravityScale = jumpGravityStart;
            }
            else
            {
                rb.gravityScale = 5.0f;
            }
        }

        TurnTo(-currentVelocity.x);

        animator.SetFloat("AbsSpeedX", Mathf.Abs(currentVelocity.x));
        animator.SetFloat("SpeedY", currentVelocity.y);
        animator.SetBool("OnGround", isGround);

        if (!isDead)
        {
            groundCollider.enabled = isGround;
            airCollider.enabled = !isGround;
        }

        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();
        if (character != null)
        {
            if (rb.velocity.y < -50.0f)
            {
                character.DealDamage(1, Vector2.down);

                rb.velocity = Vector2.up * knockbackVelocity;

                knockbackTimer = hitKnockbackDuration;
            }
        }
    }
}
