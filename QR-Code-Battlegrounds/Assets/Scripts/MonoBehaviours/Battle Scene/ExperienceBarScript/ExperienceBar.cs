using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour {
    // preventing objects being set through code
    [SerializeField] private Image xpBar;
    [SerializeField] private Text characterLevel;

    private WaitForSeconds wait = new WaitForSeconds(.05f);

    private int playerLevel;
    private int requiredXpToLevelUp;
    private int previousLevelRequiredXp;
    private int levelXpDifference;
    private int sliderValue;

    public void Setup(int _playerLevel, int _startingXp) {
        if (_playerLevel <= 0)
            Debug.LogError("Level is to low...");

        playerLevel = _playerLevel;
        requiredXpToLevelUp = Experience.LevelExp(playerLevel);
        previousLevelRequiredXp = Experience.LevelExp(playerLevel - 1);

        levelXpDifference = requiredXpToLevelUp - previousLevelRequiredXp;
        sliderValue = Mathf.Clamp(_startingXp - previousLevelRequiredXp, 0, 1000000);

        var scale = sliderValue / (float)levelXpDifference;
        xpBar.transform.localScale = new Vector3(scale, 1f, 1f);
    }


    public IEnumerator UpdateExpBar(int xpChange) {
        int newXp = sliderValue + xpChange;
        bool leveled = false;

        while (newXp > sliderValue) {
            sliderValue++;
            leveled = false;
            // Check if player level's up
            if (sliderValue == levelXpDifference) {
                leveled = true;
                playerLevel++;
                SetLevelText(playerLevel);
                previousLevelRequiredXp = requiredXpToLevelUp;
                requiredXpToLevelUp = Experience.LevelExp(playerLevel);

                newXp -= sliderValue;

                levelXpDifference = requiredXpToLevelUp - previousLevelRequiredXp;
                sliderValue = 0;
            } else {
                var scale = sliderValue / (float)levelXpDifference;
                xpBar.transform.localScale = new Vector3(scale, 1f, 1f);
            }

            if (leveled) {
                Player p = GameManager.Instance.SelectedPlayer;
                yield return StartCoroutine(p.PromptChatWithNewAbility(playerLevel));
            }

            yield return wait;
        }
    }

    private void SetLevelText(int level) {
        characterLevel.text = "Lv " + level;
    }
}