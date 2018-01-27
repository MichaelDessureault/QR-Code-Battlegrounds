using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Backpack script handles everything rated to the backpack scene
/// Do note that Backpack script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class Backpack : MonoBehaviour {

    private StoreData potionInstance;
    public CharacterGuiSetup characterGuiSetup;

    #region Potion
    public Text potionAmountText;
    public Button potionButton;
    private bool potionActive = false;
    private bool buyPotions = false;
    private int potionAmount;
    #endregion

    #region Ether
    public Text etherAmountText;
    public Button etherButton;
    private bool etherActive = false;
    private bool buyEthers = false;
    private int etherAmount;
    #endregion
    
    #region assassin variables
    private Player  assassin;
    public RawImage assassinBoarder;
    public Button   assassinButton;
    private bool assassinHealable;
    private bool assassinEtherable;
    #endregion
    
    #region guardian variables
    private Player  guardian;
    public RawImage guardianBoarder;
    public Button   guardianButton;
    private bool guardianHealable;
    private bool guardianEtherable;
    #endregion
    
    #region wizard variables
    private Player  wizard;
    public RawImage wizardBoarder;
    public Button   wizardButton;
    private bool wizardHealable;
    private bool wizardEtherable;
    #endregion


    private ScenesController scenesController;

    private void Awake()
    {
        scenesController = FindObjectOfType<ScenesController>();
        if (scenesController == null)
            scenesController = new GameObject("ScenesController").AddComponent<ScenesController>();

        potionInstance = StoreData.Instance;
        potionAmount = potionInstance.PotionAmount;
        etherAmount = potionInstance.EtherAmount;

        if (characterGuiSetup == null)
            characterGuiSetup = FindObjectOfType<CharacterGuiSetup>();

        if (assassinBoarder == null || guardianBoarder == null || wizardBoarder == null)
        {
            Debug.LogError("a boarder is null");
        }

        if (potionAmount <= 0)
            potionButton.GetComponentInChildren<Text>().text = "Buy Potions";

        if (etherAmount <= 0)
            potionButton.GetComponentInChildren<Text>().text = "Buy Ethers";

        potionAmountText.text = potionAmount.ToString();
        etherAmountText.text = etherAmount.ToString();
    }

    private void Start()
    {
        GameManager gmInstance = GameManager.Instance;
        assassin = gmInstance.Assassin;
        guardian = gmInstance.Guardian;
        wizard = gmInstance.Wizard;


        assassinBoarder.enabled = false;
        guardianBoarder.enabled = false;
        wizardBoarder.enabled   = false;
        
        assassinHealable = assassin.IsHealable();
        guardianHealable = guardian.IsHealable();
        wizardHealable = wizard.IsHealable();

        assassinEtherable = assassin.IsEtherable();
        guardianEtherable = guardian.IsEtherable();
        wizardEtherable = wizard.IsEtherable();
    }

    #region Potion Methods
    public void PotionAction ()
    {
        if (potionAmount <= 0) {
            scenesController.LoadScene(ScenesEnum.store);
            return;
        }

        etherActive = false;
        DeactivateAll();

        // if button is clicked again while potion is active then deactive potion
        potionActive = !potionActive;

        if (assassinHealable)
            UpdateAssassinBoarderAndButton(potionActive);
        if (guardianHealable)
            UpdateGuardianBoarderAndButton(potionActive);
        if (wizardHealable)
            UpdateWizardBoarderAndButton(potionActive);
    }
    
    public void HealCharacter (IndexData index)
    {
        if (!potionActive)
            return;

        switch (index.IndexValue)
        {
            case 0:
                assassinHealable = false;
                assassin.Heal();
                StartCoroutine(characterGuiSetup.assassinHealthBar.BeginHealing(assassin.Stats_Component.MaxHp));
                break;
            case 1:
                guardianHealable = false;
                guardian.Heal();
                StartCoroutine(characterGuiSetup.guardianHealthBar.BeginHealing(guardian.Stats_Component.MaxHp));
                break;
            case 2:
                wizardHealable = false;
                wizard.Heal();
                StartCoroutine(characterGuiSetup.wizardHealthBar.BeginHealing(wizard.Stats_Component.MaxHp));
                break;
            default:
                print("Backpack: Out of range value for potion characters");
                return;
        }

        DeactivateAll();
        UpdatePotion();
    }
    
    void UpdatePotion()
    {
        // a character was healed
        potionActive = false;
        potionInstance.PotionUsed();
        potionAmount--;
        potionAmountText.text = potionAmount.ToString();

        if (potionAmount <= 0)
            etherButton.GetComponentInChildren<Text>().text = "BUY POTIONS";
    }
    #endregion

    #region Ether Methods
    public void EtherAction() {
        if (etherAmount <= 0) {
            scenesController.LoadScene(ScenesEnum.store);
            return;
        }

        potionActive = false;
        DeactivateAll();

        etherActive = !etherActive;

        if (assassinEtherable)
            UpdateAssassinBoarderAndButton(etherActive);
        if (guardianEtherable)
            UpdateGuardianBoarderAndButton(etherActive);
        if (wizardEtherable)
            UpdateWizardBoarderAndButton(etherActive);
    }

    public void EtherCharacter(IndexData index)
    {
        if (!etherActive)
            return;

        switch (index.IndexValue)
        {
            case 0:
                assassinEtherable = false;
                assassin.EtherUsed();
                characterGuiSetup.UpdateAbilities(assassin);
                break;
            case 1:
                guardianEtherable = false;
                guardian.EtherUsed();
                characterGuiSetup.UpdateAbilities(guardian);
                break;
            case 2:
                wizardEtherable = false;
                wizard.EtherUsed();
                characterGuiSetup.UpdateAbilities(wizard);
                break;
            default:
                print("Backpack: Out of range value for ether character");
                return;
        }

        DeactivateAll();
        UpdateEther();
    }
    
    void UpdateEther ()
    {
        // a character was healed
        etherActive = false;
        potionInstance.EtherUsed();
        etherAmount--;
        etherAmountText.text = etherAmount.ToString();

        if (etherAmount <= 0)
            etherButton.GetComponentInChildren<Text>().text = "BUY ETHERS";
    }
    #endregion

    #region Boarders and Buttons
    void UpdateAssassinBoarderAndButton (bool active)
    {
        assassinBoarder.enabled = active;
        assassinButton.enabled = active;
    }

    void UpdateGuardianBoarderAndButton (bool active)
    {
        guardianBoarder.enabled = active;
        guardianButton.enabled = active;
    }

    void UpdateWizardBoarderAndButton (bool active)
    {
        wizardBoarder.enabled = active;
        wizardButton.enabled = active;
    }

    public void DeactivateAll()
    {
        UpdateAssassinBoarderAndButton(false);
        UpdateGuardianBoarderAndButton(false);
        UpdateWizardBoarderAndButton(false);
    }
    #endregion
}