# SendSequenceCL API Reference

**Version:** 2.0 (Humanization 3.0)
**Target:** .NET 8.0 (Windows)
**Driver:** Tetherscript HID Virtual Drivers
**Architecture:** Driver-based input emulation (no Windows API injection)

## Overview

SendSequenceCL provides hardware-level input automation through Tetherscript HID drivers:
- **Mouse:** Absolute positioning, relative movement, human-like curves (Bezier/Perlin Noise), overshoot simulation, scroll
- **Keyboard:** Text input, human-like typing with typos, key combinations, state checking
- **Sequence Builder:** Fluent API for complex automation workflows with guards and conditions
- **Configuration:** Runtime behavior customization with randomization ranges

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
**[ENHANCED]** Curved human-like movement with configurable algorithm (Bezier/Perlin Noise), overshoot simulation.
- `durationMs`: Override `Configuration.MouseMovementDuration` range (null = random from range)
- Algorithm: `Configuration.MotionAlgorithm` (Bezier or PerlinNoise)
- Overshoot: 20% chance if `Configuration.EnableMouseOvershoot = true`
- Randomization: `Configuration.CurveRandomization` for curve variation

**MoveRelative(int dx, int dy)**
```csharp
void MoveRelative(int dx, int dy)
```
**[ENHANCED]** Move relative to current position using relative mouse driver.
- **Auto-splits large movements** into chunks of max 127 pixels
- Range: dx/dy unlimited (chunked automatically)

**Async:**
```csharp
Task MoveHumanAsync(int x, int y, int? durationMs = null, CancellationToken ct = default)
```

#### Clicks & Buttons

**Click(MouseButton button)**
```csharp
void Click(MouseButton button)
```
Press and release button (duration: random from `Configuration.ClickDuration` range).

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

**ScrollHuman(int totalDelta, int minChunk = 1, int maxChunk = 3)**
```csharp
void ScrollHuman(int totalDelta, int minChunk = 1, int maxChunk = 3)
```
**[NEW]** Human-like scrolling with random chunk sizes and pauses.
- Breaks `totalDelta` into random chunks from `minChunk` to `maxChunk`
- Pauses between chunks using `Configuration.KeystrokeDelay` range
- Example: `ScrollHuman(10, 1, 3)` might scroll 2, 3, 1, 3, 1 with random pauses

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
**[ENHANCED]** Type arbitrary text with auto-shift for uppercase/symbols.
- Delay between keystrokes: random from `Configuration.KeystrokeDelay` range
- Unsupported chars: skip (or throw if `Configuration.ThrowOnUnsupportedChar = true`)
- **⚠️ Requires US keyboard layout** (other layouts may produce wrong characters)

**Exceptions:**
- `ArgumentNullException` - text is null
- `NotSupportedException` - Unsupported character (if `ThrowOnUnsupportedChar = true`)

**TypeTextHuman(string text)**
```csharp
void TypeTextHuman(string text)
```
**[NEW]** Maximum humanization: random delays, typo simulation, word pauses.
- Random keystroke delays from `Configuration.KeystrokeDelay` range
- Typo simulation: `Configuration.TypoChance` probability (wrong key → backspace → correct)
- Word boundaries: extra delay from `Configuration.InterWordDelay` range after space
- **⚠️ Requires US keyboard layout** (other layouts may produce wrong characters)

**Example:**
```csharp
Configuration.TypoChance = 0.05; // 5% typo chance
Configuration.InterWordDelay = (200, 400); // 200-400ms after spaces
Input.Keyboard.TypeTextHuman("Hello World"); // May type "Hellpo<backspace> World" with pauses
```

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

## Sequence Builder

### SequenceBuilder Class

**[NEW]** Fluent API for building complex automation sequences. All methods return `this` for chaining.

```csharp
public class SequenceBuilder
```

#### Basic Usage

```csharp
new SequenceBuilder()
    .MoveHuman(500, 500)
    .Wait(200)
    .Click(MouseButton.Left)
    .TypeText("Hello")
    .Run(); // Execute all actions
```

#### Mouse Methods (Fluent)

All mouse operations from `IVirtualMouse`:

