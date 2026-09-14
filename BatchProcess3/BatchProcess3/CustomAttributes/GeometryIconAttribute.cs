using System;

namespace BatchProcess3.CustomAttributes;
/// <summary>
/// 用于标识<br/>
/// 参数                核心作用<br/>
/// AttributeTargets    限定自定义特性能标注的代码元素（类 / 方法 / 字段等），避免误用<br/>
/// AllowMultiple       控制特性是否能在同一目标上多次标注（默认不允许）<br/>
/// Inherited           控制特性是否随父类 / 接口的继承 / 实现传递给子类 / 实现类（默认传递，false 不传递）<br/>
/// </summary>
/// <param name="message">提示消息</param>
/// <param name="colorStr">颜色字符串</param>
/// <param name="geometryPath">Geometry路径</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class GeometryIconAttribute(string message, string colorStr, string geometryPath) : Attribute
{
    public string Message { get; set; } = message;
    public string ColorStr { get; set; } = colorStr;
    public string GeometryPath { get; set; } = geometryPath;
}
// 上 等于 下
// public class GeometryIconAttribute : Attribute
// {
//     public string Message { get; set; }
//     public string ColorStr { get; set; }
//     public string GeometryStr { get; set; }
//     
//     public GeometryIconAttribute(string message, string colorStr, string geometryStr)
//     {
//         Message = message;
//         ColorStr = colorStr;
//         GeometryStr = geometryStr;
//     }
// }