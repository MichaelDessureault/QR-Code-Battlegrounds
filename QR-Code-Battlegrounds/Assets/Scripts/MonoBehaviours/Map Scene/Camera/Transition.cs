using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour {

    [SerializeField] private bool transitioning = false;
    [SerializeField] private Transform trackObj;
    [SerializeField] private Transform lookAtTarget;

    [Range(0f, 0.1f)]
    public float kh; 

    public float slerpspeed = .1f;
    public bool lookatenabled = true;
    
    // this will always be returning false
    public bool IsTransitioning
    {
        get { return transitioning; }
    }

    public void Setup ()
    {
        transform.position = trackObj.position;
        transform.LookAt(lookAtTarget);
    }

    public void SetKH(float x)
    {
        if (x >= 0f && x <= 0.1f)
        {
            kh = x;
        }
    }
    
    public void SetTrackObjWithLookObj(Transform trackobj, Transform lookatobj, bool transitioning)
    {
        trackObj = trackobj;
        lookAtTarget = lookatobj;
        this.transitioning = transitioning;
    } 
    
    void LateUpdate()
    {
        if (trackObj != null)
        {
            if (Vector3.Distance(transform.position, trackObj.position) > 0.1)
            { 
                transform.position += kh * (trackObj.position - transform.position);
                if (lookatenabled) transform.LookAt(lookAtTarget.transform);
                else transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget.rotation, Time.deltaTime * slerpspeed);
            }
            else
            { 
                transitioning = false;
            } 
        } 
    }  
}
