using System;
using System.Collections.Generic;

namespace BatchProcess3.EntityFramework.Entities;

public class SettingsEntity : BaseEntity
{
    public List<string> LocationPaths { get; set; } = [];
}