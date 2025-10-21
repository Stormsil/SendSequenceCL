# SendSequenceCL API Reference

**Version:** 1.0
**Target:** .NET 8.0 (Windows)
**Driver:** Tetherscript HID Virtual Drivers
**Architecture:** Driver-based input emulation (no Windows API injection)

## Overview

SendSequenceCL provides hardware-level input automation through Tetherscript HID drivers:
- Mouse: Absolute positioning, relative movement, human-like curves, scroll
- Keyboard: Text input, key combinations, state checking
- Configuration: Runtime behavior customization

## Entry Point

### Input (Static Class)

Main API facade. Thread-safe lazy initialization.

```csharp
namespace SendSequenceCL;
public static class Input
```

**Properties:**
- `IVirtualMouse Mouse` - Mouse automation interface (lazy init on first access)
- `IVirtualKeyboard Keyboard` - Keyboard automation interface (lazy init on first access)

**Methods:**
- `void Wait(int milliseconds)` - Pause execution
- `void Dispose()` - Release HID driver handles (call on shutdown)

**Exceptions:**
- `DriverNotFoundException` - Tetherscript driver not found on first access
- `DriverCommunicationException` - Driver communication failed on first access
- `ArgumentOutOfRangeException` - Wait: milliseconds < 0

---

## Mouse API

### IVirtualMouse Interface

```csharp
public interface IVirtualMouse
```

#### Position & Movement

**GetPosition()**
```csharp
Point GetPosition()
```
Returns current cursor position (X, Y coordinates in pixels).

**MoveTo(int x, int y)**
```csharp
void MoveTo(int x, int y)
```
Instant teleport to coordinates (0,0 = top-left).

**Exceptions:** `InvalidCoordinateException` if out of screen bounds.

**MoveHuman(int x, int y, int? durationMs = null)**
```csharp
void MoveHuman(int x, int y, int? durationMs = null)
```
Curved human-like movement with Bézier interpolation.
- `durationMs`: Override `Configuration.MouseMovementDuration` (null = use default)

**MoveRelative(int dx, int dy)**
```csharp
void MoveRelative(int dx, int dy)
```
Move relative to current position using relative mouse driver.
- Range: dx/dy ∈ [-127, 127]
- **Exceptions:** `ArgumentOutOfRangeException` if outside range

**Async:**
```csharp
Task MoveHumanAsync(int x, int y, int? durationMs = null, CancellationToken ct = default)
```

#### Clicks & Buttons

**Click(MouseButton button)**
```csharp
void Click(MouseButton button)
```
Press and release button (duration: `Configuration.ClickDuration`).

**DoubleClick(MouseButton button)**
```csharp
void DoubleClick(MouseButton button)
```
Two clicks with delay between.

**Down(MouseButton button)**
```csharp
void Down(MouseButton button)
```
Press button down (hold). Must pair with `Up()`.

**Up(MouseButton button)**
```csharp
void Up(MouseButton button)
```
Release button.

**Drag(int startX, int startY, int endX, int endY, MouseButton button = Left, int? durationMs = null)**
```csharp
void Drag(int startX, int startY, int endX, int endY,
          MouseButton button = MouseButton.Left, int? durationMs = null)
```
Move to start, press button, move to end (human-like), release.

**Async:**
```csharp
Task DragAsync(int startX, int startY, int endX, int endY,
               MouseButton button = MouseButton.Left, int? durationMs = null,
               CancellationToken ct = default)
```

#### Scroll

**Scroll(int delta)**
```csharp
void Scroll(int delta)
```
Scroll wheel using joystick driver (most legitimate method).
- Positive: scroll down
- Negative: scroll up
- Implementation: `wheel = 16384 + (delta * 100)`, clamped to [0, 32767]

---

## Keyboard API

### IVirtualKeyboard Interface

```csharp
public interface IVirtualKeyboard
```

#### Text Input

