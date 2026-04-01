using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        if (TutorialProgress.Instance == null) yield break;
        if (TutorialPopupManager.Instance == null) yield break;

        if (!TutorialProgress.Instance.inventoryShown)
        {
            TutorialPopupManager.Instance.ShowPopup("Press TAB to open your inventory.");
            TutorialProgress.Instance.inventoryShown = true;

            yield return new WaitForSeconds(TutorialPopupManager.Instance.showDuration + 0.5f);
        }

        if (!TutorialProgress.Instance.showedToolScroll)
        {
            TutorialPopupManager.Instance.ShowPopup("Use the scroll wheel to change tools.");
            TutorialProgress.Instance.showedToolScroll = true;
        }
    }
}


