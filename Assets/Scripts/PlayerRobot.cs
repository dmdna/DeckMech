[System.Serializable]
public class PlayerRobot
{
    public int playerId;
    public int hp;
    public int thermal, freeze, electric, voidRes, impact;

    public PlayerRobot(int id)
    {
        playerId = id;
        hp = thermal = freeze = electric = voidRes = impact = 0;
    }
}
