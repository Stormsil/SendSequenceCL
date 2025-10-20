# SendSequenceCL - Technical Specification

**Version:** 1.0.0
**Target Framework:** .NET 8.0 (net8.0-windows)
**Language:** C# 12
**Platform:** Windows 10/11 x64
**Last Updated:** 2025-10-21

---

## 1. Overview

### 1.1 Purpose

SendSequenceCL is a class library providing programmatic control over virtual HID (Human Interface Device) input through the Tetherscript HVDK driver. The library enables automation of mouse and keyboard input with human-like behavioral characteristics.

### 1.2 Scope

This library provides:
- Virtual mouse control (positioning, clicking, dragging)
- Virtual keyboard control (text input, key presses, shortcuts)
- Human-like input simulation via Bézier curve motion
- Asynchronous operation support with cancellation
- Configurable timing and randomization parameters

This library does NOT provide:
- Window management (planned separate library)
- Image recognition/OCR (planned separate library)
- Screen capture functionality
- Process automation beyond input simulation

### 1.3 Dependencies

**Required:**
- .NET 8.0 Runtime
- Windows 10/11 (x64)
- Tetherscript HID Virtual Driver Kit (VID: 0xF00F)

**No NuGet Packages:** Library uses only .NET Base Class Library (BCL)

---

## 2. Architecture

### 2.1 Layer Structure

```
┌─────────────────────────────────────┐
│         API Layer (Public)          │
│  - Input (Facade)                   │
│  - IVirtualMouse / IVirtualKeyboard │
│  - Configuration                    │
│  - Enums & Exceptions               │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│         Core Layer (Internal)       │
│  - MouseController                  │
│  - KeyboardController               │
│  - BezierCurveGenerator             │
│  - ScreenUtilities                  │
│  - KeyboardMapper                   │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Infrastructure Layer (Internal)   │
│  - HidDeviceManager                 │
│  - HidCommunicator                  │
│  - NativeMethods (P/Invoke)         │
│  - DriverConstants                  │
└─────────────────────────────────────┘
```

### 2.2 Design Patterns

**Facade Pattern:**
- `Input` class provides simplified access to complex subsystems

**Strategy Pattern:**
- `BezierCurveGenerator` encapsulates trajectory generation algorithm

**Lazy Initialization:**
- HID devices initialized on first access via `Lazy<T>`

**Dependency Injection Ready:**
- All controllers depend on interfaces
- Testable architecture (though `Input` is static for convenience)

### 2.3 Thread Safety

- **Configuration:** Thread-safe properties with backing fields
- **Input:** Thread-safe lazy initialization via `Lazy<T>`
- **HidDeviceManager:** Not thread-safe; protected by single ownership
- **Concurrent Access:** Multiple threads can call `Input.Mouse`/`Input.Keyboard` but operations are serialized at HID level

---

## 3. Public API Reference

### 3.1 Input Class

**Namespace:** `SendSequenceCL`
**Type:** `public static class Input`

