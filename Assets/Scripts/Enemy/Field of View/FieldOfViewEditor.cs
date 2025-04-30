using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.XR;

[CustomEditor(typeof(RangeDetector))]
//[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        RangeDetector rangeDetector = (RangeDetector)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(rangeDetector.transform.position, Vector3.up, Vector3.forward, 360, rangeDetector.targetMask);

        Vector3 viewAngleLeft = DirectionFromAngle(rangeDetector.transform.eulerAngles.y, -rangeDetector.angle / 2);
        Vector3 viewAngleRight = DirectionFromAngle(rangeDetector.transform.eulerAngles.y, rangeDetector.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(rangeDetector.transform.position, rangeDetector.transform.position + viewAngleLeft * rangeDetector.targetMask);
        Handles.DrawLine(rangeDetector.transform.position, rangeDetector.transform.position + viewAngleRight * rangeDetector.targetMask);

        if (rangeDetector.DetectedTarget)
        {
            Handles.color = Color.green;
            Handles.DrawLine(rangeDetector.transform.position, rangeDetector.DetectedTarget.transform.position);
        }
    }



    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


}
