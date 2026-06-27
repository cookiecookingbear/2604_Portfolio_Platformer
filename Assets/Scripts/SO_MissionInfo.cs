using UnityEngine;

[CreateAssetMenu(fileName = "SO_MissionInfo", menuName = "Scriptable Objects/SO_MissionInfo")]
public class SO_MissionInfo : ScriptableObject
{
    public enum MissionType
    {
        CollectCoins,
        KillMonster,
        Max
    }

    [System.Serializable]public struct MissionInfo
    {
        public MissionType missionType;
        public int targetCount;
        public float missionTime;

        public MissionInfo(MissionType missionType, int targetCount, float missionTime) { 
            this.missionType = missionType;
            this.targetCount = targetCount;
            this.missionTime = missionTime;
        }
    }

    public MissionInfo collectCoins = new MissionInfo(MissionType.CollectCoins, 5, 20.0f);
    public MissionInfo killMonster = new MissionInfo(MissionType.KillMonster, 3, 20.0f);

}
