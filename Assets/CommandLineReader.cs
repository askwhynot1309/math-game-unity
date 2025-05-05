using System;
using UnityEngine;

public class CommandLineReader : MonoBehaviour
{
    public static string UserId { get; private set; }
    public static string FloorId { get; private set; }
    public static string GameId { get; private set; }
    public static string AccessToken { get; private set; }

    void Awake()
    {
        string[] args = Environment.GetCommandLineArgs();

        foreach (var arg in args)
        {
            if (arg.StartsWith("--userId="))
                UserId = arg.Substring("--userId=".Length);

            if (arg.StartsWith("--floorId="))
                FloorId = arg.Substring("--floorId=".Length);

            if (arg.StartsWith("--gameId="))
                GameId = arg.Substring("--gameId=".Length);

            if (arg.StartsWith("--accessToken="))
                AccessToken = arg.Substring("--accessToken=".Length);
        }
        #if UNITY_EDITOR
                if (string.IsNullOrEmpty(UserId)) UserId = "84aee218-f13c-4596-ad45-4d843a96e101";
                if (string.IsNullOrEmpty(FloorId)) FloorId = "779dd5d3-ddf3-49c6-8e9f-9c90003f9148";
                if (string.IsNullOrEmpty(GameId)) GameId = "1fbc7296bb6c407aa5b7856a48ad8a14    ";
        if (string.IsNullOrEmpty(AccessToken)) AccessToken = "";
        #endif

            //Debug.Log($"[Startup Args] userId: {UserId}, floorId: {FloorId}, gameId: {GameId}, accessToken: {AccessToken}");
    }
}
