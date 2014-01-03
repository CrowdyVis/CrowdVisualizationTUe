using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Camera_Related_Functions : MonoBehaviour {
	
	//values that are used for calculation
	private float heightWanted;
	private float distanceWanted;
	
	// Exposed vars for the camera position from the target.
	private float height = 50f;
	private float distance = 50f;
	
	// Result vectors.
	private Vector3 zoomResult;
	private Quaternion rotationResult;
	private Vector3 targetAdjustedPosition;
	private float r;
	private float t;
	private float a;
	private float b;
	
	
	public int moveCamera(int CamNum, Transform target, bool doZoom, float zoomStep, float zoomSpeed, float min, float max, float time,
		Transform _myTransform, bool Getswap, List<Transform> spheres, bool smooth, float damping, bool bewegen, float xspeed,
		float yspeed, float factor, bool manualcamera)
	{		
		//if the option Zoom is selected
		if (doZoom) {
			// Record our mouse input.  If we zoom add this to our height and distance.
			float mouseInput = Input.GetAxis ("Mouse ScrollWheel");
			heightWanted -= zoomStep * mouseInput;
			distanceWanted -= zoomStep * mouseInput;
 
			// Make sure they meet our min/max values.
			heightWanted = Mathf.Clamp (heightWanted, min, max);
			distanceWanted = Mathf.Clamp (distanceWanted, min, max);
 			
			//defining again the values for height and distance
			height = Mathf.Lerp (height, heightWanted, time * zoomSpeed);
			distance = Mathf.Lerp (distance, distanceWanted, time * zoomSpeed);
 
			// Post our result.
			zoomResult = new Vector3 (0f, height, -distance);
		}
		
		
		// DO NOT KNOW THE FUNCTION OF THIS !!!!!!
		// Set the camera position reference.
		targetAdjustedPosition = rotationResult * zoomResult;
		if(Getswap){
			_myTransform.position = target.position + targetAdjustedPosition;
			
			// Face the desired position.
			_myTransform.LookAt (target);
		}
		
		//camera swap	
		if (Getswap == true) {
			target = spheres [CamNum];
		}	
		
		if (target) {
			//initialising the smoothing
			if (smooth) {
				// Look at and dampen the rotation
				var rotation = Quaternion.LookRotation (target.position - _myTransform.position);
				_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, rotation, time * damping);
			} else {
				// Just lookat
				_myTransform.LookAt (target);
			}
		}
		
		//private movement
		if (bewegen) {
			if (Input.GetKey (KeyCode.A))
				r -= xspeed * factor;
			else if (Input.GetKey (KeyCode.D))
				r += xspeed * factor;
			if (Input.GetKey (KeyCode.W))
				t -= yspeed * factor;
			else if (Input.GetKey (KeyCode.S))
				t += yspeed * factor;

			
			Quaternion gedoe = Quaternion.Euler (t, r, 0);
		
			_myTransform.rotation = gedoe;	
		}	
		
		if (manualcamera){
			if(Input.GetKey(KeyCode.DownArrow))
				_myTransform.position -= _myTransform.forward*factor*xspeed;
			if(Input.GetKey(KeyCode.UpArrow))
				_myTransform.position += _myTransform.forward*factor*xspeed;
			if(Input.GetKey(KeyCode.LeftArrow))
				_myTransform.position -= _myTransform.right*factor*yspeed;
			if(Input.GetKey(KeyCode.RightArrow))
				_myTransform.position += _myTransform.right*factor*yspeed;

			if (Input.GetKey (KeyCode.A)){
				r -= xspeed * factor;
			}
			else if (Input.GetKey (KeyCode.D)){
				r += xspeed * factor;
			}
		}

			Quaternion gedoe2 = Quaternion.Euler (0, r, 0);

			_myTransform.rotation = gedoe2;

		
		return CamNum;
	}
}
