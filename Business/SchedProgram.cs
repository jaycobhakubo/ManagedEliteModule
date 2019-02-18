using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    /// <summary>
    /// Enumerates the types of bingo programs.
    /// </summary>
    public enum ProgramType
    {
        Regular = 1,
        BlazingCash = 2
    }

    public class SchedProgram : IEquatable<SchedProgram>
    {
        #region Member Variables
        private int m_id;
        private string m_name;
        private bool m_isActive = true;
        private ProgramType? m_type = ProgramType.Regular;
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two Program instances are equal.
        /// </summary>
        /// <param name="obj">The Program to compare with the current
        /// Program.</param>
        /// <returns>true if the specified Program is equal to the
        /// current Program; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            SchedProgram prog = obj as SchedProgram;

            if(prog == null)
                return false;
            else
                return Equals(prog);
        }

        /// <summary>
        /// Determines whether two Program instances are equal.
        /// </summary>
        /// <param name="other">The Program to compare with the current
        /// Program.</param>
        /// <returns>true if the specified Program is equal to the
        /// current Program; otherwise, false.</returns>
        public bool Equals(SchedProgram other)
        {
            return (other != null && Id > 0 && Id == other.Id);
        }

        /// <summary>
        /// Returns a string that represents the current Program.
        /// </summary>
        /// <returns>A string that represents the current 
        /// Program.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Serves as a hash function for a Program. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Program.</returns>
        /// <remarks>This method uses Id's implementation of
        /// GetHashCode.</remarks>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Full initialization constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="programType"></param>
        public SchedProgram(int id, string name, bool isActive, ProgramType? programType = null)
        {
            Id = id;
            Name = name;
            Type = programType;
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        public SchedProgram()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        /// <param name="prog">The program to copy.</param>
        /// <exception cref="System.ArgumentNullException">prog is a null
        /// reference.</exception>
        public SchedProgram(SchedProgram prog)
        {
            if(prog == null)
                throw new ArgumentNullException("prog");

            Id = prog.Id;
            Name = prog.Name;
            this.Type = prog.Type;
            IsActive = prog.IsActive;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the type; null if unknown/unspecified
        /// </summary>
        public ProgramType? Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this program is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return m_isActive;
            }
            set
            {
                m_isActive = value;
            }
        }
        #endregion
    }

}
