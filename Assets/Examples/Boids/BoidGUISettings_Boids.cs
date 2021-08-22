using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidGUISettings_Boids : MonoBehaviour
{
    static public float speed = 50f; // Factor to head towards the group centre
    static public float coherence = 0.01f; // Factor to head towards the group centre
    static public float seperation = 0.01f; // Factor to avoid running into others
    static public float alignment = 0.005f; // Factor to match surrounding speed and direction

    static public float boxSize = 100f; // Factor to match surrounding speed and direction


    private void OnGUI()
    {
        // Make a background box
        var style = new GUIStyle();
        style.normal.background = Texture2D.grayTexture;
        GUILayout.BeginArea(new Rect(10, 10, 200, 500), style);

        GUILayout.Label("Boid Settings");

        GUILayout.Label($"Speed {speed}");
        speed = GUILayout.HorizontalSlider(speed, 0.0f, 500.0f);
        GUILayout.Label($"Coherence {coherence}");
        coherence = GUILayout.HorizontalSlider(coherence, 0.0f, 0.1f);
        GUILayout.Label($"Seperation {seperation}");
        seperation = GUILayout.HorizontalSlider(seperation, 0.0f, 0.1f);
        GUILayout.Label($"Alignment {alignment}");
        alignment = GUILayout.HorizontalSlider(alignment, 0.0f, 0.1f);

        GUILayout.Label($"BoxSize  {boxSize}");
        boxSize = GUILayout.HorizontalSlider(boxSize, 0.0f, 200.0f);

        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxSize * 2f,boxSize * 2f, boxSize *2f));
    }
}
