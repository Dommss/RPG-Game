using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour {
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMaterial;
    private Material originalMaterial;

    [Header("Ailment Colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    private void Start() {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public IEnumerator FlashFX() {
        sr.material = hitMaterial;
        Color currentColor = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(.2f);
        sr.color = currentColor;

        sr.material = originalMaterial;
    }

    private void RedColorBlink() {
        if (sr.color != Color.white) {
            sr.color = Color.white;
        } else {
            sr.color = Color.red;
        }
    }

    private void CancelColorChange() {
        CancelInvoke();
        sr.color = Color.white;
    }

    #region Fire Effects

    public void IgniteFXFor(float _seconds) {
        InvokeRepeating("IgniteColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IgniteColorFX() {
        if (sr.color != igniteColor[0]) {
            sr.color = igniteColor[0];
        } else {
            sr.color = igniteColor[1];
        }
    }

    #endregion Fire Effects

    #region Chill Effects

    public void ChillFXFor(float _seconds) {
        InvokeRepeating("ChillColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void ChillColorFX() {
        if (sr.color != chillColor[0]) {
            sr.color = chillColor[0];
        } else {
            sr.color = chillColor[1];
        }
    }

    #endregion Chill Effects

    #region Shock Effects

    public void ShockFXFor(float _seconds) {
        InvokeRepeating("ShockColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void ShockColorFX() {
        if (sr.color != shockColor[0]) {
            sr.color = shockColor[0];
        } else {
            sr.color = shockColor[1];
        }
    }

    #endregion Shock Effects
}