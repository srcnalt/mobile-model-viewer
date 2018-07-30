using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public GameObject UserLoginPanel;
    public GameObject ModelSelectPanel;
    public GameObject ModelViewPanel;
    public GameObject ModelStatsPanel;
    public GameObject loadingPanel;
    public GameObject modelListControl;
    public GameObject BottomPanel;
    public GameObject TopPanel;

    public GameObject ScrollContent;
    public GameObject ListItem;
    public ModelLoader model;

    private GameObject ActivePanel;

    private Dictionary<string, string> fakeRepository = new Dictionary<string, string>();

    private void Start()
    {
        ActivePanel = UserLoginPanel;
        ActivePanel.SetActive(true);

        ModelSelectPanel.SetActive(false);
        ModelViewPanel.SetActive(false);
        ModelStatsPanel.SetActive(false);
        BottomPanel.SetActive(false);
        TopPanel.SetActive(false);
        
        fakeRepository.Add("chicken.obj", "https://drive.google.com/uc?authuser=0&id=1LO3u-m8DZhe8zuTDRhXOfG0u3AXOLuiT&export=download");
        fakeRepository.Add("car_4.obj", "https://drive.google.com/uc?authuser=0&id=1h8vN6rjrwtnMJdnT1xWSV9ocTvF8UyeS&export=download");
        fakeRepository.Add("car_5.obj", "https://drive.google.com/uc?authuser=0&id=1GBk1BoT7iOuRZOUA1PyFOW-aLCkSF6-I&export=download");
        fakeRepository.Add("car_6.obj", "https://drive.google.com/uc?authuser=0&id=1ET4i3Fox071zqXLW_KBRF5X1d9zGun3V&export=download");

        //fakeRepository.Add("no_model_for_thread_test_1", "https://drive.google.com/uc?export=download&confirm=L8RE&id=0B6oawVGfb_JyQUo5ZTNubU82ZlU");
        //fakeRepository.Add("no_model_for_thread_test_2", "https://drive.google.com/uc?export=download&confirm=L8RE&id=0B6oawVGfb_JyQUo5ZTNubU82ZlU");
        //fakeRepository.Add("no_model_for_thread_test_3", "https://drive.google.com/uc?export=download&confirm=L8RE&id=0B6oawVGfb_JyQUo5ZTNubU82ZlU");
    }

    public void ActivateMainPanel(GameObject nextPanel)
    {
        ActivePanel.SetActive(false);
        ActivePanel = nextPanel;
        ActivePanel.SetActive(true);
    }

    public void SwitchPanel(GameObject panel)
    {
        bool active = panel.activeSelf;
        panel.SetActive(!active);
    }

    public void DeactivatePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void ActivatePanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void LoadResources()
    {
        StartCoroutine(Co_LoadResources());
    }

    private IEnumerator Co_LoadResources()
    {
        string path = Application.persistentDataPath + "/Models";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);

            //3 is number of embedded items in to the application, all have names such ash car_n
            for (int i = 1; i <= 3; i++)
            {
                WWW www = new WWW(Application.streamingAssetsPath + "/car_" + i + ".obj");
                yield return www;

                File.WriteAllBytes(Application.persistentDataPath + "/Models/car_" + i + ".obj", www.bytes);
            }
        }

        string[] directories = { };

        directories = Directory.GetFiles(Application.persistentDataPath + "/Models");

        foreach (string dir in directories)
        {
            GameObject o = Instantiate(ListItem, ScrollContent.transform);

            Text itemText = o.GetComponentsInChildren<Text>()[0];
            itemText.text = Path.GetFileName(dir);

            Button btn = o.GetComponent<Button>();
            btn.onClick.AddListener(delegate { ActivateMainPanel(ModelViewPanel); });
            btn.onClick.AddListener(delegate { ActivatePanel(BottomPanel); });
            btn.onClick.AddListener(delegate { model.LoadModel(dir); });
        }

        DeactivatePanel(loadingPanel);
        ActivatePanel(modelListControl);

        yield return null;
    }

    public void CheckRepository()
    {
        foreach (KeyValuePair<string, string> webModel in fakeRepository)
        {
            if (!File.Exists(Application.persistentDataPath + "/Models/" + webModel.Key))
            {
                GameObject o = Instantiate(ListItem, ScrollContent.transform);

                Text name = o.GetComponentsInChildren<Text>()[0];
                name.text = webModel.Key;
                Text status = o.GetComponentsInChildren<Text>()[1];
                status.text = "press here to start download";

                Button btn = o.GetComponent<Button>();
                btn.onClick.AddListener(delegate { StartCoroutine(Co_StartDownload(webModel, btn)); });
            }
        }
    }

    private IEnumerator Co_StartDownload(KeyValuePair<string, string> webModel, Button btn)
    {
        /*
         * cannot use WWW in Threads
         * 
         * Error message: Create can only be called from the main thread.
         * Constructors and field initializers will be executed from the loading thread when loading a scene.
         * Don't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function.
         * 
         * 
        bool done = false;
        new Thread(() => {
            while (true)
            {
                www = www = new WWW(webModel.Value);

                Thread.Sleep(10);
            }
        }).Start();
        */

        WWW www = new WWW(webModel.Value);
        
        while (!www.isDone)
        {
            btn.GetComponentsInChildren<Text>()[1].text = string.Format("Downloaded {0:P1}", www.progress);
            yield return new WaitForSeconds(0.1f);
        }

        yield return www;

        File.WriteAllBytes(Application.persistentDataPath + "/Models/" + webModel.Key, www.bytes);

        btn.GetComponentsInChildren<Text>()[1].text = "model is in library";

        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => { ActivateMainPanel(ModelViewPanel); });
        btn.onClick.AddListener(() => { ActivateMainPanel(ModelViewPanel); });
        btn.onClick.AddListener(() => { SwitchPanel(BottomPanel); });
        btn.onClick.AddListener(() => { model.LoadModel(Application.persistentDataPath + "/Models/" + webModel.Key); });
    }
}
