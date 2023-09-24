using Bezier;
using UnityEngine;

public class Pipe : MonoBehaviour, IMapItem
{
    [SerializeField] private bool lastPipe;
    private bool canSendingPerson = true;
    private bool sendingPerson;

    private Stat_Manager stat_Manager;
    private Path_Follower_Object path_Follower_Object;

    private Pipe_Holder pipe_Holder;
    public bool LastPipe { get { return lastPipe; } }

    public void SetPipe(Pipe_Holder pipe_Holder)
    {
        this.pipe_Holder = pipe_Holder;
    }
    public bool UseMapItem(Transform usingPerson)
    {
        if (!canSendingPerson)
        {
            // Person cannot be sent.
            return false;
        }
        if (sendingPerson)
        {
            // Person sended.
            return false;
        }
        stat_Manager = usingPerson.GetComponent<Stat_Manager>();
        path_Follower_Object = usingPerson.GetComponent<Path_Follower_Object>();
        SendPerson();

        return true;
    }
    private void SendPerson()
    {
        pipe_Holder.path_Creator_Object.path_Object.pathFinishedEvent.RemoveAllListeners();

        pipe_Holder.path_Creator_Object.path_Object.pathFinishedEvent.AddListener(
            () =>
            {
                ResetPipe();
                stat_Manager.ExitingMapItem();
            });
        pipe_Holder.path_Creator_Object.path_Object.pathFinishedEvent.AddListener(() => pipe_Holder.ResetPipes());
        pipe_Holder.path_Creator_Object.path_Object.pathFinishedEvent.AddListener(() => pipe_Holder.path_Creator_Object.SendedPlayer());

        path_Follower_Object.SetPathCreator(pipe_Holder.path_Creator_Object);
        //Debug.Log("Send Person : " + stat_Manager.name, gameObject);
        pipe_Holder.UsePipes(this);
    }
    public void SendingPerson(bool sending)
    {
        sendingPerson = sending;
    }
    public void ResetPipe()
    {
        //Debug.Log(pipe_Holder.path_Creator_Object.path_Object.PathFinish, gameObject);
        path_Follower_Object.ResetPipe();
        canSendingPerson = true;
        path_Follower_Object = null;
    }
    public void ResetStat_Manager()
    {
        stat_Manager = null;
    }
}