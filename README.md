# ReusableExtensions

A set of helper extension methods that are used very often when coding

1. _**In() Method**_

Checks to see if a item is part of the quick list

Usage
```c#
"Magic".In("Magic", "Bean", "Stalk");
1.In(1, 2, 3);
```

2. _**string.IsNullOrEmpty() Method**_

Takes an additional bool parameter to see if you want to check whitespace or not

default will check whitespace also.

Usage
```c#
//also checks whitespace by default
"".IsNullOrEmpty(); // return true
" ".IsNullOrEmpty(); // return true

//to not check for whitespace
" ".IsNullOrEmpty(false); //return false
```
Much cleaner than

```c#
string.IsNullOrWhitespace(" ");
```

3. _**IEnumerable.CleanNullOrEmpty() Method**_

Cleans up any null items in IEnumerable if available.

Usage
```c#
var strListWithNullEmptyWs = new List<string>() {"Magic", null, "Bean", "Stalk", "", "Giant", " "};
strListWithNullEmptyWs.CleanNullOrEmpty();

IEnumerable<int?> numEnumerable = new List<int?>() {1, null, 2};
numEnumerable.CleanNullOrEmpty();
```

4. _**IEnumerable.IsNullOrEmpty() Method**_

Check if IEnumerable is null or has no items.

Usage
```c#
var emptyList = new List<string>();
emptyList.IsNullOrEmpty(); // returns true

IEnumerable<int?> numEnumerable = new List<int>();
numEnumerable.IsNullOrEmpty();
```

5 _**comparableItem.IsBetween(lowerbound, upperbound) Method**_

Check if a value is in between two comparable values default comparison type is None and it will include the lower and upper bounds in the comparison
The item being compared needs to be comparable (IComparable<T>)

Usage
```c#
decimal value = 3;
decimal lower = 1;
decimal upper = 3;
var result = value.IsBetween(lower, upper); // returns true

value = 1;
lower = 1;
upper = 3;
result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth); // returns false

value = 1;
lower = 1;
upper = 3;
result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower); // return false

value = 3;
lower = 1;
upper = 3;
result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper); // return false


```
