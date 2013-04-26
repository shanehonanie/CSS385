using UnityEngine;	
using System.Collections;

public class InteractiveControl : MonoBehaviour {
	
	public GameObject mProjectile = null;
	
    // Speed controls
    private float kHeroSpeed = 20f;
	private float kHeroRotateSpeed = Mathf.PI/2f; // 90-degrees in 2 seconds
	private float eggSpawnRate = -1f;
	private float eggFireRate = 0.1f;
	

	// Use this for initialization
	void Start () {
	// initialize enemy spawning ...
		if (null == mProjectile) 
			mProjectile = Resources.Load("Prefabs/Egg") as GameObject;
	}
	private bool eggFireEnabled()
	{
		if (Time.realtimeSinceStartup - eggSpawnRate > eggFireRate)
		{
			eggSpawnRate = Time.realtimeSinceStartup;
			return true;
		}
		return false;
	}
	// Update is called once per frame
	void Update () {
		transform.position += Input.GetAxis ("Vertical")  * transform.forward * (kHeroSpeed * Time.smoothDeltaTime);
		transform.RotateAround(Vector3.up, Input.GetAxis("Horizontal") * (kHeroRotateSpeed * Time.smoothDeltaTime));
		// transform.position += Input.GetAxis ("Horizontal") * transform.right * 0.5f;
		GlobalBehavior.theGlobalBehavior.ObjectTransformCollideWorldBound(transform);
		
		
		if ((Input.GetAxis("Fire1") >0.0) && eggFireEnabled()) { // this is Left-Control
			GlobalBehavior.theGlobalBehavior.eggCount++;
			GameObject e = Instantiate(mProjectile) as GameObject;
			EggBehavior egg = e.GetComponent<EggBehavior>(); // Shows how to get the script from GameObject
			if (null != egg) {
				e.transform.position = transform.position;
				egg.SetForwardDirection(transform.forward);
			}
		}
	}
	
	
	
	
}
