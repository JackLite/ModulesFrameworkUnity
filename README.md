# ModulesFrameworkUnity

This is documentation for Unity Adapter of Modules Framework (MF).

You can find documentation about Modules Framework
[here](https://github.com/JackLite/ModulesFramework).

For install MF like an unity package use this link:
https://github.com/JackLite/ModulesFrameworkUnityPackage.git

Unity Adapter's goal is to allow start creating game immediately when you download Modules Framework Package. It's also provide default logger, debug tools and basic entry point for manual start MF.

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

![pkg](/doc/Debug_img2.png)

![pkg](/doc/Debug_img3.png)

Debug view can show primitives, `IDictionary`, `IEnumerable`, structs, unity game objects and `MonoBehaviour`s.

You can add support for any other type by inherited 
`ModulesFieldDrawer`. Here's example for drawer of `BigInteger`.

```csharp
// DateTimeDrawer.cs
#if UNITY_EDITOR
using System;
using ModulesFrameworkUnity.Debug.Drawers;
using UnityEngine.UIElements;

namespace Project.Editor
{
    public class DateTimeDrawer : FieldDrawer<DateTime>
    {
        // save field for update
        private TextField _field;

        protected override void Draw(
            // usually field name
            string label, 
            // current value
            DateTime value, 
            // parent element
            VisualElement parent, 
            // callback for updating field from inspector
            Action<DateTime, DateTime> onChanged)
        {
            // based on text field
            _field = new TextField(label)
            {
                value = value.ToString("O")
            };
            
            // register callback for updating field from inspector
            _field.RegisterValueChangedCallback(evt =>
            {
                if (DateTime.TryParse(evt.newValue, out var newDate))
                    onChanged(value, newDate);
            });
            
            // don't forget to add element to parent
            parent.Add(_field);
        }

        // use this method to dynamic update of inspector
        // cause UIToolkit doesn't redraw inspector every frame
        // getter() is lambda to get actual value
        protected override void Update(Func<DateTime> getter)
        {
            _field.value = getter().ToString("O");
        }
    }
}
#endif
```
For more information about drawers see implementations of different drawers in this repository.

### Settings

You can change some settings from Modules -> Unity Adapter Settings.

![pkg](/doc/Settings_img1.png)

#### Main section

Start method has two options:
- Auto - MF will start automatically in any scene;
- Manual - you need to start MF by your own script or use`EntryPoint`;

Manual start allows you choose scene(s) where you need MF. You can
do it by creating GameObject with `EntryPoint` component. Or you
can manage start MF anyway you like. For more information see
[MF Documentation](https://github.com/JackLite/ModulesFramework#getting-started).

Worlds count allows you to set count of worlds that will be created when MF started. For more information see [MF Documentation](https://github.com/JackLite/ModulesFramework#multiple-worlds)

Log filter used for choose what log's messages you need. See details below.

_Auto apply changes_ allow you to change when OneData and Components will be changed using inspector. If it turn off you must press apply button to change data in OneData and Components.

___Note:___In collections changes applies immediately in any change mode to avoid complexity of temporary reference-types and as a consequences - error prone.

#### Performance monitor

When you define the `MODULES_DEBUG` MF will be record elapsed time of `IRunSystem`s and `IPostRunSystem`s. If it more then panic threshold MF will log warning. If you set `Debug mode` MF will also check the warning threshold.

**Note**: it's good to set this values to 10% and 20% of targeted frame time for warning and panic threshold respectively.

#### Saving settings
By clicking save button the settings serialized to JSON and saved to 
Assets/ModulesSettings.json.

### Logger

MF Unity Adapter has implementation of `IModulesLogger` and register
it automatically. That logger (`UnityLogger`) shows debug messages
_only_ in editor and developer builds. Warnings and errors shows in
any cases. You can choose what messages will be showed in 
Unity Adapter Settings. By default all messages shows.