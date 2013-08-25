using UnityEngine;
using System.Collections;

public class MeleeWeaponController : MonoBehaviour, WeaponController {
	public int damage = 33;
	public bool playerOnly = false;
	public float meleeRate = 3f;
	public float meleeRange = 3f;
	public float meleeCone = 1f;
	
	private float fireTimer = 0f;
	
	private Camera controlledCamera= null;
	private PlayerEntity player;
	
	public void Start(){
		if(playerOnly){
			player = GameObject.Find("Player").GetComponent<PlayerEntity>();
		}
	}
	
	
	
	
	public void Attack () {
		if(fireTimer <= 0){
			fireTimer = meleeRate;
			if(playerOnly && player){
				player.healthCheck( -damage) ; 
			}else{
				GameObject[] DamageAbleEntities = GameObject.FindGameObjectsWithTag("DamageAbleEntity");	
				// May be extend this?
			}
			
			
		}
	}
	public void Update () {
		if(fireTimer >=0){
			fireTimer-=Time.deltaTime;
		}
		if(controlledCamera != null){
			gameObject.transform.rotation = controlledCamera.transform.rotation;	
			
		}
	}
	public void passCamera(ref Camera passedCamera){
		controlledCamera = passedCamera;
	}
}
