using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UI_Noise_Level : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image UI_Noise;
    public noise_player_manager NPM;
    public float max_height = 400f;
    public Color minColor = Color.green;
    public Color maxColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        Vector2 newSize = UI_Noise.rectTransform.sizeDelta;
        Color currColor = Color.Lerp(minColor, maxColor, NPM.noise_level);
        UI_Noise.color = currColor;
        newSize.y = NPM.noise_level*max_height;
        UI_Noise.rectTransform.sizeDelta = newSize;
    }
}
