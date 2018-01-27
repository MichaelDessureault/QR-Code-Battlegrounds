using UnityEngine;
using System.Collections;

public class ObjectPosition : MonoBehaviour {
	GoogleStaticMap mainMap;
    
	public  float lat_d = 0.0f  , lon_d = 0.0f;

	private GeoPoint pos;
    
 //   public GeoPoint GeoPoint {
	//	get { return pos; }
	//}

    // This doesn't make sense ... Very bad dhdh
    public void SetLatLon(GeoPoint geoPoint)
    {
        lat_d = geoPoint.lat_d;
        lon_d = geoPoint.lon_d;
        pos = geoPoint;
    }

    public void SetLatLon (float lat, float lon)
    {
        lat_d = lat;
        lon_d = lon;
        MakeGeoPoint();
    }

	public void MakeGeoPoint() { 
		pos = new GeoPoint ();
		pos.SetLatLon_deg (lat_d, lon_d);
	}

	public void SetPositionOnMap () {
		Vector2 tempPosition = GeoGameManager.Instance.GetMainMapMap ().getPositionOnMap (this.pos);
		transform.position = new Vector3 (tempPosition.x, transform.position.y, tempPosition.y);
	}

	public void SetPositionOnMapWithInstantiate () {
		Vector2 tempPosition = GeoGameManager.Instance.GetMainMapMap ().getPositionOnMap (this.pos);
		Instantiate (gameObject, new Vector3 (tempPosition.x, transform.position.y, tempPosition.y), Quaternion.identity);
	}

	public void SetPositionOnMap (GeoPoint pos) {
		this.pos = pos;
		SetPositionOnMap ();
	}

}
