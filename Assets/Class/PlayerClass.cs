using UnityEngine;
using System.Collections;

public class PlayerClass : MonoBehaviour {

	private bool dragging = false;
	private Queue hitPoints = new Queue();
	private int speed = 8;

	public float maxForce = 10;
	public GameObject pathVertex;
	
	void Start () {
		dragging = false;
	}
	
	void Update () {
		// note - raycast needs a surface to hit against 	
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
			if (Input.GetMouseButtonDown(0)/* && hit.collider.Equals (transform.collider)*/) {
				OnTouchBegin(hit.point);
			}
			else if (dragging && Input.GetMouseButton(0)) {
				OnTouchMove(hit.point);
			}
			else if (dragging && Input.GetMouseButtonUp(0)) {
				OnTouchEnd(hit.point);
			}
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
	
	void OnTouchBegin (Vector3 point) {
		ClearHitPoints ();

		dragging = true;
	}
	
	void OnTouchMove (Vector3 point) {
		if (dragging) {
			Enqueue (point);

			// Only add first out-of-water point
			if (point.y > 0) {
				dragging = false;
			}
		}
	}
	
	void OnTouchEnd (Vector3 point) {
		dragging = false;
	}
	
	void Enqueue (Vector3 point) {
		// TODO: connect path with a line or curve
		GameObject marker = (GameObject)Instantiate(pathVertex);
		marker.transform.position = new Vector3(point.x, point.y, transform.position.z);

		hitPoints.Enqueue (marker);
	}
	
	void Move () {
		// move gameObject
		if (hitPoints.Count > 0) {
			// move gameObject along path
			GameObject next = (GameObject) hitPoints.Peek ();

			Quaternion newRotation = Quaternion.LookRotation (next.transform.position - transform.position);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, newRotation, Time.deltaTime * 40);
			
			Vector3 newForce = transform.forward * 30;
			rigidbody.AddForce(newForce);

			float distance = Vector3.Distance (next.transform.position, transform.position);
			if (distance < 3) {
				// Passed the next marker, remove it
				hitPoints.Dequeue ();
				Destroy (next);
			}
		} else {
			rigidbody.AddForce (transform.forward * 5);
		}
	}

	void FreeFall () {
		ClearHitPoints ();
		rigidbody.AddTorque (Vector3.right);
	}

	void ClearHitPoints() {
		while (hitPoints.Count > 0) {
			Destroy ((GameObject)hitPoints.Dequeue ());
		}
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
