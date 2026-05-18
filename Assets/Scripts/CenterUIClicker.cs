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
        // 화면 중앙을 클릭 위치로 정함
        pointerEventData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

        raycastResults.Clear();
        // 화면 중앙에 UI가 있는지 검사
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
                // 찾은 버튼의 클릭 기능을 실행
                button.onClick.Invoke();

                Debug.Log("버튼 클릭 실행: " + button.gameObject.name);
                return;
            }
        }

        Debug.Log("UI는 감지됐지만 Button 컴포넌트를 찾지 못함");
    }
}