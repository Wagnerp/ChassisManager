// Copyright © Microsoft Open Technologies, Inc.
// All Rights Reserved
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
// See the Apache 2 License for the specific language governing permissions and limitations under the License. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.GFS.WCS.WcsCli
{
    internal class ConsoleOperations
    {
        #region internal variables

        /// <summary>
        /// Current thread
        /// </summary>
        Thread currentProcessingThread;

        /// <summary>
        /// The text being written
        /// </summary>
        StringBuilder buffer;

        /// <summary>
        /// Prompt to display
        /// </summary>
        string prompt;

        /// <summary>
        /// Current cursor position
        /// </summary>
        int cursorPtr;

        /// <summary>
        /// Current row
        /// </summary>
        int row;

        /// <summary>
        /// Flag that indicates if processing completed
        /// </summary>
        bool isProcessingCompleted = false;

        /// <summary>
        /// History object to store command history
        /// </summary>
        CommandHistory cmdhistory;


        #endregion        

        /// <summary>
        /// 
        /// </summary>
        internal ConsoleOperations()
        {
            // initilaize WCSCLI prompt
            prompt = WcsCliConstants.consoleString + " ";
            
            // Initialize command history
            cmdhistory = new CommandHistory(20);
        }

        #region Console processing

        /// <summary>
        /// This method processes input received on the console
        /// </summary>
        /// <returns></returns>
        public string ProcessInput()
        {
            currentProcessingThread = Thread.CurrentThread;
            Console.CancelKeyPress += InterrutProcess;

            isProcessingCompleted = false;

            cmdhistory.CursorToEnd();
            DisplayText("");
            cmdhistory.Append("");

            while (!isProcessingCompleted)
            {
                try
                {
                    Handlekeys();
                }
                catch (ThreadAbortException)
                {
                    HandleException();
                }
            }

            Console.WriteLine();

            Console.CancelKeyPress -= InterrutProcess;

            if (buffer == null)
            {
                return null;
            }

            string result = buffer.ToString();
            if (result != "")
                cmdhistory.Accept(result);
            else
                cmdhistory.RemoveLast();
            
            return result;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initial"></param>
        void DisplayText(string textToDisplay)
        {
            buffer = new StringBuilder(textToDisplay);
            cursorPtr = buffer.Length;
            Console.Write(prompt);
            Console.Write(textToDisplay);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newtext"></param>
        void SetText(string newtext)
        {
            if (buffer.Length + prompt.Length > Console.WindowWidth)
            {
                int line = 0;

                // remove the console window width ( except the 8 characters for WCSCLI prompt)
                int countNew = buffer.Length - (Console.WindowWidth - prompt.Length);

                // count the number of lines we need to delete
                while (countNew > 0)
                {
                    line = line + 1;
                    countNew = countNew - Console.WindowWidth;
                }

                // get the count back
                countNew = countNew + Console.WindowWidth;
                
                // clear the last line.
                Console.SetCursorPosition(0, row);
                Console.Write(new String(' ', countNew + 1));

                for (int i = 0; i < line; i++)
                {
                    Console.SetCursorPosition(0, row - (i + 1));
                    Console.Write(new String(' ', Console.WindowWidth));
                }
                row = row - line;
                Console.SetCursorPosition(0, row);

            }
            Console.SetCursorPosition(0, Console.CursorTop);
            DisplayText(newtext);
        }

        /// <summary>
        /// Write text to console
        /// </summary>
        /// <param name="textToWrite"></param>
        void WriteText(string textToWrite)
        {
            Console.Write(textToWrite);
            for (int i = 0; i < textToWrite.Length; i++)
            {
                buffer = buffer.Insert(cursorPtr, textToWrite[i]);
                cursorPtr++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newprompt"></param>
        void SetPrompt(string newprompt)
        {
            prompt = newprompt;
            Console.SetCursorPosition(0, row);
        }

        /// <summary>
        /// 
        /// </summary>
        void Handlekeys()
        {
            ConsoleKeyInfo consoleKey;

            while (!isProcessingCompleted)
            {
                bool isKeyHandled = false;
                consoleKey = Console.ReadKey(true);
                if (consoleKey.Key == ConsoleKey.Escape)
                {
                    buffer.Length = 0;
                    ProcessingComplete();
                    break;
                }

                // handle the key enetered
                switch (consoleKey.Key)
                {
                    case ConsoleKey.Backspace:  isKeyHandled = true;
                                                HandleBackspace();
                                                break;
                    case ConsoleKey.Delete:     isKeyHandled = true;
                                                HandleDeleteChar();
                                                break;
                    case ConsoleKey.LeftArrow: isKeyHandled = true;
                                                HandleLeftArrow();
                                                break;
                    case ConsoleKey.RightArrow: isKeyHandled = true;
                                                HandleRightArrow();
                                                break;
                    case ConsoleKey.DownArrow: isKeyHandled = true;
                                                HandleDownArrow();
                                                break;
                    case ConsoleKey.UpArrow:    isKeyHandled = true;
                                                HandleUpArrow();
                                                break;
                    case ConsoleKey.Enter:      isKeyHandled = true;
                                                ProcessingComplete();
                                                break;
                    case ConsoleKey.Tab:        isKeyHandled = true;
                                                HandleTab();
                                                break;
                    case ConsoleKey.Escape:     isKeyHandled = true;
                                                HandleEscape();
                                                break;
                }

                // Check if key was handled
                if (isKeyHandled)
                {
                    continue;
                }
                else if (consoleKey.KeyChar != (char)0)
                {
                    InsertToBuffer(consoleKey.KeyChar);
                }
            }
        }

        /// <summary>
        /// Insert character to input buffer
        /// </summary>
        /// <param name="c">Char to add</param>
        void InsertToBuffer(char c)
        {
            buffer = buffer.Insert(cursorPtr, c);
            cursorPtr++;
            int prevPosition = Console.CursorLeft;
            Console.SetCursorPosition(prompt.Length, Console.CursorTop);
            Console.Write(buffer);
            Console.SetCursorPosition(prevPosition + 1, Console.CursorTop);
        }

        #endregion

        #region Handle keys

        /// <summary>
        /// Handle left arrow key
        /// </summary>
        void HandleLeftArrow()
        {
            if (cursorPtr == 0 || cursorPtr == cursorPtr -1)
                return;
            cursorPtr = cursorPtr - 1;
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

        /// <summary>
        /// Handle right arrow key
        /// </summary>
        void HandleRightArrow()
        {
            if (cursorPtr == buffer.Length)
                return;
            cursorPtr = cursorPtr + 1;
            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
        }

        /// <summary>
        /// Handle UP arrow key to get the previous command executed
        /// </summary>
        void HandleUpArrow()
        {
            if (!cmdhistory.PreviousAvailable())
            {
                return;
            }

            cmdhistory.Update(buffer.ToString());

            string value = cmdhistory.Previous();

            if (value != null)
            {
                ClearConsoleLine();
                SetText(value);
            }
        }

        /// <summary>
        /// Handle DOWN arrow key to get the next command executed
        /// </summary>
        void HandleDownArrow()
        {
            if (!cmdhistory.NextAvailable())
            {
                return;
            }

            cmdhistory.Update(buffer.ToString());

            string value = cmdhistory.Next();

            if (value != null)
            {
                ClearConsoleLine();
                SetText(value);
            }
        }

        /// <summary>
        /// Indicates processing completed
        /// </summary>
        void ProcessingComplete()
        {
            this.isProcessingCompleted = true;
        }

        /// <summary>
        /// Handle backspace key
        /// </summary>
        void HandleBackspace()
        {
            if (cursorPtr == 0)
                return;
            
            buffer.Remove(--cursorPtr, 1);
            
            int prevPosition = Console.CursorLeft;

            // print backspace and erase the last character from console
            Console.Write("\b");
            Console.Write(" ");
            Console.Write("\b");

            Console.SetCursorPosition(prompt.Length, Console.CursorTop);
            ClearConsoleLine();
            Console.Write(buffer);
            Console.SetCursorPosition(prevPosition - 1, Console.CursorTop);
        }

        /// <summary>
        /// Delete a character from console
        /// </summary>
        void HandleDeleteChar()
        {
            // Check if there is no input
            if (buffer.Length == 0)
            {
                isProcessingCompleted = true;
                buffer = null;
                Console.WriteLine();
                return;
            }
            // Check if buffer length is same as cursor position
            else if (buffer.Length == cursorPtr)
            {
                // Do nothing
                return;
            }
            else
            {
                buffer.Remove(cursorPtr, 1);

                int prevPosition = Console.CursorLeft;
                ClearConsoleLine();
                SetText(buffer.ToString());
                Console.SetCursorPosition(prevPosition, Console.CursorTop);
                cursorPtr = prevPosition - prompt.Length;
            }
        }

        /// <summary>
        /// Handle Tab key, by autocompleting the command
        /// </summary>
        void HandleTab()
        {
            string searchWord;

            if (buffer.ToString().Contains(' '))
            {
                searchWord = buffer.ToString().Split(' ').Last();
            }
            else
            {
                searchWord = buffer.ToString();
            }

            // Send the word string to Auto tab handler
            string[] results = TabHelper.GetTabOptions(searchWord.ToLower());

            if (results == null || results.Length == 0)
                return;

            int count = results.Length;

            // if there is just one matching string, print it to string.
            if (results.Length == 1)
            {
                WriteText(results[0]);
            }
            else
            {
                int last = -1;

                // find the first character that differs from user entered string
                for (int p = 0; p < results[0].Length; p++)
                {
                    char c = results[0][p];

                    for (int i = 1; i < count; i++)
                    {
                        if (results[i].Length < p)
                            goto mismatch;

                        if (results[i][p] != c)
                        {
                            goto mismatch;
                        }
                    }
                    last = p;
                }
            mismatch:
                if (last != -1)
                {
                   WriteText(results[0].Substring(0, last + 1));
                }
                Console.WriteLine();
                int countPerline = 3;
                int cntr = 0;
                foreach (string s in results)
                {
                    Console.Write(searchWord);
                    Console.Write(s);
                    Console.Write(' ');
                    cntr++;
                    if (cntr == countPerline)
                    {
                        Console.WriteLine();
                        cntr = 0;
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
                DisplayText(buffer.ToString());
            }

        }

        void HandleEscape()
        {
            Console.Clear();
        }

        #endregion

        #region Command history operations

        /// <summary>
        /// Clear history buffer
        /// </summary>
        public void ClearConsoleHistory()
        {
            try
            {
                lock (this)
                {
                    if (cmdhistory != null)
                    {
                        // call internal method to clear history
                        cmdhistory.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to clear history for WCSCLI console: " + ex);
            }
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Clear console line
        /// </summary>
        private void ClearConsoleLine()
        {
            // Get current line
            int currentLineCursor = Console.CursorTop;

            Console.SetCursorPosition(prompt.Length, Console.CursorTop);

            // clear line
            Console.Write(new string(' ', Console.WindowWidth - prompt.Length));

            Console.SetCursorPosition(prompt.Length, currentLineCursor);
        }

        /// <summary>
        /// Handle thread exception
        /// </summary>
        private void HandleException()
        {
            // Abort the thread
            Thread.ResetAbort();
            Console.WriteLine();
            SetPrompt(prompt);
            SetText("");
        }
        
        #endregion

        #region end processing

        /// <summary>
        /// This method processes the interupt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        void InterrutProcess(object sender, ConsoleCancelEventArgs a)
        {
            // Do not abort our program:
            a.Cancel = true;

            // Interrupt the editor
            currentProcessingThread.Abort();
        }

        #endregion

    }


}
