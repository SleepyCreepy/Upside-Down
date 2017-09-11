﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public bool m_canBePicked = true;
    public bool m_isFloating = false;
    public bool m_isCarring = false;
    public float m_floatingSpeed = 10.0f;
    public float m_rotationSpeed = 100.0f;
    public GameObject m_aura;
    public Vector3 m_chargingPivot = Vector3.zero;
    public float m_timeToRotate = 2.0f;

    GameObjectGravity m_objectGravity;
    Rigidbody m_rigidBody;
    Collider[] m_collider;

    Transform m_floatingPoint;


    FloatingAroundPlayer m_targetPlayer = null;
    PickedObject m_playerPicked = null;

    TrailRenderer m_trail;


    [HideInInspector] public bool m_canDamage = false;
    bool m_applyThrownForce = false;
    bool m_movingHorizontal = false;
    float m_thrownForce = 0.0f;
    float m_horizontalThrownForce = 1.0f;
    Vector3 m_vectorUp = Vector3.up;
    Vector3 m_vectorFroward = Vector3.forward;
    float m_minVelocityDamage = 2.0f;
    Vector3 m_rotationRandomVector = Vector3.zero;

    float m_timeRotating = 0.0f;

    public GameObject m_prefabHit1;

    // Use this for initialization
    void Start()
    {
        m_objectGravity = GetComponent<GameObjectGravity>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_collider = GetComponentsInChildren<Collider>();
        m_trail = GetComponent<TrailRenderer>();

        m_rigidBody.freezeRotation = true;

        m_prefabHit1 = (GameObject)Resources.Load("Prefabs/Effects/CFX3_Hit_Misc_D (Orange)", typeof(GameObject));
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_movingHorizontal && !m_rigidBody.freezeRotation)
        {
            m_timeRotating += Time.deltaTime;
            if (m_timeRotating >= m_timeToRotate)
            {
                m_rigidBody.freezeRotation = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (m_movingHorizontal)
        {
            m_rigidBody.MovePosition(m_rigidBody.position + m_vectorFroward * Time.deltaTime * m_horizontalThrownForce);
        }
        if (m_applyThrownForce)
        {
            //transform.parent = null;
            //m_playerPicked.FreeSpace();

            m_playerPicked = null;
            m_rigidBody.velocity = m_vectorUp * m_thrownForce;
            //m_rigidBody.AddForce(m_thrownForce * m_rigidBody.mass, ForceMode.Impulse);
            m_applyThrownForce = false;
            m_movingHorizontal = true;
        }
    }

    //This function should be called when the object is thrown
    public void ThrowObject(float throwForce, float horizontalThrowForce, Vector3 up, Vector3 forward)
    {
        if (m_playerPicked)
            StopCarried();
        else if (m_targetPlayer)
            StopFloating();

        m_rigidBody.freezeRotation = false;
        m_thrownForce = throwForce;
        m_horizontalThrownForce = horizontalThrowForce;
        m_vectorUp = up;
        m_vectorFroward = forward;
        m_applyThrownForce = true;
        m_canDamage = true;

        //m_objectGravity.m_ignoreGravity = true;
        if (m_trail)
            m_trail.enabled = true;
    }

    public void ThrowObjectNow()
    {
        if (m_applyThrownForce)
        {
            transform.parent = null;
            m_playerPicked.FreeSpace();
            m_playerPicked = null;

            m_rigidBody.velocity = m_vectorUp * m_thrownForce;
            //m_rigidBody.AddForce(m_thrownForce * m_rigidBody.mass, ForceMode.Impulse);
            m_applyThrownForce = false;
            m_movingHorizontal = true;
        }
    }

    // This function should be called when an object begins to float around the character
    public void BeginFloating(Transform floatingPoint, FloatingAroundPlayer player, float timeFloating = 0.0f)
    {
        m_floatingPoint = floatingPoint;
        m_targetPlayer = player;

        m_isFloating = true;
        m_canBePicked = false;

        if (m_rigidBody)
            m_rigidBody.isKinematic = true;

        for (int i = 0; i < m_collider.Length; i++)
        {
            m_collider[i].enabled = false;
        }

        m_rotationRandomVector = new Vector3(Random.value, Random.value, Random.value).normalized;

        if (m_aura != null)
            m_aura.SetActive(true);
    }

    // This function is called when the object is picked by the player
    public void BeginCarried(Transform floatingPoint, PickedObject player)
    {
        transform.parent = floatingPoint;
        Enemy enemy = GetComponent<Enemy>();
        if (enemy) {
            transform.forward = m_vectorUp;
            enemy.m_animator.SetBool("Charged", true);
            enemy.enabled = false;
        } else {
            transform.up = m_vectorUp;
        }
        transform.position = floatingPoint.position;
        transform.Translate(m_chargingPivot);
        m_floatingPoint = floatingPoint;
        m_playerPicked = player;

        m_isCarring = true;
        m_canBePicked = false;

        m_objectGravity.m_ignoreGravity = true;
        if (m_rigidBody)
            m_rigidBody.isKinematic = true;
        for (int i = 0; i < m_collider.Length; i++)
        {
            m_collider[i].enabled = false;
        }

        if (m_aura)
            m_aura.SetActive(true);
    }

    //This function should be called when an object stop floating around the character
    public void StopFloating()
    {
        transform.parent = null;
        m_targetPlayer.FreeSpace(m_floatingPoint);
        m_targetPlayer = null;
        m_floatingPoint = null;

        m_isFloating = false;
        m_canBePicked = true;

        if (m_rigidBody)
            m_rigidBody.isKinematic = false;

        for (int i = 0; i < m_collider.Length; i++)
        {
            m_collider[i].enabled = true;
        }

        m_rotationRandomVector = Vector3.zero;
        if (m_aura != null)
            m_aura.SetActive(false);
    }

    //This function should be called when the object stop been carried by the player
    public void StopCarried()
    {
        transform.parent = null;
        m_playerPicked.FreeSpace();
        m_playerPicked = null;
        m_floatingPoint = null;

        m_isCarring = false;
        m_canBePicked = true;

        m_objectGravity.m_ignoreGravity = false;
        if (m_rigidBody)
            m_rigidBody.isKinematic = false;

        for (int i = 0; i < m_collider.Length; i++)
        {
            m_collider[i].enabled = true;
        }

        if (m_aura != null)
            m_aura.SetActive(false);
    }

    void OnCollisionEnter(Collision col)
    {
        SoundEffects sound = GetComponent<SoundEffects>();
        if (sound != null)
        {
            sound.PlaySound("HitSomething");
			if(transform.tag != "EnemySnail")
				EffectsManager.Instance.GetEffect(m_prefabHit1, col.transform.position, transform.up, null);
        }
        int floor = LayerMask.NameToLayer("Floor");
        int water = LayerMask.NameToLayer("HarmfulTerrain");
        int terrain = LayerMask.NameToLayer("Terrain");
        if (col.collider.gameObject.layer == terrain || col.collider.gameObject.layer == water || col.collider.gameObject.layer == floor)
        {
            if (m_movingHorizontal)
            {
                Enemy enemy = GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.m_animator.SetBool("Charged", false);
                    enemy.enabled = true;
                    enemy.FallDamage(col.collider.gameObject.layer == water);
                }
                m_rigidBody.velocity = Vector3.zero;
                m_movingHorizontal = false;
                m_canDamage = false;
                if (m_trail)
                    m_trail.enabled = false;
            }
        }
    }


}
