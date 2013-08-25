using UnityEngine;
using System.Collections;

// Script has to be interfacing with a Character and its Character Controller 
// (naturally, required by Character class). 
// Character is moved using the Character, 
// instead of Character controller, as Character handles health and speed, and gravity.
[RequireComponent (typeof(CharacterEntity))]
public class BasicAIEntity : MonoBehaviour {
	public int navigationGroup = 1;
	public float navigationDistanceTolerance = 0.5f;
	public float offset = 0.25f;
	
	
	private CharacterEntity character;
	private NavigationPoint previousTarget;
	private NavigationPoint navigationTarget;
	private bool aiLock = false;
	private float timer = 0f;
	
	void Start () {
		character = (CharacterEntity)GetComponent<CharacterEntity>();
		if(!character){
			Debug.LogWarning("No Character Available to control");
			enabled= false;
		}
	}
	
	private Vector3 NavigationNormalize(Vector3 diff ){
		diff.y = 0;
		return diff;
			
	}

	// Update is called once per frame
	void Update () {
		if(timer <= 0){
			if(character.aiControllable() && !aiLock){
				if(navigationTarget){	
					//enabled=  false;
					
					
					if(navigationTarget.getDistance(GetPositionOnLocalAxis(gameObject.transform.position  )) >= navigationDistanceTolerance){
						Quaternion requiredRotation = Quaternion.LookRotation( NavigationNormalize( GetPositionOnLocalAxis(navigationTarget.transform.position) - gameObject.transform.position));
						character.setFrameRotation(requiredRotation, character.rotationSpeed);
						character.setCharacterVelocity(requiredRotation* new Vector3(0f,0f,character.moveSpeed * character.patrolSpeedPercentage));
						
					}else if( navigationTarget.getDistance(GetPositionOnLocalAxis(gameObject.transform.position)) < navigationDistanceTolerance){
						
						timer = navigationTarget.pause;
					
						NavigationPoint temp = navigationTarget.getRandomPoint(navigationTarget,previousTarget); // Get next Navigation Point	
						previousTarget = navigationTarget;
						navigationTarget = temp;
					}
					
					Debug.DrawLine(
						this.gameObject.transform.position, 
						navigationTarget.transform.position, Color.white);
					
				}else if(!aiLock){
					if(!locateNearestNavigationPoint() ){
						Debug.Log ( gameObject.name + ": I'm lost. No nearby navigation points found in sight. Waiting until Enemy is seen");	
						aiLock = true;
					}else{
						//Debug.Log ( navigationTarget.name + " Found. Moving towards it in the next frame");	
					}
				}
			}else{
				
				navigationTarget = null; // Reset Target;	
				aiLock = false; // entity AI has kicked in, reticulate paths after we have control again.
				
			}
			
				
		}else{
				
		}
		if(timer > 0){
			// We shouldnt be moving if we have paused. Make sure of it.
			if(character.aiControllable() && !aiLock){
				character.setCharacterVelocity(Vector3.zero);	
			}
			timer -= Time.deltaTime;
		}
	}
	
	bool locateNearestNavigationPoint(){
	
		navigationTarget = null; // Nullify Find a new  point.
		float lastValue = 0f;
		GameObject[] others = GameObject.FindGameObjectsWithTag("Navigation");
		
		foreach(GameObject other in others){
			NavigationPoint point = (NavigationPoint)other.GetComponent<NavigationPoint>();
			if(point){ // Point is a Navigation tPoint.
				if(point.navigationGroupID == navigationGroup){
					if(navigationTarget){
						float newValue = point.getDistance( GetPositionOnLocalAxis(gameObject.transform.position) );
						if( lastValue > newValue && newValue >= 0){
							
							lastValue = newValue;	
							navigationTarget = point;
						}
						
					}else{
						lastValue = point.getDistance(gameObject.transform.position);
						if(lastValue >= 0){ // if < 0 There is no clear connection to the point, thus negative.
							navigationTarget = point;	
						}
					}

				}
				
			}
		}
			
		return (navigationTarget != null); 
			
	}
	
	
	private Vector3 GetPositionOnLocalAxis(Vector3 position){
		return new Vector3( position.x, gameObject.transform.position.y + offset, position.z);
	}
	
}
