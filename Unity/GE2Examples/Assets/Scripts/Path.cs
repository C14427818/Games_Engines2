using UnityEngine;
using System.Collections;


public class Path : MonoBehaviour {

   
    private Waypoint[] points;

    public float radius;

    
	public Waypoint[] Points{
    
        get { return points; }
    }

	//Before all
	void Awake (){
    
        radius = 10f;
        
        points = new Waypoint[transform.childCount];

		for (int i = 0; i < points.Length; i++){
        
            points[i] = transform.GetChild(i).GetComponent<Waypoint>();
        }        
	}
}
