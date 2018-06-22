using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Gestures.UnitySdk;
using Microsoft.Gestures.Toolkit;


public class HandChanger : MonoBehaviour {

    public GameObject handRight_Open;
    public GameObject handRight_Closed;
    public GameObject handRight_Point;
    public GameObject handRight_Peace;
    public GameObject handRight_Knarly;

    public GameObject handLeft_Open;
    public GameObject handLeft_Closed;
    public GameObject handLeft_Point;
    public GameObject handLeft_Peace;
    public GameObject handLeft_Knarly;

    public GameObject wrist_Right;
    public GameObject wrist_Left;
    private Quaternion wristR_Default;
    private Quaternion wristL_Default;
    private Quaternion wristR_GestDir;

    public GameObject infoCube;
    private CubeDisplay cubeDisplay;

    public AudioClip handSound;
    private AudioSource source;
    bool closedSound = false;

    // Use this for initialization
    void Start () {

        SetAllRightInactive();
        SetRightToOpen();

        SetAllLeftInactive();
        SetLeftToOpen();

        source = GetComponent<AudioSource>();
        source.volume = 0.35f;

        cubeDisplay=infoCube.GetComponent <CubeDisplay>();

        wristR_Default = wrist_Right.transform.localRotation;
        wristL_Default = wrist_Left.transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        var skeleton = GesturesManager.Instance.SmoothDefaultSkeleton;
        if (skeleton == null)
        {
            return;
        }
        wristR_GestDir = Quaternion.Euler(skeleton.PalmOrientation);
	}

    public void SetRightToOpen()
    {
        SetAllRightInactive();
        handRight_Open.SetActive(true);
        closedSound = false;
        wrist_Right.transform.localRotation = wristR_Default;
    }

    public void SetRightToClosed()
    {
        SetAllRightInactive();
        handRight_Closed.SetActive(true);
        if (!closedSound)
        {
            source.PlayOneShot(handSound);
            closedSound = true;
        }
    }

    public void SetRightToPoint()
    {
        SetAllRightInactive();
        handRight_Point.SetActive(true);
        cubeDisplay.forward = true;
    }

    public void SetRightToPeace()
    {
        SetAllRightInactive();
        handRight_Peace.SetActive(true);
        cubeDisplay.backward = true;
    }

    public void SetLeftToOpen()
    {
        SetAllLeftInactive();
        handLeft_Open.SetActive(true);
        closedSound = false;
        wrist_Left.transform.localRotation = wristL_Default;
    }

    public void SetLeftToClosed()
    {
        SetAllLeftInactive();
        handLeft_Closed.SetActive(true);
        if (!closedSound)
        {
            source.PlayOneShot(handSound);
            closedSound = true;
        }
    }

    public void SetLeftToPoint()
    {
        SetAllLeftInactive();
        handLeft_Point.SetActive(true);
        cubeDisplay.forward = true;
    }

    public void SetLeftToPeace()
    {
        SetAllLeftInactive();
        handLeft_Peace.SetActive(true);
        cubeDisplay.backward = true;
    }

    private void SetAllRightInactive()
    {
        handRight_Closed.SetActive(false);
        handRight_Point.SetActive(false);
        handRight_Peace.SetActive(false);
        handRight_Knarly.SetActive(false);
        handRight_Open.SetActive(false);
    }

    private void SetAllLeftInactive()
    {
        handLeft_Closed.SetActive(false);
        handLeft_Point.SetActive(false);
        handLeft_Peace.SetActive(false);
        handLeft_Knarly.SetActive(false);
        handLeft_Open.SetActive(false);
    }

    private void OnEnable()
    {
        // Register to skeleton events
        GesturesManager.Instance.RegisterToSkeleton();
    }

    private void OnDisable()
    {
        // Unregister from skeleton events
        GesturesManager.Instance.UnregisterFromSkeleton();
    }

}
