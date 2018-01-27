using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarWithCoroutine : MonoBehaviour {

    public string levelText = "Lv";

    private int hp = 100;
    private int maxHp = 100;
    
    // preventing objects being set through code
    [SerializeField] private Image healthbar;
    [SerializeField] private Text  ratioText;

    [SerializeField] private Text charactername;
    [SerializeField] private Text characterlevel;

    public int Hp
    {
        get { return hp; }
    }

    public bool Healable
    {
        get { return (hp != maxHp); }
    }

    public void SetUp(Character character)
    {
        Stats stats_component = character.Stats_Component;

        if (charactername != null)
            charactername.text  = character.name;
        if (characterlevel != null)
            characterlevel.text = levelText + " " + stats_component.Level;

        if (stats_component != null)
        {
            hp = stats_component.Hp;
            maxHp = stats_component.MaxHp;

            ratioText.text = hp + "/" + maxHp;
            float scale = (float)hp / maxHp;
            healthbar.transform.localScale = new Vector3(scale, 1f, 1f);
        }
    }
    
    // taking damage
    public IEnumerator BeginTakingDamage(int newHp)
    {
        while (hp > newHp)
        {
            hp -= 1;
            float scale = (float) hp / maxHp;
            ratioText.text = (hp.ToString("F0") + "/" + maxHp);
            healthbar.transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        } 
    }

    // healing
    public IEnumerator BeginHealing(int newHp)
    {
        while (newHp > hp)
        {
            hp += 1;
            float scale = (float)hp / maxHp;
            ratioText.text = (hp.ToString("F0") + "/" + maxHp);
            healthbar.transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }
    }
}