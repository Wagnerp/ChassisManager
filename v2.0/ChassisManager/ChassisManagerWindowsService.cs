// Copyright © Microsoft Open Technologies, Inc.
// All Rights Reserved
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
// See the Apache 2 License for the specific language governing permissions and limitations under the License. 

namespace Microsoft.GFS.WCS.ChassisManager
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Description;
    using System.Threading;
    using System.ComponentModel;
    using System.ServiceProcess;
    using System.Configuration.Install;
    using Microsoft.GFS.WCS.Contracts;
    using System.Security.Cryptography.X509Certificates;
    
    /// <summary>
    /// This class creates and initializes the Chassis Manager service
    /// </summary>
    public class ChassisManagerWindowsService : ServiceBase
    {
        public WebServiceHost serviceHost = null;

        public ChassisManagerWindowsService()
        {
            // Name the Windows Service
            ServiceName = "ChassisManager";
        }

        /// <summary>
        /// Chassis manager release function
        /// </summary>
        public void Release()
        {
            ChassisManagerInternal.Halt();
        }

        /// <summary>
        /// Chassis Manager initialize function
        /// </summary>
        public void Initialize()
        {
            Tracer.WriteInfo("Chassis Manager Internal Initialization started..");
            byte status = ChassisManagerInternal.Initialize();

            if (status != (byte)CompletionCode.Success)
            {
                Tracer.WriteError("Chassis manager failed to initialize at {0}", DateTime.Now);
                this.Stop();
            }
            Tracer.WriteInfo("Chassis Manager initialization completed");
        }

        public static void Main()
        {
            ServiceBase.Run(new ChassisManagerWindowsService());
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CustomUnhandledExceptionEventHandler);
            try
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }

                // Creating a Restful binding
                WebHttpBinding bind = new WebHttpBinding();
                bind.ReceiveTimeout = TimeSpan.FromMinutes(ConfigLoaded.ServiceTimeoutInMinutes);

                Tracer.WriteInfo("CM Service: PortNo: {0}, Encryption:{1}", ConfigLoaded.CmServicePortNumber, ConfigLoaded.EnableSslEncryption);

                if (!ConfigLoaded.EnableSslEncryption)
                {
                    // Http url endpoint for the service
                    serviceHost = new WebServiceHost(typeof(ChassisManager), new Uri("http://localhost:" + ConfigLoaded.CmServicePortNumber.ToString() + "/"));

                    bind.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                }
                else
                {
                    // Https url endpoint for the service
                    serviceHost = new WebServiceHost(typeof(ChassisManager), new Uri("https://localhost:" + ConfigLoaded.CmServicePortNumber.ToString() + "/"));

                    // Self-signed certificate located in standard certifcate store location in local machine 
                    // TODO: Change this to use remote active directory based certificate signed by Microsoft Certificate Authority
                    serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, ConfigLoaded.SslCertificateName);

                    // Specify transport level security (SSL ENCRYPTION)
                    bind.Security.Mode = WebHttpSecurityMode.Transport;
                }

                // Client AUTHENTICATION is done using Windows credentials (Active directory)
                bind.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

                // Establish a service endpoint
                ServiceEndpoint ep = serviceHost.AddServiceEndpoint(typeof(IChassisManager), bind, "");

                // Add a custom authorization manager to the service authorization behavior.
                serviceHost.Authorization.ServiceAuthorizationManager = new MyServiceAuthorizationManager();

                ServiceDebugBehavior sdb = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                sdb.HttpHelpPageEnabled = false;
            }
            catch (InvalidOperationException ex)
            {
                Tracer.chassisManagerEventLog.WriteEntry(ex.Message + ". You may try disabling encryption through app.config or install certificate with the name provided in app.config.");
                Tracer.WriteError(ex.Message + ". You may try disabling encryption through app.config or install certificate with the name provided in app.config.");
                Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Tracer.chassisManagerEventLog.WriteEntry("Exception in starting CM service : " + ex.Message);
                Tracer.WriteError("Exception in starting CM service " + ex.Message);
                Environment.Exit(-1);
            }

            int requiredTime = (ConfigLoaded.Population * 60000);

            // CM intialization
            RequestAdditionalTime(requiredTime); // This time period is based on blade population (might need to be tuned)

            Tracer.WriteInfo(string.Format("Additional Time Requeted: {0}", (requiredTime)));

            this.Initialize();

            Tracer.WriteInfo("Internal Initialize Complete. Attempting to open WCF host for Business");

            // Service open for connections
            serviceHost.Open();

            Tracer.WriteInfo("WCF opened for Business");
        }

        private static void CustomUnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            if (Tracer.chassisManagerEventLog != null)
            {
                Tracer.chassisManagerEventLog.WriteEntry("CustomUnhandledExceptionEventHandler caught : " + e.Message);
            }
            Environment.Exit(-1);
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            Tracer.WriteInfo("OnStop: Service closed");

            RequestAdditionalTime(60 * 1000); // This is to prevent Windows service from timeouts

            // Release Chassis Manager threads
            this.Release();
            Tracer.WriteInfo("OnStop: Chassis Manager threads stopped");

            // Try to gracefully Close Open Ipmi sessions
            WcsBladeFacade.Release();
            Tracer.WriteInfo("OnStop: WcsBladeFacade released");

            // Release the communication device layer holds
            CommunicationDevice.Release();
            Tracer.WriteInfo("OnStop: Communication Device released");

        }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "ChassisManager";
            this.service.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
