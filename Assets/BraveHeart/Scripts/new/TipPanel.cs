using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TipPanel : MonoBehaviour
{
    public GameObject messagePanel;
    public Text messageText;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowingMessage = false;
    public CanvasGroup canvasGroup;
    public float fadeInDuration = 1f;
    public float displayDuration = 2f;
    public float fadeOutDuration = 1f;

    public System.Action OnAllMessagesShown; 

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isShowingMessage)
        {
            ShowNextMessage();
        }
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        messagePanel.SetActive(true);
        messageText.DOText( message,2);

        // 
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = elapsedTime / fadeInDuration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        // 
        elapsedTime = 0f;
        while (elapsedTime < displayDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = 1f - (elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // 
        messagePanel.SetActive(false);

        isShowingMessage = false;
        if (messageQueue.Count > 0)
        {
            ShowNextMessage();
        }
        else if (OnAllMessagesShown != null)
        {
            OnAllMessagesShown.Invoke();
        }
    }

    private void ShowNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            isShowingMessage = true;
            StartCoroutine(ShowMessageCoroutine(messageQueue.Dequeue()));
        }
    }

    private void OnEnable()
    {
        if (!isShowingMessage && messageQueue.Count > 0)
        {
            ShowNextMessage();
        }
    }

    private void OnDisable()
    {
        messageQueue.Clear();
    }
}