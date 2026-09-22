using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;

namespace BatchProcess3.Data;

public static class ResourceToken
{
    public const string AppName = "BatchProcess3";
    
    // 从 Avalonia 应用程序自动获取（更贴合框架）App.axaml 里写的：<Application Name="BatchProcess3">
    // 若 Application.Current = null，则 反射自动获取当前项目/程序集名称
    public static readonly string AppName1 = Application.Current.Name ?? Assembly.GetExecutingAssembly().GetName().Name ?? "UnknownApp";
    public static string AppName11 { get; } = Application.Current.Name ?? Assembly.GetExecutingAssembly().GetName().Name ?? "UnknownApp";
    /* 推荐选型规则（工程实践）
        1. **`const`**
            > 适用：编译期固定不变的基础字面量，**仅同程序集内部使用**（比如内部魔法数字、内部字符串标记）
            > ❌ 不推荐：库对外公开常量（跨程序集版本陷阱）；需要运行时求值；非基元类型
        2. **`static readonly`（字段）**
            > 适用：**private / internal** 的静态只读值，内部类用，简单高效
            > ❌ 禁止：`public static readonly` 对外暴露（.NET 设计准则禁止公开字段）
        3. **`public static { get; }` 自动只读属性**
            > ✅ 对外公开常量文本、配置默认值，**类库公共 API 首选**
            > 优势：未来可以改成计算属性、懒加载、加日志，保持二进制兼容，适配序列化 / EF 等框架
     */
    
    
}