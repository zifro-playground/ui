[&lt; Back to table of contents](/README.md)

---

# `3.7.` _IDE/Compiler_ - **Global functions**

Global functions are functions that are defined on a separate level compared to the compilers added functions (that you set via `PMWrapper.SetCompilerFunctions`). These are accessible in all games (that uses the right version) and do pretty basic stuff, but they're nice to have.

## Added global functions

> In the list a question mark after a parameter means that it's optional.

### Converters between datatypes

Name & Args | Description | Example | Result
----------- | ----------- | ------- | ------
`bool(value?)` | Tries to convert a given value to the boolean representation. See conversion specs over at pythons docs for [Truth Value Testing](https://docs.python.org/2/library/stdtypes.html#truth-value-testing)   | `x = bool("hello")`<br>`y = bool(0)` | `x: True`<br>`y: False`
`float(value?)` | Tries to convert a given value to the floating point representation. | `x = float("20.33")` | `x: 20.33`
`int(value?,base?)` | Tries to convert a given value to the integer representation. Second parameter defines which base to convert from. | `x = int(2.3)`<br>`y = int("ff", 16)` | `x: 2`<br>`y: 255`
`long(value?,base?)` | (identical to `int`) | `x = long("19")`<br>`y = long()` | `x: 19`<br>`y: 0`
`str(value?)` | Tries to convert a given value to the string representation. | `x = str(True)`<br>`y = str(0.5)` | `x: "True"`<br>`y: "0.5"`

### Math functions

Name & Args | Description | Example | Result
----------- | ----------- | ------- | ------
`abs(number)` | Returns the absolute value of a number. | `x = abs(3)`<br>`y = abs(-8)` | `x: 3`<br>`y: 8`
`round(number)` | Rounds a number to the closest integer. | `x = round(12.33)`<br>`y = round(5.5)` | `x: 12`<br>`y: 6`
`min(a,b?,c?,d?,e?,...)` | Returns the lowest value among numerous values. | `x = min(1,2,-3)`<br>`y = min("a","b")` | `x: -3`<br>`y: "a"`
`max(a,b?,c?,d?,e?,...)` | Returns the highest value among numerous values. | `x = max(1,2,-3)`<br>`y = max(False, True)` | `x: 2`<br>`y: True`

### Miscellaneous functions

Name & Args | Description | Example | Result
----------- | ----------- | ------- | ------
`bin(number)` | Converts a number to the string version of that number in binary. | `x = bin(43)` | `x: "101011"`
`hex(number)` | Converts a number to the string version of that number in hexadecimal. | `x = hex(42)` | `x: "2a"`
`oct(number)` | Converts a number to the string version of that number in octal. | `x = oct(25)` | `x: "31"`
`len(string)` | Returns the number of characters in a string. | `x = len("Hello world!")` | `x: 12`
`print(value)` | Shows a popup bubble with the value inside. | `print("Hello World!")` | _N/A_
`time()` | Gets the Unix time. I.e. the number of seconds since 00:00 1st of January 1970 in UTC | `x = time()` | `x: 1486747644`
~~_hax()_~~ | **DEPRECATED!** Replacement: [`5.4.`&nbsp;Miscellaneous - Hax](/content/5-hax.md)<br><br>_~~If this function is executed, while the users holds down `LEFT SHIFT` and `RIGHT SHIFT`, the current level is marked as complete. This was the use to intentionally skip levels.~~_ | _~~hax()~~_ | _~~N/A~~_

---

[&lt; Back to table of contents](/README.md)
