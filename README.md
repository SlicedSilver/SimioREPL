SimioREPL
=========

I've created a tool which makes my life a bit easier when trying to code Design Add-Ins for Simio and I thought that I would share it with everyone else.

What the tool does is allow you to try c# code directly within Simio and evaluate the results. If you have ever used a dynamic programming language like Matlab or Mathematica then it will be familiar to you. Its like the command window in Matlab where you can enter commands without compiling. Its called a REPL (Read-Eval-Print-Loop). more info here: http://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop

The code uses the Microsoft Roslyn Compiler API. more info here: http://en.wikipedia.org/wiki/Microsoft_Roslyn
If you want to play around with the source code then you will probably need to install the Roslyn CTP. Download here: http://msdn.microsoft.com/en-gb/roslyn. I created the solution using Visual Studio 2012 and I believe that the roslyn compiler only works with VS 2012 at the moment.

### How to install:
1. Install dll into either:
`C:\Program Files (x86)\Simio\UserExtensions\SimioREPL\SimioREPL.dll`
or
`C:\Program Files\Simio\UserExtensions\SimioREPL\SimioREPL.dll`
2. The folder structure should be `..\Simio\UserExtensions\SimioREPL`
3. The drop-down code snippet tool (green drop-down) loads stored functions stored in the following file: `c:\SimioReferences\REPL_Functions.txt`.

### How to get started:
1. Create a new model in simio.
2. Select SimioREPL from the Select Add-In drop Menu (Project Home Ribbon)
3. You can now start playing with the interactive coding environment

The bottom textbox displays the results from each execution loop. If no return values are given then it will display just the execution count.

### How to use (here is an example of how to use the tool):
Enter c# code just like in a normal program, for example:
```csharp
    int x = 10;
```
You can find out the current value of a variable by entering its name without the ending semi-colon, for example: 
```csharp
    x or x*2 + 10 -3
```
This will display the value in the output text box at the bottom of the window.
I've added the ability to use `Write()` and `WriteLine()`, just like in a console application. for example:
```csharp
    WriteLine(object.ObjectName);
```

If the code is invalid then the Exception message will be displayed in the results text box, and the input code will remain. If the code is valid then the input code is cleared.

The context object is already defined. Use context as you normally would.

### Some example code Walkthrough:
Add some objects to the model
```csharp
    IIntelligentObjects intelligentObjects = context.ActiveModel.Facility.IntelligentObjects;
    IFixedObject sourceObject = intelligentObjects.CreateObject("Source", new FacilityLocation(-5, 0, -5)) as IFixedObject;
    IFixedObject serverObject = intelligentObjects.CreateObject("Server", new FacilityLocation(0, 0, 0)) as IFixedObject;
    IFixedObject sinkObject = intelligentObjects.CreateObject("Sink", new FacilityLocation(5, 0, 5)) as IFixedObject;
    ILinkObject path1 = intelligentObjects.CreateLink("Path",sourceObject.Nodes[0],serverObject.Nodes[0],null) as ILinkObject;
    ILinkObject path2 = intelligentObjects.CreateLink("Path",serverObject.Nodes[1],sinkObject.Nodes[0],null) as ILinkObject;
```

`[Execute]` <ctrl + enter>  or <press the execute button>


Move the source object
```csharp
    intelligentObjects["Source1"].Location = new FacilityLocation(-4.5, 0, -2);
```
`[Execute]`


List all the nodes on the server object
```csharp
    foreach(var node in serverObject.Nodes)
    {
        WriteLine(node);
    }
```
`[Execute]`


List all the objects in the model
Code:
```csharp
    foreach(var ob in intelligentObjects)
    {
      WriteLine(ob.ObjectName);
    }
```
`[Execute]`
