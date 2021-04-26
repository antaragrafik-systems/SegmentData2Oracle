using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SegmentData2Oracle
{
    class Program
    {
        static void Main(string[] args)
        {
            Process proc = null;
            string filename = "output.txt", bat_filename = "bat_status.txt";

            try
            {
                File.WriteAllText(filename, "processing");

                DeleteFile(bat_filename);                

                proc = new Process();
                proc.StartInfo.FileName = "esri2oracle.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();

                //Wait for 5 seconds
                Thread.Sleep(5000);

                if(File.Exists(bat_filename))
                {
                    bool complete = ReadFile(bat_filename);

                    if (complete)
                    {
                        DeleteFile(bat_filename);

                        proc.Refresh();
                        proc.StartInfo.FileName = "esri2oracle_text.bat";
                        proc.StartInfo.CreateNoWindow = false;
                        proc.Start();

                        //Wait for 5 seconds
                        Thread.Sleep(5000);                        

                        if (File.Exists(bat_filename))
                        {
                            complete = false;
                            complete = ReadFile(bat_filename);

                            if (complete)
                            {
                                proc.Refresh();
                            }
                        }
                        else
                        {
                            Console.WriteLine("esri converter for text is not running");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("esri converter is not running");
                }

                Thread.Sleep(3000);

                File.WriteAllText(filename, "done");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        static void DeleteFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        static bool ReadFile(string filename)
        {
            bool running = true;

            do
            {
                string status = File.ReadAllText(filename);

                if (status.Contains("done"))
                {
                    running = false;
                }

                Thread.Sleep(3000);
            } while (running);

            return true;
        }
    }
}

//http://net-informations.com/q/faq/batfile.html
//https://stackoverflow.com/questions/22444422/read-updated-text-file-every-x-seconds
//https://docs.microsoft.com/en-us/dotnet/api/system.console.setcursorposition?view=net-5.0