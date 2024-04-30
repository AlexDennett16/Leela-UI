using DynamicData.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2003_avalonia.ViewModels;

public static class PresetValues
{
    //Will always return a dict, parse 1, 2 or 3 for the options though
    public static Dictionary<string, double> HardCodedPresetFetch(int presetNum)
    {
        switch (presetNum)
        {
            case 1:
                return new Dictionary<string, double>
                {
                    { "dimensions", 2 },
                    { "gravityX", 0.0 },
                    { "gravityZ", -9.81 },
                    { "gravityY", 0.0 },
                    { "refDensity", 800 },
                    { "refSoundSpeed", 320 },
                    { "defaultParticleSpacing", 0.22 },
                    { "expRatioH", 1.2 },
                    { "HKappa", 2 },
                    { "startTime", 0.0 },
                    { "endTime", 4 },
                    { "maxTimeStep", 0.001 },
                    { "timeStepCoeff", 0.35 },
                    { "timeStepVisCoeff", 0.128 },
                    { "timeStepSurfTensionCoeff", 0.1 },
                    { "outputInterval", 0.02 },

                };

            case 2:
                return new Dictionary<string, double>
                {
                    {"dimensions", 3 },
                    {"gravityX", 0.0 },
                    {"gravityZ", -9.81 },
                    {"gravityY", 0.0 },
                    {"refDensity", 1200 },
                    {"refSoundSpeed", 300 },
                    {"defaultParticleSpacing", 0.3 },
                    {"expRatioH", 1.3 },
                    {"HKappa", 2 },
                    {"startTime", 0.0 },
                    {"endTime", 2 },
                    {"maxTimeStep", 0.001 },
                    {"timeStepCoeff", 0.4 },
                    {"timeStepVisCoeff", 0.13 },
                    {"timeStepSurfTensionCoeff", 0.12 },
                    {"outputInterval", 0.04 },
                };
            //We prevent a null return, by returning this if no desireable value returned
            case 3:
            default:
                return new Dictionary<string, double>
                {
                    {"dimensions", 2 },
                    {"gravityX", 0.0 },
                    {"gravityZ", -9.81 },
                    {"gravityY", 0.0 },
                    {"refDensity", 1000 },
                    {"refSoundSpeed", 340 },
                    {"defaultParticleSpacing", 0.2 },
                    {"expRatioH", 1.25 },
                    {"HKappa", 2 },
                    {"startTime", 0.0 },
                    {"endTime", 10 },
                    {"maxTimeStep", 0.001 },
                    {"timeStepCoeff", 0.5 },
                    {"timeStepVisCoeff", 0.1 },
                    {"timeStepSurfTensionCoeff", 0.12 },
                    {"outputInterval", 0.03 },
                };
        }

    }

}

