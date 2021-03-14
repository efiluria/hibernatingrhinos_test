using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace serchlog
{
    public class Program
    {

        public static long Position { get; set; } 
        public static string FileName { get; set; }

        private static readonly Regex Format = new Regex(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$");
        static void Main(string[] args)
        {
            FileName = @"C:\efrat\test1\log\Windows.txt";
            //FileName = @"C:\efrat\test1\log\c.txt";
            Position = 0;

            
            long Length = new FileInfo(FileName).Length;
            DateTime StartDate = DateTime.Parse("2016-09-21"); 
            DateTime EndDate = DateTime.Parse("2016-09-25");

            //Get the first line    
            string StartLine= (SearchLineNumberOnStartDate(StartDate, Length)).ToString();
            Console.WriteLine(StartLine);

            //Retrive lines between dates
            PrintFromStartToEnd(EndDate);

        }

        public static object SearchLineNumberOnStartDate(DateTime DateInStartLine, long FileLength)
        {
            long min = 0;
            long max = FileLength;
            long mid = 0;
            DateTime DateInMidLine = new DateTime();
            DateTime DateBeforeMidLine = new DateTime();
            string Line = "";

            while (min <= max)
            {
                mid = (min + max) / 2;
                DateInMidLine = GetDateOfNextLine(mid);
                DateBeforeMidLine = GetDateOfPrevLine(mid);

                if (DateInStartLine == DateInMidLine)
                {
                    Line =  LineAfterPosition(mid);
                    max = mid - 1;
                }
                else if (DateInStartLine < DateInMidLine)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            if(Line !=null)
            {
                Position = mid;
                return Line;
            }
            return -1;
        }
        
        public static string GetPartLine(long position)
        {
            string line;
            using (Stream stream = File.Open(FileName, FileMode.Open))
            {
                stream.Seek(position, SeekOrigin.Current);
                using (StreamReader reader = new StreamReader(stream))
                {
                    line = reader.ReadLine();
                }
            }
            return line;
        }

        public static DateTime GetDateOfNextLine(long mid)
        {
            string line = LineAfterPosition(mid);
            if(String.IsNullOrEmpty(line))
            {
                return new DateTime();
            }
            if(line.Length<10||!Format.IsMatch(line.Substring(0, 10)))
            {
                return new DateTime();
            }

            string DateS = line.Substring(0, 10);
            DateTime Date = DateTime.Parse(DateS);
            return Date;
        }

        public static DateTime GetDateOfPrevLine(long mid)
        {
            string line = LineBeforePosition(mid);
            string DateS = line.Substring(0, 10);
            DateTime Date = DateTime.Parse(DateS);
            return Date;
        }
        public static string LineBeforePosition(long position)
        {
            // PartLine gets part of the line, from a specific location
            string PartLine = GetPartLine(position);
            string DateS = "";
            if (PartLine.Length >= 10)
            {
                DateS = PartLine.Substring(0, 10);
            }
            //DateTime result;

            // while PartLine is at the middle of the line
            while (!Format.IsMatch(DateS))
            {
                position --;
                PartLine = GetPartLine(position);
                if (PartLine.Length >= 10)
                {
                    DateS = PartLine.Substring(0, 10);
                }
            }
            return PartLine;
        }

        public static string LineAfterPosition(long position) 
        {
            string line = GetPartLine(position);
            long len = line.Length;
            Position = position + len + 2;
            return GetPartLine(Position);
        }

        public static void PrintFromStartToEnd(DateTime EndDate)
        {
            DateTime dateTime = GetDateOfNextLine(Position);
            string line;
            //int count = 0;
            while ((dateTime <= EndDate || dateTime == null)/*&&count<1000*/)
            {
                Position += 2;
                dateTime = GetDateOfNextLine(Position);
                line = GetPartLine(Position);
                Console.WriteLine(line);
                //count++;
            }
        }
    }
}
