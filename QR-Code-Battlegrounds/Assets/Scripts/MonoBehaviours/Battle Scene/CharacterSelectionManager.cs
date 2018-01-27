using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Do note that CharacterSelectionManager script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class CharacterSelectionManager : MonoBehaviour {

    // Public
    public Color deathColor;

    // Serialized Private Fields
    [SerializeField] private Image enemyBlur;
    [SerializeField] private Image characterBlur;

    [SerializeField] private GameObject characterSelectionContainer;

    [SerializeField] private RawImage[] charactersRawImageArray = new RawImage[GameManager.numberOfCharacters];
    [SerializeField] private HealthBarWithCoroutine[] charactersHealthBarArray = new HealthBarWithCoroutine[GameManager.numberOfCharacters];

    // Private
    private GameManager gmInstance;
    private BattleSceneController battleSceneController;
    private BattleGUIManager battleGUIManager;
    private Player[] characters;

    private string allDeadMessage = "All characters are dead.";

    private string storeMessage = "Buy potions to revive";
    private string backpackMessage = "Use a potion to revive a character";

    private string bossMessage = "You have incounted a boss!";
    private string normalMessage = "Please select a character";

    private bool enemyBeenSetup = false;

    private void Start() {
        gmInstance = GameManager.Instance;
        
        battleSceneController = FindObjectOfType<BattleSceneController>();
        battleGUIManager = battleSceneController.battleGUIManager;
        characters = gmInstance.GetCharactersArrayAsPlayer();

        if (!gmInstance.IsaCharacterAlive()) {

            var potionAmount = StoreData.Instance.PotionAmount;

            bool tostore = (potionAmount <= 0); // else to backpack

            var message = allDeadMessage + "\n" + ((potionAmount <= 0) ? storeMessage : backpackMessage);

            battleGUIManager.TextMessage(message);
            battleGUIManager.AllDead(tostore);
        } else if (gmInstance.SelectedEnemy.IsBoss) {
            battleGUIManager.TextMessage(bossMessage + "\n" + normalMessage);
        } else {
            battleGUIManager.TextMessage(normalMessage);
        }

        Begin();
    }
    
    public void Begin() {
        // check all characters hp's
        // if a character is dead then they can not be selected
        characterSelectionContainer.SetActive(true);
        characterBlur.gameObject.SetActive(true);
        CharacterButtonSetup(characters);
        HealthBarSetup(characters);
    }

    // if a character is dead than the buttons will be disabled and the colour of the 
    // character will be set to the deathColor
    private void CharacterButtonSetup(Player[] characters)
    {
        int index = 0;
        foreach (Player p in characters)
        {
            if (p.Stats_Component.Hp == 0)
            {
                RawImage rawImage = charactersRawImageArray[index];
                rawImage.color = deathColor;

                var button = rawImage.gameObject.GetComponent<Button>();
                if (button != null)
                    button.enabled = false;
            }
            index++;
        }
    }

    private void HealthBarSetup (Player[] characters)
    {
        for (int i = 0; i < charactersHealthBarArray.Length; i++)
        {
            charactersHealthBarArray[i].SetUp(characters[i]);
        }
    }
    
    public void CharacterSelected(IndexData indexData) {

        // check for invalid inputs
        if (indexData.IndexValue < 0 || indexData.IndexValue >= GameManager.numberOfCharacters)
        {
            Debug.LogWarning("index passed for characterselected is out of range");
            return;
        }
        
        battleSceneController.battleGUIManager.CharacterHasBeenSelected();

        gmInstance.SetSelectedCharacterByIndex(indexData.IndexValue);

        // Warning: order matters for player muts be started before enemy. Enemy requires a reference to the players stats_component

        // player setup
        battleSceneController.PlayerBegin();
        characterBlur.gameObject.SetActive(false);

        // enemy setup, prevents the enemy from refreshing if the player swap'd characters
        if (!enemyBeenSetup) {
            battleSceneController.EnemyBegin();
            enemyBlur.gameObject.SetActive(false);
            enemyBeenSetup = true;
        }

        
        // Close selection widnow
        CloseSelectionWindow();

    }

    public void CloseSelectionWindow () {
        // disable character selection
        characterSelectionContainer.SetActive(false);
    }
}
