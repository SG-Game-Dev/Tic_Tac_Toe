using UnityEngine;

public class SetUiPosintion : MonoBehaviour
{
    public RectTransform contectBnt;
    public RectTransform WecLable;


    // Update is called once per frame
    void Update()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            print("protrait");
            contectBnt.anchoredPosition = Vector3.zero;
            WecLable.anchoredPosition = new Vector3 (WecLable.anchoredPosition.x, -100f, 0);
        }
        else if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            print("landscap.");
            contectBnt.anchoredPosition = new Vector3(-99.53191f, WecLable.anchoredPosition.x, 0);
            WecLable.anchoredPosition = new Vector3(WecLable.anchoredPosition.x, 0f, 0);
        }
    }
}
