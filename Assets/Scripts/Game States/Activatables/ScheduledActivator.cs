﻿using UnityEngine;

public class ScheduledActivator : Activator {

	public float timeout = 0f;
	public float delay = 0f;

	[Header("Random Delay")]
	public bool randomDelayEnabled;
	public float lowBound;
	public float highBound;

	public bool oneShot = false;

	void OnEnable() {
		Invoke("InvokedActivation", GetDelay());
	}

	void OnDisable() {
		CancelInvoke("InvokedActivation");
	}

	void InvokedActivation() {
		if (!enabled) return;

		Activate();
		if (oneShot) return;
		Invoke("InvokedActivation", timeout);
	}

	float GetDelay() {
		return randomDelayEnabled ? Random.Range(lowBound, highBound) : delay;
	}
}
