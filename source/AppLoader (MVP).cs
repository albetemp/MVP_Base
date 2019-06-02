using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using A.Common;
using A.Common;
using A.Common.ApplicationSettings;
using A.Common.Containers;
using A.Common.Tasks;
using A.Common.Models;
using A.Common.Views;
using A.Model.Classes;
using A.Presenter.Classes;
using A.View.Forms;
using NLog;

namespace A
{
    static class AppLoader
    {

        private static readonly AppSettings _settings = AppSettings.GetInstance();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // prevent to run application twice

            bool mutexIsNew;

            using (new System.Threading.Mutex(true, AppDefs.APP_GUID, out mutexIsNew))
            {
                if (!mutexIsNew)
                {
                    MessageBox.Show(ResourcesText.Common_Application_is_already_running_EN, AppDefs.MAIN_FORM_CAPTION, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    WindowActivator.ActivateWindow(AppDefs.MAIN_FORM_CAPTION);

                    return;
                }

                if (!System.Diagnostics.Debugger.IsAttached) // use internal exception handler without debuggers
                {
                    // AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
                    // Application.ThreadException += NBug.Handler.ThreadException;
                    // TaskScheduler.UnobservedTaskException += NBug.Handler.UnobservedTaskException;
                }

                NLog.Logger appLogger = null;
                NLog.Logger cuLogger = null;

                WindowsFormsSettings.ForceDirectXPaint();

                // trying to use logger
                try
                {
                    appLogger = LogManager.GetLogger(AppDefs.APP_LOGGER_NAME);
                    cuLogger = LogManager.GetLogger(AppDefs.CU_LOGGER_NAME);

                    appLogger.Trace(@"NLog internal test");
                    cuLogger.Trace(@"NLog internal test");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ResourcesText.Common_Error_opening_log_file_EN + Environment.NewLine + ResourcesText.Common_Exception_EN + @": " + ex.ToString(), AppDefs.MAIN_FORM_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //DevExpress.Skins.SkinManager.DisableFormSkins();

                IApplicationController controller = new ApplicationController(new LightInjectAdapder());
                //try
                //{
                    // register all dependencies to run MainPresenter
                    controller.RegisterInstance(new ApplicationContext());
                    controller.RegisterService<IMainModel, MainModel>();
                    controller.RegisterView<IMainView, MainForm>();
                    controller.RegisterView<IAboutView, AboutForm>();
                    controller.Run<MainPresenter>();
                //}
                //catch (Exception ex)
                /*{
                    MessageBox.Show(ResourcesText.Common_Error_loading_app_EN + Environment.NewLine + ResourcesText.Common_Exception_EN + @": " + ex.ToString(),
                        AppDefs.MAIN_FORM_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Application.Exit();
                }*/
            }
        }
    }
}
