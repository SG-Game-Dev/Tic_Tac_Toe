using UnityEngine;

public class SetUiPosintion : MonoBehaviour
{
    public RectTransform contectBnt;
    public RectTransform WecLable;

    private bool? lastPortrait;

    void Update()
    {
        if (contectBnt == null || WecLable == null || Screen.width <= 0 || Screen.height <= 0)
            return;

        bool isPortrait = Screen.height > Screen.width;
        if (lastPortrait == isPortrait)
            return;

        lastPortrait = isPortrait;
        contectBnt.anchoredPosition = Vector2.zero;
        WecLable.anchoredPosition = new Vector2(WecLable.anchoredPosition.x,isPortrait ? -100f : 0f);
    }
}
