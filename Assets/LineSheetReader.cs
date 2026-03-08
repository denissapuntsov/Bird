using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;


[ExecuteInEditMode]
public class LineSheetReader : MonoBehaviour
{
    [SerializeField] private string sheet = "Sheet1";
    [SerializeField] private string rangeMin = "A";
    [SerializeField] private string rangeMax = "J";
    
    private static string spreadsheetId = "1cxG14Y6nzJhHcV808kAELi5T5duPYRSI-4ROHRH1yG8";
    private static string jsonPath = "/Credentials/bird-489617-8a2372cf4532.json";
 
    private static SheetsService _service;
    
    [InspectorButton("UpdateValues")] public bool update;
    
    static LineSheetReader()
    {
        string fullPath = Application.streamingAssetsPath + jsonPath;
        
        Stream jsonCred = File.Open(fullPath, FileMode.Open);
        
        ServiceAccountCredential credential = ServiceAccountCredential.FromServiceAccountData(jsonCred);

        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });
    }

    private void UpdateValues()
    {
        var sheetRangeValues = GetSheetRange($"{sheet}!{rangeMin}:{rangeMax}");
        
       foreach (IList<object> row in sheetRangeValues)
       {
           foreach (object column in row)
           {
               Debug.Log(column);
           }
       }
    }

    public IList<IList<object>> GetSheetRange(string sheetNameAndRange)
    {
        SpreadsheetsResource.ValuesResource.GetRequest request = 
            _service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);
        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            return values;
        }
        Debug.Log("No data found.");
        return null;
    }
}
