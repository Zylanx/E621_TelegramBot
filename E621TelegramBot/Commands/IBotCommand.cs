// This code has been modified from TelegramBotTemplate by Franklin89

// MIT License
//
// Copyright (c) 2020 damienbod
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Threading.Tasks;

namespace E621TelegramBot.Commands
{
    public interface IBotCommand
    {
        // So what do we need for this?
        // Ideally it would have some descriptive information on the command for both a help function
        //     and to register the command list on startup.
        // It should also have some ability to set up access control for the different commands.
        // It should have a validation method that is executed before executing the function body.
        // TODO: We need to think about promises and related things. Otherwise there is no way to have a command await another message coming in.
        public string Command { get; }
        public string Description { get; }

        public Task<bool> Validate(); // Need to add in the arguments

        public Task Execute();
    }
}