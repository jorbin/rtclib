/*
 * 
 *  RRRRRRRRRRRRRRRRR   TTTTTTTTTTTTTTTTTTTTTTT       CCCCCCCCCCCCCLLLLLLLLLLL             IIIIIIIIIIBBBBBBBBBBBBBBBBB   
 *  R::::::::::::::::R  T:::::::::::::::::::::T    CCC::::::::::::CL:::::::::L             I::::::::IB::::::::::::::::B  
 *  R::::::RRRRRR:::::R T:::::::::::::::::::::T  CC:::::::::::::::CL:::::::::L             I::::::::IB::::::BBBBBB:::::B 
 *  RR:::::R     R:::::RT:::::TT:::::::TT:::::T C:::::CCCCCCCC::::CLL:::::::LL             II::::::IIBB:::::B     B:::::B
 *    R::::R     R:::::RTTTTTT  T:::::T  TTTTTTC:::::C       CCCCCC  L:::::L                 I::::I    B::::B     B:::::B
 *    R::::R     R:::::R        T:::::T       C:::::C                L:::::L                 I::::I    B::::B     B:::::B
 *    R::::RRRRRR:::::R         T:::::T       C:::::C                L:::::L                 I::::I    B::::BBBBBB:::::B 
 *    R:::::::::::::RR          T:::::T       C:::::C                L:::::L                 I::::I    B:::::::::::::BB  
 *    R::::RRRRRR:::::R         T:::::T       C:::::C                L:::::L                 I::::I    B::::BBBBBB:::::B 
 *    R::::R     R:::::R        T:::::T       C:::::C                L:::::L                 I::::I    B::::B     B:::::B
 *    R::::R     R:::::R        T:::::T       C:::::C                L:::::L                 I::::I    B::::B     B:::::B
 *    R::::R     R:::::R        T:::::T        C:::::C       CCCCCC  L:::::L         LLLLLL  I::::I    B::::B     B:::::B
 *  RR:::::R     R:::::R      TT:::::::TT       C:::::CCCCCCCC::::CLL:::::::LLLLLLLLL:::::LII::::::IIBB:::::BBBBBB::::::B
 *  R::::::R     R:::::R      T:::::::::T        CC:::::::::::::::CL::::::::::::::::::::::LI::::::::IB:::::::::::::::::B 
 *  R::::::R     R:::::R      T:::::::::T          CCC::::::::::::CL::::::::::::::::::::::LI::::::::IB::::::::::::::::B  
 *  RRRRRRRR     RRRRRRR      TTTTTTTTTTT             CCCCCCCCCCCCCLLLLLLLLLLLLLLLLLLLLLLLLIIIIIIIIIIBBBBBBBBBBBBBBBBB   
 *    
 *    
 * concrete class for field correction by correXionPro.exe
 * Description : super easy library for SCANLAB's RTC 5, 6, 6Ethernet
 * Author : hong chan, choi / sepwind@gmail.com (https://sepwind.blogspot.com)
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Diagnostics;
using System.IO;

namespace sepwind
{
    public sealed class RtcCorrection2D : IRtcCorrection
    {
        private readonly string exeFileName = @"correXionPro.exe";
        private readonly List<Vector3> container;
        private uint numOfPoints;
        private float kFactor;
        private float interval;
        private string srcCtbFile;
        private string targetCtbFile;
        private string message;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="numOfPoints">the number of points (ex: 81 if 9x9)</param>
        /// <param name="kFactor">bit/mm value</param>
        /// <param name="interval">distance between each grids</param>
        /// <param name="srcCtbFile">source ct5 filename</param>
        /// <param name="targetCtbFile">target ct5 filename</param>
        public RtcCorrection2D(uint numOfPoints, float kFactor, float interval, string srcCtbFile, string targetCtbFile)
        {
            Debug.Assert(numOfPoints > 0);
            this.container = new List<Vector3>();
            this.numOfPoints = numOfPoints;
            this.kFactor = kFactor;
            this.interval = interval;
            this.srcCtbFile = srcCtbFile;
            this.targetCtbFile = targetCtbFile;
        }
        public bool Add(Vector3 v)
        {
            if (this.container.Count > this.numOfPoints)           
                return false;            
            this.container.Add(v);
            return true;
        }
        public bool AddRange(IEnumerable<Vector3> v)
        {
            if (v.Count() > this.numOfPoints)
                return false;
            this.container.AddRange(v);
            return true;
        }
        public uint Count()
        {
            return (uint)(this.container.Count);
        }
        public void RemoveAll()
        {
            this.container.Clear();
        }
        public bool Convert()
        {
            this.message = string.Empty;

            #region create .dat file
            string datFileName = String.Format($"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}-{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}.dat");
            string datFileFullPath = String.Format($"{Directory.GetCurrentDirectory()}\\{datFileName}");
            using (var stream = new StreamWriter(datFileFullPath))
            {
                stream.WriteLine($"//copyright to https://sepwind.blogspot.com : {datFileName}");
                stream.WriteLine($"OLDCTFILE\t= {this.srcCtbFile}");
                stream.WriteLine($"NEWCTFILE\t= {this.targetCtbFile}");
                stream.WriteLine($"TOLERANCE\t0");
                stream.WriteLine($"NEWCAL\t= {this.kFactor:F8}");
                int grid = (int)Math.Sqrt((double)this.numOfPoints);
                int index = 0;
                double top = this.interval * (double)(grid / 2);   
                for (int row = 0; row < grid; row++)
                {
                    double left = -this.interval * (double)(grid / 2); 
                    top = top - (double)row * this.interval;
                    for (int col = 0; col < grid; col++)
                    {
                        left = left + (double)col * this.interval;
                        stream.WriteLine($"\t{left} \t{this.container[index].X:F3} {this.container[index].Y:F3}");
                        index++;
                    }
                }
            }
            #endregion

            #region create correXionPro.exe process 
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exeFileName;  
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = datFileName;
            using (Process proc = Process.Start(startInfo))
            {
                if (!proc.WaitForExit(10 * 1000))
                    return false;
                if (0 != proc.ExitCode)
                    return false;
            }
            #endregion

            #region read result log file
            String resultLogFileFullPath = datFileFullPath + ".txt";
            if (File.Exists(resultLogFileFullPath))
                return false;
            this.message = File.ReadAllText(resultLogFileFullPath);
            #endregion
            return true;
        }

        public string Result()
        {
            return this.message;
        }
    }
}