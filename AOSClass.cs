using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;

namespace ProjectFullCIL
{
    class AOSClass
    {
        //PATH XPPIL @"C:\Program Files\Microsoft Dynamics AX\60\Server\GIAX2012_PROD\bin"

        private readonly string[] strArray = new string[11]
        {
            /*"TEST-ARCHIVE"*/
            "SJOAOS01",
            "SJOAOS02",
            "SJOAOS03",
            "SJOAOS04",
            "SJOAOS05",
            "SJOAOS06",
            "SJOAOS07",
            "SJOAOS08",
            "SJOAOS09",
            "SJOAOS10",
            "SJORetailHQ001"
        };

        /// <summary>
        /// Paso 1 Bajar todos los AOS menos el MAIN
        /// </summary>
        /// <param name="mainAOS"></param>
        public void DetenerAOSMenosMain(string mainAOS)
        {
            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||||||||||PASO 1||||||||||||\n" +
                              "||||||||||||||||||||||||||||||");

            string name = "AOS60$01";

            for (int index = 0; index < strArray.Length; ++index)
            {
                if (!mainAOS.Equals(strArray[index]))
                {
                    try
                    {
                        ServiceController serviceController = new ServiceController(name, strArray[index]);
                        serviceController.Stop();
                        Console.WriteLine(strArray[index] + " - " + serviceController.Status.ToString());
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        Console.WriteLine(strArray[index] + " - " + serviceController.Status.ToString());
                    }
                    catch
                    {
                        Console.WriteLine("No se pudo bajar el servicio " + strArray[index]);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Paso 2 Ejecutar el Full CIL sobre la máquina en la que se trabaja
        /// </summary>
        public int EjecutarFullCIL()
        {
            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||||||||||PASO 2||||||||||||\n" +
                              "||||||||||||||||||||||||||||||");

            try
            {
                Console.WriteLine("Iniciando Full CIL ... Esto podría tardar unos minutos");
                EjecutarComando(@"C:\Program Files (x86)\Microsoft Dynamics AX\60\Client\Bin\", "ax32.exe -startupcmd=CompileIL");
                Console.WriteLine("Se ha terminado el Full CIL \n" +
                                  "¿Desea continuar? \n" +
                                  "1 = Sí\n" +
                                  "0 = No");

                string NoYes = Console.ReadLine();

                if (NoYes.Equals("1"))
                {
                    Console.WriteLine("Se continua con el proceso ...");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Se ha cancelado toda la operación");
                    return 0;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Se ha producido una error al realizar el Full CIL");
                throw;
            }
        }

        /// <summary>
        /// Paso 3 Se detiene que el servidor donde se ejecute el Full CIL
        /// </summary>
        /// <param name="AOS"></param>
        public void DetenerAOSEspecifico(string mainAOS)
        {
            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||||||||||PASO 3||||||||||||\n" +
                              "||||||||||||||||||||||||||||||");

            string name = "AOS60$01";

            Console.WriteLine("Se está deteniendo el servicio del servidor " + mainAOS);

            try
            {
                ServiceController serviceController = new ServiceController(name, mainAOS);
                serviceController.Stop();
                Console.WriteLine(mainAOS + " - " + serviceController.Status.ToString());
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                Console.WriteLine(mainAOS + " - " + serviceController.Status.ToString());
            }
            catch
            {
                Console.WriteLine("Ha ocurrido un error al detener el servicio de " + mainAOS);
                throw;
            }
        }

        /// <summary>
        /// Paso 4 Se elimina el XppIL que del servidor donde se ejecutó el Full CIL
        /// </summary>
        public void EliminarXppILEspecifico()
        {
            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||||||||||PASO 4||||||||||||\n" +
                              "||||||||||||||||||||||||||||||");

            try
            {
                EjecutarComando(@"C:\Program Files\Microsoft Dynamics AX\60\Server\TEST-ARCHIVE\bin", "rmdir /s /q \"C:\\Program Files\\Microsoft Dynamics AX\\60\\Server\\TEST-ARCHIVE\\bin\\XppIL\"");
                Console.WriteLine("Se ha eliminado la carpeta XppIL del server donde corrió el FULL CIL");
            }
            catch (Exception)
            {
                Console.WriteLine("No se ha eliminado la carpeta XppIL del server donde corrió el FULL CIL");
                throw;
            }

        }

        /// <summary>
        /// Paso 5 Se eliminaran todos los demás XppIL de los servers
        /// </summary>
        public void EliminarDemasXppIL(string mainAOS)
        {
            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||||||||||PASO 5||||||||||||\n" +
                              "||||||||||||||||||||||||||||||");

            for (int index = 1; index < strArray.Length; ++index)
            {
                if (!mainAOS.Equals(strArray[index]))
                {
                    try
                    {
                        Directory.Delete(@"\\" + strArray[index] + @"\bin\XppIL", true);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No se puedo eliminar la carpeta XppIL");
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Paso 6 Levanta todos y cada uno de los servidores
        /// </summary>
        public void LevantarAOSEnCascada()
        {
            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||||||||||PASO 6||||||||||||\n" +
                              "||||||||||||||||||||||||||||||");

            string name = "AOS60$01";

            Console.WriteLine("Se levantarán en cascada los servicios de AOS");

            for (int index = 0; index < strArray.Length; ++index)
            {
                try
                {
                    ServiceController serviceController = new ServiceController(name, strArray[index]);
                    serviceController.Start();
                    Console.WriteLine(strArray[index] + " - " + serviceController.Status.ToString());
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                    Console.WriteLine(strArray[index] + " - " + serviceController.Status.ToString());
                }
                catch
                {
                    Console.WriteLine(strArray[index] + " no se pudo levantar");
                }
            }
        }

        /// <summary>
        /// Método que consigue realizar un comando por CMD
        /// </summary>
        /// <param name="path">Ruta donde se quiere que se ejecute el comando</param>
        /// <param name="command">Comando que uno quiere que se ejecute</param>
        public void EjecutarComando(string path, object command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("cmd.exe", ("/c " + command))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = path
                };

                StringBuilder sb = new StringBuilder();
                Process p = Process.Start(processInfo);
                p.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
                p.BeginOutputReadLine();
                p.WaitForExit();
                if (!sb.ToString().Equals(""))
                {
                    Console.WriteLine(sb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Valida que el AOS consultado esté dentro de la lista
        /// </summary>
        /// <param name="mainAOS">AOS específicado por el usuario</param>
        /// <returns></returns>
        public bool ValidarExistenciaAOS(string mainAOS)
        {
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Equals(mainAOS))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
