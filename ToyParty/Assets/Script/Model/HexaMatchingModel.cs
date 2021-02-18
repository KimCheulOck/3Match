using System.Collections.Generic;
using UnityEngine;

public class HexaMatchingModel
{
    private const int MATCH_COUNT_MIN = 3;
    private const int MATCH_COUNT_FIRECRACKER = 4;
    private const int MATCH_COUNT_UFO = 5;
    private MapModel mapModel = null;

    public void SetMapModel(MapModel mapModel)
    {
        this.mapModel = mapModel;
    }

    public void CheckBoomAftermath()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (!mapModel.CheckShowBlock(y, x))
                    continue;

                int directionIndex = (x % 2);
                bool isBoomAftermath = false;
                for (int j = 0; j < mapModel.DirectionLengthY; ++j)
                {
                    if (! mapModel.CheckQuestBlock(y,x))
                        continue;

                    for (int k = 0; k < mapModel.DirectionLengthX; ++k)
                    {
                        Vector2Int direction = mapModel.GetDirection(directionIndex, j, k);
                        int nextY = y + direction.y;
                        int nextX = x + direction.x;

                        if (mapModel.CheckRangeOver(nextY, nextX))
                            continue;

                        Block nectBlock = mapModel.GetBlock(nextY, nextX);
                        if (!nectBlock.IsShow)
                            continue;

                        if (nectBlock.IsQuestBlock)
                            continue;

                        if (nectBlock.IsBoom)
                        {
                            isBoomAftermath = true;
                            break;
                        }
                    }

                    if (isBoomAftermath)
                    {
                        mapModel.SetQuestCountMinus(y, x);
                        break;
                    }
                }                
            }
        }
    }

    public bool CheckQuestBlock()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (!mapModel.CheckShowBlock(y, x))
                    continue;
                if (mapModel.CheckQuestBlock(y, x))
                    return true;
            }
        }

        return false;
    }

    public bool CheckPossibleSwapBlocks()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (!mapModel.CheckShowBlock(y, x))
                    continue;
                if (mapModel.CheckEmptyBlock(y, x))
                    continue;
                if (mapModel.CheckFixedBlock(y, x))
                    continue;

                int directionIndex = (x % 2);
                for (int j = 0; j < mapModel.DirectionLengthY; ++j)
                {
                    for (int k = 0; k < mapModel.DirectionLengthX; ++k)
                    {
                        Vector2Int swapDirection = mapModel.GetDirection(directionIndex, j, k);
                        int swapY = y + swapDirection.y;
                        int swapX = x + swapDirection.x;

                        if (mapModel.CheckRangeOver(swapY, swapX))
                            continue;
                        if (mapModel.CheckEmptyBlock(swapY, swapX))
                            continue;
                        if (mapModel.CheckFixedBlock(swapY, swapX))
                            continue;

                        mapModel.SwapBlocks(mapModel.GetBlock(y, x), mapModel.GetBlock(swapY, swapX));
                        if (mapModel.GetBlock(y, x).BlockType > BlockType.Red)
                        {
                            mapModel.SwapBlocks(mapModel.GetBlock(y, x), mapModel.GetBlock(swapY, swapX));
                            continue;
                        }

                        for (int n = 0; n < mapModel.DirectionLengthY; ++n)
                        {
                            Queue<Block> block = new Queue<Block>();
                            block.Enqueue(mapModel.GetBlock(y, x));

                            for (int m = 0; m < mapModel.DirectionLengthX; ++m)
                            {
                                Vector2Int direction = mapModel.GetDirection(directionIndex, n, m);
                                int nextY = y + direction.y;
                                int nextX = x + direction.x;
                                DFSMatchingBlock(nextY, nextX, n, m, block);
                            }

                            if (block.Count >= MATCH_COUNT_MIN)
                            {
                                mapModel.SwapBlocks(mapModel.GetBlock(y, x), mapModel.GetBlock(swapY, swapX));
                                return true;
                            }
                        }

                        mapModel.SwapBlocks(mapModel.GetBlock(y, x), mapModel.GetBlock(swapY, swapX));
                    }
                }
            }
        }

        return false;
    }

    public bool CheckNormalMatching(Block moveBlock, SwapDirectionType swapDirectionType)
    {
        bool isBoom = false;

        if (moveBlock.BlockType > BlockType.Red)
            return false;

        if (!moveBlock.IsShow)
            return false;

        if (moveBlock.IsBoom)
            return false;

        int directionIndex = (moveBlock.X % 2);
        for (int j = 0; j < mapModel.DirectionLengthY; ++j)
        {
            Queue<Block> blocks = new Queue<Block>();
            blocks.Enqueue(moveBlock);

            for (int k = 0; k < mapModel.DirectionLengthX; ++k)
            {
                Vector2Int direction = mapModel.GetDirection(directionIndex, j, k);
                int nextY = moveBlock.Y + direction.y;
                int nextX = moveBlock.X + direction.x;
                DFSMatchingBlock(nextY, nextX, j, k, blocks);
            }

            if (blocks.Count >= MATCH_COUNT_MIN)
            {
                if (blocks.Count >= MATCH_COUNT_UFO)
                {
                    blocks.Dequeue();
                    moveBlock.SetBlockType(BlockType.UFO);
                    moveBlock.SetBoomFlag(false);
                }
                else if (blocks.Count >= MATCH_COUNT_FIRECRACKER)
                {
                    blocks.Dequeue();
                    Block nextBlock = blocks.Peek();

                    if(swapDirectionType == SwapDirectionType.None)
                        swapDirectionType =  GetSwapDirectionType(moveBlock, nextBlock);

                    if (swapDirectionType == SwapDirectionType.Left)
                        moveBlock.SetItemType(ItemType.Firecracker_Diagonal_Right);
                    else if (swapDirectionType == SwapDirectionType.Right)
                        moveBlock.SetItemType(ItemType.Firecracker_Diagonal_Left);
                    else
                        moveBlock.SetItemType(ItemType.Firecracker_UpDown);
                    moveBlock.SetBoomFlag(false);
                }

                while (blocks.Count > 0)
                {
                    Block boomBlock = blocks.Dequeue();
                    boomBlock.SetBoomFlag(true);
                }

                isBoom = true;
            }
        }

        return isBoom;
    }

    public bool CheckFirecrackerBoom()
    {
        bool isBoom = false;

        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                Block block = mapModel.GetBlock(y, x);
                SwapDirectionType swapDirectionType = SwapDirectionType.None;
                if (block.ItemType == ItemType.Firecracker_Diagonal_Left)
                    swapDirectionType = SwapDirectionType.Left;
                else if (block.ItemType == ItemType.Firecracker_Diagonal_Right)
                    swapDirectionType = SwapDirectionType.Right;
                else if (block.ItemType == ItemType.Firecracker_UpDown)
                    swapDirectionType = SwapDirectionType.UpDown;
                if (swapDirectionType == SwapDirectionType.None)
                    continue;

                if (!block.IsBoom)
                    continue;

                for (int directionK = 0; directionK < mapModel.DirectionLengthX; ++directionK)
                    SetFirecrackerBoom(block.Y, block.X, directionK, swapDirectionType);

                isBoom = true;
            }
        }

        return isBoom;
    }

    private void SetFirecrackerBoom(int y, int x, int directionK, SwapDirectionType swapDirectionType)
    {
        if (mapModel.CheckRangeOver(y, x))
            return;
        if (mapModel.CheckFixedBlock(y, x))
            return;
        if (!mapModel.CheckShowBlock(y, x))
            return;
        if (mapModel.CheckEmptyBlock(y, x))
            return;
        if (!mapModel.CompareBlockType(y, x, BlockType.UFO) &&
            !mapModel.CheckQuestBlock(y,x))
            mapModel.SetBoomFlag(y, x, isBoom: true);

        int directionIndex = (x % 2);
        Vector2Int direction = mapModel.GetDirection(directionIndex, (int)swapDirectionType, directionK);
        int nextY = y + direction.y;
        int nextX = x + direction.x;
        SetFirecrackerBoom(nextY, nextX, directionK, swapDirectionType);
    }

    public bool CheckUFOBlocks(Block startBlock, Block targetBlock)
    {
        if (startBlock.BlockType == BlockType.UFO ||
            targetBlock.BlockType == BlockType.UFO)
        {
            startBlock.SetBoomFlag(true);
            targetBlock.SetBoomFlag(true);
            return true;
        }

        return false;
    }

    public bool CheckUFOBlocksBoom(Block startBlock, Block targetBlock)
    {
        if (startBlock == null || targetBlock == null)
            return false;

        if (startBlock.BlockType == BlockType.UFO && targetBlock.BlockType == BlockType.UFO)
        {
            if (startBlock.IsBoom && targetBlock.IsBoom)
            {
                for (int y = (mapModel.LengthY - 1); y >= 0; --y)
                {
                    for (int x = 0; x < mapModel.LengthX; ++x)
                    {
                        if (!mapModel.CheckShowBlock(y, x))
                            continue;
                        if (mapModel.CheckFixedBlock(y, x))
                            continue;
                        if (mapModel.CheckEmptyBlock(y, x))
                            continue;
                        if (mapModel.CheckQuestBlock(y, x))
                            continue;

                        mapModel.SetBoomFlag(y, x, true);
                    }
                }

                return true;
            }
        }
        else if ((startBlock.BlockType == BlockType.UFO && targetBlock.BlockType != BlockType.UFO) ||
                 (startBlock.BlockType != BlockType.UFO && targetBlock.BlockType == BlockType.UFO))
        {
            if (startBlock.BlockType == BlockType.UFO && startBlock.IsBoom)
            {
                SetUFOBlocksBoom(startBlock, targetBlock);
                return true;
            }
            else if (targetBlock.BlockType == BlockType.UFO && targetBlock.IsBoom)
            {
                SetUFOBlocksBoom(targetBlock, startBlock);
                return true;
            }
        }

        return false;
    }

    private void SetUFOBlocksBoom(Block moveBlock, Block targetBlock)
    {
        mapModel.SetBoomFlag(moveBlock.Y, moveBlock.X, isBoom: true);

        int count = 0;
        int maxCount = 6;

        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (!mapModel.CheckShowBlock(y, x))
                    continue;
                if (mapModel.CheckFixedBlock(y, x))
                    continue;
                if (mapModel.CheckQuestBlock(y, x))
                    continue;
                if (mapModel.CheckEmptyBlock(y, x))
                    continue;
                if (y == moveBlock.Y && x == moveBlock.X)
                    continue;

                if (targetBlock.ItemType == ItemType.None &&
                    targetBlock.BlockType == mapModel.GetBlockType(y,x))
                {
                    mapModel.SetBoomFlag(y, x, isBoom: true);
                    count++;
                }
                else if(targetBlock.ItemType > ItemType.None &&
                        targetBlock.BlockType == mapModel.GetBlockType(y, x) &&
                        !mapModel.CheckBoomBlock(y,x))
                {
                    mapModel.SetItemType(y, x, targetBlock.ItemType);
                    mapModel.SetBoomFlag(y, x, isBoom: true);
                    count++;
                }

                if (count == maxCount)
                    break;
            }
        }
    }

    private SwapDirectionType GetSwapDirectionType(Block block, Block nextBlock)
    {
        if (block.X == nextBlock.X)
        {
            return SwapDirectionType.UpDown;
        }
        else
        {
            int directionIndex = (block.X % 2);
            if (directionIndex == 0)
            {
                if ((block.X > nextBlock.X && block.Y == nextBlock.Y) ||
                    (block.X < nextBlock.X && block.Y < nextBlock.Y))
                {
                    return SwapDirectionType.Left;
                }
                else if ((block.X < nextBlock.X && block.Y == nextBlock.Y) ||
                    (block.X > nextBlock.X && block.Y < nextBlock.Y))
                {
                    return SwapDirectionType.Right;
                }
            }
            else if (directionIndex == 1)
            {
                if ((block.X > nextBlock.X && block.Y == nextBlock.Y) ||
                    (block.X < nextBlock.X && block.Y > nextBlock.Y))
                {
                    return SwapDirectionType.Left;
                }
                else if ((block.X > nextBlock.X && block.Y > nextBlock.Y) ||
                    (block.X < nextBlock.X && block.Y == nextBlock.Y))
                {
                    return SwapDirectionType.Right;
                }
            }
        }

        return SwapDirectionType.UpDown;
    }

    public bool CheckAllMatchingProcess()
    {
        bool isBoom = false;

        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)            
                isBoom |= CheckNormalMatching(mapModel.GetBlock(y, x), SwapDirectionType.None);
        }

        return isBoom;
    }

    public ResultType GetResultType(int moveCount)
    {
        bool isQuestBlock = CheckQuestBlock();
        bool isMaxMoveCount = moveCount == mapModel.MaxMoveCount;
        bool isGameComplete = !isQuestBlock;
        bool isGameFail = isQuestBlock && isMaxMoveCount;
        if (isGameComplete)
            return ResultType.Complete;
        else if (isGameFail)
            return ResultType.Fail;
        return ResultType.None;
    }

    public void ResetBlockBoomFlag()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
                mapModel.SetBoomFlag(y, x, isBoom: false);
        }
    }

    public void CreateBlockProcess()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (!mapModel.CheckEmptyBlock(y, x))
                    continue;
                if (mapModel.CheckShowBlock(y, x))
                    continue;
                if (mapModel.CheckFixedBlock(y, x))
                    continue;

                Block createBlock = new Block(y, x);
                createBlock.SetBlockType((BlockType)Random.Range(1, ((int)BlockType.Red + 1)));
                createBlock.SetItemType(ItemType.None);
                Block block = mapModel.GetBlock(y, x);
                block.Swap(createBlock);
            }
        }
    }

    public bool CheckRefreshBlocks()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                Block moveBlock = mapModel.GetBlock(y, x);
                if (moveBlock.BlockType > BlockType.Red)
                    continue;

                if (!moveBlock.IsShow)
                    continue;

                int directionIndex = (moveBlock.X % 2);
                for (int j = 0; j < mapModel.DirectionLengthY; ++j)
                {
                    Queue<Block> block = new Queue<Block>();
                    block.Enqueue(moveBlock);

                    for (int k = 0; k < mapModel.DirectionLengthX; ++k)
                    {
                        Vector2Int direction = mapModel.GetDirection(directionIndex, j, k);
                        int nextY = moveBlock.Y + direction.y;
                        int nextX = moveBlock.X + direction.x;
                        DFSMatchingBlock(nextY, nextX, j, k, block);
                    }

                    if (block.Count >= MATCH_COUNT_MIN)
                        return true;
                }
            }
        }

        return false;
    }

    public bool FindingDownPathProcess()
    {
        bool isFind = false;
        for (int y = 0; y < mapModel.LengthY; ++y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (mapModel.CheckFixedBlock(y, x))
                    continue;
                if (mapModel.CheckEmptyBlock(y, x))
                    continue;

                Queue<Block> bfs = new Queue<Block>();
                bfs.Enqueue(mapModel.GetBlock(y, x));
                FindingDownPath(y, x, bfs, isRecursion: true);
                isFind |= SetSwapBlockPath(y, x, bfs);
            }
        }

        return isFind;
    }

    public bool FindingWidthPathProcess()
    {
        for (int y = (mapModel.LengthY - 1); y >= 0; --y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (mapModel.CheckFixedBlock(y, x))
                    continue;
                if (mapModel.CheckEmptyBlock(y, x))
                    continue;

                Queue<Block> bfs = new Queue<Block>();
                bfs.Enqueue(mapModel.GetBlock(y, x));
                FindingWidthPath(y, x, bfs);
                if (SetSwapBlockPath(y, x, bfs))
                    return true;
            }
        }

        return false;
    }

    private bool FindingDownPath(int y, int x, Queue<Block> blocks, bool isRecursion)
    {
        int directionIndex = (x % 2);
        Vector2Int direction = mapModel.GetDirection(directionIndex, 2, 1);
        int findingY = y + direction.y;
        int findingX = x + direction.x;
        if (mapModel.CheckRangeOver(findingY, findingX))
            return false;
        if (mapModel.CheckFixedBlock(findingY, findingX))
            return false;
        if (!mapModel.CheckEmptyBlock(findingY, findingX))
            return false;

        blocks.Enqueue(mapModel.GetBlock(findingY, findingX));

        if(isRecursion)
            FindingDownPath(findingY, findingX, blocks, isRecursion);

        return true;
    }


    private void FindingWidthPath(int y, int x, Queue<Block> blocks)
    {
        int directionIndex = (x % 2);
        if (FindingDownPath(y, x, blocks, isRecursion: false))
        {
            Vector2Int direction = mapModel.GetDirection(directionIndex, 2, 1);
            int findingY = y + direction.y;
            int findingX = x + direction.x;
            FindingWidthPath(findingY, findingX, blocks);
            return;
        }

        for (int j = 0; j < (mapModel.DirectionLengthY - 1); ++j)
        {
            Vector2Int direction = mapModel.GetDirection(directionIndex, j, 1);
            int findingY = y + direction.y;
            int findingX = x + direction.x;
            if (mapModel.CheckRangeOver(findingY, findingX))
                continue;
            if (mapModel.CheckFixedBlock(findingY, findingX))
                continue;
            if (!mapModel.CheckEmptyBlock(findingY, findingX))
                continue;

            blocks.Enqueue(mapModel.GetBlock(findingY, findingX));
            FindingWidthPath(findingY, findingX, blocks);
            break;
        }
    }

    private bool SetSwapBlockPath(int y, int x, Queue<Block> bfs)
    {
        if (bfs.Count > 1)
        {
            Queue<Vector3> paths = new Queue<Vector3>();
            while (true)
            {
                Block findPath = bfs.Dequeue();
                int direction = findPath.X % 2 == 0 ? -1 : 1;
                float locationX = GameManager.ITEM_SIZE * findPath.X;
                float locationY = (GameManager.ITEM_SIZE * findPath.Y) + (GameManager.ITEM_SIZE * GameManager.GRID_Y * direction);
                paths.Enqueue(new Vector3(locationX, locationY, 0));

                if (bfs.Count == 0)
                {
                    mapModel.SwapBlocks(mapModel.GetBlock(y, x), mapModel.GetBlock(findPath.Y, findPath.X));
                    mapModel.SetPaths(findPath.Y, findPath.X, paths);
                    return true;
                }
            }
        }

        return false;
    }

    private void DFSMatchingBlock(int y, int x, int j, int k, Queue<Block> block)
    {
        if (mapModel.CheckRangeOver(y, x))
            return;
        if (mapModel.CheckEmptyBlock(y, x))
            return;
        if (!mapModel.CompareBlockType(y, x, block.Peek().BlockType))
            return;
        if (block.Peek().BlockType > BlockType.Red)
            return;
        if (!mapModel.CheckShowBlock(y, x))
            return;

        block.Enqueue(mapModel.GetBlock(y, x));

        int directionIndex = (x % 2);
        Vector2Int direction = mapModel.GetDirection(directionIndex, j, k);
        int nextX = x + direction.x;
        int nextY = y + direction.y;
        DFSMatchingBlock(nextY, nextX, j, k, block);
    }
}
