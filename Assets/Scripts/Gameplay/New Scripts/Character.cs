﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public float m_health = 100.0f;
    public float m_moveSpeed = 4.0f;
    float m_turnSpeed;
    public float m_jumpForce = 4.0f;
    public float m_lerpSpeed = 10.0f;

    protected GameObjectGravity m_gravityOnCharacter;
    protected Rigidbody m_rigidBody;
    CapsuleCollider m_capsule;
    public float m_capsuleHeight;

    protected float m_groundCheckDistance;
    protected float m_defaultGroundCheckDistance = 0.3f;

    protected bool m_isGrounded;

    // Use this for initialization
    public virtual void Start ()
    {
        m_gravityOnCharacter = GetComponent<GameObjectGravity>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();
        m_capsuleHeight = m_capsule.height;

        m_rigidBody.freezeRotation = true;
	}

    // We use FixedUpdate since we will be dealing with forces
    // This method should control character's movements
    public virtual void FixedUpdate()
    {

	}

    // This function checks if the character is currently touching a collider below their "feet"
    public bool CheckGroundStatus()
    {
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position + (transform.up * 0.1f), transform.position + (transform.up * 0.1f) + (-transform.up * m_groundCheckDistance), Color.magenta);
        if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out hitInfo, m_groundCheckDistance))
        {
            m_gravityOnCharacter.GravityOnFeet(hitInfo);
            m_isGrounded = true;
        }
        else
        {
            m_isGrounded = false;
        }

        return m_isGrounded;
    }

    //This function rotates the character so its Vector.up aligns with the direction of the attractor's gravity
    public void UpdateUp()
    {
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, m_gravityOnCharacter.m_gravity) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, m_lerpSpeed * Time.fixedDeltaTime);
    }

    //This function deals with the jump of the character
    //It mainly adds a velocity to the rigidbody in the direction of the gravity.
    protected void Jump()
    {
        m_rigidBody.velocity += m_gravityOnCharacter.m_gravity * m_jumpForce;
        m_isGrounded = false;
        m_groundCheckDistance = 0.1f;
    }

    //This function should be called while character is on air.
    //It controls the detection of the floor. If the character is going up, the detection is small in order to avoid being unable to jump.
    protected void OnAir()
    {
        m_groundCheckDistance = Vector3.Dot(m_rigidBody.velocity, transform.up) < 0 ? m_defaultGroundCheckDistance : 0.01f;
    }
}