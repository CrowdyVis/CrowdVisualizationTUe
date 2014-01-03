using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class RotationScript : MonoBehaviour 
{

	// !!! All variables defined in the y direction in MATLAB are defined in the z direction in UNITY !!!
	// These are public variables, in the user interface the user can select which text files to use for the x and z coordinates. 
	public TextAsset myTextFilex; 
	public TextAsset myTextFilez;
	
	public float threshold = 0.001f;
	public float threshold2 = 10.0f;
	public float threshold3 = 2.0f; // used to prevent rotation jumps
		
	public float RotationSpeed = 1.0f;
	
	// Private variables, startTime records the startTime, and the xcoefficeint and zcoefficient are variable which will take the value 1 or -1, we will see why later on
	float startTime;
	float xcoefficient;
	float zcoefficient;
	
	// Private variables, These will become the lists containing the x and z coordinates respectivly.
	float[] xWaypoints;
	float[] zWaypoints;
	
	// This is the iteration index, will allow us to iterate through the coordinates.
	int index = 1;
	
	// Use this for initialization
	void Start () 
	{
		startTime = Time.time; // Assign time to the variable startTime
		string[] xWaypointsstring = myTextFilex.text.Split(";"[0]); // Splits the text file on its semicolons, each element is saved as a string
		string[] zWaypointsstring = myTextFilez.text.Split(";"[0]); // Splits the text file on its semicolons, each element is saved as a string
		
		xWaypoints = new float[xWaypointsstring.Length]; // create array for x positions			
		zWaypoints = new float[zWaypointsstring.Length]; // create array for y position
		
		for ( int i = 0; i < xWaypointsstring.Length; i ++ ) // fill both arrays with their coordinates
		{
 			xWaypoints[i] = float.Parse(xWaypointsstring[i]);
 			zWaypoints[i] = float.Parse(zWaypointsstring[i]);
  		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 tempstart = transform.position; // vector variable used to store start position
	    Vector3 tempnext = transform.position; // vector variable used to store next position
		Vector3 nextlook = transform.position; // vector variable used to store next position to look at
		
		Quaternion currotation = transform.localRotation; // saves the current rotation position
		
		
		if(index < (xWaypoints.Length)-3) // to prevent array out of bound exception	
		{
			
			tempstart.x = xWaypoints[index-1]; // start position is determined
			tempstart.z = zWaypoints[index-1];
			
			tempnext.x = xWaypoints[index]; // target position is determined
			tempnext.z = zWaypoints[index];
		    
			float averagex = ((xWaypoints[index+1])+(xWaypoints[index+2])+(xWaypoints[index+3]))/3.0f; // the average value for x is calculated from the next 3 values
			float averagey = ((zWaypoints[index+1])+(zWaypoints[index+2])+(zWaypoints[index+3]))/3.0f; // the average value for y is calculated from the next 3 values
			
//			// used for error checking
//			Debug.Log(index);
//			Debug.Log("averagex"+averagex);
//			Debug.Log("averagey"+averagey);
			
			nextlook.x = averagex; // x coordinate of the next position to look at is the average of x
			nextlook.z = averagey; // y coordinate of the next position to look at is the average of y
			
			// The Rotation is determined by the rotationspeed, start position, current rotation and the position of the look target
			Quaternion nextrotation = Quaternion.Slerp(currotation, Quaternion.LookRotation(nextlook - tempstart), RotationSpeed * Time.deltaTime);
			
			// If the rotation changes to fast the object rotates extremely slow (this to prevent unnatural rotation jumps)
			float deltarotation = currotation.eulerAngles.y - nextrotation.eulerAngles.y;
			
			// used for error checking 
//			Debug.Log(deltarotation);
			
			// When the rotation is too sudden instead the currotation is used = inelegant solution though
			if(deltarotation > threshold3) 
			{	
				nextrotation = currotation;
			}				
		    
			// The rotation is adjusted.
			transform.localRotation = nextrotation;
				
	        // When the object reaches its target position, it targets the next position
			if(Vector3.Distance(transform.position,tempnext) < threshold)
			{                                                         
				startTime = Time.time;
                index++;
				if (xWaypoints[index-1] - averagex > threshold2)
				{
					index++;	
				}
			}
		}
	}
}

