namespace NetCoreBB.Domain.Model.SystemConfig
{
    public class System
    {
        public bool Installed { get; set; }
        public bool Maintenance { get; set; }
        public bool Development { get; set; }
        public bool UnderAttack { get; set; }
    }
}
