using UnityEngine;
using System.Collections;

public class EggBehavior : MonoBehaviour {
	
	private float mSpeed = 100f;
	
	
	// Update is called once per frame
	void Update () {
		transform.position += (mSpeed * Time.smoothDeltaTime) * transform.forward;
		
		if(GlobalBehavior.theGlobalBehavior.ObjectCollideWorldBound(transform.collider.bounds) == GlobalBehavior.WorldBoundStatus.Outside)
		{
			Destroy(this);
			GlobalBehavior.theGlobalBehavior.eggCount--;
			
		}
	}
	
	public void SetForwardDirection(Vector3 f)
	{
		transform.forward = f;
	}
	
	
}
