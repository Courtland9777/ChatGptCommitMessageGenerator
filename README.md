# ChatGptCommitMessageGenerator VSIX Project

## Introduction
ChatGptCommitMessageGenerator is a Visual Studio 2022 extension that generates commit messages for your code using OpenAI's GPT-3.5-Turbo model. Improve your commit messages and save time by leveraging the power of AI.

## Prerequisites
- Visual Studio 2022
- An OpenAI_APIKey
- Git installed on your system

## Installation
1. Open Visual Studio 2022.
2. Navigate to **Extensions > Manage Extensions**.
3. Search for `ChatGptCommitMessageGenerator` and click on it.
4. Click on **Download** and follow the installation prompts.

## Configuration
1. Set up the OpenAI environment variable:
   - Create a new environment variable named `OPENAI_API_KEY` and set its value to your OpenAI_APIKey.
2. Ensure that Git is installed and properly configured on your system.

## Usage
1. Select the Extensions tab and navigate to Commit Message Generator and select the only option.
2. If there are changes present between your last commit and the working directory a brief commit message will be generated.
3. The message will appear in two places. A message box will pop up so the message can be read and easily copied. Also if you look for the Commit Message Generator tab in the output window you will also see the generated message. Here is where you will find any other outputs as well.
4. I have intentionally avoided auto-commiting the current changes with a generated git message. The ability to manually edit the message before commiting is intended. 

![Usage example](./usage-example.gif)

## Contributing
We welcome contributions from the community! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch with a descriptive name.
3. Make your changes and commit them with a clear and concise commit message.
4. Open a pull request against the `main` branch of the original repository.

For reporting issues, please use the GitHub Issues tracker.

## Contact Information
For any questions or concerns, please contact the project maintainer:

- GitHub: [courtland9777](https://github.com/courtland9777)

## Recommended Shields
![Visual Studio Marketplace Version](https://img.shields.io/visual-studio-marketplace/v/YourAccountName.ChatGptCommitMessageGenerator)
![Visual Studio Marketplace Downloads](https://img.shields.io/visual-studio-marketplace/d/YourAccountName.ChatGptCommitMessageGenerator)
![License](https://img.shields.io/github/license/YourAccountName/ChatGptCommitMessageGenerator)