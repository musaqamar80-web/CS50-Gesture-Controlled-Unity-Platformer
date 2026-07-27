using UnityEngine;

public class ModelSelector : MonoBehaviour
{
    public const string PrefKey = "SelectedModelIndex";

    
    // 0 = FP32 320, 1 = FP16 320, 2 = FP32 640, 3 = FP16 640
    private static readonly int[] ModelInputSizes = { 320, 320, 640, 640 };

    public void SelectModel(int index)
    {
        PlayerPrefs.SetInt(PrefKey, index);
        PlayerPrefs.Save();
    }

    
    public static int GetSelectedInputSize()
    {
        int index = PlayerPrefs.GetInt(PrefKey, 0);
        index = Mathf.Clamp(index, 0, ModelInputSizes.Length - 1);
        return ModelInputSizes[index];
    }
}
