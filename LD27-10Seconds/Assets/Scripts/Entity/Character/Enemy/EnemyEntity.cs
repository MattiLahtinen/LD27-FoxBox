using UnityEngine;
using System.Collections;

public class EnemyEntity : CharacterEntity {
	

	private float rotationModifier = 1;
	
	public int sightRangeSqr = 15;
	public int fireRangeSqr = 100;
	public int meleeRangeSqr = 6;
	
	public float visibleTurnModifier = 2;
	public float trackingTurnModifier = 10;
	public float cosAngle = 0.6f; // of 180
	public float fireAngle = 0.98f; // of cozAngle
	public float aheadAim = 1f;
	public float forgetRate = 3; //Seconds
	
	
	private float forgetTimer = 0;
		
	private PlayerEntity player;
	private bool forgotPlayer = false;
	private RangeWeaponController rangedWeapon; 
	private MeleeWeaponController meleeWeapon; 
	
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
		
		
		rangedWeapon = GetComponentInChildren<RangeWeaponController>();
		if(rangedWeapon != null){
			
			Debug.Log ("Found Ranged Weapon");
		}
		meleeWeapon = GetComponentInChildren<MeleeWeaponController>();
		if(meleeWeapon != null){
			
			Debug.Log ("Found Melee Weapon");
		}
		
		if(aheadAim > 1f){
			aheadAim = 1f;	
		}else if(aheadAim < 0){
			aheadAim = 0;	
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
			if(((visibilityStatus & LineOfSight.LookingAt) == LineOfSight.LookingAt && player.visible) ||
				((visibilityStatus & LineOfSight.Visible) == LineOfSight.Visible) ){
				
				aiControl = false;
				Vector3 currentPosition = GetPositionOnLocalAxis(player.transform.position);
				lastKnownHeading = (currentPosition-lastKnownPosition)*(aheadAim-1) + (player.getCharacterVelocity())*aheadAim;
				
				// Target abit ahead to where the player is moving to.
				Quaternion requiredRotation = Quaternion.LookRotation( currentPosition + lastKnownHeading - transform.position);
				
				
				setFrameRotation(requiredRotation, trackingTurnModifier);
				//transform.rotation = Quaternion.Lerp ( transform.rotation, requiredRotation, Time.deltaTime* trackingTurnModifier);
			
				lastKnownPosition = GetPositionOnLocalAxis(player.transform.position);
				
				if(((visibilityStatus & LineOfSight.Melee) != LineOfSight.Melee)){ // In Zone.
					currentVelocity = new Vector3(0,0,moveSpeed);
					// LookingAt, Enemy Is VIsible. Are we in Range tho?
					
					
					if((LineOfSight.InRange & visibilityStatus) == LineOfSight.InRange){
						currentVelocity = Vector3.zero;
						
						if(rangedWeapon != null && player.healthCheck()){
							Vector3 difference = (rangedWeapon.transform.position - (currentPosition+lastKnownHeading));
							
							//Quaternion weaponRotation = Quaternion.LookRotation (  difference );
							//rangedWeapon.transform.rotation =  Quaternion.Lerp (rangedWeapon.transform.rotation, weaponRotation, Time.deltaTime);
							
							rangedWeapon.Attack();	
							
						}	
					}
					
				}else{
					// We are at Melee Range
					
					if(meleeWeapon != null && player.healthCheck()){
						meleeWeapon.Attack();	
						
					}
				}
				
				
				forgetTimer = forgetRate;
				
			}else if(lastKnownPosition != Vector3.zero && forgetTimer > 0){
				Quaternion requiredRotation;			
				
				aiControl = false; // Patrolling.
				Vector3 direction = lastKnownPosition - transform.position;
				
				if( direction.sqrMagnitude>2  ){ 
					requiredRotation = Quaternion.LookRotation( lastKnownPosition  + lastKnownHeading - transform.position);
					Debug.DrawLine ( transform.position, lastKnownPosition+ lastKnownHeading, Color.blue*0.5f);
					currentVelocity = new Vector3(0,0,moveSpeed);
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
					
					if(cos > fireAngle){
						forgotPlayer  = false;
						player.visible |= true;
						// In Fireing Angle. FIREEEEEEE
						rotationModifier = rotationSpeed;
						
						visibility |= LineOfSight.Targeted; 	//  in Sights.
						if((direction.sqrMagnitude<=fireRangeSqr) && (direction.sqrMagnitude>meleeRangeSqr)){
							Debug.DrawLine ( (headOffset+transform.position), hitTarget.point, new Color(1f,0.5f,0f,1f));	
							visibility |= LineOfSight.InRange;	 // are in Range
						}else if(direction.sqrMagnitude<=meleeRangeSqr){
							Debug.DrawLine ( (headOffset+transform.position), hitTarget.point, new Color(1f,0f,0f,1f));		
							visibility |= LineOfSight.Melee;	 // are in Range for Meelee
						}else{
							Debug.DrawLine ( transform.position, hitTarget.point, new Color(1f,0f,0f,0.25f));
						}
						
						
					}else{
						// Lets try turning faster.
						rotationModifier = rotationSpeed * visibleTurnModifier;
						Debug.DrawLine ( (headOffset+transform.position), hitTarget.point, Color.red*0.5f);	
						visibility |= LineOfSight.Visible;
					}
				}else{
					// Seeing but something else is infront.
					
					Debug.DrawLine ( (headOffset+transform.position), hitTarget.point, Color.magenta);	
					if(player.visible && !forgotPlayer){
						player.visible = false;
						forgotPlayer = true;
					}
					visibility = LineOfSight.LookingAt;
				}
			}
		}else {
			// Player is not visible, but perhaps is meelee range?	
			if(direction.sqrMagnitude<=meleeRangeSqr){
				// Player is potentially in Meelee Range.
				Debug.DrawLine ( (headOffset+transform.position), objectPosition, new Color(1f,0.5f,1f,0.8f));		
				visibility |= LineOfSight.Melee;	 // are in Range for Meelee
				
				
			}
			if(player.visible && !forgotPlayer){
				player.visible = false;
				forgotPlayer = true;
			}
		}
		
		return visibility; 
	}	
}


public enum LineOfSight :int{
	NotVisible = 0,
	Visible = 1,
	LookingAt = 2,
	Targeted = 3,
	InRange = 4,
	Fireing = 7,
	Melee = 8
}
