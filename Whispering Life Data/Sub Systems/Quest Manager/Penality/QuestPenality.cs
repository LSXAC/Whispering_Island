using System;
using Godot;

public partial class QuestPenality : Node2D
{
    private Resource quest_dialogue = ResourceLoader.Load<Resource>(
        ResourceUid.UidToPath("uid://c1p7gva6jex80")
    );

    /// <summary>
    /// Determines the penalty type based on current game state
    /// </summary>
    public int DeterminePenalty()
    {
        int penalty = -1;
        Random rnd = new Random();

        if (HeartManager.instance.current_hearts == 2)
            penalty = (int)rnd.NextInt64(0, 2);
        else if (HeartManager.instance.current_hearts == 1)
            penalty = 2;

        // Check if next quest is doubled items - avoid double amount penalty
        if (penalty == 1 && QuestManager.next_quest_is_doubled_items)
        {
            QuestManager.next_quest_is_doubled_items = false;
            penalty = 0;
        }

        return penalty;
    }

    /// <summary>
    /// Queues the appropriate cutscene based on penalty type
    /// </summary>
    public void PlayPenaltyCutscene(int penalty)
    {
        if (penalty == 0)
        {
            if (TranslationServer.GetLocale() == "de")
                CutsceneManager.instance.QueueCutscene(quest_dialogue, "Quest_Not_Completed_DE");
            else
                CutsceneManager.instance.QueueCutscene(quest_dialogue, "Quest_Not_Completed_ENG");
        }
        else if (penalty == 1)
        {
            if (TranslationServer.GetLocale() == "de")
                CutsceneManager.instance.QueueCutscene(quest_dialogue, "Quest_Not_Completed_1_DE");
            else
                CutsceneManager.instance.QueueCutscene(quest_dialogue, "Quest_Not_Completed_1_ENG");
        }
        else if (penalty == 2)
        {
            if (TranslationServer.GetLocale() == "de")
                CutsceneManager.instance.QueueCutscene(quest_dialogue, "Quest_Not_Completed_2_DE");
            else
                CutsceneManager.instance.QueueCutscene(quest_dialogue, "Quest_Not_Completed_2_ENG");
        }
    }

    /// <summary>
    /// Applies the penalty effects to the game
    /// </summary>
    public void ApplyPenalty(int penalty)
    {
        if (penalty == (int)QuestAcceptPanel.PENEALTY.DOUBLE_AMOUNT)
            QuestManager.next_quest_is_doubled_items = true;

        if (penalty == 2)
            IslandManager.instance.RemoveIslandsThroughQuest();
    }
}
