using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

public class WallCreatorWithObstacles : MonoBehaviour 
{
    // This class has some nested classes which are used to deserialize the xml file containing geometry information.

    // Nested class where walls are stored
	public class Wall
	{
		public Vector2 Start {get; set; }
		public Vector2 End {get; set; }
	}
	
    // nested class where obstacles are stored
	public class Obstacle
	{
		public Vector2 Centre {get; set; }
		public float Diameter {get; set; }
	}
	
    // nested class where geometry is stored. The geometry consists of an array with exterior walls
    // an array with interior walls, and an array with obstacles.
	public class Geometry
	{
		public Wall[] ExteriorWalls {get; set; }
		public Wall[] InteriorWalls {get; set; }
		public Obstacle[] Obstacles {get; set; }
	}
	
	// These game objects are prefabs used to instantiate the geometry
	public GameObject Floor;
	public GameObject ExtWall;
	public GameObject IntWall;
	public GameObject Obst;

	public string XMLlocation; // the location of the xml file containing geometry information
	private GameObject FloorClone; // the actual floor
	private GameObject[] ExtWallArray; // the actual exterior walls
	private GameObject[] IntWallArray; //  the actual interior walls
	private GameObject[] ObstArray; // the actual obstacles
	private Transform tf; // used to transform the walls and obstacles.
	
    // constructor. Sets references to the floor prefab, wall prefabs, obstacle prefabs, and xml location.
	public WallCreatorWithObstacles(GameObject Floor, GameObject ExtWall, GameObject IntWall, GameObject Obst, string XMLlocation)
	{
		this.Floor = Floor;
		this.ExtWall = ExtWall;
		this.IntWall = IntWall;
		this.Obst = Obst;	
		this.XMLlocation = XMLlocation;
	}
	
	// this function actually creates the geometry.
	public void makeWalls() 
	{
		//Read the xml file with geometry information
		Geometry geometry;
		XmlSerializer serializer = new XmlSerializer(typeof(Geometry));
		StreamReader reader = new StreamReader(@XMLlocation);
		geometry = (Geometry)serializer.Deserialize(reader); // store the information in geometry
		reader.Close ();
		
		// instantiate the floor
        // first determine the coordinates of the floor. This is done by looking at the cooridnates of all exterior walls.
		float minX = geometry.ExteriorWalls[0].Start.x;
		float minY = geometry.ExteriorWalls[0].Start.y;
		float maxX = geometry.ExteriorWalls[0].Start.x;
		float maxY = geometry.ExteriorWalls[0].Start.y;
		
        // finding the minimal and maximal coordinates
		for(int i = 0; i < geometry.ExteriorWalls.Length; i++)
		{
			minX = Mathf.Min(minX, geometry.ExteriorWalls[i].Start.x);
			minX = Mathf.Min(minX, geometry.ExteriorWalls[i].End.x);
			minY = Mathf.Min(minY, geometry.ExteriorWalls[i].Start.y);
			minY = Mathf.Min(minY, geometry.ExteriorWalls[i].End.y);
			
			maxX = Mathf.Max(maxX, geometry.ExteriorWalls[i].Start.x);
			maxX = Mathf.Max(maxX, geometry.ExteriorWalls[i].End.x);
			maxY = Mathf.Max(maxY, geometry.ExteriorWalls[i].Start.y);
			maxY = Mathf.Max(maxY, geometry.ExteriorWalls[i].End.y);
		}
		Instantiate (Floor); // instantiate the floor
		FloorClone = GameObject.FindGameObjectWithTag("Floor");
		tf = FloorClone.GetComponent<Transform>(); // transform the floor into the right position, rotation, and scale
		tf.position = new Vector3((minX + maxX) / 2, 0, (minY + maxY) / 2);
		tf.localScale = new Vector3((maxX - minX) / 10, tf.localScale.y, (maxY - minY) / 10);
		
		// for each element in ExteriorWalls, instantiate a clone of prefab "ExteriorWallPrefab"
		for(int i=0; i<geometry.ExteriorWalls.Length; i++)
		{
			Instantiate(ExtWall);
		}
		
		// for each element in InteriorWalls, instantiate a clone of prefab "InteriorWallPrefab"
		for(int i=0; i<geometry.InteriorWalls.Length; i++)
		{
			Instantiate(IntWall);
		}
		
		// for each element in Obstacles, instantiate a clone of prefab "ObstaclePrefab"
		for(int i=0; i<geometry.Obstacles.Length; i++)
		{
			Instantiate(Obst);
		}
		
		// Store all exterior walls in array
		ExtWallArray = GameObject.FindGameObjectsWithTag("ExteriorWall");
		
		// adapt the position, rotation and scale of the exterior walls
		for(int i=0; i<ExtWallArray.Length; i++)
		{
			tf = ExtWallArray[i].GetComponent<Transform>();
			//adapt position
			tf.position = new Vector3 ((geometry.ExteriorWalls[i].Start.x + geometry.ExteriorWalls[i].End.x)/2, 
				tf.position.y, (geometry.ExteriorWalls[i].Start.y + geometry.ExteriorWalls[i].End.y)/2);
			//adapt rotation
			tf.Rotate (new Vector3(0, 0, Mathf.Sign(geometry.ExteriorWalls[i].Start.x - geometry.ExteriorWalls[i].End.x) 
				* Vector2.Angle (new Vector2 (0,1), geometry.ExteriorWalls[i].End - geometry.ExteriorWalls[i].Start)));
			//adapt scale
			tf.localScale = new Vector3 ((geometry.ExteriorWalls[i].End - geometry.ExteriorWalls[i].Start).magnitude/10, 
				tf.localScale.y, tf.localScale.z);
		}
		
		// store all interior walls in array
		IntWallArray = GameObject.FindGameObjectsWithTag("InteriorWall");
		
		// adapt the position, rotation and scale of interior walls
		for(int i=0; i<IntWallArray.Length; i++)
		{
			tf = IntWallArray[i].GetComponent<Transform>();
			//adapt position
			tf.position = new Vector3 ((geometry.InteriorWalls[i].Start.x + geometry.InteriorWalls[i].End.x)/2, 
				tf.position.y, (geometry.InteriorWalls[i].Start.y + geometry.InteriorWalls[i].End.y)/2);
			//adapt rotation
			tf.Rotate (new Vector3(0, 0, Mathf.Sign(geometry.InteriorWalls[i].Start.x - geometry.InteriorWalls[i].End.x) 
				* Vector2.Angle (new Vector2 (0,1), geometry.InteriorWalls[i].End - geometry.InteriorWalls[i].Start)));
			//adapt scale
			tf.localScale = new Vector3 ((geometry.InteriorWalls[i].End - geometry.InteriorWalls[i].Start).magnitude/10, 
				tf.localScale.y, tf.localScale.z);
		}
		
		// store all obstacles in array
		ObstArray = GameObject.FindGameObjectsWithTag("Obstacle");
		
		// adapt the position and the scale of the obstacles
		for(int i=0; i<ObstArray.Length; i++)
		{
			tf = ObstArray[i].GetComponent<Transform>();
			//adapt position
			tf.position = new Vector3 (geometry.Obstacles[i].Centre.x, tf.position.y, geometry.Obstacles[i].Centre.y);
			//adapt scale
			tf.localScale = new Vector3 (geometry.Obstacles[i].Diameter, tf.localScale.y, geometry.Obstacles[i].Diameter);
		}
	}
}