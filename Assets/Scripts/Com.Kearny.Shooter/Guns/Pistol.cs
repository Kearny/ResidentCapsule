namespace Com.Kearny.Shooter.Guns
{
    public class Pistol : Gun
    {
        protected override FireMode FireMode { get; set; } = FireMode.SemiAuto;
    }
}