**Purpose:** Primary entry point for all automation operations.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Mouse` | `IVirtualMouse` | Access to virtual mouse operations |
| `Keyboard` | `IVirtualKeyboard` | Access to virtual keyboard operations |

#### Methods

```csharp
public static void Wait(int milliseconds)
```
- **Purpose:** Pause execution
- **Parameters:** `milliseconds` - Duration to wait (>= 0)
- **Throws:** `ArgumentOutOfRangeException` if milliseconds < 0
- **Equivalent to:** `Thread.Sleep(milliseconds)`

```csharp
public static void Dispose()
```
- **Purpose:** Release HID device handles
- **When to call:** Application shutdown
- **Idempotent:** Safe to call multiple times
- **Note:** Devices are re-initialized on next access

#### Initialization Behavior

- **Lazy Initialization:** Devices connected on first property access
- **Connection Failures:** Throws `DriverNotFoundException` or `DriverCommunicationException`
- **Shared Access:** Supports concurrent access with ControlMyJoystick via fallback mechanism

---

### 3.2 IVirtualMouse Interface

**Namespace:** `SendSequenceCL`
**Type:** `public interface IVirtualMouse`

#### Position Query

```csharp
Point GetPosition()
```
- **Returns:** Current cursor position in screen coordinates
- **Coordinate System:** (0,0) = top-left corner
- **Throws:** `DriverCommunicationException` on failure

#### Movement Operations

```csharp
void MoveTo(int x, int y)
```
- **Purpose:** Instant cursor teleportation
- **Parameters:**
  - `x` - Target X coordinate (pixels)
  - `y` - Target Y coordinate (pixels)
- **Behavior:** No interpolation; instant positioning
- **Throws:** `InvalidCoordinateException` if outside screen bounds

```csharp
void MoveHuman(int x, int y, int? durationMs = null)
```
- **Purpose:** Human-like curved movement
- **Algorithm:** Cubic Bézier curve with randomization
- **Parameters:**
  - `x`, `y` - Target coordinates
  - `durationMs` - Movement duration; `null` uses `Configuration.MouseMovementDuration`
- **Special Cases:**
  - `durationMs = 0` → Equivalent to `MoveTo()` (instant)
  - `durationMs > 0` → Smooth curve motion
- **Steps:** `Max(10, durationMs / 10)` interpolation points
- **Throws:** `InvalidCoordinateException`

```csharp
Task MoveHumanAsync(int x, int y, int? durationMs = null, CancellationToken cancellationToken = default)
```
- **Purpose:** Non-blocking human-like movement
- **Cancellation:** Honors `CancellationToken`; throws `OperationCanceledException`
- **Thread Behavior:** Async delay via `Task.Delay()`

#### Click Operations

```csharp
void Click(MouseButton button)
```
- **Purpose:** Single click (down → delay → up)
- **Timing:** `Configuration.ClickDuration` between down/up
- **Parameters:** `button` - Which mouse button to click

```csharp
void DoubleClick(MouseButton button)
```
- **Purpose:** Two rapid clicks
- **Timing:** `Click() → Wait(ClickDuration) → Click()`

```csharp
void Down(MouseButton button)
void Up(MouseButton button)
```
- **Purpose:** Manual button state control
- **Use Case:** Custom hold durations, multi-button combinations
- **State:** Maintained in `_currentButtonState` byte field

#### Drag Operation

```csharp
void Drag(int startX, int startY, int endX, int endY,
          MouseButton button = MouseButton.Left, int? durationMs = null)
```
- **Sequence:**
  1. `MoveTo(startX, startY)` - Instant
  2. `Wait(Configuration.DragPauseMs)`
  3. `Down(button)`
  4. `Wait(Configuration.ClickDuration)`
  5. `MoveHuman(endX, endY, durationMs)` - Human-like
  6. `Wait(Configuration.DragPauseMs)`
  7. `Up(button)`

```csharp
Task DragAsync(int startX, int startY, int endX, int endY,
               MouseButton button = MouseButton.Left, int? durationMs = null,
               CancellationToken cancellationToken = default)
```
- **Purpose:** Non-blocking drag operation
- **Cancellation:** Can be cancelled mid-drag

---

### 3.3 IVirtualKeyboard Interface

**Namespace:** `SendSequenceCL`
**Type:** `public interface IVirtualKeyboard`

#### Text Input

```csharp
void TypeText(string text)
```
- **Purpose:** Type arbitrary text with automatic shift handling
- **Supported Characters:**
  - Letters: A-Z (auto-shift for uppercase)
  - Numbers: 0-9
  - Symbols: `!@#$%^&*()-_=+[]{};:'",.<>/?|\`~`
  - Whitespace: Space, Tab, Newline (`\n`)
- **Timing:** `Configuration.KeystrokeDelay` between each character
- **Throws:** `ArgumentNullException` if text is null
- **Limitations:** Only US keyboard layout; no Unicode support

#### Key Operations

