using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetController;

namespace SpreadsheetControllerTester
{
    class SpreadsheetViewStub : ISpreadsheetView
    {
        // These two properties record whether a method has been called
        public bool CalledDoClose
        {
            get; private set;
        }

        public bool CalledOpenNew
        {
            get; private set;
        }

        public bool CalledOpenNewFile
        {
            get; private set;
        }

        public bool CalledSetValue
        {
            get; private set;
        }
        
        // These four methods cause events to be fired
        public void FireCloseEvent()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        public void FireEditEvent(string substring)
        {
            if (EditEvent != null)
            {
                EditEvent(substring);
            }
        }

        public void FireFileChosenEvent(string filename)
        {
            if (FileChosenEvent != null)
            {
                FileChosenEvent(filename);
            }
        }

        public void FireNewEvent()
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        public void FireSaveEvent()
        {
            if (SaveEvent != null)
            {
                SaveEvent();
            }
        }

        // These four properties implement the interface
        public string Cell
        {
            get; set;
        }

        public string Contents
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }
        
        // These six events implement the interface
        public event Action CloseEvent;

        public event Action<string> EditEvent;

        public event Action<string> FileChosenEvent;

        public event Action NewEvent;

        public event Action SaveEvent;

        public event Action<int, int> UpdateEvent;


        // These four methods implement the interface
        public void DoClose()
        {
            CalledDoClose = true;
        }

        public void OpenNew()
        {
            CalledOpenNew = true;
        }

        public void OpenNew(string filePath)
        {
            CalledOpenNewFile = true;
        }

        public void SetValue(int col, int row, string cellValue)
        {
            CalledSetValue = true;
        }
    }
}
