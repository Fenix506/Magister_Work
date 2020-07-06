using Image.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinFiler
{
    class Program
    {
        static void Main(string[] args)
        {
           //var shower = new FileShow("D:\\Project_C#\\magister_v1\\FileGenerator\\bin\\Debug\\151907");
            //var shower = new FileShow("C:\\Users\\maria\\Desktop\\russiandmaddins\\releases\\1\\source.xlsx"); 
            var shower = new FileShow("D:\\Downloads\\SQL2012DevTrainingKit.Setup.exe");
            shower.Create();
            var res1 = shower.Entropy();
            var res = ImageIO.ImageToBitmap(shower.getImage());
             res.Save("fileimg");
            Console.ReadKey();
        }
    }
}
