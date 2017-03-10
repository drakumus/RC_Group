using System.Windows.Forms;

namespace SpreadsheetController
{
    /// <summary>
    /// Keeps track of how many top-level forms are running, shuts down
    /// the application when there are no more.
    /// </summary>
    class SpreadsheetControllerApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int windowCount = 0;

        // Singleton ApplicationContext
        private static SpreadsheetControllerApplicationContext context;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetControllerApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadsheetControllerApplicationContext.
        /// </summary>
        public static SpreadsheetControllerApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new SpreadsheetControllerApplicationContext();
            }
            return context;
        }

        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        public void RunNew()
        {
            // Create the window
            SpreadsheetController window = new SpreadsheetController();

            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            window.Show();
        }

        /// <summary>
        /// Runs a form in this application context with the window
        /// </summary>
        /// <param name="window"></param>
        public void RunNew(SpreadsheetController window)
        {
            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            window.Show();
        }
    }
}