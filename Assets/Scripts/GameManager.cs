using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerRobot player1Robot;
    public PlayerRobot player2Robot;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveRobot(PlayerRobot robot)
    {
        if (robot.playerId == 1) player1Robot = robot;
        else player2Robot = robot;
    }
}
