namespace nucs.Automation.Controllers {
    /// <summary>
    ///     A more advanced implementation that allows combo pressing.
    /// </summary>
    public interface IExtendedKeyboardController : IKeyboardController {
        void Enter();
        void Back();

        void Alt(KeyCode keycode);
        void Shift(KeyCode keycode);
        void Control(KeyCode keycode);
        void Win(KeyCode keycode);

        void PressAsync(KeyCode keycode, uint delay = 20);
    }
}