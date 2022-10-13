using UnityEngine;

public class SimpleAnimator : Activatable {
    public float initialDelay;
    public bool running = false;

    protected virtual void Start() {
        Invoke("Activate", initialDelay);
        Init();
    }

    override public void Activate() {
        this.running = true;
    }

    override public void ActivateSwitch(bool b) {
        this.running = b;
    }

    void Update() {
        if (!Application.isPlaying) {
            Draw();
        }
    }

    void LateUpdate() {
        if (!running) return;
        Draw();
    }

    virtual protected void Draw() {

    }

    virtual protected void Init() {

    }

    void OnValidate() {
        if (initialDelay > 0) {
            running = false;
        }
    }
}
