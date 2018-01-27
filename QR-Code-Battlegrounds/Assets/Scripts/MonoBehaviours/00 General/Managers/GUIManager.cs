using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    private ScenesController sceneController;
    
    // Map Gui's
    public GameObject TempCanvas;
    public GameObject SelectionCanvas;
    public GameObject QRScannerCanvas;
    
    // Use this for initialization
    void Start () {
        sceneController = FindObjectOfType<ScenesController>();
    }
    
    // Map 
    public void MainMenuClicked ()
    {
        sceneController.LoadScene(ScenesEnum.mainmenu);
    }

    public void SetTempCanvas(bool x)
    {
        TempCanvas.SetActive(x);
    }

    public void SetSelectionCanvas(bool x)
    {
        SelectionCanvas.SetActive(x); 
    }

    public void SetQRScannerCanvas (bool x)
    {
        QRScannerCanvas.SetActive(x);
    }
    
    public void CharacterSelected(IndexData indexData)
    {
        GameManager.Instance.SetSelectedCharacterByIndex(indexData.IndexValue);

        // Load Battlescene
        sceneController.LoadScene(ScenesEnum.battlescene);
    }
}
