﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xaml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using SteamKit2;
using SteamChatBot.Triggers;
using SteamChatBot.Triggers.TriggerOptions;
using SteamChatBot.Triggers.TriggerOptions.Windows;

namespace SteamChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Log Log;
        string logFile;
        string sentryFile;
        string username;
        string password;
        string displayName;
        string fll;
        string cll;
        string chatbots;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream("chatbots.txt", FileMode.Open))
                {
                    using (TextReader tr = new StreamReader(fs))
                    {
                        chatbots = tr.ReadToEnd();
                    }
                }
                if (chatbots.Length > 0)
                {
                    chatbotsBox.ItemsSource = chatbots.Split('\n');
                }
            }
            catch (FileNotFoundException err) { }
        }

        #region file browse dialogs

        private void sentryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                filename = ofd.FileName;
                sentryFileTextBox.Text = filename;
                sentryFile = filename;
            }
        }

        private void logFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                filename = ofd.FileName;
                logFileTextBox.Text = filename;
                logFile = filename;
            }
        }

        #endregion

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text == "" && chatbotsBox.SelectedIndex != -1)
            {
                username = chatbotsBox.SelectedValue.ToString();
                if (File.Exists(username + "/login.json") && passwordBox.Password == "" && sentryFileTextBox.Text == "" &&
                    logFileTextBox.Text == "" && displaynameBox.Text == "" && consoleLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose" &&
                    fileLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose")
                {
                    var _data = Bot.ReadData(username);
                    logFile = _data.logFile;
                    sentryFile = _data.sentryFile;
                    username = _data.username;
                    password = _data.password;
                    displayName = _data.displayName;
                    cll = _data.cll;
                    fll = _data.fll;

                    if (sharedSecretBox.Text != "")
                    {
                        Bot.sharedSecret = sharedSecretBox.Text;
                    }

                    Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true),
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true));

                    Log.Instance.Silly("Successfully read login data from file");
                    Close();
                    Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
                }
            }
            else if (usernameBox.Text != "")
            {
                username = usernameBox.Text;
                if (File.Exists(username + "/login.json") && passwordBox.Password == "" && sentryFileTextBox.Text == "" &&
                    logFileTextBox.Text == "" && displaynameBox.Text == "" && consoleLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose" &&
                    fileLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose")
                {
                    var _data = Bot.ReadData(username);
                    logFile = _data.logFile;
                    sentryFile = _data.sentryFile;
                    username = _data.username;
                    password = _data.password;
                    displayName = _data.displayName;
                    cll = _data.cll;
                    fll = _data.fll;

                    if (sharedSecretBox.Text != "")
                    {
                        Bot.sharedSecret = sharedSecretBox.Text;
                    }

                    Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true),
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true));

                    Log.Instance.Silly("Successfully read login data from file");
                    Close();
                    Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
                }
                else
                {
                    if (passwordBox.Password != "")
                    {
                        object cll = ((ListBoxItem)consoleLLBox.SelectedValue).Content;
                        object fll = ((ListBoxItem)fileLLBox.SelectedValue).Content;

                        Log = Log.CreateInstance((logFileTextBox.Text == "" ? usernameBox.Text + ".log" : logFileTextBox.Text), usernameBox.Text,
                            (cll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Verbose", true) :
                            (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll.ToString(), true)),
                            (fll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Verbose", true) :
                            (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll.ToString(), true)));

                        Log.Instance.Silly("Console started successfully!");
                        if (displaynameBox.Text != "")
                        {
                            Close();
                            if (sharedSecretBox.Text != "")
                            {
                                Bot.sharedSecret = sharedSecretBox.Text;
                            }
                            Bot.Start(usernameBox.Text, passwordBox.Password, (cll == null ? "Verbose" :
                                cll.ToString()), (fll == null ? "Verbose" :
                                fll.ToString()), (logFile == null ? usernameBox.Text + ".log" : logFile),
                                displaynameBox.Text, (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile));

                        }
                        else
                        {
                            MessageBox.Show("Missing Display Name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Missing password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Missing username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Steam-Chat-Bot/SteamChatBot/issues");
        }

        private void plusTriggerButton_Click(object sender, RoutedEventArgs e)
        {
            string selected = "";
            TriggerType type;
            try
            {
                selected = ((ListBoxItem)triggerListBox.SelectedValue).Name;
            }
            catch (Exception err) { return; }


            ChatCommand cc = new ChatCommand();
            ChatCommandApi cca = new ChatCommandApi();
            ChatReply cr = new ChatReply();
            NoCommand nc = new NoCommand();
            TriggerLists tl = new TriggerLists();
            TriggerNumbers tn = new TriggerNumbers();

            if (selected == "isUpTrigger" || selected == "leaveChatTrigger" || selected == "kickTrigger"
                || selected == "banTrigger" || selected == "unbanTrigger" || selected == "lockTrigger"
                || selected == "unlockTrigger" || selected == "moderateTrigger" || selected == "unmoderateTrigger")
            {
                ChatCommandWindow ccw = new ChatCommandWindow();
                ccw.ShowDialog();
                if (ccw.DialogResult.HasValue && ccw.DialogResult.Value)
                {
                    ChatCommand ccw_cc = ccw.CC;
                    cc = GetChatCommandOptions(ccw_cc);
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", cc.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, cc.Name, cc);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "chatReplyTrigger")
            {
                ChatReplyWindow crw = new ChatReplyWindow();
                crw.ShowDialog();
                if (crw.DialogResult.HasValue && crw.DialogResult.Value)
                {
                    ChatReply crw_cr = crw.CR;
                    cr = GetChatReplyOptions(crw_cr);
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", cr.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, cr.Name, cr);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "linkNameTrigger" || selected == "doormatTrigger")
            {
                NoCommandWindow ncw = new NoCommandWindow();
                ncw.ShowDialog();
                if (ncw.DialogResult.HasValue && ncw.DialogResult.Value)
                {
                    NoCommand ncw_nc = ncw.NC;
                    nc = GetNoCommandOptions(ncw_nc);
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", nc.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, nc.Name, nc);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "banCheckTrigger" || selected == "weatherTrigger")
            {
                ChatCommandApiWindow ccaw = new ChatCommandApiWindow();
                ccaw.ShowDialog();
                if (ccaw.DialogResult.HasValue && ccaw.DialogResult.Value)
                {
                    ChatCommandApi ccaw_cca = ccaw.CCA;
                    cca = GetChatCommandApiOptions(ccaw_cca);
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", cca.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, cca.Name, cca);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "acceptFriendRequestTrigger" || selected == "autojoinChatTrigger" || selected == "acceptChatInviteTrigger")
            {
                TriggerListWindow tlw = new TriggerListWindow(selected);
                tlw.ShowDialog();
                if(tlw.DialogResult.HasValue && tlw.DialogResult.Value)
                {
                    TriggerLists tlw_tl = tlw.TL;
                    tl = GetTriggerListOptions(tlw_tl);
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", tl.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, tl.Name, tl);
                    Bot.triggers.Add(trigger);
                }

            }
        }

        private void minusTriggerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedString = ((string)addedTriggersListBox.SelectedValue);
                addedTriggersListBox.Items.Remove(addedTriggersListBox.SelectedValue);
                IEnumerable<BaseTrigger> triggers = Bot.triggers.Where(x => x.Name == selectedString.Substring(0, selectedString.IndexOf(" -")));
                foreach (BaseTrigger trigger in triggers)
                {
                    Bot.triggers.Remove(trigger);
                }
            }
            catch (Exception err) { throw err; }
        }

        private ChatCommand GetChatCommandOptions(ChatCommand c)
        {
            try
            {
                return new ChatCommand
                {
                    Name = c.Name,
                    Command = c.Command,
                    TriggerLists = c.TriggerLists,
                    TriggerNumbers = c.TriggerNumbers
                };
            }
            catch (Exception e) { return null; }
        }

        private ChatReply GetChatReplyOptions(ChatReply c)
        {
            try
            {
                return new ChatReply
                {
                    Name = c.Name,
                    Matches = c.Matches,
                    Responses = c.Responses,
                    TriggerLists = c.TriggerLists,
                    TriggerNumbers = c.TriggerNumbers
                };
            }
            catch (Exception e) { return null; }
        }

        private NoCommand GetNoCommandOptions(NoCommand n)
        {
            try
            {
                return new NoCommand
                {
                    Name = n.Name,
                    TriggerLists = n.TriggerLists,
                    TriggerNumbers = n.TriggerNumbers
                };
            }
            catch (Exception e) { return null; }
        }

        private ChatCommandApi GetChatCommandApiOptions(ChatCommandApi c)
        {
            try
            {
                return new ChatCommandApi
                {
                    ApiKey = c.ApiKey,
                    Name = c.Name,
                    ChatCommand = c.ChatCommand
                };
            }
            catch (Exception e) { return null; }
        }

        private TriggerLists GetTriggerListOptions(TriggerLists tl)
        {
            try
            {
                return new TriggerLists
                {
                    Name = tl.Name,
                    Rooms = tl.Rooms,
                    User = tl.User,
                    Ignore = tl.Ignore
                };
            }
            catch (Exception e) { return null; }
        }
    }
}