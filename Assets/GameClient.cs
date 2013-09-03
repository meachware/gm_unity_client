using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using Entity = com.game_machine.entity_system.generated.Entity;
using TrackEntity = com.game_machine.entity_system.generated.TrackEntity;
using EVector3 = com.game_machine.entity_system.generated.Vector3;
using ClientMessage = com.game_machine.entity_system.generated.ClientMessage;
using Player = com.game_machine.entity_system.generated.Player;
using Neighbors = com.game_machine.entity_system.generated.Neighbors;
using GetNeighbors = com.game_machine.entity_system.generated.GetNeighbors;
using IsNpc = com.game_machine.entity_system.generated.IsNpc;
using PlayerLogout = com.game_machine.entity_system.generated.PlayerLogout;
using  ProtoBuf;
using System.Collections.Concurrent;

public class GameClient : MonoBehaviour
{
	
	private Thread networkThread;
	private int playerCount = 0;
	private double lastUpdate = 0;
	private double updatesPerSecond = 8;
	private double updateInterval;
	private IPEndPoint udp_ep;
	private UdpClient udpClient;
	public static Dictionary<string,Entity> npcs = new Dictionary<string,Entity> ();
	private ConcurrentDictionary<string,Neighbors> neighborsFromServer = new ConcurrentDictionary<string,Neighbors> ();
	
	void Start ()
	{
		Debug.Log ("Start");
		updateInterval = 0.60 / updatesPerSecond;
		udp_ep = new IPEndPoint (IPAddress.Any, 11000);
		udpClient = new UdpClient (udp_ep);
		receiveData ();
		doLogin ();
		
	}
	
	void Update ()
	{
		if (Time.time > (lastUpdate + updateInterval)) {
			lastUpdate = Time.time;
			
			sendPlayerPosition ();
			
			Neighbors neighbors = new Neighbors ();
			if (neighborsFromServer.TryGetValue ("1", out neighbors)) {
				foreach (Entity npc in neighbors.npc) {
					if (GameClient.npcs.ContainsKey (npc.id)) {
						GameClient.npcs [npc.id] = npc;
					} else {
						createNpc (npc);
						GameClient.npcs.Add (npc.id, npc);
					}
					
				}
				neighborsFromServer.Clear ();	
			}
		}
	}
	
	void sendMessage (ClientMessage message)
	{
		MemoryStream stream = new MemoryStream ();
		Serializer.Serialize (stream, message);
		byte[] bytes = stream.ToArray ();
		udpClient.Send (bytes, bytes.Length, "192.168.1.6", 8100);
	}
		
	void sendPlayerPosition ()
	{
		GameObject gameObject = GameObject.FindWithTag ("Player");
		Vector3 position = gameObject.transform.position;
		ClientMessage clientMessage = baseMessage ();
		Player player = clientMessage.player;
		
		Entity entity = new Entity ();
		EVector3 evector = new EVector3 ();
		entity.vector3 = evector;
		
		entity.vector3.x = position.x;
		entity.vector3.y = position.z;
		entity.vector3.z = position.y;
		
		entity.id = player.id;
		entity.entityType = "player";
		TrackEntity trackEntity = new TrackEntity ();
		trackEntity.value = true;
		entity.trackEntity = trackEntity;
		GetNeighbors getNeighbors = new GetNeighbors ();
		getNeighbors.neighborType = "npc";
		getNeighbors.vector3 = new EVector3 ();
		getNeighbors.vector3.x = position.x;
		getNeighbors.vector3.y = position.z;
		getNeighbors.vector3.z = position.y;
		entity.getNeighbors = getNeighbors;
		clientMessage.entity.Add (entity);
		sendMessage (clientMessage);
	}
	
	void doLogin ()
	{
		sendMessage (baseMessage ());
	}
	
	ClientMessage baseMessage ()
	{
		Player player = basePlayer ();
		ClientMessage clientMessage = new ClientMessage ();
		clientMessage.player = player;
		return clientMessage;
	}
	
	Player basePlayer ()
	{
		Player player = new Player ();
		player.id = "player";
		player.authtoken = "authorized";
		return player;
	}
	
	void createNpc (Entity npc)
	{
		float x = npc.vector3.xi;
		float z = npc.vector3.yi;
		Vector3 pos = new Vector3 (x, 10f, z);
		GameObject go = Instantiate (Resources.Load ("Npc Controller"), pos, Quaternion.identity) as GameObject;
		go.gameObject.name = npc.id;
		playerCount += 1;
		Debug.Log (playerCount);
	}
	
	void printResponse (ClientMessage message)
	{
		foreach (Entity entity in message.entity) {
			neighborsFromServer.TryAdd ("1", entity.neighbors);
		}
	}
	
	void OnApplicationQuit ()
	{
		ClientMessage clientMessage = baseMessage ();
		PlayerLogout logout = new PlayerLogout();
		logout.playerId = basePlayer().id;
		clientMessage.playerLogout = logout;
		sendMessage (clientMessage);
		udpClient.Close ();
		//networkThread.Abort ();
		//networkThread.Join ();
		Debug.Log ("Networking stopped");
	}
	
	private void dataReady (IAsyncResult ar)
	{
		byte[] bytes = udpClient.EndReceive (ar, ref udp_ep);
		MemoryStream stream = new MemoryStream (bytes);
		ClientMessage clientMessage = Serializer.Deserialize<ClientMessage> (stream);
		printResponse (clientMessage);
		receiveData ();
	}
	
	private void receiveData ()
	{
		udpClient.BeginReceive (new AsyncCallback (dataReady), udp_ep);
	}
	
}
