using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityQuickSheet;
using GDataDB;
using GDataDB.Impl;
using Google.GData.Spreadsheets;

public class test : MonoBehaviour
{
    [MenuItem("Tools/test")]
    private static void das()
    {
        var client = new DatabaseClient("", "");

        string error = string.Empty;
        var db = client.GetDatabase("CardGamePrototypeB", ref error);
        if (db != null)
        {
            var worksheet = ((Database)db).GetWorksheetEntry("TEST");
            // Fetch the cell feed of the worksheet.
            CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
            var cellFeed = client.SpreadsheetService.Query(cellQuery);

            // Iterate through each cell, printing its value.
            foreach (CellEntry cell in cellFeed.Entries)
            {
                // onCell(cell);
            }
        }
    }

}
