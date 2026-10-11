using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BatchProcess3.Core.SolidWorksHost;

namespace BatchProcess3.Tools.SolidWorks;

public class BatchProcessClient
{
    public bool GetDummyData { get; set; } = false;

    private readonly string _dummyDataPath = Path.Combine(
        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
        "..", "..", "..", "..", "BatchProcess3", "SolidWorks", "SampleFiles");

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private string _hostAddress = "";

    public void Connect(string hostAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hostAddress);
        
        _hostAddress = hostAddress;
    }
    
    public async Task<List<SolidWorksFileDetails>> GetActiveFileReferencesAsync()
    {
        try
        {
            if (GetDummyData)
            {
                
            }

            var httpClient = new HttpClient();
            
            var response = await httpClient.GetAsync(_hostAddress + BatchProcessHostUrls.SolidWorksActiveFileList);
            
            var responseString = await response.Content.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<List<SolidWorksFileDetails>>(responseString, _jsonOptions) ?? [];

            return result;
        }
        catch (Exception e)
        {
            return [];
        }
    }
}