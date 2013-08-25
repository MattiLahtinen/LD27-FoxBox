using UnityEditor;
using UnityEngine;
using System.Collections;

public class SpawnGizmo { // Custom Tool I wrote for Developing 
	// levels and seeing if the waypoints are connected to each other
	 public bool rendered = false;
	 [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
	 static void RenderLightGizmo (GameObject obj, GizmoType gizmoType) {
		SpawnPoint script = (SpawnPoint) obj.GetComponent<SpawnPoint>();
		
		if(script != null){ 
			
			Gizmos.color = script.color;
			Gizmos.DrawWireSphere( obj.transform.position, 0.5f );
			
			
			
		}
	}
	
}

