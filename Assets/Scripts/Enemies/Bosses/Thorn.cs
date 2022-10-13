using UnityEngine;
using System.Collections;

public class Thorn : Boss {

    public GameObject orbitingSwords;
    public GameObject blurredSwords;
    public AudioClip swordSpinNoise;
    public GameObject shieldBreakPrefab;

    public int maxSwords;
    int currentSwords;

    float targetSwordAngle = 0;
    float swordRingVelocity = 0;
    float initialSwordRingAngle = 0;

    public float smoothAmount = 10f;
    public float maxSpinSpeed = 10f;

    public Spinner swordSpinner;
    public bool targetingRotation = true;

    override protected void Start() {
        base.Start();
        maxSwords = orbitingSwords.transform.childCount;
        currentSwords = maxSwords;
        targetSwordAngle = orbitingSwords.transform.localRotation.eulerAngles.z;
        initialSwordRingAngle = targetSwordAngle;
        SpaceSwordRotation();
    }

    [ContextMenu("Space Swords")]
    void SpaceSwordRotation() {
        currentSwords = GetActiveSwords();
        for (int i=0; i<currentSwords; i++) {
            orbitingSwords.transform.GetChild(i).gameObject.transform.localRotation = Quaternion.Euler(
                0,
                0,
                i * (360f / currentSwords)
            );
        }
    }

    public void OnSwordRingHit() {
        currentSwords = GetActiveSwords();
        if (currentSwords <= 1) {
            RingBreak();
            return;
        }
        LoseSword();
        anim.SetTrigger("RingHit");
        rb2d.velocity = new Vector2(
            rb2d.velocity.x - ForwardScalar(),
            rb2d.velocity.y
        );
    }

    public void LoseSword() {
        orbitingSwords.transform.GetChild(currentSwords-1).gameObject.SetActive(false);
        SoundManager.WorldSound(swordSpinNoise);
        SpinSwords();
    }

    protected override void Update() {
        base.Update();
        currentSwords = GetActiveSwords();
        anim.SetInteger("CurrentSwords", currentSwords);

        if (targetingRotation && Mathf.Abs(orbitingSwords.transform.localRotation.eulerAngles.z - targetSwordAngle) < 1f) {
            swordSpinner.enabled = true;
            targetingRotation = false;
        } else if (targetingRotation) {
            swordSpinner.enabled = false;
            
            orbitingSwords.transform.localRotation = Quaternion.Euler(
                0,
                0,
                Mathf.SmoothDamp(
                    orbitingSwords.transform.localRotation.eulerAngles.z,
                    targetSwordAngle,
                    ref swordRingVelocity,
                    smoothAmount * Time.unscaledDeltaTime
            ));
        }
    }

    override public void DamageFor(int dmg) {
        if (GetActiveSwords() > 0) {
            return;
        }
        base.DamageFor(dmg);
    }

    void RingBreak() {
        Instantiate(shieldBreakPrefab, this.transform.position, shieldBreakPrefab.transform.rotation, this.transform);
        anim.SetTrigger("RingBreak");
        // in case weird frame things happen
        if (GetActiveSwords() > 0) {
            orbitingSwords.transform.GetChild(currentSwords-1).gameObject.SetActive(false);
        }
    }

    public void SpinSwords() {
        targetSwordAngle += (currentSwords % 2 == 0) ? 170 : -170;
        targetSwordAngle = targetSwordAngle % 360;
        targetingRotation = true;
        SpaceSwordRotation();
    }

    public void ResetSwords() {
        if (hp <= 0) return;
        currentSwords = maxSwords;
        targetSwordAngle = initialSwordRingAngle;
        StartCoroutine(ShowSwords());
    }

    IEnumerator ShowSwords() {
        foreach (Transform s in orbitingSwords.transform) {
            s.gameObject.SetActive(true);
            SpaceSwordRotation();
        }
        yield return new WaitForSeconds(0f);
    }

    int GetActiveSwords() {
        int c = 0;
        foreach (Transform t in orbitingSwords.transform) {
            if (t.gameObject.activeSelf) c++;
        }
        return c;
    }
}
