﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public static class CustomDrawers
    {
        private static Dictionary<Type, ModulesFieldDrawer> _drawers = new();

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterCustomDrawers()
        {
            _drawers = new();
            var commonDrawer = new EditorDrawer();
            var drawers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(ModulesFieldDrawer)) && !t.IsAbstract)
                    .Select(t => (ModulesFieldDrawer)Activator.CreateInstance(t)));
            foreach (var propertyDrawer in drawers)
            {
                propertyDrawer.Init(commonDrawer);
                RegisterDrawer(propertyDrawer);
            }
        }
        
        public static void RegisterDrawer(ModulesFieldDrawer drawer)
        {
            if (_drawers.ContainsKey(drawer.PropertyType))
            {
                UnityEngine.Debug.LogError($"Drawer for {drawer.PropertyType} already exists");
                return;
            }

            _drawers[drawer.PropertyType] = drawer;
        }

        public static bool TryDraw<T>(string fieldName, T fieldValue, ref int level, out object newValue)
        {
            newValue = fieldValue;
            if (!_drawers.ContainsKey(fieldValue.GetType()))
                return false;
            var drawer = _drawers[fieldValue.GetType()];
            newValue = drawer.Draw(fieldName, fieldValue, level);
            return true;
        }
    }
}