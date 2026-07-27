using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;

public class YOLOInference : MonoBehaviour
{
    [Tooltip("Assign all 4 model variants here, in the same order as your selection buttons: 0=FP32 320, 1=FP16 320, 2=FP32 640, 3=FP16 640")]
    public ModelAsset[] yoloModelOptions;
    public WebcamManager webcamManager;
    public float confidenceThreshold = 0.25f;
    public string[] classNames;
    private float Screencentre;

    private Model runtimeModel;
    private Worker worker;

    // fires with (leftGesture, rightGesture) — either can be null
    public System.Action<string, string> OnGesturesDetected;

    void OnEnable()
    {
        int selectedIndex = PlayerPrefs.GetInt(ModelSelector.PrefKey, 0);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, yoloModelOptions.Length - 1);

        
        Screencentre = ModelSelector.GetSelectedInputSize() / 2f;

        runtimeModel = ModelLoader.Load(yoloModelOptions[selectedIndex]);
        Debug.Log(yoloModelOptions[selectedIndex]);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);
    }

    private int frameCount = 0;

    void Update()
    {
        frameCount++;
        if (frameCount % 5 != 0) return;

        Tensor<float> input = webcamManager.GetInputTensor();
        worker.Schedule(input);

        using Tensor<float> output = worker.PeekOutput("output0") as Tensor<float>;
        using var cpu = output.ReadbackAndClone();

        ParseDetections(cpu);
    }

    void ParseDetections(Tensor<float> output)
    {
        int numClasses = classNames.Length;
        int numAnchors = output.shape[2]; // 2100

        // store best detection per side
        float bestLeftConf = confidenceThreshold;
        float bestRightConf = confidenceThreshold;
        int bestLeftClass = -1;
        int bestRightClass = -1;
        float bestLeftX = -1;
        float bestRightX = -1;

        for (int a = 0; a < numAnchors; a++)
        {
            // bounding box center x (normalized 0-1)
            float cx = output[0, 0, a];

            float maxConf = confidenceThreshold;
            int maxClass = -1;

            for (int c = 0; c < numClasses; c++)
            {
                float conf = output[0, 4 + c, a];
                if (conf > maxConf)
                {
                    maxConf = conf;
                    maxClass = c;
                }
            }

            
            // split by x coordinate  left half vs right half
            if (cx > Screencentre)
            {
                if (maxConf > bestLeftConf)
                {
                    bestLeftConf = maxConf;
                    bestLeftClass = maxClass;
                    bestLeftX = cx;
                }
            }
            else
            {
                if (maxConf > bestRightConf)
                {
                    bestRightConf = maxConf;
                    bestRightClass = maxClass;
                    bestRightX = cx;
                }
            }
        }

        string leftGesture = bestLeftClass >= 0 ? classNames[bestLeftClass] : null;
        string rightGesture = bestRightClass >= 0 ? classNames[bestRightClass] : null;



        OnGesturesDetected?.Invoke(leftGesture, rightGesture);
    }

    void OnDisable()
    {
        worker?.Dispose();
    }
}
