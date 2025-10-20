# SendSequenceCL

**Human-like Input Automation Class Library for .NET**

SendSequenceCL provides an intuitive API for mouse and keyboard automation through the Tetherscript HID virtual driver.

## Features

✅ **Mouse Automation**
- Instant teleport positioning (`MoveTo`)
- Human-like curved movement with Bézier curves (`MoveHuman`)
- Click, double-click, drag operations
- Support for Left, Right, Middle buttons

✅ **Keyboard Automation**
- Type text with automatic shift handling (`TypeText`)
- Press individual keys (`KeyPress`)
- Execute keyboard shortcuts with modifiers (`Chord`)
- Manual key control (`KeyDown`, `KeyUp`)

✅ **Human-Like Behavior**
- Configurable movement curves with randomization
- Adjustable timing for clicks and keystrokes
- Natural delays between operations

✅ **Type-Safe API**
- Strong typing with enums for buttons and keys
- Comprehensive exception handling
- XML documentation for IntelliSense

## Prerequisites

- **.NET 8.0** or later
- **Windows 10/11** (x64)
- **Tetherscript HID Virtual Driver** installed and connected

### Obtaining Tetherscript HID Driver

**Important:** The Tetherscript HVDK was discontinued in December 2022. However, you can still obtain the drivers:

1. Download **ControlMyJoystick 14-day free trial** from: https://controlmyjoystick.com/
2. Install the trial - this will install the Tetherscript HID drivers
3. The drivers will continue to work even after the trial expires
4. No purchase required for driver functionality

**Note:** If you cannot install the drivers due to certificate expiry, this library will throw `DriverNotFoundException` when attempting to initialize.

## Quick Test

To quickly test the library with Tetherscript HID driver:

```bash
# Clone or download the repository
cd SendSequenceCL

# Run test application
cd TestApp
dotnet run
```

The test application will execute 4 comprehensive test suites:
1. **Connection Test** - Verify driver connectivity
2. **Mouse Test** - MoveTo, MoveHuman, Click, Drag
3. **Keyboard Test** - TypeText, KeyPress, Chord shortcuts
4. **Configuration Test** - Runtime settings modification

See `TestApp/README.md` for detailed test documentation.

## Installation

Add package reference to your project:

```xml
<ItemGroup>
  <PackageReference Include="SendSequenceCL" Version="1.0.0" />
</ItemGroup>
```

Or via .NET CLI:

```bash
dotnet add package SendSequenceCL
```

## Quick Start

### Basic Mouse Control

```csharp
using SendSequenceCL;

// Move mouse instantly to coordinates
Input.Mouse.MoveTo(960, 540);

// Move mouse with human-like curved motion
Input.Mouse.MoveHuman(500, 300);

// Click at current position
Input.Mouse.Click(MouseButton.Left);

// Drag and drop
Input.Mouse.Drag(100, 100, 400, 300, MouseButton.Left);
```

### Basic Keyboard Control

```csharp
using SendSequenceCL;

// Type text (handles uppercase and symbols automatically)
Input.Keyboard.TypeText("Hello from SendSequenceCL!");

// Press individual keys
Input.Keyboard.KeyPress(VirtualKey.Enter);
Input.Keyboard.KeyPress(VirtualKey.Escape);

// Execute keyboard shortcuts
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C); // Ctrl+C
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.V); // Ctrl+V

// Multiple modifiers
Input.Keyboard.Chord(
    new[] { VirtualKey.LeftControl, VirtualKey.LeftShift },
    VirtualKey.Escape  // Ctrl+Shift+Esc
);
```

### Configuration

```csharp
using SendSequenceCL;

// Configure human-like behavior
Configuration.MouseMovementDuration = 400;  // ms (default: 400)
Configuration.CurveRandomization = 0.15;    // 0.0-1.0 (default: 0.10)
Configuration.ClickDuration = 50;           // ms (default: 50)
Configuration.KeystrokeDelay = 75;          // ms (default: 75)
Configuration.ModifierHoldDuration = 100;   // ms (default: 100)

// Fast automation (speed priority)
Configuration.MouseMovementDuration = 100;
Configuration.KeystrokeDelay = 30;

// Very human-like (anti-bot detection)
Configuration.MouseMovementDuration = 450;
Configuration.CurveRandomization = 0.15;
Configuration.KeystrokeDelay = 90;
```

## Complete Examples

### Example 1: Fill Web Form

```csharp
using SendSequenceCL;

// Click in username field
Input.Mouse.MoveHuman(300, 200);
Input.Mouse.Click(MouseButton.Left);
Input.Wait(100);

// Type username
Input.Keyboard.TypeText("john.doe@example.com");

// Tab to password field
Input.Keyboard.KeyPress(VirtualKey.Tab);
Input.Wait(50);

// Type password
Input.Keyboard.TypeText("SecurePassword123");

// Submit form
Input.Keyboard.KeyPress(VirtualKey.Enter);
```

