using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis
{
    public class Tokenizer(InputReader inputReader, DFA automaton, Func<string, string, Token?> postProcess)
    {
        private readonly InputReader _inputReader = inputReader;
        private readonly DFA _automaton = automaton;
        private readonly Func<string, string, Token?> _postProcess = postProcess;

        public Token GetNextToken()
        {
            // End of file.
            if (_inputReader.CurrentSymbol == Alphabet.EndOfFile)
            {
                _inputReader.CloseFile();
                return Token.End;
            }

            State? _currentState = _automaton.InitialState;
            while (_currentState != null)
            {
                if (_currentState.TryGetNextState(_inputReader.CurrentSymbol, out _currentState))
                {
                    if (_automaton.AcceptingStates.TryGetValue(_currentState, out string? tokenName))
                    {
                        // Accepting state.
                        _inputReader.RetractPointer();
                        string lexeme = _inputReader.ConsumeLexeme();

                        Token? token = _postProcess.Invoke(tokenName, lexeme);
                        if (token == null)
                        {
                            return GetNextToken();
                        }
                        else
                        {
                            return token;
                        }
                    }
                    else
                    {
                        // Non accepting state.
                        _inputReader.AdvancePointer();
                    } 
                }
            }

            // Dead state.
            _inputReader.CloseFile();
            throw new Exception("Unable to generate next token.");
        }
    }
}
