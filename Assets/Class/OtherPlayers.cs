using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class OtherPlayers : MonoBehaviour {
	
	private float nextQuery;
	private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	private IEnumerable<ParseObject> threadResult;
	private PlayerClass thisPlayer;
	
	public GameObject playerPrefab;
	
	// Use this for initialization
	void Start () {
		thisPlayer = (PlayerClass) GameObject.Find ("Player").GetComponent ("PlayerClass");
	}
	
	// Update is called once per frame
	void Update () {
		// first do whatever we got back from the last process
		if (threadResult != null) {
			foreach (ParseObject playerData in threadResult)
			{
				//print ("Player " + playerData.ObjectId);
				
				if (playerData.ObjectId == thisPlayer.playerId) {
					continue;
				}

				GameObject player;
				if (players.TryGetValue (playerData.ObjectId, out player)) {
					//print("Player found " + player);
				} else {
					player = (GameObject) Instantiate(playerPrefab);
					player.SetActive (false); // start inactive
					player.transform.position = new Vector3 (playerData.Get<float>("position_x"), playerData.Get<float>("position_y"), playerData.Get<float>("position_z"));
					players.Add (playerData.ObjectId, player);
					print ("Player created " + player);
				}

				PlayerClass playerScript = (PlayerClass) player.GetComponent ("PlayerClass");

				if (playerScript.updatedAt == null || playerScript.updatedAt == playerData.UpdatedAt) {
					playerScript.SetActive (false); // should be false; set to true for testing with lots of whales
				} else {
					playerScript.SetActive (true);
				}
				playerScript.updatedAt = playerData.UpdatedAt;

				if (playerData.Get<float>("target_z") != 0) {
					playerScript.SetTarget (
						new Vector3 (playerData.Get<float>("target_x"), playerData.Get<float>("target_y"), playerData.Get<float>("target_z"))
					);
				} else {
					playerScript.SetTarget (
						new Vector3 (playerData.Get<float>("position_x"), playerData.Get<float>("position_y"), playerData.Get<float>("position_z"))
					);
				}
			}
			
			threadResult = null;
		}
		
		if (Time.time > nextQuery) {
			nextQuery += 5f;
			
			var query = ParseObject.GetQuery("Character");
			query.FindAsync().ContinueWith(t =>
			                               {
				threadResult = t.Result;
			});
		}
	}
}
