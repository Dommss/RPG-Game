using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : Skill {
    [Header("Clone information")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [SerializeField] private bool canCreateCloneOnStart;
    [SerializeField] private bool canCreateCloneOnArrival;
    [SerializeField] private bool canCreateCloneOnCounterAttack;

    [Header("Duplication")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal X Clone")]
    public bool crystalXClone;

    public void CreateClone(Transform _clonePosition, Vector3 _offset) {
        if (crystalXClone) {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player);
    }

    public void CreateCloneOnDashStart() {
        if (canCreateCloneOnStart) {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CreateCloneOnDashOver() {
        if (canCreateCloneOnArrival) {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform) {
        if (canCreateCloneOnCounterAttack) {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(1.5f * player.facingDir, 0)));
        }
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset) {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }
}