using System;
using System.Collections.Generic;

public class EnvironmentEvents
{
    public static Action<Interactable, string[]> InteractedWithObject;
    public static Action<Interactable> CompletedInteractionWithObject;
    public static Action PlayerTeleporting;
    public static Action PlayerTeleported;
    public static Action FadeOutCompleted;
    public static Action FadeInCompleted;
    public static Action<PickupType> PlayerPickedUpObject;
    public static Action<Dictionary<PickupType, int>> PlayerInventoryUpdated;
    public static Action PlayerOpenedMainDoor;
    public static Action<BattleDataDefinition> BattleInitiated;
    

    public static void InvokeInteractedWithObject(Interactable interactable, string[] textLines) => InteractedWithObject?.Invoke(interactable, textLines);
    public static void InvokeCompletedInteractionWithObject(Interactable interactable) => CompletedInteractionWithObject?.Invoke(interactable);
    public static void InvokePlayerTeleporting() => PlayerTeleporting?.Invoke();
    public static void InvokePlayerTeleported() => PlayerTeleported?.Invoke();
    public static void InvokeFadeInCompleted() => FadeInCompleted?.Invoke();
    public static void InvokeFadeOutCompleted() => FadeOutCompleted?.Invoke();
    public static void InvokePlayerPickedUpObject(PickupType pickupType) => PlayerPickedUpObject?.Invoke(pickupType);
    public static void InvokePlayerInventoryUpdated(Dictionary<PickupType, int> inventory) => PlayerInventoryUpdated?.Invoke(inventory);
    public static void InvokePlayerOpenedMainDoor() => PlayerOpenedMainDoor?.Invoke();
    public static void InvokeBattleInitiated(BattleDataDefinition battleDefinition) => BattleInitiated?.Invoke(battleDefinition);
}