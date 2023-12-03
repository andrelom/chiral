namespace Chiral.Console.Utilities;

public static class ConsoleUtility
{
    public static void ClearCharacters(int total)
    {
        // '\x1b': This is the escape character.
        //
        // '\x1b[D': Move the cursor one character to the left.
        // '\x1b[P': Erase the character at the current cursor position.
        for (var idx = 0; idx < total; idx++)
        {
            System.Console.Write("\x1b[D");
            System.Console.Write("\x1b[P");
        }
    }

    public static void ClearEntireScreen()
    {
        // '\x1b': This is the escape character.
        //
        // '\x1b[2J': This sequence instructs the terminal to clear the entire screen.
        //            '[2': Specifies the type of operation, where 2 indicates the clear operation.
        //            'J': Clears part of the screen. In this case, it clears the entire screen
        //                 because 2 is specified.
        //
        // '\x1b[3J': This is another clear screen operation. The 3 in '[3J' represents a
        //            different type of clear operation, but in practice, it might have a
        //            similar effect to '[2J' depending on the terminal.
        //
        // '\x1b[H': This sequence moves the cursor to the home position (top-left corner) of the screen.
        //           'H': Stands for "Home" and sets the cursor position. In this case, it
        //                doesn't have additional parameters, so it defaults to the top-left corner.
        System.Console.WriteLine("\x1b[2J\x1b[3J\x1b[H");
    }
}
