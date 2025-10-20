# SendSequenceCL

**Библиотека классов для человекоподобной автоматизации ввода в .NET**

SendSequenceCL предоставляет интуитивный API для автоматизации мыши и клавиатуры через виртуальный HID-драйвер Tetherscript. Библиотека имитирует реальное поведение человека с плавными движениями мыши по кривым Безье и настраиваемыми задержками между действиями.

## Содержание

- [Возможности](#возможности)
- [Системные требования](#системные-требования)
- [Установка и настройка](#установка-и-настройка)
- [Быстрый старт](#быстрый-старт)
- [Примеры использования](#примеры-использования)
- [Справочник API](#справочник-api)
- [Конфигурация](#конфигурация)
- [Асинхронные операции](#асинхронные-операции)
- [Обработка ошибок](#обработка-ошибок)
- [Устранение неполадок](#устранение-неполадок)
- [Архитектура проекта](#архитектура-проекта)

---

## Возможности

### ✅ Автоматизация мыши

- **Мгновенное перемещение** (`MoveTo`) - телепорт курсора в указанные координаты
- **Человекоподобное движение** (`MoveHuman`) - плавное движение по кривой Безье с рандомизацией
- **Операции клика** - одинарный клик, двойной клик, нажатие/отпускание кнопок
- **Перетаскивание** (`Drag`) - drag & drop с зажатой кнопкой мыши
- **Поддержка всех кнопок** - левая, правая, средняя кнопки мыши
- **Асинхронные версии** - неблокирующие операции с поддержкой CancellationToken

### ✅ Автоматизация клавиатуры

- **Набор текста** (`TypeText`) - автоматическая обработка заглавных букв и символов
- **Нажатие клавиш** (`KeyPress`) - отдельные клавиши
- **Комбинации клавиш** (`Chord`) - Ctrl+C, Alt+F4, Ctrl+Shift+Esc и т.д.
- **Ручное управление** (`KeyDown`, `KeyUp`) - полный контроль над клавишами
- **Поддержка модификаторов** - Ctrl, Shift, Alt, Windows

### ✅ Человекоподобное поведение

- **Кривые Безье** - естественные траектории движения мыши
- **Рандомизация** - случайные вариации в контрольных точках кривых
- **Настраиваемые задержки** - между кликами, нажатиями клавиш, движениями
- **Конфигурируемые паузы** - перед/после перетаскивания, удержания модификаторов

### ✅ Типобезопасный API

- **Строгая типизация** - enum'ы для кнопок мыши и клавиш клавиатуры
- **Валидация параметров** - проверка координат экрана, значений enum
- **Обработка исключений** - специализированные типы исключений
- **XML-документация** - полная поддержка IntelliSense

---

## Системные требования

### Обязательные требования

- **.NET 8.0** или выше
- **Windows 10/11** (только x64)
- **Tetherscript HID Virtual Driver** - установлен и активирован

### Получение драйвера Tetherscript

**ВАЖНО:** Tetherscript HVDK был прекращен в декабре 2022 года, но драйверы все еще можно получить:

1. **Скачайте ControlMyJoystick** с официального сайта: https://controlmyjoystick.com/
2. **Установите 14-дневную пробную версию** - это установит драйверы Tetherscript HID
3. **Запустите ControlMyJoystick** и активируйте виртуальные устройства
4. **Драйверы продолжат работать** даже после истечения пробного периода

**Примечание:** Для работы библиотеки покупка ControlMyJoystick НЕ требуется - достаточно установленных драйверов.

### Проверка установки драйвера

Откройте **Диспетчер устройств** и найдите:

```
Устройства HID (Human Interface Devices)
├── Tetherscript Virtual HID Gamepad (VID=0xF00F, PID=0x0001)
├── Tetherscript Virtual HID Mouse Absolute (VID=0xF00F, PID=0x0002)
├── Tetherscript Virtual HID Mouse Relative (VID=0xF00F, PID=0x0003)
├── Tetherscript Virtual HID Keyboard (VID=0xF00F, PID=0x0004)
└── Tetherscript Virtual HID Firmware (VID=0xF00F, PID=0x0005)
```

Если устройства отображаются, драйвер установлен корректно.

---

## Установка и настройка

### Вариант 1: Добавление ссылки на проект

Если вы используете библиотеку из исходного кода:

```xml
<ItemGroup>
  <ProjectReference Include="..\SendSequenceCL\SendSequenceCL.csproj" />
</ItemGroup>
```

### Вариант 2: Компиляция и использование DLL

```bash
# Компиляция библиотеки
cd SendSequenceCL
dotnet build -c Release

# Скопируйте SendSequenceCL.dll в ваш проект
# Добавьте ссылку в .csproj:
```

```xml
<ItemGroup>
  <Reference Include="SendSequenceCL">
    <HintPath>путь\к\SendSequenceCL.dll</HintPath>
  </Reference>
</ItemGroup>
```

### Проверка работы

Запустите приложение Examples для тестирования:

```bash
cd Examples
dotnet run
```

Программа предложит выбрать один из 7 примеров для демонстрации возможностей библиотеки.

---

## Быстрый старт

### Пример 1: Базовое использование мыши

```csharp
using System;
using SendSequenceCL;

class Program
{
    static void Main()
    {
        try
        {
            // Мгновенное перемещение курсора
            Input.Mouse.MoveTo(960, 540);

            // Человекоподобное плавное движение
            Input.Mouse.MoveHuman(500, 300, durationMs: 400);

            // Клик левой кнопкой
            Input.Mouse.Click(MouseButton.Left);

            // Двойной клик
            Input.Mouse.DoubleClick(MouseButton.Left);

            // Освобождение ресурсов
            Input.Dispose();
        }
        catch (DriverNotFoundException ex)
        {
            Console.WriteLine("Драйвер Tetherscript не найден!");
            Console.WriteLine("Установите ControlMyJoystick и запустите его.");
        }
    }
}
```

### Пример 2: Базовое использование клавиатуры

```csharp
using System;
using SendSequenceCL;

class Program
{
    static void Main()
    {
        try
        {
            // Набор текста (автоматически обрабатывает Shift)
            Input.Keyboard.TypeText("Hello, World!");

            // Нажатие Enter
            Input.Keyboard.KeyPress(VirtualKey.Enter);

            // Ctrl+C (копирование)
            Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C);

            // Ctrl+Shift+Esc (диспетчер задач)
            Input.Keyboard.Chord(
                new[] { VirtualKey.LeftControl, VirtualKey.LeftShift },
                VirtualKey.Escape
            );

            Input.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}
```

### Пример 3: Настройка поведения

```csharp
using SendSequenceCL;

// Настройка для быстрой автоматизации
Configuration.MouseMovementDuration = 150;  // Быстрое движение мыши
Configuration.ClickDuration = 20;           // Быстрые клики
Configuration.KeystrokeDelay = 30;          // Быстрый набор текста

// Настройка для естественного поведения (обход антибот-систем)
Configuration.MouseMovementDuration = 450;  // Медленное движение
Configuration.CurveRandomization = 0.15;    // Больше рандомизации
Configuration.KeystrokeDelay = 90;          // Медленный набор
```

---

## Примеры использования

### Автоматическое заполнение веб-форм

```csharp
using SendSequenceCL;

// Клик в поле "Email"
Input.Mouse.MoveHuman(400, 250, 300);
Input.Mouse.Click(MouseButton.Left);
Input.Wait(100);

// Ввод email
Input.Keyboard.TypeText("example@mail.com");

// Переход к следующему полю (Tab)
Input.Keyboard.KeyPress(VirtualKey.Tab);
Input.Wait(50);

// Ввод пароля
Input.Keyboard.TypeText("MySecurePassword123");

// Отправка формы (Enter)
Input.Keyboard.KeyPress(VirtualKey.Enter);
```

### Автоматизация копирования текста

```csharp
using SendSequenceCL;

// Выделить весь текст (Ctrl+A)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.A);
Input.Wait(100);

// Скопировать в буфер обмена (Ctrl+C)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C);
Input.Wait(200);

// Переместиться в новое окно
Input.Mouse.MoveHuman(800, 400, 400);
Input.Mouse.Click(MouseButton.Left);
Input.Wait(100);

// Вставить текст (Ctrl+V)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.V);
```

### Drag & Drop файлов

```csharp
using SendSequenceCL;

// Перетаскивание файла с рабочего стола в папку
Input.Mouse.Drag(
    startX: 200,     // Начальная позиция (файл на рабочем столе)
    startY: 200,
    endX: 800,       // Конечная позиция (папка)
    endY: 500,
    button: MouseButton.Left,
    durationMs: 600  // Длительность перетаскивания
);

Console.WriteLine("Файл перемещен!");
```

### Открытие и закрытие приложений

```csharp
using SendSequenceCL;

// Открыть диалог "Выполнить" (Win+R)
Input.Keyboard.Chord(VirtualKey.Windows, VirtualKey.R);
Input.Wait(500);

// Набрать команду для открытия Блокнота
Input.Keyboard.TypeText("notepad");
Input.Wait(200);

// Нажать Enter для запуска
Input.Keyboard.KeyPress(VirtualKey.Enter);
Input.Wait(1500);

// Набрать текст в Блокноте
Input.Keyboard.TypeText("SendSequenceCL - автоматизация ввода для .NET");
Input.Wait(500);

// Закрыть Блокнот (Alt+F4)
Input.Keyboard.Chord(VirtualKey.LeftAlt, VirtualKey.F4);
Input.Wait(500);

// Отказаться от сохранения (N)
Input.Keyboard.KeyPress(VirtualKey.N);
```

### Скриншоты с помощью автоматизации

```csharp
using SendSequenceCL;

// Открыть утилиту "Ножницы" (Win+Shift+S)
Input.Keyboard.Chord(
    new[] { VirtualKey.Windows, VirtualKey.LeftShift },
    VirtualKey.S
);
Input.Wait(1000);

// Выделить область экрана (drag)
Input.Mouse.Drag(100, 100, 800, 600, MouseButton.Left, 500);
Input.Wait(1000);

// Сохранить скриншот (Ctrl+S)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.S);
```

---

## Справочник API

### Класс Input (точка входа)

Статический класс-фасад для доступа к автоматизации.

```csharp
public static class Input
{
    // Доступ к операциям мыши
    public static IVirtualMouse Mouse { get; }

    // Доступ к операциям клавиатуры
    public static IVirtualKeyboard Keyboard { get; }

    // Пауза выполнения
    public static void Wait(int milliseconds);

    // Освобождение ресурсов HID-устройств
    public static void Dispose();
}
```

**Примеры:**

```csharp
// Пауза на 1 секунду
Input.Wait(1000);

// Освобождение ресурсов (вызывайте в конце программы)
Input.Dispose();
```

---

### Интерфейс IVirtualMouse

#### Методы получения информации

```csharp
// Получить текущую позицию курсора
Point GetPosition();
```

**Пример:**

```csharp
Point pos = Input.Mouse.GetPosition();
Console.WriteLine($"Курсор находится в: X={pos.X}, Y={pos.Y}");
```

#### Методы перемещения

```csharp
// Мгновенное перемещение (телепорт)
void MoveTo(int x, int y);

// Человекоподобное плавное движение
void MoveHuman(int x, int y, int? durationMs = null);

// Асинхронное плавное движение
Task MoveHumanAsync(int x, int y, int? durationMs = null, CancellationToken cancellationToken = default);
```

**Примеры:**

```csharp
// Телепорт в центр экрана
Input.Mouse.MoveTo(960, 540);

// Плавное движение за 500мс
Input.Mouse.MoveHuman(1200, 400, 500);

// Движение с дефолтной скоростью (Configuration.MouseMovementDuration)
Input.Mouse.MoveHuman(800, 600);

// Асинхронное движение с возможностью отмены
CancellationTokenSource cts = new CancellationTokenSource();
await Input.Mouse.MoveHumanAsync(500, 300, 400, cts.Token);
```

#### Методы кликов

```csharp
// Одинарный клик
void Click(MouseButton button);

// Двойной клик
void DoubleClick(MouseButton button);

// Нажать кнопку (без отпускания)
void Down(MouseButton button);

// Отпустить кнопку
void Up(MouseButton button);
```

**Примеры:**

```csharp
// Клик левой кнопкой
Input.Mouse.Click(MouseButton.Left);

// Двойной клик левой кнопкой
Input.Mouse.DoubleClick(MouseButton.Left);

// Клик правой кнопкой (контекстное меню)
Input.Mouse.Click(MouseButton.Right);

// Ручное управление (для специальных сценариев)
Input.Mouse.Down(MouseButton.Left);
Input.Wait(1000);  // Держать кнопку нажатой 1 секунду
Input.Mouse.Up(MouseButton.Left);
```

#### Методы перетаскивания

```csharp
// Перетаскивание объекта
void Drag(int startX, int startY, int endX, int endY,
          MouseButton button = MouseButton.Left,
          int? durationMs = null);

// Асинхронное перетаскивание
Task DragAsync(int startX, int startY, int endX, int endY,
               MouseButton button = MouseButton.Left,
               int? durationMs = null,
               CancellationToken cancellationToken = default);
```

**Примеры:**

```csharp
// Перетаскивание с дефолтной кнопкой (Left) и скоростью
Input.Mouse.Drag(100, 100, 500, 500);

// Перетаскивание правой кнопкой за 800мс
Input.Mouse.Drag(200, 200, 600, 400, MouseButton.Right, 800);

// Асинхронное перетаскивание
await Input.Mouse.DragAsync(300, 300, 700, 700, MouseButton.Left, 600);
```

---

### Интерфейс IVirtualKeyboard

#### Методы набора текста

```csharp
// Набор текста (автоматически обрабатывает Shift для заглавных букв)
void TypeText(string text);
```

**Примеры:**

```csharp
// Набор текста с автоматическим Shift
Input.Keyboard.TypeText("Hello, World!");

// Набор текста с символами
Input.Keyboard.TypeText("Test123!@#");

// Многострочный текст
Input.Keyboard.TypeText("Строка 1\nСтрока 2\nСтрока 3");
```

#### Методы нажатия клавиш

```csharp
// Нажатие отдельной клавиши (press + release)
void KeyPress(VirtualKey key);

// Нажать клавишу (без отпускания)
void KeyDown(VirtualKey key);

// Отпустить клавишу
void KeyUp(VirtualKey key);
```

**Примеры:**

```csharp
// Нажать Enter
Input.Keyboard.KeyPress(VirtualKey.Enter);

// Нажать Escape
Input.Keyboard.KeyPress(VirtualKey.Escape);

// Ручное управление Shift (для специальных сценариев)
Input.Keyboard.KeyDown(VirtualKey.LeftShift);
Input.Keyboard.TypeText("uppercase");  // Будет набрано "UPPERCASE"
Input.Keyboard.KeyUp(VirtualKey.LeftShift);
```

#### Методы комбинаций клавиш

```csharp
// Комбинация с одним модификатором
void Chord(VirtualKey modifier, VirtualKey key);

// Комбинация с несколькими модификаторами
void Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key);
```

**Примеры:**

```csharp
// Ctrl+C (копировать)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.C);

// Ctrl+V (вставить)
Input.Keyboard.Chord(VirtualKey.LeftControl, VirtualKey.V);

// Alt+F4 (закрыть окно)
Input.Keyboard.Chord(VirtualKey.LeftAlt, VirtualKey.F4);

// Win+R (диалог "Выполнить")
Input.Keyboard.Chord(VirtualKey.Windows, VirtualKey.R);

// Ctrl+Shift+Esc (диспетчер задач)
Input.Keyboard.Chord(
    new[] { VirtualKey.LeftControl, VirtualKey.LeftShift },
    VirtualKey.Escape
);

// Shift+Win+S (скриншот области)
Input.Keyboard.Chord(
    new[] { VirtualKey.LeftShift, VirtualKey.Windows },
    VirtualKey.S
);
```

---

### Enum MouseButton

```csharp
public enum MouseButton : byte
{
    Left = 1,    // Левая кнопка (основная)
    Right = 2,   // Правая кнопка (контекстное меню)
    Middle = 3   // Средняя кнопка (колесико)
}
```

---

### Enum VirtualKey

Ключевые клавиши (полный список в `API/VirtualKey.cs`):

```csharp
// Буквы A-Z
VirtualKey.A, VirtualKey.B, ..., VirtualKey.Z

// Цифры 0-9 (на основной клавиатуре)
VirtualKey.D0, VirtualKey.D1, ..., VirtualKey.D9

// Функциональные клавиши
VirtualKey.F1, VirtualKey.F2, ..., VirtualKey.F12

// Модификаторы
VirtualKey.LeftControl   // Левый Ctrl
VirtualKey.RightControl  // Правый Ctrl
VirtualKey.LeftShift     // Левый Shift
VirtualKey.RightShift    // Правый Shift
VirtualKey.LeftAlt       // Левый Alt
VirtualKey.RightAlt      // Правый Alt (AltGr)
VirtualKey.Windows       // Клавиша Windows

// Управляющие клавиши
VirtualKey.Enter         // Enter
VirtualKey.Escape        // Esc
VirtualKey.Backspace     // Backspace
VirtualKey.Tab           // Tab
VirtualKey.Space         // Пробел
VirtualKey.Delete        // Delete
VirtualKey.Home          // Home
VirtualKey.End           // End
VirtualKey.PageUp        // Page Up
VirtualKey.PageDown      // Page Down

// Стрелки
VirtualKey.Left          // Стрелка влево
VirtualKey.Right         // Стрелка вправо
VirtualKey.Up            // Стрелка вверх
VirtualKey.Down          // Стрелка вниз
```

---

## Конфигурация

Класс `Configuration` позволяет настраивать поведение автоматизации в реальном времени.

### Параметры движения мыши

```csharp
// Длительность человекоподобных движений мыши (в миллисекундах)
Configuration.MouseMovementDuration = 400;  // Дефолт: 400мс
```

**Рекомендации:**
- `0` - мгновенное движение (как MoveTo)
- `100-300` - быстрое движение
- `300-500` - естественное движение (рекомендуется)
- `500+` - медленное движение

```csharp
// Степень рандомизации кривой Безье (0.0 - 1.0)
Configuration.CurveRandomization = 0.10;  // Дефолт: 0.10 (10%)
```

**Рекомендации:**
- `0.0` - детерминированная траектория (без рандомизации)
- `0.05-0.15` - естественная вариация (рекомендуется)
- `0.2+` - заметная рандомизация
- `0.5+` - хаотичное движение

### Параметры кликов мыши

```csharp
// Задержка между нажатием и отпусканием кнопки мыши (в миллисекундах)
Configuration.ClickDuration = 50;  // Дефолт: 50мс

// Пауза перед и после операции drag (в миллисекундах)
Configuration.DragPauseMs = 50;  // Дефолт: 50мс
```

**Рекомендации:**
- `0-30` - очень быстрые клики
- `30-100` - нормальные клики
- `100-200` - медленные клики

### Параметры клавиатуры

```csharp
// Задержка между нажатиями клавиш (в миллисекундах)
Configuration.KeystrokeDelay = 75;  // Дефолт: 75мс

// Задержка после нажатия модификаторов (Ctrl, Shift, Alt)
Configuration.ModifierHoldDuration = 100;  // Дефолт: 100мс
```

**Рекомендации KeystrokeDelay:**
- `0-30` - очень быстрая печать (робот)
- `30-50` - быстрая печать
- `50-100` - нормальная печать (рекомендуется)
- `100+` - медленная печать

### Примеры профилей конфигурации

#### Профиль "Скорость" (максимальная производительность)

```csharp
Configuration.MouseMovementDuration = 100;
Configuration.CurveRandomization = 0.0;
Configuration.ClickDuration = 10;
Configuration.KeystrokeDelay = 20;
Configuration.ModifierHoldDuration = 50;
Configuration.DragPauseMs = 10;
```

#### Профиль "Естественный" (баланс скорости и естественности)

```csharp
Configuration.MouseMovementDuration = 400;
Configuration.CurveRandomization = 0.10;
Configuration.ClickDuration = 50;
Configuration.KeystrokeDelay = 75;
Configuration.ModifierHoldDuration = 100;
Configuration.DragPauseMs = 50;
```

#### Профиль "Человек" (обход антибот-систем)

```csharp
Configuration.MouseMovementDuration = 500;
Configuration.CurveRandomization = 0.15;
Configuration.ClickDuration = 80;
Configuration.KeystrokeDelay = 120;
Configuration.ModifierHoldDuration = 150;
Configuration.DragPauseMs = 100;
```

---

## Асинхронные операции

Библиотека поддерживает асинхронные версии длительных операций для неблокирующего выполнения.

### Async методы

```csharp
// Асинхронное плавное движение мыши
Task MoveHumanAsync(int x, int y, int? durationMs = null,
                    CancellationToken cancellationToken = default);

// Асинхронное перетаскивание
Task DragAsync(int startX, int startY, int endX, int endY,
               MouseButton button = MouseButton.Left,
               int? durationMs = null,
               CancellationToken cancellationToken = default);
```

### Примеры использования

#### Базовое использование async/await

```csharp
using System;
using System.Threading.Tasks;
using SendSequenceCL;

class Program
{
    static async Task Main()
    {
        // Асинхронное движение мыши
        await Input.Mouse.MoveHumanAsync(800, 400, 500);
        Console.WriteLine("Движение завершено!");

        // Асинхронное перетаскивание
        await Input.Mouse.DragAsync(100, 100, 500, 500, MouseButton.Left, 600);
        Console.WriteLine("Перетаскивание завершено!");

        Input.Dispose();
    }
}
```

#### Отмена операции с CancellationToken

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SendSequenceCL;

class Program
{
    static async Task Main()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        // Отмена через 2 секунды
        cts.CancelAfter(2000);

        try
        {
            // Длительное движение (5 секунд)
            await Input.Mouse.MoveHumanAsync(1500, 800, 5000, cts.Token);
            Console.WriteLine("Движение завершено");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Движение было прервано!");
        }
        finally
        {
            cts.Dispose();
            Input.Dispose();
        }
    }
}
```

#### Параллельное выполнение с UI

```csharp
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SendSequenceCL;

// В WinForms или WPF приложении
private async void Button_Click(object sender, EventArgs e)
{
    // UI не блокируется во время выполнения
    await Input.Mouse.MoveHumanAsync(500, 500, 1000);

    MessageBox.Show("Движение мыши завершено!");
}
```

---

## Обработка ошибок

### Иерархия исключений

```
SendSequenceException (базовый класс)
├── DriverNotFoundException          - Драйвер не найден/не установлен
├── DriverCommunicationException     - Ошибка связи с драйвером (повторяемая)
├── InvalidCoordinateException       - Координаты вне границ экрана
└── InvalidKeyCodeException          - Неподдерживаемый код клавиши
```

### Описание исключений

#### DriverNotFoundException

Выбрасывается когда HID-драйвер Tetherscript не найден в системе.

**Возможные причины:**
- Драйвер не установлен
- ControlMyJoystick не запущен
- Виртуальные устройства не активированы

**Решение:**
1. Установите ControlMyJoystick
2. Запустите программу ControlMyJoystick
3. Убедитесь что устройства активированы в Диспетчере устройств

```csharp
try
{
    Input.Mouse.MoveTo(500, 500);
}
catch (DriverNotFoundException ex)
{
    Console.WriteLine("Ошибка: Драйвер Tetherscript не найден!");
    Console.WriteLine("Решение: Установите и запустите ControlMyJoystick");
    Console.WriteLine($"Детали: {ex.Message}");
}
```

#### DriverCommunicationException

Выбрасывается при ошибках связи с HID-драйвером.

**Возможные причины:**
- Драйвер временно недоступен
- Конфликт с другими приложениями
- Системная ошибка HID

**Решение:**
- Повторите операцию
- Перезапустите ControlMyJoystick
- Проверьте Диспетчер устройств на ошибки

```csharp
int retries = 3;
for (int i = 0; i < retries; i++)
{
    try
    {
        Input.Mouse.MoveTo(500, 500);
        break; // Успех
    }
    catch (DriverCommunicationException ex)
    {
        Console.WriteLine($"Попытка {i+1}/{retries} не удалась: {ex.Message}");
        if (i == retries - 1)
            throw; // Последняя попытка - пробросить исключение

        Thread.Sleep(500); // Подождать перед повтором
    }
}
```

#### InvalidCoordinateException

Выбрасывается когда координаты находятся за пределами экрана.

```csharp
try
{
    // Попытка переместить курсор за пределы экрана
    Input.Mouse.MoveTo(50000, 50000);
}
catch (InvalidCoordinateException ex)
{
    Console.WriteLine($"Неверные координаты: {ex.Message}");
    // Использовать безопасные координаты
    Input.Mouse.MoveTo(960, 540);
}
```

#### InvalidKeyCodeException

Выбрасывается при попытке использовать неподдерживаемый код клавиши.

```csharp
try
{
    VirtualKey invalidKey = (VirtualKey)255; // Неподдерживаемое значение
    Input.Keyboard.KeyPress(invalidKey);
}
catch (InvalidKeyCodeException ex)
{
    Console.WriteLine($"Неподдерживаемая клавиша: {ex.Message}");
}
```

### Комплексная обработка ошибок

```csharp
using System;
using SendSequenceCL;

class Program
{
    static void Main()
    {
        try
        {
            // Попытка автоматизации
            Input.Mouse.MoveHuman(800, 600);
            Input.Mouse.Click(MouseButton.Left);
            Input.Keyboard.TypeText("Test");
        }
        catch (DriverNotFoundException ex)
        {
            Console.WriteLine("КРИТИЧЕСКАЯ ОШИБКА: Драйвер не найден");
            Console.WriteLine("Установите ControlMyJoystick и перезапустите программу");
            Environment.Exit(1);
        }
        catch (DriverCommunicationException ex)
        {
            Console.WriteLine($"Ошибка связи с драйвером: {ex.Message}");
            Console.WriteLine("Попробуйте перезапустить ControlMyJoystick");
        }
        catch (InvalidCoordinateException ex)
        {
            Console.WriteLine($"Ошибка координат: {ex.Message}");
            Console.WriteLine("Проверьте разрешение экрана и используйте корректные координаты");
        }
        catch (SendSequenceException ex)
        {
            Console.WriteLine($"Общая ошибка библиотеки: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Непредвиденная ошибка: {ex.Message}");
        }
        finally
        {
            // Всегда освобождать ресурсы
            Input.Dispose();
        }
    }
}
```

---

## Устранение неполадок

### Проблема: "DriverNotFoundException" при запуске

**Симптомы:**
```
DriverNotFoundException: HID device with VID=0xF00F PID=0x0002 not found.
```

**Решение:**

1. **Проверьте установку драйвера:**
   - Откройте Диспетчер устройств
   - Найдите раздел "Устройства HID (Human Interface Devices)"
   - Убедитесь что присутствуют устройства Tetherscript (VID=0xF00F)

2. **Запустите ControlMyJoystick:**
   - Найдите ярлык ControlMyJoystick на рабочем столе
   - Запустите программу
   - Убедитесь что устройства активированы

3. **Переустановите драйвер:**
   - Скачайте установщик ControlMyJoystick
   - Запустите установку
   - Следуйте инструкциям установщика

### Проблема: Курсор не двигается или движется рывками

**Возможные причины и решения:**

1. **Слишком маленькое значение MouseMovementDuration:**
   ```csharp
   // Проблема
   Configuration.MouseMovementDuration = 10; // Слишком быстро

   // Решение
   Configuration.MouseMovementDuration = 400; // Плавное движение
   ```

2. **Конфликт с другими приложениями:**
   - Закройте другие программы автоматизации
   - Убедитесь что только одно приложение использует драйвер

3. **Системная нагрузка:**
   - Закройте ресурсоемкие приложения
   - Проверьте загрузку CPU

### Проблема: Набор текста работает некорректно

**Симптомы:**
- Пропускаются символы
- Неправильная обработка заглавных букв
- Символы набираются неверно

**Решение:**

1. **Увеличьте задержку между нажатиями:**
   ```csharp
   Configuration.KeystrokeDelay = 100; // Увеличить с 75 до 100мс
   ```

2. **Проверьте раскладку клавиатуры:**
   - Библиотека работает с US-раскладкой
   - Переключитесь на английскую раскладку для тестирования

3. **Убедитесь что целевое приложение активно:**
   ```csharp
   // Клик в окно перед набором текста
   Input.Mouse.MoveHuman(500, 300);
   Input.Mouse.Click(MouseButton.Left);
   Input.Wait(200); // Подождать активации окна
   Input.Keyboard.TypeText("текст");
   ```

### Проблема: Приложение зависает при использовании библиотеки

**Причина:** Использование синхронных методов в UI-потоке

**Решение:** Используйте асинхронные версии методов

```csharp
// Плохо (блокирует UI)
private void Button_Click(object sender, EventArgs e)
{
    Input.Mouse.MoveHuman(800, 600, 2000); // UI заблокирован на 2 секунды
}

// Хорошо (не блокирует UI)
private async void Button_Click(object sender, EventArgs e)
{
    await Input.Mouse.MoveHumanAsync(800, 600, 2000); // UI остается отзывчивым
}
```

### Проблема: Ошибка доступа к устройству когда ControlMyJoystick запущен

**Симптом:**
```
DriverCommunicationException: Failed to open device
```

**Решение:**
Это нормальное поведение. Библиотека автоматически использует режим совместного доступа:
- Сначала пытается открыть устройство с полным доступом (ReadWrite)
- Если не удается (устройство занято ControlMyJoystick), использует режим "только команды" (FileAccess=0)
- Оба режима полностью функциональны

Если ошибка сохраняется:
1. Перезапустите ControlMyJoystick
2. Перезапустите ваше приложение
3. Проверьте Диспетчер устройств на ошибки драйвера

---

## Архитектура проекта

### Структура решения

```
SendSequenceCL/
├── API/                          # Публичные интерфейсы и типы
│   ├── Input.cs                  # Точка входа (фасад)
│   ├── IVirtualMouse.cs          # Интерфейс мыши
│   ├── IVirtualKeyboard.cs       # Интерфейс клавиатуры
│   ├── Configuration.cs          # Глобальные настройки
│   ├── MouseButton.cs            # Enum кнопок мыши
│   ├── VirtualKey.cs             # Enum клавиш клавиатуры
│   └── Exceptions.cs             # Пользовательские исключения
│
├── Core/                         # Бизнес-логика
│   ├── MouseController.cs        # Реализация IVirtualMouse
│   ├── KeyboardController.cs     # Реализация IVirtualKeyboard
│   ├── BezierCurveGenerator.cs   # Генератор кривых Безье
│   ├── ScreenUtilities.cs        # Утилиты экрана (координаты)
│   └── KeyboardMapper.cs         # Маппинг символов на клавиши
│
└── Infrastructure/               # Низкоуровневое взаимодействие с HID
    ├── HidDeviceManager.cs       # Обнаружение и подключение к HID
    ├── HidCommunicator.cs        # Отправка команд в HID
    ├── NativeMethods.cs          # P/Invoke для Windows API
    ├── HidStructures.cs          # Структуры данных HID
    └── DriverConstants.cs        # Константы драйвера (VID/PID)

Examples/                         # Примеры использования
└── Program.cs                    # 7 демонстрационных примеров
```

### Поток данных

```
Приложение пользователя
    ↓
Input.Mouse / Input.Keyboard (API Layer)
    ↓
MouseController / KeyboardController (Core Layer)
    ↓
ScreenUtilities / BezierCurveGenerator (Core Utilities)
    ↓
HidCommunicator (Infrastructure Layer)
    ↓
HidDeviceManager (Infrastructure Layer)
    ↓
Windows HID API (SetupAPI, hid.dll)
    ↓
Tetherscript HID Virtual Driver
    ↓
Системный курсор / Клавиатура
```

### Ключевые компоненты

#### Input.cs - Фасад

Статический класс-синглтон с ленивой инициализацией HID-устройств:
- Первое обращение к `Input.Mouse` подключает mouse HID-драйвер
- Первое обращение к `Input.Keyboard` подключает keyboard HID-драйвер
- `Input.Dispose()` освобождает ресурсы

#### BezierCurveGenerator.cs

Генерирует плавные траектории движения мыши:
- Использует кубические кривые Безье
- Добавляет рандомизацию в контрольные точки
- Возвращает массив точек для интерполяции

#### HidDeviceManager.cs

Управляет жизненным циклом HID-устройств:
- Перечисляет все HID-устройства в системе (SetupAPI)
- Фильтрует по VID/PID (0xF00F)
- Открывает устройство с fallback для совместного доступа
- Реализует IDisposable для корректного освобождения handles

#### HidCommunicator.cs

Отправляет команды в HID-драйвер:
- `SendMouseAbsolute()` - абсолютное позиционирование мыши
- `SendKeyboardReport()` - отправка состояния клавиатуры
- Использует HidD_SetFeature для передачи данных

---

## Лицензия

См. файл LICENSE для деталей.

---

## Благодарности

Библиотека построена на основе **Tetherscript HID Virtual Driver Kit**.

---

## Дополнительные ресурсы

- **Примеры кода:** См. проект `Examples/Program.cs`
- **Архитектура:** См. раздел "Архитектура проекта"
- **Исходный код:** https://github.com/yourusername/SendSequenceCL

---

**Версия:** 1.0.0
**Дата обновления:** 2025-10-21
**Автор:** SendSequenceCL Team