**TypeText(string text)**
```csharp
void TypeText(string text)
```
Type arbitrary text with auto-shift for uppercase/symbols.
- Delay between keystrokes: `Configuration.KeystrokeDelay`
- Unsupported chars: skip (or throw if `Configuration.ThrowOnUnsupportedChar = true`)

**Exceptions:**
- `ArgumentNullException` - text is null
- `NotSupportedException` - Unsupported character (if `ThrowOnUnsupportedChar = true`)

#### Key Operations

**KeyPress(VirtualKey key)**
```csharp
void KeyPress(VirtualKey key)
```
Press and release single key (atomic). Cannot use with modifiers.

**Exceptions:** `InvalidKeyCodeException` if key is a modifier (use `Chord` instead).

**KeyDown(VirtualKey key)**
```csharp
void KeyDown(VirtualKey key)
```
Press key/modifier down. Max 6 non-modifier keys simultaneously.

**Exceptions:** `InvalidOperationException` if >6 keys pressed.

**KeyUp(VirtualKey key)**
```csharp
void KeyUp(VirtualKey key)
```
Release key/modifier. Safe even if not pressed.

#### Combinations

**Chord(VirtualKey modifier, VirtualKey key)**
```csharp
void Chord(VirtualKey modifier, VirtualKey key)
```
Single modifier + key (e.g., Ctrl+C, Alt+F4).

**Chord(IEnumerable&lt;VirtualKey&gt; modifiers, VirtualKey key)**
```csharp
void Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key)
```
Multiple modifiers + key (e.g., Ctrl+Shift+Esc).

**Exceptions:**
- `ArgumentNullException` - modifiers is null
- `ArgumentException` - modifiers is empty
- `InvalidKeyCodeException` - Non-modifier in modifiers list

#### State Checking

**IsKeyDown(VirtualKey key)**
```csharp
bool IsKeyDown(VirtualKey key)
```
Check if physical key currently pressed (uses Windows `GetAsyncKeyState`).

---

## Configuration

### Configuration (Static Class)

```csharp
public static class Configuration
```

All properties thread-safe. Changes apply immediately to subsequent operations.

#### Mouse Configuration

**MouseMovementDuration** (int, default: 400)
- Duration for `MoveHuman()` in milliseconds
- Range: ≥ 0 (0 = instant like `MoveTo`)
- Guide: 100-300 fast, 300-500 natural, 500+ slow

**CurveRandomization** (double, default: 0.10)
- Randomization in Bézier curves (0.0-1.0)
- Guide: 0 = deterministic, 0.05-0.15 natural, 0.5+ erratic

**MinCurveSteps** (int, default: 10)
- Minimum steps in movement curve
- Range: > 0
- Guide: 1-5 choppy, 10-20 smooth, 20+ very smooth

**MillisecondsPerCurveStep** (int, default: 10)
- Target duration per step in ms
- Range: > 0
- Calculation: `steps = max(MinCurveSteps, duration / MillisecondsPerCurveStep)`
- Guide: 1-5 fast updates, 10-20 normal, 20+ slow

**ClickDuration** (int, default: 50)
- Delay between button down/up in milliseconds
- Range: ≥ 0 (0 = instant, 30-100 normal, 100-200 slow)

**DragPauseMs** (int, default: 50)
- Pause before/after drag in milliseconds
- Range: ≥ 0

#### Keyboard Configuration

**KeystrokeDelay** (int, default: 75)
- Delay between keystrokes and key press/release in milliseconds
- Range: ≥ 0 (0 = robotic, 30-50 fast, 50-100 normal, 100+ slow)

**ModifierHoldDuration** (int, default: 100)
- Delay after pressing modifier before main key in milliseconds
- Range: ≥ 0 (0 = risky, 50-150 natural, 150+ slow)

**ThrowOnUnsupportedChar** (bool, default: false)
- If true: `TypeText()` throws `NotSupportedException` on unsupported character
- If false: silently skip unsupported characters

---

## Enumerations

### MouseButton

```csharp
public enum MouseButton : byte
{
    Left = 1,    // Primary button
    Right = 2,   // Secondary (context menu)
    Middle = 3   // Scroll wheel click
}
```

