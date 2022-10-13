using UnityEngine;
using System.Collections;

public class CloseableUI : MonoBehaviour {
    
    public bool invincibleDuring;
    public GameObject targetUI;
    public bool interactSound = true;
    public bool stopTime = false;
    public bool exclusive = true;
    public bool closeOnGenericEscape = false;
    public bool useSelf = false;
    public bool closeAtStart = false;
    public bool soloUISound = false;
    public bool continuousCutscene = false;

    public bool debug = false;

    protected bool open;
    protected bool started;

    virtual public void Open() {
        if ((exclusive && GlobalController.openUIs > 0)) {
            return;
        }
        if (debug) Debug.Log(gameObject.name + " opened");

        if (!open) GlobalController.openUIs += 1;
        this.open = true;
        Hitstop.Interrupt();
        if (interactSound) SoundManager.InteractSound();
        GlobalController.pc.EnterCutscene(invincible:invincibleDuring);
        if (targetUI != null) targetUI.SetActive(true);
        if (stopTime) Time.timeScale = 0f;
        if (soloUISound) SoundManager.SoloUIAudio();
    }

    virtual public void Close() {
        if (open) GlobalController.openUIs -= 1;

        if (stopTime) Time.timeScale = 1f;
        this.open = false;
        if (debug) Debug.Log(gameObject.name + " closed");
        // avoid race conditions
        if (GlobalController.pc.gameObject.activeInHierarchy) GlobalController.pc.ExitCutscene();
        if (targetUI != null) targetUI.SetActive(false);
        if (soloUISound) SoundManager.DefaultAudio();
    }

    void Start() {
        started = true;
        OnEnable();
        if (closeAtStart) gameObject.SetActive(false);
    }

    void OnEnable() {
        if (useSelf && started) {
            Open();
        }
    }
    
    void OnDisable() {
        if (useSelf) {
            Close();
        }
    }

    void Update() {
        if (open && closeOnGenericEscape && InputManager.GenericEscapeInput()) {
            StartCoroutine(WaitAndClose());
        }
        if (stopTime) Time.timeScale = 0f;
        if (continuousCutscene) GlobalController.pc.EnterCutscene();
    }

    // again, deal with input frame timing
    protected IEnumerator WaitAndClose() {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}
