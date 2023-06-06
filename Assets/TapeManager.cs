using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;

public class TapeManager : MonoBehaviour
{
    ARRaycastManager arrayManager;

    public GameObject[] tapePoints;

    public GameObject reticle;

    public float distance = 0f;

    int currentIndexPoint = 0;

    bool placementEnabled = true;

    bool canPlaceTapePoint = false;

    public TMP_Text displayText;

    public TMP_Text floatingDisplayText;
    public GameObject floatingDistanceObject;

    public LineRenderer line;

    public GameObject planeWarningObject;
    public TMP_Text planeWarningText;

    public TMP_Dropdown dropdown;

    private ARPlaneManager planeManager;
    public GameObject planePrefab;



    Unit currentUnit = Unit.cm;

    // Start is called before the first frame update
    void Start()
    {
        arrayManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistance();
        PlaceFloatingText();

        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        arrayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);


        if (hits.Count > 0)
        {
            planeWarningObject.SetActive(false);

            reticle.transform.position = hits[0].pose.position;
            reticle.transform.rotation = hits[0].pose.rotation;

            //draw the line to the reticle if the first point is placed
            if (currentIndexPoint == 1)
            {
                DrawLine();
            }

            // enable the reticle if its disabled and the tape points aren't placed
            if (!reticle.activeInHierarchy && currentIndexPoint < 2)
            {
                reticle.SetActive(true);
            }

            //if the user taps, place a tape point. disable more placements until the end of the touch
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !canPlaceTapePoint)
            {
                if (currentIndexPoint < 2)
                {
                    PlacePoint(hits[0].pose.position, currentIndexPoint);
                }
                placementEnabled = false;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                placementEnabled = true;
            }
        }

        //if the raycast isn't hitting anything, don't display the reticle
        else if (hits.Count == 0 || currentIndexPoint == 2)
        {
            reticle.SetActive(false);
            planeWarningObject.SetActive(true);
        }

        if(currentIndexPoint == 2)
        {
            dropdown.gameObject.SetActive(true);
        }
        else
        {
            dropdown.gameObject.SetActive(false);
        }
    }

    public void PlacePoint(Vector3 pointPosition, int pointIndex)
    {
        tapePoints[pointIndex].SetActive(true);

        tapePoints[pointIndex].transform.position = pointPosition;

        currentIndexPoint = currentIndexPoint + 1;
    }

    void UpdateDistance()
    {
        if (currentIndexPoint == 0)
        {
            distance = 0f;
        }
        else if (currentIndexPoint == 1)
        {
            distance = Vector3.Distance(tapePoints[0].transform.position, reticle.transform.position);
        }
        else if (currentIndexPoint == 2)
        {
            distance = Vector3.Distance(tapePoints[0].transform.position, tapePoints[1].transform.position);
        }

        //convert units
        float convertedDistance = 0f;

        switch (dropdown.value)
        {
            case 0: //cm
                currentUnit = Unit.cm;
                convertedDistance = distance * 100;
                break;
            case 1: //m
                currentUnit = Unit.m;
                convertedDistance = distance;
                break;
            case 2: //in
                currentUnit = Unit.@in;
                convertedDistance = distance / 0.0254f;
                break;
            case 3: //ft
                currentUnit = Unit.@ft;
                convertedDistance = distance * 3.2808f;
                break;
            default:
                break;
        }

        if (distance > 0f)
        {
            //change the text to display the distance
            string distanceStr = convertedDistance.ToString("#.##") + currentUnit;

            displayText.text = distanceStr;
            floatingDisplayText.text = distanceStr;
        }
        else
        {
            displayText.text = "0" + currentUnit;
        }

    }

    //set the positions of the line to the tape points (or reticle)
    void DrawLine()
    {
        line.enabled = true;
        line.SetPosition(0, tapePoints[0].transform.position);
        //updating
        if (currentIndexPoint == 1)
        {
            line.SetPosition(1, reticle.transform.position);

        }
        //final input
        else if (currentIndexPoint == 2)
        {
            line.SetPosition(1, tapePoints[1].transform.position);

        }
    }

    void PlaceFloatingText()
    {
        RectTransform floatingRectTransform = floatingDisplayText.GetComponent<RectTransform>();

        if (currentIndexPoint == 0)
        {
            floatingDistanceObject.SetActive(false);
        }
        else if (currentIndexPoint == 1)
        {
            floatingDistanceObject.SetActive(false);
            floatingRectTransform.anchoredPosition = Vector3.Lerp(tapePoints[0].transform.position, reticle.transform.position, 0.5f);
            //floatingDisplayText.transform.position = Vector3.Lerp(tapePoints[0].transform.position, reticle.transform.position, 0.5f);
        }
        else if (currentIndexPoint == 2)
        {
            floatingDistanceObject.SetActive(false);
            floatingRectTransform.anchoredPosition = Vector3.Lerp(tapePoints[0].transform.position, tapePoints[1].transform.position, 0.5f);
            ///floatingDisplayText.transform.position = Vector3.Lerp(tapePoints[0].transform.position, tapePoints[1].transform.position, 0.5f);
        }

        floatingDistanceObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

    }


    //casting string (from the inspector) into a Unit so we can act upon it
    public void ChangeUnit(string unit)
    {
        currentUnit = (Unit)System.Enum.Parse(typeof(Unit), unit);
    }
    public void DecrementCurrentIndexPoint()
    {
        if(currentIndexPoint == 2)
        {
            tapePoints[1].SetActive(false);
            currentIndexPoint -= 1;
            UpdateDistance();
        }
        else if(currentIndexPoint == 1) {
            tapePoints[0].SetActive(false);
            currentIndexPoint -= 1;
            UpdateDistance();
        }
    }

    // Set canPlaceTapePoint to false when button or dropdown is clicked
    public void OnButtonClick()
    {
        canPlaceTapePoint = true;
    }

    // Set canPlaceTapePoint to true when button or dropdown is not clicked
    public void OnButtonRelease()
    {
        canPlaceTapePoint = false;
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
    public void CaptureScreen()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = "meAR_" + timestamp + ".png";
        ScreenCapture.CaptureScreenshot(filename);
    }
    

}

public enum Unit
{
    cm,
    m,
    @in,
    @ft
}