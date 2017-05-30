﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChanging : PlayerStates
{
    public float m_maxTimeChanging = 1.0f;

    float m_timeChanging;
    Quaternion m_initialRotation;
    Quaternion m_finalRotation;

    public override void Start()
    {
        base.Start();
        m_timeChanging = 0.0f;
        m_type = States.CHANGING;
    }

    //Main player update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool jumping, bool pickObjects, bool aimGravity, bool changeGravity, bool aimingObject, bool throwing, float timeStep)
    {
        bool ret = false;

        m_timeChanging += timeStep;
        float perc = m_timeChanging / m_maxTimeChanging;
        transform.rotation = Quaternion.Lerp(m_initialRotation, m_finalRotation, perc);

        if (m_timeChanging > m_maxTimeChanging)
        {
            m_player.m_currentState = m_player.m_onAir;
            ret = true;
        }

        return ret;
    }

    public override void OnEnter()
    {
        m_rigidBody.isKinematic = true;
        m_initialRotation = transform.rotation;
        m_finalRotation = Quaternion.FromToRotation(transform.up, m_player.m_gravityOnCharacter.GetGravityVector()) * transform.rotation;
    }

    public override void OnExit()
    {
        m_rigidBody.isKinematic = false;
        m_timeChanging = 0.0f;
    }
}
