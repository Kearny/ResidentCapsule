namespace Com.Kearny.Shooter.Weapons
{
    public class Pistol : Gun
    {
        protected override FireMode FireMode { get; set; } = FireMode.Auto;
    }
}