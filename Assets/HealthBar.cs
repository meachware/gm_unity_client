using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{

    Rect box = new Rect(10, 10, 100, 20);

    private Texture2D background;
    private Texture2D foreground;

    public float health = 50;
    public int maxHealth = 100;

    void Start()
    {

        background = new Texture2D(1, 1, TextureFormat.RGB24, false);
        foreground = new Texture2D(1, 1, TextureFormat.RGB24, false);

        background.SetPixel(0, 0, Color.red);
        foreground.SetPixel(0, 0, Color.green);

        background.Apply();
        foreground.Apply();
    }

    void Update()
    {
        health += Input.GetAxisRaw("Horizontal");
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;
    }

    void OnGUI()
    {
        GUI.BeginGroup(box);
        {
            GUI.DrawTexture(new Rect(0, 0, box.width, box.height), background, ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(0, 0, box.width*health/maxHealth, box.height), foreground, ScaleMode.StretchToFill);
        }
        GUI.EndGroup(); ;
    }

}