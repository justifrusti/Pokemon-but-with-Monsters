using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Datasets/Player Controller Data", fileName = "Controller Data")]
public class ControllerData : ScriptableObject
{
    [System.Serializable]
    public class Type
    {
        public enum CamType
        {
            UnityStandard,
            Cinemachine
        }

        public enum CinemachineType
        {
            VirtualCam,
            FreeLookCam,
        }

        public enum ControlMode
        {
            ThirdPerson,
            FirstPerson
        }

        public CamType camType;
        public CinemachineType cinemachineMode;
        public ControlMode controlMode;
    }

    public Type type;

    [Header("Data")]
    public bool canWalk;
    public bool canSprint;
    public bool canJump;
    public bool canInteract;
    public bool canLean;
    public bool canCrouch;
    public bool canLookLeftAndRight;
    public bool canLookUpAndDown;
    [Space]
    public bool camClamped;
    [Space]
    public bool detectCollision;
    public bool canDubbleJump;
    public bool canPickUp;
    public bool lockMouseInEditor;

    [Header("Tags")]
    public string groundTag;

    [Header("Inputs")]
    public string jump = "Jump";
    public string sprint = "Sprint";
    public string interact = "Interact";
}
