public var mouseSensitivity : float = 1.0;
private var lastPosition : Vector3;
public var player : GameObject;
public var horizon : GameObject;

function FixedUpdate() {
	var screenPosition = camera.WorldToViewportPoint(player.transform.position);
	
	transform.position.x -= 0.5 - screenPosition.x;
	transform.position.y -= 0.5 - screenPosition.y;
	horizon.transform.position.x -= 0.5 - screenPosition.x;
}
