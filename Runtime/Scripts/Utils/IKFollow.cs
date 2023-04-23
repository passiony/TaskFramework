using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFollow : MonoBehaviour
{
    public Transform CameraRig { get; set; }
    public Transform CameraEye { get; set; }

    private Transform EyeAnchor;
    private Vector3 head_position;
    private Vector3 foot_position;
    private Vector3 rotation;

    private void Awake()
    {
        EyeAnchor = transform.Find("CenterEyeAnchor");
    }

    public void Init(Transform rig,Transform head)
    {
        CameraRig = rig;
        CameraEye = head;
    }
    
    void LateUpdate()
    {
        if (CameraEye != null)
        {
            //脚位置
            foot_position = CameraEye.position;
            foot_position.y = CameraRig.position.y;
            transform.position = foot_position;

            //头位置
            head_position = CameraEye.position;
            head_position.y = Mathf.Clamp(head_position.y, foot_position.y+1, foot_position.y+1.6f);
            EyeAnchor.position = head_position;
            
            //头旋转
            rotation = transform.eulerAngles;
            rotation.y = CameraEye.eulerAngles.y;
            transform.eulerAngles = rotation;
        }
    }
}
