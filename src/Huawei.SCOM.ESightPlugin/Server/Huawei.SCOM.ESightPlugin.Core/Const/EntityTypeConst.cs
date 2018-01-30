// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.Core
// Author           : yayun
// Created          : 11-14-2017
//
// Last Modified By : yayun
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="EntityTypeConst.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.Core.Const
{
    /// <summary>
    /// The entity type const.
    /// </summary>
    public class EntityTypeConst
    {
        /// <summary>
        /// The blade server.
        /// </summary>
        public static class BladeServer
        {
            /// <summary>
            /// The fan.
            /// </summary>
            public static string Fan => "ESight.BladeServer.Fan";

            /// <summary>
            /// The fan group.
            /// </summary>
            public static string FanGroup => "ESight.BladeServer.FanGroup";

            /// <summary>
            /// The hmm.
            /// </summary>
            public static string Hmm => "ESight.BladeServer.HMM";

            /// <summary>
            /// The hmm group.
            /// </summary>
            public static string HmmGroup => "ESight.BladeServer.HMMGroup";

            /// <summary>
            /// The main name.
            /// </summary>
            public static string MainName => "ESight.BladeServer";

            /// <summary>
            /// The power supply.
            /// </summary>
            public static string PowerSupply => "ESight.BladeServer.PowerSupply";

            /// <summary>
            /// The power supply group.
            /// </summary>
            public static string PowerSupplyGroup => "ESight.BladeServer.PowerSupplyGroup";

            /// <summary>
            /// The switch.
            /// </summary>
            public static string Switch => "ESight.BladeServer.Switch";

            /// <summary>
            /// The switch group.
            /// </summary>
            public static string SwitchGroup => "ESight.BladeServer.SwitchGroup";

            /// <summary>
            /// The blade.
            /// </summary>
            public static class Blade
            {
                /// <summary>
                /// The cpu.
                /// </summary>
                public static string Cpu => "ESight.BladeServer.Blade.CPU";

                /// <summary>
                /// The cpu group.
                /// </summary>
                public static string CpuGroup => "ESight.BladeServer.Blade.CPUGroup";

                /// <summary>
                /// The disk.
                /// </summary>
                public static string Disk => "ESight.BladeServer.Blade.Disk";

                /// <summary>
                /// The disk group.
                /// </summary>
                public static string DiskGroup => "ESight.BladeServer.Blade.DiskGroup";

                /// <summary>
                /// The main group.
                /// </summary>
                public static string MainGroup => "ESight.BladeServer.BladeGroup";

                /// <summary>
                /// The main name.
                /// </summary>
                public static string BladeName => "ESight.BladeServer.Blade";

                /// <summary>
                /// The memory.
                /// </summary>
                public static string Memory => "ESight.BladeServer.Blade.Memory";

                /// <summary>
                /// The memory group.
                /// </summary>
                public static string MemoryGroup => "ESight.BladeServer.Blade.MemoryGroup";

                /// <summary>
                /// The mezz.
                /// </summary>
                public static string Mezz => "ESight.BladeServer.Blade.Mezz";

                /// <summary>
                /// The mezz group.
                /// </summary>
                public static string MezzGroup => "ESight.BladeServer.Blade.MezzGroup";

                /// <summary>
                /// The raid controller.
                /// </summary>
                public static string RaidController => "ESight.BladeServer.Blade.RaidController";

                /// <summary>
                /// The raid controller group.
                /// </summary>
                public static string RaidControllerGroup => "ESight.BladeServer.Blade.RaidControllerGroup";
            }
        }

        /// <summary>
        /// The e sight.
        /// </summary>
        public static class ESight
        {
            /// <summary>
            /// The huawei server.
            /// </summary>
            public static string HuaweiServer => "ESight.HuaweiServer";
        }

        /// <summary>
        /// The highdensity server.
        /// </summary>
        public static class HighdensityServer
        {
            /// <summary>
            /// The fan.
            /// </summary>
            public static string Fan => "ESight.HighdensityServer.Fan";

            /// <summary>
            /// The fan group.
            /// </summary>
            public static string FanGroup => "ESight.HighdensityServer.FanGroup";

            /// <summary>
            /// The hmm.
            /// </summary>
            public static string Hmm => "ESight.HighdensityServer.HMM";

            /// <summary>
            /// The hmm group.
            /// </summary>
            public static string HmmGroup => "ESight.HighdensityServer.HMMGroup";

            /// <summary>
            /// The main name.
            /// </summary>
            public static string MainName => "ESight.HighdensityServer";

            /// <summary>
            /// The power supply.
            /// </summary>
            public static string PowerSupply => "ESight.HighdensityServer.PowerSupply";

            /// <summary>
            /// The power supply group.
            /// </summary>
            public static string PowerSupplyGroup => "ESight.HighdensityServer.PowerSupplyGroup";

            /// <summary>
            /// The highdensity.
            /// </summary>
            public static class Highdensity
            {
                /// <summary>
                /// The cpu.
                /// </summary>
                public static string Cpu => "ESight.HighdensityServer.Highdensity.CPU";

                /// <summary>
                /// The cpu group.
                /// </summary>
                public static string CpuGroup => "ESight.HighdensityServer.Highdensity.CPUGroup";

                /// <summary>
                /// The disk.
                /// </summary>
                public static string Disk => "ESight.HighdensityServer.Highdensity.Disk";

                /// <summary>
                /// The disk group.
                /// </summary>
                public static string DiskGroup => "ESight.HighdensityServer.Highdensity.DiskGroup";

                /// <summary>
                /// The main group.
                /// </summary>
                public static string MainGroup => "ESight.HighdensityServer.HighdensityGroup";

                /// <summary>
                /// The main name.
                /// </summary>
                public static string HighdensityName  => "ESight.HighdensityServer.Highdensity";

                /// <summary>
                /// The memory.
                /// </summary>
                public static string Memory => "ESight.HighdensityServer.Highdensity.Memory";

                /// <summary>
                /// The memory group.
                /// </summary>
                public static string MemoryGroup => "ESight.HighdensityServer.Highdensity.MemoryGroup";

                /// <summary>
                /// The raid controller.
                /// </summary>
                public static string RaidController => "ESight.HighdensityServer.Highdensity.RaidController";

                /// <summary>
                /// The raid controller group.
                /// </summary>
                public static string RaidControllerGroup => "ESight.HighdensityServer.Highdensity.RaidControllerGroup";
            }
        }

        /// <summary>
        /// The kun lun server.
        /// </summary>
        public static class KunLunServer
        {
            /// <summary>
            /// The cpu.
            /// </summary>
            public static string Cpu => "ESight.KunLunServer.CPU";

            /// <summary>
            /// The cpu group.
            /// </summary>
            public static string CpuGroup => "ESight.KunLunServer.CPUGroup";

            /// <summary>
            /// The fan.
            /// </summary>
            public static string Fan => "ESight.KunLunServer.Fan";

            /// <summary>
            /// The fan group.
            /// </summary>
            public static string FanGroup => "ESight.KunLunServer.FanGroup";

            /// <summary>
            /// The main name.
            /// </summary>
            public static string MainName => "ESight.KunLunServer";

            /// <summary>
            /// The memory.
            /// </summary>
            public static string Memory => "ESight.KunLunServer.Memory";

            /// <summary>
            /// The memory group.
            /// </summary>
            public static string MemoryGroup => "ESight.KunLunServer.MemoryGroup";

            /// <summary>
            /// The physical disk.
            /// </summary>
            public static string PhysicalDisk => "ESight.KunLunServer.PhysicalDisk";

            /// <summary>
            /// The physical disk group.
            /// </summary>
            public static string PhysicalDiskGroup => "ESight.KunLunServer.PhysicalDiskGroup";

            /// <summary>
            /// The power supply.
            /// </summary>
            public static string PowerSupply => "ESight.KunLunServer.PowerSupply";

            /// <summary>
            /// The power supply group.
            /// </summary>
            public static string PowerSupplyGroup => "ESight.KunLunServer.PowerSupplyGroup";

            /// <summary>
            /// The raid controller.
            /// </summary>
            public static string RaidController => "ESight.KunLunServer.RaidController";

            /// <summary>
            /// The raid controller group.
            /// </summary>
            public static string RaidControllerGroup => "ESight.KunLunServer.RaidControllerGroup";
        }

        /// <summary>
        /// The rack server.
        /// </summary>
        public static class RackServer
        {
            /// <summary>
            /// The cpu.
            /// </summary>
            public static string Cpu => "ESight.RackServer.CPU";

            /// <summary>
            /// The cpu group.
            /// </summary>
            public static string CpuGroup => "ESight.RackServer.CPUGroup";

            /// <summary>
            /// The fan.
            /// </summary>
            public static string Fan => "ESight.RackServer.Fan";

            /// <summary>
            /// The fan group.
            /// </summary>
            public static string FanGroup => "ESight.RackServer.FanGroup";

            /// <summary>
            /// The main name.
            /// </summary>
            public static string MainName => "ESight.RackServer";

            /// <summary>
            /// The memory.
            /// </summary>
            public static string Memory => "ESight.RackServer.Memory";

            /// <summary>
            /// The memory group.
            /// </summary>
            public static string MemoryGroup => "ESight.RackServer.MemoryGroup";

            /// <summary>
            /// The physical disk.
            /// </summary>
            public static string PhysicalDisk => "ESight.RackServer.PhysicalDisk";

            /// <summary>
            /// The physical disk group.
            /// </summary>
            public static string PhysicalDiskGroup => "ESight.RackServer.PhysicalDiskGroup";

            /// <summary>
            /// The power supply.
            /// </summary>
            public static string PowerSupply => "ESight.RackServer.PowerSupply";

            /// <summary>
            /// The power supply group.
            /// </summary>
            public static string PowerSupplyGroup => "ESight.RackServer.PowerSupplyGroup";

            /// <summary>
            /// The raid controller.
            /// </summary>
            public static string RaidController => "ESight.RackServer.RaidController";

            /// <summary>
            /// The raid controller group.
            /// </summary>
            public static string RaidControllerGroup => "ESight.RackServer.RaidControllerGroup";
        }
    }
}