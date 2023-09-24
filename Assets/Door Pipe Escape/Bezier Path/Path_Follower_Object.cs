using UnityEngine;
using UnityEngine.Events;

namespace Bezier
{
    public class Path_Follower_Object : MonoBehaviour
    {
        private Path_Creator_Object path_Creator_Object;

        private void Update()
        {
            if (path_Creator_Object == null)
            {
                return;
            }
            if (path_Creator_Object.path_Object.PathFinish)
            {
                return;
            }
            transform.position = path_Creator_Object.path_Object.CalculateEvenlySpacedPoints();
        }
        public void SetPathCreator(Path_Creator_Object path_Creator_Object)
        {
            //Debug.Log("Send Person : " + gameObject.name, gameObject);

            this.path_Creator_Object = path_Creator_Object;
        }
        public void ResetPipe()
        {
            path_Creator_Object = null;
        }
    }
}