using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huawei.SCOM.ESightPlugin.Core.Const;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Huawei.SCOM.ESightPlugin.DAO;
using Huawei.SCOM.ESightPlugin.Models;
using Microsoft.EnterpriseManagement.Configuration;

namespace Huawei.SCOM.ESightPlugin.Core
{
    public class ESightConnector : BaseConnector, IHWESightHostDal
    {
        public static Guid connectorGuid = new Guid("{528C8486-2E62-42FB-9AFB-96CB8C089860}");
        public ESightConnector(string connectorName, Guid connectorGuid, ConnectorInfo connectorInfo) : base(connectorName, connectorGuid, connectorInfo)
        {

        }

        private static ESightConnector _instance;


        public static ESightConnector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ESightConnector("ESight.Connector", connectorGuid, new ConnectorInfo()
                    {
                        Description = "bladeServer Connector Description",
                        DisplayName = "BladeServer Connector",
                        Name = "BladeServer.Connector",
                        DiscoveryDataIsManaged = true
                    });
                }
                return _instance;
            }
        }


        #region MyRegion
        private ManagementPackClass _eSightClass;
        public ManagementPackClass ESightClass
        {
            get
            {
                if (_eSightClass == null)
                {
                    _eSightClass = MGroup.Instance.GetManagementPackClass(EntityTypeConst.ESight.MainName);
                }
                return _eSightClass;
            }
        }
        #endregion

        public int InsertEntity(HWESightHost entity)
        {
            throw new NotImplementedException();
        }

        public int UpdateEntity(HWESightHost entity)
        {
            throw new NotImplementedException();
        }

        public HWESightHost GetEntityById(int id)
        {
            throw new NotImplementedException();
        }

        public IList<HWESightHost> GetList(string condition)
        {
            throw new NotImplementedException();
        }

        public HWESightHost FindByIP(string eSightIp)
        {
            throw new NotImplementedException();
        }

        public void DeleteESight(int eSightId)
        {
            throw new NotImplementedException();
        }

        public int UpdateSubscriptionNeDeviceStatus(string hostIp, int status)
        {
            throw new NotImplementedException();
        }

        public int UpdateSubscriptionAlarmStatus(string hostIp, int status)
        {
            throw new NotImplementedException();
        }

        public HWESightHost GetEntityBySystemId(string systemId)
        {
            throw new NotImplementedException();
        }

        public HWESightHost GetEntityByHostIP(string hostIp)
        {
            throw new NotImplementedException();
        }
    }
}    
