using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderStrikeEffect", menuName = "Data/ItemEffect/ThunderStrike")]
public class ThunderStrikeItemEffect : ItemEffect {

    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyPos) {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPos.position, Quaternion.identity);
        Destroy(newThunderStrike, .5f);
    }
}
