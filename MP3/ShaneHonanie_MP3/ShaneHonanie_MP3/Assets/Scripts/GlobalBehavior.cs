using UnityEngine;
using System.Collections;

public class GlobalBehavior : MonoBehaviour {
	
	public static GlobalBehavior theGlobalBehavior;
	private Bounds mWorldBound;  // this is the world bound
	private Camera mMainCamera;
	public int enemyCount = 50;
	public int eggCount = 0;
	public bool enemyMove = false;
	
	
	
	// to support time ...
	private float mPreEnemySpawnTime = -1f; // 
	private float kEnemySpawnInterval = 3.0f; // in seconds
	
	// spwaning enemy ...
	public GameObject mEnemyToSpawn = null;
	
	// Use this for initialization
	void Start () {
		theGlobalBehavior = this;
		mMainCamera = Camera.main;
		EnemyBehavior.theHero = GameObject.Find ("Hero");
		// World bound support
		mWorldBound = new Bounds(Vector3.zero, Vector3.one);
		UpdateWorldWindowBound();
		
		for(int i = 0; i < 50; i++)
		{
			mEnemyToSpawn = Resources.Load("Prefabs/Enemy") as GameObject;
			GameObject e = (GameObject) Instantiate(mEnemyToSpawn);
		}
		
		// initialize enemy spawning ...
		if (null == mEnemyToSpawn) 
			mEnemyToSpawn = Resources.Load("Prefabs/Enemy") as GameObject;
		
	}
	
	// Update is called once per frame
	void Update () {
		// try to spawn a new enemy
		SpawnAnEnemy();
		
		// To echo text to a defined GUIText
		GameObject echoObject = GameObject.Find("EchoText");
		GUIText gui = echoObject.GetComponent<GUIText>();
		gui.text = "Enemy Count: " + enemyCount + "\nEgg Count: " + eggCount;
		

		

	
	}
	
	#region Game Window World size bound support
	public enum WorldBoundStatus {
		CollideTop,
		CollideLeft,
		CollideRight,
		CollideBottom,
		Outside,
		Inside
	};
	
	/// <summary>
	/// This function must be called anytime the MainCamera is moved, or changed in size
	/// </summary>
	public void UpdateWorldWindowBound()
	{
		// get the main 
		if (null != mMainCamera) {
			float maxZ = mMainCamera.orthographicSize;
			float maxX = mMainCamera.orthographicSize * mMainCamera.aspect;
			float sizeX = 2 * maxX;
			float sizeZ = 2 * maxZ;
			float sizeY = Mathf.Abs(mMainCamera.farClipPlane - mMainCamera.nearClipPlane);
			
			// assumes camera is looking in the negative y-axis
			Vector3 c = mMainCamera.transform.position;
			c.y -= (0.5f * sizeY);
			mWorldBound.center = c;
			mWorldBound.size = new Vector3(sizeX, sizeY, sizeZ);
		}
	}
	
	public Bounds WorldBound { get { return mWorldBound; } }
	
	public WorldBoundStatus ObjectTransformCollideWorldBound(Transform t)
	{
		WorldBoundStatus status = WorldBoundStatus.Inside;
		Vector3 transform = t.position;
		
		if (transform.x > WorldBound.max.x)
		{
			status = WorldBoundStatus.CollideRight;
			transform.x = WorldBound.max.x;
		}
		else if (transform.x < WorldBound.min.x)
		{
			status = WorldBoundStatus.CollideLeft;
			transform.x = WorldBound.min.x;
		}
		if (transform.z > WorldBound.max.z)
		{
			status = WorldBoundStatus.CollideTop;
			transform.z = WorldBound.max.z;
		}
		else if (transform.z < WorldBound.min.z)
		{
				status = WorldBoundStatus.CollideBottom;
				transform.z = WorldBound.min.z;
		}
		if ( (transform.y < WorldBound.min.y) || (transform.y > WorldBound.max.y))
				status = WorldBoundStatus.Outside;
		
		t.position = transform;
		return status;
			
	}
	
	
	public WorldBoundStatus ObjectCollideWorldBound(Bounds objBound)
	{
		WorldBoundStatus status = WorldBoundStatus.Inside;
		if (WorldBound.Intersects(objBound)) {
			if (objBound.max.x > WorldBound.max.x)
				status = WorldBoundStatus.CollideRight;
			else if (objBound.min.x < WorldBound.min.x)
				status = WorldBoundStatus.CollideLeft;
			else if (objBound.max.z > WorldBound.max.z)
				status = WorldBoundStatus.CollideTop;
			else if (objBound.min.z < WorldBound.min.z)
					status = WorldBoundStatus.CollideBottom;
			else if ( (objBound.min.y < WorldBound.min.y) || (objBound.max.y > WorldBound.max.y))
					status = WorldBoundStatus.Outside;
		} else 
			status = WorldBoundStatus.Outside;
			
		return status;
			
	}
	#endregion 
	
	#region to support spawnning of enemies		
	private void SpawnAnEnemy()
	{
		if ((Time.realtimeSinceStartup - mPreEnemySpawnTime) > kEnemySpawnInterval && enemyMove) {
			GameObject e = (GameObject) Instantiate(mEnemyToSpawn);
			enemyCount++;
			mPreEnemySpawnTime = Time.realtimeSinceStartup;
			Debug.Log("New enemy at: " + mPreEnemySpawnTime.ToString());
		}
	}
	#endregion 
		
		private void NewDirection() {
		Vector2 d = Random.insideUnitCircle;
		d.Normalize();
		Vector3 newForward = new Vector3(d.x, 0f, d.y); // NOTICE!! 
		transform.forward = newForward;
	}
	
}
