using System;

public class QuestEvent {
    public event Action<string> onStartQuest;
    public event Action<string> onAdvanceQuest;
    public event Action<string> onFinishQuest;
    public event Action<Quest> onQuestStateChange;

    
}
