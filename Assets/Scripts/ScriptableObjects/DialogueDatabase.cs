using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueDatabase", menuName = "Dialogue/Database")]

public class DialogueDatabase : ScriptableObject
{
    public DialogueEntry[] entries;
}
