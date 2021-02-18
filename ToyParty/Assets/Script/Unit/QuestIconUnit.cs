using UnityEngine;
using UnityEngine.UI;

public class QuestIconUnit : MonoBehaviour
{
    [SerializeField]
    private Image imgIcon = null;
    [SerializeField]
    private Text txtQuestCount = null;

    public void SetQuestIcon()
    {
        // 현재는 강제로 팽이
        imgIcon.sprite = Resources.Load<Sprite>("Texture/img_PegTop");
    }

    public void SetQuestCount(int count)
    {
        txtQuestCount.text = count.ToString();
    }
}
