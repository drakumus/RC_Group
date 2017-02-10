// Skeleton implementation written by Joe Zachary for CS 3500, January 2017.
// Greg Rosich - u0917936

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> dependees;
        private Dictionary<string, HashSet<string>> dependents;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }

        public DependencyGraph(DependencyGraph dg)
        {
            dependents = dg.dependents;
            dependees = dg.dependees;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                int count = 0;
                foreach(HashSet<string> dependents in dependees.Values)
                {
                    foreach(string dependent in dependents)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            try
            {
                return dependees[s].Count != 0;
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            try
            {
                return dependents[s].Count != 0;
            }
            catch(System.Collections.Generic.KeyNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            if (!dependees.ContainsKey(s))
            {
                yield break;
            }
            foreach (string t in dependees[s])
            {
                yield return t;
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            if (!dependents.ContainsKey(s))
            {
                yield break;
            }
            foreach (string t in dependents[s])
            {
                yield return t;
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            if (!dependees.ContainsKey(s))
            {
                // creates set for dependee if one doesnt already exist
                dependees[s] = new HashSet<string>();
            }
            if (!dependents.ContainsKey(s))
            {
                // creates set for dependee as dependent if one doesnt already exist
                dependents[s] = new HashSet<string>();
            }
            // adds dependent t to dependee s
            dependees[s].Add(t);

            if (!dependents.ContainsKey(t))
            {
                // creates set for dependent if one doesnt already exist
                dependents[t] = new HashSet<string>();
            }
            if (!dependees.ContainsKey(t))
            {
                // creates set for dependent as dependee if one doesnt already exist
                dependees[t] = new HashSet<string>();
            }
            // adds dependee s to dependent t
            dependents[t].Add(s);

        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            if (!dependees.ContainsKey(s) || !dependees[s].Contains(t))
            {
                return;
            }
            // removes dependent t from dependees s
            dependees[s].Remove(t);
            if(dependees[s].Count == 0 && dependents[s].Count == 0)
            {
                // no more dependees or dependents relating to s so remove s
                dependees.Remove(s);
                dependents.Remove(s);
            }

            // removes dependee s from dependent t
            dependents[t].Remove(s);
            if (dependents[t].Count == 0 && dependees[t].Count == 0)
            {
                // no more dependents or dependees relating to t so remove t
                dependents.Remove(t);
                dependees.Remove(t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// If newDependents contains a null value, throws an ArgumentNullException.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null || newDependents == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            // checks if dependee s exists
            if (dependees.ContainsKey(s))
            {
                var list = dependees[s].ToList();
                // removes all dependents from dependee s
                for (int i = dependees[s].Count - 1; i >= 0; i--)
                {
                    RemoveDependency(s, list[i]);
                }
            }
            // adds all new dependents to dependee s
            foreach(string t in newDependents)
            {
                if(t == null)
                {
                    throw new ArgumentNullException("Cannot pass null parameter");
                }
                AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// If newDependents contains a null value, throws an ArgumentNullException.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (t == null || newDependees == null)
            {
                throw new ArgumentNullException("Cannot pass null parameter");
            }
            // checks if dependent t exists
            if (dependents.ContainsKey(t))
            {
                var list = dependents[t].ToList();
                //removes all dependees from dependent t
                for(int i = dependents[t].Count - 1; i >= 0; i--)
                {
                    RemoveDependency(list[i], t);
                }
            }
            // adds all new dependees to dependent t
            foreach (string s in newDependees)
            {
                if(s == null)
                {
                    throw new ArgumentNullException("Cannot pass null parameter");
                }
                AddDependency(s, t);
            }
        }
    }
}
