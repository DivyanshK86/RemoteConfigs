using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ExpirationDateChecker : MonoBehaviour
{
    [Tooltip("Direct URL to the text file containing the expiration date in yyyy-MM-dd format.")]
    public string expirationDateURL;

    void Start()
    {
        StartCoroutine(CheckExpiration());
    }

    IEnumerator CheckExpiration()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(expirationDateURL))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("Failed to download expiration date: " + www.error);
                QuitApplication();
                yield break;
            }

            string dateString = www.downloadHandler.text.Trim();

            if (DateTime.TryParse(dateString, out DateTime expirationDate))
            {
                DateTime currentDate = DateTime.UtcNow.Date;

                if (currentDate > expirationDate)
                {
                    Debug.LogWarning($"Application expired on {expirationDate:yyyy-MM-dd}. Current date is {currentDate:yyyy-MM-dd}. Quitting...");
                    QuitApplication();
                }
                else
                {
                    Debug.Log($"Application is valid. Expires on {expirationDate:yyyy-MM-dd}.");
                }
            }
            else
            {
                Debug.LogError("Invalid date format in downloaded file.");
                QuitApplication();
            }
        }
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
}
