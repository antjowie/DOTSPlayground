using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseElementGUISettings : MonoBehaviour
{
    static public float speed = 0.5f;
    static public float yScale = 50f;
    static public float zoom = 50f;

    private void OnGUI()
    {
        // Make a background box
        GUILayout.Box("Noise Settings");

        GUILayout.Label("Speed");
        speed = GUILayout.HorizontalSlider(speed, 0.0f, 10.0f);
        GUILayout.Label("Y Scale");
        yScale = GUILayout.HorizontalSlider(yScale, 0.0f, 100.0f);
        GUILayout.Label("Zoom");
        zoom = GUILayout.HorizontalSlider(zoom, 0.1f, 100.0f);

    }
}
