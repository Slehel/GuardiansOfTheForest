using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowVictory()
    {
        gameObject.SetActive(true);
        resultText.text = "Victory!\nThe forest is safe!";
    }

    public void ShowDefeat()
    {
        gameObject.SetActive(true);
        resultText.text = "Defeat...\nThe corruption wins.";
    }

    public void OnRetryClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
