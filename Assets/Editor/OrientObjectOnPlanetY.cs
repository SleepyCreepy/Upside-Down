﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OrientObjectOnPlanetY : ScriptableWizard
{
    
    [MenuItem ("Additional Tools/Align Y radially with father")]
    static void AlignWithFather()
    {
        foreach (GameObject child in Selection.gameObjects)
        {
            Transform parentTransform = child.transform.parent;
            if (parentTransform.tag == "Planet")
            {
                Vector3 localPosition = child.transform.localPosition;

                Vector3 radialPosition = child.transform.position - parentTransform.position;
                Quaternion targetRotation = Quaternion.FromToRotation(child.transform.up, radialPosition.normalized);
                child.transform.rotation = targetRotation * child.transform.rotation;

                child.transform.localPosition = localPosition;
            }
        }
    }

   
	
}