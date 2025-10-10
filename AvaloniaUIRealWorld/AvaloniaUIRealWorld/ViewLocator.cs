using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaUIRealWorld.ViewModels;
using AvaloniaUIRealWorld.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaUIRealWorld
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data == null)
                return null;

            var viewName = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.InvariantCulture);
            var viewType = Type.GetType(viewName);

            if (viewType == null)
            {
                // 获取当前应用程序的程序集
                //var assembly = Assembly.GetExecutingAssembly();
                ////var projName = assembly.FullName;
                //var projName = assembly.GetName().Name;
                // 或者
                var projName = viewName.Split('.')[0];
                var errorViewName = $"{projName}.Views.ErrorPageView";
                var type = Type.GetType(errorViewName);
                if (type != null)
                {
                    var errorControl = (Control)Activator.CreateInstance(type)!;
                    errorControl!.DataContext = data;
                    return errorControl;
                }

                return null;
            }

            var control = (Control)Activator.CreateInstance(viewType)!;
            control.DataContext = data;
            return control;
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