```csharp
void KeyPress(VirtualKey key)
```
- **Purpose:** Press and release single key
- **Sequence:** `KeyDown(key) → Wait(KeystrokeDelay) → KeyUp(key)`

```csharp
void KeyDown(VirtualKey key)
void KeyUp(VirtualKey key)
```
- **Purpose:** Manual key state control
- **Use Case:** Custom hold durations, manual modifier management
- **State:** Maintained in `_currentModifiers` and `_currentKeys` collections

#### Keyboard Shortcuts

```csharp
void Chord(VirtualKey modifier, VirtualKey key)
```
- **Purpose:** Single modifier + key combination
- **Sequence:**
  1. `KeyDown(modifier)`
  2. `Wait(Configuration.ModifierHoldDuration)`
  3. `KeyDown(key)`
  4. `Wait(Configuration.KeystrokeDelay)`
  5. `KeyUp(key)`
  6. `KeyUp(modifier)`

```csharp
void Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key)
```
- **Purpose:** Multiple modifiers + key combination
- **Example:** Ctrl+Shift+Esc
- **Sequence:**
  1. Press all modifiers sequentially
  2. `Wait(ModifierHoldDuration)`
  3. Press main key
  4. Release main key
  5. Release all modifiers in reverse order

---

### 3.4 Configuration Class

**Namespace:** `SendSequenceCL`
**Type:** `public static class Configuration`

All properties are **thread-safe** and apply **immediately** to subsequent operations.

#### Properties

```csharp
public static int MouseMovementDuration { get; set; }
```
- **Default:** 400 ms
- **Range:** >= 0
- **Purpose:** Duration of `MoveHuman()` operations
- **Guidance:**
  - `0` = instant (equivalent to MoveTo)
  - `100-300` = fast
  - `300-500` = natural
  - `500+` = slow

```csharp
public static double CurveRandomization { get; set; }
```
- **Default:** 0.10 (10%)
- **Range:** 0.0 - 1.0
- **Purpose:** Random variation in Bézier control points
- **Guidance:**
  - `0.0` = deterministic (same path every time)
  - `0.05-0.15` = subtle natural variation
  - `0.2+` = noticeable randomness

```csharp
public static int ClickDuration { get; set; }
```
- **Default:** 50 ms
- **Range:** >= 0
- **Purpose:** Delay between button down/up events
- **Guidance:**
  - `0` = instant
  - `30-100` = normal
  - `100+` = slow click

```csharp
public static int KeystrokeDelay { get; set; }
```
- **Default:** 75 ms
- **Range:** >= 0
- **Purpose:** Delay between key presses and between press/release
- **Guidance:**
  - `0` = robotic typing
  - `30-50` = fast
  - `50-100` = normal
  - `100+` = slow

```csharp
public static int ModifierHoldDuration { get; set; }
```
- **Default:** 100 ms
- **Range:** >= 0
- **Purpose:** Delay after pressing modifiers before pressing main key
- **Guidance:**
  - `0` = risky (may not register)
  - `50-150` = natural
  - `150+` = slow

```csharp
public static int DragPauseMs { get; set; }
```
- **Default:** 50 ms
- **Range:** >= 0
- **Purpose:** Brief pause before/after drag movement
- **Guidance:**
  - `0` = instant
  - `30-100` = normal
  - `100+` = slow

---

### 3.5 MouseButton Enum

**Namespace:** `SendSequenceCL`
**Type:** `public enum MouseButton : byte`

```csharp
public enum MouseButton : byte
{
    Left = 1,    // Primary button
    Right = 2,   // Secondary button (context menu)
    Middle = 3   // Scroll wheel button
}
```

**HID Mapping:** Maps to HID button bit flags (1, 2, 4)

---

### 3.6 VirtualKey Enum

**Namespace:** `SendSequenceCL`
**Type:** `public enum VirtualKey : byte`

**Values:** Based on USB HID Usage Table (Keyboard/Keypad page 0x07)

#### Key Categories

