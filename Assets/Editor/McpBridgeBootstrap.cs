using System.Threading.Tasks;
using MCPForUnity.Editor.Services;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Temporary editor helper: reconnects the MCP for Unity bridge after a domain reload
/// when the bridge failed to resume. Safe to delete once the session is stable.
/// </summary>
[InitializeOnLoad]
internal static class McpBridgeBootstrap
{
    static McpBridgeBootstrap()
    {
        EditorApplication.delayCall += TryConnect;
    }

    private static void TryConnect()
    {
        if (MCPServiceLocator.Bridge.IsRunning)
        {
            Debug.Log("[MCP Bootstrap] Bridge already running.");
            return;
        }

        _ = ConnectAsync();
    }

    private static async Task ConnectAsync()
    {
        bool started = await MCPServiceLocator.Bridge.StartAsync();
        Debug.Log("[MCP Bootstrap] Bridge started: " + started);
    }
}
