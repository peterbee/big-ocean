using UnityEngine;
using System.Collections;
using Parse;

public class PlayerClass : MonoBehaviour {

	private bool dragging = false;
	private GameObject target;
	private float nextSave;

	private ParseObject parseObject;

	public bool isThisPlayer = false;
	public float maxForce = 10;
	public GameObject pathVertex;
	public GameObject following;
	public string playerId;
	public System.DateTime? updatedAt;

	void Start () {
		dragging = false;

		if (isThisPlayer) {
			LoadPlayer ();
		}
	}

	void LoadPlayer () {
		string playerId = PlayerPrefs.GetString ("id");	
		print ("Player Id " + playerId);

		parseObject = new ParseObject ("Character");

		/*if (playerId != null) {
			parseObject.ObjectId = playerId;
		}*/

		/*if (playerId != null) {
			print ("querying");
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Character");
			query.GetAsync(query).ContinueWith(t =>
			{
				parseObject = t.Result;
				print ("Parse Object " + parseObject);

				if (parseObject == null) {
					parseObject = new ParseObject ("Character");
				}
			});
		} else {
			print ("creating new");
			parseObject = new ParseObject ("Character");
		}*/
	}

	void SavePlayer () {
		if (parseObject != null && Time.time > nextSave) {
			nextSave += 5f;

			parseObject["position_x"] = transform.position.x;
			parseObject["position_y"] = transform.position.y;
			parseObject["position_z"] = transform.position.z;
			parseObject["target_x"] = 0;
			parseObject["target_y"] = 0;
			parseObject["target_z"] = 0;

			if (target != null) {
				parseObject["target_x"] = target.transform.position.x;
				parseObject["target_y"] = target.transform.position.y;
				parseObject["target_z"] = target.transform.position.z;
			}

			parseObject.SaveAsync().ContinueWith(task => 
			{
				if (task.IsCanceled)
				{
					// the save was cancelled.
					print ("Task cancelled");
				}
				else if (task.IsFaulted)
				{
					foreach(var e in task.Exception.InnerExceptions) {
						ParseException parseException = (ParseException) e;
						Debug.Log("Error message " + parseException.Message);
						Debug.Log("Error code: " + parseException.Code);
					}
				}
				else
				{
					// the object was saved successfully.
					print ("Saved " + parseObject.ObjectId);
					playerId = parseObject.ObjectId;
				}
			});
		}
	}
	
	void Update () {
		if (isThisPlayer) {
			// note - raycast needs a surface to hit against 	
			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				if (Input.GetMouseButtonDown(0)/* && hit.collider.Equals (transform.collider)*/) {
					OnTouchBegin(hit);
				}
				else if (dragging && Input.GetMouseButton(0)) {
					OnTouchMove(hit);
				}
				else if (dragging && Input.GetMouseButtonUp(0)) {
					OnTouchEnd(hit);
				}
			}

			SavePlayer ();
		}
	}

	void FixedUpdate () {
		bool isInWater = transform.position.y < 0;

		if (isInWater) {
			rigidbody.useGravity = false;
			rigidbody.drag = 2;
			Move ();
		} else {
			rigidbody.useGravity = true;
			rigidbody.drag = 0;
			FreeFall ();
		}
	}
	
	void OnTouchBegin (RaycastHit hit) {
		ClearHitPoints ();

		dragging = true;
		Enqueue (hit);
	}
	
	void OnTouchMove (RaycastHit hit) {
		if (dragging) {
			Enqueue (hit);

			// Only add first out-of-water point
			if (hit.point.y > 0) {
				dragging = false;
			}
		}
	}
	
	void OnTouchEnd (RaycastHit hit) {
		dragging = false;
	}

	public void SetTarget (Vector3 position) {
		Destroy (target);
		target = (GameObject)Instantiate (pathVertex);
		target.transform.position = position;
	}

	void Enqueue (RaycastHit hit) {
		GameObject marker = (GameObject)Instantiate(pathVertex);
		marker.transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);

		Destroy (target);
		target = marker;
	}
	
	void Move () {
		// move gameObject
		if (following != null) {
			float distance = Vector3.Distance (following.transform.position, transform.position);

			// if not too close, follow the leader
			if (distance > 20) {
				TurnTowards (following);
			} else {
				Divert ();
			}
			GoForward ();
		} else if (target != null) {
			// move gameObject towards target
			TurnTowards (target);
			GoForward ();

			float distance = Vector3.Distance (target.transform.position, transform.position);
			if (distance < 3) {
				// Passed the next marker, remove it
				Destroy (target);
				target = null;
			}
		} else {
			rigidbody.AddForce (transform.forward * 5);
		}
	}

	void TurnTowards (GameObject other) {
		Quaternion newRotation = Quaternion.LookRotation (other.transform.position - transform.position);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, newRotation, Time.deltaTime * 40);
	}
	
	void Divert () {
		transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (Vector3.up), Time.deltaTime * 40);
	}
	
	void GoForward () {
		Vector3 newForce = transform.forward * 30;
		rigidbody.AddForce(newForce);
	}

	void FreeFall () {
		ClearHitPoints ();
		rigidbody.AddTorque (Vector3.right);
	}

	void ClearHitPoints() {
		Destroy (target);
		target = null;
	}

	/*public float speed;
	public GameObject bombPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x > 18) {
			//get new speed
			speed = Random.Range(8f,12f);
			transform.position = new Vector3( -18f, transform.position.y, transform.position.z );
		}		
		transform.Translate(0, 0, speed * Time.deltaTime);

		var fwd = transform.TransformDirection (Vector3.forward);
		if (Physics.Raycast (transform.position, fwd, 10)) {
			print ("There is something in front of the object!");
		}

		/*if (Input.anyKeyDown) {
			GameObject bombObject = (GameObject)Instantiate(bombPrefab);
			bombObject.transform.position = this.gameObject.transform.position;
		}*
	}*/
}
