using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityCommon
{
    public class SceneSwitcher : MonoBehaviour
    {
        private const int buttonHeight = 50;
        private const int buttonWidth = 150;

        private void OnGUI ()
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var yPos = Screen.height - (buttonHeight + (buttonHeight * i));
                var scene = SceneUtility.GetScenePathByBuildIndex(i).GetAfter("/").GetBefore(".");
                if (GUI.Button(new Rect(0, yPos, buttonWidth, buttonHeight), scene)) SceneManager.LoadScene(i);
            }
        }
    }
}
