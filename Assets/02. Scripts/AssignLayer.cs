using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class AssignLayer : MonoBehaviour {

    [SerializeField]
    private string sortingLayerName;
    [SerializeField]
    private int sortingOrder;

    void Awake()
    {
        try
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer.sortingLayerName != sortingLayerName)
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = sortingOrder;
            }
        }
        catch(System.InvalidOperationException e) { }
    }
    public void ChangeLayer(string layerName)
    {
        sortingLayerName = layerName;
        Renderer renderer = GetComponent<Renderer>();
        renderer.sortingLayerName = sortingLayerName;
    }
    public void ChangeOrder(int order)
    {
        sortingOrder = order;
        Renderer renderer = GetComponent<Renderer>();
        renderer.sortingOrder = sortingOrder;
    }
}
