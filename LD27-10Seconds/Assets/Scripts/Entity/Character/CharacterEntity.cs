using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(AudioSource))]

public class CharacterEntity : MonoBehaviour, DamageAbleEntity {
	// Handles All Character Logics
	public float moveSpeed = 1.0f;
	public float patrolSpeedPercentage = 1.0f;
	public float rotationSpeed = 1.0f;
	public int maxHealth = 100;
	public Vector3 headOffset = Vector3.zero;
	private int currentHealth;
	protected bool gravityOverride = false;
	
	public AudioClip hit;
	public AudioClip death;
	
	protected Vector3 currentEulerRotation;
	protected Vector3 gravity = Vector3.zero;
	protected CharacterController characterController;
	protected Vector3 currentVelocity = Vector3.zero; 
	
	public float destroyTime = 5f;
	
	
	protected bool aiControl = false; // Is the character being controller by the AIController? By Default No.
	

	
	// Use this for initialization
	public virtual void Start () {
		
		characterController = GetComponent<CharacterController>();
		if(!characterController){
			Debug.LogError ("Character does not have a CharacterController");	
			enabled = false;
		}
				
		
		currentHealth = maxHealth;
	}
	
	
	public bool aiControllable(){
		return aiControl;	
	}

	public CharacterController getController(){
		return characterController;	
	}
	public void setCharacterVelocity(Vector3 velocity){
		currentVelocity = velocity;	
	}
	public Vector3 getCharacterVelocity (){
		return currentVelocity;	
	}
	public virtual bool healthCheck(int modifier=0){ 
		if(modifier!=0){ // Can be postive for healing so check against 0 only.
			currentHealth +=modifier;
			if(hit && modifier < 0){
				
				AudioSource.PlayClipAtPoint(hit, transform.position);	
			}
		}
		if(death && (currentHealth <= 0)){
			
			AudioSource.PlayClipAtPoint(death, transform.position);
			death = null;
		}
		if((currentHealth <= 0)){
//			this.renderer.material.SetColor("_Color", this.renderer.material.GetColor ("_Color") * 0.5f);	
		}
		return (currentHealth > 0); 
	}
	
	public virtual float healthPercent(){
		if(currentHealth <= 0){
			return 0;	
		}
		return (float)(currentHealth)/(float)(maxHealth);	
	}
	
	public void setFrameRotation(Quaternion requiredRotation, float modifier=2f){
			
			transform.rotation = Quaternion.Lerp ( transform.rotation, requiredRotation, Time.deltaTime* modifier);
		
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if(healthCheck ()){
			if(!gravityOverride){
				if(!characterController.isGrounded){
					gravity += Physics.gravity;	
				}else{
					gravity = Vector3.zero;
				}
			}
			
			if(currentVelocity != Vector3.zero){
				currentVelocity = currentVelocity * moveSpeed;
				characterController.Move ((currentVelocity + gravity) * Time.deltaTime ) ;
			}else{
				characterController.Move (gravity * Time.deltaTime ) ;
			}
			
		
			
			gameObject.transform.Rotate (currentEulerRotation);
		}else{
			
			if(destroyTime > 0){
			Destroy (this.gameObject,destroyTime);
			}
		}
		
	}
	
}


 