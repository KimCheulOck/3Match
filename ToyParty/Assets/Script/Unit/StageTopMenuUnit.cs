using UnityEngine;
using UnityEngine.UI;

public class StageTopMenuUnit : MonoBehaviour
{
    [SerializeField]
    private QuestIconUnit questIconUnit = null;

    [SerializeField]
    private Text txtMoveCount = null;

    public void RefreshQuestIcon(int count)
    {
        questIconUnit.SetQuestIcon();
        questIconUnit.SetQuestCount(count);
    }

    public void RefreshMoveCount(int count)
    {
        txtMoveCount.text = count.ToString();
    }
}
