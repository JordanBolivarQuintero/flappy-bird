using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{

    [SerializeField] private string URL;
    [SerializeField] Transform texts;
    [SerializeField] GameObject board;
    ScoreData[] scoresToShow = new ScoreData[5];

    // Start is called before the first frame update

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    IEnumerator GetScores()
    {
        string url = URL + "/leaders";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            board.SetActive(true);
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);

            foreach (ScoreData score in resData.scores)
            {
                score.position = resData.scores.Length;
                foreach (ScoreData otherScore in resData.scores)
                {
                    if (score.value > otherScore.value)
                    {
                        score.position--;
                    }
                }
                //Debug.Log(score.userId +" | "+ score.value + " | " + score.position);
            }
            foreach (ScoreData score in resData.scores)
            {
                scoresToShow[score.position-1] = score;
            }
            for (int i = 0; i < scoresToShow.Length; i++)
            {
                texts.GetChild(i).gameObject.GetComponent<Text>().text = scoresToShow[i].position + ". " + scoresToShow[i].name + ":   " + scoresToShow[i].value;
            }

        }
        else
        {
            Debug.Log(www.error);
        }
    }
   
}


[System.Serializable]
public class ScoreData
{
    public int userId;
    public int value;
    public string name;
    public int position;
}

[System.Serializable]
public class Scores
{
    public ScoreData[] scores;
}
