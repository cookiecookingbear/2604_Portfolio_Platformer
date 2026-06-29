using System;
using UnityEngine;

public class Player_Mission : MonoBehaviour
{
    public event Action MissionTargetCount;

    public void TargetSecured()
    {
        MissionTargetCount?.Invoke();
    }
}
