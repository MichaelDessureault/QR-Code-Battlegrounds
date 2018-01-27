using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// do not want to make this method a singleton because it will be carried throughout multiple scenes when it's not required.
public class CharacterControllersController : GameObjectSingleton<CharacterControllersController>
{
    public enum CharacterControllersControllerState
    { 
        birds   = 0,
        player  = 1
    };
     
    // by default the values are set to the player's state
    private CharacterControllersControllerState cccs     = CharacterControllersControllerState.player;
    private CharacterControllersControllerState pre_cccs = CharacterControllersControllerState.player;
 
    public CharacterController characterController;
    public CharacterController birdsEyeController; 

    public override void Awake()
    {
        if (characterController == null || birdsEyeController == null)
        {
            print("a controller is null in CharacterControllersController");
        } 
    }

    public void SetCharacterControllerControllerState (CharacterControllersControllerState state)
    {
        cccs = state;
    }
    
    // Update is called once per frame
    void Update () {
        ControllerUpdateCheck();
	}

    void ControllerUpdateCheck()
    {
        if (cccs != pre_cccs)
        { 
            birdsEyeController.enabled  = !birdsEyeController.enabled;
            characterController.enabled = !characterController.enabled; 
        }
        pre_cccs = cccs;
    }

    public CharacterController GetActiveController()
    {
        return (characterController.enabled) ? characterController : birdsEyeController; 
    }
}