using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Bullet : MonoBehaviour {
	
	
	public int damage = -20;
	public float timer = 1f;
	
	void Start(){
		
		Physics.IgnoreLayerCollision(10,10,true);
		Destroy(gameObject, timer);
	}
	
	void OnCollisionEnter ( Collision collision){
		
		CharacterEntity character = (CharacterEntity) collision.gameObject.GetComponent<CharacterEntity> ( );
		if( character != null){
			character.healthCheck(damage);
		}
		
		
		
		Destroy(gameObject);
		
	}
	
	
}
