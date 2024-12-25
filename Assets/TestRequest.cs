using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestRequest : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private Button button;

    private const string URL = "http://localhost:3000/test";

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        Debug.Log("Button was clicked");
        StartCoroutine(SendGetRequest());
    }

    IEnumerator SendGetRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                text.text = request.downloadHandler.text;
            }
            else
            {
                Debug.LogError(request.error);
            }

        }
    }
}
