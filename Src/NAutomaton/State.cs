﻿using System;
/*
 * dk.brics.automaton
 * 
 * Copyright (c) 2001-2011 Anders Moeller
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NAutomaton
{
    /** 
     * <tt>Automaton</tt> state. 
     * @author Anders M&oslash;ller &lt;<a href="mailto:amoeller@cs.au.dk">amoeller@cs.au.dk</a>&gt;
     */
    [Serializable]
    public class State : IComparable<State>
    {
        bool accept;
        HashSet<Transition> transitions;

        int number;

        int id;
        static int next_id;

        /** 
         * Constructs a new state. Initially, the new state is a reject state. 
         */
        public State()
        {
            ResetTransitions();
            id = next_id++;
        }

        /** 
         * Resets transition set. 
         */
        private void ResetTransitions()
        {
            transitions = new HashSet<Transition>();
        }

        /** 
         * Returns the set of outgoing transitions. 
         * Subsequent changes are reflected in the automaton.
         * @return transition set
         */
        public HashSet<Transition> getTransitions()
        {
            return transitions;
        }

        /**
         * Adds an outgoing transition.
         * @param t transition
         */
        public void addTransition(Transition t)
        {
            transitions.Add(t);
        }

        /** 
         * Sets acceptance for this state.
         * @param accept if true, this state is an accept state
         */
        public void setAccept(bool accept)
        {
            this.accept = accept;
        }

        /**
         * Returns acceptance status.
         * @return true is this is an accept state
         */
        public bool IsAccept()
        {
            return accept;
        }

        /** 
         * Performs lookup in transitions, assuming determinism. 
         * @param c character to look up
         * @return destination state, null if no matching outgoing transition
         * @see #step(char, Collection)
         */
        public State Step(char c)
        {
            foreach (Transition t in transitions)
                if (t.min <= c && c <= t.max)
                    return t.to;
            return null;
        }

        /** 
         * Performs lookup in transitions, allowing nondeterminism.
         * @param c character to look up
         * @param dest collection where destination states are stored
         * @see #step(char)
         */
        public void step(char c, Collection<State> dest)
        {
            foreach (Transition t in transitions)
                if (t.min <= c && c <= t.max)
                    dest.Add(t.to);
        }

        private void AddEpsilon(State to)
        {
            if (to.accept)
                accept = true;

            foreach (Transition t in to.transitions)
                transitions.Add(t);
        }

        /** Returns transitions sorted by (min, reverse max, to) or (to, min, reverse max) */
        private Transition[] GetSortedTransitionArray(bool toFirst)
        {
            Transition[] e = transitions.toArray(new Transition[transitions.size()]);
            Arrays.sort(e, new TransitionComparator(toFirst));
            return e;
        }

        /**
         * Returns sorted list of outgoing transitions.
         * @param to_first if true, order by (to, min, reverse max); otherwise (min, reverse max, to)
         * @return transition list
         */
        public List<Transition> GetSortedTransitions(bool toFirst)
        {
            return Arrays.asList(getSortedTransitionArray(toFirst));
        }

        /** 
         * Returns string describing this state. Normally invoked via 
         * {@link Automaton#toString()}. 
         */
        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("state ").Append(number);
            if (accept)
                b.Append(" [accept]");
            else
                b.Append(" [reject]");
            b.Append(":\n");
            foreach (Transition t in transitions)
                b.Append("  ").Append(t.toString()).Append("\n");
            return b.ToString();
        }

        /**
         * Compares this object with the specified object for order.
         * States are ordered by the time of construction.
         */
        public int CompareTo(State s)
        {
            return s.id - id;
        }
    }
}
