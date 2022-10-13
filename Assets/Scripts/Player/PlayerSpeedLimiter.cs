using UnityEngine;

public class PlayerSpeedLimiter : SpeedLimiter {

    public float dragAmount = 0.01f;
    const float dragMultiplier = 50f;
    public bool clampSpeed = false;
    PlayerController pc;

    override protected void Start() {
        base.Start();
        pc = GetComponent<PlayerController>();
    }

    override protected void SlowRigidBody() {
		if (rb2d.velocity.sqrMagnitude < 0.01f || (rb2d.constraints == RigidbodyConstraints2D.FreezePositionX)) {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			return;
		}
		float originalSign = Mathf.Sign(rb2d.velocity.x);
		if (IsSpeeding()) {
            float reduced = maxSpeedX;
            if (!clampSpeed) {
			    reduced = Mathf.Max(Mathf.Abs(rb2d.velocity.x)-(dragAmount*Time.deltaTime*dragMultiplier), maxSpeedX);
            }
            rb2d.velocity = new Vector2(
                reduced * originalSign,
                rb2d.velocity.y
		    );
		}
	}
}
