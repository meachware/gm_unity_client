using UnityEngine;
using System.Collections;
using Entity = com.game_machine.entity_system.generated.Entity;

public class NpcBehavior : MonoBehaviour
{
	
	private CharacterMotor motor;
	public GameObject target;
	public float speed = 0f;
	private double lastUpdate = 0;
	private double updatesPerSecond = 10;
	private double updateInterval;
	private bool useServer = true;
	private Vector3 targetPositionFromClient;
	private Vector3 targetPositionFromServer;
	private Vector3 directionVector;
	
	void Start ()
	{
		motor = GetComponent ("CharacterMotor") as CharacterMotor;
		target = GameObject.Find ("Admin Controller");
		updateInterval = 0.60 / updatesPerSecond;
	}
	
	void Update ()
	{
		directionVector = getDirectionVector ();
		targetPositionFromClient = target.transform.position;
			
		if (useServer) {
			targetPositionFromServer = npcPosition ();
			
			if (targetPositionFromServer != Vector3.zero) {
				if (Vector3.Distance (targetPositionFromServer, targetPositionFromClient) <= 0) {
					targetPositionFromServer = targetPositionFromClient;
				}
				moveTowards (transform, targetPositionFromServer, directionVector);
			}
		} else {
			moveTowards (transform, targetPositionFromClient, directionVector);
		}
			
		lastUpdate = Time.time;
		
	}
	
	Vector3 npcPosition ()
	{
		if (GameClient.npcs.ContainsKey (name)) {
			Entity npc = GameClient.npcs [name];
			Vector3 currentVector = new Vector3 ((float)npc.vector3.xi, transform.position.y, (float)npc.vector3.yi);
			return  currentVector;
		} else {
			return Vector3.zero;
		}
	}
	
	void moveTowards (Transform transform, Vector3 targetPosition, Vector3 directionVector)
	{
		motor.inputMoveDirection = transform.rotation * directionVector;
		Vector3 targetDir = targetPosition - transform.position;
		float step = 300f * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);
	}
	
	Vector3 getDirectionVector ()
	{
		speed = 0.8f;
		Vector3 directionVector = new Vector3 (0f, 0f, speed);
	
		if (directionVector != Vector3.zero) {
			
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
		
			directionLength = Mathf.Min (1, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		return directionVector;
	}
}
