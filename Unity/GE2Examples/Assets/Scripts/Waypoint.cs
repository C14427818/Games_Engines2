using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{ 

	public Vector3 Position{
    
        get { return transform.position;}
    }
		
	void OnDrawGizmos(){
    
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,1);
    }
}
