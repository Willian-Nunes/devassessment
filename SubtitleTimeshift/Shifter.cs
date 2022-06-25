using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleTimeshift
{
    public class Shifter
    {
        async static public Task Shift(Stream input, Stream output, TimeSpan timeSpan, Encoding encoding, int bufferSize = 1024, bool leaveOpen = false)
        {
            Console.WriteLine("OLA");
            using (var streamReader = new StreamReader(input, encoding, false, bufferSize, leaveOpen))
            using (var streamWriter = new StreamWriter(output, encoding, bufferSize, leaveOpen))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = await streamReader.ReadLineAsync();
                    string outLine = line;

                    const string separator = "-->";
                    if (line.Contains(separator))
                    {
                        string[] TimeStamps = line.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var start = TimeStamps[0];
                        var end = TimeStamps[1];

                        start = shiftByTimeSpan(start, timeSpan);
                        end = shiftByTimeSpan(end, timeSpan);

                        outLine = $"{start} {separator} {end}";
                    }
                    await streamWriter.WriteLineAsync(outLine);
                }
            }
        }

        private static string shiftByTimeSpan(string timeStamp, TimeSpan timeSpan)
        {
            char[] separators = { ':', ','};
            var aux = timeStamp.Split(separators);

            var nums = new int[4];
            for (int i = 0; i < 4; i++)
            {
                nums[i] = int.Parse(aux[i]);
            }

            var result = new TimeSpan(0,nums[0], nums[1], nums[2], nums[3])
                                    .Add(timeSpan);
            
            return result.ToString(@"hh\:mm\:ss\.fff");
        }
    }
}
