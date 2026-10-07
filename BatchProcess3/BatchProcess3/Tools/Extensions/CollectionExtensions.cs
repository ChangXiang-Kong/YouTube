using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Tools.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    /// Sets the given collection reference and attaches observers to the collection and any contained ViewModelBase
    /// items so the parent ViewModel raises PropertyChanged when the collection or its items change.
    /// </summary>
    /// <remarks>Only items that derive from ViewModelBase are observed for PropertyChanged. Handlers are
    /// attached to CollectionChanged and to each observed item's PropertyChanged, and notifications are raised
    /// immediately after subscription and on subsequent changes.</remarks>
    /// <typeparam name="T">The element type stored in the collection.</typeparam>
    /// <param name="parent">Parent ViewModel that receives PropertyChanged notifications.</param>
    /// <param name="existingCollection">Reference to the current collection; updated to the provided value if it differs.</param>
    /// <param name="value">The new collection to assign and observe.</param>
    /// <param name="propertyChangedNames">Optional additional property names to raise PropertyChanged for when the collection or its items change.</param>
    /// <param name="propertyName">Primary property name to raise when the collection or its items change; supplied by CallerMemberName when
    /// omitted.</param>
    /// <returns>True if the collection reference was replaced and observers were attached; false if the existing and new
    /// collection are considered equal.</returns>
    public static bool SetAndObserveEverything<T>(this ViewModelBase parent, 
        ref ObservableCollection<T> existingCollection,
        ObservableCollection<T> value, 
        string[]? propertyChangedNames = null,
        [CallerMemberName] string propertyName = "")
    {
        // if (EqualityComparer<T>.Default.Equals(existingCollection, newCollection))  // 报错 Cannot access static method 'Equals' in non-static context
        if (EqualityComparer<T>.Equals(existingCollection, value))
            return false;
        
        existingCollection = value;
        
        PropertyChangedEventHandler propertyChangedEventHandler = (sender, e) =>
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
                parent.OnPropertyChanged(propertyName);

            if (propertyChangedNames == null)
                return;
            
            foreach (var propertyChangedName in propertyChangedNames)
                parent.OnPropertyChanged(propertyChangedName);
        };

        var properties = existingCollection.OfType<ViewModelBase>();
        
        void NotifyCollectionChangedEventHandler(object? s, NotifyCollectionChangedEventArgs e)
        {
            // 当集合的 内部元素 调用 PropertyChanged 时，调用 ChangedDelegate() 
            foreach (var property in properties)
            {
                // 防止重复事件
                property.PropertyChanged -= propertyChangedEventHandler;
                property.PropertyChanged += propertyChangedEventHandler;
            }
        
            // TODO: 将 null 改成真正的对象
            propertyChangedEventHandler(null, null!);
        }
        
        // 当集合的 内部元素 调用 PropertyChanged 时，调用 ChangedDelegate() 
        foreach (var property in existingCollection.OfType<ViewModelBase>())
            property.PropertyChanged += propertyChangedEventHandler;
        
        // 当集合变更时，调用 ChangedDelegate() 
        existingCollection.CollectionChanged += NotifyCollectionChangedEventHandler;
        
        // TODO: 将 null 改成真正的对象
        propertyChangedEventHandler(null, null!);

        return true;
    }
}