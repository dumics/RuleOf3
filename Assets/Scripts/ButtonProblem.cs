using UnityEngine;
using UnityEngine.UI;

public class ButtonProblem : MonoBehaviour
{
    public Button yourButton;
    public int sceneIndex;

    public bool exit;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if(exit) Application.Quit();
        Debug.Log("You have clicked the button!");
        SceneControler.LoadSceneByIndex(sceneIndex);
    }

}
