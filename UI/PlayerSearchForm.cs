#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2008 GameTech
// International, Inc.
#endregion

// TTP 50114

using System;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The form that allows a user to search for a player in a number of ways.
    /// </summary>
    /// <remarks>If a server communication exception occurs while this from is 
    /// processing, it will close with a DialogResult of Abort.</remarks>
    public partial class PlayerSearchForm : GradientForm
    {
        #region Member Variables
        protected WaitForm m_waitForm;
        protected int m_operatorId;
        protected bool m_forceEnglish;
        protected bool m_machineAccounts;
        protected BackgroundWorker m_worker;
        protected Player m_selectedPlayer;
        protected bool m_serverCommFailed;
        protected MagneticCardReader m_magCardReader; // PDTS 1064
        protected bool m_detectedSwipe; // PDTS 1064
        private bool m_tryingCardInsteadOfName = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PlayerSearchForm class.
        /// </summary>
        /// <param name="defaultCardSearch">true if the default search mode is 
        /// by card; otherwise false.</param>
        /// <param name="operatorId">The id of the operator who's players to 
        /// search.</param>
        /// <param name="magCardReader">The mag. card reader used to swipe 
        /// cards.</param>
        /// <param name="machineAccounts">Whether anonymous player accounts are 
        /// linked to a machine.</param>
        /// <param name="forceEnglish">Whether to force English for the 
        /// UI.</param>
        /// <exception cref="System.ArgumentNullException">magCardReader is a 
        /// null reference.</exception>
        public PlayerSearchForm(bool defaultCardSearch, int operatorId, MagneticCardReader magCardReader, bool machineAccounts, bool forceEnglish)
        {
            // PDTS 1064
            if(magCardReader == null)
                throw new ArgumentNullException("magCardReader");

            InitializeComponent();
            SetMaxTextLengths();

            // Start listening for swipes.
            m_magCardReader = magCardReader;
            m_magCardReader.CardSwiped += new MagneticCardSwipedHandler(CardSwiped);

            m_operatorId = operatorId;
            m_machineAccounts = machineAccounts;
            m_forceEnglish = forceEnglish;

            // Do they want to search by cards by default.
            if(!defaultCardSearch)
            {
                m_cardSearchRadio.Checked = false;
                m_nameSearchRadio.Checked = true;

                SearchModeChanged(this, new EventArgs());
            }
            else
                m_cardNumber.Select();
                

            // Change some text on the UI if they are anonymous machine accounts.
            if(m_machineAccounts)
            {
                Text = Resources.SearchMachines;
                m_cardSearchRadio.Text = Resources.SearchByNumber;
                m_cardNumberLabel.Text = Resources.MachineNumber;
                m_okButton.Text = Resources.SelectMachine;
                m_resultsList.DisplayMember = "FullName";
            }
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Sets the MaxLength property of all the text boxes based on
        /// the database.
        /// </summary>
        protected void SetMaxTextLengths()
        {
            m_cardNumber.MaxLength = StringSizes.MaxMagneticCardLength;
            m_firstName.MaxLength = StringSizes.MaxNameLength;
            m_lastName.MaxLength = StringSizes.MaxNameLength;
        }

        /// <summary>
        /// Handles the form's KeyPress event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An KeyPressEventArgs object that contains the 
        /// event data.</param>
        private void KeyPressed(object sender, KeyPressEventArgs e)
        {
            // PDTS 1064
            if(m_cardSearchRadio.Checked && m_magCardReader.ProcessCharacter(e.KeyChar))
                e.Handled = true; // Don't send to the active control.
        }

        // PDTS 1064
        /// <summary>
        /// Handles the MagneticCardReader's CardSwiped event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An MagneticCardSwipeArgs object that contains the
        /// event data.</param>
        void CardSwiped(object sender, MagneticCardSwipeArgs e)
        {
            if(ContainsFocus && m_cardSearchRadio.Checked)
            {
                // Put the card number in the number box.
                m_detectedSwipe = true;
                m_cardNumber.Text = e.CardData;

                // Perform the search
                SearchClick(this, new EventArgs());
            }
        }

        // PDTS 1064
        /// <summary>
        /// Handles the FormClosing event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An FormClosingEventArgs object that contains the 
        /// event data.</param>
        private void FormClose(object sender, FormClosingEventArgs e)
        {
            // Don't listen to the CardSwiped event anymore since we 
            // are closing.
            m_magCardReader.CardSwiped -= CardSwiped;
            m_detectedSwipe = false;
        }

        /// <summary>
        /// Handles when the search by radios' selection changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains the
        /// event data.</param>
        private void SearchModeChanged(object sender, EventArgs e)
        {
            // Disable all controls.
            m_cardNumber.Enabled = false;
            m_errorProvider.SetError(m_cardNumberLabel, string.Empty);

            m_lastName.Enabled = false;
            m_firstName.Enabled = false;

            // Which option is now selected?
            if(m_cardSearchRadio.Checked)
            {
                m_cardNumber.Enabled = true;
                m_cardNumber.Select();
            }
            else // Assume search by name.
            {
                m_lastName.Enabled = true;
                m_firstName.Enabled = true;
                m_firstName.Select();
            }
        }

        /// <summary>
        /// Handles when the user presses enter while a text box has focus.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains the
        /// event data.</param>
        private void EnterSearch(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if((textBox != null) && textBox.Enabled && textBox.Visible && (e.KeyCode == Keys.Enter))
                SearchClick(sender, new EventArgs());
        }

        /// <summary>
        /// Handles the card number's validate event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An CancelEventArgs object that contains the
        /// event data.</param>
        private void CardNumberValidate(object sender, CancelEventArgs e)
        {
            if(m_cardSearchRadio.Checked)
            {
                if(string.IsNullOrEmpty(m_cardNumber.Text.Trim()))
                {
                    e.Cancel = true;
                    m_errorProvider.SetError(m_cardNumberLabel, Resources.CardSearchError);
                }
            }
            else
                m_errorProvider.SetError(m_cardNumberLabel, string.Empty);
        }

        /// <summary>
        /// Handles the search button's click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains the
        /// event data.</param>
        private void SearchClick(object sender, EventArgs e)
        {
            if(!ValidateChildren(ValidationConstraints.Visible))
                return;

            StartPlayerSearch();
        }

        /// <summary>
        /// Starts a thread to search for players and displays a wait form.
        /// </summary>
        protected void StartPlayerSearch()
        {
            try
            {
                // Create the wait form.
                m_waitForm = new WaitForm();
                m_waitForm.WaitImage = Resources.WaitAnimation;
                m_waitForm.CancelButtonVisible = false;
                m_waitForm.ProgressBarVisible = false;
                m_waitForm.Cursor = Cursors.WaitCursor;

                if(m_machineAccounts)
                    m_waitForm.Message = Resources.WaitFormFindingMachines;
                else
                    m_waitForm.Message = Resources.WaitFormFindingPlayers;

                // Set the search params.
                string[] parameters = new string[3];

                if(m_cardSearchRadio.Checked)
                    parameters[0] = m_cardNumber.Text;
                else // Assume search by name.
                {
                    parameters[1] = m_lastName.Text;
                    parameters[2] = m_firstName.Text;
                }
                
                // Create the worker thread and run it.
                m_worker = new BackgroundWorker();
                m_worker.WorkerReportsProgress = false;
                m_worker.WorkerSupportsCancellation = false;
                m_worker.DoWork += new DoWorkEventHandler(SearchPlayers);
                m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SearchPlayersComplete);
                m_worker.RunWorkerAsync(parameters);

                // Block until we are finished searching.
                m_waitForm.ShowDialog(this);

                if (m_tryingCardInsteadOfName && m_resultsList.Items.Count > 0)
                    m_firstName.Text = string.Empty;

                if (m_resultsList.Items.Count == 1)
                    m_resultsList.SelectedIndex = 0;

                if (m_cardSearchRadio.Checked)
                    m_cardNumber.Focus();
                else
                    m_firstName.Focus();
            }
            catch(Exception ex)
            {
                MessageForm.Show(this, ex.Message);
            }
            finally
            {
                if(m_waitForm != null)
                {
                    m_waitForm.Dispose();
                    m_waitForm = null;
                }
            }

            // Did we lose the server?
            if(m_serverCommFailed)
            {
                DialogResult = DialogResult.Abort;
                Close();
            }

            if (m_resultsList.Items.Count == 0) //nothing found
            {
                if (m_nameSearchRadio.Checked) //see if they swiped a card in the name field
                {
                    m_cardSearchRadio.Checked = true;
                    m_tryingCardInsteadOfName = true;

                    if (!m_magCardReader.ProcessString(m_firstName.Text))
                    {
                        m_nameSearchRadio.Checked = true;
                        m_tryingCardInsteadOfName = false;
                    }

                    return;
                }

                if (m_tryingCardInsteadOfName)
                {
                    m_nameSearchRadio.Checked = true;
                    m_cardNumber.Text = string.Empty;
                    m_firstName.Focus();
                    m_firstName.SelectAll();
                    m_tryingCardInsteadOfName = false;
                }
            }

            m_tryingCardInsteadOfName = false;
        }

        /// <summary>
        /// Gets a list of players from the server.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The DoWorkEventArgs object that 
        /// contains the event data.</param>
        private void SearchPlayers(object sender, DoWorkEventArgs e)
        {
            // Set the language.
            if(m_forceEnglish)
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            // Wait a couple of ticks to let the wait form display.
            Thread.Sleep(100);

            // Unbox the search params.
            string[] parameters = (string[])e.Argument;

            if(!string.IsNullOrEmpty(parameters[0])) // Mag. Card
            {
                FindPlayerByCardMessage cardMsg = new FindPlayerByCardMessage();
                cardMsg.MagCardNumber = parameters[0];

                // Send the message.
                try
                {
                    cardMsg.Send();
                }
                catch(ServerCommException ex)
                {
                    throw ex; // Don't repackage the ServerCommException
                }
                catch(Exception ex)
                {
                    throw new ModuleException(string.Format((m_machineAccounts) ? Resources.GetMachineFailed : Resources.GetPlayerFailed, ServerExceptionTranslator.FormatExceptionMessage(ex)), ex);
                }

                if(cardMsg.PlayerId > 0)
                {
                    PlayerListItem item = new PlayerListItem();
                    item.Id = cardMsg.PlayerId;
                    item.LastName = cardMsg.LastName;
                    item.FirstName = cardMsg.FirstName;
                    item.MiddleInitial = cardMsg.MiddleInitial;
                    
                    e.Result = new PlayerListItem[] { item };
                }
            }
            else // First and Last Name
            {
                GetPlayerListMessage listMsg = new GetPlayerListMessage();
                listMsg.LastName = parameters[1];
                listMsg.FirstName = parameters[2];

                // Send the message.
                try
                {
                    listMsg.Send();
                }
                catch(ServerCommException ex)
                {
                    throw ex; // Don't repackage the ServerCommException
                }
                catch(Exception ex)
                {
                    throw new ModuleException(string.Format((m_machineAccounts)? Resources.GetMachineListFailed : Resources.GetPlayerListFailed, ServerExceptionTranslator.FormatExceptionMessage(ex)), ex);
                }

                e.Result = listMsg.Players;
            }
        }

        /// <summary>
        /// Handles the event when the search players BackgroundWorker is 
        /// complete.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RunWorkerCompletedEventArgs object that 
        /// contains the event data.</param>
        private void SearchPlayersComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // Update the results list.
            m_resultsList.BeginUpdate();
            m_resultsList.Items.Clear();

            if(e.Error == null)
            {
                PlayerListItem[] players = e.Result as PlayerListItem[];

                if(players != null)
                    m_resultsList.Items.AddRange(players);
            }
            else // There was an error.
            {
                if(e.Error is ServerCommException)
                    m_serverCommFailed = true;
                else
                    MessageForm.Show(this, e.Error.Message);
            }

            m_resultsList.EndUpdate();

            // Close the wait form.
            m_waitForm.CloseForm();
        }

        /// <summary>
        /// Handles when the current player is selected.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains the
        /// event data.</param>
        private void SelectPlayerClick(object sender, EventArgs e)
        {
            if(m_resultsList.SelectedIndex < 0)
            {
                if(sender == m_okButton)
                    MessageForm.Show(this, (m_machineAccounts) ? Resources.NoMachineSel : Resources.NoPlayerSel);

                return;
            }

            try
            {
                // Create the wait form.
                m_waitForm = new WaitForm();
                m_waitForm.WaitImage = Resources.WaitAnimation;
                m_waitForm.CancelButtonVisible = false;
                m_waitForm.ProgressBarVisible = false;
                m_waitForm.Cursor = Cursors.WaitCursor;

                if(m_machineAccounts)
                    m_waitForm.Message = Resources.WaitFormGettingMachine;
                else
                    m_waitForm.Message = Resources.WaitFormGettingPlayer;

                // Create the worker thread and run it.
                m_worker = new BackgroundWorker();
                m_worker.WorkerReportsProgress = false;
                m_worker.WorkerSupportsCancellation = false;
                m_worker.DoWork += new DoWorkEventHandler(GetPlayer);
                m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetPlayerComplete);
                m_worker.RunWorkerAsync(((PlayerListItem)m_resultsList.SelectedItem).Id);

                // Block until we are finished getting the player.
                m_waitForm.ShowDialog(this);

                if(m_serverCommFailed)
                    DialogResult = DialogResult.Abort;
                else
                    DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageForm.Show(this, ex.Message);
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                if(m_waitForm != null)
                {
                    m_waitForm.Dispose();
                    m_waitForm = null;
                }
            }

            Close();
        }

        /// <summary>
        /// Gets a player from the server.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The DoWorkEventArgs object that 
        /// contains the event data.</param>
        private void GetPlayer(object sender, DoWorkEventArgs e)
        {
            // Set the language.
            if(m_forceEnglish)
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            // Wait a couple of ticks to let the wait form display.
            Thread.Sleep(100);

            // Unbox the param.
            int playerId = (int)e.Argument;

            Player player = null;

            try
            {
                player = new Player(playerId, m_operatorId);
            }
            catch(ServerCommException)
            {
                throw; // Don't repackage the ServerCommException
            }
            catch(ServerException ex)
            {
                throw new ModuleException(string.Format(CultureInfo.CurrentCulture, (m_machineAccounts) ? Resources.GetMachineFailed : Resources.GetPlayerFailed, ServerExceptionTranslator.FormatExceptionMessage(ex)) + " " + string.Format(CultureInfo.CurrentCulture, Resources.MessageName, ex.Message), ex);
            }
            catch(Exception ex)
            {
                throw new ModuleException(string.Format(CultureInfo.CurrentCulture, (m_machineAccounts) ? Resources.GetMachineFailed : Resources.GetPlayerFailed, ServerExceptionTranslator.FormatExceptionMessage(ex)), ex);
            }

            e.Result = player;
        }

        /// <summary>
        /// Handles the event when the get player BackgroundWorker is 
        /// complete.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RunWorkerCompletedEventArgs object that 
        /// contains the event data.</param>
        private void GetPlayerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            m_selectedPlayer = null;

            if(e.Error == null)
                m_selectedPlayer = (Player)e.Result;
            else // There was an error.
            {
                if(e.Error is ServerCommException)
                    m_serverCommFailed = true;
                else
                    MessageForm.Show(this, e.Error.Message);
            }

            // Close the wait form.
            m_waitForm.CloseForm();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the player that was selected by the user or null if an error 
        /// occurred.
        /// </summary>
        public Player SelectedPlayer
        {
            get
            {
                return m_selectedPlayer;
            }
        }
        #endregion
    }
}