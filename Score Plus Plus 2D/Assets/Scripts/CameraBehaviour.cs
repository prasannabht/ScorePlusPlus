
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

	public float smoothness = 3f;
	//public float rotateSmoothness = 1f;
	//float camRotX;

	public float offset = -10f;

	float currYOffset = 0;
	public float YOffset = -1f;

	int zoomType;
	float zoomOffset;

	GameObject platform, prevPlatform;

	private Camera cam;

	[HideInInspector]
	public bool cameraAdjusted = false;

	float currOrthoSize;

	public float orthoZoom = 14f;
	public float orthoSize = 14f;
	public float orthoSize2 = 22f;
	public float orthoSize3 = 30f;

	public float orthoZoomHoriz = 6f;
	public float orthoSizeHoriz = 10f;
	public float orthoSizeHoriz2 = 10f;
	public float orthoSizeHoriz3 = 10f;

	[HideInInspector]
	public bool IsLandscapeMode = false;

	bool zoomInPlatformFlag = false;

	void Awake(){
		if (Screen.height < Screen.width) {
			IsLandscapeMode = true;
		}
	}
	void Start(){
		cam = GetComponent<Camera> ();
		//camRotX = transform.rotation.eulerAngles.x;
		zoomInPlatformFlag = false;
		currYOffset = 0;

		if (Screen.height >= Screen.width) {
			IsLandscapeMode = false;
			cam.orthographicSize = orthoSize;

		} else {
			IsLandscapeMode = true;
			cam.orthographicSize = orthoSizeHoriz;
		}

		transform.position = offset * cam.transform.forward;
	}

	void Update (){

		if (Screen.height >= Screen.width) {
			IsLandscapeMode = false;
			if (zoomInPlatformFlag) {
				currOrthoSize = orthoZoom;
			} else {
				if (zoomType == 1) {
					currOrthoSize = orthoSize + zoomOffset;
				} else if (zoomType == 2) {
					currOrthoSize = orthoSize2 + zoomOffset;
				} else if (zoomType == 3) {
					currOrthoSize = orthoSize3 + zoomOffset;
				}
			}
		} else {
			IsLandscapeMode = true;
			if (zoomInPlatformFlag) {
				currOrthoSize = orthoZoomHoriz;
			} else {
				if (zoomType == 1) {
					currOrthoSize = orthoSizeHoriz + zoomOffset;
				} else if (zoomType == 2) {
					currOrthoSize = orthoSizeHoriz2 + zoomOffset;
				} else if (zoomType == 3) {
					currOrthoSize = orthoSizeHoriz3 + zoomOffset;
				}
			}
		}
	
	}

	void LateUpdate () {

		if (platform == null) {
			return;
		}


		//if (!cameraAdjusted) {
		if(true){

			Vector3 centerPoint = new Vector3();
			Bounds objectBounds = new Bounds();
			float screenRatio;
			float targetRatio;
			
			centerPoint = FindObjectOfType<GameManager> ().GetCenterPoint (platform, false);
			objectBounds = FindObjectOfType<GameManager> ().GetBounds (platform);

			if (cam.orthographicSize != currOrthoSize) {
				cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currOrthoSize, smoothness * Time.deltaTime);
			}

			Vector3 _position = centerPoint + offset * cam.transform.forward + new Vector3 (0, currYOffset, 0);

			transform.position = Vector3.Lerp (transform.position, _position, smoothness * Time.deltaTime);
			//Vector3 rotation = Quaternion.Lerp (transform.rotation, platform.transform.rotation, rotateSmoothness * Time.deltaTime).eulerAngles;
			//transform.rotation = Quaternion.Euler (camRotX, 0, rotation.z);

			//transform.Translate(0, currYOffset, 0);

			float dist = Vector3.Distance (transform.position, _position);
			if (dist < 0.1f) {
				cameraAdjusted = true;

				if (prevPlatform != null) {
					Destroy (prevPlatform);
					prevPlatform = null;
				}
			} else {
				cameraAdjusted = false;
			}
		}
	}

	public void MoveCamera(GameObject _platform){

		if (prevPlatform != null) {
			Destroy (prevPlatform);
			prevPlatform = null;
		}

		//Reset flag to move camera
		cameraAdjusted = false;
		platform = _platform;
		prevPlatform = null;
		currYOffset = 0;
		currOrthoSize = cam.orthographicSize;

		zoomInPlatformFlag = false;

	}

	public void MoveCamera(GameObject _platform, GameObject _prevPlatform){

		if (prevPlatform != null) {
			Destroy (prevPlatform);
			prevPlatform = null;
		}

		//Reset flag to move camera
		cameraAdjusted = false;
		platform = _platform;
		prevPlatform = _prevPlatform;
		currYOffset = 0;
		currOrthoSize = cam.orthographicSize;

		zoomInPlatformFlag = false;

	}

	public void MoveCamera(GameObject _platform, GameObject _prevPlatform, int _zoomType, float _zoomOffset, int _puzzleNum){

		if (prevPlatform != null) {
			Destroy (prevPlatform);
			prevPlatform = null;
		}

		//Reset flag to move camera
		cameraAdjusted = false;
		platform = _platform;
		prevPlatform = _prevPlatform;

		zoomType = _zoomType;
		zoomOffset = _zoomOffset;

		if (_puzzleNum == 2) {
			currYOffset = YOffset;
		} else {
			currYOffset = 0;
		}

		zoomInPlatformFlag = false;

	}

	public void ZoomInPlatform(){
		zoomInPlatformFlag = true;
		currYOffset = 0;
	}


	public void ZoomOutPlatform(){
		zoomInPlatformFlag = false;
		currYOffset = 0;
	}


}
