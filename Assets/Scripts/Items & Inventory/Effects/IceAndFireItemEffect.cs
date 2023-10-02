using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "iceAndFire", menuName = "Data/ItemEffect/IceAndFire")]
public class IceAndFireItemEffect : ItemEffect {
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _enemyPos) {
        Player player = PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttackState.comboCounter == 2;

        if (thirdAttack) {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _enemyPos.position, player.transform.rotation);

            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, 3);
        }
    }
}