- `SequenceBuilder MoveTo(int x, int y)`
- `SequenceBuilder MoveHuman(int x, int y, int? durationMs = null)`
- `SequenceBuilder MoveRelative(int dx, int dy)`
- `SequenceBuilder Click(MouseButton button)`
- `SequenceBuilder DoubleClick(MouseButton button)`
- `SequenceBuilder Down(MouseButton button)`
- `SequenceBuilder Up(MouseButton button)`
- `SequenceBuilder Drag(int startX, int startY, int endX, int endY, MouseButton button = Left, int? durationMs = null)`
- `SequenceBuilder Scroll(int delta)`
- `SequenceBuilder ScrollHuman(int totalDelta, int minChunk = 1, int maxChunk = 3)`

#### Keyboard Methods (Fluent)

All keyboard operations from `IVirtualKeyboard`:

- `SequenceBuilder TypeText(string text)`
- `SequenceBuilder TypeTextHuman(string text)`
- `SequenceBuilder KeyPress(VirtualKey key)`
- `SequenceBuilder Chord(VirtualKey modifier, VirtualKey key)`
- `SequenceBuilder Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key)`
- `SequenceBuilder KeyDown(VirtualKey key)`
- `SequenceBuilder KeyUp(VirtualKey key)`

#### Timing Methods

**Wait(int milliseconds)**
```csharp
SequenceBuilder Wait(int milliseconds)
```
Fixed delay.

**WaitRandom(int minMs, int maxMs)**
```csharp
SequenceBuilder WaitRandom(int minMs, int maxMs)
```
Random delay between min and max.

```csharp
.Wait(500)               // Always 500ms
.WaitRandom(200, 800)    // Random 200-800ms
```

#### Guard Methods (Conditionals)

**DoIf(Func&lt;bool&gt; condition, Action action)**
```csharp
SequenceBuilder DoIf(Func<bool> condition, Action action)
```
Execute action only if condition is true at runtime.

```csharp
.DoIf(() => DateTime.Now.Hour > 12,
      () => Input.Keyboard.TypeText("Good afternoon"))
```

**WaitUntil(Func&lt;bool&gt; condition, int timeoutMs = 5000, int checkIntervalMs = 50)**
```csharp
SequenceBuilder WaitUntil(Func<bool> condition, int timeoutMs = 5000, int checkIntervalMs = 50)
```
Block until condition is true or timeout expires.

**Exceptions:** `TimeoutException` if condition not met within timeout.

```csharp
.WaitUntil(() => Input.Keyboard.IsKeyDown(VirtualKey.LeftControl),
           timeoutMs: 10000,
           checkIntervalMs: 100)
```

**ClickIfAt(Point targetPosition, MouseButton button, int tolerance = 5)**
```csharp
SequenceBuilder ClickIfAt(Point targetPosition, MouseButton button, int tolerance = 5)
```
Click only if cursor is within `tolerance` pixels of `targetPosition`.

```csharp
.ClickIfAt(new Point(500, 500), MouseButton.Left, tolerance: 10)
```

#### Execution & Control

**Run()**
```csharp
void Run()
```
Execute all accumulated actions in order.

**Clear()**
```csharp
SequenceBuilder Clear()
```
Remove all actions from sequence.

**Count**
```csharp
int Count { get; }
```
Number of actions in sequence.

#### Complex Example

```csharp
var sequence = new SequenceBuilder()
    // Open Run dialog
    .Chord(VirtualKey.Windows, VirtualKey.R)
    .Wait(500)

    // Type "notepad" with human-like typing
    .TypeTextHuman("notepad")
    .KeyPress(VirtualKey.Enter)
    .Wait(1500)

    // Wait for Notepad window to appear (pseudo-check)
    .WaitRandom(500, 1000)

    // Type text with random delay
    .TypeTextHuman("This is a test with typos and natural delays.")

    // Conditional: Save if Ctrl is held
    .DoIf(() => Input.Keyboard.IsKeyDown(VirtualKey.LeftControl),
          () => {
              Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.S);
              Input.Wait(500);
          })

    // Close without saving
    .Chord(VirtualKey.LeftAlt, VirtualKey.F4)
    .Wait(300)
    .KeyPress(VirtualKey.N)

    // Execute
    .Run();
```

---

## Configuration

### Configuration (Static Class)

```csharp
public static class Configuration
```

All properties thread-safe. Changes apply immediately to subsequent operations.
**[MAJOR CHANGE]** Most timing properties now use `(int Min, int Max)` tuples for randomization.

#### Mouse Configuration

