using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TogglePrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    //public ARPlaneManager planeManager;
    private ARPlaneManager planeManager;
    public GameObject planePrefab;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }
    public void TogglePlanePrefab()
    {
        if (planePrefab != null)
        {
                if (planeManager.planePrefab == null)
                {
                    planeManager.planePrefab = planePrefab;
                }
                else
                {
                    planeManager.planePrefab = null;
                }
        }
    }
}
