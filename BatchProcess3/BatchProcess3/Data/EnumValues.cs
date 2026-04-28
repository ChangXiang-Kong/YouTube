using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcess3.Data
{
    public enum ApplicationPageName
    {
        Unknown = 0,
        Home = 1,
        Process = 2,
        Actions = 3,
        Macros = 4,
        Reporter = 5,
        History = 6,
        Settings = 7,
        Detail = 8,
    }
    
    public enum ActionsPageName
    {
        Unknown = 0,
        Print = 1,
        CustomProperties = 2,
        FileInfo = 3,
        SaveModelAs = 4,
        SaveDrawingAs = 5,
        ImportFile = 6,
        DrawingTemplates = 7,
        Macros = 8,
    }
    
}
