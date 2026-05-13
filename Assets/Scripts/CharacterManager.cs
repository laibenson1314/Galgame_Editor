using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private float smoothMoveSpeed = .01f;

    private void Update()
    {
        int count = transform.childCount;
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;

        for (int i = 0; i < count; i++)
        {
            float t = (float)(i + 1) / (count + 1);
            float x = -screenWidth / 2f + screenWidth * t;
            var child = transform.GetChild(i);
            child.transform.position = Vector3.Lerp(child.position, new Vector3(x, -1.22f, 0f), smoothMoveSpeed);
        }
    }
}
