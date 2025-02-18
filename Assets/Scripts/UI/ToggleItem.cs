using TMPro;
using UnityEngine;

public class ToggleItem : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private TextMeshProUGUI toggleText;

    [SerializeField] private bool defaultOn;
    public int selectedValue;

    private void Start()
    {
        toggleGroup.Subscribe(this);
        if (defaultOn) OnToggleClicked();
    }

    public void OnToggleClicked()
    {
        toggleGroup.ToggleClicked(this);
    }

    public void SetToggle(bool selected)
    {
        if (selected) toggleText.text = "X";
        else toggleText.text = "";
    }
}
