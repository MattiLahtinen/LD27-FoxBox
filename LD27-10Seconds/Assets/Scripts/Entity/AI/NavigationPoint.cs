using UnityEngine;
using System.Collections;

public class NavigationPoint : MonoBehaviour {
	
	public float pause = 0f;
	public int navigationGroupID = 1; // 
	public float rangeSqr = 20f;
	private ArrayList connectedPoints;
	public Color color = Color.blue;
	
	private NavigationPoint thisInstance; 
	
	void Start () {
	
		refreshClosestPoints();
		
	}
	public void refreshClosestPoints(){
		
		GameObject[] others = GameObject.FindGameObjectsWithTag("Navigation");
		connectedPoints = new ArrayList();
		// getNearest. Calculate.
		if(others.Length > 0){
			foreach( GameObject other in others){
				if(other != gameObject){ // First make sure this is not this instance of the gameObject.	
					NavigationPoint point = (NavigationPoint) other.GetComponent<NavigationPoint>();
					if( checkConnection ( point )){
						
						connectedPoints.Add(point);	
					}
				}else{
				
				}
			}
			// Using ArrayLists until all Vectors have been found, This is then converted into an actual vector3 array.
			
			others = null; // Clear others. We dont need the full list anymore as full, 
			// and not used else where anymore. Let the garbage collector handle the leftover.
			
		}else{
			Destroy(gameObject); // this Waypoint has nothing connected to anything. Might as well not be in use.
		}
		
	
		
	}
	
	// Returns a random Point that isnt exclude
	public NavigationPoint getRandomPoint(NavigationPoint current, NavigationPoint exclude = null){
		bool selected = false;
		NavigationPoint point =  null;
		
		
		if(!exclude){
			return (NavigationPoint)connectedPoints[0];
		}
		
		
		
		int attempts = 25; 
		// By 10th, the chances of having multiple in a row is quite big. 
		// This is a deepthought guard. So just in case, lets try atleast 25 times;
		
		int amountOfPoints = connectedPoints.Count;
		if(amountOfPoints > 2){ // Should be more than 3.
			while(!selected && attempts > 0){
			
				point = (NavigationPoint) connectedPoints[Random.Range (0,connectedPoints.Count)];
				if(exclude != point){
					selected = true;
					break;
				}
				attempts --;
			}
			return point;	// By now we should have a point. Otherwise decide to go back (this is absolutely by chance!)
		
		}else if(amountOfPoints == 2){ //Having issues when there are 2 points. Occasionally starts just looping
		
			if(current != exclude && (((NavigationPoint) connectedPoints[0]) != exclude)){
				return (NavigationPoint)connectedPoints[0];	
			}else if(current != exclude && (((NavigationPoint) connectedPoints[1]) != exclude)){
				return (NavigationPoint)connectedPoints[1];
			}else if(current == exclude && (((NavigationPoint) connectedPoints[0]) == exclude)) {
				return (NavigationPoint)connectedPoints[1];	
			}else{
				return (NavigationPoint)connectedPoints[0];	
			}
			
		}else if(amountOfPoints == 1){
			return (NavigationPoint)connectedPoints[0];
		}
		Debug.LogError("How did this happen? This should NOT occur. No Paths available, yet how did I get here?" );	
		
		return null;
		
	}
	
	
	
	public float getDistance(Vector3 From){
		Vector3 origin = gameObject.transform.position;
		
		
		if(!Physics.Linecast(origin, From, 1<<30 )){
			return (origin - From).magnitude;		
		}else{
			return  -1.0f;	// Do not have clear connection to this.
		}
		
	}
	
	// Should only be used during Start as the scene is loaded. This is to make sure of any connections, and point only the nearest points.
	public bool checkConnection(NavigationPoint To){
		if(To){ // Check if this is even defined?
			Vector3 origin = gameObject.transform.position;
			Vector3 to = To.transform.position;
			Vector3 distance = (origin - to);
			
			if( navigationGroupID == To.navigationGroupID){ // Is in the same group?
				if((distance.sqrMagnitude <= rangeSqr) &&
								(distance.sqrMagnitude <= To.rangeSqr)){

					return !Physics.Linecast(origin, to, 1<<30 ); // Colliode with Layer 31.
				}
			}
		}
		return false;
	}
	
}
