﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowing : EnemyStates {
	public GameObject player;
	int speed;
	public bool canChange = true;
	public GameObject wallToChange;
	public Vector3 up;
	float radiusCollider;
	public bool x;
	public bool y;
	public bool z;
	public Vector3 bUp; //a boolean vector

	// Use this for initialization
	public override void Start () {
		base.Start ();
		x = false;
		y = false;
		z = false;
	}
	
	// Update is called once per frame
	public override bool OnUpdate () {
		up = transform.up;
		bool ret = false;

		if (player != null) 
		{
			if (canChange)
				Move ();
			else if (player.transform.up.y >= transform.up.y - 0.1f && player.transform.up.y <= transform.up.y + 0.1f)
				Move ();
			else if (wallToChange != null) 
			{
			}
		}
		return ret;
	}

	public override void OnEnter()
	{
		m_type = States.FOLLOWING;

		player = m_enemy.player;
		speed = m_enemy.m_speed;

		//m_rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		/*if (player.transform.up.x > 0.5f)
			m_rigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
		if (player.transform.up.y)
			m_rigidBody.constraints |= RigidbodyConstraints.FreezeRotationY;
		if (player.transform.up.z )
			m_rigidBody.constraints |= RigidbodyConstraints.FreezeRotationZ;*/
		if (player.transform.up.x > 0.5f || player.transform.up.x < -0.5f)
			x = true;
		if (player.transform.up.y > 0.5f || player.transform.up.y < -0.5f)
			y = true;
		if (player.transform.up.z > 0.5f || player.transform.up.z < -0.5f)
			z = true;

		bUp = new Vector3 (transform.up.x , transform.up.y, transform.up.z);

		radiusCollider = m_enemy.GetComponent<SphereCollider> ().radius;
		m_enemy.GetComponent<SphereCollider> ().radius = 0;
	}

	public override void OnExit()
	{
		m_enemy.GetComponent<SphereCollider> ().radius = radiusCollider;
		x = false;
		y = false;
		z = false;
	}

	public void Move()
	{
		Vector3 target = player.transform.position;
		/*if (x)
		  	target.x = transform.position.x;
		if (y)
			target.y = transform.position.y;
		if (z)
			target.z = transform.position.z;*/
		//transform.LookAt (target);

		Vector3 difference = target - transform.position;
		if (x) 
		{
			float rotationX = Mathf.Atan2 (difference.y, difference.z) * Mathf.Rad2Deg;
			if (transform.up.x < 0)
				rotationX *= -1;
			transform.rotation = Quaternion.Euler (rotationX , m_enemy.currentWall.transform.rotation.eulerAngles.y, m_enemy.currentWall.transform.rotation.eulerAngles.z);
		}
		else if (y) 
		{
			float rotationY = Mathf.Atan2 (difference.x, difference.z) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (m_enemy.currentWall.transform.rotation.eulerAngles.x, rotationY, m_enemy.currentWall.transform.rotation.eulerAngles.z);
		}
		else if (z) 
		{
			float rotationZ = Mathf.Atan2 (difference.x, difference.z) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (m_enemy.currentWall.transform.rotation.eulerAngles.x, m_enemy.currentWall.transform.rotation.eulerAngles.y, rotationZ);
		}

		transform.position += transform.forward * speed * Time.deltaTime;
		//if (y)
		//	transform.up = new Vector3(transform.up.x, transform.up.y, transform.up.z);
	}
}
