using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using InventraceDLogic;
using System.Data;

namespace InventraceBLogic
{
    class Zone
    {
        InventraceDLogic.BasicDL _dl = new BasicDL();

        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ZoneDesc { get; set; }
        public int DepartmentId { get; set; }
        public bool IsExitZone { get; set; }
        public bool EnableShiftInventory { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifieDate { get; set; }
        public int CreatedBy { get; set; }

        public Zone()
        {
            ZoneId = 0;
            ZoneName = "";
            ZoneDesc = "";
            DepartmentId = 0;
            IsExitZone = true;
            EnableShiftInventory = true;
            CreatedDate = DateTime.Now;
            ModifieDate = DateTime.Now;
            CreatedBy = 0;
        }

        public string AddZone(Zone zone)
        {
            return _dl.InsertZone(zone.ZoneName, zone.ZoneDesc, zone.DepartmentId, zone.IsExitZone, zone.EnableShiftInventory, zone.CreatedDate, zone.ModifieDate, zone.CreatedBy);
        }

        public string RemoveZone(Zone zone)
        {

            return _dl.RemoveZone(zone.ZoneId);

        }

        public string UpdateZone(Zone zone)
        {
            return _dl.UpdateZone(zone.ZoneId, zone.ZoneName, zone.ZoneDesc, zone.DepartmentId, zone.IsExitZone, zone.EnableShiftInventory, zone.CreatedDate, zone.ModifieDate, zone.CreatedBy);
        }

        public List<Zone> GetAllZones()
        {
            return GetZone(_dl.GetAllZones());
        }

        public List<Zone> GetZoneById(int zoneId)
        {
            return GetZone(_dl.GetZoneById(zoneId));
        }

        private List<Zone> GetZone(DataTable dt)
        {
            var list = dt.AsEnumerable()
                .Select(row => new Zone()
                {
                    ZoneId = Convert.ToInt16(row["ZoneId"]),
                    ZoneName = row["ZoneName"].ToString(),
                    ZoneDesc = row["ZoneDesc"].ToString(),
                    DepartmentId = Convert.ToInt16(row["DepartmentId"]),
                    IsExitZone = Convert.ToBoolean(row["IsExitZone"]),
                    EnableShiftInventory = Convert.ToBoolean(row["EnableShiftInventory"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifieDate = Convert.ToDateTime(row["ModifiedDate"]),
                    CreatedBy = Convert.ToInt16(row["CreatedBy"])
                }).ToList();
            return list;
        }


    }
}
