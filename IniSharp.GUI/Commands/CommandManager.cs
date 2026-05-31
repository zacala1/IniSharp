using System;
using System.Collections.Generic;

namespace IniSharp.GUI.Commands
{
    /// <summary>
    /// Manages undo/redo operations using the Command Pattern.
    /// </summary>
    public sealed class CommandManager
    {
        private sealed class CommandHistoryEntry
        {
            public CommandHistoryEntry(ICommand command, long beforeStateId, long afterStateId)
            {
                Command = command;
                BeforeStateId = beforeStateId;
                AfterStateId = afterStateId;
            }

            public ICommand Command { get; }

            public long BeforeStateId { get; }

            public long AfterStateId { get; }
        }

        private readonly Stack<CommandHistoryEntry> _undoStack = new();
        private readonly Stack<CommandHistoryEntry> _redoStack = new();
        private const int MaxStackSize = 100;

        /// <summary>
        /// Tracks logical document states rather than stack depth so dirty state remains correct
        /// after undoing to a save point and branching with a different command.
        /// </summary>
        private long _currentStateId;
        private long _savePointStateId;
        private long _nextStateId = 1;

        /// <summary>
        /// Occurs when the undo/redo state changes.
        /// </summary>
        public event EventHandler? StateChanged;

        /// <summary>
        /// Gets a value indicating whether there are commands to undo.
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Gets a value indicating whether there are commands to redo.
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Gets a value indicating whether the current state differs from the last save point.
        /// </summary>
        public bool IsDirtyFromSavePoint => _currentStateId != _savePointStateId;

        /// <summary>
        /// Gets the description of the command that would be undone.
        /// </summary>
        public string? UndoDescription => CanUndo ? _undoStack.Peek().Command.Description : null;

        /// <summary>
        /// Gets the description of the command that would be redone.
        /// </summary>
        public string? RedoDescription => CanRedo ? _redoStack.Peek().Command.Description : null;

        /// <summary>
        /// Executes a command and adds it to the undo stack.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            var entry = new CommandHistoryEntry(command, _currentStateId, _nextStateId++);
            _currentStateId = entry.AfterStateId;
            _undoStack.Push(entry);

            TrimUndoStack();

            // Clear redo stack when new command is executed
            _redoStack.Clear();

            OnStateChanged();
        }

        /// <summary>
        /// Undoes the last executed command.
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                return;

            var entry = _undoStack.Pop();
            try
            {
                entry.Command.Undo();
            }
            catch
            {
                _undoStack.Push(entry);
                throw;
            }

            _currentStateId = entry.BeforeStateId;
            _redoStack.Push(entry);

            OnStateChanged();
        }

        /// <summary>
        /// Redoes the last undone command.
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
                return;

            var entry = _redoStack.Pop();
            try
            {
                entry.Command.Execute();
            }
            catch
            {
                _redoStack.Push(entry);
                throw;
            }

            _currentStateId = entry.AfterStateId;
            _undoStack.Push(entry);
            TrimUndoStack();

            OnStateChanged();
        }

        /// <summary>
        /// Clears all undo/redo history.
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _currentStateId = 0;
            _savePointStateId = 0;
            _nextStateId = 1;
            OnStateChanged();
        }

        /// <summary>
        /// Marks the current state as the save point.
        /// Call this after successfully saving the document.
        /// </summary>
        public void MarkSavePoint()
        {
            _savePointStateId = _currentStateId;
            OnStateChanged();
        }

        private void TrimUndoStack()
        {
            if (_undoStack.Count <= MaxStackSize)
                return;

            var entries = _undoStack.ToArray();
            _undoStack.Clear();

            for (int i = MaxStackSize - 1; i >= 0; i--)
            {
                _undoStack.Push(entries[i]);
            }
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
