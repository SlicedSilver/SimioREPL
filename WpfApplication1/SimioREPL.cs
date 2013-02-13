using SimioAPI.Extensions;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace SimioREPL
{
    class SimioREPL : IDesignAddIn
    {
        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Simio Interactive Window"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "Simio REPL Interactive Window. (Developer Tool) Opens a REPL (read-evaluate-print-loop) window that can be used to test design add-in code. Created by Mark Silverwood."; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.smallIcon; }
        }

        /// <summary>
        /// Method called when the add-in is run.
        /// </summary>
        public void Execute(IDesignContext context)
        {
            // This example code places some new objects from the Standard Library into the active model of the project.
            if (context.ActiveModel != null)
            {
                ScriptHost.context = context;
                //REPLWin window = new REPLWin();
                //window.Show();
                MainWindow window = new MainWindow();
                System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
                window.Show();
            }
        }
    }
    public static class Sc
    {
        public static ScriptHost Host;
    }
    public class ScriptHost
    {
        readonly ScriptEngine _scriptEngine;
        readonly Session _session;
// ReSharper disable InconsistentNaming
        public static IDesignContext context;
// ReSharper restore InconsistentNaming
        public ScriptHost()
        {
            _scriptEngine = new ScriptEngine();
            _session = _scriptEngine.CreateSession(this);
        }

        public object Execute(string code)
        {
            return _session.Execute(code);
        }

        public T Execute<T>(string code)
        {
            return _session.Execute<T>(code);
        }

        public void ExecuteFile(string path)
        {
            _session.ExecuteFile(path);
        }
    }
}
