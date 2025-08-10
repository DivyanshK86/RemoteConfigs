using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LenientBricker : MonoBehaviour
{
    [Tooltip("Direct URL to the text file containing the expiration date in yyyy-MM-dd format.")]
    public string url;

    void Start()
    {
        StartCoroutine(CheckExpiration());
    }

    IEnumerator CheckExpiration()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("Failed to download expiration date: " + www.error);
                CheckLocalBrickedState();
                yield break;
            }

            if(www.downloadHandler.text == "0")
            {
                PlayerPrefs.SetInt("IsSoftwareValid", 0);
                CheckLocalBrickedState();
            }
            else
            {
                PlayerPrefs.SetInt("IsSoftwareValid", 1);
                Debug.Log("All good!");
            }
        }

        CheckLocalBrickedState();
    }

    void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        while (true)
        {
            string s = "";
        }
        Application.Quit();
#endif
    }

    private void CheckLocalBrickedState()
    {
        if(PlayerPrefs.GetInt("IsSoftwareValid", 1) == 0)
        {
            Debug.Log("Quitting");
            QuitApplication();
        }
    }
}
