using System;


public interface DamageAbleEntity
{
	
	bool healthCheck(int modifier=0);

	float healthPercent();
}


