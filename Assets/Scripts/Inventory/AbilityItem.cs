using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "AbilityItem", order = 2)]
public class AbilityItem : Item {
    public Ability ability;
    [TextArea] public string instructions;

    override public void OnPickup(bool quiet = false) {
        if (!quiet) GlobalController.abilityUIAnimator.GetComponent<AbilityGetUI>().GetItem(this);
        GlobalController.UnlockAbility(this.ability);
    }

    override public string GetDescription() {
        return base.GetDescription() 
            + "\n\n<color=white>"
            + ControllerTextChanger.ReplaceText(instructions)
            + "</color>";
    }

    override public bool IsType(ItemType t) {
        if (t==ItemType.ABILITY) return true;
        else return base.IsType(t);
    }
}