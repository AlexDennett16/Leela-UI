using comp2003_avalonia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace comp2003_avalonia
{
    internal class ComponentInitialise
    {
        public void InitialiseBaseValues(MainView mainViewObject)
        {
            //Here specify the starting default values
            mainViewObject.dimProblem.Text = (3).ToString();
            mainViewObject.gravityXTextBox.Text = (0.0).ToString();
            mainViewObject.gravityZTextBox.Text = (-9.81).ToString();
            mainViewObject.gravityYTextBox.Text = (0.0).ToString();
            mainViewObject.refDensity.Text = (1000).ToString();
            mainViewObject.refSoundSpeed.Text = (350).ToString();
            mainViewObject.defaultParticleSpacing.Text = (0.2).ToString();
            mainViewObject.expRatioH.Text = (1.2).ToString();
            mainViewObject.HKappaTextBox.Text = (2).ToString();
            mainViewObject.startTime.Text = (0).ToString();
            mainViewObject.endTime.Text = (3.0).ToString();
            mainViewObject.maxTimeStep.Text = (1.0e-3).ToString();
            mainViewObject.timeStepCoefficient.Text = (0.4).ToString();
            mainViewObject.timeStepViscosityCoefficient.Text = (0.125).ToString();
            mainViewObject.timeStepSurfaceTensionCoefficient.Text = (0.1).ToString();
            mainViewObject.outputInterval.Text = (0.02).ToString();
            mainViewObject.kernelType.SelectedIndex = 0;

        }

        public String GetLeelaLocation()
        {
            return @"compiled leela\leela test.exe";
        }

        public String GetLinuxLeelaLocation()
        {
            return @"compiled leela/bin/leela";
        }

        public String GenerateLeelaConfigString(MainView mainViewObject, String useCase)
        {
            String tempDimProblem = (mainViewObject.dimProblem.Text).Replace(",", ".");
            //String tempGrav = (mainViewObject.grav.Text).Replace(",", ".");
            String gravityX = (mainViewObject.gravityXTextBox.Text).Replace(",", ".");
            String gravityZ = (mainViewObject.gravityZTextBox.Text).Replace(",", ".");
            String gravityY = (mainViewObject.gravityYTextBox.Text).Replace(",", ".");
            

            String tempRefDensity = (mainViewObject.refDensity.Text).Replace(",", ".");
            String tempRefSoundSpeed = (mainViewObject.refSoundSpeed.Text).Replace(",", ".");
            String tempDefaultParticleSpacing = (mainViewObject.defaultParticleSpacing.Text).Replace(",", ".");

            String tempExpRatioH = (mainViewObject.expRatioH.Text).Replace(",", ".");
            String tempHKappa = (mainViewObject.HKappaTextBox.Text).Replace(",", ".");
            
            String tempKernelType = (mainViewObject.kernelType.SelectedItem as ComboBoxItem).Content.ToString();

            String tempStartTime = (mainViewObject.startTime.Text).Replace(",", ".");
            String tempEndTime = (mainViewObject.endTime.Text).Replace(",", ".");
            String tempMaxTimeStep = (mainViewObject.maxTimeStep.Text).Replace(",", ".");
            String tempTimeStepCoefficient = (mainViewObject.timeStepCoefficient.Text).Replace(",", ".");
            String tempTimeStepViscosityCoefficient = (mainViewObject.timeStepViscosityCoefficient.Text).Replace(",", ".");
            String tempTimeStepSurfaceTensionCoefficient = (mainViewObject.timeStepSurfaceTensionCoefficient.Text).Replace(",", ".");
            String tempOutputInterval = (mainViewObject.outputInterval.Text).Replace(",", ".");

            String tempStructureBox = mainViewObject.structureBox.Text;

            if (useCase == "preview")
            {
                tempStartTime = "0";
                tempEndTime = "0";
            }



            String generatedString = @"
# Constants
constant.dim " + tempDimProblem + @" 
constant.g " + gravityX + @" " + gravityZ + @" " + gravityY + @"
constant.rho_0 " + tempRefDensity + @"                        # reference density, water @ 20deg = 998
constant.c_0 " + tempRefSoundSpeed + @"                            # reference sound speed
constant.delta_s " + tempDefaultParticleSpacing + @"                         # default particle spacing for particle distribution

# SPH kernel
constant.h_eta " + tempExpRatioH + @"                       # expansion ratio for h, typically 1.2-1.3
constant.h_kappa " + tempHKappa + @"                         # support domain size = h_kappa * h = kernel size, for eg cubic kernel h_kappa = 2
kernel.type " + tempKernelType + @"                   # allows for selection of kernel

# Time
time.t_begin " + tempStartTime + @"                         # default start time
time.t_end " + tempEndTime + @"                         # default end time
time.dt_max " + tempMaxTimeStep + @"                       # maximum time step
time.dt_c_cfl " + tempTimeStepCoefficient + @"                       # time step cfl coefficient, Monaghan (1992) recommends 0.4
time.dt_c_viscosity " + tempTimeStepViscosityCoefficient + @"                      # time step viscosity coefficient, Morris (1997) recommends 0.125
time.dt_c_surfacetension " + tempTimeStepSurfaceTensionCoefficient + @"                     # time step surface tension coefficient, Adami et al (2010) recommends 0.1
time.dt_output " + tempOutputInterval + @"                     # output interval

# IO
io.core_base data                  # datafile basename
io.core_ext .core                  # datafile extension
io.vtk_base_vtu data                   # vtk vtu basename
io.vtk_ext_vtu .vtu                    # vtk vtu extension
io.vtk_base_pvd data                   # vtk pvd basename
io.vtk_ext_pvd .pvd                    # vtk pvd extension
io.precision 9                      # output string precision



" + tempStructureBox;




            return generatedString;
        }









    }
}
