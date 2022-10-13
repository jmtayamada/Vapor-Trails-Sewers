﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {
	
	int energyGained = 1;

	public bool gainsEnergy = false;
	public bool costsEnergy = false;
	public int energyCost = 1;
	public float hitstopLength = 0.2f;
	public bool rotateHitmarker = true;
	public bool pullInEntity = false;
	HashSet<Entity> entitiesHitThisActive = new HashSet<Entity>();

	public bool attackLandEvent = true;

	PlayerController player;
	BoxCollider2D bc2d;
	bool colliderOnLastFrame;

	public GameObject[] baseDamageHitmarkers;


	void Start() {
		player = GameObject.Find("Player").GetComponent<PlayerController>();
		attackerParent = player;
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
		rb2d = attackerParent.GetComponent<Rigidbody2D>();
		bc2d = GetComponent<BoxCollider2D>();
	}

	void OnAttackDisable() {
		entitiesHitThisActive.Clear();
	}

	void Update() {
		if (!bc2d.enabled && colliderOnLastFrame) {
			OnAttackDisable();
		}
		colliderOnLastFrame = bc2d.enabled;
	}

	public override void ExtendedAttackLand(Entity e) {
		if (e == null) {
			return;
		}

		entitiesHitThisActive.Add(e);

		// the succ
		Rigidbody2D r = e.GetComponent<Rigidbody2D>();
		if (pullInEntity && e.staggerable && r != null) {
			r.MovePosition(this.transform.position);
		}

		//run self knockback
		if (selfKnockBack) {
			SelfKnockBack();
		}
		//give the player some energy
		if (gainsEnergy) {
			player.GainEnergy(this.energyGained);
		}
		//deplete energy if necessary
		if (costsEnergy) {
			player.LoseEnergy(this.energyCost);
		}

		if (attackLandEvent) {
			player.OnAttackLand(this);
		}
		
		//run hitstop if it's a player attack
		if (hitstopLength > 0.0f && this.gameObject.CompareTag(Tags.PlayerHitbox)) {
			CameraShaker.TinyShake();
			Hitstop.Run(this.hitstopLength);
		}
	}

	override protected void OnTriggerEnter2D(Collider2D otherCol) {
		if (attackedTags.Contains(otherCol.tag)) {
			//if it takes energy to inflict damage, don't run any of the hit code
			if (costsEnergy && energyCost > attackerParent.GetComponent<PlayerController>().CheckEnergy()) {
				return;
			}

			Hurtbox hurtbox = otherCol.GetComponent<Hurtbox>();

			if (entitiesHitThisActive.Contains(hurtbox.GetParent().GetComponent<Entity>())) {
				return;
			}

			if (hurtbox != null && hurtbox.OnHit(this)) {
				OnAttackLand(hurtbox.GetParent(), hurtbox);
			}
		}
	}

	public void SelfKnockBack() {
		attackerParent.GetComponent<Rigidbody2D>().velocity = new Vector2(
			forceX ? selfKnockBackVector.x * attackerParent.ForwardScalar() : attackerParent.GetComponent<Rigidbody2D>().velocity.x,
			selfKnockBackVector.y
		);
	}

	override public void MakeHitmarker(Transform hurtboxPos) {
		// Vector2 midpoint = Vector2.MoveTowards(this.transform.position, hurtboxPos.position, Vector2.Distance(this.transform.position, hurtboxPos.position)/2f);
		
		GameObject h = Instantiate(
			hitmarker,
			this.transform
		) as GameObject;
		TransformHitmarker(h);

		// other level hitmarkers
		// base dmg starts at 1, if so we don't need to spawn any extra hitmarkers
		for (int i=0; (i<baseDamageHitmarkers.Length) && (i<player.baseDamage-1); i++) {
			GameObject g = Instantiate(
				baseDamageHitmarkers[i],
				this.transform.position,
				Quaternion.identity
			);
			TransformHitmarker(g);
		}
				
	}

	void TransformHitmarker(GameObject h) {
		if (rotateHitmarker) {
			h.transform.eulerAngles = new Vector3(
				0,
				0,
				Vector2.Angle(Vector3.right, knockbackVector)
			);
		}
		h.transform.parent = null;
	}

	override public int GetDamage() {
		return this.damage * attackerParent.GetComponent<PlayerController>().baseDamage;
	}
}
