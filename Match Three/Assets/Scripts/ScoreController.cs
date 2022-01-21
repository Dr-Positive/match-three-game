using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ScoreLine;
    public GameObject ScorePlayer1;
    public GameObject ScorePlayer2;
    public float ScoreLineWidth;
    public RectTransform CurrentScoreRectP1;
    public RectTransform CurrentScoreRectP2;
    public float CurrentScoreWidthP1;
    public float CurrentScoreWidthP2;
    public float TargetScoreWidthP1;
    public float TargetScoreWidthP2;

    public Text ScorePlayer1Text;
    public Text ScorePlayer2Text;

    void Start()
    {
        ScoreLine = transform.Find("ScoreLine").gameObject;
        ScorePlayer1Text = transform.Find("Score1").GetComponent<Text>();
        ScorePlayer2Text = transform.Find("Score2").GetComponent<Text>();
        ScoreLineWidth = ScoreLine.GetComponent<RectTransform>().rect.width;
        ScorePlayer1 = ScoreLine.transform.Find("ScorePlayer1").gameObject;
        ScorePlayer2 = ScoreLine.transform.Find("ScorePlayer2").gameObject;
        CurrentScoreRectP1 = ScorePlayer1.GetComponent<RectTransform>();
        CurrentScoreRectP2 = ScorePlayer2.GetComponent<RectTransform>();
        CurrentScoreWidthP1 = CurrentScoreRectP1.rect.width;
        CurrentScoreWidthP2 = CurrentScoreRectP2.rect.width;
        TargetScoreWidthP1 = CurrentScoreWidthP1;
        TargetScoreWidthP2 = CurrentScoreWidthP2;
        //ScorePlayer1.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(TargetScoreWidthP1 - CurrentScoreWidthP1) > 0.1f)
        {
            CurrentScoreWidthP1 += TargetScoreWidthP1 > CurrentScoreWidthP1 ? 0.1f : -0.1f;
            CurrentScoreRectP1.sizeDelta = new Vector2(CurrentScoreWidthP1, 30);
        }
        if (Mathf.Abs(TargetScoreWidthP2 - CurrentScoreWidthP2) > 0.1f)
        {
            CurrentScoreWidthP2 += TargetScoreWidthP2 > CurrentScoreWidthP2 ? 0.1f : -0.1f;
            CurrentScoreRectP2.sizeDelta = new Vector2(CurrentScoreWidthP2, 30);
        }
        var allPlayers = GameObject.FindGameObjectsWithTag("Player");
        var unitsP1 = allPlayers
            .Where(item => item.GetComponent<PlayerController>().team == PlayerController.Team.Player1)
            .ToList()
            .Count;
        var unitsP2 = allPlayers
            .Where(item => item.GetComponent<PlayerController>().team == PlayerController.Team.Player2)
            .ToList()
            .Count;
        float dx = 980.0f / (unitsP1 + unitsP2);
        //Debug.Log(dx);
        ScorePlayer1Text.text = unitsP1.ToString();
        ScorePlayer2Text.text = unitsP2.ToString();
        TargetScoreWidthP1 = dx * unitsP1;
        TargetScoreWidthP2 = dx * unitsP2;
    }

}