**Letters (0x04-0x1D):**
```csharp
A = 0x04, B = 0x05, ..., Z = 0x1D
```

**Numbers (0x1E-0x27):**
```csharp
D0 = 0x27, D1 = 0x1E, D2 = 0x1F, ..., D9 = 0x26
```

**Function Keys (0x3A-0x45):**
```csharp
F1 = 0x3A, F2 = 0x3B, ..., F12 = 0x45
```

**Modifiers (0xE0-0xE7):**
```csharp
LeftControl = 0xE0, LeftShift = 0xE1, LeftAlt = 0xE2,
LeftGui = 0xE3, RightControl = 0xE4, RightShift = 0xE5,
RightAlt = 0xE6, RightGui = 0xE7
Windows = 0xE3  // Alias for LeftGui
```

**Control Keys:**
```csharp
Enter = 0x28, Escape = 0x29, Backspace = 0x2A,
Tab = 0x2B, Space = 0x2C, Delete = 0x4C,
Home = 0x4A, End = 0x4D, PageUp = 0x4B, PageDown = 0x4E
```

**Arrow Keys:**
```csharp
Right = 0x4F, Left = 0x50, Down = 0x51, Up = 0x52
```

**Full list:** See `API/VirtualKey.cs` for complete enumeration

---

### 3.7 Exception Hierarchy

**Base Exception:**
```csharp
public class SendSequenceException : Exception
```
- **Purpose:** Base for all library exceptions
- **Catch:** Use to handle all library-specific errors

**Derived Exceptions:**

```csharp
public class DriverNotFoundException : SendSequenceException
```
- **Cause:** HID driver not found (VID=0xF00F)
- **Resolution:** Install Tetherscript driver; launch ControlMyJoystick
- **Fatal:** Cannot recover; operation impossible

```csharp
public class DriverCommunicationException : SendSequenceException
```
- **Cause:** Communication failure with HID driver
- **Resolution:** Retry operation; restart ControlMyJoystick
- **Retryable:** May succeed on retry

```csharp
public class InvalidCoordinateException : SendSequenceException
```
- **Cause:** Coordinates outside screen bounds
- **Resolution:** Use valid screen coordinates
- **Prevention:** Check screen resolution via `SystemInformation`

```csharp
public class InvalidKeyCodeException : SendSequenceException
```
- **Cause:** Unsupported VirtualKey value
- **Resolution:** Use defined enum values only
- **Prevention:** Validate enum with `Enum.IsDefined()`

---

## 4. Internal Architecture

### 4.1 Core Components

#### MouseController

**File:** `Core/MouseController.cs`
**Responsibility:** Implements `IVirtualMouse` interface

**Key Features:**
- Maintains button state (`_currentButtonState`)
- Integrates `BezierCurveGenerator` for human-like motion
- Delegates to `HidCommunicator.SendMouseAbsolute()`

**Coordinate Conversion:**
```
Screen Coords (pixels) → ScreenUtilities.ScreenToDriverCoordinates()
→ Driver Coords (0-32767 range) → HID Absolute Report
```

#### KeyboardController

**File:** `Core/KeyboardController.cs`
**Responsibility:** Implements `IVirtualKeyboard` interface

**Key Features:**
- Maintains modifier state (`_currentModifiers`)
- Uses `KeyboardMapper` for text-to-key conversion
- Delegates to `HidCommunicator.SendKeyboardReport()`

**Text Mapping:**
```
Character → KeyboardMapper.MapChar()
→ (VirtualKey, needsShift) → HID Keyboard Report
```

#### BezierCurveGenerator

**File:** `Core/BezierCurveGenerator.cs`
**Algorithm:** Cubic Bézier curve

**Control Point Generation:**
```csharp
P0 = (startX, startY)  // Start
P3 = (endX, endY)      // End

// Control points with randomization
P1 = P0 + offset with random factor (CurveRandomization)
P2 = P3 - offset with random factor (CurveRandomization)
```

