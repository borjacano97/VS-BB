# VS-BB
Bi-directional integartion between Unity's Visual Scripting and Pada1's BehaviourTree

# Introduction
This project is a bi-directional integration between Unity's Visual Scripting and Pada1's BehaviourTree. The goal is to allow the user to create a behaviour tree in Pada1's BehaviourTree and then run it from within a Visual Scripting graph in Unity. An to run Visual Scripts "coded" in VS as a Behaviour Tree's leaf node.

# Installation

1. Get Behavior Bricks from [Unity's Assets Store](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-bricks-74816) and add it to your account.
Then import de package into your project ![BB import](doc/img/package_manager.png) and import it into your project. ![PacketManager_Import_BB](doc/img/import_bb.png)
2. Get Visual Scripting from [Unity's Package Manager]() and import it into your project.
![PacketManager_Import_VS](doc/img/package_manager_visual_scripting.png)
3. Get the latest release of this project from [releases](https://github.com/borjacano97/VS-BB/releases) and import it into your project. ![Import_VS_BB](doc/img/import_bb_vs.png)
4. Regeneate the Visual Scripting Nodes. Go to `Project Settings > Visual Scripting > Regenerate Nodes` and click on `Regenerate Nodes`. ![Regenerate_Nodes](doc/img/regenerate_nodes.png)
5. You are ready to go!


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
 ![Executor Generator](doc/img/Create_BB_node_using_VS_1.PNG)

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
    - Output Directory (Optional): The directory where the executor will be saved.

 ![Executor Generator](doc/img/Create_BB_node_using_VS_2.PNG)

3. Click on `Generate`.
4. Now you've created a custom node for your Behavior Tree to execute your Script/State Machine under the name you have provided. You now can find it in the Behaviour Brick editor at the `ActionPath` you have provided.

 ![Executor Generator](doc/img/Create_BB_node_using_VS_3.PNG)

5. Select the new node on the BB editor, go to Parameters and click on `Machine` input parameter, select `BlackBoard`, give it a name and click on `Create`.

 ![Executor Generator](doc/img/Create_BB_node_using_VS_4.PNG)

6. Add a new Script/State Machine component to the GameObject that has the Behaviour Tree, and add the Visual Scripting Graph you want to run to it.
7. Drag and drop the Component to the `Machine` input parameter in the `Behaviour Executor Component`'s `BlackBoard`.

 ![Executor Generator](doc/img/Create_BB_node_using_VS_6.PNG)
 ![Executor Generator](doc/img/Create_BB_node_using_VS_5.PNG) 

## Script Machines to be used as Behaviour Tree's leaf nodes
The script machines that are going to be used as leaf nodes in the Behaviour Tree must have special requirements:
- The Script Machine (or Start Script Machine in the case of State Machines) must have a `On BB Update` node.
- Then the Task ends, the flow must end on one of the following nodes:
  - `Complete BB Task`: This node will end the task as completed.
  - `Abort BB Task`: This node will end the task as failed.
  - `End Failed Task`: This node will end the task as failed.
  - `Suspend BB Task`: This node will suspend the task.
  - `Resume BB Task`: This node will resume the task.
  - `Ending the flow with nothing`: the Executor will asume the task status is `Running`
- Before ending the task, the Script Machine must set the output parameters of the task (if any). To do so, you must use the `Set Task Output Argumetns` node.


 ![Executor Generator](doc/img/Create_BB_node_using_VS_8.PNG)

- If the Script Machine has input parameters, the input parameters count must be set on the `On BB Update` node. Once the number of input parameters is set, the node will display a series of Arguments to use as parameter inputs. The order of the arguments is the same as defined on the Generator.

 ![Executor Generator](doc/img/Create_BB_node_using_VS_7.PNG)

> ⚠ **WARNING** ⚠: The Script Machine must have the **same number** of input parameters as defined on the Generator. <u>No checks are done to ensure this</u>, so make sure the number of input parameters is correct.
> ⚠ **WARNING** ⚠: No type checking is done on the input parameters. Make sure the input parameters are of the correct type.
