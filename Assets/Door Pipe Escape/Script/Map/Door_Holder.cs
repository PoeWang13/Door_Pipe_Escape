using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public enum ExitPos
{
    Right,
    Left
}
public class Door_Holder : MonoBehaviour
{
    private void OnValidate()
    {
        SetExitDoor();
    }
    [SerializeField] private ExitPos exitPos;
    [SerializeField] private Transform stair;

    private Door enterDoor = null;
    private Door exitDoor = null;
    private List<Door> doors = new List<Door>();
    private void Awake()
    {
        doors.AddRange(GetComponentsInChildren<Door>());

        Door altDoor = null;
        if (doors[0].transform.position.y < doors[1].transform.position.y)
        {
            doors[0].SetDoor(this, true);
            doors[1].SetDoor(this, false);

            altDoor = doors[0];
        }
        else
        {
            doors[0].SetDoor(this, false);
            doors[1].SetDoor(this, true);

            altDoor = doors[1];
        }
        SetStair(altDoor);
    }
    private void SetStair(Door door)
    {
        stair.gameObject.SetActive(true);
        stair.SetParent(door.transform);
        stair.localPosition = new Vector3(-0.67f, -1.45f, 0.8f);
    }
    [ContextMenu("Set Exit Door")]
    private void SetExitDoor()
    {
        Transform firstDoor = transform.Find("First_Door");
        Transform secondDoor = transform.Find("Second_Door");
        if (exitPos == ExitPos.Right)
        {
            secondDoor.localPosition = firstDoor.localPosition + new Vector3(1.65f, 4.0f, 0);
        }
        else if (exitPos == ExitPos.Left)
        {
            secondDoor.localPosition = firstDoor.localPosition + new Vector3(-1.65f, 4.0f, 0);
        }
    }
    public void UseDoors(Door enter, Transform usingPerson)
    {
        enterDoor = enter;
        if (doors[0] == enter)
        {
            exitDoor = doors[1];
        }
        else
        {
            exitDoor = doors[0];
        }
        doors.ForEach(x => x.SendingPlayer(true));
        usingPerson.DOMove(enterDoor.PlayerSendingPoint, 1).OnComplete(() =>
        {
            enterDoor.ResetDoor();
            usingPerson.position = exitDoor.PlayerSendingPoint;
            Vector3 exitPoint = exitDoor.transform.position + Vector3.down * 1.5f;
            DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.25f).OnComplete(() => {

                usingPerson.DOMove(exitPoint, 1).OnComplete(() =>
                {
                    usingPerson.GetComponent<Stat_Manager>().ExitingMapItem();
                    doors.ForEach(x => x.SendingPlayer(false));
                });
            });
        });
    }
}