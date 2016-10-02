namespace nucs.Automation.Controllers {
    /// <summary>
    ///     A more advanced implementation that allows a press of button with 
    /// </summary>
    public interface IModernKeyboard : IKeyboard {
        void Enter();
        void Back();

        void Alt(KeyCode keycode);
        void Shift(KeyCode keycode);
        void Control(KeyCode keycode);
        void Win(KeyCode keycode);

        void PressAsync(KeyCode keycode, uint delay = 20);
    }
}