using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script is used to have easy access with updating indexs by being able to search it's reference within a scene
// It contains information on which index of a selection window was clicked, the information is reterived when it's passed
// Best explain is while the abilities are selected this script will contain a value for that index
// This script could be removed and the same hardcoded value could be inputed into the onclick event
public class IndexData : MonoBehaviour {
    
    [SerializeField] private int indexValue = -1;

    public int IndexValue
    {
        get { return indexValue; }
    }

	// Use this for initialization
	void Start () {
        // -1 index is used for health potion, therefore less than -1
		if (indexValue < -1) {
			print ("index value is not set correctly for: " + gameObject.name);
		}
	}
}
