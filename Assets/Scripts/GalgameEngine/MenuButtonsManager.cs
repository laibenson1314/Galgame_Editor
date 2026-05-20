using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonsManager : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("LoadStory"))
            transform.GetChild(1).GetComponent<Button>().enabled = false;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.DeleteKey("LoadStory");
    }

    public void Continue()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}