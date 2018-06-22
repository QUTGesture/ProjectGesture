using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class RobotRawView : MonoBehaviour {

    public Material BoneMaterial;
    public GameObject BodyManager;

    public GameObject myRobot;
    public GameObject botHead;
    public GameObject botTorso;
    public GameObject botShoulderRight;
    public GameObject botElbowRight;
    public GameObject botShoulderLeft;
    public GameObject botElbowLeft;


    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private KinectBodyData _BodyManager;
    public string avatarAssetName;


    // Public function so other scripts can send out the data
    public Dictionary<ulong, GameObject> GetData()
    {
        return _Bodies;
    }

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };


    // Robot Joints - not used as easier to debug with direct calls.

    private Dictionary<string, string> _RigMap = new Dictionary<string, string>()
    {
        {"SpineBase", "nurbsCircle3/Pelvis"},
        //Legs not added
        {"SpineMid", "nurbsCircle3/Pelvis/Abdomine"},
        {"SpineShoulder", "nurbsCircle3/Pelvis/Abdomine/Torso"},
        {"ShoulderLeft", "nurbsCircle3/Pelvis/Abdomine/Torso/ShoulderBallRight"},
        {"ShoulderRight", "nurbsCircle3/Pelvis/Sbdomine/Torso/ShoulderBallLeft"},
        {"ElbowLeft", "nurbsCircle3/Pelvis/Abdomine/Torso/ShoulderBallRight/SholderRight/ElbowRight"},
        {"ElbowRight", "nurbsCircle3/Pelvis/Sbdomine/Torso/ShoulderBallLeft/ShoulderLeft/ElbowLeft"},
        {"WristLeft", "nurbsCircle3/Pelvis/Abdomine/Torso/ShoulderBallRight/SholderRight/ElbowRight/WristRightOpen"},
        {"WristRight", "nurbsCircle3/Pelvis/Sbdomine/Torso/ShoulderBallLeft/ShoulderLeft/ElbowLeft/WristLeft"},
        //Hands not added
        {"Neck", "nurbsCircle3/Pelvis/Abdomine/Torso/Neck"},
        {"Head", "nurbsCircle3/Pelvis/Abdomine/Torso/Neck/Head"},

    };

    private Dictionary<string, Quaternion> _RigMapOffsets = new Dictionary<string, Quaternion>()
    {
        {"SpineBase", Quaternion.Euler(0.0f,0.0f, 0.0f)},
        //Legs not add
        {"SpineMid",Quaternion.Euler(0.0f, 0.0f, 0.0f)},
        {"SpineShoulder",Quaternion.Euler(0.0f, 0.0f, 0.0f)},
        {"ShoulderLeft", Quaternion.Euler(0.0f, 90.0f, 0.0f)},
        {"ShoulderRight", Quaternion.Euler(0.0f, -90.0f, 0.0f)},
        {"ElbowLeft", Quaternion.Euler(0.0f, 180.0f, 0.0f)},
        {"ElbowRight", Quaternion.Euler(0f, -180.0f, 0.0f)},
        {"WristLeft", Quaternion.Euler(0.0f, 90.0f, 0.0f)},
        {"WristRight", Quaternion.Euler(0.0f, -90.0f, 0.0f)},
        //Hands not added
        {"Neck", Quaternion.Euler(0.0f, 0.0f, 0.0f)},
        {"Head", Quaternion.Euler(0.0f, 0.0f, 0.0f)},

    };

    
    void Update()
    {
        if (BodyManager == null)
        {
            return;
        }

        _BodyManager = BodyManager.GetComponent<KinectBodyData>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
 
        
        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                //Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    //code for creating multiple avatars if desired.
                    //_Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId); 
                }

                RefreshRobot(body);
                //Avatar refresh if used
                //RefreshBodyObject(body, _Bodies[body.TrackingId]);
                //SetAvatarScale(_Bodies[body.TrackingId]);
            }

        }
        

    }

    //Not useed - for general avatar creation.
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = new GameObject();

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;

        }
        // Add avatar gameobject from source
        GameObject avatar = Instantiate(Resources.Load(avatarAssetName, typeof(GameObject))) as GameObject;
        avatar.transform.parent = body.transform;
        avatar.name = "Avatar";
        //avatar.transform.parent = body.transform.Find("SpineBase");
        return body;
    }

    //Not used - for general avatar scale.
    private void SetAvatarScale(GameObject bodyObject)
    {

        Transform avatar = bodyObject.transform.Find("Avatar");
        if (avatar.localScale.x != 1)
        {
            return;
        }

        //Scale avatar based on torso distance
        Transform hips = avatar.Find("Pelvis");
        Transform spineBase = bodyObject.transform.Find("SpineBase");
        Transform spineShoulder = bodyObject.transform.Find("SpineShoulder");
        float bodyScale = Vector3.Magnitude(spineShoulder.position - spineBase.position);
        Transform neck = avatar.Find("Pelvis/Abdomine/Torso/Neck/Head");
        float avatarScale = Vector3.Magnitude(neck.position - hips.position);
        float scaleFactor = bodyScale / avatarScale;
        avatar.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

    }

    //To be refined with dictonary data
    private void RefreshRobot(Kinect.Body body)
    {

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
           
            if (jt.ToString() == "Neck")
            {
                botHead.transform.rotation = GetQuaternionFromJointOrientation(body.JointOrientations[jt]) * Quaternion.Euler(0.0f, 160.0f, 0.0f);
            }
            if (jt.ToString() == "SpineShoulder")
            {
                botTorso.transform.rotation = GetQuaternionFromJointOrientation(body.JointOrientations[jt]) * Quaternion.Euler(0.0f, 160.0f, 0.0f);
            }
            if (jt.ToString() == "ElbowLeft") 
            {
                botShoulderRight.transform.rotation = GetQuaternionFromJointOrientation(body.JointOrientations[jt]) * Quaternion.Euler(180.0f, 160.0f, 0.0f);
            }
            if (jt.ToString() == "WristLeft")
            {
                botElbowRight.transform.rotation = GetQuaternionFromJointOrientation(body.JointOrientations[jt]) * Quaternion.Euler(0.0f, 0.0f, 180.0f);
            }
            if (jt.ToString() == "ElbowRight")
            {
                botShoulderLeft.transform.rotation = GetQuaternionFromJointOrientation(body.JointOrientations[jt]) * Quaternion.Euler(180.0f, 160.0f, 0.0f);
            }
            if (jt.ToString() == "WristRight")
            {
                botElbowLeft.transform.rotation = GetQuaternionFromJointOrientation(body.JointOrientations[jt]) * Quaternion.Euler(0.0f, 0.0f, 180.0f);
            }
            
        }
    }

    //Not used - for refreshing generl avatar
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        
        Transform avatar = bodyObject.transform.Find("Avatar");

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.JointOrientation sourceJointOrientation = body.JointOrientations[jt];

            if (_BoneMap.ContainsKey(jt))
            {

                if (_RigMap.ContainsKey(jt.ToString()))
                {
                    Transform avatarItem = avatar.Find(_RigMap[jt.ToString()]);
                    Transform bodyItem = bodyObject.transform.Find(jt.ToString());
                    
                    if (jt.ToString() == "SpineBase")
                    {
                        avatarItem.position = bodyItem.position;
                    }
                    
                    avatarItem.rotation = bodyItem.rotation * _RigMapOffsets[jt.ToString()];
                }
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            jointObj.localRotation = GetQuaternionFromJointOrientation(sourceJointOrientation);

        }
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 3, joint.Position.Y * 3, joint.Position.Z * 3);
    }

    private static Quaternion GetQuaternionFromJointOrientation(Kinect.JointOrientation jointOrientation)
    {
        return new Quaternion(-jointOrientation.Orientation.X, jointOrientation.Orientation.Y, -jointOrientation.Orientation.Z, jointOrientation.Orientation.W);
    }
}
