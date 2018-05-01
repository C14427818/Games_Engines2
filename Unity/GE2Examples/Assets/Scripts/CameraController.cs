using UnityEngine;
using System.Collections;

//Camera mode
enum CameraMode{

    Fish,//Follow fish
    Shark,//Follow shark
    Free//Free cam
}

//Control camera with different states
public class CameraController : MonoBehaviour {

	GameManager gm;//Gamemanager for scene
	CameraMode cameraMode;//Current camera mode
    public Transform target;//Target for camera to follow
    public float distance = 3.0f;//Follow distance
    public float height = 1.50f;//Height offset from target
    public float heightDamping = 1.0f;//For 
    public float positionDamping = 1.0f;//Smoother
    public float rotationDamping = 1.0f;//Movement
    public float lookSensitivity = 60f;//Sensitivity for mouse look
    public float moveSpeed = 1f;//Speed of free camera
    private float rotationX, rotationY;//Rotation changes based on mouse movement
    Quaternion initialRotation;//Initial rotation of camera when switched to free cam

    // Use this for initialization
	void Start (){
    
        //Get game manager
        gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>();

        //Default to fish cam
        SwitchToFishCam();
    }
	
	// Update is called once per frame
	void Update (){
    
        //FISH FOLLOW MODE
		if (cameraMode == CameraMode.Fish){
        
            //If there are no fish left switch to shark cam
			if (gm.FishList.Count == 0){
            
                SwitchToSharkCam(); 
            }      

            //If player presses 2 go to shark 
			if (Input.GetKeyDown(KeyCode.Alpha2)){
            
                SwitchToSharkCam();                
            }
            //If player presses 3 go to free cam
			else if(Input.GetKeyDown(KeyCode.Alpha3)){
            
                SwitchToFreeCam();
            }
        }
        //SHARK FOLLOW MODE
		else if (cameraMode == CameraMode.Shark){
        
            //If player presses 1 and there are still living fish, switch to fish cam
			if (Input.GetKeyDown(KeyCode.Alpha1) && gm.FishList.Count > 0){
            
                SwitchToFishCam();
            }
            //If player presses 3 go to free cam
			else if (Input.GetKeyDown(KeyCode.Alpha3)){
            
                SwitchToFreeCam();
            }
        }

        //FREE CAMERA MODE
		else if (cameraMode == CameraMode.Free){
        
            //Get total x and y changes from beginning of camera switch, these will be added to initial rotation
            rotationX += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
			rotationX %= 360; //Use modulo to keep rotation within -360 and 360
            rotationY %= 360;

            //X rotation is rotation around up axis, y rotation is rotation around left axis
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
	
			transform.localRotation = initialRotation * xQuaternion * yQuaternion;//Change local rotation by multiplying initial rotation by quaternions for x and y rotation

            //WASD translate camera forward/backward and left/right 
			if (Input.GetKey(KeyCode.W)){
            
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
			else if(Input.GetKey(KeyCode.S)){
            
                transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
            }
			if(Input.GetKey(KeyCode.A)){
            
                transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime);
            }
			else if(Input.GetKey(KeyCode.D)){
            
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
            //Shift moves camera up Ctrl moves camera down
			if(Input.GetKey(KeyCode.LeftShift)){
            
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftControl)){
				
                transform.Translate(-Vector3.up * moveSpeed * Time.deltaTime);
            }

            //If 1 is pressed switch to fish cam
			if (Input.GetKeyDown(KeyCode.Alpha1) && gm.FishList.Count > 0){
            
                SwitchToFishCam();
            }
            //If 2 is pressed switch to shark cam
            else if (Input.GetKeyDown(KeyCode.Alpha2)){
				
                SwitchToSharkCam();
            }
        }
    }

 
    void LateUpdate(){
		
        //If camera mode isn't free
		if (cameraMode != CameraMode.Free){

			if (!target) {
				return;
			}

            float wantedHeight = target.position.y + height;
            float currentHeight = transform.position.y;

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Set the position of the camera 
            Vector3 wantedPosition = target.position - target.forward * distance;
			transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);
			transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);// adjust the height of the camera
            transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * rotationDamping);
        }

    }

    //Switches mode to fish cam and sets target to direction averages of fish
    void SwitchToFishCam(){
		
        target = gm.transform;
        cameraMode = CameraMode.Fish;
    }

    //Switches mode to shark cam and sets target to shark
    void SwitchToSharkCam(){
		
        target = gm.Shark.transform;
        cameraMode = CameraMode.Shark;
    }

    //Switches mode to free cam and stores current camera rotation as initial rotation
    void SwitchToFreeCam(){
		
        cameraMode = CameraMode.Free;
        initialRotation = transform.localRotation;
    }

}
