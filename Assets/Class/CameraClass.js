public var mouseSensitivity : float = 1.0;
private var lastPosition : Vector3;
public var player : GameObject;
public var horizon : GameObject;

function Update() {
	if (Input.GetKey (KeyCode.DownArrow)) {
		if (camera.orthographicSize < 20)
			camera.orthographicSize += 0.1;
	}
	if (Input.GetKey (KeyCode.UpArrow)) {
		if (camera.orthographicSize > 4)
			camera.orthographicSize -= 0.1;
	}
}

function FixedUpdate() {
	var screenPosition = camera.WorldToViewportPoint(player.transform.position);
	
	transform.position.x -= 0.5 - screenPosition.x;
	transform.position.y -= 0.5 - screenPosition.y;
	horizon.transform.position.x -= 0.5 - screenPosition.x;
}
