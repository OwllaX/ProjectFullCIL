using System;

namespace AutoFullCIL
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoFullCIL cIL = new AutoFullCIL();

            string msg = "||||||||||||||||||||||||||||||\r\n" +
                         "||||MANTENIMIENTO FULL CIL||||\r\n" +
                         "||||||||by Axel Vargas||||||||\r\n" +
                         "||||||||||||||||||||||||||||||";
            Console.WriteLine(msg);
            
            string mainAOS = Environment.MachineName.ToString();

            Console.WriteLine("El Mantenimiento correrá en " + mainAOS);
            msg = msg + "\r\n" + "El Mantenimiento correrá en " + mainAOS;
            cIL.AppendTextToLOG(msg);

            if (cIL.ValidateMainAOS(mainAOS))
            {
                cIL.StopAllAOSExceptOne(mainAOS);
                cIL.ExecuteFullCIL();
                cIL.StopMainAOS(mainAOS);
                cIL.DeleteXppILFolder();
                cIL.DeleteAllXppILFolders(mainAOS);
                cIL.DeleteSYSClientSessions();
                cIL.CascadeStartupAOS(mainAOS);
            } else
            {
                Console.WriteLine("El servidor donde se ejecuta el programa, no pertenece a ninguno de PROD");
                cIL.AppendTextToLOG("El servidor donde se ejecuta el programa, no pertenece a ninguno de PROD");
            }
            

            Console.WriteLine("||||||||||||||||||||||||||||||\r\n" +
                              "|||||FIN DE MANTENIMIENTO|||||\r\n" +
                              "||||||||||||||||||||||||||||||");
            cIL.AppendTextToLOG("||||||||||||||||||||||||||||||\r\n" +
                                "|||||FIN DE MANTENIMIENTO|||||\r\n" +
                                "||||||||||||||||||||||||||||||");

            Console.WriteLine("\nPresiona tecla ENTER para salir");
            Console.ReadKey();
        }
    }
}
