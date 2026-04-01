using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        if (TutorialProgress.Instance == null) yield break;
        if (TutorialProgress.Instance.inventoryShown) yield break;
        if (TutorialPopupManager.Instance == null) yield break;

        TutorialPopupManager.Instance.ShowPopup("Press TAB to open your menu and inventory.");
        TutorialProgress.Instance.inventoryShown = true;
    }
}


