using UnityEngine;
using System.Collections;

public class MaxAnimation : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!animation.IsPlaying ("run")) {
			animation.CrossFade ("run");
		}
	
	}
}
