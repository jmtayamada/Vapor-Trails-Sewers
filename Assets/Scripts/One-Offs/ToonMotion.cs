using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMotion : MonoBehaviour 
{
    private class Snapshot
    {
        public Transform transform;
        public Vector3 localPosition;
        public Quaternion localRotation;

        public Snapshot(Transform transform)
        {
            this.transform = transform;
            this.Update();
        }

        public void Update()
        {
            this.localPosition = this.transform.localPosition;
            this.localRotation = this.transform.localRotation;
        }
    }

    private Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot>();
    private float updateTime = 0f;

    public int fps = 20;

    bool forceUpdateThisFrame = false;

    private void LateUpdate()
    {
        if (forceUpdateThisFrame || Time.unscaledTime - this.updateTime > 1f/this.fps)
        {
            this.SaveSnapshot(transform);
            this.updateTime = Time.unscaledTime;
        }

        foreach(KeyValuePair<int, Snapshot> item in this.snapshots)
        {
            if (item.Value.transform != null)
            {
                item.Value.transform.localPosition = item.Value.localPosition;
                item.Value.transform.localRotation = item.Value.localRotation;
            }
        }

        forceUpdateThisFrame = false;
    }

    private void SaveSnapshot(Transform parent)
    {
        if (parent == null) return;
        int childrenCount = parent.childCount;

        for (int i = 0; i < childrenCount; ++i)
        {
            Transform target = parent.GetChild(i);
            int uid = target.GetInstanceID();

            this.snapshots[uid] = new Snapshot(target);
            this.SaveSnapshot(target);
        }
    }

    public void ForceUpdate() {
        forceUpdateThisFrame = true;
    }
}