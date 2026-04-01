using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialPopupManager : MonoBehaviour
{
    public static TutorialPopupManager Instance { get; private set; }

    [Header("UI")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    [Header("Timing")]
    public float showDuration = 3f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        if (popupPanel == null || popupText == null)
        {
            Debug.LogWarning("TutorialPopupManager UI references missing.");
            return;
        }

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowPopupRoutine(message));
    }

    private IEnumerator ShowPopupRoutine(string message)
    {
        popupPanel.SetActive(true);
        popupText.text = message;

        yield return new WaitForSeconds(showDuration);

        popupPanel.SetActive(false);
        currentRoutine = null;
    }
}
