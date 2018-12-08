/**
 * Floater: A minimalistic floating web browser with cool superpowers :p
 * Developer: Mohammed Sazid Al Rashid
 * LICENSE: MIT | See the LICENSE file for more information
 * https://github.com/sazid/Floater/
 * https://linkedin.com/in/sazidz/
 */

using System;
using System.Windows.Input;

namespace Floater.Utils
{
    public class WindowCommand : ICommand
    {
        //Set this delegate when you initialize a new object. This is the method the command will execute. You can also change this delegate type if you need to.
        public Action ExecuteDelegate { get; set; }

        //always called before executing the command, mine just always returns true
        public bool CanExecute(object parameter)
        {
            return true; //mine always returns true, yours can use a new CanExecute delegate, or add custom logic to this method instead.
        }

        public event EventHandler CanExecuteChanged; //i'm not using this, but it's required by the interface

        //the important method that executes the actual command logic
        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
            {
                ExecuteDelegate();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

}
