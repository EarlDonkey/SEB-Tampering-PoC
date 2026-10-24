# Educational SEB Tampering Proof of Concept (PoC)

**Author:** Earl Donkey
**Context:** Boredom, and this unfathomable hunger for class points
**Disclaimer:** This tool was created strictly for educational purposes to understand .NET assembly structures, anti-tamper mechanisms, and payload delivery. It is intended to be used only in isolated, offline virtual machines. I do not condone or support using this tool to violate academic integrity policies or bypass exam security in live environments.

## Overview
Safe Exam Browser (SEB) is a kiosk application designed to lock down a Windows environment during exams. Because SEB is an open-source .NET Framework application, it is vulnerable to static source-code tampering. 

This project demonstrates a Proof of Concept (PoC) Patcher that successfully bypasses SEB's monitoring and lockdown features.

## How the Bypass Works
1. **Source Code Modification:** The open-source SEB codebase was downloaded. Logic enforcing process termination and window restrictions (specifically within `ApplicationMonitor.cs`) was commented out. Additionally, the `Integrity` module was intentionally excluded from the build.
2. **Assembly Mismatch:** Recompiling the modified code changes the `PublicKeyToken` of the resulting `.dll` and `.exe` files. If only a few files are replaced, SEB will crash with a `FileLoadException`. 
3. **Payload Packaging:** To solve the assembly mismatch, the entire recompiled `Release` folder was compressed into a `.zip` payload. 
4. **Custom Patcher Delivery:** A custom C# Windows Forms application was built. It embeds the `.zip` payload as a resource, requests Administrator privileges via the `app.manifest`, and extracts the modified binaries directly into `C:\Program Files\SafeExamBrowser\Application\`.

## How to Compile (For Researchers)
**Prerequisites:** Visual Studio with .NET Desktop Development workload.
1. Clone this repository.
2. Compile the modified SEB source code into a ZIP file.
3. Open this Patcher project in Visual Studio.
4. Add your compiled SEB ZIP file to `Resources.resx` and name it `SEB_x64_Zip`.
5. Build the project in `Release` mode.

## Key Findings for AP Cybersecurity
* **Kiosk software must be verified at the kernel level.** Purely user-space .NET applications can be easily terminated or modified if the user has local admin rights.
* **Open-source software requires trust.** If a student recompiles the open-source code of their exam browser, the exam provider has no way to verify the binary is unmodified unless it is running in a secure, hardware-backed enclave (like TPM or Windows Hello).
