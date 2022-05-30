using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PinkWpf
{
    public static class DependencyObjectExtensions
    {
        public static T GetChildOfType<T>(this DependencyObject owner) where T : DependencyObject
        {
            if (owner == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(owner); i++)
            {
                var child = VisualTreeHelper.GetChild(owner, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static IEnumerable<T> GetChildsOfType<T>(this DependencyObject owner) where T : DependencyObject
        {
            if (owner == null)
                return null;

            var collection = new List<T>();

            GetChildsOfType<T>(owner, collection);

            return collection;
        }

        private static void GetChildsOfType<T>(DependencyObject owner, List<T> collection) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(owner); i++)
            {
                var child = VisualTreeHelper.GetChild(owner, i);

                if (child is T tChild)
                    collection.Add(tChild);
                else
                    GetChildsOfType(child, collection);
            }
        }
    }
}
