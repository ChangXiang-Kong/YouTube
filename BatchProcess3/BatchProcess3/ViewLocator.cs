using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BatchProcess3.ViewModels;
using BatchProcess3.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3;
using BatchProcess3.ViewModels;

namespace BatchProcess3
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (App.Current == null)
                throw new InvalidOperationException("App.Current 为 null，请确保在应用程序启动后调用此方法");
            if (data is null)
                return null;

            // 获取 ViewModel 对应 View 的完整类型名
            var test = data.GetType();
            if (App.ViewModelMappings.TryGetValue(data.GetType(), out var viewType))
            {
                // 根据 viewType 类型创建 View
                var control = (Control)Activator.CreateInstance(viewType)!;
                control.DataContext = data;
                return control;
            }
            else
            {
                // 尝试查找 ErrorPageView
                var assembly = Assembly.GetExecutingAssembly(); // 获取当前应用程序的程序集
                var projName = assembly.GetName().Name; // 或者 var projName = assembly.FullName;
                var errorViewName = $"{projName}.Views.ErrorPageView";
                var type = Type.GetType(errorViewName);
                if (type is null)
                {
                    // 输出调试信息
                    Debug.WriteLine($"Not found view: {errorViewName}");
                    return null;
                }

                var controlErrorPageView = (Control)Activator.CreateInstance(type)!;
                return controlErrorPageView;
            }
        }

        public bool Match(object? data)
        {
            // return data is ViewModelBase;
            return data is PageViewModel;
        }
    }
}