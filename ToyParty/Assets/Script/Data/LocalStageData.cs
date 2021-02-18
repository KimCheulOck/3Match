using UnityEngine;

[CreateAssetMenu(fileName = "LocalStageData", menuName = "Scriptable/LocalStageData")]
public class LocalStageData : ScriptableObject
{
    [System.Serializable]
    public struct Data
    {
        public BlockType BlockType;
        public Vector2Int Location;
        public bool IsFixedBlock;
        public bool IsShow;
        public bool IsQuestBlock;
        public int QuestCount;
    }

    public Data[] Datas;
    public int MaxMoveCount;
    public int StageLengthY;
    public int StageLengthX;
}


