namespace CRUDServices.BusinessObjects
{
    public class BInterfaceAPI
    {
        public string InterfaceName { get; set; } = "";
        public string? BaseAddress { get; set; }
        public List<Header>? DefaultHeader { get; set; } = new List<Header>();
        public string? LoginPath { get; set; }
        public string TokenJsonPath { get; set; } = "";
        public string? UserEnc { get; set; }
        public string? PassEnc { get; set; }
        public string? Payload { get; set; }

        public class Header
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
