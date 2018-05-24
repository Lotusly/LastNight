using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Supportive
{
    public class MyWindow : EditorWindow
    {

        [MenuItem("Window/MyWindow")]
        public static void ShowWindow()
        {
            GetWindow<MyWindow>(false, "MyWindow", true);
        }
    }
}
