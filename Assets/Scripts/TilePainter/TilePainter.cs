using UnityEngine;

public class TilePainter : MonoBehaviour
{
    private PaintTool currentTool;

    private void Update()
    {
        if (currentTool == null) return;

        currentTool.RunTool();
    }

    public void SetTool(PaintTool tool)
    {
        currentTool = tool;
    }
}
