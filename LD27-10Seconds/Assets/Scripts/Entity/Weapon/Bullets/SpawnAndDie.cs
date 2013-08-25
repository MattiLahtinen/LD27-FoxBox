using UnityEngine;
using System.Collections;

public class SpawnAndDie : MonoBehaviour {
	public float lifeTime =5;
	
	// Update is called once per frame
	void Start(){
		Destroy (gameObject,lifeTime);	
	}
	
}
