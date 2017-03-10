using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSGui;

namespace SpreadsheetController
{
    /// <summary>
    /// Controllable interface of SpreadsheetWindow
    /// used for interactions between controller and view.
    /// </summary>
    interface ISpreadsheetView
    {
        event Action CloseEvent;

        event Action<string> FileChosenEvent;

        event Action NewEvent;

        event Action SaveEvent;

        event Action<string> EditEvent;

        event Action<int, int> UpdateEvent;

        string Value { set; }

        string Cell { set; }

        string Contents { set; }
        string Message { set; }

        void DoClose();

        void OpenNew();

        void OpenNew(string filePath);

        void SetValue(int col, int row, string cellValue);
    }
}
