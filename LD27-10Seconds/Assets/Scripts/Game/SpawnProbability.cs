using UnityEngine;

namespace customGameVariables {
	[System.Serializable]
	public class SpawnProbability
	{
		public float startProbability = 1f;
		
		public float endProbability = 0f;
		public int maxAmount = 5;
		public int minAmount = 2;
		
		public GameObject npc;
		
		
	}
	

}