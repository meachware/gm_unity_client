using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	void Start ()
	{
		Random.seed = (int)(System.DateTime.Now.Ticks / 10000);
		for (int i = 0; i < 0; i++) {
			Vector3 pos = new Vector3(Random.Range (0f, 1990f), 100f,Random.Range (0f, 1990f));
			GameObject go = Instantiate (Resources.Load ("Npc Controller"), pos, Quaternion.identity) as GameObject;
			go.gameObject.name = string.Format ("npc_{0}", i.ToString ());
		}
	}
	
	void Update ()
	{
	
	}
}
