﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour {

	public Transform target;
	public string targetName;

	void Start() {
		if (!string.IsNullOrEmpty(targetName)) {
			target = GameObject.Find(targetName).transform;
		}
	}
	
	public void Flip() {
		Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
	}

	void LateUpdate() {
		var dir = target.transform.position - this.transform.position;
		var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
