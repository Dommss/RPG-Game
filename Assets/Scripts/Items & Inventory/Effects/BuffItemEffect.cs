using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType {
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critDamage,
    maxHealth,
    armor,
    evasion,
    magicResist,
    fireDamage,
    iceDamage,
    lightningDamage
}

[CreateAssetMenu(fileName = "BuffEffect", menuName = "Data/ItemEffect/BuffEffect")]
public class BuffItemEffect : ItemEffect {
    private PlayerStats playerStats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private int buffDuration;

    public override void ExecuteEffect(Transform _enemyPos) {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.IncreaseStatBy(buffAmount, buffDuration, StatToModify());
    }

    private Stat StatToModify() {
        if (buffType == StatType.strength) return playerStats.strength;
        else if (buffType == StatType.agility) return playerStats.agility;
        else if (buffType == StatType.intelligence) return playerStats.intelligence;
        else if (buffType == StatType.vitality) return playerStats.vitality;
        else if (buffType == StatType.damage) return playerStats.damage;
        else if (buffType == StatType.critChance) return playerStats.critChance;
        else if (buffType == StatType.critDamage) return playerStats.critDamage;
        else if (buffType == StatType.maxHealth) return playerStats.maxHealth;
        else if (buffType == StatType.armor) return playerStats.armor;
        else if (buffType == StatType.evasion) return playerStats.evasion;
        else if (buffType == StatType.magicResist) return playerStats.magicResist;
        else if (buffType == StatType.fireDamage) return playerStats.fireDamage;
        else if (buffType == StatType.iceDamage) return playerStats.iceDamage;
        else if (buffType == StatType.lightningDamage) return playerStats.lightningDamage;

        return null;
    }
}