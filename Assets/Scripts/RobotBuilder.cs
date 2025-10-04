using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RobotBuilder : MonoBehaviour
{
    public float robotSize;
    [Header("Robot Parts (UI Images)")]
    public RobotPart helmetPart;
    public RobotPart chestPart;
    public RobotPart leftArmPart;
    public RobotPart rightArmPart;
    public RobotPart leftLegPart;
    public RobotPart rightLegPart;

    /// <summary>
    /// Builds the robot visual entirely from a PlayerRobot's CardData references.
    /// </summary>
    public void BuildRobotFromData(PlayerRobot robot, bool isPlayerTwo = false)
    {
        if (robot == null)
        {
            Debug.LogWarning("RobotBuilder: No PlayerRobot data to build.");
            return;
        }

        AssignPart(helmetPart, robot.helmetCard);
        AssignPart(chestPart, robot.chestCard);
        AssignPart(leftArmPart, robot.gauntletCard);
        AssignPart(rightArmPart, robot.gauntletCard, true);  // mirrored
        AssignPart(leftLegPart, robot.legCard);
        AssignPart(rightLegPart, robot.legCard, true);        // mirrored

        // Flip entire robot horizontally if player two
        transform.localScale = isPlayerTwo ? new Vector3(-1, 1, 1) : Vector3.one;
        transform.localScale *= robotSize;
        Debug.Log($"RobotBuilder: Built robot for Player {robot.playerId}");
    }

    void AssignPart(RobotPart part, CardData card, bool flipX = false)
    {
        if (part == null) return;

        if (card == null)
        {
            part.SetSprites(null, null, flipX);
            return;
        }

        part.SetSprites(card.baseArt, card.detailsArt, flipX);
    }
}
