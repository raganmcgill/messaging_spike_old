using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dashboard.ui.Controllers
{
    public class HospitalController : Controller
    {
        // GET: Hospital
        public ActionResult Index()
        {
            return View();
        }
        private bool ProgramIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    isRunning = true;
                    break;
                }
            }

            return isRunning;
        }

        public ActionResult GetStatuses()
        {
            var result = new List<Beat>();
            string root = $@"C:\dev\Stores\hospital\";

            DirectoryInfo rootInfo = new DirectoryInfo(root);

            if (rootInfo.Exists)
            {
                var files = rootInfo.GetFiles();

                foreach (var fileInfo in files)
                {
                    var item = JsonConvert.DeserializeObject<Beat>(System.IO.File.ReadAllText(fileInfo.FullName));

//                    if (item.Age >20 && !ProgramIsRunning(item.Path))
//                    {
//                        Process.Start(item.Path);
//                    }

                    result.Add(item);
                }
            }

            return PartialView("_Heartbeat", result);
        }
    }

}
