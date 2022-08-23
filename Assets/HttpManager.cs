using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private string URL;
    [SerializeField] Transform texts;

    string token_;
    string username_;
    int score_;

    private void Start()
    {
        token_ = PlayerPrefs.GetString("token");
        username_ = PlayerPrefs.GetString("username");
        score_ = PlayerPrefs.GetInt("score");

        print("username: " + username_ + ", token: " + token_ + ", score: " + score_);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(GetPerfil());
        }
        UploadNewScore();
    }

    public void ClickSignUp()
    {
        string postData = GetInputData();
        StartCoroutine(SignUp(postData));
    }
    public void ClickLogIn()
    {
        string postData = GetInputData();
        StartCoroutine(LogIn(postData));
    }
    public void UploadNewScore()
    {
        ScoreData data = new ScoreData();

        data.username = username_;
        data.score = score_;

        string postData = JsonUtility.ToJson(data);
        StartCoroutine(SetScore(postData));
    }
    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }
    private string GetInputData()
    {
        AuthData data = new AuthData();

        data.username = GameObject.Find("UsernameField").GetComponent<InputField>().text;
        data.password = GameObject.Find("PasswordField").GetComponent<InputField>().text;

        string postData = JsonUtility.ToJson(data);
        return postData;
    }
    IEnumerator SignUp(string postData)
    {
        //Debug.Log(postData);
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            //bienvenido 
            print("Registrado" + resData.usuaio.username + ", id:" + resData.usuaio._id);
            StartCoroutine(LogIn(postData));
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator LogIn(string postData)
    {
        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            //bienvenido 
            print("Autenticado " + resData.usuaio.username + ", id:" + resData.usuaio._id + ", token:" + resData.token);
            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuaio.username);
            PlayerPrefs.SetInt("score", resData.usuaio.score);
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator GetPerfil()
    {
        //Debug.Log(postData);
        string url = URL + "/api/usuarios" + username_;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", token_);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            //bienvenido 
            print("Token valido" + resData.usuaio.username + ", id:" + resData.usuaio._id);
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator SetScore(string postData)
    {
        //Debug.Log(postData);
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", token_);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            print(resData.usuaio.username + ", score:" + resData.usuaio.score);
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    IEnumerator GetScores()
    {
        //Debug.Log(postData);
        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", token_);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            Scores resData_ = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            for (int i = 0; i < resData_.usuarios.Length; i++)
            {
                texts.GetChild(i).gameObject.GetComponent<Text>().text = (i+1) + ". " + resData_.usuarios[i].username + ":   " + resData_.usuarios[i].score;
            }
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuaio;
    public string token;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username; 
    public bool estado;
    public int score;
}

[System.Serializable] 
public class Scores
{
    public UserData[] usuarios;
}

[System.Serializable]
public class ScoreData
{
    public string username;
    public int score;
}