**MouseMovementDuration** ((int Min, int Max), default: (300, 500))
- Duration range for `MoveHuman()` in milliseconds
- Random value picked from [Min, Max] for each movement
- Range: Min ≥ 0, Max ≥ Min (0 = instant, 100-300 fast, 300-500 natural, 500+ slow)

**Exceptions:** `ArgumentOutOfRangeException` if Min < 0 or Max < Min.

**MotionAlgorithm** (MouseMotionAlgorithm, default: Bezier)
```csharp
public enum MouseMotionAlgorithm { Bezier, PerlinNoise }
```
- `Bezier`: Smooth, predictable curves (classic)
- `PerlinNoise`: Organic, slightly chaotic paths (more human-like)

**CurveRandomization** (double, default: 0.10)
- Randomization in curve control points (0.0-1.0)
- Guide: 0 = deterministic, 0.05-0.15 natural, 0.5+ erratic

**EnableMouseOvershoot** (bool, default: true)
- If true, `MoveHuman` occasionally overshoots target and corrects back (20% chance)
- Simulates natural human imprecision

**OvershootAmount** ((double Min, double Max), default: (0.02, 0.07))
- Overshoot distance as percentage of total distance (0.0-1.0)
- Random value from [Min, Max] when overshoot occurs
- Example: (0.05, 0.10) = 5-10% overshoot

**PerlinNoiseIntensity** ((int Min, int Max), default: (5, 15))
- Intensity range of Perlin noise when `MotionAlgorithm = PerlinNoise`
- Higher values = more chaotic/jittery movement
- Range: Min ≥ 0, Max ≥ Min

**MinCurveSteps** (int, default: 10)
- Minimum steps in movement curve
- Range: > 0
- Guide: 1-5 choppy, 10-20 smooth, 20+ very smooth

**MillisecondsPerCurveStep** (int, default: 10)
- Target duration per step in ms
- Range: > 0
- Calculation: `steps = max(MinCurveSteps, duration / MillisecondsPerCurveStep)`

**ClickDuration** ((int Min, int Max), default: (30, 70))
- Delay range between button down/up in milliseconds
- Random value from [Min, Max] for each click
- Range: Min ≥ 0, Max ≥ Min (0 = instant, 30-100 normal, 100-200 slow)

**DragPauseMs** ((int Min, int Max), default: (40, 80))
- Pause duration range before/after drag in milliseconds
- Random value from [Min, Max] for each drag

#### Keyboard Configuration

**KeystrokeDelay** ((int Min, int Max), default: (50, 120))
- Delay range between keystrokes and key press/release in milliseconds
- Random value from [Min, Max] for each keystroke
- Range: Min ≥ 0, Max ≥ Min (0 = robotic, 30-50 fast, 50-100 normal, 100+ slow)

**ModifierHoldDuration** ((int Min, int Max), default: (80, 150))
- Delay range after pressing modifier before main key in milliseconds
- Random value from [Min, Max] for each chord
- Range: Min ≥ 0, Max ≥ Min (0 = risky, 50-150 natural, 150+ slow)

**InterWordDelay** ((int Min, int Max), default: (100, 250))
- **[NEW]** Additional delay range after space character (word boundary)
- Random value from [Min, Max] after each space
- Used by `TypeTextHuman`

**TypoChance** (double, default: 0.01)
- **[NEW]** Probability of typing mistake during `TypeTextHuman` (0.0-1.0)
- After correct key: wrong key → backspace → continue
- Range: 0.0-1.0 (0 = no typos, 0.01 = 1%, 0.1 = 10%)

**ThrowOnUnsupportedChar** (bool, default: false)
- If true: `TypeText()` throws `NotSupportedException` on unsupported character
- If false: silently skip unsupported characters

#### Configuration Example

```csharp
// Slower, more varied movement
Configuration.MouseMovementDuration = (500, 800);
Configuration.CurveRandomization = 0.15;
Configuration.MotionAlgorithm = MouseMotionAlgorithm.PerlinNoise;
Configuration.EnableMouseOvershoot = true;
Configuration.OvershootAmount = (0.03, 0.08);

// Slower, more human typing with typos
Configuration.KeystrokeDelay = (80, 150);
Configuration.InterWordDelay = (150, 300);
Configuration.TypoChance = 0.03; // 3% typo rate

// Faster clicks
Configuration.ClickDuration = (20, 40);
```

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

### MouseMotionAlgorithm

