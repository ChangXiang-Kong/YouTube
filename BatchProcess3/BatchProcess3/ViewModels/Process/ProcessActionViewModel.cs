using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.EntityFramework.Entities.Process;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Process;

public partial class ProcessActionViewModel : ActionViewModel
{
    /// <summary>
    /// The underlying action Id
    /// </summary>
    [ObservableProperty]
    private string? _actionId;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _processId = "";

    // C# 字符串切片语法说明：
    //     写法	        含义                          说明
    //     s[..4]	    前 4 位，0~3
    //     s[^4..]	    最后 4 位                      `^` 表示 从末尾开始索引，⚠️边界注意：字符串不足 4 个字符会抛异常
    //     s[^4..^2]    倒数第 4 位 到 倒数第 2 位（不含）
    // public string TestString => $"{JobName} (Id: {Id[..4]} AId: {ActionId[..4]}) ({SortOrder})";
    public string TestString => $"{JobName} (Id: {Id[^4..]} AId: {ActionId[^4..]}) ({SortOrder})";
}

public static class ProcessActionViewModelExtensions
{
    public static ProcessActionEntity ToEntity(this ProcessActionViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        JobName = viewModel.JobName,
        Description = viewModel.Description,
        SortOrder = viewModel.SortOrder,
        ProcessId = viewModel.ProcessId,
        ActionId = viewModel.ActionId ?? ""
    };
    
    public static ProcessActionViewModel ToViewModel(this ProcessActionEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        SortOrder = entity.SortOrder,
        ProcessId = entity.ProcessId,
        ActionId = entity.ActionId ?? ""
    };
    
    public static ProcessActionViewModel ToProcessActionViewModel(this ActionEntity entity) => new()
    {
        ActionId = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        SortOrder = entity.SortOrder,
        // ProcessId = entity.ProcessId,
    };
}