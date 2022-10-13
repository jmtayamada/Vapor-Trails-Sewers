using UnityEngine;
using System.Collections.Generic;

public class GameFlagWanter : Activatable {
    public List<GameFlag> gameFlags;
    public Activatable yesActivatable;
    public Activatable noActivatable;

    override public void Activate() {
        foreach (GameFlag f in gameFlags) {
            if (!SaveManager.HasFlag(f)) {
                if (noActivatable != null) {
                    noActivatable.Activate();
                    if (yesActivatable != null) {
                        yesActivatable.ActivateSwitch(false);
                    }
                    return;
                }
            }
            yesActivatable.Activate();
        }
    }

    override public void ActivateSwitch(bool b) {
        Activate();
    }
}
