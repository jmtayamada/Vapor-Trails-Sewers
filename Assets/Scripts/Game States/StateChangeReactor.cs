using UnityEngine;

public abstract class StateChangeReactor : MonoBehaviour, IStateChangeListener {
	public void Awake() {
		StateChangeRegistry.Add(this);
	}

	public void OnDestroy() {
		StateChangeRegistry.Remove(this);
	}

	void Start() {
		React(true);
	}
	
	public abstract void React(bool fakeSceneLoad);
}