```csharp
public enum MouseMotionAlgorithm
{
    Bezier,       // Smooth, predictable Bezier curves
    PerlinNoise   // Organic, slightly chaotic Perlin noise-based movement
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

// Human-like move (random 300-500ms, Bezier curve, possible overshoot)
Input.Mouse.MoveHuman(800, 600);

// Custom duration
Input.Mouse.MoveHuman(100, 100, durationMs: 1000);

// Click (random 30-70ms between down/up)
Input.Mouse.Click(MouseButton.Left);

// Drag
Input.Mouse.Drag(100, 100, 500, 500);

// Scroll (instant)
Input.Mouse.Scroll(3);  // Down

// Human scroll (chunked with pauses)
Input.Mouse.ScrollHuman(10, minChunk: 1, maxChunk: 3);

// Relative movement (auto-splits large distances)
Input.Mouse.MoveRelative(500, -300);
```

### Basic Keyboard

```csharp
using SendSequenceCL;

// Type text (random delays)
Input.Keyboard.TypeText("Hello World!");

// Human typing (with typos and word pauses)
Configuration.TypoChance = 0.02;
Configuration.InterWordDelay = (150, 300);
Input.Keyboard.TypeTextHuman("Hello World!");

// Single key
Input.Keyboard.KeyPress(VirtualKey.Enter);

// Shortcut
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C);

// Multiple modifiers
Input.Keyboard.Chord(
    new[] { VirtualKey.LeftControl, VirtualKey.LeftShift },
    VirtualKey.Escape
);

// Check state
bool ctrlPressed = Input.Keyboard.IsKeyDown(VirtualKey.LeftControl);
```

### Sequence Builder

```csharp
using SendSequenceCL;

// Simple sequence
new SequenceBuilder()
    .MoveHuman(500, 500)
    .Wait(200)
    .Click(MouseButton.Left)
    .TypeTextHuman("Hello World")
    .Run();

// Complex workflow with conditionals
var sequence = new SequenceBuilder()
    // Open program
    .Chord(VirtualKey.Windows, VirtualKey.R)
    .Wait(500)
    .TypeText("notepad")
    .KeyPress(VirtualKey.Enter)
    .WaitRandom(1000, 2000)

    // Type with humanization
    .TypeTextHuman("This text has natural delays and occasional typos.")

    // Conditional click (only if cursor at expected position)
    .ClickIfAt(new Point(800, 600), MouseButton.Left, tolerance: 10)

    // Wait for condition (e.g., wait for Ctrl to be pressed)
    .WaitUntil(() => Input.Keyboard.IsKeyDown(VirtualKey.LeftControl),
               timeoutMs: 5000)

    // Conditional action
    .DoIf(() => DateTime.Now.Hour > 12,
          () => Input.Keyboard.TypeText(" (afternoon)"))

    // Execute all
    .Run();

// Reusable sequences
var builder = new SequenceBuilder();
for (int i = 0; i < 5; i++)
{
    builder.MoveHuman(100 + i * 100, 100)
           .Click(MouseButton.Left)
           .WaitRandom(200, 500);
}
builder.Run();
builder.Clear(); // Reuse
```

### Humanization Configuration

