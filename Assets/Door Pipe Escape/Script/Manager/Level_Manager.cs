using UnityEngine;

public class Level_Manager : MonoBehaviour
{
    [SerializeField] private Vector2 levelLimit;
    [SerializeField] private Transform playerStartPos;
    [SerializeField] private Transform enemiesParent;
    private void Start()
    {
        playerStartPos.GetChild(0).gameObject.SetActive(false);
        Camera_Manager.Instance.SetCamera(levelLimit);
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.LevelStart(playerStartPos.position);
        Player.Instance.gameObject.SetActive(true);

        for (int e = 0; e < enemiesParent.childCount; e++)
        {
            for (int h = e; h < enemiesParent.childCount; h++)
            {
                Physics.IgnoreCollision(enemiesParent.GetChild(e).GetComponent<Collider>(),
                    enemiesParent.GetChild(h).GetComponent<Collider>(), true);
            }
        }
    }
    public void LevelFinish()
    {
        if (Player.Instance.HasLevelFinishKey)
        {
            Canvas_Manager.Instance.LevelWin();
        }
    }
}