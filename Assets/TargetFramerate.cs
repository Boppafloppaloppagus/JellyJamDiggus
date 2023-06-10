using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFramerate : MonoBehaviour
{
    public int frameRate;
    void Update()
    {
        if (Application.targetFrameRate != frameRate)
        {
            Application.targetFrameRate = frameRate;
        }
    }
}
