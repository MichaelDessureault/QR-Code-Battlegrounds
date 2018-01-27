using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Do note that SwitchCamera script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class SwitchCamera : MonoBehaviour {
     
	public new GameObject camera;

    // Consts
    private const float KTRANSITION_KH = .03f;
    private const float KBIRDS_EYE_VIEW_KH = .1f;
    private const float KPLAYER_VIEW_KH = .03f;

    // Character controllers 
    public GameObject PlayersCharacterController;
    public GameObject BirdsEyeCharacterController;
    
    // Track points
    public Transform birdsEyeViewTracker;
    public Transform playersTracker;

    // Look at objects
    public Transform playerLookAtObject;
    public Transform birdsEyeViewLookAtObject;
    public Transform mainLookAtObject;

    // other public
    public GameObject playerCameraIcon;
    public GameObject birdsEyeCameraIcon;
    
    // local positions the parent is the character...
    private Vector3 birdsEyeViewTrackPointDefaultPosition; 
    private Vector3 birdsEyeCharacterControllerDefaultPosition;

    // other private
    private bool isBirdsEyeView = false;
    private bool transitioning = false;
    private Transition cameraTransition;
    //private Transition lookatObjectTransition;

    private CharacterControllersController ccc_instance;

    // Use this for initialization
    void Start () {
		if (PlayersCharacterController == null || camera == null) {
			print ("character or camera is null");
		} else {
            ccc_instance = CharacterControllersController.Instance;

            // Set the camera property within GeoGameManager
            GeoGameManager.BirdsEyeViewCamera = camera;
            cameraTransition = camera.GetComponent<Transition>();
            //lookatObjectTransition = mainLookAtObject.GetComponent<Transition>();

            birdsEyeViewTrackPointDefaultPosition = birdsEyeViewTracker.transform.localPosition;
            birdsEyeCharacterControllerDefaultPosition = BirdsEyeCharacterController.transform.localPosition;

            // camera by default is tracking playersTracker and looking at the playerLookAtObject
            cameraTransition.SetTrackObjWithLookObj(trackobj: playersTracker, lookatobj: playerLookAtObject, transitioning: false);
        } 
	}


	// Update is called once per frame
	void Update () {
        if (transitioning)
        {
            if (!camera.GetComponent<Transition>().IsTransitioning)
            {
                // change the look at object if it's in birds eye view
                if (isBirdsEyeView)
                {
                    cameraTransition.SetKH(KBIRDS_EYE_VIEW_KH);
                    cameraTransition.SetTrackObjWithLookObj (trackobj: birdsEyeViewTracker, lookatobj: birdsEyeViewLookAtObject, transitioning: false);
                    cameraTransition.lookatenabled = false;
                } else{
                    cameraTransition.SetKH(KPLAYER_VIEW_KH);
                    cameraTransition.SetTrackObjWithLookObj (trackobj: playersTracker, lookatobj: playerLookAtObject, transitioning: false);
                }
                MovementController.isenabled = true;
                transitioning = false;
            }
        }
    }
    
    public void Toggle ()
    {
        if (!transitioning)
        { 
            transitioning = true;
            cameraTransition.lookatenabled = true;
            isBirdsEyeView = !isBirdsEyeView;
            cameraTransition.SetKH(KTRANSITION_KH);
            MovementController.isenabled = false;

            if (isBirdsEyeView)
            {
                cameraTransition.SetTrackObjWithLookObj(trackobj: birdsEyeViewTracker, lookatobj: mainLookAtObject, transitioning: true);
                ccc_instance.SetCharacterControllerControllerState(CharacterControllersController.CharacterControllersControllerState.birds);
            } else
            {
                //lookatObjectTransition.SetTrackObjWithLookObj(trackobj: playerLookAtObject, lookatobj: null, transitioning: false);
                cameraTransition.SetTrackObjWithLookObj(trackobj: playersTracker, lookatobj: mainLookAtObject, transitioning: true);
                ResetLocalPositions();
                ccc_instance.SetCharacterControllerControllerState(CharacterControllersController.CharacterControllersControllerState.player);
            }


            GeoGameManager.BirdsEyeView = isBirdsEyeView;
            ChangeIcons();
        }
    }

    private void ResetLocalPositions()
    {
        birdsEyeViewTracker.transform.localPosition = birdsEyeViewTrackPointDefaultPosition;
        BirdsEyeCharacterController.transform.localPosition = birdsEyeCharacterControllerDefaultPosition;
    }

    private void ChangeIcons ()
    {
        playerCameraIcon.SetActive(!playerCameraIcon.activeSelf);
        birdsEyeCameraIcon.SetActive(!birdsEyeCameraIcon.activeSelf);
    }
}
