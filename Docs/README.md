# SendSequenceCL - AI Agent Quick Reference

## Core Concept
Human-like input automation using **Tetherscript HID drivers** instead of WinAPI for maximum legitimacy and anti-detection.

## Requirements
- .NET 8.0
- Windows OS
- Tetherscript HID drivers installed:
  - Mouse Absolute (VID: 0xFEED, PID: 0x0002)
  - Mouse Relative (VID: 0xFEED, PID: 0x0005)
  - Keyboard (VID: 0xFEED, PID: 0x0003)

## Installation
Add DLL reference to project or use as library dependency.

<ItemGroup>
  <ProjectReference Include="..\SendSequenceCL\SendSequenceCL.csproj" />
</ItemGroup>

## Basic Usage

```csharp
using SendSequenceCL;

// Mouse operations
Input.Mouse.MoveTo(500, 300);                    // Instant teleport
Input.Mouse.MoveHuman(500, 300);                 // Human-like curved motion
Input.Mouse.Click(MouseButton.Left);             // Click
Input.Mouse.Drag(100, 100, 500, 500);           // Drag with human motion

// Keyboard operations
Input.Keyboard.TypeText("Hello World");          // Type text
Input.Keyboard.TypeTextHuman("Hello World");     // Type with typos/delays
Input.Keyboard.KeyPress(VirtualKey.Enter);       // Press key
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C); // Ctrl+C

// Sequence Builder (fluent API)
Input.Sequence()
    .MoveMouseHuman(500, 300)
    .Click()
    .TypeTextHuman("search query")
    .PressKey(VirtualKey.Enter)
    .Wait(1000)
    .Execute();

// Cleanup when done
Input.Dispose();
```

## Key Classes

**Input** - Static facade, main entry point
- `Input.Mouse` → IVirtualMouse operations
- `Input.Keyboard` → IVirtualKeyboard operations
- `Input.Sequence()` → SequenceBuilder for chaining

**Configuration** - Global humanization settings
```csharp
Configuration.MouseMovementDuration = (300, 800);  // Random 300-800ms
Configuration.MotionAlgorithm = MotionAlgorithm.PerlinNoise;
Configuration.EnableMouseOvershoot = true;
Configuration.TypoChance = 0.02; // 2% typo chance
```

## Humanization Features
- **Mouse**: Curved motion (Bezier/Perlin Noise), overshoot, random duration
- **Keyboard**: Random keystroke delays, typos with backspace, inter-word pauses
- **Timing**: All delays use (Min, Max) ranges for randomization

## Important Notes

### Mouse Scrolling
`Scroll()` uses **arrow keys** (Up/Down) not mouse wheel - HID driver limitation:
```csharp
Input.Mouse.Scroll(5);  // Presses Down key 5 times
Input.Mouse.Scroll(-3); // Presses Up key 3 times
```

### Key State Detection
```csharp
bool isPressed = Input.Keyboard.IsKeyDown(VirtualKey.LeftControl);
```

### VirtualKey Enum
Uses **HID Usage IDs** (not Windows VK codes):
- Letters: `VirtualKey.A` through `VirtualKey.Z`
- Numbers: `VirtualKey.D0` through `VirtualKey.D9`
- Special: `VirtualKey.Enter`, `VirtualKey.Space`, `VirtualKey.Backspace`
- Modifiers: `VirtualKey.LeftControl`, `VirtualKey.LeftShift`, etc.

### Async Support
```csharp
await Input.Mouse.MoveHumanAsync(x, y, cancellationToken);
await Input.Mouse.DragAsync(x1, y1, x2, y2, cancellationToken);
```

### SequenceBuilder Guards
```csharp
Input.Sequence()
    .DoIf(() => someCondition, seq => seq.Click())
    .WaitUntil(() => elementVisible, timeoutMs: 5000)
    .ClickIfAt(x, y, tolerance: 5)
    .Execute();
```

## Exception Handling
- `DriverNotFoundException` - HID driver not found
- `DriverCommunicationException` - Failed to send HID report
- `InvalidCoordinateException` - Coordinates outside screen bounds
- `InvalidKeyCodeException` - Invalid key for operation

## Why HID Drivers?
Traditional WinAPI methods (`SendInput`, `mouse_event`, `keybd_event`) are easily detected by:
- Kernel-level anti-cheat
- Signed driver validation
- Input filtering
- Behavioral analysis

Tetherscript creates **virtual HID devices** that appear as real USB peripherals to OS and applications - indistinguishable from physical input.
