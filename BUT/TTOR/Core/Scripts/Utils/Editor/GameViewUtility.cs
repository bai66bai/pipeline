using System;
using System.Reflection;
using UnityEngine;

namespace BUT.TTOR.Core.Utils
{
    public static class GameViewUtility
    {
        public static bool IsSelectedSizeTheSame(Vector2Int resolutionToCheck)
        {
            Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
            MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return ((Vector2)Res).x == resolutionToCheck.x && ((Vector2)Res).y == resolutionToCheck.y;
        }
    }
}
