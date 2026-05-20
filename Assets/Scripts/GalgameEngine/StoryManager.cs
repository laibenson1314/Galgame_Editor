using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Background;
    [SerializeField] private Transform CharactersFolder;
    [SerializeField] private Transform ButtonsFolder;
    [SerializeField] private ContentShowing contentShowing;
    [SerializeField] private GameObject switchScene;
    [SerializeField] private Button buttonPrefab;

    public string nextStory;

    string[] _scripts;
    string _path;
    int _currentIndex;

    [HideInInspector] public bool isChoising;

    public void NextStep()
    {
        SetUp();
        if(_currentIndex >= _scripts.Length)
        {
            string line = _scripts[_currentIndex - 1];
            if (!line.StartsWith("#jump") && !line.StartsWith("#end"))
                Debug.LogWarning($"[StoryManager] Story ended, theres not any #jump or #end here");
            //switchScene.SetActive(true);
            return;
        }
        string[] parts = _scripts[_currentIndex].Split(":");
        if (parts.Length == 2) contentShowing.ShowText(parts[0].TrimEnd(), parts[1].TrimStart());
        else Debug.LogError($"[StoryManager] Invalid command at story {Path.GetFileNameWithoutExtension(_path)} line {_currentIndex + 1}.\r\n" +
            $"Expected format: character_name: text");
        _currentIndex++;
    }

    public void StartStory(string path)
    {
        PlayerPrefs.SetString("LoadStory", Path.GetFileNameWithoutExtension(path));

        _path = path;
        _scripts = File.ReadAllLines(path)
            .ToList()
            .Where(x => x.Length > 0)
            .ToArray();
        _currentIndex = 0;
        NextStep();
    }

    private void SetUp()
    {
        bool loopping = true;
        do
        {
            if(_currentIndex >= _scripts.Length) break;
            string currentLine = _scripts[_currentIndex];
            if (currentLine.StartsWith("//"))
            {
                _currentIndex++;
                continue;
            }
            else if (currentLine.StartsWith("#"))
            {
                string[] parts = currentLine[1..].Split(' ');
                Compiler(parts[0], parts[1..]);
                if (parts[0] == "scene")
                {
                    if (CheckArgs("SwitchBackground", parts[1..].Length, parts[0])) return;
                    ChangeScene(parts[1]);
                }
                _currentIndex++;
            }
            else if (currentLine.StartsWith("*"))
            {
                string line;
                do
                {
                    line = _scripts[_currentIndex];
                    _currentIndex++;
                } while (line.StartsWith("*"));
            }
            else loopping = false;
        } while (loopping);
    }

    private void Compiler(string cmd, string[] arg)
    {
        switch (cmd)
        {
            case "bg":
                if(CheckArgs("SwitchBackground", arg.Length, cmd)) return;
                SwitchBackground(arg[0]);
                break;
            case "bgm":
                if (CheckArgs("PlayBackgroundMusic", arg.Length, cmd)) return;
                PlayBackgroundMusic(arg[0], arg[1]);
                break;
            case "stopbgm":
                if(CheckArgs("StopBackgroundMusic", arg.Length, cmd)) return;
                StopBackgroundMusic(arg[0]);
                break;
            case "enter":
                if(CheckArgs("CharacterEnter", arg.Length, cmd)) return;
                CharacterEnter(arg[0], arg[1]);
                break;
            case "exit":
                if(CheckArgs("CharacterExit", arg.Length, cmd)) return;
                CharacterExit(arg[0]);
                break;
            case "change":
                if(CheckArgs("CharacterChangeSprite", arg.Length, cmd)) return;
                CharacterChangeSprite(arg[0], arg[1]);
                break;
            case "jump":
                if(CheckArgs("JumpToPart", arg.Length, cmd)) return;
                JumpToPart(arg[0]);
                break;
            case "choice":
                SpawnChoiceButton();
                break;
            case "end":
                if(CheckArgs("ChangeScene", arg.Length, cmd)) return;
                StoryEnd(arg[0]);
                break;
        }
    }
    private void SwitchBackground(string backgroundName)
    {
        if(!TryLoadSprite(LoadBackground(backgroundName), "Background", out Sprite sprite)) return;
        Background.sprite = sprite;
    }
    private void PlayBackgroundMusic(string bgmName, string volumnString)
    {
        if(!int.TryParse(volumnString, out int volumn) || volumn < 0 || volumn > 100)
        {
            Debug.LogError($"[StoryManager] Invalid volume format at story {Path.GetFileNameWithoutExtension(_path)} line {_currentIndex + 1}. " +
                $"\r\n Expected format: 0-100");
            return;
        }
        AudioClip clip = Resources.Load<AudioClip>($"Audios/{bgmName}");
        if (clip == null)
        {
            Debug.LogError($"[StoryManager] Background music {bgmName} not found!");
            return;
        }
        BackgroundMusicManager.Instance.Play(clip, volumn);
    }
    private void StopBackgroundMusic(string bgmName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{bgmName}");
        if (clip == null)
        {
            Debug.LogError($"[StoryManager] Background music {bgmName} not found!");
            return;
        }
        BackgroundMusicManager.Instance.Stop();
    }
    private void CharacterEnter(string characterName, string characterStatus)
    {
        if (!TryLoadSprite(LoadCharacter(characterName, characterStatus), "Character", out Sprite sprite)) return;
        GameObject character = new GameObject(characterName);
        character.AddComponent<SpriteRenderer>().sprite = sprite;
        character.GetComponent<SpriteRenderer>().sortingOrder = 10;
        character.transform.localScale = new Vector2(1.57f, 1.57f);
        character.transform.position = new Vector2(0f, -1.22f);
        character.transform.parent = CharactersFolder;
    }
    private void CharacterExit(string characterName)
    {
        if(CharactersFolder.Find(characterName) == null)
        {
            Debug.LogError($"[StoryManager] Character {characterName} not found in Story!");
            return;
        }
        Destroy(CharactersFolder.Find(characterName).gameObject);
    }
    private void CharacterChangeSprite(string characterName, string characterStatus)
    {
        if (CharactersFolder.Find(characterName) == null)
        {
            Debug.LogError($"[StoryManager] Character {characterName} not found in Story!");
            return;
        }
        if (!TryLoadSprite(LoadCharacter(characterName, characterStatus), "Character", out Sprite sprite)) return;
        GameObject character = CharactersFolder.Find(characterName).gameObject;
        character.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    private void JumpToPart(string part)
    {
        nextStory = part;
        contentShowing.pausing = true;
        switchScene.SetActive(true);
        Invoke(nameof(stopContentPausing), 1f);
    }private void stopContentPausing() => contentShowing.pausing = false;
    private void SpawnChoiceButton()
    {
        contentShowing.pausing = true;
        _currentIndex++;
        do
        {
            if(!TryParseChoise(_scripts[_currentIndex], out string left, out string right))
            {
                _currentIndex++;
                continue;
            }

            var btn = Instantiate(buttonPrefab, ButtonsFolder);
            btn.GetComponentInChildren<Text>().text = left;
            btn.onClick.AddListener(() =>
            {
                _currentIndex = _scripts.ToList().IndexOf($"*{right}") + 1;
                contentShowing.pausing = false;
                foreach (Transform child in ButtonsFolder) Destroy(child.gameObject);
                NextStep();
            });

            _currentIndex++;
        } while (_scripts[_currentIndex] != "#choice");
    } 
    private bool TryParseChoise(string input, out string left, out string right)
    {
        string[] args = input.Split("=>");
        if (args.Length != 2)
        {
            Debug.LogError($"[StoryManager] Invalid choice command format at story {Path.GetFileNameWithoutExtension(_path)} line {_currentIndex + 1}." +
                $"\r\n Expected format: ButtonText => TargetPart");
            left = right = null;
            return false;
        }
        if(args[1].TrimStart() != args[1].Trim())
        {
            Debug.LogError($"[StoryManager] Invalid part format at story {Path.GetFileNameWithoutExtension(_path)} line {_currentIndex + 1}. " +
                $"\r\n Expected format: my_part");
            left = right = null;
            return false;
        }

        left = args[0];
        right = args[1].Trim();
        return true;
    }
    private void ChangeScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) == null)
        {
            Debug.LogError($"Scene {sceneName} not found! Make sure it's added to the build settings.");
            return;
        }
        SceneController.Instance.SwitchScene("TestScene");
    }
    private void StoryEnd(string endName)
    {
        if(!TryLoadSprite(LoadEnd(endName), "End image", out Sprite end)) return;
        foreach (Transform child in CharactersFolder) Destroy(child.gameObject);
        contentShowing.gameObject.SetActive(false);
        Background.sprite = end;
    }
    private bool CheckArgs(string methodName, int argCount, string cmd)
    {
        var method = typeof(StoryManager).GetMethod(
            methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (method == null)
        {
            Debug.LogError($"[StoryManager] Method '{methodName}' not found!");
            return true;
        }

        int expectedCount = method.GetParameters().Length;
        if (expectedCount != argCount)
        {
            Debug.LogError($"[StoryManager] Command '{cmd}' requires {expectedCount} arguments, but got {argCount}!");
            return true;
        }

        return false;
    }

    bool TryLoadSprite(Sprite sprite, string cmd, out Sprite outSprite)
    {
        if(sprite == null)
        {
            Debug.LogError($"[StoryManager] {cmd} not found!");
            outSprite = null;
            return false;
        }
        outSprite = sprite;
        return true;
    }
    Sprite LoadCharacter(string characterName, string characterStatus) => Resources.Load<Sprite>($"Characters/{characterName}/{characterStatus}");
    Sprite LoadBackground(string backgroundName) => Resources.Load<Sprite>($"Backgrounds/{backgroundName}");
    Sprite LoadEnd(string endName) => Resources.Load<Sprite>($"Endings/{endName}");
}