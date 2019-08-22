using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI totalScoreText;

    [SerializeField]
    private GameObject gameOverPanel;

    private void OnEnable()
    {
        HexManager.SetTotalScoreAction += SetTotalScoreText;
        HexObject.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        HexManager.SetTotalScoreAction -= SetTotalScoreText;
        HexObject.GameOver -= OnGameOver;
    }

    void SetTotalScoreText(int totalScore)
    {
        totalScoreText.text = totalScore.ToString();

    }

    void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void Quit()
    {
        Application.Quit();
    }

}
