public var mouseSensitivity : float = 1.0;
private var lastPosition : Vector3;
public var player : GameObject;
public var horizon : GameObject;

function FixedUpdate() {
	var screenPosition = camera.WorldToScreenPoint(player.transform.position);
	
	var ratioX = screenPosition.x/camera.pixelWidth;
	var ratioY = screenPosition.y/camera.pixelHeight;
	
	transform.position.x -= 0.5 - ratioX;
	transform.position.y -= 0.5 - ratioY;
	horizon.transform.position.x -= 0.5 - ratioX;
}
