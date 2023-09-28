using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    [Header("Major Stats")]
    public Stat strength; // 1pt increase dmg by 1 and crit power by 1%
    public Stat agility; // 1pt increase evasion by 1% and crit chance by 1%
    public Stat intelligence; // 1pt increase magic damage by 1 and magic resistance by 3
    public Stat vitality; // 1pt increase health by 3 pts

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResist;

    [Header("Attack Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critDamage; // default value is 150%

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    [SerializeField] private int currentHealth;

    protected virtual void Start()
    {
        critDamage.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        // _targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(_targetStats);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetMagicRes(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);


    }

    private static int CheckTargetMagicRes(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResist.GetValue() + (_targetStats.intelligence.GetValue() + 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    public void ApplyAilments(bool _ignited, bool _chilled, bool _shocked)
    {
        if (isIgnited || isChilled || isShocked) return;

        isIgnited = _ignited;
        isChilled = _chilled;
        isShocked = _shocked;
    }

    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }

    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // throw new NotImplementedException();
    }

    private bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCritDamage(int _damage)
    {
        float critDamageCalc = (critDamage.GetValue() + strength.GetValue()) * .01f;
        float totalCritDamage = _damage * critDamageCalc;

        return Mathf.RoundToInt(totalCritDamage);
    }
}
