using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CenterUIClicker : MonoBehaviour
{
    public KeyCode clickKey = KeyCode.Mouse0;

    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    private void Update()
    {
        if (Input.GetKeyDown(clickKey))
        {
            ClickCenterButton();
        }
    }

    private void ClickCenterButton()
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("EventSystem이 없습니다.");
            return;
        }

        pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count == 0)
        {
            return;
        }

        foreach (RaycastResult result in raycastResults)
        {
            Button button = result.gameObject.GetComponentInParent<Button>();

            if (button != null)
            {
                button.onClick.Invoke();

                Debug.Log("버튼 클릭 실행: " + button.gameObject.name);
                return;
            }
        }

        Debug.Log("UI는 감지됐지만 Button 컴포넌트를 찾지 못함");
    }
}