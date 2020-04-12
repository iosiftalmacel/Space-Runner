using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIResultsPage : UIPage {
    public Text lastScore;
    public Text highestScore;
    public Text newRecord;
    public Animator anim;

    protected bool exiting;


    public override void OnShow()
    {
        base.OnShow();
        anim.Play("enter");

        exiting = false;

        int highest = PlayerPrefs.GetInt("highesScore", GameManager.Instance.lastScore);
        int current = GameManager.Instance.lastScore;
        lastScore.text = "Score: " + current.ToString();
        highestScore.text = "Highest Score: " + highest.ToString();

        newRecord.gameObject.SetActive(highest == current);            
    }

    public override void OnHide()
    {
        exiting = true;
        anim.Play("exit");
    }

    public void OnRestartClicked()
    {
        GameManager.Instance.StartGame();
    }

    void Update()
    {
        if (exiting)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("finished"))
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
