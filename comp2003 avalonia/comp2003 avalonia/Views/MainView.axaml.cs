using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Text.Json;
using System.Globalization;
using comp2003_avalonia.ViewModels;
using static comp2003_avalonia.ViewModels.PresetFileReader;
using System.IO.Enumeration;
using Avalonia.Styling;

namespace comp2003_avalonia.Views;

public partial class MainView : UserControl
{

    bool simulationComplete = false;
    string paraLocation = "";
    private AppConfiguration appConfig;
    bool paraButtonPressed = true;
    string programMode = "light";

    PresetsDataDump presetDump = new PresetFileReader.PresetsDataDump();


    int presetToChange = 0;




    public MainView()
    {
        InitializeComponent();

        new ComponentInitialise().InitialiseBaseValues(this);

        ThemeButton.Content = "Dark Mode";
        
        try
        {
            FileInitialiser();

        }
        catch
        {

        }

        LoadConfiguration();
    }

    



    public void SubmitClicked(object sender, RoutedEventArgs args)
    {
        GenerateConfig();
        StartSimulation(false);
        SaveConfiguration();
    }



    public async void StartSimulation(Boolean forceParaView)
    {
        string leelaExePath = new ComponentInitialise().GetLeelaLocation();
        paraLocation = paraViewLocation.Text;


        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            leelaExePath = new ComponentInitialise().GetLinuxLeelaLocation();
            Console.WriteLine("using linux " + leelaExePath);
        }


