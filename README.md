# ModulesFrameworkUnity

This is documentation for Unity Adapter of Modules Framework (MF).

Documentation about Modules Framework you can find 
[here](https://github.com/JackLite/ModulesFramework).

Unity Adapter's goal is to allow start creating game immediately when
you download Modules Framework Package. It's also provide default logger,
debug tools and basic entry point for manual start MF.

### Getting Started

1. In Unity go to Window -> Package Manager.<br>
![pkg](/doc/GettingStarted_img1.png)
2. Click on plus in left-top corner and choose "Add package from git URL"<br>
![pkg](/doc/GettingStarted_img2.png)
3. Paste https://github.com/JackLite/ModulesFrameworkUnityPackage.git 
and click "Add"
4. Wait until Unity download package and recompile scripts.

That's all. When you start game the MF will start automatically.
Now you can create your first module, components and systems.

_Note_: for more information about work with MF see 
[MF Documentation](https://github.com/JackLite/ModulesFramework/blob/main/README.md#getting-started)

### Debug

When you start game in Unity Editor you will see this game objects.

![pkg](/doc/Debug_img1.png)

`EcsWorld` is just provider for Unity game loop. `DebugViewer` is 
container with info about entities and one data. It allows you to see
what entities exists, what components they contains and data in that
components.

__Important__: you can't change data. It will be added later.

![pkg](/doc/Debug_img2.png)

![pkg](/doc/Debug_img3.png)

Debug view can show primitives, `IDictionary`, `IEnumerable`
(first it checks if field is `IDictionary` so it should show keys 
and values), structs, unity game objects and `MonoBehaviour`s.

You can add support for any other type by inherited 
`ModulesFieldDrawer`. Here's example for drawer of `BigInteger`.

```csharp
#if UNITY_EDITOR
using System;
using System.Numerics;
using IdleRoad.Utils;
using ModulesFrameworkUnity.Debug;
using UnityEditor;

namespace IdleRoad.Editor
{
    public class BigIntDrawer : ModulesFieldDrawer
    {
        public override Type PropertyType => typeof(BigInteger);
        public override void Draw(string fieldName, object fieldValue, int level)
        {
            var val = (BigInteger)fieldValue;
            EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
            EditorGUILayout.TextField(fieldName, val.FormatUI());
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
```

`DrawerUtility` has two static methods:
- `GUIStyle OneFieldStyle(int level)` - return style for simple field;
- `GUIStyle OneFieldStyle(int level)` - return style for container
like a `List<T>` or some struct;

_Note_: parameter `level` set the margin from left border. 

### Settings

You can change some settings from Modules -> Unity Adapter Settings.

![pkg](/doc/Settings_img3.png)

Start method has two options:
- Auto - MF will start automatically in any scene;
- Manual - you need to start MF by your own script or use`EntryPoint`;

Manual start allows you choose scene(s) where you need MF. You can
do it by creating GameObject with `EntryPoint` component. Or you
can manage start MF anyway you like. For more information see
[MF Documentation](https://github.com/JackLite/ModulesFramework/blob/main/README.md#getting-started).

Log filter used for choose what log's messages you need. See details below.

By clicking save button the settings serialized to JSON and saved to 
Assets/ModulesSettings.json.

### Logger

MF Unity Adapter has implementation of `IModulesLogger` and register
it automatically. That logger (`UnityLogger`) shows debug messages
_only_ in editor and developer builds. Warnings and errors shows in
any cases. You can choose what messages will be showed in 
Unity Adapter Settings. By default all messages shows.

### Roadmap

- [ ] Allow to change data in DebugViews;
- [ ] Ability to add/remove components marked by attribute;
- [ ] Ability to rise events marked by attribute;
- [ ] Improvements of visual in debug;