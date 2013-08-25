using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(PlayerEntity))]
public class PlayerGUI : MonoBehaviour {
	public float gameTimer = 10;
	private float multiplier = 1f;
	private int score = 0;
	public bool startTimer =false;
	private PlayerEntity player;
	
	private List<SpawnPoint> spawns;
	public AudioClip pickup;
	
	
	public void Start(){
		
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Spawn");
		
		spawns = new List<SpawnPoint>();
		foreach( GameObject point in temp ){
			spawns.Add(  (SpawnPoint)point.GetComponent<SpawnPoint>());
		}
		player = (PlayerEntity)GetComponent<PlayerEntity>();		
	}
	
	public void OnGUI(){
		
		if(player.healthPercent() <= 0){
				GUI.Box ( new Rect(Screen.width /2 - 50, Screen.height /2 - 25, 100, 50 ), " YOU ARE \n DEAD \n Score: " + score.ToString ());
				GUI.TextArea( new Rect( Screen.width /2 +50, Screen.height /2 + 75, 200, 50), " Press Space or Gamepad\n Button 0 to restart");
				if( (Input.GetKey(KeyCode.Space) || Input.GetKey ( KeyCode.Joystick1Button0 ))){
					Application.LoadLevel(Application.loadedLevel);
				}
		}else{
			if(startTimer){
				
				GUI.Box ( new Rect(Screen.width /2 - 100,0,200,50), " Multiplier: " + multiplier  + " \n Next Wave: " + gameTimer.ToString () );	
			}else{
				GUI.Box ( new Rect(Screen.width /2 - 100,0,200,50), " Catch a White Cube \n to start the timer. ");
			}
			GUI.Box ( new Rect(Screen.width /2 - 100,Screen.height-50,200,50), (player.healthPercent()*100).ToString () + "%");
			GUI.Box ( new Rect(Screen.width - 250,Screen.height-50,250,50), " \n Score: " + score.ToString () );
			if(startTimer){
				gameTimer -= Time.deltaTime;
				if(gameTimer <= 0  ){
					//Spawn More 
					foreach ( SpawnPoint spawn in spawns){
						spawn.spawn (Mathf.RoundToInt(multiplier));	
					}
					
					multiplier+=0.1f;
					resetTimer ();
				}
			}
		}
		
	}
	
	void OnControllerColliderHit ( ControllerColliderHit hit){
		if( hit.collider.tag == "Score" ){
			if(pickup){
				AudioSource.PlayClipAtPoint(pickup, transform.position);
			}
			startTimer =true;
			player.healthCheck(1);
			score +=  Mathf.FloorToInt (10 * multiplier);
			Destroy(hit.gameObject);
		}
		
	}
	
	public void resetTimer(){
		gameTimer = 10;	
	}
	
}

