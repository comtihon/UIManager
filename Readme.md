## UI manager shared library for Space Engineers scripts.

### Usage:

1. Library init

```
uIManager = new UIManager(screens, new List<string> { "keyword1", "keyword2" }, Echo);
```
where:  
__screens__ is a list of all `IMyTextPanel` available on your grid  
`"keyword1", "keyword2"` is the list of __keywords__ to use. UI manager will select only screens containing your keywords (multiple screens for one keyword allowed).  
Echo is __echo__. Every information sent to the screen will be also sent to echo. If you don't want it - pass the empty delegate instead.


2. Set up action callbacks

UI manager allows you to register your functions which will be called on user input. 
If callback returns non-nullable Output object - it will be printed on the screen with 'service' keyword filter.
Object.success = false will turn a message into an error.

```
	    public Output MyCallback(params string[] argument)
            {
                <some code>
                if (<all good>)
                {
                    return new Output(true, "All good");
                }
                else
                {
                    return new Output(false, "Error <error>");
                }
            }

//in your Main
if (!uIManager.hasActions())
    uIManager.registerAction("MyKeyword", MyCallback);
```

3. Call in Main method with provided argument:
```

//in your Main when your argument is not null
if (!uIManager.processAction(argument))
  //uIManager didn't find any action for your argument, handle it here

```
4. Call manually to print info on screens
```
uIManager.printOnScreens("keyword1", myObjectExtendsPrintable);
uIManager.printOnScreens("keyword2", "text", "heading");
```
