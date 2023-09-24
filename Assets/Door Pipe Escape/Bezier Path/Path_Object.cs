using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Bezier
{
    [System.Serializable]
    public class Path_Object
    {
        [SerializeField, HideInInspector]
        private List<Vector2> points;
        [SerializeField, HideInInspector]
        private List<Vector2> normalPoints;
        [SerializeField, HideInInspector]
        private bool isClosed;
        [SerializeField, HideInInspector]
        private bool autoSetControlPoints;
        [HideInInspector]
        public PathType pathType;
        [HideInInspector]
        public UnityEvent pathFinishedEvent = new UnityEvent();
        [HideInInspector]
        public Path_Creator_Object path_Creator_Object;

        private int Segment = 0;
        private float pathTime = 0;
        private Vector2 evenlySpacedPoints = Vector2.zero;
        private int divisions = 0;
        private float controlNetLenght = 0;
        private float estimatedCurveLenght = 0;
        private int oneTimePath;
        private bool pathFinish;
        public Path_Object(Path_Creator_Object path_Creator_Object)
        {
            this.path_Creator_Object = path_Creator_Object;
            pathType = path_Creator_Object.pathType;
            isClosed = pathType == PathType.Circle;
            points = new List<Vector2>();
            normalPoints = new List<Vector2>();
            for (int e = 0; e < path_Creator_Object.transform.childCount; e++)
            {
                Transform child = path_Creator_Object.transform.GetChild(e);
                if (e == 0)
                {
                    points.Add(child.position);
                    points.Add(child.position + child.up * 0.25f);
                }
                else if (e == path_Creator_Object.transform.childCount - 1)
                {
                    points.Add(child.position - child.up * 0.25f);
                    points.Add(child.position);
                }
                else
                {
                    points.Add(child.position - child.up * 0.25f);
                    points.Add(child.position);
                    points.Add(child.position + child.up * 0.25f);
                }
                pathFinishedEvent = new UnityEvent();
            }
            if (isClosed)
            {
                ClosePath();
            }
            for (int e = 0; e < points.Count; e++)
            {
                normalPoints.Add(points[e]);
            }
        }
        public Vector2 this[int i] { get { return points[i]; } }
        public bool PathFinish { get { return pathFinish; } }
        public int NumPoints { get { return points.Count; } }
        public int NumSegments { get { return points.Count / 3; } }
        public bool IsClosed
        {
            get { return isClosed; }
            set
            {
                if (isClosed != value)
                {
                    isClosed = value;
                    if (isClosed)
                    {
                        ClosePath();
                    }
                    else
                    {
                        points.RemoveRange(points.Count - 2, 2);
                        if (autoSetControlPoints)
                        {
                            AutoSetStartAndEndControls();
                        }
                    }
                }
            }
        }
        private void ClosePath()
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add(points[0] * 2 - points[1]);
            if (autoSetControlPoints)
            {
                AutoSetAnchorControlPoints(0);
                AutoSetAnchorControlPoints(points.Count - 3);
            }
        }
        public Vector2[] GetPointsInSegment(int i)
        {
            return new Vector2[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
        }
        public void MovePoint(int i, Vector2 pos)
        {
            Vector2 deltaMove = pos - points[i];
            if (i % 3 == 0)
            {
                try
                {
                    path_Creator_Object.transform.GetChild(i / 3).position = pos;
                }
                catch
                {
                    path_Creator_Object.transform.GetComponent<Path_Creator_Object>().CreatePath();
                }
            }
            if (i % 3 == 0 || !autoSetControlPoints)
            {
                points[i] = pos;

                if (autoSetControlPoints)
                {
                    AutoSetAllAffectedControlPoints(i);
                }
                else
                {
                    if (i % 3 == 0)
                    {
                        if (i + 1 < points.Count || isClosed)
                        {
                            points[LoopIndex(i + 1)] += deltaMove;
                        }
                        if (i - 1 >= 0 || isClosed)
                        {
                            points[LoopIndex(i - 1)] += deltaMove;
                        }
                    }
                    else
                    {
                        bool nextPointIsAnchor = (i + 1) % 3 == 0;
                        int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                        int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                        if (isClosed || correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                        {
                            float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                            Vector2 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                            points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                        }
                    }
                }
            }
        }
        private void AutoSetAllAffectedControlPoints(int updateAnchorIndex)
        {
            for (int i = updateAnchorIndex - 3; i <= updateAnchorIndex + 3; i += 3)
            {
                if (i >= 0 && i < points.Count || isClosed)
                {
                    AutoSetAnchorControlPoints(LoopIndex(i));
                }
            }
            AutoSetStartAndEndControls();
        }
        private void AutoSetAnchorControlPoints(int anchorIndex)
        {
            Vector2 anchorPos = points[anchorIndex];
            Vector2 dir = Vector2.zero;
            float[] neighbourDistances = new float[2];
            if (anchorIndex - 3 >= 0 || isClosed)
            {
                Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
                dir += offset.normalized;
                neighbourDistances[0] = offset.magnitude;
            }
            if (anchorIndex + 3 >= 0 || isClosed)
            {
                Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
                dir -= offset.normalized;
                neighbourDistances[1] = -offset.magnitude;
            }
            dir.Normalize();
            for (int i = 0; i < 2; i++)
            {
                int controlIndex = anchorIndex + i * 2 - 1;
                if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
                {
                    points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * 0.5f;
                }
            }
        }
        private void AutoSetStartAndEndControls()
        {
            if (!isClosed)
            {
                points[1] = (points[0] + points[2]) * 0.5f;
                points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * 0.5f;
            }
        }
        private int LoopIndex(int i)
        {
            return (i + points.Count) % points.Count;
        }
        public Vector2 CalculateEvenlySpacedPoints()
        {
            Vector2[] p = GetPointsInSegment(Segment);
            controlNetLenght = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            estimatedCurveLenght = Vector2.Distance(p[0], p[3]) + controlNetLenght;
            divisions = Mathf.CeilToInt(estimatedCurveLenght);
            pathTime += 1.0f * path_Creator_Object.Speed / (divisions * 10);
            evenlySpacedPoints = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], pathTime);
            if (pathTime >= 1)
            {
                pathTime = 0;
                Segment++;
                if (Segment >= NumSegments)
                {
                    Segment = 0;
                    if (pathType == PathType.Circle)
                    {
                        pathFinishedEvent?.Invoke();
                    }
                    else if (pathType == PathType.PingPong)
                    {
                        PathTypePingPong(false);
                    }
                    else if (pathType == PathType.OnePingPong)
                    {
                        PathTypePingPong(true);
                    }
                    else if (pathType == PathType.OneTime)
                    {
                        pathFinish = true;
                        pathFinishedEvent?.Invoke();
                    }
                }
            }
            return evenlySpacedPoints;
        }
        private void PathTypePingPong(bool oneTime)
        {
            SetPath(false);
            oneTimePath++;
            if (oneTimePath == 2)
            {
                oneTimePath = 0;
                pathFinishedEvent?.Invoke();
                if (oneTime)
                {
                    pathFinish = true;
                }
            }
        }
        public void SetPath(bool lastPipe)
        {
            points.Clear();
            if (lastPipe)
            {
                for (int e = normalPoints.Count - 1; e >= 0; e--)
                {
                    points.Add(normalPoints[e]);
                }
            }
            else
            {
                for (int e = 0; e < normalPoints.Count; e++)
                {
                    points.Add(normalPoints[e]);
                }
            }
            ResetOneTimePath();
        }
        public void ResetOneTimePath()
        {
            pathFinish = false;
        }
    }
}