using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance;

    private static Dictionary<LaneType, float> laneYPositions = new();
    private static Dictionary<LaneType, Vector3> laneTargetPositions = new();

    [Header("Lane Targets")]
    public Transform lane1Target;
    public Transform lane2Target;
    public Transform lane3Target;

    void Awake()
    {
        Instance = this;

        laneTargetPositions[LaneType.Lane1] = lane1Target.position;
        laneTargetPositions[LaneType.Lane2] = lane2Target.position;
        laneTargetPositions[LaneType.Lane3] = lane3Target.position;

        laneYPositions[LaneType.Lane1] = lane1Target.position.y;
        laneYPositions[LaneType.Lane2] = lane2Target.position.y;
        laneYPositions[LaneType.Lane3] = lane3Target.position.y;
    }

    public static Vector3 GetLaneTargetPosition(LaneType lane)
    {
        return laneTargetPositions[lane];
    }

    public static float GetLaneY(LaneType lane)
    {
        return laneYPositions[lane];
    }
}