### VirtualKey

```csharp
public enum VirtualKey : byte
```

**Letters:** A-Z (0x04-0x1D)
**Numbers:** D0-D9 (0x27, 0x1E-0x26)
**Control:** Enter, Escape, Backspace, Tab, Space
**Symbols:** Minus, Equals, LeftBracket, RightBracket, Backslash, Semicolon, Apostrophe, Grave, Comma, Period, Slash
**Function:** F1-F24
**System:** PrintScreen, ScrollLock, Pause, Insert, Home, PageUp, Delete, End, PageDown
**Arrows:** Up, Down, Left, Right
**Numpad:** NumLock, Numpad0-9, NumpadPlus, NumpadMinus, NumpadMultiply, NumpadDivide, NumpadPeriod, NumpadEnter
**Locks:** CapsLock, NumLock, ScrollLock
**Modifiers:** LeftControl, RightControl, LeftShift, RightShift, LeftAlt, RightAlt, Windows

Values are HID Usage IDs (USB HID Usage Table, page 0x07).

---

## Exceptions

### Hierarchy

```
Exception
└─ SendSequenceException
   ├─ DriverNotFoundException
   ├─ DriverCommunicationException
   ├─ InvalidCoordinateException
   └─ InvalidKeyCodeException
```

**DriverNotFoundException**
- Tetherscript HID driver not found/not installed
- Thrown on first access to `Input.Mouse`/`Input.Keyboard`

**DriverCommunicationException**
- HID communication failed (transient, can retry)

**InvalidCoordinateException**
- Coordinates outside screen bounds

**InvalidKeyCodeException**
- Unsupported VirtualKey or incorrect usage (e.g., modifier in `KeyPress`)

---

## Usage Examples

### Basic Mouse

```csharp
using SendSequenceCL;

// Instant move
Input.Mouse.MoveTo(500, 300);

// Human-like move (400ms default)
Input.Mouse.MoveHuman(800, 600);

// Custom duration
Input.Mouse.MoveHuman(100, 100, durationMs: 1000);

// Click
Input.Mouse.Click(MouseButton.Left);

// Drag
Input.Mouse.Drag(100, 100, 500, 500);

// Scroll
Input.Mouse.Scroll(3);  // Down
Input.Mouse.Scroll(-3); // Up

// Relative movement
Input.Mouse.MoveRelative(50, -20);
```

### Basic Keyboard

```csharp
using SendSequenceCL;

// Type text
Input.Keyboard.TypeText("Hello World!");

// Single key
Input.Keyboard.KeyPress(VirtualKey.Enter);

// Shortcut (single modifier)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C);

// Multiple modifiers
Input.Keyboard.Chord(
    new[] { VirtualKey.LeftControl, VirtualKey.LeftShift },
    VirtualKey.Escape
);

// Manual control
Input.Keyboard.KeyDown(VirtualKey.LeftShift);
Input.Keyboard.KeyPress(VirtualKey.A);
Input.Keyboard.KeyUp(VirtualKey.LeftShift);

// Check state
bool ctrlPressed = Input.Keyboard.IsKeyDown(VirtualKey.LeftControl);
```

### Configuration

```csharp
using SendSequenceCL;

// Adjust timings
Configuration.MouseMovementDuration = 600;  // Slower movement
Configuration.KeystrokeDelay = 50;          // Faster typing
Configuration.CurveRandomization = 0.2;     // More random curves

// Curve calculation
Configuration.MinCurveSteps = 15;
Configuration.MillisecondsPerCurveStep = 8;

// Error handling for unsupported chars
Configuration.ThrowOnUnsupportedChar = true;
try
{
    Input.Keyboard.TypeText("™");  // Throws if not supported
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"Cannot type: {ex.Message}");
}
```

### Async Operations

```csharp
using SendSequenceCL;
using System.Threading;
using System.Threading.Tasks;

async Task AutomateAsync(CancellationToken ct)
{
    // Async human-like movement
    await Input.Mouse.MoveHumanAsync(500, 300, cancellationToken: ct);

    // Async drag
    await Input.Mouse.DragAsync(100, 100, 500, 500,
                                MouseButton.Left,
                                durationMs: 800,
                                cancellationToken: ct);
}
```

