using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Tools.Extensions;

public static class IServiceCollectionExtension
{
    private static void AddViewModelMapping(Type viewType, Type viewModelType)
    {
        if (App.Current == null)
            throw new InvalidOperationException("App.Current 为 null，请确保在应用程序启动后调用此方法");

        // 检查是否已存在映射
        if (!App.ViewModelMappings.TryAdd(viewModelType, viewType))
            throw new InvalidOperationException($"已经注册了 {viewType.Name} 与 {viewModelType.Name} 的映射");
        /* ViewModelMappings 声明如下：
            public new static App? Current => Application.Current as App;
            
            /// <summary>
            /// View与ViewModel映射，[typeof(ViewModel), typeof(View)]<br/>
            /// 因为 ViewLocator.Build(object? data) 参数 data 为 ViewModel，所以这里的映射关系顺序为 [typeof(ViewModel), typeof(View)]
            /// </summary>
            public Dictionary<Type, Type> ViewModelMappings { get; } = new();
         */
    }

    private static void AddViewModelMapping<TView, TViewModel>()
        where TView : class
        where TViewModel : class
    {
        AddViewModelMapping(typeof(TView), typeof(TViewModel));
    }

    /// <summary>
    /// 执行 AddViewModelMapping{TView, TViewModel}() 操作 与 services.AddTransient{TViewModel}() 操作，并返回后者的结果。<br/>
    /// 用于添加 View 与 ViewModel 的映射关系，并注册 TViewModel 为 Singleton，便于在 ViewLocator.cs 中为 View 自动装载 DataContext
    /// </summary>
    public static IServiceCollection AddSingletonViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        // AddViewModelMapping(typeof(TView), typeof(TViewModel));
        // 或者
        AddViewModelMapping<TView, TViewModel>();

        return services.AddSingleton<TViewModel>();
    }

    /// <summary>
    /// 执行 AddViewModelMapping{TView, TViewModel}() 操作 与 services.AddTransient{TViewModel}() 操作，并返回后者的结果。<br/>
    /// 用于添加 View 与 ViewModel 的映射关系，并注册 TViewModel 为 Transient，便于在 ViewLocator.cs 中为 View 自动装载 DataContext
    /// </summary>
    public static IServiceCollection AddTransientViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        // AddViewModelMapping(typeof(TView), typeof(TViewModel));
        // 或者
        AddViewModelMapping<TView, TViewModel>();

        return services.AddTransient<TViewModel>();
    }

    /// <summary>
    /// 执行 AddViewModelMapping{TView, TViewModel}() 操作 与 services.AddTransient{TViewModel}() 操作，并返回后者的结果。<br/>
    /// 用于添加 View 与 ViewModel 的映射关系，并注册 TViewModel 为 Scoped，便于在 ViewLocator.cs 中为 View 自动装载 DataContext
    /// </summary>
    public static IServiceCollection AddScopedViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        // AddViewModelMapping(typeof(TView), typeof(TViewModel));
        // 或者
        AddViewModelMapping<TView, TViewModel>();

        return services.AddScoped<TViewModel>();
    }
}