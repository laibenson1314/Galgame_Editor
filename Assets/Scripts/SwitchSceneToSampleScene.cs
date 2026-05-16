using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneToSampleScene : MonoBehaviour
{
    public void BackToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
