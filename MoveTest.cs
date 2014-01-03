using UnityEngine;
using System;
using System.Collections;
	
public class MoveTest : MonoBehaviour 
{

	//initialization of animationclips
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	
	public float WalkMaxAnimationSpeed = 10.0f;
	public float influenceVelocityOnSpeed = 1.0f;
		
	Vector3 previousPosition;
	
public void Start(){
	//make all animations loop
	animation.wrapMode = WrapMode.Loop;
}

public void Update(){
	//Get the difference/distance between the previous position and the current position
    float travelledDist = Vector3.Distance(previousPosition, transform.position);
		
	//Calculate Velocity
	float velocity = travelledDist / Time.deltaTime;
		
		previousPosition = transform.position;
		
		
	if(velocity > 0.01f)
		{
			animation.CrossFade(walkAnimation.name);
			animation[walkAnimation.name].speed = velocity*influenceVelocityOnSpeed;
//			animation[walkAnimation.name].speed = Mathf.Clamp(velocity*influenceVelocityOnSpeed, WalkMaxAnimationSpeed, 0.0f);
		}
		else 
			{
			animation.CrossFade(idleAnimation.name);
			}
		
	//update location of previous position
	//previousPosition = transform.position;
		// Debug.Log (velocity);
	}
}

