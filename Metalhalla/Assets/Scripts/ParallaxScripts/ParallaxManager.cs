using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("Layer ratios (& speed)")]
    [Tooltip("Layer ratio between main boundaries and layer. It also determines the layer speed.")]
    public float foreground1Ratio = 1.1f;
    public float background1Ratio = 0.9f;
    public float background2Ratio = 0.8f;
    public float background3Ratio = 0.6f;
    public float background4Ratio = 0.4f;

    [Header("Layer GameObjects")]
    [Tooltip("Their pivot must be in x=0 for the effect to work properly")]
    public GameObject foreground1;
    public GameObject background1;
    public GameObject background2;
    public GameObject background3;
    public GameObject background4;

    Bounds boundaries;

    Transform cameraTransform;
    float[] ratios;
    Transform[] layers;
    Vector2 limits;
    float limitRange;

    float camPosNormalized = 0.0f;

    void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;;

        GameObject[] tmp = { foreground1, background1, background2, background3, background4 };
        bool[] mask = { false, false, false, false, false };
        int validLayers = 0;
        for (int i = 0; i < 5; ++i)
        {
            if (tmp[i] != null)
            {
                mask[i] = true;
                validLayers++;
            }
        }

        float[] tmpratios = new float[] { foreground1Ratio, background1Ratio, background2Ratio, background3Ratio, background4Ratio };
        ratios = new float[validLayers];
        layers = new Transform[validLayers];
        int j = 0;
        for (int i = 0; i < 5; ++i)
        {
            if (mask[i] == true)
            {
                ratios[j] = tmpratios[i];
                layers[j] = tmp[i].transform;
                ++j;
            }

        }

        //get the limits of the parallax to know the position of the 
        boundaries = GetComponent<BoxCollider>().bounds;
        limits.x = boundaries.center.x - boundaries.extents.x;
        limits.y = boundaries.center.x + boundaries.extents.x;
        limitRange = limits.y - limits.x;
    }

    void Update()
    {
        UpdateCamPosNormalized();
        UpdateLayerPositions();
    }

    private void UpdateCamPosNormalized()
    {
        camPosNormalized = (cameraTransform.position.x - limits.x) / limitRange;
        if (camPosNormalized <= 0.0f)
            camPosNormalized = 0.0f;
        if (camPosNormalized >= 1.0f)
            camPosNormalized = 1.0f;
    }

    private void UpdateLayerPositions()
    {
        Vector3 tmp;
        for (int i = 0; i < layers.Length; ++i)
        {
            tmp = layers[i].position;
            tmp.x = limits.x * (1 - camPosNormalized) + (limits.x + limitRange * (1 - ratios[i])) * camPosNormalized;
            layers[i].position = tmp;
        }
    }
}
