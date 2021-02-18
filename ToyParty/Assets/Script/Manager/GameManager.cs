using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const float GRID_Y = 0.25f;
    public const float ITEM_SIZE = 100;
    public const float BOOM_DELAY = 1.0f;
    public const int REFRESH_LOOP_MAX = 10000;
    

    [SerializeField]
    private GameObject instanceBlockPool = null;
    [SerializeField]
    private GameObject intanceBlockBGPool = null;

    [SerializeField]
    private HexaBlockUnit loadPrefabsHexaBlock = null;
    [SerializeField]
    private GameObject loadPrefabsHexaBlockBG = null;

    [SerializeField]
    private StageTopMenuUnit stageTopMenuUnit = null;

    [SerializeField]
    private GameObject objGameResult = null;
    [SerializeField]
    private Text txtGameResult = null;

    private HexaMatchingModel matchingModel = new HexaMatchingModel();
    private InputModel inputModel = new InputModel();
    private MapModel mapModel = new MapModel();

    private HexaBlockUnit[] hexaBlockUnits = null;
    private HexaBlockUnit startHexaBlockUnit = null;
    private HexaBlockUnit targetHexaBlockUnit = null;

    private bool isSwapAnimation = false;
    private int moveCount = 0;

    private void Start()
    {
        mapModel.SetMatchingModel(matchingModel);
        matchingModel.SetMapModel(mapModel);

        mapModel.LoadMap();

        hexaBlockUnits = new HexaBlockUnit[mapModel.LengthY * mapModel.LengthX];
        inputModel.AddEvent(SwapBlockProcess);

        stageTopMenuUnit.RefreshQuestIcon(mapModel.GetQuestBlockCount());
        stageTopMenuUnit.RefreshMoveCount(mapModel.MaxMoveCount - moveCount);
        
        objGameResult.SetActive(false);

        RefreshBlockUnits();
    }

    private void Update()
    {
        if (isSwapAnimation)
            return;

        if (!inputModel.CheckSelectBlock())
        {
            ClearBlockUnits();
            ClearTouchFlag();
        }
    }

    private void MatchingProcess()
    {
        Block moveBlock = startHexaBlockUnit.GetBlock();
        Block targetBlock = targetHexaBlockUnit.GetBlock();

        mapModel.SwapBlocks(moveBlock, targetBlock);

        SwapDirectionType swapDirectionType = GetSwapBlockType();
        bool isBoom = matchingModel.CheckUFOBlocks(moveBlock, targetBlock);
        isBoom |= matchingModel.CheckNormalMatching(moveBlock, swapDirectionType);
        isBoom |= matchingModel.CheckNormalMatching(targetBlock, swapDirectionType);

        if (isBoom)
        {
            moveCount++;
            stageTopMenuUnit.RefreshMoveCount(mapModel.MaxMoveCount - moveCount);

            StartCoroutine(BoomBlockProcess());
        }
        else
        {
            mapModel.SwapBlocks(moveBlock, targetBlock);
            PlaySwapBlockProcess(isRevert: true);
        }
    }

    private SwapDirectionType GetSwapBlockType()
    {
        Vector3 startPosition = startHexaBlockUnit.transform.position;
        Vector3 targetPosition = targetHexaBlockUnit.transform.position;
        if (startPosition.x == targetPosition.x)
            return SwapDirectionType.UpDown;
        else if((startPosition.x > targetPosition.x && startPosition.y < targetPosition.y) ||
                (startPosition.x < targetPosition.x && startPosition.y > targetPosition.y))
            return SwapDirectionType.Left;
        else if ((startPosition.x < targetPosition.x && startPosition.y < targetPosition.y) ||
                 (startPosition.x > targetPosition.x && startPosition.y > targetPosition.y))
            return SwapDirectionType.Right;

        return SwapDirectionType.UpDown;
    }

    private IEnumerator BoomBlockProcess()
    {
        StopSwapAnimation();

        RefreshBlockUnits();

        Block startBlock = startHexaBlockUnit == null ? null : startHexaBlockUnit.GetBlock();
        Block targetBlock = targetHexaBlockUnit == null ? null : targetHexaBlockUnit.GetBlock();
        if (matchingModel.CheckUFOBlocksBoom(startBlock, targetBlock))
        {
            RefreshBlockUnits();
            yield return new WaitForSeconds(BOOM_DELAY);
        }

        if (matchingModel.CheckFirecrackerBoom())
        {
            BoomBlockUnits();
            yield return new WaitForSeconds(BOOM_DELAY);
        }

        BoomBlockUnits();
        yield return new WaitForSeconds(BOOM_DELAY);

        startHexaBlockUnit = null;
        targetHexaBlockUnit = null;

        matchingModel.CheckBoomAftermath();
        stageTopMenuUnit.RefreshQuestIcon(mapModel.GetQuestBlockCount());

        matchingModel.FindingDownPathProcess();

        yield return MoveBlockProcess();

        matchingModel.CreateBlockProcess();

        while (true)
        {
            if (matchingModel.FindingWidthPathProcess())
                continue;

            break;
        }

        yield return new WaitForSeconds(0.1f);

        yield return MoveBlockProcess();

        matchingModel.CreateBlockProcess();
        matchingModel.ResetBlockBoomFlag();

        RefreshBlockUnits();

        ResultType resultType = matchingModel.GetResultType(moveCount);
        switch (resultType)
        {
            case ResultType.Complete:
                {
                    objGameResult.SetActive(true);
                    txtGameResult.text = "클리어!";
                    break;
                }

            case ResultType.Fail:
                {
                    objGameResult.SetActive(true);
                    txtGameResult.text = "실패!";
                    break;
                }

            default:
                {
                    int loop = 0;
                    while (true)
                    {
                        if (matchingModel.CheckAllMatchingProcess())
                        {
                            StartCoroutine(BoomBlockProcess());
                            break;
                        }
                        else if (matchingModel.CheckPossibleSwapBlocks())
                        {
                            ClearBlockUnits();
                            ClearTouchFlag();
                            break;
                        }
                        else
                        {
                            mapModel.RefreshBlocks(isCheck: false);
                        }

                        loop++;
                        if (loop >= REFRESH_LOOP_MAX)
                            break;
                    }

                    break;
                }
        }
    }

    private IEnumerator MoveBlockProcess()
    {
        float delayTime = 0;
        for (int y = 0; y < mapModel.LengthY; ++y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                int index = (mapModel.LengthX * y) + x;
                hexaBlockUnits[index].Show();
                hexaBlockUnits[index].PlayPathFindAnimation();
                delayTime = Mathf.Max(delayTime, hexaBlockUnits[index].GetPathFindDuration());
            }
        }

        yield return new WaitForSeconds(delayTime);
    }

    private void PlaySwapBlockProcess(bool isRevert)
    {
        Vector3 from = startHexaBlockUnit.transform.localPosition;
        Vector3 to = targetHexaBlockUnit.transform.localPosition;
        startHexaBlockUnit.PlaySwapAnimation(from, to, ()=>
        {
            if (isRevert)
            {
                ClearBlockUnits();
                ClearTouchFlag();
            }
            else
            {
                MatchingProcess();
            }
        });
        targetHexaBlockUnit.PlaySwapAnimation(to, from, null);
    }

    private void SwapBlockProcess(HexaBlockUnit blockUnit)
    {
        if (blockUnit == null)
            return;

        if (startHexaBlockUnit == null)
        {
            startHexaBlockUnit = blockUnit;
            return;
        }

        if (startHexaBlockUnit == blockUnit)
            return;

        Block targetBlock = blockUnit.GetBlock();
        Block startBlock = startHexaBlockUnit.GetBlock();
        if (!startBlock.CheckAdjacencyBlock(targetBlock))
            return;

        targetHexaBlockUnit = blockUnit;

        PlaySwapBlockProcess(isRevert: false);

        isSwapAnimation = true;
    }
    
    private void RefreshBlockUnits()
    {
        for (int y = 0; y < mapModel.LengthY; ++y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                int posY = mapModel.LengthY - y;
                int index = (mapModel.LengthX * y) + x;
                if (index == hexaBlockUnits.Length)
                    return;

                int direction = x % 2 == 0 ? -1 : 1;
                float locationX = ITEM_SIZE * x;
                float locationY = (ITEM_SIZE * y) + (ITEM_SIZE * GRID_Y * direction);
                Vector3 pos = new Vector3(locationX, locationY, 0);

                if (hexaBlockUnits[index] == null)
                {
                    hexaBlockUnits[index] = Instantiate(loadPrefabsHexaBlock, instanceBlockPool.transform);

                    if (mapModel.CheckShowBlock(y, x))
                    {
                        GameObject objBG = Instantiate(loadPrefabsHexaBlockBG, intanceBlockBGPool.transform);
                        objBG.transform.localPosition = pos;
                    }

#if UNITY_EDITOR
                    hexaBlockUnits[index].name = string.Format("{0}_{1}", y, x);
#endif
                }

                hexaBlockUnits[index].transform.localPosition = pos;
                hexaBlockUnits[index].gameObject.SetActive(mapModel.CheckShowBlock(y, x));
                hexaBlockUnits[index].SetBlock(mapModel.GetBlock(y,x));
                hexaBlockUnits[index].Show();
            }
        }
    }

    private void BoomBlockUnits()
    {
        int index = 0;
        for (int y = 0; y < mapModel.LengthY; ++y)
        {
            for (int x = 0; x < mapModel.LengthX; ++x)
            {
                if (mapModel.CheckBoomBlock(y, x))
                {
                    mapModel.SetBlockType(y, x, BlockType.Empty);
                    hexaBlockUnits[index].Boom();
                }

                index++;
            }
        }
    }

    private void StopSwapAnimation()
    {
        if (startHexaBlockUnit != null)
            startHexaBlockUnit.StopSwapAnimation();

        if (targetHexaBlockUnit != null)
            targetHexaBlockUnit.StopSwapAnimation();
    }

    private void ClearBlockUnits()
    {
        startHexaBlockUnit = null;
        targetHexaBlockUnit = null;
    }

    private void ClearTouchFlag()
    {
        isSwapAnimation = false;
        inputModel.SetTouchFlag(false);
    }
}