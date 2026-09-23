using System.Collections.Generic;

namespace BatchProcess3.EntityFramework.Entities.Actions;

public class PrintSettingsEntity : BaseEntity
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
    
    public int Copies = 1;

    // 不能 new()
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public List<PrintSettingsProfileEntity> PrintSettingsProfilesList = []; // 不要 new()
    public List<PrintSettingsProfileEntity> PrintSettingsProfilesList; // 不要 new()
    
    // 不能 new()
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public List<PrintTabEntity> PrintTabsList = []; // 不要 new()
    public List<PrintTabEntity> PrintTabsList; // 不要 new()

    public bool CanEdit { get; set; }
    
    public bool CanDelete { get; set; }
}