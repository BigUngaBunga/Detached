public class ActivatedWall : Activator
{
    //TODO �ndra likt activated platform
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
