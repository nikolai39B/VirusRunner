﻿using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class GameGenerator : MonoBehaviour
{
	void Start()
    {
        /*int rows = 5;
        int cols = 5;
        int maxNoOfPaths = 3;
        int startRow = 4;
        int startCol = 2;
        Direction endingSide = Direction.UP;
        int numberOfPathsGenerated;

        PathNode[,] world = GeneratePaths(rows, cols, maxNoOfPaths, startRow, startCol, endingSide, out numberOfPathsGenerated);

        for (int row = 0; row < world.GetLength(0); row++)
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int col = 0; col < world.GetLength(1); col++)
            {
                PathNode currentNode = world[row, col];
                if (currentNode == null)
                {
                    strBuilder.Append(" -");
                }
                else if (currentNode.NodeRole == PathNode.Role.START)
                {
                    strBuilder.Append(" S");
                }
                else
                {
                    strBuilder.Append(string.Format(" {0}", currentNode.PathId));
                }
            }
            Debug.Log(strBuilder.ToString());
        }*/
    }

    /// <summary>
    /// Builds paths through an empty world, and returns the world.
    /// </summary>
    /// <param name="options">The options for generating the paths.</param>
    /// <returns>The world with the newly generated paths.</returns>
    public World GeneratePaths(PathFindingOptions options)
    {
        // Create the new world
        World world = new World(new List<Path>());

        // Start generating paths
        bool pathGenerationSucceeded = true;

        // Generate paths until we hit the maximum or path generation fails
        while (world.ChildPaths.Count < options.MaxNumberOfPaths && pathGenerationSucceeded)
        {
            // Attempt to generate the next path
            Path generatedPath;
            pathGenerationSucceeded = GeneratePath(out generatedPath, world, options);
        }

        return world;
    }

    public bool GeneratePath(out Path generatedPath, World world, PathFindingOptions options)
    {
        /* Diagram of World Coordinates and Directions
         *
         * array[5,4]
         *
         *         UP       
         * L [ 00 01 02 03 ] R
         * E [ 10 11 12 13 ] I
         * F [ 20 21 22 23 ] G
         * T [ 30 31 32 33 ] H
         *   [ 40 41 42 43 ] T
         *        DOWN
         *
         * World Dimension 0 Length (# of Rows): 5
         * World Dimension 1 Length (# of Cols): 4
        */


        return true;
    }

    /// <summary>
    /// Builds a given number of paths through a new world with the specified dimensions.
    /// </summary>
    /// <param name="rows">The number of rows in the world.</param>
    /// <param name="columns">The number of columns in the world.</param>
    /// <param name="maxNumberOfPaths">The maximum number of paths to place in the world. If the no more paths can be generated, the method will terminate early.</param>
    /// <param name="startRow">The starting row for the paths.</param>
    /// <param name="startColumn">The starting column for the paths.</param>
    /// <param name="endingSide">The ending side for the paths.</param>
    /// <param name="numberOfPathsGenerated">The number of paths actually generated by the method.</param>
    /// <exception cref="ArgumentException">Thrown when one or more given arguments is invalid.</exception>
    /// <returns>The new world, with the specified number of paths running through it. The start location is marked as a PathNode with directions 'NONE' and id '-1'.</returns>
    public PathNode[,] GeneratePaths(int rows, int columns, int maxNumberOfPaths, int startRow, int startColumn, Direction endingSide, out int numberOfPathsGenerated)
    {
        // Verify argument validity
        if (rows <= 0)
        {
            throw new ArgumentException(string.Format("Invalid number of rows '{0}'. Number of rows must be a positive integer.", rows));
        }
        if (columns <= 0)
        {
            throw new ArgumentException(string.Format("Invalid number of columns '{0}'. Number of columns must be a positive integer.", columns));
        }
        if (maxNumberOfPaths <= 0)
        {
            throw new ArgumentException(string.Format("Invalid maximum number of paths '{0}'. Maximum number of paths must be a positive integer.", maxNumberOfPaths));
        }
        if (startRow < 0 || startRow >= rows)
        {
            throw new ArgumentException(string.Format("Invalid start row '{0}'. Start row must be between zero (inclusive) and the number of rows '{1}' (exclusive).", startRow, rows));
        }
        if (startColumn < 0 || startColumn >= columns)
        {
            throw new ArgumentException(string.Format("Invalid start column '{0}'. Start column must be between zero (inclusive) and the number of columns '{1}' (exclusive).", startColumn, columns));
        }
        if (endingSide == Direction.NONE)
        {
            throw new ArgumentException("Ending side cannot be 'Direction.NONE'.");
        }

        // Instantiate the world
        PathNode[,] world = new PathNode[rows, columns];

        // Place the start location
        world[startRow, startColumn] = new PathNode(Direction.NONE, Direction.NONE, -1, PathNode.Role.START);

        // Add the paths
        numberOfPathsGenerated = 0;
        while (numberOfPathsGenerated < maxNumberOfPaths)
        {
            // Add this iteration's path
            bool pathGenerated = AddPathToWorld(ref world, startRow, startColumn, endingSide, numberOfPathsGenerated);

            if (pathGenerated)
            {
                // If we manage to add a path to the world, increment the numberOfPathsGenerated variable
                numberOfPathsGenerated++;
            }
            else
            {
                // Otherwise, break out early
                break;
            }
        }

        // Return our new world with the generated paths
        return world;
    }

    /// <summary>
    /// Generate and add a new path the the given world.
    /// </summary>
    /// <param name="world">The world to add a path to.</param>
    /// <param name="startRow">The path start row.</param>
    /// <param name="startColumn">The path start column.</param>
    /// <param name="endingSide">The side that the path ends on.</param>
    /// <param name="pathId">The id for the path to generate.</param>
    /// <returns>True if the path was generated and added successfully; false otherwise.</returns>
    private bool AddPathToWorld(ref PathNode[,] world, int startRow, int startColumn, Direction endingSide, int pathId)
    {
        /* Diagram of World Coordinates and Directions
         *
         * array[5,4]
         *
         *         UP       
         * L [ 00 01 02 03 ] R
         * E [ 10 11 12 13 ] I
         * F [ 20 21 22 23 ] G
         * T [ 30 31 32 33 ] H
         *   [ 40 41 42 43 ] T
         *        DOWN
         *
         * World Dimension 0 Length (# of Rows): 5
         * World Dimension 1 Length (# of Cols): 4
        */

        // Pull relevant info out of the world
        int rowsInWorld = world.GetLength(0);
        int colsInWorld = world.GetLength(1);

        // Define values for iterations
        bool addNewNode = true;
        bool pathCouldNotBeGenerated = false;
        int currentRow = startRow;
        int currentColumn = startColumn;
        Direction directionToLastNode = Direction.NONE;

        while (addNewNode)
        {
            // Determine which directions are valid
            List<Direction> validDirections = new List<Direction>();
            foreach (var dirToCheck in new Direction[] { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT })
            {
                if (CanMoveInDirection(world, currentRow, currentColumn, dirToCheck, endingSide))
                {
                    validDirections.Add(dirToCheck);
                }
            }

            if (validDirections.Count == 0)
            {
                // If we're at the start, then we can't add a new path
                if (currentRow == startRow && currentColumn == startColumn)
                {
                    addNewNode = false;
                    pathCouldNotBeGenerated = true;
                    break;
                }

                // Otherwise, step back
                else
                {
                    // Get the row and column for the previous node
                    int previousRow = currentRow;
                    int previousColumn = currentColumn;
                    GetNewCoordinatesFromDirection(ref previousRow, ref previousColumn, directionToLastNode);

                    // Set a temporary node at our current location so we don't go back here
                    PathNode temporaryNodeToAdd = new PathNode(Direction.NONE, Direction.NONE, pathId, PathNode.Role.TEMPORARY);
                    world[currentRow, currentColumn] = temporaryNodeToAdd;

                    // Set the current row and column for the next iteration
                    currentRow = previousRow;
                    currentColumn = previousColumn;
                    continue;
                }
            }

            // Get the direction to move
            int directionIndex = UnityEngine.Random.Range(0, validDirections.Count);
            Direction directionToMove = validDirections[directionIndex];

            // Add a new node at our current location (if we're not at the start)
            if (currentRow != startRow || currentColumn != startColumn)
            {
                PathNode nodeToAdd = new PathNode(directionToMove, directionToLastNode, pathId);
                world[currentRow, currentColumn] = nodeToAdd;
            }

            // Update direction to last node for next iteration (so the next node can point at us)
            directionToLastNode = ReverseDirection(directionToMove);

            // Get the new coordinates and create a node
            int targetRow = currentRow;
            int targetColumn = currentColumn;
            GetNewCoordinatesFromDirection(ref targetRow, ref targetColumn, directionToMove);

            // If we're off the world, then we must be done with this path
            if (targetRow < 0 ||
                targetRow >= rowsInWorld ||
                targetColumn < 0 ||
                targetColumn >= colsInWorld)
            {
                addNewNode = false;
                break;
            }

            // Set the current row and column for the next iteration
            currentRow = targetRow;
            currentColumn = targetColumn;
        }

        RemoveTemporaryPathNodes(ref world);

        return !pathCouldNotBeGenerated;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="currentRow"></param>
    /// <param name="currentColumn"></param>
    /// <param name="directionToCheck"></param>
    /// <param name="endingSide"></param>
    /// <returns></returns>
    private bool CanMoveInDirection(PathNode[,] world, int currentRow, int currentColumn, Direction directionToCheck, Direction endingSide)
    {
        // Pull relevant info out of the world
        int rowsInWorld = world.GetLength(0);
        int colsInWorld = world.GetLength(1);

        // We can never move in NONE direction
        if (directionToCheck == Direction.NONE)
        {
            return false;
        }

        // Create new row / column values for after movement
        int targetRow = currentRow;
        int targetColumn = currentColumn;
        GetNewCoordinatesFromDirection(ref targetRow, ref targetColumn, directionToCheck);

        // Check for moving off the world
        if (targetRow < 0)
        {
            return endingSide == Direction.UP;
        }
        else if (targetRow >= rowsInWorld)
        {
            return endingSide == Direction.DOWN;
        }
        else if (targetColumn < 0)
        {
            return endingSide == Direction.LEFT;
        }
        else if (targetColumn >= colsInWorld)
        {
            return endingSide == Direction.RIGHT;
        }

        // Finally, check the target location for an existing node
        return world[targetRow, targetColumn] == null;
    }

    /// <summary>
    /// Gets new coordinates after a movement of a single unit in the given direction.
    /// </summary>
    /// <param name="currentRow">The current row coordinate.</param>
    /// <param name="currentColumn">The current column coordinate.</param>
    /// <param name="directionToMove">The direction to move.</param>
    /// <exception cref="ArgumentException">Thrown when the direction to move is not recognized by the function.</exception>
    private void GetNewCoordinatesFromDirection(ref int currentRow, ref int currentColumn, Direction directionToMove)
    {
        switch (directionToMove)
        {
            case Direction.UP:
                currentRow--;
                break;

            case Direction.DOWN:
                currentRow++;
                break;

            case Direction.LEFT:
                currentColumn--;
                break;

            case Direction.RIGHT:
                currentColumn++;
                break;

            case Direction.NONE:
                break;

            default:
                // We should never be here
                throw new ArgumentException(string.Format("Direction to check '{0}' was not recognized.", directionToMove));
        }
    }

    /// <summary>
    /// Gets the reverse of the given direction (UP to DOWN, etc.).
    /// </summary>
    /// <param name="directionToReverse">The direction to reverse.</param>
    /// <returns>The reverse of the given direction.</returns>
    private Direction ReverseDirection(Direction directionToReverse)
    {
        switch (directionToReverse)
        {
            case Direction.UP:
                return Direction.DOWN;

            case Direction.RIGHT:
                return Direction.LEFT;

            case Direction.DOWN:
                return Direction.UP;

            case Direction.LEFT:
                return Direction.RIGHT;

            case Direction.NONE:
                return Direction.NONE;

            default:
                throw new ArgumentException(string.Format("Direction to reverse '{0}' was not recognized.", directionToReverse));
        }
    }

    /// <summary>
    /// Removes all temporary nodes from the given world.
    /// </summary>
    /// <param name="world">The world to remove the path nodes from.</param>
    private void RemoveTemporaryPathNodes(ref PathNode[,] world)
    {
        for (int row = 0; row < world.GetLength(0); row++)
        {
            for (int col = 0; col < world.GetLength(1); col++)
            {
                PathNode currentNode = world[row, col];
                if (currentNode != null && currentNode.NodeRole == PathNode.Role.TEMPORARY)
                {
                    world[row, col] = null;
                }
            }
        }
    }

    // TODO: create path class for better management

    public class PathNode
    {
        /// <summary>
        /// Instantiate a PathNode instance.
        /// </summary>
        /// <param name="directionToNextNode">The direction that path continues from this node.</param>
        /// <param name="directionToPreviousNode">The direction that path takes leading to this node.</param>
        /// <param name="pathId">The id of the path that this node corresponds to.</param>
        /// <param name="nodeRole">The role for this node (NORMAL by default).</param>
        public PathNode(Direction directionToNextNode, Direction directionToPreviousNode, int pathId, Role nodeRole = Role.NORMAL)
        {
            DirectionToNextNode = directionToNextNode;
            DirectionToPreviousNode = directionToPreviousNode;
            PathId = pathId;
            NodeRole = nodeRole;
        }

        public Direction DirectionToNextNode { get; set; }
        public Direction DirectionToPreviousNode { get; set; }
        public int PathId { get; set; }
        public Role NodeRole { get; set; }
    }
}