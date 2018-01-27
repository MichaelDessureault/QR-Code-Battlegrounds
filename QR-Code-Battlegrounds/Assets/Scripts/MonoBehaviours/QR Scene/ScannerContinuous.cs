using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScannerContinuous : MonoBehaviour {

	private IScanner BarcodeScanner;
	
	public RawImage Image;

    public Text message;
    
    private ScenesController scenesController;

    //Get Firebase database
    private FirebaseDatabaseConnector database;
    
	public AudioSource Audio;

    private bool isScannerStarted = false;

    private bool enemyFound = false;

    //private float RestartTime;
    private float RestartTime;
   
	// Disable Screen Rotation on that screen
	void Awake()
    {
        scenesController = FindObjectOfType<ScenesController>();
        //Get Object of Firebase Connector
        database = FindObjectOfType<FirebaseDatabaseConnector>();
        isScannerStarted = true;
        Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = false;
        Image.enabled = true;

        // for all enemies found within the scene make sure they do not show up through the camera view
        // in other words disable all meshrenders and box colliders for the players
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies != null) {
            foreach (Enemy e in enemies) {
                e.GetComponent<MeshRenderer>().enabled = false;
                e.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    void Start () {
		// Create a basic scanner
		BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();

		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
            // Set Orientation & Texture
            if (Image == null)
                Debug.Log("image null");
			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
			Image.transform.localScale = BarcodeScanner.Camera.GetScale();
			Image.texture = BarcodeScanner.Camera.Texture;

			// Keep Image Aspect Ratio
			var rect = Image.GetComponent<RectTransform>();
			var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

            //RestartTime = Time.realtimeSinceStartup;
            RestartTime = Time.realtimeSinceStartup;

		};
	}

	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
            if (barCodeType.Equals("QR_CODE"))
            {
                BarcodeScanner.Stop();
                Debug.Log(barCodeValue);
                // Feedback
                Audio.Play();

                #if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
#endif
                RestartTime += Time.realtimeSinceStartup + 1f;
                // Load Battlescene
                if (scenesController != null)
                {
                    if (database != null)
                    {
                            FirebaseDatabaseConnector.QRCodeCallback callback = LoadBattleScene;
                            database.CheckValidQRCode(barCodeValue, callback);
                    }
                    else
                    {
                        Debug.Log("Database not found");
                    }
                }
                else
                {
                    Debug.LogError("No scenesController found!");
                }
            }
		});
	}


    public void LoadBattleScene(QRCode code, string reply)
    {
        message.text = reply;
        if (code!=null)
        {
            if (GameManager.Instance.SelectedEnemy.code.QRCodeID.Equals(code.QRCodeID))
            {
                StopCamera();
                enemyFound = true;
                scenesController.LoadScene(ScenesEnum.battlescene);
            }
            else
            {
                message.text = "QR Code Not Locked To This Location";
            }
        }        
    }

    /// <summary>
    /// The Update method from unity need to be propagated
    /// </summary>
    void Update()
	{
		if (BarcodeScanner != null)
		{
			BarcodeScanner.Update();
        }
        
        if(RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
        {
            StartScanner();
            RestartTime = 0;
        }
	}

    #region UI Buttons

    // If the map scene gets renaimed this will not work...
    public void ClickedBack()
    {
        // Try to stop the camera before loading another scene
        try
        {
            StartCoroutine(StopCamera(() =>
              {
                  if (scenesController != null) {
                      scenesController.LoadScene(ScenesEnum.map);
                  } else {
                      SceneManager.LoadScene("Map");
                  }
              }));
        } catch(Exception e)
        {
            Debug.LogWarning(e);
        }
    }
    
    public void ClickedSkip() {
        enemyFound = true;
        // Try to stop the camera before loading another scene
        try {
            StartCoroutine(StopCamera(() =>
            {
                if (scenesController != null) {
                    scenesController.LoadScene(ScenesEnum.battlescene);
                } else {
                    SceneManager.LoadScene("BattleScene");
                }
            }));
        } catch (Exception e) {
            Debug.LogWarning(e);
        }
    }

    /// <summary>
    /// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
    /// Trying to stop the camera in OnDestroy provoke random crash on Android
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator StopCamera(Action callback)
	{
        // Stop Scanning
        if (BarcodeScanner != null)
        {
            Image = null;
            BarcodeScanner.Destroy();
            BarcodeScanner = null;
        }

		// Wait a bit
		yield return new WaitForSeconds(0.1f);

		callback.Invoke();
	}
    
    public void StopCamera()
    {
        // Stop Scanning
        if (BarcodeScanner != null)
        {
            BarcodeScanner.Destroy();
            BarcodeScanner = null;
        }
    }
    #endregion


    #region AppDelagate

    private void OnApplicationQuit()
    {
        isScannerStarted = false;
        StopCamera();
    }

    // pause is called when the application goes into background on the phone
    // pause is called on pc when the player clicks outside of the application
    private void OnApplicationPause(bool pause)
    {
        if (enemyFound)
            return;

        Debug.Log("Puase " + pause + " barcodescanner : " + BarcodeScanner + " isscannerstarted: " + isScannerStarted);

        if (pause && isScannerStarted)
        {
            Debug.Log("Stopping camera because application is paused");
            isScannerStarted = false;
            StopCamera();
            return;
        }

        if (!pause && !isScannerStarted)
        {
            isScannerStarted = true;
            Debug.Log("Starting camera because application is unpaused");
            Start();
            return;
        } 
    }

    #endregion
}
