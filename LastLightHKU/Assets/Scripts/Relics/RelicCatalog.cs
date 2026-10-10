using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LastLight/Relic Catalog")]
public class RelicCatalog : ScriptableObject
{
    public List<RelicDefinition> relics = new();

    public int Count => relics != null ? relics.Count : 0;

    public RelicDefinition Get(RelicId id)
    {
        if (relics == null) return null;
        for (int i = 0; i < relics.Count; i++)
        {
            if (relics[i] != null && relics[i].id == id)
                return relics[i];
        }
        return null;
    }
}
