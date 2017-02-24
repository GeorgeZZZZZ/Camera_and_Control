using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//0.2.0
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Seeker))]
[RequireComponent (typeof(SimpleSmoothModifier))]
public class Selectable_Unit_Controller : MonoBehaviour {
	
	public GameObject selectionCircle;	//	use for add circle above obj after been selected

	[HideInInspector] public bool startAnimation = false;
		
	private Path path;
	private List<Vector3> pathList;

	private float movNorSpeed = 1f;
	private float movRunSpeed = 2.5f;
	private float turnSpeed = 180f;
	private float nextWayPointDistance = 0.1f;
	private int currentWayPoint = 0;

	private bool mousRBTiggerOnceFlag;
	private bool mousRBTiggerOnce;

	private Quaternion nextDir;
	private Animator anim;

	void Start () {

		anim = GetComponent <Animator> ();

		if (GetComponent ("Player_Camera_Controller_RTS_RPG") != null) {
			movNorSpeed = GetComponent <Player_Camera_Controller_RTS_RPG> ().Player_Normal_Speed;
			movRunSpeed = GetComponent <Player_Camera_Controller_RTS_RPG> ().Player_Run_Speed;
			turnSpeed = GetComponent <Player_Camera_Controller_RTS_RPG> ().Player_Turnning_Speed;
		}

		GetComponent <SimpleSmoothModifier> ().maxSegmentLength = 1;
		GetComponent <SimpleSmoothModifier> ().iterations = 5;
		GetComponent <SimpleSmoothModifier> ().strength = 0.25f;
	}

	void FixedUpdate () {
		
		Quaternion roteTo;
		Vector3 mousHitPos;

		bool mousRightButton = Input.GetMouseButton (1);

		//	set a bool vaule and judge left mouse button
		//	only triger one cycle for each time left mouse button has been pusshed down
		//	do this is because sometimes Input.GetMouseButtonUp (0) miss value from mouse button
		if (mousRightButton & !mousRBTiggerOnceFlag) {
			mousRBTiggerOnce = true;
			mousRBTiggerOnceFlag = true;
		} else if (mousRightButton & mousRBTiggerOnceFlag)
			mousRBTiggerOnce = false;
		else if (!mousRightButton)
			mousRBTiggerOnceFlag = false;

		if (mousRBTiggerOnce & selectionCircle != null) {
			if (Public_Functions.Mous_Click_Get_Pos_Dir (Camera.main, transform, LayerMask.GetMask ("floor"), out mousHitPos, out roteTo)) {
				Get_New_Path (mousHitPos);
			}
		}

		Mov_To_New_WayPoint ();

		bool camFollowing = false;
		if (GetComponent ("Player_Camera_Controller_RTS_RPG") != null) {
			camFollowing = GetComponent <Player_Camera_Controller_RTS_RPG> ().Cam_Center_Point.GetComponent <CameraFunctions> ().followPlayerFlag;
		}
		if (!camFollowing)
			Animating ();
	}

	/********************************
	 * --- Functions
	 ********************************/

	private void Mov_To_New_WayPoint () {
		
		if (path == null) {
			return;
		}

		if (currentWayPoint > path.vectorPath.Count) {
			return;
		} else if (currentWayPoint == path.vectorPath.Count) {	//	reach end of the Path
			currentWayPoint ++;
			return;
		}

		Rigidbody rigid = GetComponent <Rigidbody> ();

		Quaternion newDir = Quaternion.RotateTowards(transform.rotation, nextDir, turnSpeed * Time.deltaTime);

		rigid.MoveRotation (newDir);
		Vector3 newPos = Vector3.MoveTowards (transform.position, path.vectorPath [currentWayPoint], movNorSpeed * Time.deltaTime);
		rigid.MovePosition (newPos);

		//	tutorial is using command in comment, but don't know why are not getting corrent result value, so change to use XZSqrMagnitude ()
		//if (Vector3.Distance (transform.position, path.vectorPath [currentWayPoint]) < nextWayPointDistance) {
		//if ((transform.position - path.vectorPath [currentWayPoint]).sqrMagnitude < nextWayPointDistance * nextWayPointDistance) {
		if (XZSqrMagnitude(path.vectorPath [currentWayPoint], transform.position) < nextWayPointDistance) {
			currentWayPoint ++;
			if (currentWayPoint < path.vectorPath.Count)	//	if path not end
				Next_Dir ();	//	calculation direction for next waypoint
		}
	}

	protected float XZSqrMagnitude (Vector3 a, Vector3 b) {	//	calculate distance between a and b, only for x and z
		float dx = b.x-a.x;
		float dz = b.z-a.z;

		return dx*dx + dz*dz;
	}

	private void Get_New_Path (Vector3 tarPos) {

		Seeker seeker = GetComponent <Seeker> ();
		seeker.StartPath (transform.position, tarPos, On_Path_Complete);
	}

	private void On_Path_Complete (Path pathIn) {
		
		ABPath newPath= pathIn as ABPath;

		newPath.Claim(this);

		if (newPath.error) {
			newPath.Release(this);
			return;
		}

		if (newPath != null & path != null) 
			path.Release(this);
		
		path = newPath;
		currentWayPoint = 0;
		Next_Dir ();
	}

	private void Next_Dir () {
		
		Vector3 tempDir = (path.vectorPath [currentWayPoint] - transform.position);
		tempDir.y = 0f;
		nextDir = Quaternion.LookRotation (tempDir);
	}

	//	Animation management
	private void Animating (){
		bool test = false;

		if (path != null) {
			if (currentWayPoint < path.vectorPath.Count)
				test = true;
			else
				test = false;
		}

		anim.SetBool ("IsWalking", test);
	}
}
