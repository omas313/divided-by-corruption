using System;

public class EnvironmentEvents
{
    public static Action<Interactable, string[]> InteractedWithObject;
    public static Action<Interactable> CompletedInteractionWithObject;
    public static Action PlayerTeleporting;
    public static Action PlayerTeleported;
    public static Action FadeOutCompleted;
    public static Action FadeInCompleted;
    

    public static void InvokeInteractedWithObject(Interactable interactable, string[] textLines) => InteractedWithObject?.Invoke(interactable, textLines);
    public static void InvokeCompletedInteractionWithObject(Interactable interactable) => CompletedInteractionWithObject?.Invoke(interactable);
    public static void InvokePlayerTeleporting() => PlayerTeleporting?.Invoke();
    public static void InvokePlayerTeleported() => PlayerTeleported?.Invoke();
    public static void InvokeFadeInCompleted() => FadeInCompleted?.Invoke();
    public static void InvokeFadeOutCompleted() => FadeOutCompleted?.Invoke();
}