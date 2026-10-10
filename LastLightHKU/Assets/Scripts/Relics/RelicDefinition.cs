using UnityEngine;

[CreateAssetMenu(menuName = "LastLight/Relic Definition")]
public class RelicDefinition : ScriptableObject
{
    public RelicId id;

    [Header("UI")]
    public string displayName;
    [TextArea] public string description;

    public Sprite icon;          // shown when found
    public Sprite silhouetteIcon; // shown when not found (optional)
}
