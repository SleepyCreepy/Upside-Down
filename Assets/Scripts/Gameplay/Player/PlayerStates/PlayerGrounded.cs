﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : PlayerStates
{

    public float m_floatingHeight = 1.0f;

    public override void Start()
    {
        base.Start();
        m_type = States.GROUNDED;
    }

    //Main player update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool jumping, bool pickObjects, bool aimGravity, bool changeGravity, bool aimingObject, bool throwing, float timeStep)
    {
        bool ret = false;

        if (axisHorizontal == 0.0f && axisVertical == 0.0f)
            m_player.m_playerStopped = true;
        else
            m_player.m_playerStopped = false;

        RaycastHit target;
        bool hasTarget = m_player.SetMarkedTarget(out target);

        //if (aimGravity)
        //{
        //    m_player.m_currentState = m_player.m_floating;
        //    m_player.SetFloatingPoint(m_floatingHeight);
        //    ret = true;
        //}
        if (changeGravity)
        {
            //if (hasTarget)
            //{
            //    m_player.m_playerGravity.ChangeGravityTo(target);
            //    m_player.m_gravityOnCharacter.ChangeToAttractor();
            //    m_player.m_rigidBody.velocity = Vector3.zero;
            //    m_player.m_freezeMovement = true;
            //    //m_player.m_currentState = m_player.m_changing;
            //    m_player.m_currentState = m_player.m_onAir;
            //    ret = true;
            //}
            //else
                m_player.m_gravityOnCharacter.SwapPlanetAttractor();
        }
        else if (jumping)
        {
            m_player.m_currentState = m_player.m_onAir;
            m_player.Jump();
            ret = true;
        }
        else if (aimingObject && m_player.m_floatingObjects.HasObjectsToThrow())
        {
            m_player.m_currentState = m_player.m_aimToThrow;
            ret = true;
        }
        else
        {
            m_player.UpdateUp();
            m_player.Move(timeStep);
            m_player.ThrowObjectsThirdPerson(throwing);
            if (pickObjects)
            {
                m_player.PickObjects();
            }
            if (!m_player.CheckGroundStatus())
            {
                m_player.m_currentState = m_player.m_onAir;
                ret = true;
            }
        }   

        return ret;
    }

    public override void OnEnter()
    {
        m_player.m_markAimedObject = true;
        m_player.m_rotationFollowPlayer = true;
        m_player.m_reachedGround = true;
        m_player.m_freezeMovement = false;
		EffectsManager.Instantiate (m_player.m_jumpClouds, transform.position, transform.rotation * m_player.m_jumpClouds.transform.rotation);
	    m_player.m_runClouds.GetComponent<ParticleSystem> ().Play();
        m_player.m_jumpDirection = Vector3.zero;
        m_player.m_rigidBody.velocity = Vector3.zero;
    }

    public override void OnExit()
    {
		m_player.m_runClouds.GetComponent<ParticleSystem> ().Stop();
        m_player.m_floatingObjects.UnsetTarget();
    }
}