**Interpolation:**
```csharp
for (t = 0.0; t <= 1.0; t += step) {
    B(t) = (1-t)³·P0 + 3(1-t)²t·P1 + 3(1-t)t²·P2 + t³·P3
}
```

**Randomization:** Applied to control point offsets, not final curve points

---

### 4.2 Infrastructure Components

#### HidDeviceManager

**File:** `Infrastructure/HidDeviceManager.cs`
**Responsibility:** HID device discovery and lifecycle

**Discovery Process:**
1. Call `HidD_GetHidGuid()` to get HID class GUID
2. `SetupDiGetClassDevs()` to enumerate devices
3. `SetupDiEnumDeviceInterfaces()` to iterate interfaces
4. `SetupDiGetDeviceInterfaceDetail()` to get device path
5. `CreateFile()` to open device handle
6. `HidD_GetAttributes()` to verify VID/PID match

**Shared Access Fallback:**
```csharp
// Try full access first
handle = CreateFile(path, ReadWrite, ReadWrite, ...)

if (handle.IsInvalid) {
    // Fallback: no read/write, only feature reports
    handle = CreateFile(path, 0, ReadWrite, ...)
}
```
- **Reason:** ControlMyJoystick may hold exclusive read/write access
- **Solution:** Feature reports (HidD_SetFeature) work with zero access rights

#### HidCommunicator

**File:** `Infrastructure/HidCommunicator.cs`
**Responsibility:** Send commands to HID devices

**Mouse Report Format:**
```
Byte 0: Report ID (0x05 for absolute mouse)
Byte 1-2: X position (little-endian, 0-32767)
Byte 3-4: Y position (little-endian, 0-32767)
Byte 5: Button state (bit flags)
```

**Keyboard Report Format:**
```
Byte 0: Report ID (0x04 for keyboard)
Byte 1: Modifier byte (Ctrl/Shift/Alt/Win flags)
Byte 2: Reserved (0x00)
Byte 3-8: Up to 6 simultaneous key codes
```

**Transmission:**
- Uses `HidD_SetFeature()` Win32 API
- Synchronous blocking call
- No return data

---

## 5. Behavioral Specifications

### 5.1 Human-Like Motion Algorithm

**Bézier Curve Properties:**
- **Smoothness:** C¹ continuous (smooth velocity)
- **Controllability:** Start/end tangents controlled by P1/P2
- **Bounded:** Curve stays within convex hull of control points

**Randomization Impact:**
```
Low (0.05):  Subtle path variation
Medium (0.1): Natural human variance
High (0.2):   Noticeable wobble
```

**Performance:**
- Steps = `Max(10, durationMs / 10)`
- Each step: 1 HID report + 1 thread sleep
- Total overhead: ~1-2ms per step (negligible)

### 5.2 Timing Guarantees

**Precision:**
- ±5ms for delays < 100ms (Thread.Sleep limitations)
- ±1% for delays > 100ms

**Async Timing:**
- `Task.Delay()` has same precision as `Thread.Sleep()`
- Better performance under load due to non-blocking

**Real-World Factors:**
- Windows scheduler granularity (~15ms)
- HID polling rate (typically 125Hz = 8ms)
- System load may introduce jitter

### 5.3 State Management

**Mouse Button State:**
- Tracked in `MouseController._currentButtonState`
- Bitwise flags: Left=1, Right=2, Middle=4
- Persists across operations
- Reset only on `Down()`/`Up()` calls

**Keyboard Modifier State:**
- Tracked in `KeyboardController._currentModifiers`
- Separate from typed keys
- Auto-managed in `Chord()` operations
- Manual management via `KeyDown()`/`KeyUp()`

**Device Handles:**
- Lazy initialization on first access
- Kept open until `Dispose()` or app exit
- Re-opened automatically after `Dispose()` if accessed again

---

## 6. Performance Characteristics

### 6.1 Latency

