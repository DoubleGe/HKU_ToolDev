using SFB;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
    }

}
