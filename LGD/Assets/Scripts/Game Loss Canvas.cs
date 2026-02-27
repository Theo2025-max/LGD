using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameLossCanvas : MonoBehaviour
{
    [SerializeField] LeanTweenType easetype;
    public int score;
    public TextMeshProUGUI Score_text;
    public TextMeshProUGUI highscore_text;
    public ScoreCount s_count;
    public GameObject newhighscore;
    public ShakeData shake;
    bool can_new_high = false;

    public GameObject[] causes;
    bool can_display = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("HIGHSCORE"))
        {
            PlayerPrefs.SetInt("HIGHSCORE", 0);
        }
        //now_appear();
            /*if (PlayerPrefs.HasKey("HIGHSCORE"))
            {
                highscore_text.text = score.ToString();
            }
            else
            {
                PlayerPrefs.SetInt("HIGHSCORE", score);
                highscore_text.text = score.ToString();
            }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void display_causes(int cause)
    {
        if (can_display)
        {
            causes[cause].SetActive(true);
            can_display = false;
        }
        
    }
    public void start_appearence()
    {
        StartCoroutine(appear());

    }

    private IEnumerator appear()
    {
        yield return new WaitForSeconds(1);
        now_appear();
    }
    
    private IEnumerator present_high_score()
    {
        yield return new WaitForSeconds(1);
        
    }

    public void now_appear()
    {
        LeanTween.scale(gameObject, new Vector3(0.7f, 0.7f, 1f), 0.5f).setEase(easetype);
        //s_count._value = score;
        s_count.UpdateText(score);
        //print(s_count._value);
        if (PlayerPrefs.HasKey("HIGHSCORE"))
        {
            print("highscore there");
            print("highscore is" + PlayerPrefs.GetInt("HIGHSCORE"));
            if (score > PlayerPrefs.GetInt("HIGHSCORE"))
            {
                print("new score is greater");
                PlayerPrefs.SetInt("HIGHSCORE", score);
                highscore_text.text = PlayerPrefs.GetInt("HIGHSCORE").ToString();
                can_new_high = true;
                //newhighscore.SetActive(true);
                //LeanTween.scale(newhighscore, new Vector3(1.4f, 1.4f, 1f), 0.5f).setEase(easetype).setOnComplete(new_high);

                print("highscore is" + PlayerPrefs.GetInt("HIGHSCORE"));
            }
            else 
            {
                print("new score is greater");
               // PlayerPrefs.SetInt("HIGHSCORE", score);
                highscore_text.text = PlayerPrefs.GetInt("HIGHSCORE").ToString();
                
            }
        }
        else
        {
            print("highscore not there, setting up");
            PlayerPrefs.SetInt("HIGHSCORE", score);
            highscore_text.text = PlayerPrefs.GetInt("HIGHSCORE").ToString();
            print("highscore is" + PlayerPrefs.GetInt("HIGHSCORE"));
        }
        
        //Score_text.text = score.ToString();
    }

    public void new_high()
    {
        if (can_new_high)
        {
            newhighscore.SetActive(true);
            CameraShakerHandler.Shake(shake);
            //CameraShakerHandler.Shake(shake);
            //StartCoroutine(present_high_score());
        }
        
    }


}
