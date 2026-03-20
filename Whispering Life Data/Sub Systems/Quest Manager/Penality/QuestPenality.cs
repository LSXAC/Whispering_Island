using System;
using System.Diagnostics;
using Godot;

public partial class QuestPenality : Node2D
{
    private Resource quest_dialogue = ResourceLoader.Load<Resource>(
        ResourceUid.UidToPath("uid://c1p7gva6jex80")
    );

    /// <summary>
    /// Determines the penalty type based on Monster Island mood
    /// Penalty 0 und 1 nur wenn Mood >= 50%, darunter immer Penalty 2
    /// </summary>
    public int DeterminePenalty()
    {
        int penalty = -1;
        float current_mood = MonsterIsland.instance.GetMood();

        // Penalty basierend auf Mood:
        // Mood < 0.5 = unter 50% -> Penalty 2 (schlimme Strafe)
        // Mood >= 0.5 und Mood < 0.75 -> Penalty 1 (mittlere Strafe)
        // Mood >= 0.75 -> Penalty 0 (milde Strafe)
        if (current_mood < 0.5f)
        {
            penalty = 2;
        }
        else if (current_mood < 0.75f)
        {
            penalty = 1;
        }
        else
        {
            penalty = 0;
        }

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
    public void PlayPenaltyCutscene(int penalty, bool with_poisoning = false)
    {
        Debug.Print("Penality: " + penalty + " | " + with_poisoning);
        if (!with_poisoning)
        {
            if (penalty == 0)
            {
                if (TranslationServer.GetLocale() == "de")
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_DE"
                    );
                else
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_ENG"
                    );
            }
            else if (penalty == 1)
            {
                if (TranslationServer.GetLocale() == "de")
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_1_DE"
                    );
                else
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_1_ENG"
                    );
            }
            else if (penalty == 2)
            {
                if (TranslationServer.GetLocale() == "de")
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_2_DE"
                    );
                else
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_2_ENG"
                    );
            }
        }
        else
        {
            if (penalty == 0)
            {
                if (TranslationServer.GetLocale() == "de")
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_Poisoning_DE"
                    );
                else
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_Poisoning_ENG"
                    );
            }
            else if (penalty == 1)
            {
                if (TranslationServer.GetLocale() == "de")
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_Poisoning_1_DE"
                    );
                else
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_Poisoning_1_ENG"
                    );
            }
            else if (penalty == 2)
            {
                if (TranslationServer.GetLocale() == "de")
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_Poisoning_2_DE"
                    );
                else
                    CutsceneManager.instance.QueueCutscene(
                        quest_dialogue,
                        "Quest_Not_Completed_Poisoning_2_ENG"
                    );
            }
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
