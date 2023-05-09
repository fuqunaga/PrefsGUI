# [ga.fuquna.prefsgui-v3.2.5](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.2.4...ga.fuquna.prefsgui-v3.2.5) (2023-05-09)


### Bug Fixes

* ValueChangeCallback is now called even if the value is changed from another PrefsParam instance ([3ff78b2](https://github.com/fuqunaga/PrefsGUI/commit/3ff78b2930e1d6c5e4f9d01254074874f68b0171))

# PrefsGUI
Accessors and GUIs for persistent preference values using JSON file

## [3.2.4] - 2022.10.11
### Added
MaterialPropertySerializer now also holds the description

## [3.2.3] - 2022.7.22

### Fixed
fix: PrefsMinMax.MinMax does not serialized

## [3.2.2] - 2022.7.22

### Fixed
 - multi PrefsKvsPathCustom doesn't work

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
