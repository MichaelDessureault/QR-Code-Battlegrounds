using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeoGameManager : GameObjectSingleton<GeoGameManager> {

	[HideInInspector]
	public bool locationServicesIsRunning = false;

	public GameObject mainMap;
	public GameObject newMap;

	public GameObject player;
	public GeoPoint playerGeoPosition;
	public PlayerLocationService player_loc;

	public enum PlayerStatus { TiedToDevice, FreeFromDevice }

	private PlayerStatus _playerStatus;
	public PlayerStatus playerStatus
	{
		get { return _playerStatus; }
		set { _playerStatus = value; }
	}

    private static bool birdsEyeView = false;
    private static GameObject birdsEyeViewCamera;

    public static bool BirdsEyeView
    {
        set { birdsEyeView = value; }
    }

    public static GameObject BirdsEyeViewCamera
    {
        set { birdsEyeViewCamera = value;  }
    }

	new void Awake (){
		Time.timeScale = 1;
		playerStatus = PlayerStatus.TiedToDevice;

        if (player != null) {
            player_loc = player.GetComponent<PlayerLocationService>();
        }

        if (newMap != null) {
            newMap.GetComponent<MeshRenderer>().enabled = false;
            newMap.SetActive(false);
        }
	}

	public GoogleStaticMap GetMainMapMap () {
		return mainMap.GetComponent<GoogleStaticMap> ();
	}

	public GoogleStaticMap GetNewMapMap () {
		return newMap.GetComponent<GoogleStaticMap> ();
	}

	IEnumerator Start () {
        
        if (mainMap == null) {
            yield break;
        }

        GetMainMapMap ().initialize ();
		yield return StartCoroutine (player_loc._StartLocationService ());
		StartCoroutine (player_loc.RunLocationService ());
        
		locationServicesIsRunning = player_loc.locServiceIsRunning;

		Debug.Log ("Player loc from GeoGameManager: " + player_loc.loc);
		GetMainMapMap ().centerMercator = GetMainMapMap ().tileCenterMercator (player_loc.loc);
		GetMainMapMap ().DrawMap ();

		mainMap.transform.localScale = Vector3.Scale (
			new Vector3 (GetMainMapMap ().mapRectangle.getWidthMeters (), GetMainMapMap ().mapRectangle.getHeightMeters (), 1.0f),
			new Vector3 (GetMainMapMap ().realWorldtoUnityWorldScale.x, GetMainMapMap ().realWorldtoUnityWorldScale.y, 1.0f));

		player.GetComponent<ObjectPosition> ().SetPositionOnMap (player_loc.loc);
        
        // Finds the object (Camera) with the transition script and updates it's location 
        FindObjectOfType<Transition>().Setup();
    }

    void Update () {
		if(!locationServicesIsRunning){

			//TODO: Show location service is not enabled error. 
			return;
		}
        
		playerGeoPosition = new GeoPoint();

        if (!birdsEyeView)
        {
            if (playerStatus == PlayerStatus.TiedToDevice)
            { 
                playerGeoPosition = player_loc.loc;
                player.GetComponent<ObjectPosition>().SetPositionOnMap(playerGeoPosition);
            }
            else if (playerStatus == PlayerStatus.FreeFromDevice)
            {
                playerGeoPosition = GetMainMapMap().getPositionOnMap(new Vector2(player.transform.position.x, player.transform.position.z));
            }
        } else
        {
            playerGeoPosition = GetMainMapMap().getPositionOnMap(new Vector2(birdsEyeViewCamera.transform.position.x, birdsEyeViewCamera.transform.position.z));
        }


		var tileCenterMercator = GetMainMapMap ().tileCenterMercator (playerGeoPosition);
		if(!GetMainMapMap ().centerMercator.isEqual(tileCenterMercator)) {

			newMap.SetActive(true);
			GetNewMapMap ().initialize ();
			GetNewMapMap ().centerMercator = tileCenterMercator;

			GetNewMapMap ().DrawMap ();

			GetNewMapMap ().transform.localScale = Vector3.Scale(
				new Vector3 (GetNewMapMap ().mapRectangle.getWidthMeters (), GetNewMapMap ().mapRectangle.getHeightMeters (), 1.0f),
				new Vector3(GetNewMapMap ().realWorldtoUnityWorldScale.x, GetNewMapMap ().realWorldtoUnityWorldScale.y, 1.0f));	

			Vector2 tempPosition = GeoGameManager.Instance.GetMainMapMap ().getPositionOnMap (GetNewMapMap ().centerLatLon);
			newMap.transform.position = new Vector3 (tempPosition.x, 0, tempPosition.y);

			GameObject temp = newMap;
			newMap = mainMap;
			mainMap = temp;

		}
		if(GetMainMapMap().isDrawn && mainMap.GetComponent<MeshRenderer>().enabled == false){
			mainMap.GetComponent<MeshRenderer>().enabled = true;
			newMap.GetComponent<MeshRenderer>().enabled = false;
			newMap.SetActive(false);
		}
	}

	public Vector3? ScreenPointToMapPosition(Vector2 point){
		var ray = Camera.main.ScreenPointToRay(point);
		//RaycastHit hit;
		// create a plane at 0,0,0 whose normal points to +Y:
		Plane hPlane = new Plane(Vector3.up, Vector3.zero);
		float distance = 0; 
		if (!hPlane.Raycast (ray, out distance)) {
			// get the hit point:
			return null;
		}
		Vector3 location = ray.GetPoint (distance);
		return location;
	}

}