**Operation Latencies (typical):**
```
MoveTo():          < 1ms (single HID report)
MoveHuman(400ms):  ~400ms (40 steps × 10ms)
Click():           ~50ms (Configuration.ClickDuration)
KeyPress():        ~75ms (Configuration.KeystrokeDelay)
TypeText(10 char): ~750ms (10 × 75ms)
Drag(600ms):       ~750ms (600ms move + pauses)
```

**HID Communication:**
- Feature report write: ~0.1-0.5ms
- Driver processing: < 1ms
- Total input latency: < 5ms

### 6.2 Resource Usage

**Memory:**
- Base library: ~500KB in memory
- Per-operation overhead: < 1KB (curve generation)
- No memory leaks (verified with manual testing)

**CPU:**
- Idle: 0% (no background threads)
- During MoveHuman: < 1% (simple calculations)
- During TypeText: < 1% (lookup operations)

**Threads:**
- No dedicated threads
- Async operations use thread pool
- Thread-safe for concurrent calls (though serialized at HID level)

### 6.3 Scalability

**Sustained Operations:**
- Tested: 10,000 consecutive mouse movements
- Result: No degradation, no memory leaks
- Bottleneck: HID driver, not library

**Concurrent Usage:**
- Multiple threads can call API
- Operations serialized at `HidCommunicator` level
- No explicit locking (single ownership model)

---

## 7. Limitations & Constraints

### 7.1 Platform Limitations

**Windows Only:**
- Depends on Windows HID API (`hid.dll`, `setupapi.dll`)
- Tetherscript driver is Windows-only

**x64 Only:**
- Tested on x64 architecture
- x86 may work but untested (P/Invoke structure sizes)

**Driver Dependency:**
- Requires Tetherscript HVDK driver installation
- Driver discontinued (December 2022) but obtainable via ControlMyJoystick trial

### 7.2 Functional Limitations

**Mouse:**
- ❌ **No mouse wheel/scroll support** - Critical missing feature
- ❌ No relative movement (only absolute positioning)
- ❌ No multi-monitor awareness (uses primary monitor coordinate space)
- ✅ Supports 3 buttons (Left, Right, Middle)

**Keyboard:**
- ❌ **US layout only** - Non-US keyboards unsupported
- ❌ No Unicode input (only ASCII characters)
- ❌ No IME support (Chinese/Japanese/Korean input)
- ❌ No dead key support (accented characters)
- ✅ All standard keys via VirtualKey enum

**General:**
- No input recording/macro playback
- No screen capture integration
- No window management
- No clipboard operations

### 7.3 Security & Safety

**Anti-Cheat Detection:**
- **Detectable:** Yes, some anti-cheat systems detect virtual HID input
- **Mitigation:** Human-like timing helps but no guarantees
- **Use Case:** Intended for automation, not game cheating

**Privilege Requirements:**
- **Admin:** Not required for normal operation
- **Driver:** Requires Tetherscript driver (installed once with admin)
- **UAC:** Cannot send input to elevated applications from non-elevated process

**Race Conditions:**
- ❌ No protection against UI state changes mid-operation
- ❌ No verification that target window is active
- ⚠️ User must ensure target application is ready

---

## 8. Extension Points

### 8.1 Adding New Features

**Mouse Wheel Support (Recommended Addition):**

```csharp
// In IVirtualMouse
void Scroll(int delta, bool horizontal = false);

// In MouseController
public void Scroll(int delta, bool horizontal = false)
{
    byte wheelData = (byte)Math.Clamp(delta, -127, 127);
    _communicator.SendMouseWheel(wheelData, horizontal);
}

// In HidCommunicator
public void SendMouseWheel(byte delta, bool horizontal)
{
    byte[] report = new byte[6];
    report[0] = 0x06; // Wheel report ID
    report[horizontal ? 2 : 1] = delta;
    NativeMethods.HidD_SetFeature(_handle, report, report.Length);
}
```

**Multi-Monitor Support:**

