﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {
	public BossStates m_currentState;
	[HideInInspector] public BossStates m_Idle;
	[HideInInspector] public BossStates m_Attack;

	public Animator m_animator;
	public int m_phase;
	public float m_attackRate;

	public GameObject player;
	void Awake()
	{
		m_animator = gameObject.GetComponent<Animator> ();

		m_Idle = gameObject.GetComponent<BossIdle> ();
		if (!m_Idle)
			m_Idle = gameObject.AddComponent<BossIdle> ();

		m_Attack = gameObject.GetComponent<BossAttack> ();
		if (!m_Attack)
			m_Attack = gameObject.AddComponent<BossAttack> ();

		m_currentState = m_Idle;
	}

	void Start () {
		player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		BossStates previousState = m_currentState;
		if (m_currentState.OnUpdate ()) 
		{
			previousState.OnExit ();
			m_currentState.OnEnter ();
		}
	}

	void OnCollisionEnter(Collision col)
	{
		
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.gameObject.GetComponent<Player> ().m_damageData.m_recive = true;
			col.gameObject.GetComponent<Player> ().m_damageData.m_damage = 20;
		}
			
	}
}