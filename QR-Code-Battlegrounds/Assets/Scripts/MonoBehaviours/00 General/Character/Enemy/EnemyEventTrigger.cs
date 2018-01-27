using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyEventTrigger : MonoBehaviour
{
    public int clickDistance = 5;
    private CharacterController characterController;

    // Use this for initialization
    void Start()
    {
        // different way of this but this is okay i suppose
        characterController = CharacterControllersController.Instance.characterController;
        
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Clicked(); });
        trigger.triggers.Add(entry);
    }


    public void Clicked()
    {
        if (Vector3.Distance(gameObject.transform.position, characterController.gameObject.transform.position) <= clickDistance)
        {
            // keep the gameobject 
            GameObject enemySelected = GameObject.Find("Enemy Selected");

            if (enemySelected != null)
                Destroy(enemySelected);
            enemySelected = new GameObject("Enemy Selected");
            
            gameObject.transform.parent = enemySelected.transform;
            DontDestroyOnLoad(enemySelected);

            // Update the game manager with the selected enemy 
            Enemy e = gameObject.GetComponent<Enemy>();
            GameManager.Instance.SelectedEnemy = e;
            
            // Load the qr scene
            ScenesController scenesController = FindObjectOfType<ScenesController>() as ScenesController;
            scenesController.LoadScene(ScenesEnum.scanner);
        }
    }
}