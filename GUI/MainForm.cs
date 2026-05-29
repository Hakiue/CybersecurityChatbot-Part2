using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using CybersecurityChatbot.ChatbotCore;
using static CybersecurityChatbot.ChatbotCore.ChatDelegates;

namespace CybersecurityChatbot.GUI
{
    /// <summary>
    /// Main WinForms window for the Cybersecurity Awareness Chatbot.
    /// Implements all Part 2 requirements: GUI, keyword recognition,
    /// random responses, conversation flow, memory, sentiment detection,
    /// error handling, and delegates.
    /// </summary>
    public class MainForm : Form
    {
        // ── Core chatbot components ────────────────────────────────────────────
        private readonly ResponseEngine      _responseEngine;
        private readonly SentimentDetector   _sentimentDetector;
        private readonly ConversationMemory  _memory;

        // ── Delegates ──────────────────────────────────────────────────────────
        private MessageProcessor  _messageProcessor;
        private BotResponseHandler _responseHandler;
        private ActivityLogger    _activityLogger;

        // ── State ──────────────────────────────────────────────────────────────
        private bool   _nameCollected = false;
        private string _userName      = "User";

        // ── Colours ────────────────────────────────────────────────────────────
        private static readonly Color DarkBg      = Color.FromArgb(13,  17,  23);
        private static readonly Color PanelBg     = Color.FromArgb(22,  27,  34);
        private static readonly Color InputBg     = Color.FromArgb(33,  38,  45);
        private static readonly Color AccentBlue  = Color.FromArgb(88, 166, 255);
        private static readonly Color AccentGreen = Color.FromArgb(63, 185, 80);
        private static readonly Color AccentRed   = Color.FromArgb(255,  85,  85);
        private static readonly Color AccentYellow= Color.FromArgb(255, 200,  87);
        private static readonly Color AccentPurple= Color.FromArgb(188, 140, 255);
        private static readonly Color TextPrimary = Color.FromArgb(230, 237, 243);
        private static readonly Color TextMuted   = Color.FromArgb(125, 133, 144);
        private static readonly Color BorderColor = Color.FromArgb(48,  54,  61);

        // ── Controls ───────────────────────────────────────────────────────────
        private RichTextBox _chatDisplay   = null!;
        private TextBox     _inputBox      = null!;
        private Button      _sendButton    = null!;
        private Label       _headerLabel   = null!;
        private Label       _asciiLabel    = null!;
        private Label       _statusLabel   = null!;
        private Panel       _topPanel      = null!;
        private Panel       _chatPanel     = null!;
        private Panel       _inputPanel    = null!;

        // ─────────────────────────────────────────────────────────────────────
        public MainForm()
        {
            _responseEngine    = new ResponseEngine();
            _sentimentDetector = new SentimentDetector();
            _memory            = new ConversationMemory();

            // Wire up delegates
            _messageProcessor = FollowUpProcessor(_responseEngine);

            _responseHandler = (sender, message, type) =>
            {
                // This delegate handles all bot output — routes to the display
                AppendMessage(sender, message, type);
            };

            _activityLogger = (activity) =>
            {
                // Log to status bar
                if (_statusLabel != null)
                    _statusLabel.Text = $"  {activity}";
            };

            InitialiseForm();
            InitialiseComponents();
            PlayVoiceGreeting();
            ShowWelcomeSequence();
        }

        // ── Form initialisation ───────────────────────────────────────────────

        private void InitialiseForm()
        {
            Text            = "🛡 Cybersecurity Awareness Bot — Part 2";
            Size            = new Size(950, 750);
            MinimumSize     = new Size(800, 600);
            BackColor       = DarkBg;
            ForeColor       = TextPrimary;
            Font            = new Font("Segoe UI", 10f);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void InitialiseComponents()
        {
            // ── Top header panel ─────────────────────────────────────────────
            _topPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 160,
                BackColor = PanelBg,
                Padding   = new Padding(16, 8, 16, 8),
            };

            // ASCII art label
            _asciiLabel = new Label
            {
                Text      = GetAsciiArt(),
                Font      = new Font("Courier New", 6.5f, FontStyle.Bold),
                ForeColor = AccentBlue,
                BackColor = PanelBg,
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            // Header subtitle
            _headerLabel = new Label
            {
                Text      = "🇿🇦  Department of Cybersecurity  •  Awareness Campaign 2026  •  Protecting South African Citizens",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = TextMuted,
                BackColor = PanelBg,
                Dock      = DockStyle.Bottom,
                Height    = 24,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            _topPanel.Controls.Add(_asciiLabel);
            _topPanel.Controls.Add(_headerLabel);

            // ── Chat display ─────────────────────────────────────────────────
            _chatPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = DarkBg,
                Padding   = new Padding(12, 8, 12, 8),
            };

            _chatDisplay = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = DarkBg,
                ForeColor   = TextPrimary,
                Font        = new Font("Segoe UI", 10.5f),
                ReadOnly    = true,
                BorderStyle = BorderStyle.None,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                WordWrap    = true,
            };

            _chatPanel.Controls.Add(_chatDisplay);

            // ── Input panel ──────────────────────────────────────────────────
            _inputPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 60,
                BackColor = PanelBg,
                Padding   = new Padding(12, 10, 12, 10),
            };

