public class ActivatedWall : Activator
{
    protected override void Activate()
    {
        base.Activate();
        gameObject.SetActive(false);
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(true);
    }
}
