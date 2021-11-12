using System;

namespace ProjectFullCIL
{
    class Program
    {
        static void Main(string[] args)
        {
            AOSClass aos = new AOSClass();

            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "||||MANTENIMIENTO FULL CIL||||\n" +
                              "||||||||||||||||||||||||||||||");

            Console.WriteLine("¿Cuál es el AOS donde está corriendo el programa?");
            string mainAOS = Console.ReadLine();

            if (aos.ValidarExistenciaAOS(mainAOS))
            {
                Console.WriteLine("¿En cuál paso se desea iniciar? \n" +
                                  "1 = Detener los AOS menos el indicado anteriormente \n" +
                                  "2 = Ejecutar el Full CIL \n" +
                                  "3 = Detener el AOS indicado anteriormente \n" +
                                  "4 = Eliminar el XppIL del AOS indicado anteriormente \n" +
                                  "5 = Eliminar los demás XppIL \n" +
                                  "6 = Levantar servicios de AOS en Cascada");

                string startIn = Console.ReadLine();

                switch (startIn)
                {
                    case "1":
                        //PASO 1
                        aos.DetenerAOSMenosMain(mainAOS);
                        //PASO 2
                        if (aos.EjecutarFullCIL() == 1)
                        {
                            //PASO 3
                            aos.DetenerAOSEspecifico(mainAOS);
                            //PASO 4
                            aos.EliminarXppILEspecifico();
                            //PASO 5
                            aos.EliminarDemasXppIL(mainAOS);
                            //PASO 6
                            aos.LevantarAOSEnCascada(mainAOS);
                        }
                        break;

                    case "2":
                        //PASO 2
                        if (aos.EjecutarFullCIL() == 1)
                        {
                            //PASO 3
                            aos.DetenerAOSEspecifico(mainAOS);
                            //PASO 4
                            aos.EliminarXppILEspecifico();
                            //PASO 5
                            aos.EliminarDemasXppIL(mainAOS);
                            //PASO 6
                            aos.LevantarAOSEnCascada(mainAOS);
                        }
                        break;

                    case "3":
                        //PASO 3
                        aos.DetenerAOSEspecifico(mainAOS);
                        //PASO 4
                        aos.EliminarXppILEspecifico();
                        //PASO 5
                        aos.EliminarDemasXppIL(mainAOS);
                        //PASO 6
                        aos.LevantarAOSEnCascada(mainAOS);
                        break;

                    case "4":
                        //PASO 4
                        aos.EliminarXppILEspecifico();
                        //PASO 5
                        aos.EliminarDemasXppIL(mainAOS);
                        //PASO 6
                        aos.LevantarAOSEnCascada(mainAOS);
                        break;

                    case "5":
                        //PASO 5
                        aos.EliminarDemasXppIL(mainAOS);
                        //PASO 6
                        aos.LevantarAOSEnCascada(mainAOS);
                        break;

                    case "6":
                        //PASO 6
                        aos.LevantarAOSEnCascada(mainAOS);
                        break;

                    default:
                        Console.WriteLine("Error al insertar el dato");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Error al insertar el dato");
            }

            Console.WriteLine("||||||||||||||||||||||||||||||\n" +
                              "|||||FIN DE MANTENIMIENTO|||||\n" +
                              "||||||||||||||||||||||||||||||");
            Console.WriteLine("\nPresiona cualquier tecla para salir");
            Console.ReadLine();
        }
    }
}
