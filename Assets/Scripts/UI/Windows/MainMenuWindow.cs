using UICore;

public class MainMenuWindow : Window
{
    public override void OnPush()
    {
        PushFinished();
    }

    public override void OnPop()
    {
        PopFinished();
    }
}