### Cleanup

```csharp
// On application shutdown
Input.Dispose();
```

---

## Technical Details

### Driver Architecture

**HID Drivers Used:**
- Mouse Absolute (0x0002): Absolute positioning
- Mouse Relative (0x0005): Relative movement
- Keyboard (0x0003): Key input
- Joystick (0x0001): Scroll wheel emulation

**Vendor ID:** 0xF00F (Tetherscript)

**Report Structure:**
- Mouse: X/Y coordinates (0-32767), button flags (3 bits)
- Keyboard: Modifier byte, 6 key slots (HID Usage IDs)
- Joystick: 9 axes + hat + 16 buttons (wheel axis used for scroll)

### Coordinate System

**Mouse Absolute:**
- Driver range: 0-32767 (center: 16384)
- Screen mapping: Linear transformation from screen pixels
- Origin: Top-left (0, 0)

**Mouse Relative:**
- Range: -127 to 127 per update
- Accumulative movement

### Movement Algorithm

**Bézier Curve Generation:**
1. Calculate steps: `max(MinCurveSteps, duration / MillisecondsPerCurveStep)`
2. Generate control points with randomization
3. Interpolate cubic Bézier curve
4. Move along curve with `delayPerStep = duration / steps`

### Character Mapping

**TypeText() supports:**
- a-z, A-Z (auto-shift for uppercase)
- 0-9, basic symbols (!@#$%^&*()etc.)
- Whitespace (space, tab, newline)

**Not supported:**
- Unicode beyond basic ASCII/Latin-1
- Complex symbols requiring AltGr
- Emoji

### Thread Safety

**Thread-safe:**
- `Input` static class (lazy singleton init)
- `Configuration` properties (atomic reads/writes)

**Not thread-safe:**
- Individual mouse/keyboard operations (serialize manually if needed)

### Performance

**Typical latencies:**
- Instant move: <1ms
- Human move (400ms): ~400ms + curve computation (<10ms)
- Key press: 75ms (KeystrokeDelay)
- Click: 50ms (ClickDuration)

---

## Requirements

- **.NET 8.0** (Windows)
- **Tetherscript HID Virtual Drivers** installed
- **Administrator privileges** (for driver access)
- **Windows OS** (driver dependency)

---

## Error Handling Pattern

```csharp
using SendSequenceCL;

try
{
    // First access may throw
    Input.Mouse.MoveTo(100, 100);
}
catch (DriverNotFoundException ex)
{
    Console.WriteLine("Driver not installed: " + ex.Message);
    // Prompt user to install Tetherscript drivers
}
catch (DriverCommunicationException ex)
{
    Console.WriteLine("Communication error: " + ex.Message);
    // Retry operation
}
catch (InvalidCoordinateException ex)
{
    Console.WriteLine("Invalid coordinates: " + ex.Message);
    // Validate screen bounds
}
```

---

## Design Philosophy

1. **Driver-first:** All input emulation through HID drivers (not SendInput/PostMessage)
2. **Legitimacy:** Hardware-level simulation indistinguishable from physical devices
3. **Zero dependencies:** BCL only, no NuGet packages
4. **Simplicity:** Static facade, minimal configuration
5. **Human simulation:** Bézier curves, randomization, realistic timings

---

## Limitations

- **Windows only** (Tetherscript driver dependency)
- **Single display** optimization (multi-monitor requires coordinate adjustment)
- **No input reading** (except `IsKeyDown` for state checking)
- **Sequential operations** (no built-in parallel input sequences)
- **Basic character set** (TypeText limited to common ASCII/Latin-1)

---

## Version History

**1.0 (Current)**
- Initial release
- Mouse: absolute, relative, scroll, human-like movement
- Keyboard: text input, combinations, state checking
- Configuration: runtime behavior customization
- Async support for long-running operations