            _sendButton = new Button
            {
                Text      = "Send  ➤",
                Dock      = DockStyle.Right,
                Width     = 110,
                BackColor = AccentBlue,
                ForeColor = DarkBg,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
            };
            _sendButton.FlatAppearance.BorderSize = 0;
            _sendButton.Click += SendButton_Click;

            _inputBox = new TextBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = InputBg,
                ForeColor   = TextPrimary,
                Font        = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.None,
                PlaceholderText = "Type a message or cybersecurity topic...",
            };
            _inputBox.KeyDown += InputBox_KeyDown;

            _inputPanel.Controls.Add(_inputBox);
            _inputPanel.Controls.Add(_sendButton);

            // ── Status bar ───────────────────────────────────────────────────
            _statusLabel = new Label
            {
                Dock      = DockStyle.Bottom,
                Height    = 24,
                BackColor = Color.FromArgb(8, 12, 16),
                ForeColor = TextMuted,
                Font      = new Font("Segoe UI", 8.5f),
                Text      = "  Ready",
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // ── Add to form ──────────────────────────────────────────────────
            Controls.Add(_chatPanel);
            Controls.Add(_inputPanel);
            Controls.Add(_topPanel);
            Controls.Add(_statusLabel);
        }

        // ── Startup sequence ──────────────────────────────────────────────────

        private void PlayVoiceGreeting()
        {
            try { VoiceGreeting.Play(); }
            catch { /* graceful fallback */ }
        }

        private void ShowWelcomeSequence()
        {
            AppendMessage("System", "════════════════════════════════════════════════════════", MessageType.System);
            AppendMessage("System", "🛡  CYBERSECURITY AWARENESS BOT  —  Part 2", MessageType.System);
            AppendMessage("System", "════════════════════════════════════════════════════════", MessageType.System);
            AppendMessage("Bot", "Hello! Welcome to the Cybersecurity Awareness Bot. " +
                "I'm here to help you stay safe online. 🛡", MessageType.BotResponse);
            AppendMessage("Bot", "Before we begin — what's your name?", MessageType.BotResponse);
            _activityLogger("Waiting for user name...");
        }

        // ── Message handling ──────────────────────────────────────────────────

        private void SendButton_Click(object? sender, EventArgs e) => ProcessInput();

