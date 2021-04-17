using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Output
        {
            public bool success;
            public string msg;

            public Output(bool success, string msg)
            {
                this.success = success;
                this.msg = msg;
            }
        }

        public abstract class Printable
        {
            public abstract override string ToString();
        }

        public class UIManager
        {
            /**
             * Manage user input and output
             * 
             */
            Dictionary<string, Func<String[], Output>> actions = new Dictionary<string, Func<String[], Output>>(StringComparer.OrdinalIgnoreCase);
            // 'service' keyword is registered for the service info
            Dictionary<string, List<IMyTextPanel>> output = new Dictionary<string, List<IMyTextPanel>>();
            string lastAction = null;
            List<string> errors = new List<string>();
            Action<string> echo;

            public UIManager(List<IMyTextPanel> screens, List<string> filters, Action<string> echo)
            {
                foreach (IMyTextPanel t in screens)
                {
                    foreach (string s in filters)
                    {
                        if (t.CustomName.ToLower().Contains(s))
                        {
                            if (output.ContainsKey(s))
                                output[s].Add(t);
                            else
                                output[s] = new List<IMyTextPanel> { t };
                        }
                    }
                }
                this.echo = echo;
            }

            /**
             * Register your action. It should accept multiple string arguments and should return nullable Output object as an output.
             * In case of Output is returned - it will be saved in lastAction or errors (renderable on 'service' screen)
             * When command with arguments goes in process - only arguments go to your action.
             */
            public void registerAction(string key, Func<String[], Output> action)
            {
                actions.Add(key.ToLower(), action);
                lastAction = "Action " + key + " added";
            }

            public bool processAction(string input)
            {
                if (input == null || input.Length <= 0)
                {
                    errors.Add("Wrong action");
                    return false;
                }
                string[] cmdWithArgs = input.Split(' ');
                string cmd = cmdWithArgs[0];
                string[] args = new List<string>(cmdWithArgs.Skip(1)).ToArray();
                if (actions.ContainsKey(cmd.ToLower()))
                {
                    Output output = actions[input](args);
                    if (output != null)
                    {
                        if (output.success)
                            lastAction = output.msg;
                        else
                            errors.Add(output.msg);
                    }
                    return true;
                }
                return false;
            }

            public bool hasActions()
            {
                return actions.Count > 0;
            }

            /*
             * Print on multiple screens with the same tag. data[0] is the message, data[1] (optional) is the title.
             */
            public void printOnScreens(string screenTag, string message, params string[] data)
            {
                if (output.ContainsKey(screenTag))
                {
                    foreach (IMyTextPanel screen in output[screenTag])
                    {
                        screen.WriteText(data[0]);
                        if (data.Length > 1)
                            screen.WritePublicTitle(data[1]);
                    }
                }
                else
                {
                    errors.Add("No " + screenTag + " found");
                }
                echo(message);
            }

            public void printOnScreens(string screenTag, Printable myObject)
            {
                printOnScreens(screenTag, myObject.ToString());
            }

            public void printServiceInfo()
            {
                string message = "Actions:+\n";
                foreach (KeyValuePair<string, Func<String[], Output>> entry in actions)
                {
                    message += entry.Key + "\n";
                }
                if (lastAction != null)
                {
                    message += "-------------------";
                    message += "Last action done:\n";
                    message += lastAction + "\n";
                }
                if (errors.Count > 0)
                    message += "-------------------";
                foreach (string err in errors)
                    message += err + "\n";
                if (output.ContainsKey("service"))
                {
                    printOnScreens("service", message);
                }
                errors.Clear();
            }
        }
    }
}
