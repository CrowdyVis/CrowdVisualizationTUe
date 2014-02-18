using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using UnityEditor;

public partial class Combined_Code_Version_1 : MonoBehaviour
{
		// Start is the function which is run automatically at the start of our program, will be used to
		// instantiate our game objects once we have retrieved and ordered our data. 
		void Start ()
		{	
				//Provide a reference for the starting time of the program.
				startTime = Time.time;
				
				// Creates instances for other classes to be called on during initialization
				initializer = new Initialization ();
				movementClass = new Movement_Related_Functions (numberOfPedestrians, numberOfSteps, positions, 
		                                             	prefabBlue, prefabRed, rotationSpeed, pedestrians);
		}

		// If a simulation is selected in which only the geometry is to be shown, then the following function
		// initialize only those items needed for the viewing of the geometry.
		public void viewGeometry ()
		{
				initializer.instantiateGeometry (Floor, ExtWall, IntWall, Obst);
				instantiateCameras ();
				geometry = true;
		}
		
		// If a simulation is selected in which both the geometry as well as the pedestrian movement is to be shown
		// the following function will be called and will initialize the neccessary items. It will also ensure that 
		// references to the pedestrians, the position data and the selection of puppets or speheres is returned to 
		// this main code. 
		public void startSimulation ()
		{	
				initializer.instantiateGeometry (Floor, ExtWall, IntWall, Obst);
				pedestrians = initializer.selectionSetup ();
				positions = initializer.instantiatePositionsData ();		
				spheres = initializer.instantiatePedestrians (prefabBlue, prefabRed, rotationSpeed);
				instantiateCameras ();
				simulation = true;
				finished = false;

				numberOfPedestrians = (positions.GetLength (0)) / 2;
				numberOfSteps = positions.GetLength (1);
		}

		public void instantiateCameras ()
		{
				// Will initaite the camera and target it to the first element of the gameObject list spheres
				cameraClass = new Camera_Related_Functions ();
			
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
			
				// Starting position of the new camera
				_myTransform.position = new Vector3 (10, 2, 10);
				
				// Starting angle of the new camera
				_myTransform.LookAt(new Vector3(50,0,12));
		}

		void Update ()
		{

				if (simulation) {
						if (finished) {

								if (EditorUtility.DisplayDialog ("Simulation has finished", "The simulation is finished. If you want to replay the simulation, click Replay. " +
										"If you want to go back to the main menu, click Home", "Replay", "Home")) {
										resetCounter = true;
										finished = false;
								} else {
										stopSimulation ();
								}
						} else {
								float time = Time.time;
								
								// In the following line the position of each gameObject is updated
								CamNum = movementClass.moveGameObjects (threshold, threshold2, time, CamNum, resetCounter, spheres, 
				                                        numberOfPedestrians, numberOfSteps, positions, pedestrians);
								
								
								if (CamNum == numberOfPedestrians) {
										finished = true;
										CamNum = 0;
								}
				
								// Resets the reset boolean. If button reset is pressed then the boolean is set to true and the index will be set at 
								// 1. This needs to occur once so after each movement update, the boolean needs to be given the value false. Only when
								// the button is pressed should the animation reset. If this loop is left out, then the animation will continue to 
								// reset continually after the button has been pressed once. 
								if (resetCounter == true) {
										resetCounter = false;
								}
				
								// Within this function the entire camera movement is dealt with. 
								Transform target = spheres [CamNum];	

								CamNum = cameraClass.moveCamera (CamNum, target, doZoom, zoomStep, zoomSpeed, min, max, time, _myTransform, Getswap, 
				                                 spheres, smooth, damping, bewegen, xspeed, yspeed, factor, manualcamera, point);
								

						
						}
				}

				if (geometry) {
						float time = Time.time;
						cameraClass.geometryCamera (doZoom, zoomStep, zoomSpeed, min, max, time, _myTransform, smooth, damping, bewegen, 
			                           xspeed, yspeed, factor, manualcamera, point);
		
				}
				
		}
	
