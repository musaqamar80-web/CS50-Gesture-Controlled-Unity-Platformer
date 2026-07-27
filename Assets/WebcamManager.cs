using UnityEngine;
using UnityEngine.UI;
using Unity.InferenceEngine;

public class WebcamManager : MonoBehaviour
{
    public int cameraIndex;
    public RawImage displayImage;
    public RenderTexture yolobuffer;
    private WebCamTexture webcamTexture;
    private Tensor<float> inputTensor;
    private RenderTexture runtimeBuffer;
    private int inputSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputSize = ModelSelector.GetSelectedInputSize();

        // Don't resize the inspector-assigned 'yolobuffer' asset directly — that would
        // permanently mutate the project asset. Make a runtime-only copy at the
        // resolution the selected model needs instead.
        runtimeBuffer = new RenderTexture(inputSize, inputSize, yolobuffer.depth, yolobuffer.format);
        runtimeBuffer.Create();

        inputTensor = new Tensor<float>(new TensorShape(1, 3, inputSize, inputSize));

        webcamTexture = new WebCamTexture(WebCamTexture.devices[cameraIndex].name, 640, 480, 30);
        webcamTexture.Play();

        displayImage.texture = webcamTexture;

        displayImage.uvRect = new Rect(1, 0, -1, 1);
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.didUpdateThisFrame)
        {
            Graphics.Blit(webcamTexture, runtimeBuffer);
        }
    }


    public Tensor<float> GetInputTensor()
    {
        TextureConverter.ToTensor(runtimeBuffer, inputTensor, new TextureTransform());
        return inputTensor;
    }


    void OnDisable()
    {
        inputTensor?.Dispose();
        if (runtimeBuffer != null)
        {
            runtimeBuffer.Release();
        }
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}