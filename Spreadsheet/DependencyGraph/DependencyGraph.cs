// Skeleton implementation written by Joe Zachary for CS 3500, January 2017.
// Greg Rosich - u0917936

using System;
using System.Collections.Generic;

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
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
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
        Dictionary<string, HashSet<string>> dependents;
        Dictionary<string, HashSet<string>> dependees;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return dependents.Count; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents[s] != null;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees[s] != null;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            foreach(string t in dependents[s])
            {
                yield return t;
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            foreach (string t in dependees[s])
            {
                yield return t;
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if(dependents[s] == null)
            {
                // creates set for dependent if one doesnt already exist
                dependents[s] = new HashSet<string>();
            }
            // adds dependee t to dependent s
            dependents[s].Add(t);

            if (dependees[t] == null)
            {
                // creates set for dependee if one doesnt already exist
                dependees[t] = new HashSet<string>();
            }
            // adds dependent s to dependee t
            dependees[t].Add(s);

        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            // removes dependee t from dependent s
            dependents[s].Remove(t);
            if(dependents[s].Count == 0)
            {
                // no more dependees so remove s
                dependents.Remove(s);
            }

            // removes dependent s from dependee t
            dependees[t].Remove(s);
            if (dependees[t].Count == 0)
            {
                // no more dependents so remove t
                dependees.Remove(t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            foreach(string r in dependents[s])
            {
                RemoveDependency(s, r);
            }

            foreach(string t in newDependents)
            {
                AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            foreach (string r in dependees[t])
            {
                RemoveDependency(r, t);
            }

            foreach (string s in newDependees)
            {
                AddDependency(s, t);
            }
        }
    }
}
