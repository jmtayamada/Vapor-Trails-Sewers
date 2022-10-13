using UnityEngine;
using System.Linq;

public class BeaconWrapper : MonoBehaviour {
    public Beacon beacon;
    public Activatable activateOnLoad;
    
    #if UNITY_EDITOR
    void Start() {
        if (beacon == null) Debug.LogWarning("Beaconwrapper "+ gameObject.GetHierarchicalName() + " has no beacon!");
    }
    #endif
}