### Example 2: Copy-Paste Automation

```csharp
using SendSequenceCL;

// Select all text
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.A);
Input.Wait(50);

// Copy to clipboard
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C);
Input.Wait(100);

// Click in new location
Input.Mouse.MoveHuman(500, 400);
Input.Mouse.Click(MouseButton.Left);
Input.Wait(50);

// Paste
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.V);
```

### Example 3: Drag and Drop

```csharp
using SendSequenceCL;

// Drag file from desktop to folder (slow, visible)
Input.Mouse.Drag(
    startX: 150,
    startY: 150,
    endX: 600,
    endY: 400,
    button: MouseButton.Left,
    durationMs: 800
);

Console.WriteLine("File moved!");
```

### Example 4: Error Handling

```csharp
using SendSequenceCL;

try
{
    Input.Mouse.MoveTo(500, 300);
    Input.Keyboard.TypeText("test");
}
catch (DriverNotFoundException ex)
{
    Console.WriteLine("HID driver not found!");
    Console.WriteLine("Install Tetherscript driver and retry.");
}
catch (InvalidCoordinateException ex)
{
    Console.WriteLine($"Invalid coordinates: {ex.Message}");
}
catch (DriverCommunicationException ex)
{
    Console.WriteLine($"Driver error: {ex.Message}");
}
```

## API Reference

### Input (Static Facade)

| Member | Description |
|--------|-------------|
| `Input.Mouse` | Access virtual mouse operations |
| `Input.Keyboard` | Access virtual keyboard operations |
| `Input.Wait(int ms)` | Pause execution for specified duration |

### IVirtualMouse

| Method | Description |
|--------|-------------|
| `Point GetPosition()` | Get current cursor position |
| `void MoveTo(int x, int y)` | Instant movement (teleport) |
| `void MoveHuman(int x, int y, int? durationMs)` | Human-like curved movement |
| `void Click(MouseButton button)` | Single click |
| `void DoubleClick(MouseButton button)` | Double click |
| `void Down(MouseButton button)` | Press button down |
| `void Up(MouseButton button)` | Release button |
| `void Drag(int startX, int startY, int endX, int endY, ...)` | Drag operation |

### IVirtualKeyboard

| Method | Description |
|--------|-------------|
| `void TypeText(string text)` | Type text with auto-shift |
| `void KeyPress(VirtualKey key)` | Press single key |
| `void Chord(VirtualKey modifier, VirtualKey key)` | Keyboard shortcut (1 modifier) |
| `void Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key)` | Keyboard shortcut (multiple modifiers) |
| `void KeyDown(VirtualKey key)` | Press key down |
| `void KeyUp(VirtualKey key)` | Release key |

### Configuration

| Property | Default | Description |
|----------|---------|-------------|
| `MouseMovementDuration` | 400 ms | Duration of human-like mouse movements |
| `CurveRandomization` | 0.10 | Random variation in Bézier curves (0.0-1.0) |
| `ClickDuration` | 50 ms | Delay between button down/up |
| `KeystrokeDelay` | 75 ms | Delay between keystrokes |
| `ModifierHoldDuration` | 100 ms | Delay after pressing modifiers |

## Architecture

```
SendSequenceCL/
├── API/                    # Public interfaces and types
│   ├── Input.cs           # Static facade (entry point)
│   ├── IVirtualMouse.cs
│   ├── IVirtualKeyboard.cs
│   ├── Configuration.cs
│   ├── MouseButton.cs
│   ├── VirtualKey.cs
│   └── Exceptions.cs
├── Core/                   # Business logic
│   ├── MouseController.cs
│   ├── KeyboardController.cs
│   ├── BezierCurveGenerator.cs
│   ├── ScreenUtilities.cs
│   └── KeyboardMapper.cs
└── Infrastructure/         # Low-level HID communication
    ├── HidDeviceManager.cs
    ├── HidCommunicator.cs
    ├── NativeMethods.cs
    ├── HidStructures.cs
    └── DriverConstants.cs
```

## Exception Hierarchy

- **SendSequenceException** (base)
  - **DriverNotFoundException** - HID driver not installed/connected
  - **DriverCommunicationException** - Communication error (retryable)
  - **InvalidCoordinateException** - Coordinates outside screen bounds
  - **InvalidKeyCodeException** - Unsupported key code

## License

See LICENSE file for details.

## Credits

Built on top of Tetherscript HID Virtual Driver Kit.
