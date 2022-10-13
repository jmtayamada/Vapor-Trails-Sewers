﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Enemy : Entity {

	[HideInInspector] public Rigidbody2D rb2d;

	public int hp = 5;
	[HideInInspector]
	public int totalHP;
	public int moveForce;
	public int maxSpeed;
	public float diStrength = 0.5f;
	public float diScaleMagnitude = 0.1f;
	private int selfJuggleChain;

	public GameObject playerObject;

	[HideInInspector] public Animator anim;
	[HideInInspector] public bool hasAnimator;

	[HideInInspector] public EnemyBehavior[] behaviors;

	Material whiteMaterial;
	Material defaultMaterial;

	bool dead = false;
	bool hasSprites = false;
	Renderer mainChildRenderer;
	bool fakeDamage = false;

	[HideInInspector] public SpriteRenderer spr;
	List<SpriteRenderer> spriteRenderers;

	public AudioClip hitSound;

	public bool burstOnDeath = false;
	public Transform burstEffect;

	public UnityEvent deathEvent;

	GameObject bossResources;

	public bool IsStunned() {
		return stunned;
	}

	override protected void OnEnable() {
		base.OnEnable();
		fakeDamage = hp < 0;
		totalHP = hp;
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GlobalController.pc.gameObject;
		if ((anim = this.GetComponent<Animator>()) != null) {
			this.hasAnimator = true;
			anim.logWarnings = false;
		}
		behaviors = this.GetComponents<EnemyBehavior>();

		spr = this.GetComponent<SpriteRenderer>();
		
		whiteMaterial = Resources.Load<Material>("Shaders/WhiteFlash");
		spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(includeInactive:true))
			.Where(x => x.GetComponent<IgnoreWhiteFlash>() == null).ToList();
		if (spr != null) {
			defaultMaterial = spr.material;
		} else if (spriteRenderers.Count > 0) {
			defaultMaterial = spriteRenderers[0].material;
		}
		mainChildRenderer = GetComponentInChildren<Renderer>();

		hasSprites = (spr != null || spriteRenderers.Count > 0 || mainChildRenderer != null);

		// if (dropBossResources) {
		//	bossResources = Resources.Load<GameObject>("Effects/BossResources");
		// }
	}

	override public void KnockBack(Vector2 kv) {
		selfJuggleChain++;
		kv += Random.insideUnitCircle * diStrength * (selfJuggleChain*diScaleMagnitude);
		base.KnockBack(kv);
	}

	virtual public void DamageFor(int dmg) {
		CombatMusic.EnterCombat();
		if (fakeDamage) return;
		OnDamage();
		this.hp -= dmg;
		if (this.hp <= 0 && !dead) {
			Die();
		} else {
			WhiteSprite();
		}
	}

	public override void OnHit(Attack attack) {
		if (attack.GetComponent<PlayerAttack>() != null) {
			PlayerAttack a = attack.GetComponent<PlayerAttack>();
			if (a.hitstopLength > 0 && this.hp > attack.GetDamage()) {
				Hitstop.Run(a.hitstopLength);
			}
		}
		DamageFor(attack.GetDamage());
		if (this.hitSound != null && mainChildRenderer.isVisible) {
			AlerterText.Alert("Playing hit sound: "+hitSound.name);
			SoundManager.PlayIfClose(this.hitSound, this.gameObject);
		}
		StunFor(attack.GetStunLength());
		if (attack.knockBack) {
			KnockBack(attack.GetKnockback());
		}
	}

	protected virtual void Die(){
		CloseHurtboxes();
		this.frozen = true;
		this.dead = true;
		Hitstop.Run(.1f);
		if (!burstOnDeath && anim != null) {
			anim.SetTrigger("Die");
		} else {
			if (burstEffect != null) {
				Burst();
			}
			Destroy(this.gameObject);
			return;
		}
		deathEvent.Invoke();
	}

	// for each added behavior, call it
	virtual protected void Update() {
		if (!stunned) {
			foreach (EnemyBehavior eb in this.behaviors) {
				eb.Move();
			}
		}
	}

	override protected void UnStun() {
		selfJuggleChain = 0;
		base.UnStun();
	}

	//on death, remove damage dealing even though it'll live a little bit while the dying animation finishes
	public void CloseHurtboxes() {
		foreach (Transform child in transform) {
			if (child.gameObject.tag.Equals(Tags.EnemyHurtbox)) {
				if (child.GetComponent<Collider2D>() != null) child.GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	public void WhiteSprite() {
		if (!hasSprites) {
			return;
		}
		foreach (SpriteRenderer x in spriteRenderers) {
			x.material = whiteMaterial;
		}
		if (spr != null) {
        	spr.material = whiteMaterial;
		}
		StartCoroutine(normalSprite());
    }

	IEnumerator normalSprite() {
		yield return new WaitForSecondsRealtime(.1f);
		spriteRenderers.ForEach(x => {
			x.material = defaultMaterial;
		});
		if (spr != null) {
        	spr.material = defaultMaterial;
		}
		if (anim != null) {
			anim.SetBool("WhiteSprite", false);
		}
	}

	public virtual void OnDamage() {
		if (anim != null) anim.SetTrigger("Hurt");
	}

	public void Burst() {
		Instantiate(burstEffect, this.transform.position, Quaternion.identity);
		Destroy();
	}

	public override void OnGroundHit(float impactSpeed) {
		base.OnGroundHit(impactSpeed);
		anim.SetBool("Grounded", true);
		foreach (EnemyBehavior eb in this.behaviors) {
			eb.OnGroundHit();
		}
	}

	public override void OnGroundLeave() {
		anim.SetBool("Grounded", false);
		foreach (EnemyBehavior eb in this.behaviors) {
			eb.OnGroundLeave();
		}
	}
}
