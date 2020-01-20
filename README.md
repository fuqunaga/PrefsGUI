# PrefsGUI

Accessors and GUIs for persistent preference values using JSON file

![](Documentation~/PrefsGUI.gif)

```csharp
// define PrefsParams with key.
public PrefsBool prefsBool = new PrefsBool("PrefsBool");
public PrefsInt prefsInt = new PrefsInt("PrefsInt");
public PrefsFloat prefsFloat = new PrefsFloat("PrefsFloat");
public PrefsString prefsString = new PrefsString("PrefsString");
public PrefsEnum prefsEnum = new PrefsEnum("PrefsEnum");
public PrefsColor prefsColor = new PrefsColor("PrefsColor");
public PrefsVector2 prefsVector2 = new PrefsVector2("PrefsVector2");
public PrefsVector3 prefsVector3 = new PrefsVector3("PrefsVector3");
public PrefsVector4 prefsVector4 = new PrefsVector4("PrefsVector4");
public PrefsClass prefsClass = new PrefsClass("PrefsClass");
public PrefsList prefsList = new PrefsList("PrefsList");

public void DoGUI()
{
    prefsBool.DoGUI();

    // Return true if value was changed
    var changed = prefsInt.DoGUI();
    if (changed)
    {
        // Implicitly convert
        int intValue = prefsInt;
        Debug.Log("Changed. " + intValue);
    }

    prefsFloat.DoGUI();
    prefsFloat.DoGUISlider();
    prefsString.DoGUI();
    prefsEnum.DoGUI();
    prefsColor.DoGUI();
    prefsVector2.DoGUI();
    prefsVector2.DoGUISlider();
    prefsVector3.DoGUI();
    prefsVector3.DoGUISlider();
    prefsVector4.DoGUI();
    prefsVector4.DoGUISlider();
    prefsClass.DoGUI();
    prefsList.DoGUI();
}
```

# Install

## Install dependencies

- [RapidGUI](https://github.com/fuqunaga/RapidGUI)

## Intall PrefsGUI
Download a `.unitypackage` file from [Release page](https://github.com/fuqunaga/PrefsGUI/releases).

or

**Using Pacakge Manager**  
Add following line to the `dependencies` section in the `Packages/manifest.json`.
```
"ga.fuquna.prefsgui": "https://github.com/fuqunaga/PrefsGUI.git"
```

# PrefsSearch

![](Documentation~/PrefsSearch.gif)

Display loaded PrefsParams with partial key match


# EditorWindow

![](Documentation~/PrefsGUIEditor.gif)
- **Window -> PrefsGUI**
- Display all loaded prefs that can be modiefied
- You can also feed back the current value as default value
- You can edit the key prefix for each GameObject by displaying it in order of GameObject

## JSON file path
```
Application.persistentDataPath + "/Prefs.json"
```

You can customize by placing PrefsWrapperPathCustom in the scene and set `_path` field.  
also can use the Special folders and enviroment variables.
```
- %dataPath% -> Application.dataPath
- %companyName% -> Application.companyName
- %productName% -> Application.productName
- other %[word]% -> System.Environment.GetEnvironmentVariable([word])
```


# MaterialPropertyDebugMenu

Auto create material GUI menu

![](Documentation~/MaterialPropertyDebugMenu.gif)


# Sync Prefs over network

**PrefsGUISyncUNET**  
https://github.com/fuqunaga/PrefsGUISyncUNET


# References

- **RapidGUI**  
https://github.com/fuqunaga/RapidGUI
<br>

- **PrefsGUISyncUNET**  
https://github.com/fuqunaga/PrefsGUISyncUNET
