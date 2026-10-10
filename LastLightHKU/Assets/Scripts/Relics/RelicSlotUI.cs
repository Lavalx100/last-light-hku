using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicSlotUI : MonoBehaviour
{
    public Image icon;
    public Image lockOverlay;
    public TMP_Text nameText;

    RelicDefinition def;

    public void Bind(RelicDefinition definition)
    {
        def = definition;
        if (nameText != null)
            nameText.text = def != null ? def.displayName : "";
    }

    public void SetCollected(bool collected)
    {
        if (def == null) return;

        if (icon != null)
        {
            if (collected)
                icon.sprite = def.icon;
            else
                icon.sprite = def.silhouetteIcon != null ? def.silhouetteIcon : def.icon;
            icon.enabled = icon.sprite != null;
        }

        if (lockOverlay != null)
            lockOverlay.gameObject.SetActive(!collected);
    }
}
