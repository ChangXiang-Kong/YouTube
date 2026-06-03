namespace AvaloniaApplication1.Tools.ListBoxLog;

public class LogMessage
{
    public string DateTimeStr { get; set; } = string.Empty;
    public LogType LogType { get; set; } = LogType.Tip;
    public string? Title { get; set; } = string.Empty;
    public string? SubTitle { get; set; } = string.Empty;
    public string? OtherInfo { get; set; } = string.Empty;
    public string? Message { get; set; } = string.Empty;
    public bool BoldTitleFont { get; set; } = false;
    public bool BoldSubTitleFont { get; set; } = false;
    public bool BoldOtherInfoFont { get; set; } = false;
    public bool BoldMessageFont { get; set; } = false;
    public bool ShowLogType { get; set; } = true;
    public bool ShowDate { get; set; } = true;
    public bool ShowTime { get; set; } = true;
    public bool ShowMilliseconds { get; set; } = false;
    public int MillisecondsLength
    {
        get;
        set
        {
            if (value < 1)
                value = 1;
            if (value > 6)
                value = 6;
            field = value;
        }
    } = 4;
}

public enum LogType
{
    Tip,
    Default,
    Info,
    Success,
    Warning,
    Error,
    Fatal,
}