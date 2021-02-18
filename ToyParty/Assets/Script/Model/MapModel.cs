using UnityEngine;
using System.Collections.Generic;

public class MapModel
{
    public int LengthY { get { return maps.GetLength(0); } }
    public int LengthX { get { return maps.GetLength(1); } }
    public int DirectionLengthY { get { return directions.GetLength(1); } }
    public int DirectionLengthX { get { return directions.GetLength(2); } }
    public int MaxMoveCount { get; private set; }

    private HexaMatchingModel matchingModel;
    private Block[,] maps = null;
    private Vector2Int[,,] directions = new Vector2Int[,,]
    {
        {
            { new Vector2Int(1,0) ,new Vector2Int(-1,-1)},
            { new Vector2Int(-1,0), new Vector2Int(1,-1) },
            {  new Vector2Int(0, 1), new Vector2Int(0,-1) }
        },
        {
            {  new Vector2Int(1, 1), new Vector2Int(-1,0) },
            { new Vector2Int(-1, 1), new Vector2Int(1,0) },
            {  new Vector2Int(0, 1), new Vector2Int(0,-1) }
        }
    };
    private int showLine;

    public void SetMatchingModel(HexaMatchingModel matchingModel)
    {
        this.matchingModel = matchingModel;
    }

    public void LoadMap()
    {
        LocalStageData stage = Resources.Load<LocalStageData>("Data/Stage21");

        InitializeMaps(stage);

        RefreshBlocks();

        SetStageMapData(stage);

        MaxMoveCount = stage.MaxMoveCount;
    }

    public bool CheckFixedBlock(int y, int x)
    {
        return maps[y, x].IsFixedBlock;
    }

    public bool CheckBoomBlock(int y, int x)
    {
        return maps[y, x].IsBoom;
    }

    public bool CheckRangeOver(int y, int x)
    {
        return (y >= maps.GetLength(0) || x >= maps.GetLength(1)) || (y < 0 || x < 0);
    }

    public bool CheckEmptyBlock(int y, int x)
    {
        return maps[y, x].BlockType == BlockType.Empty;
    }

    public bool CheckShowBlock(int y, int x)
    {
        return maps[y, x].IsShow;
    }

    public bool CheckQuestBlock(int y, int x)
    {
        return maps[y, x].IsQuestBlock;
    }

    public int GetQuestBlockCount()
    {
        int questBlockCount = 0;
        for (int y = 0; y < maps.GetLength(0); ++y)
        {
            for (int x = 0; x < maps.GetLength(1); ++x)
            {
                if (maps[y, x].IsQuestBlock && maps[y, x].QuestCount > 0)
                    questBlockCount++;
            }
        }

        return questBlockCount;
    }

    public Block GetBlock(int y, int x)
    {
        return maps[y, x];
    }

    public BlockType GetBlockType(int y, int x)
    {
        return maps[y, x].BlockType;
    }

    public Vector2Int GetDirection(int directionIndex, int j, int k)
    {
        return directions[directionIndex, j, k];
    }

    public bool CompareBlockType(int y, int x, BlockType blockType)
    {
        return maps[y, x].BlockType == blockType;
    }

    public void RefreshBlocks(bool isCheck = true)
    {
        for (int i = 0; i < GameManager.REFRESH_LOOP_MAX; ++i)
        {
            for (int y = 0; y < maps.GetLength(0); ++y)
            {
                for (int x = 0; x < maps.GetLength(1); ++x)
                {
                    if (maps[y, x].IsFixedBlock)
                        continue;

                    if (maps[y, x].IsQuestBlock)
                        continue;

                    maps[y, x].SetBlockType((BlockType)Random.Range(1, ((int)BlockType.Red + 1)));
                    maps[y, x].SetItemType(ItemType.None);
                    maps[y, x].SetShowFlag(y < showLine);
                }
            }

            if (!isCheck)
                break;

            if (!matchingModel.CheckRefreshBlocks())
                break;
        }
    }

    public void SetPaths(int y, int x, Queue<Vector3> paths)
    {
        maps[y, x].SetPaths(paths);
    }

    public void SetBoomFlag(int y, int x, bool isBoom)
    {
        maps[y, x].SetBoomFlag(isBoom);
    }

    public void SetBlockType(int y, int x, BlockType blockType)
    {
        maps[y, x].SetBlockType(blockType);
    }

    public void SetItemType(int y, int x, ItemType itemType)
    {
        maps[y, x].SetItemType(itemType);
    }

    public void SetQuestCountMinus(int y, int x)
    {
        maps[y, x].SetQuestCount(maps[y, x].QuestCount - 1);
        if (maps[y, x].QuestCount == 0)
        {
            maps[y, x].SetBoomFlag(true);
            maps[y, x].SetBlockType(BlockType.Empty);
        }
    }

    public void SwapBlocks(Block blockA, Block blockB)
    {
        Block swapBlock = new Block(blockB.Y, blockB.X);
        swapBlock.Swap(maps[blockB.Y, blockB.X]);
        maps[blockB.Y, blockB.X].Swap(maps[blockA.Y, blockA.X]);
        maps[blockA.Y, blockA.X].Swap(swapBlock);
    }

    private void InitializeMaps(LocalStageData stage)
    {
        showLine = stage.StageLengthY;
        maps = new Block[(stage.StageLengthY * stage.StageLengthX), stage.StageLengthX];

        for (int y = 0; y < maps.GetLength(0); ++y)
        {
            for (int x = 0; x < maps.GetLength(1); ++x)
                maps[y, x] = new Block(y, x);
        }
    }

    private void SetStageMapData(LocalStageData stage)
    {
        for (int i = 0; i < stage.Datas.Length; ++i)
        {
            int y = stage.Datas[i].Location.y;
            int x = stage.Datas[i].Location.x;

            maps[y, x].SetFixedBlock(stage.Datas[i].IsFixedBlock);
            maps[y, x].SetShowFlag(stage.Datas[i].IsShow);
            maps[y, x].SetBlockType(stage.Datas[i].BlockType);
            maps[y, x].SetItemType(ItemType.None);
            maps[y, x].SetQuestBlockFlag(stage.Datas[i].IsQuestBlock);
            maps[y, x].SetQuestCount(stage.Datas[i].QuestCount);
        }
    }
}