        private void InputBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessInput();
            }
        }

        private async void ProcessInput()
        {
            string input = _inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            _inputBox.Clear();
            _inputBox.Enabled  = false;
            _sendButton.Enabled = false;

            // Show user message
            AppendMessage(_userName, input, MessageType.UserMessage);

            // Collect name first. If the user accidentally types a cybersecurity topic
            // instead of their name, do not store that topic as their name.
            if (!_nameCollected)
            {
                if (LooksLikeCybersecurityTopicOrCommand(input))
                {
                    _responseHandler("Bot",
                        "That looks like a cybersecurity topic rather than a name. 😊 Please enter your name first, for example: Jake. After that I’ll answer your question.",
                        MessageType.BotError);
                    _inputBox.Enabled   = true;
                    _sendButton.Enabled = true;
                    _inputBox.Focus();
                    return;
                }

                var nameValidation = InputValidator.ValidateName(input);
                if (!nameValidation.IsValid)
                {
                    _responseHandler("Bot", nameValidation.ErrorMessage, MessageType.BotError);
                    _responseHandler("Bot", "Please enter your name to continue.", MessageType.BotResponse);
                }
                else
                {
                    _userName = input.Length <= 4 && input == input.ToUpper()
                        ? input.ToUpper()
                        : char.ToUpper(input[0]) + input.Substring(1).ToLower();

                    _memory.UserName = _userName;
                    _nameCollected = true;

                    await Task.Delay(400);
                    _responseHandler("Bot",
                        $"Great to meet you, {_userName}! 😊 I'm your Cybersecurity Awareness Assistant. " +
                        "I'll help you understand online threats and stay protected.",
                        MessageType.BotResponse);

                    await Task.Delay(300);
                    _responseHandler("Bot",
                        "You can ask me about phishing, passwords, the CIA triad, ransomware, VPNs, POPIA, and much more. " +
                        "Type 'what can I ask you about' for a full list.",
                        MessageType.BotTip);

                    _activityLogger($"Session started for {_userName}");
                }

                _inputBox.Enabled   = true;
                _sendButton.Enabled = true;
                _inputBox.Focus();
                return;
            }

            // End-session commands should be handled gracefully instead of falling through to the default error.
            if (IsExitCommand(input))
            {
                string goodbye = _responseEngine.GetResponse(input) ??
                    $"Thanks for chatting, {_userName}. Stay cyber-safe! 👋";

                _responseHandler("Bot", goodbye, MessageType.BotResponse);
                _activityLogger("Chat session ended by user");
                _inputBox.Enabled = false;
                _sendButton.Enabled = false;
                return;
            }

            // Validate input
            var validation = InputValidator.Validate(input);
            if (!validation.IsValid)
            {
                _responseHandler("Bot", validation.ErrorMessage, MessageType.BotError);
                _inputBox.Enabled   = true;
                _sendButton.Enabled = true;
                _inputBox.Focus();
                return;
            }

            _memory.MessageCount++;
            _activityLogger($"Processing message #{_memory.MessageCount}...");

            // Simulate thinking delay
            await Task.Delay(500);

            // 1. Check if user is declaring a favourite topic (memory)
            string? memoryResponse = _memory.TryStoreFavouriteTopic(input);
            if (memoryResponse != null)
            {
                _responseHandler("Bot", memoryResponse, MessageType.BotMemory);
                _inputBox.Enabled   = true;
                _sendButton.Enabled = true;
                _inputBox.Focus();
                _activityLogger($"Stored favourite topic: {_memory.FavouriteTopic}");
                return;
            }

            // 2. Handle follow-up commands BEFORE sentiment detection.
            // This prevents "tell me more" from being treated as general curiosity
            // and keeps the bot on the current topic, as required by Part 2 conversation flow.
            string lowerInput = input.ToLower();
            bool isFollowUpOnly =
                lowerInput.Contains("tell me more") ||
                lowerInput.Contains("explain more") ||
                lowerInput.Contains("more details") ||
                lowerInput.Contains("give me examples") ||
                lowerInput.Contains("examples") ||
                lowerInput.Contains("another tip") ||
                lowerInput.Contains("give me another") ||
                lowerInput.Contains("elaborate") ||
                lowerInput.Contains("expand on") ||
                lowerInput.Contains("continue");

            if (isFollowUpOnly && _memory.LastTopic != null)
            {
                string followUpResponse = _messageProcessor(input, _memory);
                _responseHandler("Bot", followUpResponse, MessageType.BotResponse);
                _activityLogger($"Follow-up answered  •  Topic: {_memory.LastTopic}");
                _inputBox.Enabled   = true;
                _sendButton.Enabled = true;
                _inputBox.Focus();
                return;
            }

            // 3. Check for sentiment
            var sentiment = _sentimentDetector.Detect(input);
            if (sentiment.SentimentDetected)
            {
                _responseHandler("Bot", sentiment.EmpathyMessage, MessageType.BotSentiment);
                await Task.Delay(300);
                _responseHandler("Bot", sentiment.FollowUpTip, MessageType.BotTip);

                // Also try to answer the underlying cybersecurity topic in the same turn.
                _memory.UpdateTopic(input);
                string? topicResponse = _responseEngine.GetResponse(input);
                if (topicResponse != null)
                {
                    await Task.Delay(300);
                    _responseHandler("Bot", topicResponse, MessageType.BotResponse);
                }

                _activityLogger("Sentiment detected and response personalised");
                _inputBox.Enabled   = true;
                _sendButton.Enabled = true;
                _inputBox.Focus();
                return;
            }

            // 4. Run through delegate-based message processor
            string botResponse = _messageProcessor(input, _memory);
            _responseHandler("Bot", botResponse, MessageType.BotResponse);

            // 4. Append recall message if relevant
            if (_memory.HasFavouriteTopic && _memory.MessageCount % 4 == 0)
            {
                string? recall = _memory.GetRecallMessage();
                if (recall != null)
                {
                    await Task.Delay(200);
                    _responseHandler("Bot", recall, MessageType.BotMemory);
                }
            }

            _activityLogger($"Response sent  •  Topic: {_memory.LastTopic ?? "general"}");
            _inputBox.Enabled   = true;
            _sendButton.Enabled = true;
            _inputBox.Focus();
        }


        private bool LooksLikeCybersecurityTopicOrCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            string lower = input.ToLower();

            // Topic detection from the response engine catches inputs such as
            // "SQL injection", "phishing", "password safety", etc.
            if (_responseEngine.DetectTopic(lower) != null) return true;

            // Common chatbot commands should also not become the user's name.
            return lower.Contains("what can i ask") ||
                   lower.Contains("tell me more") ||
                   lower.Contains("give me examples") ||
                   lower.Contains("another tip") ||
                   lower.Contains("explain") ||
                   lower.Contains("topic") ||
                   lower.Contains("cyber");
        }

        private static bool IsExitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            string lower = input.Trim().ToLower();
            return lower == "exit" || lower == "quit" || lower == "bye" || lower == "goodbye" || lower == "end" || lower == "close";
        }

        // ── Chat display rendering ────────────────────────────────────────────

        /// <summary>
        /// Appends a coloured, formatted message to the chat display.
        /// Uses the BotResponseHandler delegate for all output routing.
        /// </summary>
        private void AppendMessage(string sender, string message, MessageType type)
        {
            if (_chatDisplay.InvokeRequired)
            {
                _chatDisplay.Invoke(new Action(() => AppendMessage(sender, message, type)));
                return;
            }

            _chatDisplay.SuspendLayout();

            // Determine colours based on message type
            Color senderColor = type switch
            {
                MessageType.UserMessage  => AccentYellow,
                MessageType.BotResponse  => AccentGreen,
                MessageType.BotTip       => AccentBlue,
                MessageType.BotError     => AccentRed,
                MessageType.BotSentiment => AccentPurple,
                MessageType.BotMemory    => Color.FromArgb(255, 160, 80),
                MessageType.System       => TextMuted,
                _                        => TextPrimary,
            };

            string icon = type switch
            {
                MessageType.UserMessage  => "👤",
                MessageType.BotResponse  => "🤖",
                MessageType.BotTip       => "💡",
                MessageType.BotError     => "⚠️",
                MessageType.BotSentiment => "💬",
                MessageType.BotMemory    => "🧠",
                MessageType.System       => "─",
                _                        => "•",
            };

            if (type == MessageType.System)
            {
                AppendColoured($"\n  {message}\n", TextMuted);
            }
            else
            {
                // Sender line
                AppendColoured($"\n  {icon} ", senderColor);
                AppendColoured($"{sender,-8}", senderColor, bold: true);
                AppendColoured(" │  ", BorderColor);

                // Message — handle multi-line
                string[] lines = message.Split('\n');
                AppendColoured(lines[0] + "\n", TextPrimary);
                for (int i = 1; i < lines.Length; i++)
                    AppendColoured($"           │  {lines[i]}\n", TextPrimary);
            }

            _chatDisplay.ResumeLayout();
            _chatDisplay.ScrollToCaret();
            _chatDisplay.SelectionStart = _chatDisplay.Text.Length;
            _chatDisplay.ScrollToCaret();
        }

        private void AppendColoured(string text, Color color, bool bold = false)
        {
            int start = _chatDisplay.TextLength;
            _chatDisplay.AppendText(text);
            _chatDisplay.Select(start, text.Length);
            _chatDisplay.SelectionColor = color;
            _chatDisplay.SelectionFont  = bold
                ? new Font(_chatDisplay.Font, FontStyle.Bold)
                : _chatDisplay.Font;
            _chatDisplay.SelectionLength = 0;
        }

        // ── ASCII art ─────────────────────────────────────────────────────────
        private static string GetAsciiArt()
        {
            return
                "  ██████╗██╗   ██╗██████╗ ███████╗██████╗      █████╗ ██╗    ██╗ █████╗ ██████╗ ███████╗\n" +
                " ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗    ██╔══██╗██║    ██║██╔══██╗██╔══██╗██╔════╝\n" +
                " ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝    ███████║██║ █╗ ██║███████║██████╔╝█████╗  \n" +
                " ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗    ██╔══██║██║███╗██║██╔══██║██╔══██╗██╔══╝  \n" +
                " ╚██████╗   ██║   ██████╔╝███████╗██║  ██║    ██║  ██║╚███╔███╔╝██║  ██║██║  ██║███████╗\n" +
                "  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝    ╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝";
        }
    }
}
