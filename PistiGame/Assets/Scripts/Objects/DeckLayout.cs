using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckLayout : HorizontalLayoutGroup
{
    public float rotationAngle = 30f;
    public float maxYOffset = 100f;

    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        ApplyCustomLayout();
    }

    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical();
        ApplyCustomLayout();
    }

    private void ApplyCustomLayout()
    {
        var count = rectChildren.Count(x=>x.gameObject.activeInHierarchy);
        if (count == 0) return;
        for (var i = 0; i < count; i++)
        {
            var t = count == 1 ? 0.5f : (float)i / (count - 1);
            var zRotation = Mathf.Lerp(rotationAngle, -rotationAngle, t);
            var parabolaT = 1f - Mathf.Abs(t - 0.5f) * 2f;
            var yOffset = parabolaT * maxYOffset;
            if (!rectChildren[i].gameObject.activeInHierarchy) continue;
            var child = rectChildren[i];
            child.localRotation = Quaternion.Euler(0f, 0f, zRotation);
            Vector3 anchoredPos = child.anchoredPosition;
            anchoredPos.y = yOffset;
            child.anchoredPosition = anchoredPos;
        }
    }
}

