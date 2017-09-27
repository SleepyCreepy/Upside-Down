﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSounds
{
    private AudioClip[] m_footsteeps;
    private AudioClip m_jump;
    private AudioClip m_fall;

    private int m_index = 0;

    public TerrainSounds(string path)
    {
        m_footsteeps = Resources.LoadAll<AudioClip>(PlayerSoundEffects.m_resourcesPath + PlayerSoundEffects.m_footsteepPath + path + "/");
        m_jump = Resources.LoadAll<AudioClip>(PlayerSoundEffects.m_resourcesPath + PlayerSoundEffects.m_jumpPath + path + "/")[0];
        m_fall = Resources.LoadAll<AudioClip>(PlayerSoundEffects.m_resourcesPath + PlayerSoundEffects.m_fallPath + path + "/")[0];
    }

    public AudioClip GetFootSteep()
    {
        int index = m_index;
        while(index == m_index)
        {
            m_index = Random.Range(0, m_footsteeps.Length);
        }
        return m_footsteeps[m_index];
    }

    public AudioClip GetJump()
    {
        return m_jump;
    }

    public AudioClip GetFall()
    {
        return m_fall;
    }
}

public class PlayerSoundEffects : SoundEffects {

    public static string m_resourcesPath = "Audio/Player/";
    public static string m_footsteepPath = "Footsteeps/";
    public static string m_jumpPath = "Jumps/";
    public static string m_fallPath = "Fall/";
    
    private string[] m_terrains = new string[] { "Snow", "Cloud" };
    private Dictionary<string, TerrainSounds> m_fxSounds = new Dictionary<string, TerrainSounds>();

    private int m_terrainIndex = 0;

    void Start()
    {
        for(int i = 0; i < m_terrains.Length; i++)
        {
            m_fxSounds.Add(m_terrains[i], new TerrainSounds(m_terrains[i]));
        }
    }

    public void PlayFootStep()
    {
        Debug.Log(m_terrainIndex);
        base.PlaySound(m_fxSounds[m_terrains[m_terrainIndex]].GetFootSteep());
    }

    public void PlayJump()
    {
        Debug.Log(m_terrainIndex);
        base.PlaySound(m_fxSounds[m_terrains[m_terrainIndex]].GetJump());
    }

    public void PlayFall()
    {
        Debug.Log(m_terrainIndex);
        base.PlaySound(m_fxSounds[m_terrains[m_terrainIndex]].GetFall());
    }

    public void CloudEnter(bool enter)
    {
        m_terrainIndex = enter ? 1 : 0;
    }

}
