using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Actions;

namespace BatchProcess3.Tools.Services;

public class PrinterService
{
    public ObservableCollection<PrintersViewModel> GetAvailablePrinters()
    {
        var printers = new ObservableCollection<PrintersViewModel>();

        printers.Add(new PrintersViewModel() { Name = "(Default)" });

        // if (OperatingSystem.IsWindows())
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var printDocument = new PrintDocument();
            
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                printDocument.PrinterSettings.PrinterName = printerName;
                
                var printerDetailsViewModel = new PrintersViewModel() { Name = printerName };
                
                // Add PaperSizes option
                printerDetailsViewModel.PaperSizes.Add("(Default)");
                foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
                {
                    printerDetailsViewModel.PaperSizes.Add(paperSize.PaperName);
                }
                
                // Add SourceTrays option
                printerDetailsViewModel.SourceTrays.Add("(Default)");
                foreach (PaperSource sourceTray in printDocument.PrinterSettings.PaperSources)
                {
                    printerDetailsViewModel.SourceTrays.Add(sourceTray.SourceName);
                }
                
                printers.Add(printerDetailsViewModel);
            }
        }
        
        // TODO: Handles windows logic to fetch priters from OS
        
        return printers;
    }
}