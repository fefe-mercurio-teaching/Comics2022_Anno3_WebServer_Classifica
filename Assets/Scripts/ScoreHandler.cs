using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] private Text _scoreEntryTemplate;

    [SerializeField] private string _userName;
    [SerializeField] private int _score;
    
    private void Start()
    {
        ScoreEntries entries; //
        try
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://localhost:3000/score");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                entries = JsonUtility.FromJson<ScoreEntries>(reader.ReadToEnd());
            }

            foreach (var entry in entries.entries)
            {
                Text newText = Instantiate(_scoreEntryTemplate, _scoreEntryTemplate.transform.parent);
                newText.text = $"{entry.name}: {entry.score}";
                newText.gameObject.SetActive(true);
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Impossibile connettersi");
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(SendDataToServer());
        }
    }

    IEnumerator SendDataToServer()
    {
        Dictionary<string, string> requestParams = new Dictionary<string, string>();
        requestParams.Add("name", _userName);
        requestParams.Add("score", _score.ToString());

        UnityWebRequest request = UnityWebRequest.Post("http://localhost:3000/set-score", requestParams);

        yield return request.SendWebRequest();
        
        Debug.Log(request.downloadHandler.text); 
    }
}
