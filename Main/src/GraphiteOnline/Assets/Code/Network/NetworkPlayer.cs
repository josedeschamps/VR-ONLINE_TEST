using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviour
{
    [Header("Mesh Transform")]
    [SerializeField]
    private Transform head;
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Transform rightHand;
    private PhotonView photonView;

    [Header("Rig Transform")]
    [SerializeField]
    private Transform headRig;
    [SerializeField]
    private Transform leftHandRig;
    [SerializeField]
    private Transform rightHandRig;


    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private void Start()
    {
        InitializeMesh();
    }

    private void InitializeMesh()
    {
        //Rig
        XRRig rig = FindObjectOfType<XRRig>();
        headRig = rig.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(0);
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
        //leftHand = headRig.GetComponent<Transform>().GetChild(1);
        //rightHand = rig.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(2);

        //Mesh
        head = GetComponent<Transform>().GetChild(0);
        leftHand = GetComponent<Transform>().GetChild(1);
        rightHand = GetComponent<Transform>().GetChild(2);
        photonView = GetComponent<PhotonView>();

        //Mesh Checker for network
        CheckMeshDisableOverNetwork();

    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);

        }
    }

    private void CheckMeshDisableOverNetwork()
    {
        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }

    }

    private void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }


    private void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
}
