using UnityEditor;
using UnityEngine;
using System.Collections;

public class NavigationGizmo { // Custom Tool I wrote for Developing 
	// levels and seeing if the waypoints are connected to each other
	 public bool rendered = false;
	 [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
	 static void RenderLightGizmo (GameObject obj, GizmoType gizmoType) {
		NavigationPoint script = (NavigationPoint) obj.GetComponent<NavigationPoint>();
		
		if(script != null){ 
			
			Gizmos.color = script.color;
			Gizmos.DrawWireSphere( obj.transform.position, 0.125f );
			
			Gizmos.color = new Color( script.color.r, script.color.g, script.color.b, 0.25f);
			GameObject[] others = GameObject.FindGameObjectsWithTag("Navigation");
			foreach(GameObject other in others){
				NavigationPoint point = (NavigationPoint)other.GetComponent<NavigationPoint>();
				if(point){ // Point is a Navigation tPoint.
					
					if(script.checkConnection(point)){
						// Doesnt hit anything. Way is unobtruced.
						Gizmos.DrawLine( obj.transform.position, other.transform.position);	
					} 
					
				}
				
			}
			
		}
	}
	
}

