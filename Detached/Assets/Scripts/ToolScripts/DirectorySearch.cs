using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class DirectorySearch : MonoBehaviour
{
    public Dropdown dropdown;
    public DataVisualizer visualizer;
    string sourceDirectory = Application.dataPath + "/DebugTool";

    private void Start()
    {
        OpenDirectory();
    }
    public void OpenDirectory()
    {

        var txtfiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");

        foreach (string currentFile in txtfiles)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = currentFile.Substring(sourceDirectory.Length + 1) });
        }



        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;

        visualizer.DrawStart(sourceDirectory + "/" + dropdown.options[index].text);
    }
}