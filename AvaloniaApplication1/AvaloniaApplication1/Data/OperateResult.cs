using System;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Data
{
    /* 简化版 OperateResult：
        public class OperateResult
        {
            public OperateResult(bool isSuccess, string message, Exception? exception = null)
            {
                IsSuccess = isSuccess;
                Message = message;
                Exception = exception;
            }

            public bool IsSuccess { get; protected set; }
            public string Message { get; protected set; }
            public Exception? Exception { get; protected set; }

            public static OperateResult Success()
                => new OperateResult(true, "成功");
            public static OperateResult Success(string message)
                => new OperateResult(true, message);
            public static OperateResult<T> Success<T>(T? content)
                => new OperateResult<T>(true, "成功", content);
            public static OperateResult<T> Success<T>(string message, T? content)
                => new OperateResult<T>(true, message, content);

            public static OperateResult Fail()
                => new OperateResult(false, "失败");
            public static OperateResult Fail(string message, Exception? exception = null)
                => new OperateResult(false, message, exception);
            public static OperateResult<T> Fail<T>(string message, T? content = default, Exception? exception = null)
                => new OperateResult<T>(false, message, content, exception);

            public override string ToString()
                => $" {(IsSuccess ? "✅ Success" : "❌ Failed")} : Message={Message} ";

        }

        public class OperateResult<T> : OperateResult
        {
            internal OperateResult(bool isSuccess, string message, T? content = default, Exception? exception = null)
                : base(isSuccess, message, exception)
            {
                Content = content;
            }

            public T? Content { get; }

            public override string ToString()
                => $" {(IsSuccess ? "✅ Success" : "❌ Failed")} : Message={Message}, Content={Content} ";

        }

        public static class OperateResultExtensions
        {
            public static OperateResult<T> ToOperateResult<T>(this T data)
                => OperateResult.Success(data);

            public static T GetContentFromResult<T>(this OperateResult<T> action)
                => action.IsSuccess
                    ? action.Content
                    : default;

            public static async Task<T> GetContentFromResultAsync<T>(this Task<OperateResult<T>> action, bool configureAwait)
            {
                var result = await action.ConfigureAwait(configureAwait);
                return result.IsSuccess
                    ? result.Content
                    : default;
            }

        }
     */
    public class OperateResult
    {
        public OperateResult(bool isSuccess, int code, string message, Exception? exception = null)
        {
            IsSuccess = isSuccess;
            Code = code;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess { get; protected set; }
        /// <summary>
        /// 结果码（可根据需求自定义成功或者失败的结果码）
        /// </summary>
        public int Code { get; protected set; }
        /// <summary>
        /// 操作返回的消息
        /// </summary>
        public string Message { get; protected set; }
        /// <summary>
        /// 操作是否有异常
        /// </summary>
        public Exception? Exception { get; protected set; }



        #region Success Methods
        public static OperateResult Success()
            => new OperateResult(true, 200, MessageResources.SuccessText);

        public static OperateResult Success(string message)
            => new OperateResult(true, 200, message);

        //////////////////// 泛型
        public static OperateResult<T> Success<T>(T? content)
            => new OperateResult<T>(true, 200, MessageResources.SuccessText, content);

        public static OperateResult<T> Success<T>(string message, T? content)
            => new OperateResult<T>(true, 200, message, content);
        #endregion



        #region Failure Methods
        public static OperateResult Fail()
            => new OperateResult(false, 10000, MessageResources.FailedText);

        public static OperateResult Fail(string message, Exception? exception = null)
            => new OperateResult(false, 10000, message, exception);

        public static OperateResult Fail(int code, string message, Exception? exception = null)
            => new OperateResult(false, code, message, exception);

        //////////////////// 泛型
        public static OperateResult<T> Fail<T>(string message, T? content = default, Exception? exception = null)
            => new OperateResult<T>(false, 10000, message, content, exception);

        public static OperateResult<T> Fail<T>(int code, string message, T? content = default, Exception? exception = null)
            => new OperateResult<T>(false, code, message, content, exception);
        #endregion

        public override string ToString()
            => $" {(IsSuccess ? "✅ Success" : "❌ Failed")} : Code={Code}, Message={Message} ";

    }

    public class OperateResult<T> : OperateResult
    {
        internal OperateResult(bool isSuccess, int code, string message, T? content = default, Exception? exception = null)
            : base(isSuccess, code, message, exception)
        {
            Content = content;
        }

        /// <summary>
        /// 操作返回的内容
        /// </summary>
        public T? Content { get; }

        public override string ToString()
            => $" {(IsSuccess ? "✅ Success" : "❌ Failed")} : Code={Code}, Message={Message}, Content={Content} ";

    }

    public static class OperateResultExtensions
    {
        /// <summary>
        /// 使用示例：var myData = 42.ToOperateResult(); // OperateResult<int>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static OperateResult<T> ToOperateResult<T>(this T data)
            => OperateResult.Success(data);

        /// <summary>
        /// 若前面的方法执行成功，返回执行结果的内容，否则返回 default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T GetContentFromResult<T>(this OperateResult<T> action)
            => action.IsSuccess
                ? action.Content
                //: throw new InvalidOperationException(action.Message);
                : default;    // 返回 default 或者处理错误逻辑

        /// <summary>
        /// 若前面的异步方法执行成功，返回执行结果的内容，否则返回 default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<T> GetContentFromResultAsync<T>(this Task<OperateResult<T>> action, bool configureAwait)
        {
            // 错误示例：使用 .Result 会同步阻塞当前线程直到任务完成获取结果，表面上看起来是异步方法，实际上同步执行，在UI线程中使用会导致界面卡顿，在ASP.NET Core中可能导致死锁，失去了异步编程的优势
            //return action.Result.IsSuccess
            //    ? action.Result.Content
            //    //: throw new InvalidOperationException(action.Message);
            //    : default(T);    // 返回 default 或者处理错误逻辑

            var result = await action.ConfigureAwait(configureAwait);
            return result.IsSuccess
                ? result.Content
                //: throw new InvalidOperationException(action.Message);
                : default;   // 返回 default 或者处理错误逻辑
        }

        /// <summary>
        /// 对 OperateResult<T> 进行数据转换的函数式编程操作，
        /// 将成功状态的 OperateResult<T> 内容转换为 OperateResult<TNew>，失败状态直接传递错误
        /// <para>
        /// 使用场景：
        ///     1、数据格式转换：（如：从设备读取的原始字节 → 解析后的结构体）
        ///     2、业务逻辑映射：如：数据库实体 → DTO对象）
        ///     3、错误传播：（自动保留原始操作的错误状态）
        /// </para>
        /// <code>
        /// 使用示例：
        ///     // 从设备读取温度（原始值为int）
        ///     OperateResult<int> rawTemp = device.ReadTemperature();
        ///     
        ///     // 转换为带单位的字符串
        ///     OperateResult<string> formattedTemp = rawTemp.Map(t => $"{t}°C");
        ///     
        ///     // 输出结果：Success: Content="25°C" 或 Failed: Message="传感器故障"
        ///     Console.WriteLine(formattedTemp);
        /// 
        /// 进阶用法：
        ///     // 组合多个Map链式调用
        ///     plc.ReadInputRegister("IN100")
        ///         .Map(raw => raw / 10.0)          // 转换为实际值
        ///         .Map(v => Math.Round(v, 2))      // 保留2位小数
        ///         .Map(v => $"{v} mA");            // 添加单位
        ///         .Map(s => $"SN: {s}")            // 添加前缀
        ///         .Map(s => s.ToUpper())           // 转为大写
        ///         .OnSuccess(s => Display(s));     // 最终处理
        /// 实际应用案例
        ///     案例1：工业设备数据处理
        ///         // 从PLC读取原始字节
        ///         OperateResult<byte[]> rawData = plc.ReadBytes("D100");
        /// 
        ///         // 转换为浮点数（自动处理错误传播）
        ///         OperateResult<float> temperature = rawData.Map(bytes => 
        ///         {
        ///             if (bytes.Length < 4) throw new InvalidDataException();
        ///             return BitConverter.ToSingle(bytes, 0);
        ///         });
        /// 
        ///         // 使用结果
        ///         if (temperature.IsSuccess)
        ///             controlPanel.ShowTemperature(temperature.Content);
        ///         else
        ///             alarmSystem.Trigger($"PLC数据错误: {temperature.Message}");
        ///     案例2：API响应处理
        ///         // 模拟API调用
        ///         OperateResult<HttpResponse> apiResponse = CallRemoteApi();
        /// 
        ///         // 链式转换
        ///         OperateResult<UserDto> userResult = apiResponse
        ///             .Map(r => JsonSerializer.Deserialize<UserRaw>(r.Body))
        ///             .Map(raw => new UserDto(raw.Id, raw.Name.ToTitleCase()));
        /// 
        ///         // 最终处理
        ///         userResult.OnSuccess(u => SaveToDatabase(u))
        ///                 .OnFailure(e => LogError(e));
        /// 
        /// 与直接处理的对比：
        ///     传统方式（无Map）：
        ///         OperateResult<int> raw = GetData();
        ///         OperateResult<string> action;
        /// 
        ///         if (raw.IsSuccess)
        ///             action = OperateResult.Success(raw.Content.ToString());
        ///         else
        ///             action = OperateResult.Fail<string>(raw.Code, raw.Message);
        ///     函数式方式（使用Map）：
        ///         var action = GetData().Map(x => x.ToString());
        ///     优势：
        ///         1、减少 if-else 嵌套
        ///         2、代码更声明式
        ///         3、错误处理逻辑集中
        /// 
        /// 设计原理：
        ///     Monad模式：Map 实现了函数式编程中的 Functor 模式，允许在容器（OperateResult）内安全地应用函数。
        ///     错误短路：一旦出现失败状态，后续 Map 会跳过转换直接传递错误。
        ///     不可变性：每次 Map 返回新对象，原始结果保持不变。
        /// 
        /// 通过 Map 方法，可以构建出高可读性、低耦合的数据处理管道，特别适合工业通信和业务逻辑中的链式数据转换场景。
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="result"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static OperateResult<TNew> Map<T, TNew>(this OperateResult<T> result, Func<T, TNew> mapper)
            => result.IsSuccess
                ? OperateResult.Success(mapper(result.Content))             // 成功：应用转换
                : OperateResult.Fail<TNew>(result.Code, result.Message, default);   // 失败：传递错误

        /// <summary>
        /// 条件过滤
        /// <code>
        /// // 使用示例
        /// sensor.Read()
        ///     .Map(v => v* 100)
        ///     .Where(v => v < 50)     // 只接受小于50的值
        ///     .OnSuccess(v => Console.WriteLine($"有效值: {v}"));
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static OperateResult<T> Where<T>(this OperateResult<T> result, Func<T, bool> predicate)
            => result.IsSuccess && predicate(result.Content)
                ? result
                : OperateResult.Fail("条件过滤失败，数据不符合条件", default(T));


        #region Then 
        /* 工业通信场景案例
            同步场景（设备控制）
                OperateResult controlResult = CheckDeviceStatus()
                    .Then(() => EnableSafetyLock())
                    .Then(prev => SendControlCommand(Command.Start))
                    .Then(prev => LogOperation("Device started"));

                if (!controlResult.IsSuccess)
                {
                    alarmSystem.Trigger(controlResult.Message);
                }
            异步场景（数据采集）
                await sensor.InitializeAsync()
                    .ThenAsync(async _ => await sensor.CalibrateAsync())
                    .ThenAsync(async prev => 
                    {
                        var data = await sensor.ReadDataAsync();
                        return data.Map(d => d * calibrationFactor);
                    })
                    .OnSuccessAsync(data => SaveToDatabaseAsync(data))
                    .OnFailureAsync(ex => SendAlertAsync($"采集失败: {ex.Message}"));
        设计要点
            1、错误短路：任一环节失败后跳过后续操作
            2、类型安全：泛型版本保持输入输出类型约束
            3、上下文传递：带参数版本可访问前序结果
            4、同步/异步统一：提供对称的API设计

        这种方法特别适合工业控制领域的级联操作，既能保持代码整洁，又能实现完善的错误传播机制。
         */

        /// <summary>
        /// 结果链式处理，方法连续操作
        /// <code>
        /// // 连续操作示例（任一失败则终止）
        /// var finalResult = FirstStep()
        ///     .Then(SecondStep)
        ///     .Then(ThirdStep);
        /// </code>
        /// </summary>
        /// <param name="first"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static OperateResult Then(this OperateResult first, Func<OperateResult> next)
            => first.IsSuccess ? next() : first;

        public static async Task<OperateResult> ThenAsync(this OperateResult first, Func<Task<OperateResult>> next)
            => first.IsSuccess ? await next() : first;

        public static async Task<OperateResult> ThenAsync(this Task<OperateResult> first, Func<Task<OperateResult>> next)
        {
            var result = await first.ConfigureAwait(false);
            return result.IsSuccess ? await next() : result;
        }

        /// <summary>
        /// 泛型版本（支持传递内容）
        /// <code>
        /// // 使用示例：链式操作验证
        /// OperateResult action = ValidateInput(input)
        ///     .Then(prev => CheckPermissions(prev.IsSuccess ? userId : 0))
        ///     .Then(prev => SaveToDatabase(input));
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static OperateResult<T> Then<T>(this OperateResult<T> first, Func<T, OperateResult<T>> next)
            => first.IsSuccess ? next(first.Content) : first;

        public static async Task<OperateResult<T>> ThenAsync<T>(this OperateResult<T> first, Func<T, Task<OperateResult<T>>> next)
            => first.IsSuccess ? await next(first.Content) : first;

        public static async Task<OperateResult<T>> ThenAsync<T>(this Task<OperateResult<T>> first, Func<T, Task<OperateResult<T>>> next)
        {
            var result = await first.ConfigureAwait(false);
            return result.IsSuccess ? await next(result.Content) : result;
        }

        /// <summary>
        /// 泛型版本（支持传递内容）
        /// <code>
        /// // 使用示例：泛型版本
        /// OperateResult<int> idResult = ParseId("123")
        ///     .Then(parsedId => GetUserById(parsedId))
        ///     .Then(user => UpdateUser(user));
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="first"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static OperateResult<TResult> Then<T, TResult>(this OperateResult<T> first, Func<T, OperateResult<TResult>> next)
            => first.IsSuccess
                ? next(first.Content)
                : OperateResult.Fail<TResult>(first.Code, first.Message, default);

        public static async Task<OperateResult<TResult>> ThenAsync<T, TResult>(this OperateResult<T> first, Func<T, Task<OperateResult<TResult>>> next)
            => first.IsSuccess
                ? await next(first.Content)
                : OperateResult.Fail<TResult>(first.Code, first.Message, default);
        #endregion Then










    }

    public static class MessageResources
    {
        public static string ConnectedFailed => "连接失败：";
        public static string UnknownError => "未知错误";
        public static string code => "错误代号";
        public static string TextDescription => "文本描述";
        public static string ExceptionMessage => "错误信息：";
        public static string ExceptionSourse => "错误源：";
        public static string ExceptionType => "错误类型：";
        public static string ExceptionStackTrace => "错误堆栈：";
        public static string ExceptopnTargetSite => "错误方法：";
        public static string ExceprionCustomer => "用户自定义方法出错：";
        public static string SuccessText => "成功";
        public static string FailedText => "失败";
        public static string UserIsNotExist => "用户不存在";
    }
}