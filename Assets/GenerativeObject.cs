using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerativeObject : MonoBehaviour
{
    public static int clickOrderCounter = 0;
    public int clickOrder;
    private Renderer renderer;
    public Color objectColor;

    // Remove the circle after clicking it and set click order
    void OnMouseDown()
    {
        clickOrder = clickOrderCounter++;
        GeneticAlgorithm.Instance.RegisterClick(this);
        Destroy(gameObject);
        Debug.Log("Object clicked with order: " + clickOrder);
    }

    // Set color of the circle
    public void SetColor(Color color)
    {
        objectColor = color;
        if (renderer == null)
        {
            renderer = GetComponent<Renderer>();
        }

        renderer.material.color = objectColor;
    }
}
