﻿using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {
	public float moveSpeed;
	private Vector3 moveDirection;
	public float turnSpeed;

	[SerializeField]
	private PolygonCollider2D[] colliders;
	private int currentColliderIndex = 0;

	private static float AccelerometerUpdateInterval = 1 / 60;
	private static float LowPassKernelWidthInSeconds = 1;
	private float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; 
	private Vector3 lowPassValue = Vector3.zero;

	public static int score = 0;
	private GUIText scoreGT;

	// Use this for initialization
	void Start () {
		moveDirection = Vector3.right;
		lowPassValue = Input.acceleration;

		score = 0;
		GameObject scoreGO = GameObject.Find ("score");
		scoreGT = scoreGO.GetComponent<GUIText> ();
		scoreGT.text = "0";
	}
	
	public Vector3 LowPassFilterAccelerometer() {
		lowPassValue = new Vector3(Mathf.Lerp(lowPassValue.x, Input.acceleration.x, LowPassFilterFactor), Mathf.Lerp(lowPassValue.y, Input.acceleration.y, LowPassFilterFactor), 0);
		return lowPassValue;
	}

	// Update is called once per frame
	void Update () {

		Vector3 currentPosition = transform.position;
//		if( Input.GetButton("Fire1") ) {
//			Vector3 moveToward = Camera.main.ScreenToWorldPoint( Input.mousePosition );
//			moveDirection = moveToward - currentPosition;
//			moveDirection.z = 0; 
//			moveDirection.Normalize();
//		}

		//Move by accelerometer

		if (Input.deviceOrientation == DeviceOrientation.FaceUp) {
			
			transform.Translate (Input.acceleration.y, -Input.acceleration.x, 0);
			
			Vector3 moveToward = new Vector3 (Input.acceleration.y, -Input.acceleration.x, 0);
			moveDirection = moveToward - currentPosition;
			moveDirection.Normalize ();
			
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeRight) {

			transform.Translate (Input.acceleration.y, -Input.acceleration.x, 0);
		
			Vector3 moveToward = new Vector3 (Input.acceleration.y, -Input.acceleration.x, 0);
			moveDirection = moveToward - currentPosition;
			moveDirection.Normalize ();
		
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
			
			transform.Translate (-Input.acceleration.y, Input.acceleration.x, 0);
			
			Vector3 moveToward = new Vector3 (-Input.acceleration.y, Input.acceleration.x, 0);
			moveDirection = moveToward - currentPosition;
			moveDirection.Normalize ();
			
		}
		if (Input.deviceOrientation == DeviceOrientation.Portrait) {

			transform.Translate (Input.acceleration.x, Input.acceleration.y, 0);

			Vector3 moveToward = new Vector3 (Input.acceleration.x, Input.acceleration.y, 0);
			moveDirection = moveToward - currentPosition;
			moveDirection.Normalize ();
			
		}
		if (Input.deviceOrientation == DeviceOrientation.Portrait) {
			
			transform.Translate (-Input.acceleration.x, -Input.acceleration.y, 0);
			
			Vector3 moveToward = new Vector3 (-Input.acceleration.x, -Input.acceleration.y, 0);
			moveDirection = moveToward - currentPosition;
			moveDirection.Normalize ();
			
		}
//		Vector3 target = moveDirection * moveSpeed + currentPosition;
//		transform.position = Vector3.Lerp( currentPosition, target, Time.deltaTime );



		/*
		//Move by finger
		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved) {
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			
			Vector3 touchmove = new Vector3(touchDeltaPosition.x, touchDeltaPosition.y, 0);
			
			transform.position = Camera.main.ScreenToWorldPoint(touchmove);

			moveDirection = touchDeltaPosition;
		}
		*/

		//Rotate Angle
		float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		transform.rotation = 
			Quaternion.Slerp( transform.rotation, 
			                 Quaternion.Euler( 0, 0, targetAngle ), 
			                 turnSpeed * Time.deltaTime );
		EnforceBounds();
	}

	public void SetColliderForSprite( int spriteNum )
	{
		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}

	void OnTriggerEnter2D( Collider2D other )
	{
		if (other.CompareTag ("bubble")) {
			Destroy(other.gameObject);
			score += 5;
			scoreGT.text = "" + score;
		}
	}
	private void EnforceBounds()
	{
		Vector3 newPosition = transform.position; 
		Camera mainCamera = Camera.main;
		Vector3 cameraPosition = mainCamera.transform.position;
		
		float xDist = mainCamera.aspect * mainCamera.orthographicSize; 
		float xMax = cameraPosition.x + xDist;
		float xMin = cameraPosition.x - xDist;
		float yMax = mainCamera.orthographicSize;

		if ( newPosition.x < xMin || newPosition.x > xMax ) {
			newPosition.x = Mathf.Clamp( newPosition.x, xMin, xMax );
			moveDirection.x = -moveDirection.x;
		}

		if (newPosition.y < -yMax || newPosition.y > yMax) {
			newPosition.y = Mathf.Clamp( newPosition.y, -yMax, yMax );
			moveDirection.y = -moveDirection.y;
		}
		transform.position = newPosition;
	}
}
