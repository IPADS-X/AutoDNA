## PLC Software Usage Guide (IDE: inoshop)

This tutorial outlines the steps for setting up the inoshop IDE, preparing the project, and compiling the machine code.

### Prerequisites

* A Windows PC or other compatible operating system.
* The installation files for **inoshop v1.7.3** and the **SP3 patch package** (**InoProShop(V1.7.3).exe** and **InoProShop(V1.7.3)SP3.inopkg**).

### Step 1: Install inoshop v1.7.3

1.  **Locate the Installer:** Find the installation file for inoshop v1.7.3 (**InoProShop(V1.7.3).exe**).
2.  **Run the Installer:** Double-click the installer file to begin the setup process.
3.  **Follow the Prompts:** Follow the on-screen instructions.
4.  **Complete Installation:** Click **Install** or **Next** until the installation is complete. You may need to restart your computer.

### Step 2: Install the SP3 Patch Package

1.  **Locate the Patch:** Find the SP3 patch file (**InoProShop(V1.7.3)SP3.inopkg**).
2.  **Apply the Patch:** Choose **Version Install** option to install.

### Step 3: Open Archive File, Compile, and Auto-Install Files

This step involves restoring the machine's project from an archive and ensuring all necessary system and project libraries are correctly integrated.

1.  **Open archive file:** Open archive file (**archivefile.projectarchive**).
2.  **Select Options and Extract**
    * A dialog box will appear.
    * **Select all options/checkboxes** in the dialog box.
    * Click the **Extract** button to start the restoration process.
    * **Confirm subsequent dialog boxes:** If any subsequent dialog boxes appear (e.g., asking to overwrite existing files, confirm library paths, or confirm project name), click **OK** or **Yes to All** to proceed with the extraction.

### Step 4: Compile Individual Machine Codes

After the necessary libraries are in place, you can proceed to compile the code for each specific machine or PLC unit.

1.  **Select the Target:** Open the PLC file (eg. Amplification.project).
2.  **Compile the Code:**
    * Select **Compile** or **Build** from the context menu.
    * Select **Log in** to download code to hardware.
3.  **Repeat for All Machines:** Repeat the compilation process for all the individual machine codes (targets) within the project until every code block has compiled successfully and is ready for download to the respective PLC hardware.