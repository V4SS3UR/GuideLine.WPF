using System.Windows.Media;

/* Modification non fusionnée à partir du projet 'GuideLine (net461)'
Avant :
using System.Windows;
Après :
using System.Windows;
using GuideLine;
using GuideLine.WPF;
using GuideLine.WPF.Extensions;
*/
using System.Windows;

namespace GuideLine.WPF.Extensions
{
    public static class DependencyObjectExtensions
    {

        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = child.FindChild<T>(childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        public static T FindChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }

                foundChild = child.FindChild<T>();
                if (foundChild != null) break;
            }

            return foundChild;
        }
        public static FrameworkElement FindChild(this DependencyObject parent, string childName)
        {
            if (parent == null) return null;

            FrameworkElement foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    return frameworkElement;
                }

                foundChild = child.FindChild(childName);
                if (foundChild != null) break;
            }

            return foundChild;
        }



        public static T FindParent<T>(this DependencyObject child, string parentName)
            where T : DependencyObject
        {
            if (child == null) return null;

            T foundParent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                var frameworkElement = currentParent as FrameworkElement;
                if (frameworkElement.Name == parentName && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent != null);

            return foundParent;
        }
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child != null)
            {
                var dependObj = child;
                do
                {
                    dependObj = LogicalTreeHelper.GetParent(dependObj); ;
                    if (dependObj is T)
                        return dependObj as T;
                }
                while (dependObj != null);
            }

            return null;
        }
        public static FrameworkElement FindParent(this DependencyObject child, string parentName)
        {
            if (child == null) return null;

            DependencyObject currentParent = VisualTreeHelper.GetParent(child);

            while (currentParent != null)
            {
                if (currentParent is FrameworkElement frameworkElement && frameworkElement.Name == parentName)
                {
                    return frameworkElement;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);
            }

            return null;
        }



    }
}
