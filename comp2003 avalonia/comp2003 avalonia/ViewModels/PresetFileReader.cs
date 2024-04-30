using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace comp2003_avalonia.ViewModels;

internal class PresetFileReader
{

    //Just a JSON Read and Desearilser, here to reduce main screen clutter
    public static PresetsDataDump? FileReader(string fileName)
    {
        PresetsDataDump presetDump = new PresetsDataDump();
        string jsonString = File.ReadAllText("Presets.json");
        try
        {
            presetDump = JsonSerializer.Deserialize<PresetsDataDump>(jsonString);
                
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "Unable to read file, it is either in non standard format or does not exist");
        }

        return presetDump;
    }





    /*
    * The wrapper class which contains 3 PresetDataModels, to be accessed the following way
    * instancename.Preset1.Method() with the ability to access any preset seperately
    * this leaves the Json with the following heirarchy of Presets which contains 3 entries
    * of PresetDataModel
    */
    public class PresetsDataDump
    {
        public PresetDataModel Preset1 { get; set; } = new PresetDataModel();
        public PresetDataModel Preset2 { get; set; } = new PresetDataModel();
        public PresetDataModel Preset3 { get; set; } = new PresetDataModel();
    }

    /*
    * PresetDataModel is our storage for each preset individually, it contains:
    * Dictionary<string, string> Presetname with a single entry storing the name of the preset 
    * Dictionary<string, double> Dict which contains all the entries for the slider values and relevant names
    * 
    * There are methods for getting and setting each, however setting preset Dict requires a full dictionary passed, 
    * which overwrites the old one
    */
    public class PresetDataModel
    {
        [JsonPropertyName("presetName")]
        public Dictionary<string, string>? PresetName { get; set; }
        [JsonPropertyName("Dict")]
        public Dictionary<string, double>? Dict { get; set; }
        public string GetName() => PresetName["presetName"];
        public Dictionary<string, double> GetPresetValue() => Dict;
        public void SetName(string newName) { PresetName["presetName"] = newName; }
        public void SetPresetValue(Dictionary<string, double> newPreset) { Dict = newPreset; }


    }
}


