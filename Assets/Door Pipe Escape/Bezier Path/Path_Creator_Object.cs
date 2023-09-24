using UnityEngine;
using UnityEngine.Events;

namespace Bezier
{
    public enum PathType
    {
        Circle, PingPong, OnePingPong, OneTime
    }
    public class Path_Creator_Object : MonoBehaviour
    {
        [HideInInspector]
        public Path_Object path_Object;
        public PathType pathType;
        [Space]
        public Color anchorColor = Color.red;
        public Color controlColor = Color.white;
        [Space]
        public float anchorDiameter = 0.1f;
        public float controlDiameter = 0.07f;
        [Space]
        public bool displayControlPoints = true;

        private float speed = 0.25f;
        public float Speed { get { return speed; } }

        [ContextMenu("Create Path")]
        public void CreatePath()
        {
            path_Object = new Path_Object(this);
        }
        public void SendPerson(bool lastPipe)
        {
            path_Object.SetPath(lastPipe);
            speed = 0.25f;
        }
        public void SendedPlayer()
        {
            speed = 0.0f;
            path_Object.pathFinishedEvent = new UnityEvent();
        }
    }
}