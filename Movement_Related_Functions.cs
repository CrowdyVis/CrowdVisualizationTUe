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
	
    // this function instantiates the different global variables.
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
		while (counter < numberOfPedestrians) { // For all pedestrians
			if(pedestrians == false) // If pedestrian mode is off
			{
				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere); // The program creates spheres to represent the pedestrians

				if (counter < numberOfPedestrians /2){ // Half of them will be blue
					sphere.renderer.material.color = Color.blue;
				}
				else{
					sphere.renderer.material.color = Color.red; // Half of them will be red
				}
				// Furthermore a rigidbody is added, the spere is placed on its position and added to the list spheres containing the GameObjects
				sphere.AddComponent<Rigidbody> ();
				sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [counter, 0]), Convert.ToInt32 (1.0), Convert.ToInt32 (positions [counter + numberOfPedestrians, 0]));
				spheres.Add (sphere.transform);
				}
			
			else { // If pedestrian mode is selected
				if (counter < numberOfPedestrians / 2) { // Half of the GameObjects is instantiated to be prefabBlue
					GameObject sphere =(GameObject)Instantiate (prefabBlue);
					sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [counter, 0]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [counter + numberOfPedestrians, 0]));
					spheres.Add (sphere.transform);
				} 
				else { // The other half of the GameObjects is instatiated as prefabRed
					GameObject sphere = (GameObject)Instantiate (prefabRed);
					sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [counter, 0]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [counter + numberOfPedestrians, 0]));
					spheres.Add (sphere.transform);
				}
			}			
			counter++;
		}
		return spheres;
	}
	
    // This function goes through the entire list of gameobjects and moves them to their next position as well as adjusting their respective rotation.
	public int moveGameObjects(float threshold, float threshold2, float time, int CamNum, bool resetCounter, List<Transform> spheres, 
	                           int numberOfPedestrians, int numberOfSteps, double[,] positions, bool pedestrians)
	{
		if (setTime == false)
		{
			startTime = Time.time;
			setTime = true;
		}
	
		// This if statement is used to reset the animation when this is needed.
		if(resetCounter == true)
		{			
			index = 1;
			resetCounter = false;
		}
		
		int counter = 0;
		float startertime = Time.time;
		while (counter < numberOfPedestrians) { // For all pedestrians
			if(spheres[counter]){
					
				float realTime = Time.time;
				Transform sphere = spheres [counter];
				
				Vector3 tempStart = sphere.transform.position;
				Vector3 tempNext = sphere.transform.position;

				Vector3 nextlook = sphere.transform.position; // vector variable used to store next position to look at

				Quaternion currotation = sphere.transform.localRotation; // saves the current rotation position

                if (index <= numberOfSteps - 4) // Added -3 such that the rotation part of the function can be used.
				{
					tempStart.x = (float)positions[counter, index - 1];
					tempStart.z = (float)positions[(counter + numberOfPedestrians), index - 1];
					tempNext.x = (float)positions [counter, index];
					tempNext.z = (float)positions [(counter + numberOfPedestrians), index];	

					if(Math.Abs(tempNext.x-tempStart.x) > 2.0f) // When the pedestrian objects move from one side to the other the object will be destroyed (and later be remade)
					{
						Destroy(spheres[counter].gameObject);						
						
						if (CamNum == counter){
							CamNum++;
						}
					}
					
					// In order to smoothen the rotation process a rolling average is used.
					float averagex = ((float)positions [counter , index+1]+
						(float)positions [counter, index+2]+
						(float)positions [counter, index+3])/3.0f; // the average value for x is calculated from the next 3 values
					float averagey = ((float)positions [counter + numberOfPedestrians, index+1]+
						(float)positions [counter + numberOfPedestrians, index+2]+
						(float)positions [counter + numberOfPedestrians, index+3])/3.0f; // the average value for y is calculated from the next 3 values
					
					//Determines the difference in both x and y direction between now and 3 steps in the future.
					double xdifference = (Math.Abs(positions[counter, index] - positions[ counter, index + 3]));
					double ydifference = (Math.Abs(positions[counter + numberOfPedestrians, index] - positions[counter, index + 3]));
					
					// If difference between boundaries then the next rotation position is determined.
					if (xdifference < 10 || ydifference < 5 ){	
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
			for(int i = 0; i < numberOfPedestrians; i++) // This for loop loops through the list of GameObjects.
			{
				if (spheres[i] == null) // When the loop encounters an GameObject with value null (a destroyed game object) ...
				{
					if(positions[i, index] > 0.0f && positions[i,index] < 120.0f && Math.Abs(positions[i, index] - positions[i, index-1]) < 2.0f) // ... and is within the playing field ...
					{
						if(pedestrians == false){ // ... a sphere is created when pedestrians mode was turned off.
							GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
							if (i < numberOfPedestrians /2){ // This if function checks for the color of the object.
								sphere.renderer.material.color = Color.blue;
							}
							else{ 
								sphere.renderer.material.color = Color.red;
							}
							// The object is initialized on the right position.
							sphere.AddComponent<Rigidbody> (); 
							sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [i, index]), Convert.ToInt32 (1.0), Convert.ToInt32 (positions [i + numberOfPedestrians, index]));
							spheres[i] = sphere.transform; // The object is placed in the list with GameObjects
						}
						else{ // ... a pedestrian is created when pedestrian mode was on.
							if (i < numberOfPedestrians / 2) { // The color of the pedestrian model is determined and placed on the playing field.
								GameObject sphere =(GameObject)Instantiate (prefabBlue); // The blue variant
								sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [i, index]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [i + numberOfPedestrians, index]));
								spheres[i] = sphere.transform;
							}
								else{ 
								GameObject sphere = (GameObject)Instantiate (prefabRed); // The red variant
								sphere.transform.position = new Vector3 (Convert.ToInt32 (positions [i, index]), Convert.ToInt32 (0.0), Convert.ToInt32 (positions [i + numberOfPedestrians, index]));
								spheres[i] = sphere.transform;
							}
								

							}
						}
					}
				}
			}
		
		closetoend = 0;
		if (index > numberOfSteps - 4) 
		{
			return numberOfPedestrians;
		} 
		else 
		{
			return CamNum;
		}
	}

}
