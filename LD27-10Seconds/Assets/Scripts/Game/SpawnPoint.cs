using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using customGameVariables;


public class SpawnPoint: MonoBehaviour 
{
	public List<SpawnProbability> possibleSpawns = new List<SpawnProbability>();
	
	public Color color = Color.red;
	public float range = 3f;

	public void spawn(int multiplier = 1) {
	
	
				
	
		for( int i = 0; i < multiplier; i ++){
			float random =  Random.Range (0f, 1f);		
			foreach ( SpawnProbability spawn in possibleSpawns){
				
				if(spawn.startProbability <= random && random <= spawn.endProbability){
					Debug.Log ( "Spawning"  );
					
					int randomRange = Mathf.FloorToInt( Random.Range ( spawn.startProbability, spawn.endProbability) );
					for( int e =0; e <= randomRange; e++){
						Vector3 offset = new Vector3( Random.Range(-1f,1f),0, Random.Range(-1f,1f)) * range;
						GameObject obj = GameObject.Instantiate( spawn.npc, this.transform.position + offset,new Quaternion(0,0,0,1)) as GameObject;
					}
				}
			}
		}
	}
}

