using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datatypes;

/// <summary>
/// Provides structured control over the values of each of the data fields in the variable set.
/// </summary>
/// <param name="JBKNumber"></param>
/// <param name="LotNumber"></param>
/// <param name="DeburrJBKNumber"></param>
/// <param name="DieNumber"></param>
/// <param name="ModelNumber"></param>
/// <param name="HeatNumber"></param>
public partial class VariableFieldSet(JBKNumber? JBKNumber = null, LotNumber? LotNumber = null, JBKNumber? DeburrJBKNumber = null, DieNumber? DieNumber = null, ModelNumber? ModelNumber = null, HeatNumber? HeatNumber = null) : ObservableObject()
{
    [ObservableProperty]
    public partial JBKNumber? JBKNumber { get; set; } = JBKNumber;

    [ObservableProperty]
    public partial LotNumber? LotNumber { get; set; } = LotNumber;

    [ObservableProperty]
    public partial JBKNumber? DeburrJBKNumber { get; set; } = DeburrJBKNumber;

    [ObservableProperty]
    public partial DieNumber? DieNumber { get; set; } = DieNumber;

    [ObservableProperty]
    public partial ModelNumber? ModelNumber { get; set; } = ModelNumber;

    [ObservableProperty]
    public partial HeatNumber? HeatNumber { get; set; } = HeatNumber;

    /// <summary>
    /// Converts the VariableFieldSet into a JSON stream.
    /// </summary>
    /// <returns></returns>
    public string ToJSON()
    {
        // add each required field to the JSON stream
        string JSON = "{";
        if (JBKNumber is not null)
        {
            JSON += $"\"JBKNumber\":\"{JBKNumber!.Literal}\",";
        }
        if (LotNumber is not null)
        {
            JSON += $"\"LotNumber\":\"{LotNumber!.Literal}\",";
        }
        if (DeburrJBKNumber is not null)
        {
            JSON += $"\"DeburrJBKNumber\":\"{DeburrJBKNumber!.Literal}\",";
        }
        if (DieNumber is not null)
        {
            JSON += $"\"DieNumber\":\"{DieNumber!.Literal}\",";
        }
        if (ModelNumber is not null)
        {
            JSON += $"\"ModelNumber\":\"{ModelNumber!.Code}\",";
        }
        if (HeatNumber is not null)
        {
            JSON += $"\"HeatNumber\":\"{HeatNumber!.Literal}\",";
        }
        // remove trailing comma if any field was added
        if (JSON.Length > 1)
        {
            JSON = JSON[..^1];
        }
        JSON += "}";
        return JSON;
    }

    /// <summary>
    /// Attempts to parse a VariableFieldSet from a JSON stream.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public static VariableFieldSet ParseJSON(string Line)
    {
        // parse Line into JTokens
        JObject JSON = JObject.Parse(Line);
        // attempt to parse each of the field types
        VariableFieldSet VariableFields = new VariableFieldSet();
        try
        {
            VariableFields.JBKNumber = new JBKNumber(int.Parse(JSON["JBKNumber"]!.ToString()));
        }
        catch
        {
            VariableFields.JBKNumber = null;
        }
        try
        {
            VariableFields.LotNumber = new LotNumber(int.Parse(JSON["LotNumber"]!.ToString()));
        }
        catch
        {
            VariableFields.LotNumber = null;
        }
        try
        {
            VariableFields.DeburrJBKNumber = new JBKNumber(int.Parse(JSON["DeburrJBKNumber"]!.ToString()));
        }
        catch
        {
            VariableFields.DeburrJBKNumber = null;
        }
        try
        {
            VariableFields.DieNumber = new DieNumber(int.Parse(JSON["DieNumber"]!.ToString()));
        }
        catch
        {
            VariableFields.DieNumber = null;
        }
        try
        {
            VariableFields.ModelNumber = new ModelNumber(JSON["ModelNumber"]!.ToString());
        }
        catch
        {
            VariableFields.ModelNumber = null;
        }
        try
        {
            VariableFields.HeatNumber = new HeatNumber(int.Parse(JSON["HeatNumber"]!.ToString()));
        }
        catch
        {
            VariableFields.HeatNumber = null;
        }
        return VariableFields;
    }
}