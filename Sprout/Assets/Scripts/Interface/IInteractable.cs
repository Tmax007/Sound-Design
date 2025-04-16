using System.Diagnostics.Contracts;
using UnityEngine;

public interface IInteractable
{
    public void OnStartInteract();
    public void OnStopInteract();
}
