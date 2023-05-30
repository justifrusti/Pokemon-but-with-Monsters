using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactBehaviour : MonoBehaviour
{
    public enum ArtifactType
    {
        Capture,
        Weapon,
        Utility,
        Other
    }

    public ArtifactType artifactType;
}
