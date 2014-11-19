using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace InventraceBLogic
{
    class Zone
    {

        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ZoneDesc { get; set; }
        public int DepartmentId { get; set; }
        public bool IsExitZone { get; set; }
        public bool EnableShiftInventory { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifieDate { get; set; }
        public int CreatedBy { get; set; }

        public void AddZone(Zone zone)
        {

        }

        public void RemoveZone(Zone zone)
        {
            
        }

        public void UpdateZone(Zone zone)
        {
            
        }

        public List<Zone> GetZones()
        {
            return null;
        }

    }
}
