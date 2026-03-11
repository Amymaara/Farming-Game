using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

// Global helper class that makes a unique id for game object
public static class GlobalHelper 
{
    public static string GenerateUniqueID(GameObject obj)
    {
        return$"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
