using UnityEngine;

public class Door : MonoBehaviour, IMapItem
{
    private bool sendingPerson;
    private bool canSendingPerson = true;
    private Door_Holder door_Holder;
    private Vector3 playerSendingPoint;
    public Vector3 PlayerSendingPoint { get { return playerSendingPoint; } }

    public void SetDoor(Door_Holder door_Holder, bool isAltDoor)
    {
        this.door_Holder = door_Holder;

        if (isAltDoor)
        {
            playerSendingPoint = transform.position + new Vector3(0.0f, 2.0f, 3.5f);
        }
        else
        {
            playerSendingPoint = transform.position + new Vector3(0.0f, -2.0f, 3.5f);
        }
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
        door_Holder.UseDoors(this, usingPerson);
        return true;
    }
    public void ResetDoor()
    {
        canSendingPerson = true;
    }
    public void SendingPlayer(bool sending)
    {
        sendingPerson = sending;
    }
}