```csharp
using SendSequenceCL;

// Maximum humanization
Configuration.MouseMovementDuration = (400, 700);
Configuration.MotionAlgorithm = MouseMotionAlgorithm.PerlinNoise;
Configuration.EnableMouseOvershoot = true;
Configuration.OvershootAmount = (0.03, 0.10);
Configuration.CurveRandomization = 0.15;
Configuration.PerlinNoiseIntensity = (8, 18);

Configuration.KeystrokeDelay = (70, 150);
Configuration.InterWordDelay = (150, 350);
Configuration.TypoChance = 0.03; // 3% typo chance

// Test
Input.Mouse.MoveHuman(800, 600); // Perlin noise path, possible overshoot
Input.Keyboard.TypeTextHuman("This feels very human!"); // Typos, word pauses
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
- Mouse Relative (0x0005): Relative movement (auto-chunking)
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
- **Auto-splits** movements > 127 into multiple updates

### Movement Algorithms

**Bezier Curve:**
1. Calculate steps: `max(MinCurveSteps, duration / MillisecondsPerCurveStep)`
2. Generate control points with `CurveRandomization`
3. Interpolate cubic Bezier curve
4. Move along curve with `delayPerStep = duration / steps`
5. 20% chance of overshoot if enabled

**Perlin Noise:**
1. Calculate linear path
2. Add multi-frequency sine wave noise:
   - Base frequency: `2π / steps`
   - 3 frequencies: 1x (50%), 2.3x (30%), 5.7x (20%)
   - Intensity: random from `PerlinNoiseIntensity` range
3. Apply envelope (sine wave from 0 to π) to smooth start/end
4. Add perpendicular offset to path
5. 20% chance of overshoot if enabled

**Overshoot Simulation:**
1. When triggered (20% chance):
2. Calculate overshoot distance: `distance * random(OvershootAmount.Min, OvershootAmount.Max)`
3. Generate overshoot point beyond target
4. Move to overshoot point
5. Immediately move back to target (correction)

### Character Mapping

**TypeText() / TypeTextHuman() supports:**
- a-z, A-Z (auto-shift for uppercase)
- 0-9, basic symbols (!@#$%^&*()etc.)
- Whitespace (space, tab, newline)

**⚠️ Keyboard Layout Dependency:**
- **Current Implementation:** Hardcoded for US keyboard layout
- **Non-US layouts:** May produce wrong characters (e.g., QWERTZ, AZERTY)
- **Workaround:** Switch to US layout before automation
- **Future:** Layout detection/adaptation planned

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
- `SequenceBuilder` instances (one per thread)

### Performance

**Typical latencies:**
- Instant move: <1ms
- Human move (300-500ms): ~300-500ms + curve computation (<15ms)
- Key press: 50-120ms (KeystrokeDelay range)
- Click: 30-70ms (ClickDuration range)
- Typo simulation: ~200-400ms (extra keystroke + backspace)

---

## Requirements

- **.NET 8.0** (Windows)
- **Tetherscript HID Virtual Drivers** installed
- **Administrator privileges** (for driver access and input blocking hooks)
- **Windows OS** (driver dependency)
- **US Keyboard Layout** (for correct TypeText character mapping)

---

## Error Handling Pattern

```csharp
using SendSequenceCL;

try
{
    // First access may throw
    Input.Mouse.MoveTo(100, 100);
    Input.Keyboard.TypeText("test");
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
catch (TimeoutException ex)
{
    Console.WriteLine("Sequence timeout: " + ex.Message);
    // WaitUntil condition not met
}
```

---

## Design Philosophy

1. **Driver-first:** All input emulation through HID drivers (not SendInput/PostMessage)
2. **Legitimacy:** Hardware-level simulation indistinguishable from physical devices
3. **Zero dependencies:** BCL only, no NuGet packages
4. **Simplicity:** Static facade, minimal configuration
5. **Human simulation:** Multiple algorithms (Bezier/Perlin Noise), randomization, overshoot, typos
6. **Fluent API:** Sequence builder for complex workflows with natural syntax

---

## Limitations

- **Windows only** (Tetherscript driver dependency)
- **Single display** optimization (multi-monitor requires coordinate adjustment)
- **No input reading** (except `IsKeyDown` for state checking)
- **US keyboard layout required** for TypeText (other layouts produce wrong characters)
- **Basic character set** (TypeText limited to common ASCII/Latin-1)
- **Sequential operations** (SequenceBuilder executes actions in order, no built-in parallelism)

---

## Version History

**2.0 (Humanization 3.0) - Current**
- **Humanization 3.0:**
  - Range-based randomization for all timing parameters (tuples)
  - Perlin Noise movement algorithm (organic paths)
  - Mouse overshoot simulation (20% chance)
  - `TypeTextHuman` with typo simulation and word pauses
  - `ScrollHuman` with chunked scrolling
  - Enhanced `MoveRelative` with auto-chunking
- **Sequence Builder:**
  - Fluent API for complex workflows
  - Guards: `DoIf`, `WaitUntil`, `ClickIfAt`
  - Timing: `Wait`, `WaitRandom`
  - All mouse/keyboard operations chainable
- **Configuration:**
  - `MouseMotionAlgorithm` enum (Bezier/PerlinNoise)
  - `EnableMouseOvershoot`, `OvershootAmount`
  - `PerlinNoiseIntensity`
  - `InterWordDelay`, `TypoChance`
  - All timing configs now use `(Min, Max)` ranges

**1.0**
- Initial release
- Mouse: absolute, relative, scroll, human-like movement
- Keyboard: text input, combinations, state checking
- Configuration: runtime behavior customization
- Async support for long-running operations