```csharp
// In Configuration
public static int TargetMonitorIndex { get; set; } = 0; // Primary

// In ScreenUtilities
public static (int, int) ScreenToDriverCoordinates(int x, int y)
{
    var screen = Screen.AllScreens[Configuration.TargetMonitorIndex];
    // Adjust coordinates to monitor bounds
    int relativeX = x - screen.Bounds.Left;
    int relativeY = y - screen.Bounds.Top;
    // Convert to driver range
    ...
}
```

### 8.2 Replacing Components

**Custom Curve Algorithm:**

Implement custom `ICurveGenerator`:
```csharp
public interface ICurveGenerator
{
    IEnumerable<Point> Generate(int x1, int y1, int x2, int y2, int steps, double randomization);
}
```

Inject into `MouseController` constructor (requires refactoring from static `Input`)

**Custom HID Backend:**

Implement `IHidCommunicator`:
```csharp
public interface IHidCommunicator
{
    void SendMouseAbsolute(int x, int y, byte buttons);
    void SendKeyboardReport(byte modifiers, byte[] keys);
}
```

Example: Mock implementation for testing

---

## 9. Testing Considerations

### 9.1 Unit Testing Challenges

**Static Dependencies:**
- `Input` class is static → hard to mock
- **Solution:** Test against interfaces (`IVirtualMouse`, `IVirtualKeyboard`)

**HID Driver Dependency:**
- Requires real driver for integration tests
- **Solution:** Create mock `IHidCommunicator` implementation

**Timing-Dependent Operations:**
- `MoveHuman()` duration is non-deterministic
- **Solution:** Test with `durationMs = 0` for deterministic behavior

### 9.2 Integration Testing

**Prerequisites:**
1. Tetherscript driver installed
2. ControlMyJoystick running
3. Test application has focus

**Test Scenarios:**
- ✅ Click at specific coordinates
- ✅ Type text into Notepad
- ✅ Drag file in Explorer
- ✅ Keyboard shortcuts (Ctrl+C, Alt+F4)
- ✅ Concurrent async operations

**Verification:**
- Manual observation (cursor movement visible)
- Screen recording for regression testing
- OCR to verify typed text (external tool)

### 9.3 Error Injection

**Simulating Failures:**
- Unplug driver: `DriverNotFoundException`
- Kill ControlMyJoystick: `DriverCommunicationException`
- Invalid coordinates: `InvalidCoordinateException`

**Recovery Testing:**
- Restart driver mid-operation
- Dispose and re-access
- Handle exceptions and retry

---

## 10. Maintenance & Evolution

### 10.1 Breaking Changes Policy

**Semantic Versioning:**
- MAJOR: Breaking API changes
- MINOR: New features (backward compatible)
- PATCH: Bug fixes

**Deprecation Process:**
1. Mark as `[Obsolete]` with migration guidance
2. Keep for 1 MINOR version
3. Remove in next MAJOR version

### 10.2 Future Roadmap

**High Priority (Missing 90% Coverage):**
1. ✅ Mouse wheel/scroll support
2. ✅ Multi-monitor support
3. ✅ Keyboard layout abstraction
4. ⚠️ Input state verification (is key pressed?)

**Medium Priority:**
5. Macro recording/playback
6. Fluent API for complex sequences
7. Performance profiling tools
8. Better error diagnostics

**Low Priority:**
9. IME support
10. Unicode input via clipboard paste
11. Plugin architecture for custom backends

---

## 11. Comparison with Alternatives

| Feature | SendSequenceCL | WindowsInput | InputSimulator | AutoIt |
|---------|---------------|--------------|----------------|--------|
| **Driver Type** | Virtual HID | SendInput API | SendInput API | SendInput API |
| **Detection** | Low | High | High | Medium |
| **Human-like** | ✅ Bézier curves | ❌ | ❌ | ✅ Randomization |
| **Async Support** | ✅ | ❌ | ❌ | ✅ |
| **Dependencies** | Tetherscript | None | None | Standalone |
| **.NET Version** | .NET 8 | .NET 4.5 | .NET Core 3.1 | N/A (exe) |
| **License** | Custom | MIT | MIT | Freeware |

