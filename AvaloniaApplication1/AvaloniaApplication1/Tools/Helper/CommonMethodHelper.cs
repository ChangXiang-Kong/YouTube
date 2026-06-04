
using System;

namespace AvaloniaApplication1.Tools.Helper;

public static class CommonMethodHelper
{
    /// <summary>
    /// 检查调用者的类型是否是指定类型
    /// </summary>
    /// <param name="callerType">指定的类型</param>
    /// <returns></returns>
    public static bool CallerTypeIs(Type callerType)
    {
        // 检查调用者的类型
        var callingType = new System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
        return callingType != null && callingType == callerType;
    }
    
    
    
}