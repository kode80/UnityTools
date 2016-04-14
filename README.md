# kode80 UnityTools
Various editor/GUI tools for Unity3D

* GUIWrapper: classes for creating editor GUIs, with support for delegate actions & nesting
* ComponentList: editor window for easy reordering of GameObject components
* ComponentContextMenu: move components to top/bottom, display component list for reordering
* RecordVideo: easily record video footage during play mode:
  * Record multiple separate videos in one session using customizable hotkey to toggle recording
  * Options for super size rendering & framerate
  * Automatic MP4/GIF export (requires [FFmpeg](http://ffmpeg.org/))
* AssetUpdate: automatic asset update system for non-asset store assets

Download latest package: [kode80UnityTools.unitypackage](https://raw.github.com/kode80/UnityTools/master/kode80UnityTools.unitypackage)

### AssetUpdate
I distribute all my Unity3D assets outside of the official Asset Store, which meant I needed a way to make it as painless as possible for users to keep up-to-date with new versions. The Asset Update system is very simple, just create a file called AssetVersion.xml and add it to your asset. When the user opens the "Check for Asset Updates" window, it will find all AssetVersion.xml files in the current project, download their remote AssetVersion.xml files, compare the version numbers and if the remote version is later, give the user the option of viewing release notes and downloading the package.

Since all my assets are on GitHub and GitHub allows raw file linking, I get automatic updates almost for free simply by updating the AssetVersion.xml file and committing. Which is nice. Below is an example AssetVersion.xml file:
```
<asset>
    <name>kode80 Clouds</name>
    <author>kode80</author>
    <version>1.0.0</version>
    <notes>First release of kode80 Clouds.</notes>
    <package-uri>http://kode80.com/downloads/assets/kode80Clouds.unitypackage</package-uri>
    <version-uri>https://raw.github.com/kode80/kode80CloudsUnity3D/develop/Assets/kode80/Clouds/AssetVersion.xml</version-uri>
</asset>
```

### known issues
* RecordVideo outputs black frames when HW antialiasing is enabled. This is a Unity bug that only seems to affect certain systems. If you find your captured video is black, try disabling anti aliasing under Edit->Project Settings->Quality.
