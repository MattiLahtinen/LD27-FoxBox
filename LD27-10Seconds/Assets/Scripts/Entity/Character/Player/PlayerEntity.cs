using UnityEngine;
using System.Collections;


public class PlayerEntity : CharacterEntity {

	
	public bool visible = false;
	public Camera gameCamera;
	public Vector3 cameraOffset = new Vector3(0f,10f,0f);
	
	
	private float jumping = 0f;
	
		// Use this for initialization
	public override void Start () {
		
		base.Start ();
		
		if(!gameCamera){
			Debug.Log (" Camera Undefined " );	
		}else{
			gameCamera.transform.position = this.transform.position + cameraOffset;
				
		}

		
		
		currentVelocity = new Vector3();
	}
	
	
	// Update is called once per frame
	public override void Update () {
		
		
		if(healthCheck ()){	
			
			if((Input.GetKey(KeyCode.Space) || Input.GetKey ( KeyCode.Joystick1Button0 )) && !gravityOverride){
				Debug.Log ( " Jumping " );
				gravityOverride = true;
				jumping = 0.5f; 
				
			}
			currentVelocity = new Vector3(Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			

		
			
			Vector3 goingTowards = (this.transform.position + (currentVelocity*moveSpeed));
		
			if(jumping > -0.5f && gravityOverride && !characterController.isGrounded){
				 if(  currentVelocity.magnitude <= 0.125f){
					Vector3 forward =  transform.forward;
					currentVelocity.z = 0.75f * forward.z;
					currentVelocity.x = 0.75f * forward.x;		
				}
				currentVelocity.y  = currentVelocity.y + (jumping-=Time.deltaTime);
				goingTowards.y = currentVelocity.y;
			}else {
				gravityOverride = false;
				
				jumping =0;
			}	
			currentVelocity.Normalize();
			
			
			Debug.DrawLine( this.transform.position, goingTowards);		
			if(currentVelocity.magnitude >= 0.5f){
				setFrameRotation(Quaternion.LookRotation(currentVelocity*moveSpeed),rotationSpeed);
			}
			//transform.LookAt(goingTowards);
			
		}
		
		base.Update ();
		if(gameCamera){
				
			gameCamera.transform.position = this.transform.position + cameraOffset;
			//gameCameraController.SimpleMove( goingTowards*Time.deltaTime );
			//gameCamera.transform.
			//gameCamera.transform.position = this.transform.position + cameraOffset;	
		}
		if(!healthCheck()){
			this.enabled = false;
		}
		
		
	}
	
	
	
}
