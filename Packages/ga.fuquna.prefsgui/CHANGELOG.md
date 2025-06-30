# [ga.fuquna.prefsgui-v3.7.0](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.6.2...ga.fuquna.prefsgui-v3.7.0) (2025-06-30)


### Features

* -prefsgui-file-name, -prefsgui-folder-path arguments to change file path ([b53c33a](https://github.com/fuqunaga/PrefsGUI/commit/b53c33a677a5508174f8ebd6e74e2d336ba1e391))

# [ga.fuquna.prefsgui-v3.6.2](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.6.1...ga.fuquna.prefsgui-v3.6.2) (2025-03-03)


### Bug Fixes

* Add keywords to package.json to get hits in npmjs search ([d5fbdbb](https://github.com/fuqunaga/PrefsGUI/commit/d5fbdbb4ee5311d55333ef5ab7b070795e95048b))

# [ga.fuquna.prefsgui-v3.6.1](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.6.0...ga.fuquna.prefsgui-v3.6.1) (2025-01-06)


### Bug Fixes

* Deleted Debug.Log() from development because it was still there. ([77b3102](https://github.com/fuqunaga/PrefsGUI/commit/77b31026e0fc2942aa999db2b2f038633c6467c7))

# [ga.fuquna.prefsgui-v3.6.0](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.5.0...ga.fuquna.prefsgui-v3.6.0) (2024-09-25)


### Bug Fixes

* Temporary variables in Prefs are reset when the Editor's PlayModeState changes. ([6b14d9c](https://github.com/fuqunaga/PrefsGUI/commit/6b14d9c00bc26de6d932599b2eb367f93787fc89))


### Features

* add PrefsParam.onRegisterPrefsParam event ([d91a48d](https://github.com/fuqunaga/PrefsGUI/commit/d91a48d17033be8f3fb46122695846b820bedffa))


### Performance Improvements

* Improved multicast delegate allocation for sync ([1622127](https://github.com/fuqunaga/PrefsGUI/commit/1622127d9116e23f662d1304aaa2583a0e12c5cf))

# [ga.fuquna.prefsgui-v3.5.0](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.4.3...ga.fuquna.prefsgui-v3.5.0) (2024-09-11)


### Bug Fixes

* Error in copy and paste menu when right-clicking on PrefsList ([3daad89](https://github.com/fuqunaga/PrefsGUI/commit/3daad892673c4807f551cb73dd791f54da9f4fc3))
* PrefsList and PrefsDictionary: When the default button of an element is pressed, the element with defaultValue is referred to. ([c2891fc](https://github.com/fuqunaga/PrefsGUI/commit/c2891fc8b27faa54758aadd55a14b7e9e4a2bbe4))


### Features

* add PrefsIPAddressAndPort. PrefsIPEndPoint is now obsolete. ([2c8b203](https://github.com/fuqunaga/PrefsGUI/commit/2c8b2039b41fc03ed29444e2181a9c70715a197b))

# [ga.fuquna.prefsgui-v3.4.3](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.4.2...ga.fuquna.prefsgui-v3.4.3) (2024-04-24)


### Bug Fixes

* PrefsParam<T>.RegisterValueChangedCallbackAndCallOnce() > PrefsParam.RegisterValueChangedCallbackAndCallOnce() ([dc03593](https://github.com/fuqunaga/PrefsGUI/commit/dc03593f8e0aa766dc09b02e807aef101aa41b0e))

# [ga.fuquna.prefsgui-v3.4.2](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.4.1...ga.fuquna.prefsgui-v3.4.2) (2024-04-17)


### Bug Fixes

* PrefsKvsPathSelector now resets the path when Exit edit or play mode ([84307eb](https://github.com/fuqunaga/PrefsGUI/commit/84307ebdcda007fcb650c89fcddd7b75097082bb))

# [ga.fuquna.prefsgui-v3.4.1](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.4.0...ga.fuquna.prefsgui-v3.4.1) (2024-02-28)


### Bug Fixes

* Changed PrefsParamOuterInner to keep references as much as possible when TOuter is not a ValueType ([a73af95](https://github.com/fuqunaga/PrefsGUI/commit/a73af956df5251f865210ed709c9db9084f575b5))
* Make it easy to understand errors when Json and code have the same key but different types. [#24](https://github.com/fuqunaga/PrefsGUI/issues/24) ([7ce68c6](https://github.com/fuqunaga/PrefsGUI/commit/7ce68c634d44e5ecbd5819acd8d4f340d9fa2eae))

# [ga.fuquna.prefsgui-v3.4.0](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.3.2...ga.fuquna.prefsgui-v3.4.0) (2024-02-26)


### Features

* PrefsGradient on RosettaUI ([f67901e](https://github.com/fuqunaga/PrefsGUI/commit/f67901e3732fc240738ba45dede1946f32f0c6fc))

# [ga.fuquna.prefsgui-v3.3.2](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.3.1...ga.fuquna.prefsgui-v3.3.2) (2023-12-25)


### Bug Fixes

* PrefsIPEndPoints do not appear in inspector ([a748377](https://github.com/fuqunaga/PrefsGUI/commit/a748377115b981810710e4c04f47bbf476255051))

# [ga.fuquna.prefsgui-v3.3.1](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.3.0...ga.fuquna.prefsgui-v3.3.1) (2023-06-27)


### Bug Fixes

* **rosettaui:** update dependency ([16646df](https://github.com/fuqunaga/PrefsGUI/commit/16646dfdf2b73325762283ca9c017009f647d8ef))

# [ga.fuquna.prefsgui-v3.3.0](https://github.com/fuqunaga/PrefsGUI/compare/ga.fuquna.prefsgui-v3.2.5...ga.fuquna.prefsgui-v3.3.0) (2023-06-27)


### Features

* Add PrefsParam.RegisterValueChangeCallbackAndCallOnce() ([722b516](https://github.com/fuqunaga/PrefsGUI/commit/722b516aa3540bfd3a0e70f099948dfdd27ce9ae))

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
