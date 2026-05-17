using UnityEngine;

public class SwitchSceneToSampleScene : MonoBehaviour
{
    public void BackToSampleScene()
    {
        SceneController.Instance.ReturnScene();
    }
}