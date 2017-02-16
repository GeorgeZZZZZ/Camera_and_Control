using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
//using System.Linq;
using System.Text;

//	0.3a00.3b00
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
public class Player_Camera_Controller_RTS_RPG : MonoBehaviour {

	public GameObject Cam_Center_Point;
	public GameObject Select_Circle_Prefab;

	public int Edge_Boundary = 1;	//	valuable use for detect limit movement which mouse move near screen edge, unit in pixel 
	public float Player_Normal_Speed=1f;
	public float Player_Run_Speed=2.5f;
	public float Player_Turnning_Speed=72f; //72 degree per second
	public float Cam_Move_Speed=1f;

	public bool Move_Player_towards_Character_Facing = false;
	public bool Move_Player_Along_World_Axis = false;
	public bool Turn_Player_by_Keyboard = false;
	public bool Turn_Player_by_Mouse_Point = false;

	public bool Move_Camera_towards_cam_Facing = false;
	public bool Move_Camera_Along_World_Axis = false;

	public bool Force_RTS_Cam_View = false;	//	Force enter RTS view mode, not perfect yet

	private Camera playerCam;
	private Vector3 moveCalculation;
	private Rigidbody playerRigidbody;
	private Animator anim;

	private float speed;
	private float turnSpeed;
	private float retreatDivisor = 2f; // when character go backward or sideward, he's run and walk speed will divide by this number

	//---valuables for TbMP()
	private int floorMask;
	private float camRayLength = 100f;
	//---

	private bool mouseAreaSelec = false;	//	mouse area selecting flag
	private Vector3 curMousPos;

	private bool camFollowFlag;

	// Use this for initialization
	void Start () {

		floorMask = LayerMask.GetMask ("floor");		//Give a mask to floor layer for camRay in TURNING() to hit for, used by Tbmp()
		playerRigidbody = GetComponent <Rigidbody> ();
		playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;		//freeze rigidbody's rotation to prevent fall down to floor
		playerRigidbody.drag = Mathf.Infinity;
		playerRigidbody.angularDrag = Mathf.Infinity;	//prevant character keep turn not stop after finish rotation
		anim = GetComponent <Animator> ();
		playerCam = Cam_Center_Point.GetComponent <CameraFunctions> ().Cam_Obj.GetComponent <Camera> ();
	}

	// Update is called once per physics update
	void FixedUpdate () {

		DEBUG ();	//	Call debug functions

		//	get physics input
		float moveFB = Input.GetAxis ("Vertical");
		float moveLR = Input.GetAxis ("Horizontal");
		float turnLR = Input.GetAxisRaw ("Rotate");
		bool mousLefButtDow = Input.GetMouseButtonDown (0);
		bool mousLefButtUp = Input.GetMouseButtonUp (0);

		//when push shift button, not only increase walking speed also increase turning speed
		if (Input.GetButton ("Run")){
			speed = Player_Run_Speed;
			turnSpeed = Player_Turnning_Speed * 2.5f;
		}else{
			speed = Player_Normal_Speed;
			turnSpeed = Player_Turnning_Speed;
		}

		if (Force_RTS_Cam_View) {
			RELEASE ();
		}
		camFollowFlag = Cam_Center_Point.GetComponent <CameraFunctions> ().followPlayerFlag;	//check if camera disconnect form player

		if (camFollowFlag) {	//	if camera sitll follow player, keyboard control character
			//	only call move block when player received movement command
			if (moveFB != 0f || moveLR != 0f) {
				if (Move_Player_Along_World_Axis) {
					//	Move Player along World axis	
					MPaW (moveFB, moveLR, speed, retreatDivisor);
				} else if (Move_Player_towards_Character_Facing) {
					//	Move Player towards Character Facing
					MPtCF (moveFB, moveLR, speed, retreatDivisor);
				}
			}

			if (Turn_Player_by_Mouse_Point) {
				//	turning by mouse point
				TPbMP ();
			} else if (Turn_Player_by_Keyboard) {
				//	turning by keyboard
				if (turnLR != 0f) {
					TPbKC (turnLR, turnSpeed);
				}
			}

			Animating (moveFB, moveLR);	//	Animation management

		} else {	//	if not follow then move camera center point directilly, keyboard now control camera
			
			float movXOff, movZOff;
			Edge_Move_Control (out movXOff, out movZOff);	//	give offset velue by detect if mouse move near screen edge

			moveLR = turnLR;	//	make A/D key to control Camera left and right movement

			moveFB += movXOff;
			moveLR += movZOff;

			//	only call MDCtCF() when camera received movement command
			if (moveFB != 0f || moveLR != 0f) {
				if (Move_Camera_Along_World_Axis) {
					// Move Disconnect Camera Along World Axis
					MDCaW (moveFB, moveLR, Cam_Move_Speed);
				} else if (Move_Camera_towards_cam_Facing) {
					// Move Disconnect Camera towards Camera Facing
					MDCtCF (moveFB, moveLR, Cam_Move_Speed);
				}
			}

			RTS_Point_Selec (mousLefButtDow);

			RTS_Area_Selec (mousLefButtDow, mousLefButtUp);
		}
	}

