using UnityEngine;

public class Door_Level_Finish : MonoBehaviour
{
    private Level_Manager level_Manager;
    private bool hasPlayer;
    private void Start()
    {
        level_Manager = GetComponentInParent<Level_Manager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = false;
        }
    }
    private void Update()
    {
        if (!hasPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            level_Manager.LevelFinish();
        }
    }
}