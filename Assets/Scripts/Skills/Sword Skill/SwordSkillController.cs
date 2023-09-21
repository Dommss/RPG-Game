using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{

    [SerializeField] private float returnSpeed = 12f;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    [Header("Bounce")]
    [SerializeField] private float bounceSpeed;
    private bool isBouncing;
    private int amountOfBounces;
    private List<Transform> enemyTarget;
    private int targetIndex;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        player = GetComponent<Player>();
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        anim.SetBool("Rotation", true);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces)
    {
        isBouncing = _isBouncing;
        amountOfBounces = _amountOfBounces;

        enemyTarget = new List<Transform>();
    }

    public void ReturnSword()
    {
        anim.SetBool("Rotation", true);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                player.CatchSword();
            }
        }

        BounceLogic();
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                targetIndex++;
                amountOfBounces--;

                if (amountOfBounces <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
        { return; }

        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
        StuckInto(collision);
    }

    private void StuckInto(Collider2D collision)
    {

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;
        }

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
