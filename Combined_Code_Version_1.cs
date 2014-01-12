using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Combined_Code_Version_1 : MonoBehaviour
{
	//Making the Walls
	private WallCreatorWithObstacles wallCreator;
	public GameObject ExtWall;
	public GameObject IntWall;
	public GameObject Obst;
	public GameObject Floor;
	public string geometryFilename = "C:\\Users\\s115921\\Dropbox\\ICMS onderzoek Honors\\Unity\\Roy\\Geometry_poging_3.0\\geometryWithObstacles.xml";
	
	// Providing the Positions Array
	private Creation_Of_Positions_Array positionDataCreator;
	private double[,] positions;
	public string positionsFilename = "C:\\Users\\s115921\\Dropbox\\ICMS onderzoek Honors\\Matlab m files\\Ardjan\\Newest_Code\\pedestrian_data.xml";
	
	// Making of the Spheres/People
	private Movement_Related_Functions movementClass;
	private int numberOfSteps;
	private int numberOfPedestrians;
	private List<Transform> spheres = new List<Transform> ();
	public GameObject prefabBlue;
	public GameObject prefabRed;
	public bool pedestrians = false;
	
	// GUI
	bool capturingScreenshot = false;
	int countScreenshots = 1;
	public int CamNum;
	public int MaxCam = 1 ;
	bool resetCounter = false;
	
	// GameObject Movement
	float startTime;
	public float threshold = 0.001f;
	public float threshold2 = 10.0f;
	public float rotationSpeed = 1.0f;
	
	// The Camera
	private Camera_Related_Functions cameraClass;
	
	// Camera Initialisation
	public bool doZoom = true;
	
	// Camera target to look at.
	private Transform target;

	// Camera limits.
	public float min = 2f;
	public float max = 60;
 
	// Rotation.
	public float rotateSpeed = 1f;
	
	// The movement amount when zooming.
	public float zoomStep = 3f;
	public float zoomSpeed = 2f;
	
	//initialising the options	
	public float damping = 6;
	public bool smooth = true;
	public bool Getswap = false ;
	
	//private movement
	private Vector3 zoomResult;
	private float heightWanted;
	private float distanceWanted;
	private float height = 50f;
	private float distance = 50f;
	private float xspeed = 50.0f;
	private float yspeed = 50.0f;
	private Transform _myTransform;
	private float factor = 0.05f;
	public bool bewegen = true;
	public bool manualcamera = true;
	private float nx = 10;
	private float ny = 2;
	private float nz = 10;
	private Vector3 newposition;
	
	// Start is the function which is run automatically at the start of our program, will be used to
	// instantiate our game objects once we have retrieved and ordered our data. 
	void Start ()
	{	
		//Provide a reference for the starting time of the program.
		startTime = Time.time;
		
		// Will Instantiate the walls provided within an XMLfile
		wallCreator = new WallCreatorWithObstacles (Floor, ExtWall, IntWall, Obst, geometryFilename);
		wallCreator.makeWalls ();
		
		/* Will provide the program with a matrix of doubles used to store the positions of each and every
		 * game object extracted from an XML file. Also determines the number of steps and pedestrians involved. 
		 */
		positionDataCreator = new Creation_Of_Positions_Array (positionsFilename);
		positions = positionDataCreator.createPositionsArray ();
		numberOfPedestrians = (positions.GetLength (0)) / 2;
		numberOfSteps = positions.GetLength (1);
		
		/* Will provide a list of all the gameobjects needed and will instantiate these within the game at their
		 * correct starting position.
		 */
		movementClass = new Movement_Related_Functions (numberOfPedestrians, numberOfSteps, positions, 
			prefabBlue, prefabRed, rotationSpeed, pedestrians);
		spheres = movementClass.instantiateGameObjects ();
		
		// Will initaite the camera and target it to the first element ogf the gameObject list spheres
		cameraClass = new Camera_Related_Functions ();
		//cameraClass.cameraInitialisation();
		
		//for the camera
		
		//initializing the starting camera
		target = spheres [0];
		
		// zoom and autorotated
		// Initialise default zoom values
		heightWanted = height;
		distanceWanted = distance;
 
		// Setup our default camera.  We set the zoom result to be our default position.
		zoomResult = new Vector3 (0f, height, -distance);
		
		// camera swap
		//at the start, the amount of camera's changes
		MaxCam = spheres.Count - 1; 
		
		//private movement	
		//defining that the private variables will be used for transformation
		_myTransform = transform;
		
		//starting with the camera position
		Vector3 newposition = new Vector3 (10, 2, 10);
		_myTransform.position = newposition;
		
		Quaternion newangle = Quaternion.Euler (0, 0, 0);
		_myTransform.rotation = newangle;
	}

	void Update ()
	{
		float time = Time.time;
		Debug.Log (CamNum);
		// In the following line the position of each gameObject is updated.
		CamNum = movementClass.moveGameObjects (threshold, threshold2, time, CamNum, resetCounter);
		
		// Resets the reset boolean. If button reset is pressed then the boolean is set to true and the index will be set at 
		// 1. This needs to occur once so after each movement update, the boolean needs to be given the value false. Only when
		// the button is pressed should the animation reset. If this loop is left out, then the animation will continue to 
		// reset continually after the button has been pressed once. 
		if (resetCounter == true) {
			resetCounter = false;
		}
		
		// Within this function the entire camera movement is dealt with. 
		target = spheres [CamNum];	
		
		CamNum = cameraClass.moveCamera (CamNum, target, doZoom, zoomStep, zoomSpeed, min, max, time, _myTransform, Getswap, 
			spheres, smooth, damping, bewegen, xspeed, yspeed, factor, manualcamera);
	}
	
	
	
	//use this for the GUI button
	void OnGUI ()
	{
		while (System.IO.File.Exists (@"D:\Desktop\Screenshots\screenshot" + countScreenshots + ".png")) {
			capturingScreenshot = false;
			countScreenshots += 1;
		}
		
		if (!capturingScreenshot) {
			if (GUI.Button (new Rect (10, 45, 120, 30), "Screenshot " + countScreenshots)) {
				Application.CaptureScreenshot (@"D:\Desktop\Screenshots\screenshot" + countScreenshots + ".png");
				capturingScreenshot = true;
			}
		}
		
		if (Time.timeScale == 0) {
			if (GUI.Button (new Rect (10, 10, 60, 30), "Play")) {
				Time.timeScale = 1.0f;
			}
		} else {
			if (GUI.Button (new Rect (10, 10, 60, 30), "Pause")) {
				Time.timeScale = 0.0f;
			}
		}
		

		if (GUI.Button (new Rect (75, 10, 60, 30), "x2")) {
			Time.timeScale *= 2;
		}
		if (GUI.Button (new Rect (140, 10, 60, 30), "x.5")) {
			Time.timeScale /= 2;
		}
			
		//for selecting a different camera
		if (GUI.Button (new Rect (200, 10, 105, 30), " newcamera")) {
			if (manualcamera == true) {
				manualcamera = false;
				Getswap = true;
			} else if (manualcamera == false) {
				manualcamera = true;
				Getswap = false;
			}
		}
		//for selecting a new object
		if (Getswap == true) {
			if (GUI.Button (new Rect (310, 10, 95, 30), "new object")) {
				CamSwap ();
			}
			if (GUI.Button (new Rect (425, 10, 90, 30), "prev.object")) {
				PrevCam ();
			}
		}
		
		// Show Animation Speed
		GUI.Box (new Rect (205, 10, 60, 30), Time.timeScale.ToString ());
		
		//resetting the camera	
		if (GUI.Button (new Rect (350, 150, 50, 50), "reset")) {
			//resetting the postion
			Vector3 newposition = new Vector3 (10, 2, 10);
			_myTransform.position = newposition;
				
			Quaternion newangle = Quaternion.Euler (0, 0, 0);
			_myTransform.rotation = newangle;
		}	
			
//		if (GUI.Button (new Rect (650, 10, 80, 30), "Next Cam"))
//		{
//			CamSwap (true);
//			if(manualcamera == false){
//				manualcamera = true;
//				Getswap = false;
//			}
//			else if(manualcamera == true){
//				manualcamera = false;
//				Getswap = true;
//			}
//		}
//		
//		if (GUI.Button (new Rect (735, 10, 120, 30), "Previous Cam")){
//			CamSwap (false);
//			if(manualcamera == false){
//				manualcamera = true;
//				Getswap = false;
//			}
//			else if(manualcamera == true){
//				manualcamera = false;
//				Getswap = true;
//			}
//		}
		
		if (GUI.Button (new Rect (595, 10, 50, 30), "Replay")) {
			resetCounter = true;
		}
		
	}	
	
//	public void CamSwap (bool upDown)
//	{
//		if (upDown){
//			CamNum ++; 
//		}
//		else{
//			CamNum --;
//		}
//		if (CamNum >= MaxCam) {
//			CamNum = 0; 
//		} 
//		if (CamNum < 0){
//			CamNum = MaxCam-1;
//		}
//	}
	//initialising the camswap, by changing the number
	public void CamSwap ()
	{
		if (CamNum >= MaxCam) {
			CamNum = 0; 	
		} else {
			CamNum ++; 
		}
	}

	public void PrevCam ()
	{
		if (CamNum > 1) {
			CamNum --;
		} else {
			CamNum = MaxCam;
		}
	}

}
