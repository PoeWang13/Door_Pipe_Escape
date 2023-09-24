using Bezier;
using UnityEngine;
using System.Collections.Generic;

public class Pipe_Holder : MonoBehaviour
{
    public Path_Creator_Object path_Creator_Object;

    private List<Pipe> pipes = new List<Pipe>();
    private void Awake()
    {
        pipes.AddRange(GetComponentsInChildren<Pipe>());

        for (int e = 0; e < pipes.Count; e++)
        {
            pipes[e].SetPipe(this);
        }
    }
    public void UsePipes(Pipe enter)
    {
        pipes.ForEach(x => x.SendingPerson(true));
        path_Creator_Object.SendPerson(enter.LastPipe);
    }
    public void ResetPipes()
    {
        pipes.ForEach(x => 
        { 
            x.SendingPerson(false);
            x.ResetStat_Manager();
        });
    }
}