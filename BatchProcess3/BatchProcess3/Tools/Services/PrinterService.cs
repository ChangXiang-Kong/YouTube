using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Tools.Services;

public class PrinterService
{
    public ObservableCollection<PrinterDetailViewModel> GetAvailablePrinters()
    {
        var printers = new ObservableCollection<PrinterDetailViewModel>();

        printers.Add(new PrinterDetailViewModel() { Name = "(Default)" });

        // if (OperatingSystem.IsWindows())
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var printDocument = new PrintDocument();
            
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                printers.Add(new PrinterDetailViewModel() { Name = printerName });
                
                printDocument.PrinterSettings.PrinterName = printerName;
                // printDocument.PrinterSettings.PaperSizes = printerName;
                
            }
        }
        
        // TODO: Handles windows logic to fetch priters from OS
        
        return printers;
    }
}