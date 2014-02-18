using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Camera_Related_Functions : MonoBehaviour
{
	
		//values that are used for calculation
		private float heightWanted;
		private float distanceWanted;
	
		// Exposed vars for the camera position from the target.
		private float height = 50f;
		private float distance = 50f;
	
		// Result vectors.
		private Vector3 zoomResult;
		private Vector3 startingOffset = new Vector3 (3, 2, 3);
		private Vector3 relCameraPosition;
		private Quaternion lookAtRotation;
		private Quaternion additionalRotation;
		private Vector3 additionalOffset;
		private Vector3 targetAdjustedPosition;
		private float r;
		private float t;
		private float newr;
		private float newt;
		private float a;
		private float b;
		private float speedMod = 10.0f;
	
		public int moveCamera (int CamNum, Transform target, bool doZoom, float zoomStep, float zoomSpeed, float min, float max, float time,
		Transform _myTransform, bool Getswap, List<Transform> spheres, bool smooth, float damping, bool bewegen, float xspeed,
		float yspeed, float factor, bool manualcamera, Vector3 point)
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
		
		
				//To make sure that target can be selected.
				if (Getswap) {
						
						// Obtains the position of the pedestrian you are currently following
						Vector3 targetPosition = target.position;
						
						// Creates an offset between the camera and the pedestrian being followed and applies this to the camera.
						startingOffset = startingOffset + zoomResult;
						_myTransform.position = target.position + startingOffset;

						if (Input.GetKey (KeyCode.A)) {
								
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

						if (Input.GetKey (KeyCode.D)) {
				
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
			
// to be able to move the camera
				if (manualcamera) {
						if (Input.GetKey (KeyCode.DownArrow)) {
								_myTransform.position -= _myTransform.forward * factor * xspeed;
						}
						if (Input.GetKey (KeyCode.UpArrow)) {
								_myTransform.position += _myTransform.forward * factor * xspeed;
						}
						if (Input.GetKey (KeyCode.LeftArrow)) {
								_myTransform.position -= _myTransform.right * factor * yspeed;
						}
						if (Input.GetKey (KeyCode.RightArrow)) {
								_myTransform.position += _myTransform.right * factor * yspeed;
						}
						if (Input.GetKey (KeyCode.LeftControl)) {
								_myTransform.position -= _myTransform.up * factor * yspeed;
						}
						if (Input.GetKey (KeyCode.LeftAlt)) {	
								_myTransform.position += _myTransform.up * factor * yspeed;
						}
						

						if (Input.GetKey (KeyCode.A)) {
								r -= xspeed * factor;
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
						if (Input.GetKey (KeyCode.D)) {
								r += xspeed * factor;
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
						if (Input.GetKey (KeyCode.W)) {
								t -= yspeed * factor;
								t = Mathf.Clamp (t, -90, 35);
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
						if (Input.GetKey (KeyCode.S)) {
								t += yspeed * factor;
								t = Mathf.Clamp (t, -90, 35);
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}


				}

				return CamNum;
		}

		public void geometryCamera (bool doZoom, float zoomStep, float zoomSpeed, float min, float max, float time, Transform _myTransform, bool smooth, float damping, bool bewegen, 
	                           float xspeed, float yspeed, float factor, bool manualcamera, Vector3 point)
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

				// to be able to move the camera
				if (manualcamera) {
						if (Input.GetKey (KeyCode.DownArrow)) {
								_myTransform.position -= _myTransform.forward * factor * xspeed;
						}
						if (Input.GetKey (KeyCode.UpArrow)) {
								_myTransform.position += _myTransform.forward * factor * xspeed;
						}
						if (Input.GetKey (KeyCode.LeftArrow)) {
								_myTransform.position -= _myTransform.right * factor * yspeed;
						}
						if (Input.GetKey (KeyCode.RightArrow)) {
								_myTransform.position += _myTransform.right * factor * yspeed;
						}
						if (Input.GetKey (KeyCode.LeftControl)) {
								_myTransform.position -= _myTransform.up * factor * yspeed;
						}
						if (Input.GetKey (KeyCode.LeftAlt)) {	
								_myTransform.position += _myTransform.up * factor * yspeed;
						}
						
						if (Input.GetKey (KeyCode.A)) {
								r -= xspeed * factor;
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
						if (Input.GetKey (KeyCode.D)) {
								r += xspeed * factor;
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
						if (Input.GetKey (KeyCode.W)) {
								t -= yspeed * factor;
								t = Mathf.Clamp (t, -90, 35);
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
						if (Input.GetKey (KeyCode.S)) {
								t += yspeed * factor;
								t = Mathf.Clamp (t, -90, 35);
								_myTransform.rotation = Quaternion.Slerp (_myTransform.rotation, Quaternion.Euler (t, r, 0), 5);
						}
			
				}

		}
}
