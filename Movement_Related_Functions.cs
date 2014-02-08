using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Movement_Related_Functions : MonoBehaviour {
	
	// These variables will be provided by the main program and will be applied accordingly.
	public int numberOfPedestrians;
	public int numberOfSteps;
	public double[,] positions;
	public GameObject prefabBlue;
	public GameObject prefabRed;
	public float rotationSpeed;
	public bool pedestrians;
	
	// These are local variables, made and used within this class only.
	public List<Transform> spheres = new List<Transform> ();
	public int index = 1;
	private int closetoend = 0;
	private float startTime;
	private bool setTime = false;
	
	public Movement_Related_Functions(int numberOfPedestrians, int numberOfSteps, double[,] positions, 
		GameObject prefabBlue, GameObject prefabRed, float rotationSpeed, bool pedestrians)
	{
		this.numberOfPedestrians = numberOfPedestrians;
		this.numberOfSteps = numberOfSteps;
		this.positions = positions;
		this.prefabBlue = prefabBlue;
		this.prefabRed = prefabRed;
		this.rotationSpeed = rotationSpeed;
		this.pedestrians = pedestrians;
	}
	
	// This function allows the instantiation of each and every game object used within the animation.
	// The list of game objects created is then returned back to the main program. 
	public List<Transform> instantiateGameObjects()
	{
		int counter = 0;
		while (counter < numberOfPedestrians) {
			if(pedestrians == false)
			{
				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);

				if (counter < numberOfPedestrians /2){
					sphere.renderer.material.color = Color.blue;
				}
				else{
					sphere.renderer.material.color = Color.red;
				}

				sphere.AddComponent<Rigidbody> ();
				sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [counter, 0]), Convert.ToInt32 (1.0), Convert.ToInt32 (positions [counter + numberOfPedestrians, 0]));
				spheres.Add (sphere.transform);
				}
			
			else {
				if (counter < numberOfPedestrians / 2) {	
					GameObject sphere =(GameObject)Instantiate (prefabBlue);
					sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [counter, 0]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [counter + numberOfPedestrians, 0]));
					spheres.Add (sphere.transform);
				} 
				else {
					GameObject sphere = (GameObject)Instantiate (prefabRed);
					sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [counter, 0]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [counter + numberOfPedestrians, 0]));
					spheres.Add (sphere.transform);
				}
			}			
			counter++;
		}
		return spheres;
	}
	
	public int moveGameObjects(float threshold, float threshold2, float time, int CamNum, bool resetCounter, List<Transform> spheres, 
	                           int numberOfPedestrians, int numberOfSteps, double[,] positions, bool pedestrians)
	{
		if (setTime == false)
		{
			startTime = Time.time;
			setTime = true;
		}
	
		// For the resetting of the animation
		if(resetCounter == true)
		{			
			index = 1;
			resetCounter = false;
		}
		
		// For movement
		int counter = 0;
		float startertime = Time.time;
		while (counter < numberOfPedestrians) {
			if(spheres[counter]){
					
				float realTime = Time.time;
				Transform sphere = spheres [counter];
				
				Vector3 tempStart = sphere.transform.position;
				Vector3 tempNext = sphere.transform.position;

				//For Rotation
				Vector3 nextlook = sphere.transform.position; // vector variable used to store next position to look at

				//For Rotation
				Quaternion currotation = sphere.transform.localRotation; // saves the current rotation position

				// Added -3 for the rotation (Means we wont need a separate function for movement and rotation)
				if (index <= numberOfSteps-3) 
				{
					tempStart.x = (float)positions[counter, index - 1];
					tempStart.z = (float)positions[(counter + numberOfPedestrians), index - 1];
					tempNext.x = (float)positions [counter, index];
					tempNext.z = (float)positions [(counter + numberOfPedestrians), index];	

					if(Math.Abs(tempNext.x-tempStart.x) > 2.0f)
					{
						Destroy(spheres[counter].gameObject);						
						
						if (CamNum == counter){
							CamNum++;
						}
					}
					
					//For Rotation
					float averagex = ((float)positions [counter , index+1]+
						(float)positions [counter, index+2]+
						(float)positions [counter, index+3])/3.0f; // the average value for x is calculated from the next 3 values
					float averagey = ((float)positions [counter + numberOfPedestrians, index+1]+
						(float)positions [counter + numberOfPedestrians, index+2]+
						(float)positions [counter + numberOfPedestrians, index+3])/3.0f; // the average value for y is calculated from the next 3 values
					
					//Determine difference in both x and y direction between now and in 3 steps in the future.
					double xdifference = (Math.Abs(positions[counter, index] - positions[ counter, index + 3]));
					double ydifference = (Math.Abs(positions[counter + numberOfPedestrians, index] - positions[counter, index + 3]));
					
					// If difference between boundaries then the next rotation position is determined.
					if (xdifference < 10 || ydifference < 5 )
					{
	
					nextlook.x = averagex; // x coordinate of the next position to look at is the average of x
					nextlook.z = averagey; // y coordinate of the next position to look at is the average of y
					
					} 
					else {nextlook = tempNext;}
					
					// The Rotation is determined by the rotationspeed, start position, current rotation and the position of the look target
					Quaternion nextrotation = Quaternion.Slerp(currotation, Quaternion.LookRotation(nextlook - tempStart), 12 * (time - startTime));
	
					// The rotation is adjusted.
					sphere.transform.localRotation = nextrotation;
	
					
					//Back to movement of shperes
					if (spheres[counter]){
						sphere.transform.position = Vector3.Lerp (tempStart, tempNext, 6*(time - startTime));
						
						if (Vector3.Distance (sphere.transform.position, tempNext) < threshold) {                                                         
							closetoend++;
						}
					}
				}
			}
				counter++;
		}
		// If more than 50 percent of the particles have reached their destination then move onto the next destination.
		if (closetoend > (3*counter)/4){
			startTime = Time.time;
			index++;
			
			// If a game object has been destroyed then it should be remade at its next position which falls in the range of the playing field. 
			for(int i = 0; i < numberOfPedestrians; i++)
			{
				if (spheres[i] == null)
				{
					if(positions[i, index] > 0.0f && positions[i,index] < 120.0f && Math.Abs(positions[i, index] - positions[i, index-1]) < 2.0f)
					{
						if(pedestrians == false){
							GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
							if (i < numberOfPedestrians /2){
								sphere.renderer.material.color = Color.blue;
							}
							else{
								sphere.renderer.material.color = Color.red;
							}
							sphere.AddComponent<Rigidbody> ();
							sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [i, index]), Convert.ToInt32 (1.0), Convert.ToInt32 (positions [i + numberOfPedestrians, index]));
							spheres[i] = sphere.transform;
						}
						else{
							GameObject sphere = (GameObject)Instantiate (prefabBlue);
							sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [i, index]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [i + numberOfPedestrians, index]));
							spheres[i] = sphere.transform;
						}
					}
				}
			}
		}
		
		closetoend = 0;
		return CamNum;
	}

}
