using EPQ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    private bool paused = false;
    private float speedHolder = 0;
    public Generator g;
    public Text speedText;
    // Start is called before the first frame update
    void Start()
    {
        g = Generator.generator;
        speedText.text = "Speed: " + g.MovesPerFrame;
    }

    public void IncreaseSpeed()
    {
        g = Generator.generator;
        if (g.MovesPerFrame <= 1) g.MovesPerFrame *= 2;
        else if (g.MovesPerFrame >= 15) return;
        else g.MovesPerFrame += 1;
        speedText.text = "Speed: " + g.MovesPerFrame; 
    }
    public void DecreaseSpeed()
    {
        g = Generator.generator;
        if (g.MovesPerFrame <= 1) g.MovesPerFrame /= 2;
        else g.MovesPerFrame -= 1;
        speedText.text = "Speed: " + g.MovesPerFrame;
    }
    public void Pause()
    {
        g = Generator.generator;
        if (!paused)
        {
            speedHolder = g.MovesPerFrame;
            g.MovesPerFrame = 0;
            paused = true;
        }
        else
        {
            g.MovesPerFrame = speedHolder;
            paused = false;
        }
    }
}
