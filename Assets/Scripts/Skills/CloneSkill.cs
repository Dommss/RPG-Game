using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone information")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;

    public void CreateClone(Transform _clonePosition)
    {
        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration);
    }
}
