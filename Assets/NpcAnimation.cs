using UnityEngine;
using System.Collections;

public class NpcAnimation : MonoBehaviour
{
	public GameObject target;
	public float distance;
	public NpcBehavior behavior;
	void Start ()
	{
		target = GameObject.Find ("Admin Controller");
		Debug.Log (transform.parent);
		behavior = transform.parent.gameObject.GetComponent("NpcBehavior") as NpcBehavior;
	}
	
	void Update ()
	{
		distance = Vector3.Distance (transform.position, target.transform.position);
		if (distance <= 1f) {
			if (!animation.IsPlaying ("idle")) {
				animation.CrossFade ("idle");
			}
		} else {
			if (behavior.speed >= 0.8f) {
				if (!animation.IsPlaying ("run")) {
					animation.CrossFade ("run");
				}
			} else {
				if (!animation.IsPlaying ("walk")) {
					animation.CrossFade ("walk");
				}
			}
			
		}
		
	}
}
