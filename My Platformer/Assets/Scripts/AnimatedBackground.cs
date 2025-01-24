using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BackGroundType { Blue, Brown, Gray, Green, Pink, Purple, Yellow }

public class AnimatedBackground : MonoBehaviour
{
    [SerializeField] private Vector2 movementDirection;
    private MeshRenderer mesh;

    [Header("Color")]
    [SerializeField] private BackGroundType backGroundType;

    [SerializeField] private Texture2D[] textures;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();    
        UpdateBackGroundTexture();
    }

    private void Update()
    {
        mesh.material.mainTextureOffset += movementDirection * Time.deltaTime;
    }

    [ContextMenu("Update background")]
    private void UpdateBackGroundTexture()
    {
        if(mesh == null)
        {
            mesh = GetComponent<MeshRenderer>();
        }
        mesh.sharedMaterial.mainTexture = textures[((int)backGroundType)];    
    }
}
