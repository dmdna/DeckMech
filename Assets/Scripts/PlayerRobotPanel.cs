using UnityEngine;
using TMPro;

public class PlayerRobotPanel : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text hpText;
    public TMP_Text resistancesText;

    // Fill the panel with PlayerRobot info
    public void Setup(int playerId, PlayerRobot robot)
    {
        titleText.text = $"Player {playerId}";
        hpText.text = $"HP: {robot.hp}";

        // Build resistance string only with non-zero values
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (robot.thermal > 0) sb.Append($"Ther:{robot.thermal} ");
        if (robot.freeze > 0) sb.Append($"Frz:{robot.freeze} ");
        if (robot.electric > 0) sb.Append($"Ele:{robot.electric} ");
        if (robot.voidRes > 0) sb.Append($"Vd:{robot.voidRes} ");
        if (robot.impact > 0) sb.Append($"Imp:{robot.impact} ");

        resistancesText.text = sb.ToString().Trim();
    }
}
