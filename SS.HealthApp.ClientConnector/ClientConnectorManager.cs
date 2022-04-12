using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.ClientConnector
{
    public class ClientConnectorManager
    {

        #region delegates

        public delegate object ClientServiceCreator(ClientConnectorManager container);

        #endregion

        #region fields

        private readonly Dictionary<string, object> configuration = new Dictionary<string, object>();
        private readonly Dictionary<Tuple<Type, string>, ClientServiceCreator> typeToCreator = new Dictionary<Tuple<Type, string>, ClientServiceCreator>();

        #endregion

        #region Singleton  Implementation

        private static volatile ClientConnectorManager instance;
        private static object syncRoot = new Object();

        private ClientConnectorManager() { }

        public static ClientConnectorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ClientConnectorManager();
                    }
                }

                return instance;
            }
        }

        #endregion

        #region register objects

        public void Register<T>(string ClientId, ClientServiceCreator Creator)
        {
            typeToCreator.Add(new Tuple<Type, string>(typeof(T), ClientId), Creator);
        }

        #endregion

        #region creating objects

        public T Create<T>(string ClientId)
        {
            return (T)typeToCreator[new Tuple<Type, string>(typeof(T), ClientId)](this);
        }

        #endregion

        #region Application configuration strings

        public Dictionary<string, object> Configuration
        {
            get { return configuration; }
        }

        public T GetConfiguration<T>(string name)
        {
            return (T)configuration[name];
        }

        #endregion

    }
}
