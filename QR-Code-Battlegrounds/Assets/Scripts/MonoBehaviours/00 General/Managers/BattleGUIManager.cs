using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Do note that BattleGUIManager script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class BattleGUIManager : MonoBehaviour {
    
    // Privates
    private BattleSceneController battleSceneController;
    private ScenesController scenesController;

    #region private serialized variables (See able in inspector but no other classes)
    [SerializeField] private float textMessageWaitForDelay = 0.5f;
    [SerializeField] private GameObject backgroundQuad;
    
    [SerializeField] private GameObject textWindow;
    [SerializeField] private GameObject actionWindow;
    [SerializeField] private GameObject selectionWindow;
    [SerializeField] private GameObject yesNoWindow;

    // Backpack store window
    [SerializeField] private GameObject backpackStoreWindow;
    [SerializeField] private Text backpackStoreButtonText;

    [SerializeField] private GameObject runButton;
    [SerializeField] private GameObject bossButton;

    [SerializeField] private Text[] abilityButtonsText = new Text[Ability.KMaxAbilities];
    [SerializeField] private Text[] abilityPPText = new Text[Ability.KMaxAbilities];

    [SerializeField] private Text textWindowText;
    [SerializeField] private Text backButtonText;

    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    [SerializeField] private ParticleSystem enemyParticleSystem;
    [SerializeField] private ParticleSystem playerParticleSystem;
    #endregion

    // Properties
    public bool SwapActive { get; set; }
    public Text[] AbilityButtonsText { get { return abilityButtonsText; } }
    public Text[] AbilityPPText { get { return abilityPPText; } }


    // Methods
    private void Awake() {
        playerSpriteRenderer.enabled = false;
    }

    private void Start() {
        battleSceneController = BattleSceneController.Instance;
        scenesController = FindObjectOfType<ScenesController>();
        EnemySpriteSetup(GameManager.Instance.SelectedEnemy);
        RandomBackground();
    }
    
    public void EnemySpriteSetup (Enemy enemy) {
        // These should never be null unless changed in code, it is validated within the Sprites script
        if (enemy.FrontSprite != null)
            enemySpriteRenderer.sprite = enemy.FrontSprite;
        else
            Debug.LogError("No front sprite found for enemy " + enemy.name);
    }
    
    public void PlayerSpriteSetup(Player player) {
        // These should never be null unless changed in code, it is validated within the Sprites script
        if (player.BackSprite != null) {
            playerSpriteRenderer.enabled = true;
            playerSpriteRenderer.sprite = player.BackSprite;
        } else {
            Debug.LogError("No back sprite found for player " + player.name);
        }
    }
    
    void RandomBackground() {
        var sInstance = Sprites.Instance;
        Material mat = sInstance.battleSceneBackgroundMaterials[Random.Range(0, sInstance.BackgroundLength)];
        backgroundQuad.GetComponent<MeshRenderer>().material = mat;
    }

    // Update the action window buttosn to be interactable 
    // Attack, Heal, Run, Swap Character.  (Run and Swap Character will never be seen at the same time)
    public void UpdateActionWindowButtons (bool enabled) {
        foreach (Button b in actionWindow.GetComponentsInChildren<Button> ()) {
            b.interactable = enabled;
        }
    }

    // enabling yes no window, does the inverse to action window
    public void EnableYesNoWindow (bool enabled) {
        actionWindow.SetActive(!enabled);
        yesNoWindow.SetActive(enabled);
    }

    public void EnableYesNoButtons (bool enabled) {
        Button[] btns = yesNoWindow.GetComponentsInChildren<Button>();
        foreach (Button btn in btns) {
            btn.interactable = enabled;
        }
    }

    // enable backpack / store window 
    public void SetBackPackButtonText (string text) {
        backpackStoreButtonText.text = text;
    }

    public void BackPackButtonAction () {
        if (backpackStoreButtonText.text.Equals("Store")) {
            scenesController.LoadScene(ScenesEnum.store);
        } else {
            scenesController.LoadScene(ScenesEnum.backpack);
        }
    }
    
    public void AllDead(bool toStore) {
        if (toStore) {
            SetBackPackButtonText("Store");
        } else {
            SetBackPackButtonText("Backpack");
        }
        actionWindow.SetActive(false);
        backpackStoreWindow.SetActive(enabled);
    }

    public void HandleBoss() {
        // remove the runButton
        runButton.SetActive(false);

        // enable the boss swap character button
        bossButton.SetActive(true);
    }
    
    public void CharacterHasBeenSelected () {
        SwapActive = false;

        // Reset the playerSpriteRender to full alpha in case the swap character was invoked from dead character (playerSpriteRender fades to 0)
        Color c = playerSpriteRenderer.color;
        c.a = 1;
        playerSpriteRenderer.color = c;
    }

    public void SwapCharacter () {
        CharacterSelectionManager csm = FindObjectOfType<CharacterSelectionManager>();

        if (SwapActive) {
            csm.CloseSelectionWindow();
        } else {
            // Restarting
            csm.Begin();
            UpdateActionWindowButtons(false);
        }

        SwapActive = !SwapActive;
    }
    
    public void TextMessage(string message) {
        textWindowText.text = message;
    }

    public IEnumerator TextMessageCoroutine(string message) {
        textWindowText.text = "";

        foreach (char c in message.ToCharArray())
        {
            textWindowText.text += c;
            yield return null;
        }
        yield return new WaitForSeconds(textMessageWaitForDelay);
    }

    public IEnumerator TextMessageWaitForResponse(string message, IEnumerator iEnumeratorMethod) {
        textWindowText.text = "";

        foreach (char c in message.ToCharArray()) {
            textWindowText.text += c;
            yield return null;
        }

        yield return StartCoroutine(iEnumeratorMethod);
    }

    public IEnumerator WaitForYesOrNoInput() {
        EnableYesNoWindow(true);
        EnableYesNoButtons(true);

        float timePassed = 0;
        while (yesNoValue == -1 && timePassed < 30) {
            timePassed += Time.deltaTime;
            yield return null;
        }

        EnableYesNoButtons(false);
    }

    public IEnumerator WaitForAbilitySelected() {
        selectionWindow.SetActive(true);
        backButtonText.text = "Give Up";

        float timePassed = 0;
        while (removeAbilityIndex == -1 && timePassed < 30f) {
            timePassed += Time.deltaTime;
            yield return null;
        }

        selectionWindow.SetActive(false);
        backButtonText.text = "Back";
    }

    public bool removeAbility = false;
    public int removeAbilityIndex = -1;
    public int yesNoValue = -1;

    // yes is 1, no is 0
    public void YesNoSelected(IndexData indexData) {
        yesNoValue = indexData.IndexValue;
    }

    public void SelectionWindowBackButton () {
        if (removeAbility) {
            removeAbilityIndex = Ability.KMaxAbilities;
        }

        selectionWindow.SetActive(false);
    }

    // Ability selected also includes heal as an ability, passed with indexData index -1
    public void AbilitySelected(IndexData indexData) {
        var index = indexData.IndexValue;

        // if heal was selected, but the player is full health do nothing
        if (index == -1 && GameManager.Instance.SelectedPlayer.Stats_Component.IsFullHealth)
            return;

        // set all the action buttons to "false" not click able while the combat effects are happing
        UpdateActionWindowButtons(false);

        if (removeAbility) {
            removeAbilityIndex = index;
        } else {
            StartCoroutine(battleSceneController.TurnHandler(index));
        }
    }

    // Adds a death effect to the character that dead and loads the map scene after a delay
    public IEnumerator FadeForDeathAndMapLoadScene (bool isPlayer) {
        SpriteRenderer sprite = GetSpriteForCharacter(isPlayer);

        // FadeForDeath
        yield return StartCoroutine(FadeEffect(sprite, 0f, 0.5f));
 
        yield return new WaitForSeconds(.3f);

        if (isPlayer && GameManager.Instance.SelectedEnemy.IsBoss && GameManager.Instance.IsaCharacterAlive()) {
            yield return StartCoroutine(TextMessageWaitForResponse("Would you like to continue fight?", WaitForYesOrNoInput()));
            
            // 1 is yes
            if (yesNoValue == 1) {
                SwapCharacter();
                EnableYesNoWindow(false);
                yield break;
            }
        }

        scenesController.LoadScene(ScenesEnum.map);
    }

    public IEnumerator FadeForHit (bool isPlayer) {
        SpriteRenderer sprite = GetSpriteForCharacter(isPlayer);
        ParticleSystem ps = GetParticleSystemForCharacter(isPlayer);
        
        ps.Play();
        yield return StartCoroutine(FadeEffect(sprite, 0.5f, 0.25f));
        yield return StartCoroutine(FadeEffect(sprite, 1f, 0.25f));
        yield return StartCoroutine(FadeEffect(sprite, 0.5f, 0.25f));
        yield return StartCoroutine(FadeEffect(sprite, 1f, 0.25f));
        ps.Stop();
    }

    private ParticleSystem GetParticleSystemForCharacter(bool isPlayer) {
        return (isPlayer) ? playerParticleSystem : enemyParticleSystem;
    }

    private SpriteRenderer GetSpriteForCharacter (bool isPlayer) {
        return (isPlayer) ? playerSpriteRenderer : enemySpriteRenderer;
    }

    private IEnumerator FadeEffect (SpriteRenderer sprite, float finalAlpha, float fadeDuration) {
        Color color = sprite.color;

        float fadeSpeed = Mathf.Abs(color.a - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(color.a, finalAlpha))
        {
            color.a = Mathf.MoveTowards(color.a, finalAlpha, fadeSpeed * Time.deltaTime);
            sprite.color = color;
            yield return null;
        }
    }
}