		//use this for the GUI button
		void OnGUI ()
		{
				while (System.IO.File.Exists (@"D:\Desktop\Screenshots\screenshot" + countScreenshots + ".png")) {
						capturingScreenshot = false;
						countScreenshots += 1;
				}
		
				if (!capturingScreenshot) {
						if (geometry) {
								standardButtons ();
								cameraButtons ();
								screenShotButtons ();
						} else if (simulation) {
								standardButtons ();
								cameraButtons ();
								screenShotButtons ();
								simulationButtons ();
						} else {
								homeButtons ();
						}
				}
		}

		private void homeButtons ()
		{
				if (GUI.Button (new Rect (Screen.width * .1f, Screen.height * .1f, Screen.width * .35f, Screen.height * .8f), "View geometry")) {
						Debug.Log ("View geometry");
						viewGeometry ();
				}
				if (GUI.Button (new Rect (Screen.width * .55f, Screen.height * .1f, Screen.width * .35f, Screen.height * .8f), "Start simulation")) {
						startSimulation ();
				}
		}
		
		private void standardButtons ()
		{
				// home button
			
		}
		
		private void cameraButtons ()
		{
				if (simulation) {
						if (Getswap == true) {
								// previous cam
								if (GUI.Button (new Rect (380, 10, 95, 30), "new object")) {
										CamSwap ();
								}
								// next cam
								if (GUI.Button (new Rect (480, 10, 90, 30), "prev.object")) {
										PrevCam ();
								}
						}
				
						// type (free or not)
						if (GUI.Button (new Rect (270, 10, 105, 30), " newcamera")) {
								if (manualcamera == true) {
										manualcamera = false;
										Getswap = true;
								} else if (manualcamera == false) {
										manualcamera = true;
										Getswap = false;
								}
						}
				}
				// reset
				if (manualcamera) {
						if (GUI.Button (new Rect (10, 80, 50, 50), "reset")) {
				// Resetting the camera postion
				_myTransform.position = new Vector3 (10.0f, 2.0f, 10.0f);
								// Resetting the camera angle
				_myTransform.LookAt (new Vector3(50,0,12));
						}	
				}
		}
		
		private void simulationButtons ()
		{
				// play/pause
				if (Time.timeScale == 0) {
						if (GUI.Button (new Rect (10, 10, 60, 30), "Play")) {
								Time.timeScale = 1.0f;
						}
				} else {
						if (GUI.Button (new Rect (10, 10, 60, 30), "Pause")) {
								Time.timeScale = 0.0f;
						}
				}
				// speed *2, *.5
				if (GUI.Button (new Rect (75, 10, 60, 30), "x2")) {
						Time.timeScale *= 2;
				}
				if (GUI.Button (new Rect (140, 10, 60, 30), "x.5")) {
						Time.timeScale /= 2;
				}
				// label current speed
				GUI.Box (new Rect (205, 10, 60, 30), Time.timeScale.ToString ());
			
				// replay
				if (GUI.Button (new Rect (140, 45, 50, 30), "Replay")) {
						resetCounter = true;
				}
		}
		
		private void screenShotButtons ()
		{
				if (GUI.Button (new Rect (10, 45, 125, 30), "Screenshot " + countScreenshots)) {
						Application.CaptureScreenshot (@"D:\Desktop\Screenshots\screenshot" + countScreenshots + ".png");
						capturingScreenshot = true;
				}
		}

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

		void stopSimulation ()
		{
				for (int i = 0; i < numberOfPedestrians; i++) {
						if (spheres [i]) {
								Destroy (spheres [i].gameObject);
						}
				}
				destroyFunction ("InteriorWall");
				destroyFunction ("ExteriorWall");
				destroyFunction ("Obstacle");
				destroyFunction ("Floor");
				resetCounter = true;
				finished = false;
				simulation = false;
		}
	
		void destroyFunction (String tag)
		{
				GameObject[] gameObjects = GameObject.FindGameObjectsWithTag (tag);
		
				for (var i = 0; i < gameObjects.Length; i ++) {
						Destroy (gameObjects [i]);
				}
		}
	
}
