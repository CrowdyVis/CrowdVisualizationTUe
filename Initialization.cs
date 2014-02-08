using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using UnityEditor;

public class Initialization : MonoBehaviour {
	
	//Local variables
	private WallCreatorWithObstacles wallCreator;
	private Creation_Of_Positions_Array positionDataCreator;
	private Movement_Related_Functions movementClass;
	private Camera_Related_Functions cameraClass;
	bool pedestrians;
	string geometryFilename;
	string positionsFilename;
	float heightWanted;
	float distanceWanted;
	public double[,] positions;
	int numberOfPedestrians;
	int numberOfSteps;
	List<Transform> spheres;
	
	public Initialization(){}

	public void instantiateGeometry (GameObject Floor, GameObject ExtWall, GameObject IntWall, GameObject Obst)
	{
		// Will Instantiate the walls provided within an XMLfile
		if (EditorUtility.DisplayDialog ("Please select xml-file with geometry.", "", "OK", "Cancel")) {
			geometryFilename = EditorUtility.OpenFilePanel ("Open geometry", "D:\\Desktop", "xml");
		}
		wallCreator = new WallCreatorWithObstacles (Floor, ExtWall, IntWall, Obst, geometryFilename);
		wallCreator.makeWalls ();
	}
	
	public bool selectionSetup()
	{
		/* Will provide the program with a matrix of doubles used to store the positions of each and every
		 * game object extracted from an XML file. Also determines the number of steps and pedestrians involved. 
		 */
		if (EditorUtility.DisplayDialog ("Please select xml-file with pedestrian information.", "", "OK", "Cancel")) 
		{
			positionsFilename = EditorUtility.OpenFilePanel ("Open positions", "D:\\Desktop", "xml");
		}

		pedestrians = EditorUtility.DisplayDialog ("Please choose puppets or spheres.", "", "Puppets", "Spheres");
		return pedestrians;
	}

	public double[,] instantiatePositionsData()
	{
		positionDataCreator = new Creation_Of_Positions_Array (positionsFilename);
		positions = positionDataCreator.createPositionsArray ();
		numberOfPedestrians = (positions.GetLength (0)) / 2;
		numberOfSteps = positions.GetLength (1);
		return positions;
	}

		public List<Transform> instantiatePedestrians (GameObject prefabBlue, GameObject prefabRed, float rotationSpeed)
		{
		/* Will provide a list of all the gameobjects needed and will instantiate these within the game at their
		 * correct starting position.
		 */
		movementClass = new Movement_Related_Functions (numberOfPedestrians, numberOfSteps, positions, 
		                                                prefabBlue, prefabRed, rotationSpeed, pedestrians);
		spheres = movementClass.instantiateGameObjects ();
		return spheres;
		}
}
