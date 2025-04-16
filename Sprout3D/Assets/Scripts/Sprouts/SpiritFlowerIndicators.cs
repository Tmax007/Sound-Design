using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using UnityEngine;

public class SpiritFlowerIndicators : MonoBehaviour, IActivatable
{
    public MeshRenderer Mesh;

    public Texture unactivatedTexture;
    public Texture activatedTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        Mesh.material.mainTexture = activatedTexture;
    }

    public void Deactivate()
    {
        Mesh.material.mainTexture = unactivatedTexture;
    }
}
