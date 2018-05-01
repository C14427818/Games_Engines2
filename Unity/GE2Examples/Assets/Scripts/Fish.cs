using UnityEngine;
using System.Collections;

public class Fish : Vehicle {

    //Check leader
    public bool isLeader = false;
    private Vector3 steerForce;//Steering force to calculate 
    private Vector3 seekPoint;//Desired point to seek for leader
    private bool evadeMode;//When shark is within certain distance fish goes into evade mode

    //Weights for forces
    private float seekWeight = 50f;
    private float evadeWeight = 100f;  
    private float inBoundsWeight = 200f;
    private float separateWeight = 300f;
    private float cohesionWeight = 25f;
    private float alignWeight = 25f;
    private float separateDistance = 5f;//Distance for separation from other fish

    //Set up/initialise fish stuff at start
	override public void Start (){
    
        //Call vehicle start function first
		base.Start();

        //For evasion
        evadeMode = false;
	}

    //Calculate steering forces and apply them to seeker
	protected override void CalcSteeringForces(){
       
        steerForce = Vector3.zero;
        evadeMode = false;//Reset evade mode set true if shark is within safe radius
        steerForce += Evade(gm.Shark.transform, 5,25) * evadeWeight;//Add weighted force to evade shark
        steerForce += StayInBounds() * inBoundsWeight;//Add weighted force to stay within bounds of world

        //If not evading, behave as normal
		if (!evadeMode){
        
            //Leader gets point on path and seeks it
			if (isLeader){
                           
                seekPoint = gm.NextPathPoint;//Get point on path to seek
            
                steerForce += Seek(seekPoint) * seekWeight;//Add weighted force to seek next point on path
            }
            //Followers get point behind leader seek it and separate
			else{
            
                seekPoint = gm.FollowPoint;//Get follow point behind leader to seek
                steerForce += Seek(gm.Centroid) * cohesionWeight;//Add weighted cohesion force to bring fish in toward center
                steerForce += Align(gm.FishDirection) * alignWeight;//Add weighted alignment force to make fish face same direction
                steerForce += Separation(separateDistance) * separateWeight;//Add weighted separation force to avoid colliding with other fish
                steerForce += Arrive(seekPoint, gm.FollowDistance) * seekWeight;//Add weighted arrive force for following leader
            }
        }

        //Limit force to make force
        steerForce = Vector3.ClampMagnitude(steerForce, maxForce);

        ApplyForce(steerForce);
    }

    //Get force to make flockers align to given vector
	private Vector3 Align(Vector3 alignVector){
    
        desired = (alignVector.normalized * maxSpeed) - velocity;//Get desired force by subtracting velocity from alignment velocity

        return desired;//Return force to align
    }

    //Get force to separate flockers no collisions
	private Vector3 Separation(float separationDistance){
    
        desired = Vector3.zero;//Reset desired vector

        Vector3 vecFromCenter;//Vector pointing away from other flocker 
        float distanceSq;//Square distance to fish being checked against 
        vecFromCenter = transform.position - gm.Leader.transform.position;//Get vector to leader
        distanceSq = vecFromCenter.sqrMagnitude;//Get square distance to leader

        //If too close to leader steer to the side of it to stay out of its way
		if(distanceSq < Mathf.Pow(gm.FollowDistance,2)){
        
            //If we're facing the same direction as leader move left or right
			if (Vector3.Dot (transform.right, vecFromCenter) > 0 ^ Vector3.Dot (transform.forward, gm.Leader.transform.forward) < 0) {
            
				desired += Seek (gm.Leader.transform.position - (gm.Leader.transform.right * gm.FollowDistance));
			}else{
            
                desired += Seek(gm.Leader.transform.position + (gm.Leader.transform.right * gm.FollowDistance));
            }           
        }

        //Run through all fish
		for (int i = 1; i < gm.FishList.Count; i++){
                
            vecFromCenter = transform.position - gm.FishList[i].transform.position;//Get vector from center of other fish

            distanceSq = vecFromCenter.sqrMagnitude;//Get square distance from fish

            //Fish doesn't try and separate magnitude != 0 
			if (distanceSq > 0 && distanceSq < separationDistance){
            
                desired += vecFromCenter.normalized/Mathf.Sqrt(distanceSq);//Add vector normalise by dividing/distance
            }
        }

        //If desired isn't a zero vector, convert it to a force to apply
		if (desired.sqrMagnitude > 0){
        
            desired = desired.normalized * maxSpeed;//Normalise and set magnitude to max speed
            desired -= velocity;//Subtract velocity from desired to get steering force
        }

        return desired;//Return separation force
    }

    //Check if shark is too close and get force to evade it if necessary
	public Vector3 Evade(Transform evadeTarget, float predictDist, float safeRadius){
    
        desired = Vector3.zero;
        Vector3 futureVector = evadeTarget.position + evadeTarget.forward * predictDist;//Get vector for predicted position of evade target
        float distanceSq = (transform.position - evadeTarget.position).sqrMagnitude;//Get square distance to evade target

        //If evade target is too close/within safe radius, take action
		if (distanceSq < Mathf.Pow(safeRadius, 2)){
        
            evadeMode = true;//Set evade mode to true
            desired = transform.position - futureVector;//Get desired vector away from predicted positions
            desired = desired.normalized * maxSpeed;//Normalise set magnitude to max speed for desired velocity
            desired -= velocity;//Subtract current velocity from desired velocity to get steering force to apply          
        }

        return desired;//Return evade force to apply
    }


}
