using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMaterial;
    private Material originalMaterial;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public IEnumerator FlashFX()
    {
        sr.material = hitMaterial;

        yield return new WaitForSeconds(.2f);

        sr.material = originalMaterial;
    }
}
