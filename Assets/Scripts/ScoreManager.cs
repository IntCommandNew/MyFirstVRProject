using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private DartBoard board;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private DartFlight[] darts;

    private int TotalScore;

    private void Awake()
    {
        if (darts == null || darts.Length == 0)
            darts = FindObjectsByType<DartFlight>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        if (board != null)
            board.OnDartHit.AddListener(AddPoints);
    }

    private void OnDisable()
    {
        if (board != null)
            board.OnDartHit.RemoveListener(AddPoints);
    }

    private void Start()
    {
        UpdateText();
    }

    private void AddPoints(int points)
    {
        TotalScore += points;
        UpdateText();
    }

    public void ResetGame()
    {
        TotalScore = 0;
        UpdateText();

        foreach (DartFlight dart in darts)
            if (dart != null)
                dart.ResetToStart();
    }

    private void UpdateText()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {TotalScore}";
    }
}
