# PrefsGUI
Accessors and GUIs for persistent preference values using a JSON file

![](Documentation~/PrefsGUI.gif)

```csharp
// define PrefsParams with key.
public PrefsBool              prefsBool    = new("PrefsBool");
public PrefsInt               prefsInt     = new("PrefsInt");
public PrefsFloat             prefsFloat   = new("PrefsFloat");
public PrefsString            prefsString  = new("PrefsString");
public PrefsParam<EnumSample> prefsEnum    = new("PrefsEnum");
public PrefsColor             prefsColor   = new("PrefsColor");
public PrefsVector2           prefsVector2 = new("PrefsVector2");
public PrefsVector3           prefsVector3 = new("PrefsVector3");
public PrefsVector4           prefsVector4 = new("PrefsVector4");
public PrefsAny<CustomClass>  prefsClass   = new("PrefsClass");
public PrefsList<CustomClass> prefsList    = new("PrefsList");


public Element CreateElement(LabelElement label)
{
    return UI.Column(
        prefsBool.CreateElement(),
        prefsInt.CreateElement(),
        prefsFloat.CreateElement(),
        prefsFloat.CreateSlider(),
        prefsString.CreateElement(),
        prefsEnum.CreateElement(),
        prefsColor.CreateElement(),
        prefsVector2.CreateElement(),
        prefsVector2.CreateSlider(),
        prefsVector3.CreateElement(),
        prefsVector3.CreateSlider(),
        prefsVector4.CreateElement(),
        prefsVector4.CreateSlider(),
        prefsClass.CreateElement(),
        prefsList.CreateElement()
    );
}
```

# Installation

This package uses the [scoped registry] feature to resolve package dependencies. 

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html


**Edit > ProjectSettings... > Package Manager > Scoped Registries**

Enter the following and click the Save button.

```
"name": "fuqunaga",
"url": "https://registry.npmjs.com",
"scopes": [ "ga.fuquna" ]
```
![](Documentation~/2022-04-12-17-29-38.png)


**Window > Package Manager**

Select `MyRegistries` in `Packages:`

![](Documentation~/2022-04-12-17-40-26.png)

Install `PrefsGUI - RosettaUI` and `RosettaUI - UIToolkit`  
If you prefer IMGUI, select the `PrefsGUI - RapidGUI` instead.

![](Documentation~/2022-04-21-17-00-54.png)

If you do not see it, make sure that your Unity version is 2021.3 or higher.


# JSON file path
```
Application.persistentDataPath + "/Prefs.json"
```

You can customize it by placing `PrefsKvsPathCustom` in the scene and setting the `Path` field.  
Special folder names and environment variables are also available.

```
- %dataPath% -> Application.dataPath
- %companyName% -> Application.companyName
- %productName% -> Application.productName
- other %[word]% -> System.Environment.GetEnvironmentVariable([word])
```

![](Documentation~/2022-07-05-15-47-42.png)


# PrefsSearch

Display loaded PrefsParams with a partial key match.

![](Documentation~/PrefsSearch.gif)


# EditorWindow

**Window -> PrefsGUI**

![](Documentation~/editorwindow.gif)

- Display all loaded PrefsParams that can be modified.
- Feed back the current value as a default value.
- Edit the key prefix for each GameObject.


# PrefsMaterialProperty

Auto-create material menu.

![](Documentation~/prefsmaterialproperty.gif)


# Sync PrefsParams over a network

see [PrefsGUISyncForMirror](https://github.com/fuqunaga/PrefsGUISyncForMirror)

# URP PostProcessing

see [PrefsGUI-PostProcessingURP](https://github.com/fuqunaga/PrefsGUI-PostProcessingURP)

# References
- [UV Checker Map Maker](http://uvchecker.byvalle.com/)(CustomUVChecker_byValle_1K.png)
- [RosettaUI](https://github.com/fuqunaga/RosettaUI)
- [RapidGUI](https://github.com/fuqunaga/RapidGUI)
- [PrefsGUISyncForMirror](https://github.com/fuqunaga/PrefsGUISyncForMirror)
