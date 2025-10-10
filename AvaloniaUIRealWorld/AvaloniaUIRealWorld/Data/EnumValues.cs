using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaUIRealWorld.Data.EnumValues
{
    public enum ApplicationPageNames
    {
        Unknown,
        Home,
        Process,
        Actions,
        Macros,
        Reporter,
        History,
        Settings,
        Error,
    }
    
    public enum ActionsPageName
    {
        Print = 0,
        CustomProperties = 1,
        FileInfo = 2,
        SaveModelAs = 3,
        SaveDrawingAs = 4,
        ImportFile = 5,
        DrawingTemplates = 6,
        Macros = 7
    }
    
}
