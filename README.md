# 🛡️ Cybersecurity Awareness Chatbot — Part 2

## Part 1 Audio Integration

This Part 2 version uses the original `Assets/greeting.wav` file from the submitted Part 1 project, so the GUI plays the student's recorded voice greeting on startup instead of a generated or placeholder audio file.


**Module:** Programming 2A (PROG6221/w)
**Assessment:** Portfolio of Evidence — Part 2
**Institution:** The Independent Institute of Education (IIE)

---

## 📋 Overview

Part 2 extends the console chatbot from Part 1 into a full **WinForms GUI application**. All Part 1 functionality is preserved and expanded with:

- 🖥️ Windows Forms graphical interface with dark theme
- 🎯 Keyword recognition (30+ cybersecurity topics)
- 🎲 Random responses for phishing, password, scam, and browsing tips
- 💬 Conversation flow with follow-up question handling
- 🧠 Memory and recall — remembers user's name and favourite topic
- ❤️ Sentiment detection — responds to worried, frustrated, curious, happy
- 🔔 Delegates — `MessageProcessor`, `BotResponseHandler`, `ActivityLogger`
- ✅ Input validation and graceful error handling

---

## 🗂️ Project Structure

```
CybersecurityChatbot/
├── Program.cs                          # Entry point — launches WinForms
├── CybersecurityChatbot.csproj         # Project file (net9.0-windows, WinForms)
├── README.md
├── Assets/
│   └── greeting.wav                    # Voice greeting WAV file
├── ChatbotCore/
│   ├── ChatbotEngine.cs                # Part 1 console engine (preserved)
│   ├── ConsoleUI.cs                    # Part 1 console UI (preserved)
│   ├── ResponseEngine.cs               # 30+ keyword responses + random tips
│   ├── UserSession.cs                  # Auto-properties for session state
│   ├── VoiceGreeting.cs                # WAV playback via System.Media
│   ├── InputValidator.cs               # Input validation
│   ├── SentimentDetector.cs            # NEW — detects worried/frustrated/curious/happy
│   ├── ConversationMemory.cs           # NEW — stores name, topic, favourite interest
│   └── ChatDelegates.cs                # NEW — MessageProcessor, BotResponseHandler delegates
├── GUI/
│   └── MainForm.cs                     # NEW — WinForms main window
└── .github/
    └── workflows/
        └── dotnet-ci.yml               # GitHub Actions CI
```

---

## 🚀 Getting Started

### Prerequisites
- Windows 10 or later
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9)

### Run

```bash
cd Part2
dotnet restore
dotnet run
```

---

## ✨ Features Demonstrated

### Keyword Recognition
Type any topic — `phishing`, `CIA triad`, `ransomware`, `POPIA`, `SIM swap`, etc.

### Random Responses
Type `phishing tip`, `password tip`, `scam tip`, or `safe browsing tip` — a different tip each time.

### Conversation Flow
After any topic, type `tell me more`, `explain more`, `give me examples`, or `another tip` — the bot continues on the same subject instead of restarting.

### Memory and Recall
Say `I'm interested in privacy`, `I like passwords`, or `focus on phishing` — the bot stores the favourite topic and references it later.

### Sentiment Detection
Express how you feel:
- *"I'm worried about online scams"* → empathetic response + immediate tip
- *"I'm frustrated and confused"* → supportive response + simplified guidance
- *"I'm curious about encryption"* → enthusiastic response + detailed info

---

## ✅ CI/CD — GitHub Actions

This project includes `.github/workflows/dotnet-ci.yml`, which restores and builds the project on GitHub. After pushing the project, add a screenshot of the green CI run here.

> 📸 *Add your green CI screenshot here*

![CI Workflow](docs/ci-screenshot.png)

## 🏷️ Releases and Tags

Part 2 requires meaningful commit history and releases/tags. Suggested tags:

- `v1.0-part1-console` — preserved Part 1 foundation
- `v2.0-part2-gui` — GUI conversion completed
- `v2.1-part2-polish` — memory, sentiment, follow-ups and final fixes

---

## 🎥 Video Presentation

> 🎬 [YouTube Unlisted Link — insert here]

---

## 📚 References

Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review. *The African Journal of Information and Communication*, 28(28). doi: https://doi.org/10.23962/10539/32213.
