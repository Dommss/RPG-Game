using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/ItemEffect")]
public class ItemEffect : ScriptableObject {

    public virtual void ExecuteEffect() {
        Debug.Log("Effect Executed!");
    }
}