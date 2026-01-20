using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectEventSO))]
public class ObjectEventSOEditor : BaseEventSOEditor<object> 
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        
    }
}