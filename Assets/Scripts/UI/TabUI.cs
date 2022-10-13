using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabUI : MonoBehaviour {
    bool started;

    public Transform tabContainer;
    public GameObject tabPrefab;
    public Transform screenContainer;
    public bool keyNavigation = true;
    public AudioClip tabSwitchSound;

    List<GameObject> screens = new List<GameObject>();
    int currentTab = 0;

    void Update() {
        if (!keyNavigation) return;
        if (InputManager.ButtonDown(Buttons.TABLEFT)) {
            PreviousTab();
        }
        else if (InputManager.ButtonDown(Buttons.TABRIGHT)) {
            NextTab();
        }
    }

    void Start() {
        // called once! after every game object is loaded, after first OnEnable
        started = true;
        OnEnable();
    }

    // called whenever enabled
    void OnEnable() {
        if (!started) return;
        InitializeUI();
    }

    void OnDisable() {
        foreach (Transform t in screenContainer) {
            if (screens.Contains(t.gameObject)) {
                t.gameObject.SetActive(true);
            }
        }
        ClearTabs();
    }

    void InitializeUI() {
        ClearTabs();
        LinkSubscreens();
        ShowTab(currentTab, quiet: true);
    }

    void ClearTabs() {
        foreach (Transform child in tabContainer.transform) {
            Destroy(child.gameObject);
        }
    }

    void LinkSubscreens() {
        screens.Clear();
        int currentChild = 0;
        foreach (Transform child in screenContainer) {
            GameObject g = child.gameObject;
            if (g.activeSelf) {
                screens.Add(g);
                AddTab(g.name, currentChild);
            }
            currentChild++;
        }
    }

    void AddTab(string tabName, int tabNum) {
        GameObject t = Instantiate(tabPrefab, Vector3.zero, Quaternion.identity, tabContainer);
        t.name = tabName;
        t.GetComponentInChildren<Text>().text = tabName;
        Button b = t.GetComponentInChildren<Button>();
        b.onClick.AddListener(delegate { ShowTab(tabNum); });
    }

    public void NextTab() {
        ShowTab(++currentTab);
    }

    public void PreviousTab() {
        ShowTab(--currentTab);
    }

    void ShowTab(int tabNumber, bool quiet=false) {
        if (!quiet && tabSwitchSound!=null) {
            SoundManager.UISound(tabSwitchSound);
        }
        HideAll();
        DeselectOtherTabs();

        currentTab = tabNumber % screens.Count;
        if (currentTab < 0) currentTab = screens.Count-1;
        GameObject currentTabObj = screens[currentTab];
        currentTabObj.SetActive(true);

        Button b = tabContainer.transform.GetChild(currentTab).GetComponent<Button>();
        b.Select();
        b.OnSelect(null);
        b.GetComponent<Animator>().SetBool("Active", true);
    }

    void HideAll() {
        foreach (Transform t in screenContainer) {
            t.gameObject.SetActive(false);
        }
    }

    void DeselectOtherTabs() {
        foreach (Transform tab in tabContainer) {
            tab.GetComponent<Animator>().SetBool("Active", false);
        }
    }

}
