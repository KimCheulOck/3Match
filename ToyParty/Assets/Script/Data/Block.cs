using UnityEngine;
using System.Collections.Generic;

public class Block
{
    public BlockType BlockType { get; private set; }
    public ItemType ItemType { get; private set; }

    public Queue<Vector3> Paths { get; private set; }

    private Vector2Int location;
    public bool IsShow { get; private set; }
    public bool IsQuestBlock { get; private set; }
    public bool IsBoom { get; private set; }
    public int QuestCount { get; private set; }
    public int Y { get { return location.y; } }
    public int X { get { return location.x; } }
    
    
    public bool IsFixedBlock { get; private set; }
    public void SetFixedBlock(bool isFixedBlock)
    {
        IsFixedBlock = isFixedBlock;
    }


    public Block(int y, int x)
    {
        location = new Vector2Int(x, y);
        Paths = new Queue<Vector3>();
    }

    public void Initialize()
    {
        IsBoom = false;
    }

    public void SetShowFlag(bool isShow)
    {
        IsShow = isShow;
    }

    public void SetBoomFlag(bool isBoom)
    {
        IsBoom = isBoom;
    }

    public void SetQuestBlockFlag(bool isQuestBlock)
    {
        IsQuestBlock = isQuestBlock;
    }

    public void SetBlockType(BlockType blockType)
    {
        BlockType = blockType;
    }

    public void SetItemType(ItemType itemType)
    {
        ItemType = itemType;
    }

    public void SetQuestCount(int questCount)
    {
        QuestCount = questCount;
    }

    public void SetPaths(Queue<Vector3> paths)
    {
        Paths = paths;
    }

    public void Swap(Block block)
    {
        BlockType = block.BlockType;
        ItemType = block.ItemType;
        IsQuestBlock = block.IsQuestBlock;
        QuestCount = block.QuestCount;
    }

    public bool CheckAdjacencyBlock(Block block)
    {
        int distanceY = Mathf.Abs(Y - block.Y);
        int distanceX = Mathf.Abs(X - block.X);

        return distanceY <= 1 && distanceX <= 1;
    }
}
