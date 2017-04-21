﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the main class for the player. It will control the player input and character movement.
//It inherits from Character.
public class Player : Character
{

    //Variables regarding player input control
    PlayerController m_playerInput;
    float m_axisHorizontal;
    float m_axisVertical;
    float m_camHorizontal;
    float m_camVertical;
    bool m_jumping;
    bool m_changeGravity;
    bool m_throwObject;
    bool m_returnCam;

    //Variables regarding player state
    public PlayerStates m_currentState;
    public PlayerStates m_grounded;
    public PlayerStates m_onAir;
    public PlayerStates m_throwing;
    public PlayerStates m_floating;
    public PlayerStates m_changing;

    //Variables regarding player movement
    Transform m_modelTransform;
    public bool m_freezeMovementOnAir;
    public VariableCam m_camController;
    public bool m_rotationFollowPlayer;
    public bool m_playerStopped = false;
    public Vector3 m_offset = Vector3.zero;

    //Variables regarding player's change of gravity
    public float m_gravityRange = 10.0f;
    public GameObject m_gravitationSphere;
    public PlayerGravity m_playerGravity;
    public float m_maxTimeFloating = 30.0f;
    public float m_maxTimeChanging = 1.0f;
    public bool m_reachedGround = true;
    public bool m_changeButtonReleased = true;
    public float m_floatingHeight = 1.0f;

    //Variables regarding player's throw of objects
    public bool m_incresePowerWithTime = false;
    public float m_throwDetectionRange = 20.0f;
    public float m_maxTimeThrowing = 30.0f;
    public float m_throwStrengthPerSecond = 1.0f;
    public float m_throwStrengthOnce = 20.0f;
    public float m_objectsFloatingHeight = 1.0f;
    public bool m_throwButtonReleased = true;

    public Dictionary<string, TargetDetector> m_targetsDetectors;
    GameObject m_detectorsEmpty;

    public override void Awake()
    {
        m_grounded = gameObject.AddComponent<PlayerGrounded>();
        m_onAir = gameObject.AddComponent<PlayerOnAir>();
        m_floating = gameObject.AddComponent<PlayerFloating>();
        m_changing = gameObject.AddComponent<PlayerChanging>();
        m_throwing = gameObject.AddComponent<PlayerThrowing>();

        m_currentState = m_onAir;

        if (!(m_playerInput = GetComponent<PlayerController>()))
            m_playerInput = gameObject.AddComponent<PlayerController>();
        if (!(m_playerGravity = GetComponent<PlayerGravity>()))        
            m_playerGravity = gameObject.AddComponent<PlayerGravity>();

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        m_gravitationSphere = GameObject.Find("GravSphere");
        m_gravitationSphere.transform.localPosition = Vector3.zero + Vector3.up * capsuleCollider.height / 2;
        m_gravitationSphere.SetActive(false);

        GameObject cameraFree = GameObject.Find("MainCameraRig");
        if (cameraFree)
            m_camController = cameraFree.GetComponent<VariableCam>();
        m_rotationFollowPlayer = true;

        m_detectorsEmpty = GameObject.Find("TargetDetectors");
        if (!m_detectorsEmpty)
        {
            m_detectorsEmpty = new GameObject("TargetDetectors");
            m_detectorsEmpty.transform.parent = transform;
            m_detectorsEmpty.transform.localPosition = Vector3.zero;
        }    

        base.Awake();
    }

    // Use this for initialization
    public override void Start ()
    { 
        m_modelTransform = transform.FindChild("Model");

        m_freezeMovementOnAir = false;

        base.Start();

        m_targetsDetectors = new Dictionary<string, TargetDetector>();
        SetDetectors("Enemy", m_throwDetectionRange);
        SetDetectors("GravityWall", m_gravityRange);
    }

    public override void Restart()
    {
        m_currentState.OnExit();
        m_currentState = m_onAir;

        ResetInput();

        base.Restart();
    }

    // This method should control player movements
    // First, it should read input from PlayerController in Update, since we need input every frame
    public override void Update()
    {
        m_playerInput.GetDirections(ref m_axisHorizontal, ref m_axisVertical, ref m_camHorizontal, ref m_camVertical);
        m_playerInput.GetButtons(ref m_jumping, ref m_changeGravity, ref m_throwObject, ref m_returnCam);

        if (!m_changeGravity)
            m_changeButtonReleased = true;

        if (!m_throwObject)
            m_throwButtonReleased = true;

        m_playerStopped = false;

		PlayerStates previousState = m_currentState;
		if (m_currentState.OnUpdate(m_axisHorizontal, m_axisVertical, m_jumping, m_changeGravity, m_throwObject, Time.deltaTime))
		{
			previousState.OnExit();
			m_currentState.OnEnter();
		}

        if (m_camController)
        {
            m_camController.OnUpdate(m_camHorizontal, m_camVertical, m_returnCam, Time.deltaTime);
            if (m_rotationFollowPlayer)
                m_camController.RotateOnTarget(Time.fixedDeltaTime);
        }

        ResetInput();
    }

