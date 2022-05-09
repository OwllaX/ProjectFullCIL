using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;

namespace AutoFullCIL
{
    class AutoFullCIL
    {
        //PATH XPPIL @"C:\Program Files\Microsoft Dynamics AX\60\Server\GIAX2012_PROD\bin"

        private readonly string name = "AOS60$01";

        private readonly string[] strArray = new string[10]
        {
            /*"TEST-ARCHIVE"*/
            "SJOAOS01",
            "SJOAOS02",
            /*"SJOAOS03",*/
            "SJOAOS04",
            "SJOAOS05",
            "SJOAOS06",
            "SJOAOS07",
            "SJOAOS08",
            "SJOAOS09",
            "SJOAOS10",
            "SJORetailHQ001"
        };

        private string appWorkPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Paso 1 Bajar todos los AOS menos el MAIN
        /// </summary>
        /// <param name="mainAOS"></param>
        public void StopAllAOSExceptOne(string mainAOS)
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 1||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";

            Console.WriteLine(msg);

            Console.WriteLine("Se inicia con detener los otros AOS");
            msg = msg + "\r\n" + "Se inicia con detener los otros AOS";

            for (int index = 0; index < strArray.Length; ++index)
            {
                if (!mainAOS.Equals(strArray[index]))
                {
                    try
                    {
                        ServiceController serviceController = new ServiceController(name, strArray[index]);

                        if (serviceController.Status == ServiceControllerStatus.Running)
                        {
                            serviceController.Stop();
                            Console.WriteLine(strArray[index] + " - " + serviceController.Status.ToString());
                            msg = msg + "\r\n" + serviceController.Status.ToString();
                            serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        }
                        Console.WriteLine(strArray[index] + " - " + serviceController.Status.ToString());
                        msg = msg + "\r\n" + serviceController.Status.ToString();
                    }
                    catch
                    {
                        Console.WriteLine("No se pudo bajar el servicio " + strArray[index]);
                        msg = msg + "\r\n" + "No se pudo bajar el servicio " + strArray[index];
                        AppendTextToLOG(msg);
                        throw;
                    }
                }
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Paso 2 Ejecutar el Full CIL sobre la máquina en la que se trabaja
        /// </summary>
        public void ExecuteFullCIL()
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 2||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";

            Console.WriteLine(msg);

            try
            {
                Console.WriteLine("Iniciando Full CIL ... Esto podría tardar unos minutos");
                msg = msg + "\r\n" + "Iniciando Full CIL ... Esto podría tardar unos minutos";
                ExecuteCommand(@"C:\Program Files (x86)\Microsoft Dynamics AX\60\Client\Bin\", "ax32.exe -startupcmd=CompileIL");

                Console.WriteLine("Se ha completado la operación");
                msg = msg + "\r\n" + "Se ha completado la operación";
            }
            catch (Exception)
            {
                Console.WriteLine("Se ha producido una error al realizar el Full CIL");
                msg = msg + "\r\n" + "Se ha producido una error al realizar el Full CIL";
                AppendTextToLOG(msg);
                throw;
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Paso 3 Se detiene que el servidor donde se ejecute el Full CIL
        /// </summary>
        /// <param name="AOS"></param>
        public void StopMainAOS(string mainAOS)
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 3||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";


            Console.WriteLine(msg);

            Console.WriteLine("Se está deteniendo el servicio del servidor " + mainAOS);
            msg = msg + "\r\n" + "Se está deteniendo el servicio del servidor " + mainAOS;

            try
            {
                ServiceController serviceController = new ServiceController(name, mainAOS);
                serviceController.Stop();
                Console.WriteLine(mainAOS + " - " + serviceController.Status.ToString());
                msg = msg + "\r\n" + mainAOS + " - " + serviceController.Status.ToString();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                Console.WriteLine(mainAOS + " - " + serviceController.Status.ToString());
                msg = msg + "\r\n" + mainAOS + " - " + serviceController.Status.ToString();
            }
            catch
            {
                Console.WriteLine("Ha ocurrido un error al detener el servicio de " + mainAOS);
                msg = msg + "\r\n" + "Ha ocurrido un error al detener el servicio de " + mainAOS;
                AppendTextToLOG(msg);
                throw;
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Paso 4 Se elimina el XppIL que del servidor donde se ejecutó el Full CIL
        /// </summary>
        public void DeleteXppILFolder()
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 4||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";

            Console.WriteLine(msg);

            try
            {
                ExecuteCommand(@"C:\Program Files\Microsoft Dynamics AX\60\Server\GIAX2012_PROD\bin", "rmdir /s /q \"C:\\Program Files\\Microsoft Dynamics AX\\60\\Server\\GIAX2012_PROD\\bin\\XppIL\"");
                Console.WriteLine("Se ha eliminado la carpeta XppIL del server donde corrió el FULL CIL");
                msg = msg + "\r\n" + "Se ha eliminado la carpeta XppIL del server donde corrió el FULL CIL";
            }
            catch (Exception)
            {
                Console.WriteLine("No se ha eliminado la carpeta XppIL del server donde corrió el FULL CIL");
                msg = msg + "\r\n" + "No se ha eliminado la carpeta XppIL del server donde corrió el FULL CIL";
                AppendTextToLOG(msg);
                throw;
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Paso 5 Se eliminaran todos los demás XppIL de los servers
        /// </summary>
        public void DeleteAllXppILFolders(string mainAOS)
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 5||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";

            Console.WriteLine(msg);

            Console.WriteLine("Se está iniciando con eliminar los demás XppIL");
            msg = msg + "\r\n" + "Se está iniciando con eliminar los demás XppIL";

            for (int index = 0; index < strArray.Length; index++)
            {
                if (!mainAOS.Equals(strArray[index]))
                {
                    try
                    {
                        Directory.Delete(@"\\" + strArray[index] + @"\bin\XppIL", true);
                        Console.WriteLine("Se ha eliminado la carpeta XppIL de " + strArray[index]);
                        msg = msg + "\r\n" + "Se ha eliminado la carpeta XppIL de " + strArray[index];
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No se pudo eliminar la carpeta XppIL");
                        msg = msg + "\r\n" + "No se ha eliminado la carpeta XppIL de " + strArray[index];
                        AppendTextToLOG(msg);
                        throw;
                    }
                }
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Paso 6 Este método eliminará los registros de la tabla SYSClientSessions
        /// </summary>
        public void DeleteSYSClientSessions()
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 6||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";

            Console.WriteLine(msg);

            try
            {
                SqlConnection sqlConn = new SqlConnection("Data Source=GIDB00;Initial Catalog=GIAX2012_PROD;user=giprod;password=sa");
                sqlConn.Open();

                string sql = "BEGIN TRAN DELETE FROM SYSCLIENTSESSIONS COMMIT";
                SqlCommand sqlComm = new SqlCommand(sql, sqlConn);

                int rowsAffected = sqlComm.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Se eliminado " + rowsAffected.ToString() + " registros en la tabla SYSClientSessions en GIAX2012_PROD");
                    msg = msg + "\r\n" + "Se eliminado " + rowsAffected.ToString() + " registros en la tabla SYSClientSessions en GIAX2012_PROD";
                }
                else
                {
                    Console.WriteLine("No se han eliminado datos en la tabla SYSClientSessions en GIAX2012_PROD");
                    msg = msg + "\r\n" + "No se han eliminado datos en la tabla SYSClientSessions en GIAX2012_PROD";
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Ocurrió un error al eliminar los datos SYSClientSessions");
                msg = msg + "\r\n" + "Ocurrió un error al eliminar los datos SYSClientSessions";
                AppendTextToLOG(msg);
                throw;
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Paso 7 Levanta todos y cada uno de los servidores
        /// </summary>
        public void CascadeStartupAOS(string mainAOS)
        {
            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||||||||||PASO 7||||||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";

            Console.WriteLine(msg);

            Console.WriteLine("Se levantarán en cascada los servicios de AOS");
            msg = msg + "\r\n" + "Se levantarán en cascada los servicios de AOS";

            try
            {
                ServiceController serviceController = new ServiceController(name, mainAOS);
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    serviceController.Start();
                    Console.WriteLine(mainAOS + " - " + serviceController.Status.ToString());
                    msg = msg + "\r\n" + mainAOS + " - " + serviceController.Status.ToString();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                }
                Console.WriteLine(mainAOS + " - " + serviceController.Status.ToString());
                msg = msg + "\r\n" + mainAOS + " - " + serviceController.Status.ToString();

                for (int index = 0; index < strArray.Length; ++index)
                {
                    if (!mainAOS.Equals(strArray[index]))
                    {
                        try
                        {
                            ServiceController serviceController1 = new ServiceController(name, strArray[index]);
                            if (serviceController1.Status == ServiceControllerStatus.Stopped)
                            {
                                serviceController1.Start();
                                Console.WriteLine(strArray[index] + " - " + serviceController1.Status.ToString());
                                msg = msg + "\r\n" + mainAOS + " - " + serviceController1.Status.ToString();
                                serviceController1.WaitForStatus(ServiceControllerStatus.Running);
                                Console.WriteLine(strArray[index] + " - " + serviceController1.Status.ToString());
                                msg = msg + "\r\n" + mainAOS + " - " + serviceController1.Status.ToString();
                            }
                            else
                            {
                                Console.WriteLine(strArray[index] + " - " + serviceController1.Status.ToString());
                                msg = msg + "\r\n" + mainAOS + " - " + serviceController1.Status.ToString();
                            }
                        }
                        catch
                        {
                            Console.WriteLine(strArray[index] + " no se pudo levantar");
                            msg = msg + "\r\n" + strArray[index] + " no se pudo levantar";
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine(mainAOS + " no se pudo levantar");
                msg = msg + "\r\n" + mainAOS + " no se pudo levantar";
            }

            AppendTextToLOG(msg);
        }

        /// <summary>
        /// Método que consigue realizar un comando por CMD
        /// </summary>
        /// <param name="path">Ruta donde se quiere que se ejecute el comando</param>
        /// <param name="command">Comando que uno quiere que se ejecute</param>
        public void ExecuteCommand(string path, object command)
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
        public bool ValidateMainAOS(string mainAOS)
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

        /// <summary>
        /// Este método almacenará el LOG de Eventos de la Aplicación
        /// </summary>
        /// <param name="message"></param>
        public void AppendTextToLOG(string message)
        {
            string filePath = this.appWorkPath + @"\AutoFullCIL.log";

            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine("Documento con el registro de todos los eventos ocurridos con el app de AutoFullCIL");
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }

            using (StreamWriter sw = File.AppendText(filePath)) 
            {
                sw.WriteLine(" ");
                sw.WriteLine("Event:");
                sw.WriteLine(message);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}
