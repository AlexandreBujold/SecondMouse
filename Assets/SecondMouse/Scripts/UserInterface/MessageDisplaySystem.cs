using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageDisplaySystem : MonoBehaviour
{
    public Message[] messages;
    public TextMeshProUGUI textBox;

    private void Start()
    {
        textBox.enabled = false;
        DisplayMessage(0);
    }

    public void DisplayMessage(int index)
    {
        StartCoroutine(LoadMessage(index));
    }

    public IEnumerator LoadMessage(int index)
    {
        textBox.enabled = true;
        Debug.Log("Coroutine Start");
        textBox.text = messages[index].messageText;
        yield return new WaitForSeconds(messages[index].displayduration);
        textBox.enabled = false;
        Debug.Log("Coroutine End");
    }

    private float Normal(float current, float max)
    {
        return current / max;
    }
}
[System.Serializable]
public class Message
{
    public string messageText;
    public float displayduration;
}
