using UnityEngine;
using System.Collections;

public class NpcEntity : CharacterEntity {
	

	private float rotationModifier = 1;
	
	public int sightRangeSqr = 15;

	
	public float visibleTurnModifier = 2;
	public float trackingTurnModifier = 10;
	public float cosAngle = 0.6f; // of 180

	public float forgetRate = 3; //Seconds
	
	
	private float forgetTimer = 0;
		
	private PlayerEntity player;

	private Vector3 lastKnownPosition = Vector3.zero;
	private Vector3 lastKnownHeading = Vector3.zero;

	public override void Start () {
		base.Start ();
		Physics.IgnoreLayerCollision(9,9,true);
		player = GameObject.Find("Player").GetComponent<PlayerEntity>();
		lastKnownPosition = Vector3.zero;

		if(player == null){
			Debug.LogError (" Player could not be found " );	
			enabled = false;
		}
		
	
	}
	
	 
	
	// Update is called once per frame
	public override void Update () {
		if(!player.healthCheck(0) && healthCheck()){
			aiControl = true;	
			lastKnownPosition = Vector3.zero;
		}else if(healthCheck() ){// A bit larger check. Probably need to move lookRotations to Character.
			LineOfSight visibilityStatus = Sight(player.transform.position);
		
			
			if(!aiControl){
				currentVelocity = Vector3.zero; //Let the navigation decide this.	
			}
			if(((visibilityStatus & LineOfSight.LookingAt) == LineOfSight.LookingAt) ||
				((visibilityStatus & LineOfSight.Visible) == LineOfSight.Visible) ){
				
				aiControl = false;
			
				
				Vector3 currentPosition = GetPositionOnLocalAxis(player.transform.position);
				lastKnownHeading = (currentPosition-lastKnownPosition);
				
				// Target abit ahead to where the player is moving to.
				Quaternion requiredRotation = Quaternion.LookRotation( currentPosition + lastKnownHeading - transform.position);
				
				
				setFrameRotation(requiredRotation, trackingTurnModifier);
				//transform.rotation = Quaternion.Lerp ( transform.rotation, requiredRotation, Time.deltaTime* trackingTurnModifier);
			
				lastKnownPosition = GetPositionOnLocalAxis(player.transform.position);
					currentVelocity = new Vector3(0,0,-moveSpeed);
				
				
				
				forgetTimer = forgetRate;
				
			}else if(lastKnownPosition != Vector3.zero && forgetTimer > 0){
				Quaternion requiredRotation;			
				aiControl = false; // Patrolling.
				Vector3 direction = lastKnownPosition - transform.position;
				if( direction.sqrMagnitude>2  ){ // Lets look Back.
					requiredRotation = Quaternion.LookRotation( lastKnownPosition  + lastKnownHeading - transform.position);
					Debug.DrawLine ( transform.position, lastKnownPosition+ lastKnownHeading, Color.blue*0.5f);
					currentVelocity = new Vector3(0,0,-moveSpeed/2); // Backpaddling.
					rotationModifier = rotationSpeed;
					
				}else{// If we close, we shouldnt chase in circles but, instead stop, Look at direction of momentum.
					Vector3 guestimate = lastKnownPosition + ((lastKnownHeading/Time.deltaTime));
					Debug.DrawLine ( transform.position, guestimate, Color.yellow);
					
					requiredRotation = Quaternion.LookRotation( guestimate - transform.position);	
					
					//currentVelocity = new Vector3(0,0,(moveSpeed*0.8f));
					
					rotationModifier = rotationSpeed * 5;
				}
				
				setFrameRotation(requiredRotation, rotationModifier);
				//transform.rotation = Quaternion.Lerp ( transform.rotation, requiredRotation, Time.deltaTime* rotationModifier);
				
				
			}else{
				aiControl = true; // Allow Navigation to control again.
				lastKnownPosition = Vector3.zero;
				
				
			}
			
			if(forgetTimer >= 0){
				currentVelocity =  transform.TransformDirection(currentVelocity); 
				forgetTimer-=Time.deltaTime;
			}
			
			
		}
		base.Update();
	}
	
	private Vector3 GetPositionOnLocalAxis(Vector3 position){
		return new Vector3( position.x, this.transform.position.y, position.z);
	}
	
	private LineOfSight Sight(Vector3 objectPosition){
		// TODO: Maybe disconnect this, and create an Enemy Controller that handles sighting and actions?
		Vector3 direction = objectPosition - (transform.position+headOffset);
				
		// Is player inobstructed? Advanced Line of Sight by Ray comparison.
		
		float cos = Vector3.Dot( direction.normalized, transform.forward );
		
		bool inSightCone = direction.sqrMagnitude<=sightRangeSqr && cos > cosAngle;
		LineOfSight visibility = 0;
		
		if(inSightCone){
			// We "See the player" but is there anything infront of the player?
			RaycastHit hitTarget = new RaycastHit();
			if(Physics.Raycast( (transform.position+headOffset), direction, out hitTarget, Mathf.Sqrt(sightRangeSqr) ) ){
				if(hitTarget.collider.name.Equals("Player")){
					
					// First object collided with is Player! We have line of sight!
					
					
					// Oh we can see the player, RUN AWAAAAY
					rotationModifier = rotationSpeed * visibleTurnModifier;
					Debug.DrawLine ( (headOffset+transform.position), hitTarget.point, Color.red*0.5f);	
					visibility |= LineOfSight.Visible;
				
				}else{
					// Seeing but something else is infront.
					
					Debug.DrawLine ( (headOffset+transform.position), hitTarget.point, Color.magenta);	
					
					visibility = LineOfSight.LookingAt;
				}
			}
		}
		
		return visibility; 
	}	
}