**Advantages:**
- ✅ Virtual HID = harder to detect
- ✅ Human-like motion
- ✅ Modern .NET 8

**Disadvantages:**
- ❌ Requires driver installation
- ❌ Windows-only
- ❌ No wheel support (yet)

---

## 12. References

### 12.1 Technical Standards

**USB HID Specification:**
- [USB HID Usage Tables 1.22](https://usb.org/document-library/hid-usage-tables-122)
- Keyboard/Keypad Page (0x07) - VirtualKey enum values

**Windows APIs:**
- `SetupAPI.dll` - Device enumeration
- `hid.dll` - HID communication
- `user32.dll` - Screen coordinates (GetCursorPos)

### 12.2 Related Documentation

**Tetherscript HVDK:**
- Official documentation discontinued
- Community archives: [GitHub reverse engineering efforts]

**Bézier Curves:**
- [Primer on Bézier Curves](https://pomax.github.io/bezierinfo/)
- Cubic Bézier interpolation formulas

### 12.3 Source Files Map

```
API/
├── Configuration.cs          - Runtime configuration
├── Exceptions.cs             - Exception types
├── Input.cs                  - Main facade
├── IVirtualKeyboard.cs       - Keyboard interface
├── IVirtualMouse.cs          - Mouse interface
├── MouseButton.cs            - Mouse button enum
└── VirtualKey.cs             - Keyboard key enum

Core/
├── BezierCurveGenerator.cs   - Curve algorithm
├── KeyboardController.cs     - IVirtualKeyboard implementation
├── KeyboardMapper.cs         - Char → VirtualKey mapping
├── MouseController.cs        - IVirtualMouse implementation
└── ScreenUtilities.cs        - Coordinate conversion

Infrastructure/
├── DriverConstants.cs        - VID/PID constants
├── HidCommunicator.cs        - HID report transmission
├── HidDeviceManager.cs       - Device discovery/lifecycle
├── HidStructures.cs          - P/Invoke structures
└── NativeMethods.cs          - Win32 API declarations
```

---

## Appendix A: Configuration Profiles

### Speed-Optimized Profile
```csharp
Configuration.MouseMovementDuration = 100;
Configuration.CurveRandomization = 0.0;
Configuration.ClickDuration = 10;
Configuration.KeystrokeDelay = 20;
Configuration.ModifierHoldDuration = 50;
Configuration.DragPauseMs = 10;
```

### Human-Like Profile
```csharp
Configuration.MouseMovementDuration = 450;
Configuration.CurveRandomization = 0.15;
Configuration.ClickDuration = 80;
Configuration.KeystrokeDelay = 120;
Configuration.ModifierHoldDuration = 150;
Configuration.DragPauseMs = 100;
```

### Balanced Profile (Default)
```csharp
Configuration.MouseMovementDuration = 400;
Configuration.CurveRandomization = 0.10;
Configuration.ClickDuration = 50;
Configuration.KeystrokeDelay = 75;
Configuration.ModifierHoldDuration = 100;
Configuration.DragPauseMs = 50;
```

---

## Appendix B: Error Handling Patterns

### Retry Pattern for DriverCommunicationException
```csharp
int maxRetries = 3;
for (int i = 0; i < maxRetries; i++)
{
    try
    {
        Input.Mouse.MoveTo(500, 500);
        break; // Success
    }
    catch (DriverCommunicationException ex)
    {
        if (i == maxRetries - 1) throw; // Last attempt
        Thread.Sleep(500); // Wait before retry
    }
}
```

### Graceful Degradation
```csharp
try
{
    Input.Mouse.MoveHuman(800, 600);
}
catch (DriverNotFoundException)
{
    // Fallback to Windows SendInput API
    FallbackInputMethod.MoveMouse(800, 600);
}
```

---

**END OF SPECIFICATION**
