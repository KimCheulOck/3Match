using UnityEngine;
using UnityEngine.UI;

public class HexaBlockUnit : MonoBehaviour
{
    [SerializeField]
    private TweenPosition tween = null;

    [SerializeField]
    private Image imgBlock = null;

    [SerializeField]
    private GameObject objBoomText = null;

    [SerializeField]
    private Color[] colors;

    private Block block = null;

    public void SetBlock(Block block)
    {
        this.block = block;
    }

    public void Show()
    {
        SetBlockIcon();
    }

    public Block GetBlock()
    {
        return block;
    }

    public void PlaySwapAnimation(Vector3 from, Vector3 to, System.Action onFinished)
    {
        tween.from = from;
        tween.to = to;
        tween.ClearFinishedEvent();
        {
            tween.AddFinishedEvent(() =>
            {
                if (onFinished != null)
                    onFinished();
            });
        }
        tween.PlayEvent();
    }


    public bool PlayPathFindAnimation()
    {
        if (block.Paths == null || block.Paths.Count <= 1)
            return false;

        tween.from = block.Paths.Dequeue();
        tween.to = block.Paths.Peek();
        tween.ClearFinishedEvent();
        {
            tween.AddFinishedEvent(() =>
            {
                PlayPathFindAnimation();
            });
        }

        tween.PlayEvent();
        return true;
    }

    public void StopSwapAnimation()
    {
        tween.ClearFinishedEvent();
        tween.StopEvent();
    }

    public float GetPathFindDuration()
    {
        return tween.duration * block.Paths.Count;
    }

    public void Boom()
    {
        // 임시 폭발처리
        imgBlock.color = Color.white;
        objBoomText.SetActive(true);
    }

    private void SetBlockIcon()
    {
        objBoomText.SetActive(false);

        if (block.BlockType == BlockType.Empty)
        {
            imgBlock.gameObject.SetActive(false);
            return;
        }

        // 이후에는 테이블을 불러와서 리소스를 교체하도록 수정 예정
        // 현재는 임시처리

        if (block.IsQuestBlock && block.QuestCount == 1)
            imgBlock.color = Color.gray;
        else
            imgBlock.color = colors[(int)block.BlockType];

        if (block.ItemType == ItemType.None)
            imgBlock.transform.localRotation = Quaternion.Euler(Vector3.zero);
        else if (block.ItemType == ItemType.Firecracker_Diagonal_Left)
            imgBlock.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 120));
        else if (block.ItemType == ItemType.Firecracker_Diagonal_Right)
            imgBlock.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 60));
        else if (block.ItemType == ItemType.Firecracker_UpDown)
            imgBlock.transform.localRotation = Quaternion.Euler(Vector3.zero);

        imgBlock.gameObject.SetActive(block.IsShow);

        if (block.BlockType == BlockType.PegTop)
            imgBlock.sprite = Resources.Load<Sprite>("Texture/img_PegTop");
        else if (block.BlockType == BlockType.UFO)
            imgBlock.sprite = Resources.Load<Sprite>("Texture/img_Puzzle_UFO");
        else if (block.ItemType > ItemType.None)
            imgBlock.sprite = Resources.Load<Sprite>("Texture/img_Puzzle_Fire");
        else
            imgBlock.sprite = Resources.Load<Sprite>("Texture/img_Puzzle");
    }
}
