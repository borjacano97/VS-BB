# VS-BB
Bi-directional integartion between Unity's Visual Scripting and Pada1's BehaviourTree

# Introduction
This project is a bi-directional integration between Unity's Visual Scripting and Pada1's BehaviourTree. The goal is to allow the user to create a behaviour tree in Pada1's BehaviourTree and then run it from within a Visual Scripting graph in Unity. An to run Visual Scripts "coded" in VS as a Behaviour Tree's leaf node.

# Installation

1. Get Behavior Bricks from [Unity's Assets Store](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-bricks-74816) and add it to your account.
Then import de package into your project ![BB import](doc/img/package_manager.png) and import it into your project. ![PacketManager_Import_BB](doc/img/import_bb.png)
2. Get Visual Scripting from [Unity's Package Manager]() and import it into your project.
![PacketManager_Import_VS](image-1.png)
1. Get the latest release of this project from [releases](https://github.com/borjacano97/VS-BB/releases) and import it into your project. ![Import_VS_BB](doc/img/import_bb_vs.png)
2. You are ready to go!


# Hello World
You can find a simple Hello World example in [this repository](https://github.com/borjacano97/HelloWorld-VS_BB) .

# Usage

## Run a Behaviour Tree from a Visual Scripting Graph

1. Open the Visual Scripting Graph Editor.
2. Add the node `BehaviourTree Executer` to the graph. ![Add_BB_executer_VS](doc/img/BB_executor-VS.PNG)
3. Drag and drop the Behaviour Tree asset you want to run into the `Behaviour` field. ![Drop_Behaviour](doc/img/BB-VS_select_tree.PNG)
4. You **must** connect to flow input of the `BehaviourTree Executer` node to a `Update` flow node. It can have as many nodes as you want in between, but the flow must reach the `BehaviourTree Executer` node each frame.
5. The `BehaviourTree Executer` node has two outputs by default:
	- `Task RUNNING`: This output will be triggered while the Behaviour Tree has not finished executing.
	- `Task COMPLETED`: This output will be triggered when the Behaviour Tree has finished executing.
  ![Drop_Behaviour](doc/img/BB-VS_excutor_parameters.PNG)
6. If the Behaviour attached had a blackboard with parameters as In/Out the node will auto detect those, and will create the corresponding entry and exit points for them. (example count) 

## Run a Visual Scripting Graph from a Behaviour Tree

1. Open the Visual Scripting Executor generator on `Window > Behaviour Bricks > Create New Visual Scripting Executor`.
2. Fill the fields with the desired information.
   - Script Machine or State Machine
   - Name of the Visual Scripting Graph
   - Action path: The path of the executor in the Behaviour Brick's editor.
   - Help: A brief description of the executor.
   - Input parameters: Number of input parameters the executor will have.
     - Type: The type of the input parameter.
     - Name: The name of the input parameter.
   - Output parameters: Number of output parameters the executor will have.
	 - Type: The type of the output parameter.
	 - Name: The name of the output parameter.
3. Click on `Generate`.
4. Now you've created a custom node for your Behavior Tree to execute your script/State Machine under the name you have provided.
5. With your selected node in the Behavior Tree, set at least the parameter Machine as an In aprameter for the blackboard, put a recognizeable name for it like "node_name_machine"
6. From the Unity inspector, provide to the BehaviourTree BlackBoard the Machine with your script/StateMachine you wanna run.
