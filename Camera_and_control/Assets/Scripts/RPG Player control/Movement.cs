/*	This script is base on 
 *	Hack & Slash RPG
 *	Unity3d Tutorial - Player Movement 2.0 Part 1
 *	to
 *	Unity3d Tutorial - Player Movement 2.0 Part 3
 */

using UnityEngine;
using System.Collections;

//To auto attach charactercontroller to Object
[RequireComponent (typeof(CharacterController))]
public class Movement : MonoBehaviour {
	public float rotateSpeed = 100;		//
	public float rsAtwalking = 70;		//rotateSpeed at walking
	public float rsAtrunning = 50;		//rotateSpeed at running
	public float forwardmoveSpeed = 3;	//forward moving speed
	public float backwardMoveSpeed = 1; //backward moving speed
	public float runMultiplier = 2;		//How fast the player runs compare to walk
	public float strafeSpeed = 2.5f;	//
	public int runningDetective = 0;	//Detect if player pressed shift
	public int movingDetective = 0;		//Detect if player pressed arrow key
	
	private Transform _myTransorm;
	private CharacterController _controller;
	
	
	public void Awake(){
		_myTransorm = transform;
		_controller= GetComponent<CharacterController>();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!_controller.isGrounded){
			/* This judgment is detecting  
			 * if player is not colliding with ground
			 * then give a value as falling speed
			 * We can use this commond add our own physics effect
			 */
			_controller.Move(Vector3.down*Time.deltaTime);
		}		
		
		if(Input.GetButtonDown("Run")){
			runningDetective = 1;
		}
		else if(Input.GetButtonUp("Run")){
			runningDetective = 0;
		}
		/* Using movingDetective make sure player is still pressing the arrow key
		 * if don't add this judgement 
		 */
		if(Mathf.Abs(Input.GetAxis("Vertical"))>0){
			if(movingDetective==0){
				movingDetective = 1;
			}
		}
		else{
			if(movingDetective==1){
				movingDetective = 0;
			}
		}
		
		
		if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0){
			//make sure character is doing idle animation when player stop moving
			GetComponent<Animation>().CrossFade("idle");
		}
		
		Turn();
		Walk();
		//Strafe();

	}
	
	private void Turn(){
		if(Mathf.Abs(Input.GetAxis("Horizontal"))>0){
			GetComponent<Animation>().CrossFade("walk");
			if(movingDetective == 1 && runningDetective == 1){
				//if player press arrow key and shift at same time, do this
				_myTransorm.Rotate(0,Input.GetAxis("Horizontal")*Time.deltaTime*rsAtrunning, 0);
			}
			else if(movingDetective == 1){
				//if player only press arrow key, do this
				_myTransorm.Rotate(0,Input.GetAxis("Horizontal")*Time.deltaTime*rsAtwalking, 0);
			}
			else{
				//normally rotating
				_myTransorm.Rotate(0,Input.GetAxis("Horizontal")*Time.deltaTime*rotateSpeed, 0);	
			}
		
		}
	}
	private void Walk(){
		if(Mathf.Abs(Input.GetAxis("Vertical"))>0){
			/* Original stript is not working very well
			 * if you press shift before W or S, it's not going in to Run loop
			 * even game going in to Run and you keep press shift
			 * GetButtonDown can not identify what's shift's status
			 * it will auto consider you didn't press shift
			 * so player can not keep running
			 * It's working fine in turtoial, I think is becaouse rule has changed
			 */
			/****************************************
			 * Original commond from Hack & Slash RPG
			 * **************************************
			 * 
			if(Input.GetButtonDown("Run")){
				//Debug.Log("Move Forward: " + Input.GetAxis("Move Forward"));
				_controller.SimpleMove(_myTransorm.TransformDirection(Vector3.forward)*Input.GetAxis("Vertical")*moveSpeed*runMultiplier);
			}
			else{
				//Debug.Log("Move Forward: " + Input.GetAxis("Move Forward"));
				_controller.SimpleMove(_myTransorm.TransformDirection(Vector3.forward)*Input.GetAxis("Vertical")*moveSpeed);	
			}
			*****************************************
			*/
			float mspeed = 0.0f;
			if(Input.GetAxis("Vertical")>0){
				mspeed = forwardmoveSpeed;
			}else{
				mspeed = backwardMoveSpeed;
			}
			if(runningDetective == 1){
				if(Input.GetAxis("Vertical")>0){
					GetComponent<Animation>().CrossFade("run");
					_controller.SimpleMove(_myTransorm.TransformDirection(Vector3.forward)*Input.GetAxis("Vertical")*mspeed*runMultiplier);
				}else{
					/* Haven't look in side the simplemove method
					 * don't know why if backwalk speed more than 2
					 * character will flay
					 */
					GetComponent<Animation>().CrossFade("walk");
					_controller.SimpleMove(_myTransorm.TransformDirection(Vector3.forward)*Input.GetAxis("Vertical")*mspeed);
				}
			}
			else if(runningDetective == 0){
				GetComponent<Animation>().CrossFade("walk");
				_controller.SimpleMove(_myTransorm.TransformDirection(Vector3.forward)*Input.GetAxis("Vertical")*mspeed);
			}
			
		}
	}
	/*
	private void Strafe(){
		if(Mathf.Abs(Input.GetAxis("Strafe"))>0){
			//Debug.Log("Strafe: " + Input.GetAxis("Strafe"));
			animation.CrossFade("walk");
			_controller.SimpleMove(_myTransorm.TransformDirection(Vector3.right)*Input.GetAxis("Strafe")*strafeSpeed);	
			
		}
	}
	*/
}
