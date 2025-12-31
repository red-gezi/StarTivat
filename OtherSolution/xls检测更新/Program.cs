using Spire.Xls.Core;
using System;
using System.IO;
using System.Threading.Tasks;
namespace xls检测更新
{
    partial class Program
    {
        static string direPath => Directory.GetCurrentDirectory().Replace(@"\OtherSolution\xls检测更新\bin\Debug\net6.0", "") + @"\Assets\GameResources\GameData\";

        static void Main(string[] args)
        {
            BuffSystem.Init(direPath, "buff.xlsx");
            OccurrenceSystem.Init(direPath, "Occurrence.xlsx");
            //自动执行
            Task.Run(async () =>
            {
                while (true)
                {
                    BuffSystem.Check();
                    OccurrenceSystem.Check();
                    await Task.Delay(100);
                }
            });
            //手动执行
            while (true)
            {
                Console.WriteLine("可回车进行手动更新");
                Console.ReadLine();
                BuffSystem.UpdateJson();
                OccurrenceSystem.UpdateJson();
            }
        }
    }
}