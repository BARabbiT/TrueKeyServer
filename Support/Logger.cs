using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Sentry;

namespace TrueKeyServer.Support
{
    /// <summary>
    /// Класс отвечающий за запись логов.
    /// Методы:
    /// Log - функция, записывающая лог,
    /// LogArch - функция, которая архивирует все старые папки логов.
    /// </summary>
    public class Logger
    {
        private static readonly char separator = Path.DirectorySeparatorChar;
        private static string folder; //папка для хранения логов
        private static string filename; //название файла лога
        private static long maxlogsize; //максимальный размер файла лога (файлы бОльшего размера будут заархивированы)
        private static string file;

        public Logger(string filename) : this(Path.Combine(Directory.GetCurrentDirectory() + separator + "Logs"), filename, 10000000) { }
        /// <summary>
        /// Конструктор класса Logger с обязательными параметрами:
        /// folder  - папка для хранения логов,
        /// filename - название файла лога;
        /// и необязательным:
        /// maxlogsize  - максимальный объем файла лога, после которого он будет архивирован, по умолчанию 10 мбайт.
        /// </summary>
        public Logger(string folder, string filename, long maxlogsize)
        {
            Logger.folder = folder;
            Logger.filename = filename;
            Logger.maxlogsize = maxlogsize;
        }

        /// <summary>
        /// Функция записывающая лог (
        /// func  - название функции записывающей строку лога,
        /// str - строка лога, указать "START" для инициализации стартового сообщения лога,
        /// type - тип лога, по умолчанию "Info", для типа "ERROR" лог дублируется в отдельный файл,
        /// сaret  - добавляет указанное количество переносов после записи основного значения.)
        /// </summary>
        public void Log(string func, string str, string type = "INFO", int caret = 1)
        {
            try
            {
                file = folder + separator + filename;
                ChekLogSize(folder, filename);
                Directory.CreateDirectory(folder); //Создает папку для логов
                if (func != null && str != null)
                {
                    StreamWriter sw = new StreamWriter(File.Open(file, FileMode.Append));  //Создает файл для записи
                    if (str == "START")
                    {
                        sw.WriteLine(" ");
                        sw.WriteLine(" ");
                        sw.WriteLine("************************SERVICE STARTED************************");
                        sw.WriteLine("*************************(TEST LAUNCH)*************************");
                    }
                    else
                    {
                        sw.WriteLine("{0} - [{2}][{4}] {1} : {3}", DateTime.Now.ToString(), func, type, str, Thread.CurrentThread.ManagedThreadId);
                        if (caret > 1)   //Формирует переносы, в случае, если значение переменной больше 1. 
                        {
                            while (caret != 0)
                            {
                                sw.WriteLine("");
                                caret--;
                            }
                        }
                    }
                    sw.Close();
                }
                if (type == "ERROR") //Создает отдельный файл для записи ошибок и записывает в него лог.
                {
                    file = folder + separator + "ERROR.txt";
                    ChekLogSize(folder, "ERROR.txt");
                    StreamWriter sw = new StreamWriter(File.Open(file, FileMode.Append));  //Создает файл для записи
                    sw.WriteLine("{0} - [{2}][{4}] {1} : {3}", DateTime.Now.ToString(), func, type, str, Thread.CurrentThread.ManagedThreadId);
                    if (caret > 1)   //Формирует переносы, в случае, если значение переменной больше 1. 
                    {
                        while (caret != 0)
                        {
                            sw.WriteLine("");
                            caret--;
                        }
                    }
                    sw.Close();
                    using (SentrySdk.Init("https://97c2f05764f84757b0542365bab7b731@o517649.ingest.sentry.io/5626080")) SentrySdk.CaptureMessage(str);
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// Функция, которая архивирует все старые папки логов.
        /// Возвращает описание ошибки при архивировании или "noner" если ошибок нет.
        /// </summary>
        public string LogArch()
        {
            foreach (var dir in Directory.GetDirectories(folder))
            {
                if (dir + "." != folder + separator + "Log arch on " + DateTime.Now.ToString("D"))
                {
                    try
                    {
                        ZipFile.CreateFromDirectory(dir, dir + ".zip");
                        Directory.Delete(dir, true);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
            return "noner";
        }
        /// <summary>
        /// Сулжебная функция, архивирующая файл логов, в случае, если он превышает указанный размер.
        /// </summary>
        private void ChekLogSize(string path, string filename)
        {
            if (File.Exists(path + separator + filename))
            {
                long si = new FileInfo(path + separator + filename).Length;
                if (si > maxlogsize)
                {
                    string archfolder = folder + separator + "Log arch on " + DateTime.Now.ToString("D");
                    if (!Directory.Exists(archfolder)) Directory.CreateDirectory(archfolder);
                    File.Move(path + separator + filename, archfolder + separator + filename + " " + DateTime.Now.ToString("HH.mm.ss"));
                }
            }
        }
    }
}
