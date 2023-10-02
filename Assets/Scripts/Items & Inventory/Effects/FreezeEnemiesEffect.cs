using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FreezeEnemiesEffect", menuName = "Data/ItemEffect/FreezeEnemiesEffect")]
public class FreezeEnemiesEffect : ItemEffect {
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _enemyPos) {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats.currentHealth > playerStats.GetMaxHealthValue() * .15f) return;

        if (!InventoryManager.Instance.CanUseArmor()) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_enemyPos.position, 2);

        foreach (var hit in colliders) {
            hit.GetComponent<Enemy>()?.StartCoroutine("FreezeTimeFor", duration);
        }
    }
}