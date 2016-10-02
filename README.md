# nucs.Automation

Automation library aims to provide a simple async-task library to simulating both keyboard and mouse with essential classes to provide a way to automate any action on your pc.

#### Capabilities

##### Simulating
- `Mouse` - static mouse simulator (instance of `MouseController` w/o interface).
    - `Move(Point xy, double velocity)` - moves the cursor in a realistic way.
    - `MoveAbsolute(Point)` - moves the cursor instantly to the given point.
    - `Click()` - left click in the current location.
    - `DoubleClick()` - you guessed it!
    - `MoveRelative()` - Move the mouse relativly to the mouse current location.
    - `MiddleClick()` ,`RightClick()` ,`DragDrop(Point from, Point to)`, `LeftDown()`, `LeftUp()`, `WheelDown()`, `WheelUp()` and so on...
- `Keyboard` - static keyboard simulator (instance of `KeyboardController` that implements `IModernKeyboard`).
    - `Keyboard.Write(string)` - types down a string
    - `Keyboard.Press(KeyCode)` - presses a keyboard button - duhh
    - `Keyboard.Control(KeyCode)` - presses a button while holding Control, exists for `Shift`, `Win` and `Alt`.
    - Some more similar actions
##### Mirroring  - Windows Reflection
- SmartProcess - A cached version of an existing `Process` type with some neat features
    - `WaitForExitAsync()` - async wait for the process to exit.
    - `WaitForRespondingAsync()` - async wait for the process to be able to accept input.
    - `Windows` - property - returns all instances of `Window` associated to this window.
    - `MainWindow` - property - returns an instance of `Window` of the MainWindow of this process
    - All methods available in Process including `OGProcess` prop to access the native process
    - Static Methods
        - `SmartProcess.Get(string processname)` - Gets cached or caches the first process with the name `processname`
        - `SmartProcess.Get(Process)` - Gets cached or caches the passed process.
        - `SmartProcess GetCached(Guid)` - Gets a cached SmartProcess that has the given guid (accessable from SmartProcess instance `smartproc.GUID`)
- Window
    - `WaitForRespondingAsync()` - async wait for the process to be able to accept input.
    - `ChangeWindowState(WindowState)` - minimize, maximaze or normal the window.
    - `SetWindowSize(Size)` - changes the window size.
    - `SetWindowPosition(x, y)` - moves the window to the given xy.
    - `IsFocused` - is it foreground atm?
    - `Title` - returns the window's title.
    - `IsVisible` - is it currently visible on the screen otherwise it is a part of a UI window.
    - `IsValid` - does the window still exist?
    - `Position` - returns a `Rectangle` of the window's position and size.
    - `BringToFront` - sets the window as foreground / focused.
    - `Mouse` - is an instance of `RelativeMouse` that any clicks using this property will result in relative clicking to the window's boundries.
    - `Keyboard` - is essentially same as static `Keyboard`.
    - Static Methods
        - `Create(SmartProcess process, HWND handle)` creates a window object.

#### Real-World Example
This example will run a program using WIN+R and will return the process that has opened.
```C#
Func<Process, bool> processIdentifier = proc=>proc.ProcessName=="notepad++"; //will open a notepad
var application = "notepad++.exe"

var sproc = SmartProcess.Get("explorer"); //Find the explorer process
await Task.Yield(); //Make the rest code below async!
_recapture:
//Find a window in explorer process that is a Run window.
var win = sproc.Windows.FirstOrDefault(w => w.Type == WindowType.Run); 
if (win == null) { //if it does not already exist - create one.
	Keyboard.Window(KeyCode.R); //press WIN+R
	goto _recapture;
}
win.BringToFront(); //make sure it is in the front
await win.WaitForRespondingAsync(); //wait till the window is able to accept input - important for slow PCs

win.Keyboard.Write(application); //Equivalent to Keyboard.Write(...) but this call makes sure it is sent to the window!
win.Keyboard.Enter(); //Press enter which will launch the run-line command
Thread.Sleep(200); //some delay to let the program actually open

//Get the process

if (processIdentifier != null) { //if for some reason it is not in the front - get the newest process of this type
	var p = Process.GetProcesses().Where(processIdentifier).OrderByDescending(pp => {
		try {
			return pp.StartTime.Ticks;
		} catch {
			return 0;
		}
	}).ToArray();

	var @out = p.FirstOrDefault(proc => proc.Id == foreg.Id && proc.ProcessName == foreg.ProcessName);
	if (@out != null) {
		return SmartProcess.Get(@out); //cache this process into a smartprocess
	}
}

return SmartProcess.GetForeground(); //returns a SmartProcess object of the foreground process - just the one that was recently opened;

```

##### License
MIT License

Copyright (c) 2016 Eli Belash

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.