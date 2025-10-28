using System;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaUIRealWorld.Extensions;

public static class IServiceCollectionExtension
{
    private static void AddViewModelMapping(Type viewType, Type viewModelType)
    {
        if (App.Current == null)
            throw new InvalidOperationException("App.Current 为 null，请确保在应用程序启动后调用此方法");

        // 检查是否已存在映射
        if (!App.Current.ViewModelMappings.TryAdd(viewModelType, viewType))
            throw new InvalidOperationException($"已经注册了 {viewType.Name} 与 {viewModelType.Name} 的映射");
        /* ViewModelMappings 声明如下：
            public new static App? Current => Application.Current as App;
            
            /// <summary>
            /// View与ViewModel映射，[typeof(ViewModel), typeof(View)]
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
    /// 用于注册 AddTransient 实例的同时添加 View 与 ViewModel 的映射关系，便于在 ViewLocator.cs 中为 View 添加 DataContext
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
    /// 用于注册 AddScoped 实例的同时添加 View 与 ViewModel 的映射关系，便于在 ViewLocator.cs 中为 View 添加 DataContext
    /// </summary>
    public static IServiceCollection AddScopedViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        // AddViewModelMapping(typeof(TView), typeof(TViewModel));
        // 或者
        AddViewModelMapping<TView, TViewModel>();

        return services.AddTransient<TViewModel>();
    }

    /// <summary>
    /// 用于注册 AddSingleton 实例的同时，添加 View 与 ViewModel 的映射关系，便于在 ViewLocator.cs 中为 View 添加 DataContext
    /// </summary>
    public static IServiceCollection AddSingletonViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        // AddViewModelMapping(typeof(TView), typeof(TViewModel));
        // 或者
        AddViewModelMapping<TView, TViewModel>();

        return services.AddTransient<TViewModel>();
    }
}