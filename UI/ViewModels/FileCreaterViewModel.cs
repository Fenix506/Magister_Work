using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.ViewModels
{
    class FileCreaterViewModel:Conductor<IScreen>
    {
        public FileCreaterViewModel()
        {
            DisplayName = "Generate file";
        }
    }
}
