﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class IsometricCamera : MonoBehaviour {

	public GameObject player;
	public GameObject[] lerpPoints;
	public int actualPoint;

	public int zoom = 2;
	public int normal = 60;
	public float m_turnSpeed = 2f;

	//private bool isZoomed = false;

	float m_lookAngleX;
	float m_lookAngleY;
	float m_tiltAngle;
	float m_tiltMin = 45f;
	float m_tiltMax = 60f;

	public float FOV;
	// Update is called once per frame
	void Update () {
		float x = CrossPlatformInputManager.GetAxis ("Mouse X") * 2.5f;
		float y = CrossPlatformInputManager.GetAxis ("Mouse Y") * 2.5f;

		//m_lookAngleX += x * m_turnSpeed;
		//m_lookAngleY += y * m_turnSpeed;
		m_lookAngleX = Mathf.Lerp(m_lookAngleX, x, m_turnSpeed * Time.deltaTime);
		m_lookAngleY = Mathf.Lerp(m_lookAngleY, y, m_turnSpeed * Time.deltaTime);

		transform.LookAt (player.transform.position + transform.right * m_lookAngleX + transform.up * m_lookAngleY, player.transform.up);
		if(actualPoint != -1 && transform.parent == null)
			transform.position = Vector3.Lerp (transform.position, lerpPoints [actualPoint].transform.position, Time.deltaTime);

		FOV = normal - Vector3.Distance (transform.position, player.transform.position) * zoom + 10;
		GetComponent<Camera> ().fieldOfView = FOV;

		/*float x = CrossPlatformInputManager.GetAxis("Mouse X");
		float y = CrossPlatformInputManager.GetAxis("Mouse Y");
		m_lookAngle += x * m_turnSpeed;
		m_tiltAngle -= y * m_turnSpeed;
		m_tiltAngle = Mathf.Clamp(m_tiltAngle, -m_tiltMin, m_tiltMax);

		Quaternion targetRotation = Quaternion.Euler(m_lookAngle * Vector3.up);
		Quaternion tiltRotation = Quaternion.Euler(m_tiltAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

		transform.rotation = targetRotation * tiltRotation;*/
	}
}
