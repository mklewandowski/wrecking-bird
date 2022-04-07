using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public void SetOrthographicSize()
    {
        float aspectRatioHW = (float)Screen.height / (float)Screen.width;
        float artMaxWidth = 720;
        float orthographicSize = artMaxWidth * aspectRatioHW / 200f;
        Debug.Log("Setting camera orthographicSize: " + orthographicSize);
        this.GetComponent<Camera>().orthographicSize = orthographicSize;
    }
}
