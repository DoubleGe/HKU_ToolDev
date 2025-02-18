using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ToggleGroup : MonoBehaviour
{
    private List<ToggleItem> toggleItems;
    private ToggleItem selectedToggle;

    public UnityEvent<int> OnToggleUpdate;

    public void Subscribe(ToggleItem toggle)
    {
        if (toggle == null) throw new NullReferenceException("Toggle Item is null!");

        if (toggleItems == null) toggleItems = new List<ToggleItem>();

        if (!toggleItems.Contains(toggle))
        {
            toggleItems.Add(toggle);
        }
    }

    public void ToggleClicked(ToggleItem toggle)
    {
        if (toggle == null) throw new NullReferenceException("Toggle Item is null!");

        if (selectedToggle == toggle) return;

        toggleItems.ForEach(t => t.SetToggle(false));

        if (toggleItems.Contains(toggle))
        {
            selectedToggle = toggle;
            toggle.SetToggle(true);
            OnToggleUpdate?.Invoke(toggle.selectedValue);
        }
    }
}
