using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour {
    private Player player;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5f;

    private Transform closestEnemy;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestEnemy, Player _player) {
        player = _player;
        crystalTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestEnemy = _closestEnemy;
    }

    public void ChooseRandomEnemy() {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0) {
            closestEnemy = colliders[Random.Range(0, colliders.Length)].transform;
        }
    }

    private void Update() {
        crystalTimer -= Time.deltaTime;

        if (crystalTimer < 0) {
            CrystalExplosion();
        }

        if (canMove && closestEnemy != null) {
            transform.position = Vector2.MoveTowards(transform.position, closestEnemy.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestEnemy.position) < 1) {
                CrystalExplosion();
                canMove = false;
            }
        }

        if (canGrow) {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExplodeEvent() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders) {
            if (hit.GetComponent<Enemy>() != null) {
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());
                ItemDataEquipment equippedAmulet = InventoryManager.Instance.GetEquipment(EquipmentType.Amulet);

                if (equippedAmulet != null) {
                    equippedAmulet.ExecuteItemEffect(hit.transform);
                }
            }
        }
    }

    public void CrystalExplosion() {
        if (canExplode) {
            canGrow = true;
            anim.SetTrigger("Explode");
        } else {
            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}