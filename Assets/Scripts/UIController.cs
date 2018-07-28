using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {
    public Transform UserLoginPanel;
    public Transform ModelSelectPanel;
    public Transform ModelViewPanel;
    public Transform ModelStatsPanel;

    private Transform ActivePanel;

    private void OnEnable()
    {
        ActivePanel = UserLoginPanel;
        ActivePanel.gameObject.SetActive(true);

        ModelSelectPanel.gameObject.SetActive(false);
        ModelViewPanel.gameObject.SetActive(false);
        ModelStatsPanel.gameObject.SetActive(false);
    }

    public void SwitchPanel(Transform nextPanel)
    {
        ActivePanel.gameObject.SetActive(false);
        ActivePanel = nextPanel;
        ActivePanel.gameObject.SetActive(true);
    }
}
