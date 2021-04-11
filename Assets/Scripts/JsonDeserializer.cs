using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JsonDeserializer : MonoBehaviour
{
    public PositionCollection positionCollection;
    TMP_Dropdown m_Dropdown;
    Testing t;
    // Start is called before the first frame update
    void Start()
    {
        t = FindObjectOfType<Testing>();
        m_Dropdown = FindObjectOfType<TMP_Dropdown>();
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
        Parse("BlockPattern");
    }
    void DropdownValueChanged(TMP_Dropdown change)
    {
        if (change.value == 0)
        {
            Parse("BlockPattern");
        }
        else if (change.value == 1)
        {
            Parse("BoatPattern");
        }
        else if (change.value == 2)
        {
            Parse("BlinkerPattern");
        }
        else if (change.value == 3)
        {
            Parse("ToadPattern");
        }
    }
    void Parse(string fileName)
    {
        
        string filePath = Application.streamingAssetsPath + "/"+fileName+".json";
        string json;
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(filePath);
            while (!reader.isDone) { }

            json = reader.text;
        }
        else
        {
            json = File.ReadAllText(filePath);
        }
        positionCollection = JsonUtility.FromJson<PositionCollection>(json);
        Invoke("wait", .1f); //wait for .1f before calling this method to avoid null reference exception
    }

    void wait()
    {
        t.PreDefinedPatternSelector();
    }
}
