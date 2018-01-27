using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StoreSceneManager : MonoBehaviour {

    public InputField potionInputField;
    //public Text text;
    private Text potionTextComponent;
	// Use this for initialization
	void Start () {
        potionTextComponent = potionInputField.textComponent;
        potionInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        potionInputField.characterValidation = InputField.CharacterValidation.Integer;
	}

    public void ValueChanged ()
    {
        vChanged();
    }

    void vChanged()
    {
        print(potionInputField.textComponent.text);
    }
}