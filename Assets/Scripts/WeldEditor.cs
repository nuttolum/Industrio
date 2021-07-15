using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Weld))]
public class WeldEditor : Editor
{
    public GameObject firstObj;
    public GameObject secondObj;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        Weld weld = (Weld)target;
        if(GUILayout.Button("Weld Objects")) {
            weld.WeldObjects();
        }
    }


}
