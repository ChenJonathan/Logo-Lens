# LogoLens

A Microsoft Hololens augmented reality application that displays stock information about companies in the surrounding environment.

This application was originally developed at HackMIT in September 2016, winning the "Best use of NASDAQ's Data On Demand API".

We continued working on it until we demoed the application to NASDAQ's executives in January 2017.

## Build & Installation

1. Clone this repo into a empty folder
2. Create a folder name "Build"
2. Open up the project in Unity 5.50f3
3. Click on File -> Build & Run
4. Build for the Universal Windows Platform, selecting the "Build" folder
5. Open up the solution file (found in the "Build" folder) in Microsoft Visual Studio
6. Select "Release, x86, Device" to deploy to the Microsoft Hololens

## Usage

1) Wearing the Microsoft Hololens, stare directly at a well-known logo and perform the [Air Tap Gesture](https://developer.microsoft.com/en-us/windows/holographic/gestures)

2) A **card** should open up. You should see a **Graph** of stock prices and other information relevant to the logo.

	* If logo detection fails, the card should contain a message saying so. Tap on the middle of the card to close it, then try again.
	* Note: If the stock did not trade on that day, you will not see a graph. That is ok, just continue.

3) You can perform Air Tap Gestures on the **Left** and **Right Buttons** in order to change the date range for the displayed stock informartion.

4) If the **Loading Animation** appears, wait for stock prices to be retrieved from NASDAQ's API in the background. The card will change as soon as it's done.

5) You can **Hover** over certain points on the graph to view more detailed information about that **Point**.

6) Tap on the **Middle** of the card to close it.

## Technologies Used

* **Microsoft Hololens**: Platform for the application to run on
* **Google Cloud Vision API**: Company logo detection
* **NASDAQ Data On Demand API**: Stock information retrieval
* **Unity Game Engine**: Where all the augmented reality graphics were developed
* **Microsoft Visual Studio**: C# IDE for Unity

Note that Unity 5.50f3 is the last version of Unity that this project was built and tested on. Later versions are not supported.

## Contact

Contact any of the contributers of this project for any additional information!

