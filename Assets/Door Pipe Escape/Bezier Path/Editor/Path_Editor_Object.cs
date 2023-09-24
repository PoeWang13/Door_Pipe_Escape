using UnityEngine;
using UnityEditor;

namespace Bezier
{
    [CustomEditor(typeof(Path_Creator_Object))]
    public class Path_Editor_Object : Editor
    {
        Path_Creator_Object creator;
        Path_Object Path_Object
        {
            get
            {
                return creator.path_Object;
            }
        }
        const float segmentSelectDistanceThreshold = 0.1f;
        int selectedSegmentIndex = -1;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Create New Path"))
            {
                Undo.RecordObject(creator, "Create New");
                creator.CreatePath();
            }
            GUILayout.Space(10);
            //bool isClosed = GUILayout.Toggle(Path_Object.IsClosed, "Closed");
            //if (isClosed != Path_Object.IsClosed)
            //{
            //    Undo.RecordObject(creator, "Toogle Close");
            //    Path_Object.IsClosed = isClosed;
            //}
            if (creator.pathType != Path_Object.pathType)
            {
                Path_Object.ResetOneTimePath();
                Path_Object.pathType = creator.pathType;
            }
            if ((Path_Object.pathType == PathType.PingPong || Path_Object.pathType == PathType.OnePingPong) && Path_Object.IsClosed)
            {
                EditorGUILayout.HelpBox("If path is close, you should choose circle Path.", MessageType.Warning);
            }
            else if (!Path_Object.IsClosed && Path_Object.pathType == PathType.Circle)
            {
                EditorGUILayout.HelpBox("If path is not close, you should not choose circle Path.", MessageType.Warning);
            }
            //bool autoSetControlPoints = GUILayout.Toggle(Path_Object.AutoSetControlPoints, "Auto Set Control Points");
            //if (autoSetControlPoints != Path_Object.AutoSetControlPoints)
            //{
            //    Undo.RecordObject(creator, "Toggle Auto Set Control Points");
            //    Path_Object.AutoSetControlPoints = autoSetControlPoints;
            //}
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }
        private void OnSceneGUI()
        {
            Input();
            Draw();
        }
        private void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
            if (guiEvent.type == EventType.MouseMove)
            {
                float minDistanceSegment = segmentSelectDistanceThreshold;
                int newSelectedSegmentIndex = -1;
                for (int i = 0; i < Path_Object.NumSegments; i++)
                {
                    Vector2[] points = Path_Object.GetPointsInSegment(i);
                    float dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                    if (dst < minDistanceSegment)
                    {
                        minDistanceSegment = dst;
                        newSelectedSegmentIndex = i;
                    }
                }
                if (newSelectedSegmentIndex != selectedSegmentIndex)
                {
                    selectedSegmentIndex = newSelectedSegmentIndex;
                    HandleUtility.Repaint();
                }
            }
        }
        private void Draw()
        {
            for (int i = 0; i < Path_Object.NumSegments; i++)
            {
                Vector2[] point = Path_Object.GetPointsInSegment(i);
                if (creator.displayControlPoints)
                {
                    Handles.color = Color.black;
                    Handles.DrawLine(point[1], point[0]);
                    Handles.DrawLine(point[2], point[3]);
                }
                Handles.DrawBezier(point[0], point[3], point[1], point[2], Color.green, null, 2);
            }
            //if (creator.path_Object.IsClosed)
            //{
            //    Debug.Log("Draw");
            //    Vector2[] point = Path_Object.GetPointsInSegment(Path_Object.NumSegments - 1, true);
            //    Handles.DrawBezier(point[0], point[3], point[1], point[2], Color.green, null, 2);
            //}
            for (int i = 0; i < Path_Object.NumPoints; i++)
            {
                if (i % 3 == 0 || creator.displayControlPoints)
                {
                    Handles.color = (i % 3 == 0) ? creator.anchorColor : creator.controlColor;
                    float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiameter;
                    Vector2 newPos = Handles.FreeMoveHandle(Path_Object[i], Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
                    if (Path_Object[i] != newPos)
                    {
                        Path_Object.MovePoint(i, newPos);
                        Undo.RecordObject(creator, "Move Point");
                    }
                }
            }
        }
        private void OnEnable()
        {
            creator = (Path_Creator_Object)target;
            if (creator.path_Object == null)
            {
                creator.CreatePath();
            }
        }
    }
}