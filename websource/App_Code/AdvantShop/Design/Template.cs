using System;

namespace AdvantShop.Design
{
    [Serializable]
    public class Template
    {
        public int Id { get; set; }
        public string StringId { get; set; }
        public bool Active { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        public string PreviewImage { get; set; }
        public bool IsInstall { get; set; }
        public string DemoLink { get; set; }
    }
}