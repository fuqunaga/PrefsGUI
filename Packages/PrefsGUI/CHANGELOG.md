# PrefsGUI
Accessors and GUIs for persistent preference values using JSON file

## [3.2.1] - 2022.7.12

### Added
 - PrefsKvsPathCustom new magic path %currentDir%

## [3.2.0] - 2022.7.05
### Added
 - PrefsGUIEditorRosettaUI

### Changed
 - Rename PrefsGUIEditor > PrefsGUIEditorRapidGUI

### Fixed
 - Different PrefsParam instances Get() may return different values for the same key.
 - PrefsParam.defaultValue could be changed from outside after SetCurrentToDefault() when TOuter is a class.


## [3.1.0] - 2022.6.03
### Changed
 - PrefsParamOuterInner InnerValue access via IPrefsInnnerAccessor
 - Rename Prefs.KVS > Prefs.Kvs
 - Rename MaterialPropertyDebugMenu > PrefsMaterialProperty
 - PrefsKvsJson alloc reduced

### Added
 - PrefsParam.[Register/Unregister]ValueChangedCallback()


## [3.0.0] - 2022.4.14
### Changed
- RapidGUI separated into a dependent package
- PrefsMixMax now has its own MinMax class instead of RapidGUI's MinMax
- DoGUI() move to PrefsGUI.RapidGUI

## [2.2.1] - 2021.5.07
### Added
- Supports RapidGUI 1.2.2 - PrefsList size field

## [2.2.0] - 2021.5.07
### Added
- PrefsParam,PrefsAny,PrefsList - UnityEngine.Serializable attribute to support generic serialization

## [2.1.0] - 2020.1.20
### Added
- PrefsSearch - Display loaded PrefsParams with partial key match

## [2.0.0] - 2019.12.10
### Changed
- Remove GUIUtil and use RapidGUI package instead
- PrefsSync is now a separated package

## [1.0.0] - long long ago
- First release