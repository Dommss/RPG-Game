using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour {

    #region Components

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    #endregion Components

    [Header("Knockback")]
    [SerializeField] protected Vector2 knockbackDir;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    [Header("Collision")]
    public Transform attackCheck;

    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake() {
    }

    protected virtual void Start() {
        fx = GetComponent<EntityFX>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update() {
    }

    public virtual void DamageEffect() {
        fx.StartCoroutine(fx.FlashFX());
        StartCoroutine(HitKnockback());
    }

    protected virtual IEnumerator HitKnockback() {
        isKnocked = true;
        rb.velocity = new Vector2(knockbackDir.x * -facingDir, knockbackDir.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        SetZeroVelocity();
    }

    #region Velocity

    public virtual void SetZeroVelocity() {
        if (isKnocked == true) {
            return;
        }

        rb.velocity = new Vector2(0, 0);
    }

    public virtual void SetVelocity(float _xVelocity, float _yVelocity) {
        if (isKnocked == true) {
            return;
        }

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    #endregion Velocity

    #region Collision

    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public virtual bool isWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos() {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion Collision

    #region Flipping

    public virtual void Flip() {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFlipped != null) {
            onFlipped();
        }
    }

    public virtual void FlipController(float _x) {
        if (_x > 0 && !facingRight) {
            Flip();
        } else if (_x < 0 && facingRight) {
            Flip();
        }
    }

    #endregion Flipping

    public void MakeTransparent(bool _transparent) {
        if (_transparent) {
            sr.color = Color.clear;
        } else {
            sr.color = Color.white;
        }
    }

    public virtual void Die() {
    }
}