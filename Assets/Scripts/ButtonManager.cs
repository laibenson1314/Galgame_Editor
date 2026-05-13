using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private float smoothMoveSpeed = .01f;
    [SerializeField] private float layoutHeight = 400f; // ±±¨î«ö¶s¤À§Gªº½d³ò

    private void Update()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            float t = (float)(i + 1) / (count + 1);
            float y = -layoutHeight / 2f + layoutHeight * t;
            var child = transform.GetChild(i);
            RectTransform rect = child.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, new Vector2(0f, y), smoothMoveSpeed);
        }
    }
}
