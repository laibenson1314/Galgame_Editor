using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneToSampleScene : MonoBehaviour
{
    public void BackToSampleScene()
    {
        SceneController.Instance.ReturnScene("SampleScene");
    }
}
