using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class RangeWeaponController :MonoBehaviour, WeaponController
{
	public float fireRate = 5; // Bullets every X seconds 
	public int bulletCount = 1;
	
	public float bulletVelocity = 20f;
	public float firingCone = 5; 
	private float fireTimer = 0;
	public bool constantThrust = false; // Is the ammo a rocket or something of the type?
	private bool reloaded = true;
	private Camera controlledCamera= null;
	
	
	public AudioClip shoot;
	public AudioClip reload;
	
	public GameObject bullet;
	
	private Transform bulletSpawner;
	// Use this for initialization
	void Start () {
		
		if( bullet == null){
			Debug.LogError(" Could not find bullet type");
			enabled = false;
		}
		bulletSpawner = transform.FindChild ("bulletSpawn");
		if(bulletSpawner == null){
			Debug.LogError(" Could not find spawner");
			enabled = false;
		}
	
	}
	

	
	public void Attack(){
		if(fireTimer <= 0){
			
			
			fireTimer = fireRate;
			if(gameObject != null){
				
				/*
				 *  Old script which I had done in Second Life to calculate spread. Applicable in Unity too!
				 *  calculatedDeviation = < deviationMultiplier*( llFrand(deviationTimer) - (deviationTimer)/2.0),
                                deviationMultiplier*( llFrand(deviationTimer) - (deviationTimer)/2.0),
                                deviationMultiplier*( llFrand(deviationTimer) - (deviationTimer)/2.0)>;
    				referenceRotation = referenceRotation * llEuler2Rot(calculatedDeviation);
				 * 
				 * 
				 */ 
				for(int count =0; count< bulletCount; count++){
					Quaternion randomRotation = new Quaternion( 0,0,0,1);
					if(firingCone > 0){
						randomRotation = Quaternion.Euler(firingCone*Random.Range (-1.0f,1.0f), firingCone*Random.Range (-0.50f,0.50f), firingCone*Random.Range (-1.0f,1.0f));
					}
					
					GameObject spawnedBullet = GameObject.Instantiate(bullet,  bulletSpawner.position , bulletSpawner.rotation * randomRotation) as GameObject; 
					
					if(!constantThrust){
						spawnedBullet.rigidbody.AddRelativeForce(Vector3.forward.normalized * bulletVelocity, ForceMode.Impulse);
					}else{
						spawnedBullet.rigidbody.velocity = spawnedBullet.transform.TransformDirection(Vector3.forward * bulletVelocity);
					}
				}
				reloaded = false;
				if(shoot)
					AudioSource.PlayClipAtPoint(shoot, bulletSpawner.transform.position);
			}
			
			
		}
		
	}
	
	public void passCamera(ref Camera passedCamera){
		controlledCamera = passedCamera;
	}
	
	public void Update () {
		if(fireTimer >=0){
			fireTimer-=Time.deltaTime;
		}else if(reload && !reloaded){
			reloaded = true;
				AudioSource.PlayClipAtPoint(reload, this.transform.position);	
		}
		if(controlledCamera != null){
			gameObject.transform.rotation = controlledCamera.transform.rotation;	
			
		}
	}
}


