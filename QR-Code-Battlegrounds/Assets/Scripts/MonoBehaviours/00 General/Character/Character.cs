using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
    // Protected Variables
    protected Stats stats_component;

    protected List<Skill> abilitySet;
    protected Skill selectedAbility;

    // Private variables
    private BattleGUIManager _battleGUIManager;
    private HealthBarController _healthBarController;
    private BattleSceneController _battleSceneController;
    
    // Properties
    public List<Skill> AbilitySet { get { return abilitySet; } set { abilitySet = value; } }

    public StylesEnum.CombatTypes CombatType { get; set; }

    public Skill SelectedAbility {
        get { return selectedAbility; }
    }

    public new string name { get; set; }

    public bool IsDead {
        get { return stats_component.IsDead;  }
        set { stats_component.IsDead = value; }
    }

    public Stats Stats_Component {
        get { return stats_component; }
        set { stats_component = value; }
    }

    // Methods 
    public void AbilitySetSetup() {
        if (abilitySet == null) {
            abilitySet = Ability.Instance.GetAbilitySetForCombatType(stats_component.CombatType, stats_component.Level);
        }
    }
    public BattleGUIManager battleGUIManager {
        get {
            if (_battleGUIManager == null) {
                _battleGUIManager = FindObjectOfType<BattleGUIManager>();

                if (_battleGUIManager == null) {
                    Debug.LogError("No object found that contains the BattleGUIManager script within "
                        + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " scene");
                }
            }

            return _battleGUIManager;
        }
    }

    public HealthBarController healthBarController {
        get {
            if (_healthBarController == null) {
                _healthBarController = FindObjectOfType<HealthBarController>();

                if (_healthBarController == null) {
                    Debug.LogError("No object found that contains the HealthBarController script within "
                        + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " scene");
                }
            }

            return _healthBarController;
        }
    }

    public BattleSceneController battleSceneController {
        get {
            if (_battleSceneController == null)
                _battleSceneController = BattleSceneController.Instance;

            return _battleSceneController;
        }
    }

    public Skill GetAbilityAtIndex(int index) {
        return AbilitySet[index];
    }

    public abstract IEnumerator TakeDamage(int damage);

    public void Heal () {
        stats_component.Hp = stats_component.MaxHp;
        IsDead = false;
    }

    public abstract IEnumerator HealWithCoroutine();

    public void Dead() {
        StartCoroutine(DeadCoroutine());
    }

    protected abstract IEnumerator DeadCoroutine();
}