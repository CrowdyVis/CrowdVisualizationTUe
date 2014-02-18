using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using UnityEditor;

public partial class Combined_Code_Version_1 : MonoBehaviour
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
	private Initialization initializer;
	private bool geometry = false;
	private bool simulation = false;
	bool capturingScreenshot = false;
	int countScreenshots = 1;
	public int CamNum;
	public int MaxCam = 1 ;
	bool resetCounter = false;
	bool finished;
	
	// GameObject Movement
	float startTime;
	public float threshold = 0.001f;
	public float threshold2 = 10.0f;
	public float rotationSpeed = 1.0f;
	
	// The Camera
	private Camera_Related_Functions cameraClass;
	
	// Camera Initialisation
	private Vector3 resetTarget = new Vector3 (40.0f, 0.0f, 15.0f);
	private Vector3 relCameraPosition;
	private Quaternion lookAtRotation;
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
	private Vector3 point;
}
