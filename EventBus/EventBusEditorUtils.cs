using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Extensions.EventBus
{
    [InitializeOnLoad]
    public static class EventBusEditorUtils
    {
        static EventBusEditorUtils()
        {
            // Subscribe to play mode state changes
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear all lazy-initialized EventBus<T> instances
                EventBusTracker.ClearAll();
            }
        }
    }
}
#endif