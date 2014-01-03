using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class Creation_Of_Positions_Array : MonoBehaviour {
	
	public int numberOfSteps;
	public int numberOfPedestrians;
	string positionsFilename;
	
	public Creation_Of_Positions_Array(string positionsFilename)
	{
		this.positionsFilename = positionsFilename;	
	}

	
	// retrieveArraySize is a function that allows us to extract the size of our data array in matlab
	// so that we will be able to prepare an array of the same proportions to save the actual data into. 
	public List<string> retrieveArraySize ()
	{
		// Define doc as an XML file. 
		XmlDocument doc = new XmlDocument ();
		doc.Load (positionsFilename);		
		
		//Define NOP and NOI as Nodes of an XML file.
		XmlNode NOP;
		XmlNode NOI;

		// Define root as the top node of the xml (e.g. Simulation_Data).
		XmlElement root = doc.DocumentElement;
		
		// Assign the Nodes NOP and NOI from the XML file to the NOP 
		// and NOI variables of our program.
		NOP = root.SelectSingleNode ("General_Data/NOP");
		NOI = root.SelectSingleNode ("General_Data/NOI");

		// Assign the values of the nodes NOP and NOI in the xml files to the 
		// variables NumberofPeds and NumberofIts. 
		string NumberPeds = NOP.Attributes ["Number_of_Pedestrians"].Value;
		string NumberIts = NOI.Attributes ["Number_of_Iterations"].Value;	
		
		// Makes a new array in which we can save the data we extracted from the XML file (The size of our Data Array).
		List<string> dataArray = new List<string> ();
		dataArray.Add (NumberPeds);
		dataArray.Add (NumberIts); 
		
		// Return of our function retrieveArraySize(). 
		return dataArray; 
		
	}
	
	public double[,] createPositionsArray ()
	{
		// Here we initailize a new array which we will be using to store the output of our function retrieveArraySize.
		List<string> dataArray = new List<string> ();
		dataArray = retrieveArraySize ();
		
		// Here we create the Array in which we will save all the XML file data for the game object paths. It is being 
		// made to be the same size as the array in our matlab file using the data provided by the XML and retrieved by
		// the function retrieveArraySize. 
		int numberOfPedestrians = Convert.ToInt32 (dataArray [0]);		
		int numberOfSteps = Convert.ToInt32 (dataArray [1]);
		double[,] positionData = new double[(numberOfPedestrians * 2), numberOfSteps];
		
		// Define doc as an XML file. 
		XmlDocument doc = new XmlDocument ();
		doc.Load (positionsFilename);
		
		//Make a Node variable
		XmlNodeList pedestrians;
		XmlElement root = doc.DocumentElement;
		
		pedestrians = root.SelectNodes ("//Pedestrian_Position");
		//int numofpeds = pedestrians.Count;
		
		int pedCount = 0;
		int stepCount = 0; // Will count number of steps performed by each pedestrian so far.
		while (pedCount < numberOfPedestrians) {
			while ((stepCount < ((pedCount+1)*numberOfSteps)) && (stepCount >= ((pedCount)*numberOfSteps))) {
				string id = pedestrians [stepCount].Attributes ["id"].Value;
				string xpos = pedestrians [stepCount].Attributes ["X"].Value;
				string ypos = pedestrians [stepCount].Attributes ["Y"].Value;
				
				double xPos = Convert.ToDouble (xpos);
				double yPos = Convert.ToDouble (ypos);
				
				positionData [pedCount, (stepCount - (numberOfSteps * pedCount))] = xPos;
				positionData [(pedCount + numberOfPedestrians), (stepCount - (numberOfSteps * pedCount))] = yPos;
				
				stepCount = stepCount + 1;
			}
			pedCount = pedCount + 1;
		}

		return positionData;
	}
	
}