    // Second, it should update player state regarding the current state & input
    // We use FixedUpdate when we need to deal with physics
    // We also clean the input only after a FixedUpdate, so we are sure we have at least one FixedUpdate with the correct input recieved in Update
    public override void FixedUpdate ()
    {
        base.FixedUpdate();
        HUDManager.ChangeEnergyValue(base.m_health / base.m_maxHealth);
    }

    //This functions controls the character movement and the model orientation.
    //TODO: Probably we will need to change this function when we have the character's animations.
    public void Move(float timeStep)
    {
        Vector3 forward = Vector3.Cross(Camera.main.transform.right, transform.up);
        Vector3 movement = m_axisHorizontal * Camera.main.transform.right + m_axisVertical * forward;
        movement.Normalize();

        m_rigidBody.MovePosition(transform.position + m_offset + movement * m_moveSpeed * timeStep);
        m_offset = Vector3.zero;

        if (movement != Vector3.zero)
        {
            Quaternion modelRotation = Quaternion.LookRotation(movement, transform.up);
            m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, modelRotation, 10.0f * timeStep);
        }
    }

    public void SetFloatingPoint(float height)
    {
        PlayerFloating floating = (PlayerFloating)m_floating;

        floating.m_floatingPoint = transform.position + transform.up * height;

        m_groundCheckDistance = 0.1f;
    }

    //public void Move(float timeStep)
    //{
    //    Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1));
    //    Vector3 movement = - m_axisHorizontal * Camera.main.transform.right + m_axisVertical * camForward;
    //    movement.Normalize();
    //    //movement = transform.InverseTransformDirection(movement);

    //    m_rigidBody.MovePosition(transform.position + movement * m_moveSpeed * timeStep);

    //    //if (movement != Vector3.zero)
    //    //{
    //    //    Quaternion modelRotation = Quaternion.LookRotation(movement, transform.up);
    //    //    m_modelTransform.rotation = Quaternion.Lerp(m_modelTransform.rotation, modelRotation, 10.0f * timeStep);
    //    //}
    //}

    void OnCollisionEnter(Collision col)
	{
        if(col.collider.tag == "Liquid")
        {
            base.m_damageRecive = true;
            base.m_damagePower = 20;
            base.m_respawn = true;
        }

		if (col.collider.tag.Contains ("Enemy")) 
		{
			//if (col.gameObject.GetComponent<Enemy> ().m_animator.GetCurrentAnimatorStateInfo (0).IsName ("Attack")) 
			if (col.transform.GetComponentInParent<Enemy> ().m_animator.GetCurrentAnimatorStateInfo (0).IsName ("Attack"))
			{
				base.m_damageRecive = true;
				base.m_damagePower = 20;
			}
		}

        if(col.collider.tag == "HarmfulObject")
        {
            base.m_damageRecive = true;
            base.m_damagePower = 20;
        }
    }

    void SetDetectors(string tagName, float radiusCollider)
    {
        string objectName = string.Concat(tagName, "Detector");
        if (!m_targetsDetectors.ContainsKey(tagName))
        {
            GameObject newObject = new GameObject(objectName);
            newObject.transform.parent = m_detectorsEmpty.transform;
            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localRotation = Quaternion.identity;
            newObject.transform.localScale = Vector3.one;

            newObject.AddComponent<SphereCollider>();
            TargetDetector newDetector = newObject.AddComponent<TargetDetector>();
            newDetector.SetUpCollider(tagName, new Vector3(0, m_capsuleHeight / 2, 0), radiusCollider);
            m_targetsDetectors.Add(tagName, newDetector);
        }
    }

    void ResetInput()
    {
        m_axisHorizontal = 0.0f;
        m_axisVertical = 0.0f;
        m_camHorizontal = 0.0f;
        m_camVertical = 0.0f;
        m_jumping = false;
        m_changeGravity = false;
        m_throwObject = false;
        m_returnCam = false;
    }
}