        string fullLeelaExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, leelaExePath);
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = fullLeelaExePath;
        info.WindowStyle = ProcessWindowStyle.Normal;
        info.UseShellExecute = true;

        Process leelaProccess = new Process();
        leelaProccess.StartInfo = info;
        leelaProccess.EnableRaisingEvents = true;
        leelaProccess.Exited += (sender, e) => OnSimulationComplete();
        //leelaProccess.Start();
        Boolean canRun = true;

        if (structureBox.Text == "" || structureBox.Text == null)
        {
            simulationStructureError.IsVisible = true;
            canRun = false;
        }
        else
        {
            simulationStructureError.IsVisible = false;
        }



        if (paraButtonPressed == true || forceParaView == true)
        {
            if (paraViewLocation.Text == "" || paraViewLocation.Text == null)
            {
                selectParaLabel.IsVisible = true;
            }
            else
            {
                selectParaLabel.IsVisible = false;
                if (canRun == true)
                {
                    simulationStructureError.IsVisible = false;
                    leelaProccess.Start();
                }
            }
        }
        else
        {
            selectParaLabel.IsVisible = false;
            if (canRun == true)
            {
                simulationStructureError.IsVisible = false;
                Process.Start(info);
            }
        }

    }

    private void OnSimulationComplete()
    {
        simulationComplete = true;
        if (simulationComplete)
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) == false){
            StartParaView();
            //}
        }
    }


    public void GenerateConfig()
    {
        string configString = new ComponentInitialise().GenerateLeelaConfigString(this, "simulation");
        string configFileName = "leela-db.config";
        string conFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);


        //string updatedPath = FileSystem.Current.AppDataDirectory;




        using (StreamWriter writer = new StreamWriter(conFilePath))
        {
            writer.Write(configString);
        }
    }

    public void StartParaView()
    {

        simulationComplete = true;

        if (simulationComplete)
        {
            Process paraViewprocess = new Process();
            //String fName = "C:\\Program Files\\ParaView 5.12.0\\bin\\paraview.exe";
            String fName = paraLocation;


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

                fName = paraLocation;
            }


            paraViewprocess.StartInfo.FileName = fName;
            string vtuPath = @"/data.pvd";
            string fullVtuPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vtuPath);
            Console.WriteLine(fullVtuPath);
            paraViewprocess.StartInfo.Arguments = $"--data={fullVtuPath}";
            paraViewprocess.Start();
        }

    }

    public async void BrowseClicked(object sender, RoutedEventArgs args)
    {


        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open ParaView",
            AllowMultiple = false,
        });

        if (files.Count >= 1)
        {
            paraViewLocation.Text = files[0].Path.ToString();

            if (files[0].Path.ToString().StartsWith("file:///"))
            {
                paraViewLocation.Text = files[0].Path.ToString().Substring(8);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                paraViewLocation.Text = "/" + paraViewLocation.Text;
            }

        }

        appConfig.ParaViewPath = paraViewLocation.Text;
        SaveConfiguration();

    }

    public void paraYesClicked(object sender, RoutedEventArgs args)
    {
        paraYesButton.Background = new SolidColorBrush(Colors.Blue);
        paraYesButton.Foreground = new SolidColorBrush(Colors.White);
        paraNoButton.Background = new SolidColorBrush(Colors.LightGray);
        paraNoButton.Foreground = new SolidColorBrush(Colors.Black);
        paraButtonPressed = true;



    }

    public void paraNoClicked(object sender, RoutedEventArgs args)
    {
        paraNoButton.Background = new SolidColorBrush(Colors.Blue);
        paraNoButton.Foreground = new SolidColorBrush(Colors.White);
        paraYesButton.Background = new SolidColorBrush(Colors.LightGray);
        paraYesButton.Foreground = new SolidColorBrush(Colors.Black);
        paraButtonPressed = false;




    }

    public void HardCodePresets(object sender, RoutedEventArgs args)
    {
        Button clickedButton = (Button)sender;
        string buttonTag = clickedButton.Tag?.ToString();

        
        DiscolourPresets();
        Dictionary<string, double> presetDict = null;

        switch (buttonTag)
        {
            case "1":
                preset1.Background = new SolidColorBrush(Colors.Blue);
                preset1.Foreground = new SolidColorBrush(Colors.White);
                presetDict = PresetValues.HardCodedPresetFetch(1);
                ApplyPresets(presetDict);
                break;
            case "2":
                preset2.Background = new SolidColorBrush(Colors.Blue);
                preset2.Foreground = new SolidColorBrush(Colors.White);
                presetDict = PresetValues.HardCodedPresetFetch(2);
                ApplyPresets(presetDict); 
                break;
            case "3":
                preset3.Background = new SolidColorBrush(Colors.Blue);
                preset3.Foreground = new SolidColorBrush(Colors.White);
                presetDict = PresetValues.HardCodedPresetFetch(3);
                ApplyPresets(presetDict);
                break;
        }
    }

    public void UserPresets(object sender, RoutedEventArgs args)
    {
        Button clickedButton = (Button)sender;
        string? buttonTag = clickedButton.Tag?.ToString();

        DiscolourPresets();
        Dictionary<string, double>? presetDict = null;

        switch (buttonTag)
        {
            case "1":
                userPreset1.Background = new SolidColorBrush(Colors.Blue);
                userPreset1.Foreground = new SolidColorBrush(Colors.White);
                presetDict = presetDump.Preset1.GetPresetValue();
                presetToChange = 1;
                break;
            case "2":
                userPreset2.Background = new SolidColorBrush(Colors.Blue);
                userPreset2.Foreground = new SolidColorBrush(Colors.White);
                presetDict = presetDump.Preset2.GetPresetValue();
                presetToChange = 2;
                break;
            case "3":
                userPreset3.Background = new SolidColorBrush(Colors.Blue);
                userPreset3.Foreground = new SolidColorBrush(Colors.White);
                presetDict = presetDump.Preset3.GetPresetValue();
                presetToChange = 3;
                break;
        }
        ApplyPresets(presetDict);
    }


    //User has selected to save, this just shows future options for saving or not
    public void FirstSavePreset(object sender, RoutedEventArgs args)
    {
        SavePreset.IsVisible = false;
        ConfirmSaves.IsVisible = true;
    }

    //Actual saving confirmation button
    public void SecondSavePreset(object sender, RoutedEventArgs args)
    {

        SavePreset.IsVisible = true;
        ConfirmSaves.IsVisible = false;

        //Create dict and intialise with user values
        Dictionary<string, double> presetDict = new Dictionary<string, double>
        {
            {"dimensions", dimProblemSlider.Value},
            {"gravityX", gravSliderX.Value},
            {"gravityZ", gravSliderZ.Value},
            {"gravityY", gravSliderY.Value},
            {"refDensity", refDenSlider.Value},
            {"refSoundSpeed", refSoundSlider.Value},
            {"defaultParticleSpacing", defaultParticleSpacingSlider.Value},
            {"expRatioH", expRatioHSlider.Value},
            {"HKappa", HKappaSlider.Value},
            {"startTime", startTimeSlider.Value},
            {"endTime", endTimeSlider.Value},
            {"maxTimeStep", maxTimeStepSlider.Value},
            {"timeStepCoeff", timeStepCoefficientSlider.Value},
            {"timeStepVisCoeff", timeStepViscosityCoefficientSlider.Value},
            {"timeStepSurfTensionCoeff", timeStepSurfaceTensionCoefficientSlider.Value},
            {"outputInterval", outputIntervalSlider.Value},
        };

        switch (presetToChange)
        {
            case 1:
                presetDump.Preset1.SetPresetValue(presetDict);
                presetDump.Preset1.SetName(presetNameBox.Text);
                userPreset1.Content = presetNameBox.Text;
                break;
            case 2:
                presetDump.Preset2.SetPresetValue(presetDict);
                presetDump.Preset2.SetName(presetNameBox.Text);
                userPreset2.Content = presetNameBox.Text;
                break;
            case 3:
                presetDump.Preset3.SetPresetValue(presetDict);
                presetDump.Preset3.SetName(presetNameBox.Text);
                userPreset3.Content = presetNameBox.Text;
                break;
        }

        presetNameBox.Text = string.Empty;

        SaveSettings();

    }

    //Just packages PresetDataDump, Serializes and sends JSON, logic handled in secondSaveButton 
    public void SaveSettings()
    {
        string filePath = "Presets.json";

        PresetsDataDump presetsData = new PresetsDataDump
        {
            Preset1 = presetDump.Preset1,
            Preset2 = presetDump.Preset2,
            Preset3 = presetDump.Preset3,
        };

        string jsonString = JsonSerializer.Serialize(presetsData);

        File.WriteAllText(filePath, jsonString);
    }

    //User does not want to save changes so we must return to default menu, UI effects only
    public void DontSavePreset(object sender, RoutedEventArgs args)
    {
        SavePreset.IsVisible = true;
        ConfirmSaves.IsVisible = false;
    }

    //Easy method ot wipe highlighting of buttons, rehighlighting done method specifically
    public void DiscolourPresets()
    {
        preset1.Background = new SolidColorBrush(Colors.LightGray);
        preset2.Background = new SolidColorBrush(Colors.LightGray);
        preset3.Background = new SolidColorBrush(Colors.LightGray);
        userPreset1.Background = new SolidColorBrush(Colors.LightGray);
        userPreset2.Background = new SolidColorBrush(Colors.LightGray);
        userPreset3.Background = new SolidColorBrush(Colors.LightGray);
        preset1.Foreground = new SolidColorBrush(Colors.Black);
        preset2.Foreground = new SolidColorBrush(Colors.Black);
        preset3.Foreground = new SolidColorBrush(Colors.Black);
        userPreset1.Foreground = new SolidColorBrush(Colors.Black);
        userPreset2.Foreground = new SolidColorBrush(Colors.Black);
        userPreset3.Foreground = new SolidColorBrush(Colors.Black);
    }

    //Sets all siders equal to value of fed dictionary
    public void ApplyPresets(Dictionary<string, double> presetDict)
    {
        dimProblemSlider.Value = presetDict["dimensions"];
        gravSliderX.Value = presetDict["gravityX"];
        gravSliderZ.Value = presetDict["gravityZ"];
        gravSliderY.Value = presetDict["gravityY"];
        refDenSlider.Value = presetDict["refDensity"];
        refSoundSlider.Value = presetDict["refSoundSpeed"];
        defaultParticleSpacingSlider.Value = presetDict["defaultParticleSpacing"];
        expRatioHSlider.Value = presetDict["expRatioH"];
        HKappaSlider.Value = presetDict["HKappa"];
        startTimeSlider.Value = presetDict["startTime"];
        endTimeSlider.Value = presetDict["endTime"];
        maxTimeStepSlider.Value = presetDict["maxTimeStep"];
        timeStepCoefficientSlider.Value = presetDict["timeStepCoeff"];
        timeStepViscosityCoefficientSlider.Value = presetDict["timeStepVisCoeff"];
        timeStepSurfaceTensionCoefficientSlider.Value = presetDict["timeStepSurfTensionCoeff"];
        outputIntervalSlider.Value = presetDict["outputInterval"];
    }

    /*
     * Will create a file should the file not exist, should only ideally run on first boot or removal of presets
     * File is called "Presets.json" and is placed in same file as the .exe in file line below
     * .\comp2003-2023-22\comp2003 avalonia\comp2003 avalonia.Desktop\bin\Debug\net7.0
     * 
     * We also call FileReader if the file is found First, and return striaght out as no initialisation necessary
     * 
     */
    public void FileInitialiser()
    {
        //Change me if the Presets are moved / renamed!!
        String fileName = "Presets.json";

        if (File.Exists(fileName))
        {
            presetDump = PresetFileReader.FileReader(fileName);
            userPreset1.Content = presetDump.Preset1.GetName();
            userPreset2.Content = presetDump.Preset2.GetName();
            userPreset3.Content = presetDump.Preset3.GetName();
            return;
        }

        string relativePath = "Presets.json";
        string currentDirectory = Directory.GetCurrentDirectory();
        string fullPath = Path.Combine(currentDirectory, relativePath);

        //Sets up each instance with no name (not null name) and grabs a dictionary as a placeholder
        //Ensure this declaration does not have shallow copies in declaration to avoid bad JSON writes!
        PresetsDataDump presetsData = new PresetsDataDump
        {
            Preset1 = new PresetDataModel { PresetName = new Dictionary<string, string> { { "presetName", "No Preset" }, }, Dict = PresetValues.HardCodedPresetFetch(1), },
            Preset2 = new PresetDataModel { PresetName = new Dictionary<string, string> { { "presetName", "No Preset" }, }, Dict = PresetValues.HardCodedPresetFetch(2), },
            Preset3 = new PresetDataModel { PresetName = new Dictionary<string, string> { { "presetName", "No Preset" }, }, Dict = PresetValues.HardCodedPresetFetch(3), },
        };

        presetDump = presetsData;

        string jsonString = JsonSerializer.Serialize(presetsData);
        File.WriteAllText(fullPath, jsonString);
    }

    public class AppConfiguration
    {
        public string ParaViewPath { get; set; } // property to store ParaView path
        public string themeMode { get; set; }
    }

    // The method to load the application configuration from a JSON file
    private void LoadConfiguration()
    {
        try
        {
            string configPath = "config.json"; 
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                appConfig = JsonSerializer.Deserialize<AppConfiguration>(json);
            }
            else
            {
                appConfig = new AppConfiguration();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            appConfig = new AppConfiguration();
        }

        paraViewLocation.Text = appConfig.ParaViewPath; // Updates the UI with the loaded path
        paraLocation = appConfig.ParaViewPath; // sets the local variable paraLocation to the loaded path
        programMode = appConfig.themeMode;

        SetStartMode();
    }

    // Method to save the configuration to a JSON file
    private void SaveConfiguration()
    {
        try
        {
            string configPath = "config.json";
            string json = JsonSerializer.Serialize(appConfig);
            File.WriteAllText(configPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration: {ex.Message}");
        }
    }

    private void PreviewSim(object sender, RoutedEventArgs args)
    {
        GeneratePreviewConfig();
        StartSimulation(true);
    }
    private void GeneratePreviewConfig()
    {
       /*
        *    # fluid block_{ p1 0.0 0.0 0 p2 1.0 0.0 0 p3 0.0 2.0 0 p4 2.0 0.0 1 vel 0 0 0 vel_o 0 0 0 rho 1000 rho_o 1000 mu 0.001 gamma 0.07 }_block
        *    fluid plane_{ p1 0.0 0.0 0 p2 1.0 0.0 0 p3 0.0 2.0 0 vel 0 0 0 vel_o 0 0 0 rho 1000 rho_o 1000 mu 0.001 gamma 0.07 }_plane
        *
        *    # Solid boundary
        *    # Options: plane, line
        *    boundary line_{ p1 0 0 0 p2 0 3 0 vel 0 0 0 }_line
        *    boundary line_{ p1 0 0 0 p2 4 0 0 vel 0 0 0 }_line
        *    boundary line_{ p1 4 0 0 p2 4 3 0 vel 0 0 0 }_line
        *    # boundary plane_{ p1 4 0 2 p2 4 3 2 p3 0 0 2 vel 0 0 0 }_plane
        *    # Boundary plane_{ p1 4 0 2 p2 0 0 2 p3 4 0 0 vel 0 0 0 }_plane
        *    # Boundary plane_{ p1 4 0 0 p2 4 3 0 p3 0 0 0 vel 0 0 0 }_plane
        *    # Boundary plane_{ p1 4 0 2 p2 4 3 2 p3 4 0 0 vel 4 0 0 }_plane
        *    # Boundary plane_{ p1 0 0 2 p2 0 3 2 p3 4 0 0 vel 4 0 0 }_plane";
        */

        String configString = new ComponentInitialise().GenerateLeelaConfigString(this, "preview");

        string configFileName = "leela-db.config";
        string conFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
        using (StreamWriter writer = new StreamWriter(conFilePath))
        {
            writer.Write(configString);
        }
    }

    private bool BeginnerMode = false;

    private void ToggleModeClicked(object sender, RoutedEventArgs e)
    {
        BeginnerMode = !BeginnerMode;
        UpdateUIforMode();
    }

    // Calls our UI updates depending if we turn beginner mode on or off
    private void UpdateUIforMode()
    {
        if (BeginnerMode)
        {
            HighlightBeginnerFields();
        }
        else
        {
            ResetHighlightFields();
        }
    }

    // If beginner mode = true highlight these fields and change their labels
    private void HighlightBeginnerFields()
    {
        StartLabel.Background = Brushes.SeaGreen;
        StartLabel.Content = "Start Time # 0 to 100000 defaults to 0";
        EndLabel.Background = Brushes.SeaGreen;
        EndLabel.Content = "End Time # 0 to 100000 defaults to 3 (needs to be higher then Start Time)";
        SpacingLabel.Background = Brushes.SeaGreen;
        SpacingLabel.Content = "Default particle spacing (sets the space between the particles)";
        DimLabel.Background = Brushes.SeaGreen;
        DimLabel.Content = "Dimensionality of the problem (2d, 3d etc)";

        gravityXLabel.Background = Brushes.SeaGreen;
        gravityZLabel.Background = Brushes.SeaGreen;
        gravityYLabel.Background = Brushes.SeaGreen;

        // Sets beginner mode button to say it will switch to advanced mode
        BeginnerToggleLabel.Content = "Advanced Mode Toggle";
        
    }

    // If beginner mode = false reset these fields and change their labels back to default
    private void ResetHighlightFields()
    {
        StartLabel.Background = Brushes.Transparent;
        StartLabel.Content = "Start Time";
        EndLabel.Background = Brushes.Transparent;
        EndLabel.Content = "End Time";
        SpacingLabel.Background = Brushes.Transparent;
        SpacingLabel.Content = "Default particle spacing";
        DimLabel.Background = Brushes.Transparent;
        DimLabel.Content = "Dimensionality of the problem";

        gravityXLabel.Background = Brushes.Transparent;
        gravityZLabel.Background = Brushes.Transparent;
        gravityYLabel.Background = Brushes.Transparent;

        // Sets beginner mode button to say it will switch to beginner mode
        BeginnerToggleLabel.Content = "Beginner Mode Toggle";
    }

    public void ChangeMode(object sender, RoutedEventArgs args)
    {
        if (themeChanger.RequestedThemeVariant == ThemeVariant.Light)
        {
            themeChanger.RequestedThemeVariant = ThemeVariant.Dark;
            this.Background = Brushes.Black;
            ThemeButton.Content = "Light Mode";
            programMode = "dark";
        }
        else
        {
            themeChanger.RequestedThemeVariant = ThemeVariant.Light;
            this.Background = Brushes.White;
            ThemeButton.Content = "Dark Mode";
            programMode = "light";
        }

        appConfig.themeMode = programMode;
        SaveConfiguration();


    }

    public void SetStartMode()
    {
        if (programMode == "light")
        {
            themeChanger.RequestedThemeVariant = ThemeVariant.Light;
            this.Background = Brushes.White;
            DiscolourPresets();

        }
        if (programMode == "dark")
        {
            themeChanger.RequestedThemeVariant = ThemeVariant.Dark;
            this.Background = Brushes.Black;
            DiscolourPresets();
        }

    }
}
