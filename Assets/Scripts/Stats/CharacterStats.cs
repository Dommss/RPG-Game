using UnityEngine;

public class CharacterStats : MonoBehaviour {
    private EntityFX fx;

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
    private int shockDamage;
    [SerializeField] private GameObject thunderStrikePrefab;

    public int currentHealth;
    public bool isDead { get; private set; }

    public System.Action onHealthChange;

    protected virtual void Start() {
        critDamage.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update() {
        igniteTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (igniteTimer < 0) {
            isIgnited = false;
        }

        ApplyIgniteDamage();

        if (chillTimer < 0) {
            isChilled = false;
        }

        if (shockTimer < 0) {
            isShocked = false;
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats) {
        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit()) {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        // DoMagicalDamage(_targetStats);
    }

    #region Magical Damage and Ailments

    public virtual void DoMagicalDamage(CharacterStats _targetStats) {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetMagicRes(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0) return;
        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage) {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock) {
            if (Random.value < .5f && _fireDamage > 0) {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0) {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightningDamage > 0) {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite) {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        if (canApplyShock) {
            _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignited, bool _chilled, bool _shocked) {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignited && canApplyIgnite) {
            isIgnited = _ignited;
            igniteTimer = 2;

            fx.IgniteFXFor(igniteTimer);
        }

        if (_chilled && canApplyChill) {
            isChilled = _chilled;
            chillTimer = 2;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, chillTimer);
            fx.ChillFXFor(chillTimer);
        }

        if (_shocked && canApplyShock) {
            if (!isShocked) {
                ApplyShock(_shocked);
            } else {
                if (GetComponent<Player>() != null) return;

                HitNearestTargetWithThunderStrike();
            }
        }
    }

    public void ApplyShock(bool _shocked) {
        if (isShocked) return;

        isShocked = _shocked;
        shockTimer = 2;

        fx.ShockFXFor(shockTimer);
    }

    private void ApplyIgniteDamage() {
        if (igniteDamageTimer < 0 && isIgnited) {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead) {
                Die();
            }

            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    private void HitNearestTargetWithThunderStrike() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders) {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1) {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance) {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null) {
                closestEnemy = transform;
            }
        }

        if (closestEnemy != null) {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);
            newThunderStrike.GetComponent<ShockStrikeController>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupThunderStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion Magical Damage and Ailments

    public virtual void TakeDamage(int _damage) {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine(fx.FlashFX());

        if (currentHealth < 0 && !isDead) {
            Die();
        }
    }

    public virtual void IncreaseHealthBy(int _amount) {
        currentHealth += _amount;

        if (currentHealth > GetMaxHealthValue()) {
            currentHealth = GetMaxHealthValue();
        }

        if (onHealthChange != null) {
            onHealthChange();
        }
    }

    protected virtual void DecreaseHealthBy(int _damage) {
        currentHealth -= _damage;

        if (onHealthChange != null) {
            onHealthChange();
        }
    }

    protected virtual void Die() {
        isDead = true;
    }

    #region Stat Calculations

    public bool TargetCanAvoidAttack(CharacterStats _targetStats) {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (_targetStats.isShocked) {
            totalEvasion += 20;
        }

        if (Random.Range(0, 100) < totalEvasion) {
            return true;
        }
        return false;
    }

    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage) {
        if (_targetStats.isChilled) {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        } else {
            totalDamage -= _targetStats.armor.GetValue();
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetMagicRes(CharacterStats _targetStats, int totalMagicDamage) {
        totalMagicDamage -= _targetStats.magicResist.GetValue() + (_targetStats.intelligence.GetValue() + 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    private bool CanCrit() {
        int totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCritChance) {
            return true;
        }

        return false;
    }

    private int CalculateCritDamage(int _damage) {
        float critDamageCalc = (critDamage.GetValue() + strength.GetValue()) * .01f;
        float totalCritDamage = _damage * critDamageCalc;

        return Mathf.RoundToInt(totalCritDamage);
    }

    public int GetMaxHealthValue() {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion Stat Calculations
}