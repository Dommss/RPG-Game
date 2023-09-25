using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{
    private float crystalTimer;

    public void SetupCrystal(float _crystalDuration)
    {
        crystalTimer = _crystalDuration;
    }

    private void Update()
    {
        crystalTimer -= Time.deltaTime;

        if (crystalTimer < 0)
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}