	void OnGUI() {

		//	RTS selection function, mouse click then start draw selection area
		if (mouseAreaSelec) {
			// Create a rect from both mouse positions
			var rect = Utils_RTS_Draw.GetScreenRect( curMousPos, Input.mousePosition );
			Utils_RTS_Draw.DrawScreenRect( rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f ) );
			Utils_RTS_Draw.DrawScreenRectBorder( rect, 2, new Color( 0.8f, 0.8f, 0.95f ) );
		}
	}

	/********************************
	 * --- Functions
	 ********************************/

	//	RTS style Point Selection
	private void RTS_Point_Selec (bool mousLBdown) {
		
		//	search obj which contain Selectable_Unit_Controller.cs
		if (mousLBdown) {
			
			//	cast a ray from camera and go through mouse position
			Ray camMousRay = playerCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit selectionHit;

			//	cast a ray from camera and go through mouse position
			foreach (var selObj in FindObjectsOfType <Selectable_Unit_Controller>()) {

				/******************
				 * may need add code for selected obj in to a global list for AI control at here
				 ******************/

				if (Physics.Raycast (camMousRay, out selectionHit, 50f)) {
					if (selObj.GetComponent <Collider> ().bounds.Contains (selectionHit.point)) {
						if (selObj.selectionCircle == null) {
							selObj.selectionCircle = Instantiate (Select_Circle_Prefab);
							selObj.selectionCircle.transform.SetParent (selObj.transform, false);
							selObj.selectionCircle.transform.eulerAngles = new Vector3 (90, 0, 0);
						}
					} else {
						if (selObj.selectionCircle != null) {
							Destroy (selObj.selectionCircle.gameObject);
							selObj.selectionCircle = null;
						}
					}
				}
			}
		}
	}

	//	RTS style Area Selection
	private void RTS_Area_Selec (bool mousLBdown, bool mousLBup) {
		
		//	if press left mouse button start draw square
		if (mousLBdown) {
			mouseAreaSelec = true;
			curMousPos = Input.mousePosition;
		}

		//	if release left mouse button stop draw square
		if (mousLBup) {
			mouseAreaSelec = false;
		}

		//	use projector give a circle under selected unity
		if (mouseAreaSelec) {
			//	search obj which contain component Selectable_Unit_Controller.cs
			foreach (var selObj in FindObjectsOfType <Selectable_Unit_Controller>()) {
				//	call judgement function and see if obj is in selection area
				if (IsWithinSelectionBounds (selObj.gameObject)) {
					if (selObj.selectionCircle == null) {
						selObj.selectionCircle = Instantiate (Select_Circle_Prefab);
						selObj.selectionCircle.transform.SetParent( selObj.transform, false );
						selObj.selectionCircle.transform.eulerAngles = new Vector3( 90, 0, 0 );
					}
				} 
			}
		}
	}


	//	RTS selection function, judgement for selectable obj in or not in selction area from camera view angle 
	public bool IsWithinSelectionBounds (GameObject gameObject) {
		
		if (!mouseAreaSelec)
			return false;
		
		var cam = playerCam;
		var viewportBounds =
			Utils_RTS_Draw.GetViewportBounds (cam, curMousPos, Input.mousePosition);
			
		return viewportBounds.Contains (
			cam.WorldToViewportPoint (gameObject.transform.position));	//	use bounds() search if obj is in selection area
	}

	//	get current mouse position on screen
	private void MousPos (out float mousXPos, out float mousYPos) {
		//	current mouse position on screen in pixels
		//	0 point is at left, down of game window
		mousXPos = Input.mousePosition.x;	
		mousYPos = Input.mousePosition.y;
	}

	//	Edge Boundary Movement Control
	//	out xMovOff: x Movement Offset, out yMovOff: y Movement Offset
	private void Edge_Move_Control (out float xMovOff, out float zMovOff) {
		float mousXPos, mousYPos;
		float newXOff = 0f, newZOff = 0f;
		float curScreWid = Screen.width;	//	read current game window width in pixels
		float curScreHei = Screen.height;	//	read current game window height in pixels

		MousPos (out mousXPos, out mousYPos);

		//	mouse movement on Y axis, change obj Z direction in world axis
		if (mousXPos > curScreWid - Edge_Boundary) {
			newZOff = 1f;
		} else if (mousXPos < 0 + Edge_Boundary) {
			newZOff = -1f;
		}

		//	mouse movement on Y axis, change obj X direction in world axis
		if (mousYPos > curScreHei - Edge_Boundary) {
			newXOff = 1f;
		} else if (mousYPos < 0 + Edge_Boundary) {
			newXOff = -1f;
		}

		xMovOff = newXOff;
		zMovOff = newZOff;
	}

	//	Move Player towards Character Facing
	//	FB: forward/backward, LR: Left/Right, SP: Speed, RD: retreatDivisor
	void MPtCF(float FB, float LR, float SP, float RD){
		
		//move basis on the character face on (character local axis x, y, z)
		moveCalculation = (transform.forward * FB) + (transform.right * LR);
		if (FB > 0f && LR == 0f) {
			playerRigidbody.MovePosition (transform.position + moveCalculation.normalized * SP * Time.deltaTime);
		} else {
			playerRigidbody.MovePosition (transform.position + moveCalculation.normalized * SP/RD * Time.deltaTime);
		}
	}

	//	Move Player along world axis x, y, z
	//	FB: forward/backward, LR: Left/Right, SP: Speed, RD: retreatDivisor
	void MPaW(float FB, float LR, float SP, float RD){

		moveCalculation.Set (LR, 0f, FB);		//package keyboard value into vector3 type for later calcuation

		//judgment if character is walk or run backward or sideward, speed will dive by divier
		if (FB > 0f && LR == 0f) {
			moveCalculation = moveCalculation.normalized * SP * Time.deltaTime;
		} else {
			moveCalculation = moveCalculation.normalized * SP/RD * Time.deltaTime;
		}
		playerRigidbody.MovePosition (transform.position + moveCalculation);
	}

	//	Move Disconnect Camera center towards Camera Facing
	//	FB: forward/backward, LR: Left/Right, SP: Speed
	void MDCtCF(float FB, float LR, float SP){

		Cam_Center_Point.transform.Translate(Vector3.forward * FB * SP * Time.deltaTime);
		// multiply -1 is because input need be turn over
		Cam_Center_Point.transform.Translate(Vector3.left * LR * -1 * SP * Time.deltaTime);
	}

	//	Move Disconnect Camera center along world axis x, y, z
	//	FB: forward/backward, LR: Left/Right, SP: Speed
	void MDCaW(float FB, float LR, float SP){

		Cam_Center_Point.transform.Translate(Vector3.forward * FB * SP * Time.deltaTime, Space.World);
		// multiply -1 is because input need be turn over
		Cam_Center_Point.transform.Translate(Vector3.left * LR * -1 * SP * Time.deltaTime, Space.World);
	}

	//	Turning Player by Keyboard Control
	//	LR: Left/Right, TS: Turn Speed
	void TPbKC(float LR, float TS){
		Vector3 playerToKeyboard= new Vector3(0f, LR*TS*Time.deltaTime, 0);	//Same if write: float playerToKeyboard = lr*TS*Time.deltaTime; 
		Quaternion tbkcRotation = Quaternion.Euler (playerToKeyboard);	//if use float then the code will be: Quaternion.Euler (0f, playerToKeyboard, 0f);

		playerRigidbody.MoveRotation (playerRigidbody.rotation*tbkcRotation);
	}

	//	Turning Player by Mouse Pointing
	void TPbMP(){
		Ray camRay = playerCam.ScreenPointToRay (Input.mousePosition);

		RaycastHit floorHit;

		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
			
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0f;
			Quaternion tbmpRotation = Quaternion.LookRotation (playerToMouse);

			playerRigidbody.MoveRotation (tbmpRotation);
		}
	}

	//	Animation management
	private void Animating (float FB, float LR){
		bool walking = FB != 0f || LR != 0f;

		anim.SetBool ("IsWalking", walking);
	}

	//	force camera center stop follow player
	private void RELEASE () {
		Cam_Center_Point.GetComponent <CameraFunctions> ().followPlayerFlag = false;
	}

	private void DEBUG () {
		
		//debug try disconnect camera follow
		if (Input.GetKey (KeyCode.T)) {
			RELEASE ();
		}
	}
}

//		Debug.Log ("GetButtonDown:  " + Input.GetButtonDown ("Run"));