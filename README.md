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
[MF Documentation](https://github.com/JackLite/ModulesFramework/blob/master/README.md)

### Debug

In the top-bar menu, choose Modules -> Data Viewer.
You will see a debug window with modules that you have in your project.

The Debug Window has three modes, and you can switch between them by buttons in the right-top corner.

![DebugWindow_img1.png](doc%2FDebugWindow_img1.png)

#### Modules

The modules mode shows you all modules that you have in the project.
It's the only mode that works without entering Unity play mode.
It's useful when you want to take a look at the whole project at once without going through thousands of code lines.
With properly organizing modules mode is a powerful tool to manage huge projects.

Modules mode can be in two submodes: list and graph.
You can switch between them easily.

**Note**.
Modules mode rearranges submodule orders when you enter the Unity play mode.
In editor mode it sorts submodules in alphanumeric order.

#### OneData
![DebugWindow_OneData.png](doc%2FDebugWindow_OneData.png)
This mode allows you to inspect and change any OneData that you created in runtime.

You can pin OneData in the list, and it will always be at the top of the list.

You also can search OneData by the name using search bar above the list.

#### Entities
![DebugWindow_Entities.png](doc%2FDebugWindow_Entities.png)

This mode allows you to inspect all created entities and their components.
You can change any data inside components.

At the top you can see the search field that allows you to search entities with specified components name.
You don't need to enter the whole name of a component.

Above the list of entities you can see the other search field
that allows you to search entities by their label in the list.
This label shows entity's id by default.
If the entity has a custom id, you will see it.
But the main usage of list's search is with the editor tags (see below).

You also can choose to automatically open all components in entity.
Large and complex components will affect editor performance.
And you can choose to always open specified components.
With the ability to pin components, it allows you to search faster entity with specified data inside some components.

### Editor tags
Entity may have one or more tags using in editor.

```csharp
// tag can be any string
entity.AddTag("Enemy");

// in contrast to custom id it can be non-unique
anotherEntity.AddTag("Enemy");

// you can add more tags to specify entity in editor
entity.AddTag("Goblin");
```

### Drawers
Debug view can show primitives, `IDictionary`, `IEnumerable`, structs, classes, unity game objects and `MonoBehaviour`s.

However, you can add support for any other type by inherited 
`ModulesFieldDrawer`. Here's example for drawer of `BigInteger`.

```csharp
// DateTimeDrawer.cs
#if UNITY_EDITOR
using System;
using ModulesFrameworkUnity.Debug.Drawers;
using UnityEngine.UIElements;

namespace Project.Editor.ModulesDrawers
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
                    onChanged(value, newDate.ToUniversalTime());
            });
            
            // don't forget to add element to parent
            parent.Add(_field);
        }

        // use this method to dynamic update of inspector
        // getter() is lambda to get actual value
        protected override void Update(Func<DateTime> getter)
        {
            _field.SetValueWithoutNotify(getter().ToString("O"));
        }
    }
}

#endif
```
For more information about drawers, see implementations of different drawers in this repository. 

### Settings

You can change some settings from Modules -> Unity Adapter Settings.

![pkg](/doc/Settings_img1.png)

#### Main section

_Start method_ has two options:
- Auto - MF will start automatically in any scene;
- Manual - you need to start MF by your own script or use`EntryPoint`;

Manual start allows you to choose scene(s) where you need MF. You can
do it by creating GameObject with `EntryPoint` component or inherit from it.
Of course, you
can start MF any other way you like. For more information, see
[MF Documentation](https://github.com/JackLite/ModulesFramework#getting-started).

_Use old debug_ allows you to turn on debug based on GameObjects. This debug is obsolete and will be removed in the next versions.

_Worlds count_ allows you to set count of worlds that will be created when MF started. For more information see [MF Documentation](https://github.com/JackLite/ModulesFramework#gs-multiple-worlds). Debug window doesn't support yet multiple worlds.

_Log filter_ using for choose what log's messages you need. See details [below](#logger).

_Delete empty entities_ - if set the entity without components will be automatically deleted.

#### Performance monitor

When you define the `MODULES_DEBUG` MF will record elapsed time of `IRunSystem`s and `IPostRunSystem`s. If it more than panic threshold MF will log warning. If you set `Debug mode` MF will also check the warning threshold.

**Note**: it's good to set this values to 10% and 20% of targeted frame time for a warning and panic threshold respectively.

#### Saving settings
By clicking the save button, the settings serialized to JSON and saved to 
Assets/Resources/ModulesSettings.json.

### <a id="logger"/> Logger

MF Unity Adapter has implementation of `IModulesLogger` and register
it automatically. That logger (`UnityLogger`) shows debug messages
_only_ in editor and developer builds. Warnings and errors shows in
any cases. You can choose what messages will be showed in 
Unity Adapter Settings. By default all messages shows.