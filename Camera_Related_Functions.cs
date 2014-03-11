using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Camera_Related_Functions : MonoBehaviour
{
	
		//values that are used for calculation.
		private float heightWanted;
		private float distanceWanted;
	
		// Exposed variables for the camera position from the target.
		private float height = 50f;
		private float distance = 50f;
	
		// Result vectors.
		private Vector3 zoomResult;
		private Vector3 startingOffset = new Vector3 (3, 2, 3);
		private Vector3 relCameraPosition;
		private Quaternion lookAtRotation;
		private Quaternion additionalRotation;
		private Vector3 additionalOffset;
		
		//variables for vectors/quaternions.
		private float r;
		private float t;
		
		// importing the variables from the public class for the animation scenario.
		public int moveCamera (int CamNum, Transform target, bool doZoom, float zoomStep, float zoomSpeed, float min, float max, float time,
		Transform _myTransform, bool Getswap, List<Transform> spheres, bool smooth, float damping, bool bewegen, float xspeed,
		float yspeed, float factor, bool manualcamera, Vector3 point)

		//update function for the animation.
		{		
				//zoom function.
			if (doZoom){

						// Record our mouse scrollwheel input.  If we zoom add this to our height and distance.
						float mouseInput = Input.GetAxis ("Mouse ScrollWheel");
						heightWanted -= zoomStep * mouseInput;
						distanceWanted -= zoomStep * mouseInput;
 
						// Make sure they meet our min/max values.
						heightWanted = Mathf.Clamp (heightWanted, min, max);
						distanceWanted = Mathf.Clamp (distanceWanted, min, max);
 			
						//smoothing the transformation between the current position and the desired position
						height = Mathf.Lerp (height, heightWanted, time * zoomSpeed);
						distance = Mathf.Lerp (distance, distanceWanted, time * zoomSpeed);
 
						// Post our result in the vector.
						zoomResult = new Vector3 (0f, height, -distance);
						}
		
		
				//the function that allows to change the camera position to a different moving object
			if (Getswap){
						
						// Obtains the position of the pedestrian you are currently following
						Vector3 targetPosition = target.position;
						
						// Creates an offset between the camera and the pedestrian being followed and applies this to the camera.
						startingOffset = startingOffset + zoomResult;
						_myTransform.position = target.position + startingOffset;

						//rotation around the object clockwise
						if (Input.GetKey (KeyCode.A)) 
								{
								// Will create a new offset, using the current offset and multiplying this by a rotation around the focus
								// of this offset, in this case the pedestrian. The camera position is then updated to hold this position.
								additionalRotation = Quaternion.AngleAxis (1, Vector3.up);
								additionalOffset = additionalRotation * startingOffset;
								startingOffset = additionalOffset;
								_myTransform.position = Vector3.Lerp ((target.position + startingOffset), (target.position + additionalOffset), 5);
								
								// Determines the new viewing angle of the camera. A vector is created between the camera and the pedestrian
								// which is then used to determine the roation the camera should hold to view the pedestrian. This camera rotation
								// is then set to this value.
								relCameraPosition = target.position - _myTransform.position;
								lookAtRotation = Quaternion.LookRotation (relCameraPosition, Vector3.up);
								_myTransform.rotation = lookAtRotation;
								
								// The newly defined offset must be stored for the next time. 
								startingOffset = additionalOffset;
								}
						
						//rotation around the object counter clockwise
						if (Input.GetKey (KeyCode.D)) 
								{
								// Will create a new offset, using the current offset and multiplying this by a rotation around the focus
								// of this offset, in this case the pedestrian. The camera position is then updated to hold this position.
								additionalRotation = Quaternion.AngleAxis (-1, Vector3.up);
								additionalOffset = additionalRotation * startingOffset;
								startingOffset = additionalOffset;
								_myTransform.position = Vector3.Lerp ((target.position + startingOffset), (target.position + additionalOffset), 5);
				
								// Determines the new viewing angle of the camera. A vector is created between the camera and the pedestrian
								// which is then used to determine the roation the camera should hold to view the pedestrian. This camera rotation
								// is then set to this value.
								relCameraPosition = target.position - _myTransform.position;
								lookAtRotation = Quaternion.LookRotation (relCameraPosition, Vector3.up);
								_myTransform.rotation = lookAtRotation;
				
								// The newly defined offset must be stored for the next time. 
								startingOffset = additionalOffset;
								}
						}
			
		// the manual camera function
				if (manualcamera) 
						{
								//movement function.
								// if a button is pressed, the camera will be adjusted in the desired direction on the x,y and z axis
								
								//forward direction
								if (Input.GetKey (KeyCode.DownArrow)) {
										_myTransform.position -= _myTransform.forward * factor * xspeed;
								}
								//backwards direction
								if (Input.GetKey (KeyCode.UpArrow)) {
										_myTransform.position += _myTransform.forward * factor * xspeed;
								}
								//left direction
								if (Input.GetKey (KeyCode.LeftArrow)) {
										_myTransform.position -= _myTransform.right * factor * yspeed;
								}
								//right direction
								if (Input.GetKey (KeyCode.RightArrow)) {
										_myTransform.position += _myTransform.right * factor * yspeed;
								}
								//downwards direction
								if (Input.GetKey (KeyCode.LeftControl)) {
										_myTransform.position -= _myTransform.up * factor * yspeed;
								}
								//upwards direction
								if (Input.GetKey (KeyCode.LeftAlt)) {	
										_myTransform.position += _myTransform.up * factor * yspeed;
								}
								
								//rotation function
								// if a button is pressed, the camera will rotate around the x and y axis 
								
								//rotating clockwise
								if (Input.GetKey (KeyCode.A)) {
										r -= xspeed * factor;
										_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
								}
								//rotating counterclockwise
								if (Input.GetKey (KeyCode.D)) {
										r += xspeed * factor;
										_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
								}
								//rorating downwards
								if (Input.GetKey (KeyCode.W)) {
										t -= yspeed * factor;
										t = Mathf.Clamp (t, -90, 35);
										_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
								}
								//rotating upwards
								if (Input.GetKey (KeyCode.S)) {
										t += yspeed * factor;
										t = Mathf.Clamp (t, -90, 35);
										_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
								}


						}
				//returning the number of the camera
				return CamNum;
		}
		
		// importing the variables from the public class for the geometry scenario.
		public void geometryCamera (bool doZoom, float zoomStep, float zoomSpeed, float min, float max, float time, Transform _myTransform, bool smooth, float damping, bool bewegen, 
	    	                            float xspeed, float yspeed, float factor, bool manualcamera, Vector3 point)
		//update function for the geometry.
		{
				//zoom function.
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

				// the manual camera function
				if (manualcamera)
						{
							//movement function.
							// if a button is pressed, the camera will be adjusted in the desired direction on the x,y and z axis
							
							//forward direction
							if (Input.GetKey (KeyCode.DownArrow)){
									_myTransform.position -= _myTransform.forward * factor * xspeed;
							}
							//backwards direction
							if (Input.GetKey (KeyCode.UpArrow)) {
									_myTransform.position += _myTransform.forward * factor * xspeed;
							}
							//left direction
							if (Input.GetKey (KeyCode.LeftArrow)){
									_myTransform.position -= _myTransform.right * factor * yspeed;
							}
							//right direction
							if (Input.GetKey (KeyCode.RightArrow)) {
									_myTransform.position += _myTransform.right * factor * yspeed;
							}
							//downwards direction
							if (Input.GetKey (KeyCode.LeftControl)) {
									_myTransform.position -= _myTransform.up * factor * yspeed;
							}
							//upwards direction
							if (Input.GetKey (KeyCode.LeftAlt)) {	
									_myTransform.position += _myTransform.up * factor * yspeed;
							}
							
							//rotation function
							// if a button is pressed, the camera will rotate around the x and y axis 

							//rotating clockwise
							if (Input.GetKey (KeyCode.A)) {
									r -= xspeed * factor;
									_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
							}
							//rotating counter clockwise
							if (Input.GetKey (KeyCode.D)) {
									r += xspeed * factor;
									_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
							}
							//rotating downwards
							if (Input.GetKey (KeyCode.W)) {
									t -= yspeed * factor;
									t = Mathf.Clamp (t, -90, 35);
									_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
							}
							//rotating upwards
							if (Input.GetKey (KeyCode.S)) {
									t += yspeed * factor;
									t = Mathf.Clamp (t, -90, 35);
									_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
							}
						
						}

		}
}
