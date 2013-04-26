using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {
	
	public static GameObject theHero;
    private float kReferenceSpeed = 20f;
	public float mSpeed = 20f;
	private bool enemyMove = false;
	//GlobalBehavior gb1;
	private int curEnemyState;
	private float maxStunDUration = 5f;
	private float stunStartTime;
	private float eggHitsWhileStunned = 0;
	private float maxNumStunIterations = 3f;
	private float minSpeed = 20f;
	private float maxSpeed = 40f;
	private bool alreadyDestroyed = false;
			
	// Use this for initialization
	void Start () {
		Bounds b = GameObject.Find("GameManager").GetComponent<GlobalBehavior>().WorldBound;
		Vector3 v = Random.insideUnitSphere;
		v.x *= b.size.x * 0.5f;  // half of the size;
		v.z *= b.size.z * 0.5f;
		transform.position = v + b.center;
		v.y = 0f;
		
		mSpeed = Random.Range(minSpeed, maxSpeed);
		
		NewDirection();	
		setNormalState();
		//gb1 = GetComponent<GlobalBehavior>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown("Jump"))
		{
			enemyMove = !enemyMove;
			GlobalBehavior.theGlobalBehavior.enemyMove = enemyMove;
		}
		
		if(enemyMove && curEnemyState != (int)enemyState.Stunned)
			transform.position += (mSpeed * Time.smoothDeltaTime) * transform.forward;
		
		GlobalBehavior globalBehavior = GameObject.Find ("GameManager").GetComponent<GlobalBehavior>();
		
		GlobalBehavior.WorldBoundStatus status =
				globalBehavior.ObjectCollideWorldBound(this.transform.collider.bounds);
		
		if (status != GlobalBehavior.WorldBoundStatus.Inside) {
			NewDirection();
		}	
		
		switch(curEnemyState)
		{
			case (int)enemyState.Normal:
			updateNormalState();
				break;
			case (int)enemyState.Run:
				updateRunState();
				break;
			case (int)enemyState.Stunned:
			updateStunnedState();
				break;
		}
	}
	
	public void updateStunnedState()
	{
		if((Time.realtimeSinceStartup - stunStartTime) < maxStunDUration)
		{
			transform.RotateAround(Vector3.up, .3f*Time.smoothDeltaTime);
		}
		else
		{
			setNormalState();
			GetComponent<MeshRenderer>().material.mainTexture = Resources.Load ("Textures/EnemyNormal") as Texture;	
		}
	}
	
	public void updateRunState()
	{
		Vector3 distVec = (EnemyBehavior.theHero.transform.position - transform.position);
		if (distVec.sqrMagnitude < (30*30))
		{
			Vector3 heroForward = EnemyBehavior.theHero.transform.forward;
			Vector3 dir = Mathf.Sign(Vector3.Cross(distVec,heroForward).y) * EnemyBehavior.theHero.transform.right;
			float theta = Mathf.Acos(Vector3.Dot (dir, transform.forward));
			if(Mathf.Abs(theta) > 0.01f)
				transform.RotateAround(Vector3.up, Time.smoothDeltaTime * Mathf.PI * Mathf.Sign (Vector3.Cross (transform.forward, dir).y * theta));
			transform.position += mSpeed * Time.smoothDeltaTime * transform.forward;
		}
		else
		{
			setNormalState();
			GetComponent<MeshRenderer>().material.mainTexture = Resources.Load ("Textures/EnemyNormal") as Texture;	
		}
	}
	
	public void updateNormalState()
	{
		if(EnemyBehavior.theHero != null)
		{
			Vector3 dirVec = (EnemyBehavior.theHero.transform.position - transform.position);
			Vector3 heroForward = EnemyBehavior.theHero.transform.forward;
			
			if((dirVec.sqrMagnitude >= (30f*30f)) || Vector3.Dot (dirVec, heroForward) >= 0.01f)
				return;
			
			setRunState();
			GetComponent<MeshRenderer>().material.mainTexture = Resources.Load ("Textures/EnemyRun") as Texture;	
		}
	}
	
	public void setNormalState()
	{
		curEnemyState = (int)enemyState.Normal;
		// To change texture for an object
		//	GameObject hero = GameObject.Find("Enemy");
		//	Renderer r = hero.GetComponent<Renderer>();
		//	r.material.mainTexture = Resources.Load("Textures/EnemyStunned") as Texture;
	}
	
	public void setStunnedState()
	{
		eggHitsWhileStunned++;
		if ( eggHitsWhileStunned >= maxNumStunIterations)
		{
			Destroy(gameObject);
			GlobalBehavior.theGlobalBehavior.enemyCount--;
		}
		curEnemyState = (int)enemyState.Stunned;
		stunStartTime = Time.realtimeSinceStartup;
		GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/EnemyStunned") as Texture;
	}
	
	public void setRunState()
	{
		curEnemyState = (int)enemyState.Run;
	}
	
	void OnTriggerEnter(Collider other) {
		//Debug.Log("EnemyBehavior OnTrigger called");
		if (other.gameObject.name == "Egg(Clone)") {
			Destroy (other.gameObject);
			//alreadyDestroyed = false;
			//Destroy(gameObject);
			
		//	if(!alreadyDestroyed)
				GlobalBehavior.theGlobalBehavior.eggCount--;
			
			setStunnedState();
		//	alreadyDestroyed = true;
		}
	}
	
	private void NewDirection() {
		Vector2 d = Random.insideUnitCircle;
		d.Normalize();
		Vector3 newForward = new Vector3(d.x, 0f, d.y); // NOTICE!! 
		transform.forward = newForward;
	}
	
	
	enum enemyState {
		Normal = 0, Run = 1, Stunned = 2
	}
	
}
