using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    [Header("Major Stats")]
    public Stat strength;       // 1pt increase dmg by 1 and crit power by 1%
    public Stat agility;        // 1pt increase evasion by 1% and crit chance by 1%
    public Stat intelligence;   // 1pt increase magic damage by 1 and magic resistance by 3
    public Stat vitality;       // 1pt increase health by 3 pts

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResist;

    [Header("Attack Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critDamage;     // default value is 150%

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;      // does damage over time
    public bool isChilled;      // reduce armor by 20%
    public bool isShocked;      // reduce accuracy by 20%

    private float igniteTimer;
    private float chillTimer;
    private float shockTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    [SerializeField] private int currentHealth;

    protected virtual void Start()
    {
        critDamage.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
    }

    protected virtual void Update()
    {
        igniteTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (igniteTimer < 0)
        {
            isIgnited = false;
        }

        if (igniteDamageTimer < 0 && isIgnited)
        {
            Debug.Log("Take burn damage " + igniteDamage);
            currentHealth -= igniteDamage;
            igniteDamageTimer = igniteDamageCooldown;

            if (currentHealth < 0)
            {
                Die();
            }
        }

        if (chillTimer < 0)
        {
            isChilled = false;
        }

        if (shockTimer < 0)
        {
            isShocked = false;
        }
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

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0) return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .5f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
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

        if (_ignited)
        {
            isIgnited = _ignited;
            igniteTimer = 2;
        }

        if (_chilled)
        {
            isChilled = _chilled;
            chillTimer = 2;
        }

        if (_shocked)
        {
            isShocked = _shocked;
            shockTimer = 2;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (_targetStats.isShocked)
        {
            totalEvasion += 20;
        }

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
