// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

//US4804: Linear game numbers in the Edge system.

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using GTI.Controls;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// An abstract base class from which all bingo cards should derive.
    /// </summary>
    /// <remarks>All derived classes should implement 
    /// IEquatable(Of T), IComparable, and IComparable(Of T).</remarks>
    public abstract class BingoCard : IEquatable<BingoCard>, IComparable, IComparable<BingoCard>
    {
        #region Constants and Data Types
        protected const int MaxNumberCount = 100;
        #endregion

        #region Member Variables
        protected int m_num;
        protected int m_lookNum;
        protected bool m_isStartingCard; // Rally TA5749
        protected CardLevel m_level;
        protected CardMedia m_media = CardMedia.Electronic;
        protected byte[] m_face;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BingoCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public BingoCard(int number, CardLevel level, CardMedia media, byte[] face)
        {
            m_num = number;
            m_level = level;
            m_media = media;

            if(face != null && face.Length > MaxNumberCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
            else
                m_face = face;
        }

        /// <summary>
        /// Initializes a new instance of the BingoCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public BingoCard(BingoCard card)
        {
            if(card == null)
                throw new ArgumentNullException("card");

            m_num = card.m_num;
            m_level = card.m_level;
            m_media = card.m_media;

            if(card.m_face != null)
                m_face = (byte[])card.m_face.Clone();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header 
        /// instead of BINGO.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public abstract string[] ToString(bool printLotto);

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        public abstract void ParseFaceData(BinaryReader dataReader);

        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public abstract void Draw(Panel panel);

        /// <summary>
        /// Determines whether two BingoCard instances are equal.
        /// </summary>
        /// <param name="obj">The BingoCard to compare with the 
        /// current BingoCard.</param>
        /// <returns>true if the specified BingoCard is equal to the current 
        /// BingoCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            BingoCard card = obj as BingoCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a BingoCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current BingoCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two BingoCard instances are equal.
        /// </summary>
        /// <param name="other">The BingoCard to compare with the 
        /// current BingoCard.</param>
        /// <returns>true if the specified BingoCard is equal to the current 
        /// BingoCard; otherwise, false.</returns>
        public bool Equals(BingoCard other)
        {
            bool equal = (other != null &&
                          (GetType().Equals(other.GetType())) &&
                          m_num == other.m_num &&
                          m_media == other.m_media);

            // Check the level.
            if(m_level != null && !m_level.Equals(other.m_level))
                equal = false;
            else if(m_level == null && other.m_level != null)
                equal = false;

            if(equal)
            {
                // Check the faces.
                if(m_face != null && other.m_face != null
                   && m_face.Length == other.m_face.Length)
                {
                    for(int x = 0; x < m_face.Length; x++)
                    {
                        if(m_face[x] != other.m_face[x])
                            return false;
                    }

                    return true;
                }
                else if(m_face == null && other.m_face == null)
                    return true;
                else
                    return false;
            }
            else
                return equal;
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// obj. Zero - This instance is equal to obj. Greater than zero - 
        /// This instance is greater than obj.</returns>
        /// <exception cref="System.ArgumentException">obj is not a 
        /// BingoCard.</exception>
        public int CompareTo(object obj)
        {
            BingoCard card = obj as BingoCard;

            if(card == null)
                throw new ArgumentException(Resources.NotAClass + "BingoCard");

            return CompareTo(card);
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// other. Zero - This instance is equal to other. Greater than zero - 
        /// This instance is greater than other.</returns>
        public int CompareTo(BingoCard other)
        {
            if(other == null)
                return 1;
            else
                return m_num.CompareTo(other.m_num);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the card's number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_num;
            }
            set
            {
                m_num = value;
            }
        }

        // Rally US 139 - Dispute Resolution Center for Main Stage.
        /// <summary>
        /// Gets or sets the card's look number.
        /// </summary>
        public int LookNumber
        {
            get
            {
                return m_lookNum;
            }
            set
            {
                m_lookNum = value;
            }
        }

        // Rally TA5749
        /// <summary>
        /// Gets or sets whether this card is the start card of a game.
        /// </summary>
        public bool IsStartingCard
        {
            get
            {
                return m_isStartingCard;
            }
            set
            {
                m_isStartingCard = value;
            }
        }

        /// <summary>
        /// Gets or sets the card's level or null if the card does not have a 
        /// level.
        /// </summary>
        public CardLevel Level
        {
            get
            {
                return m_level;
            }
            set
            {
                m_level = value;
            }
        }

        /// <summary>
        /// Gets or sets the card's media.
        /// </summary>
        public CardMedia Media
        {
            get
            {
                return m_media;
            }
            set
            {
                m_media = value;
            }
        }

        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public abstract int FaceNumberCount
        {
            get;
        }

        // PDTS 1098 - Card lookup / dispute resolution.
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public abstract int RowCount
        {
            get;
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public abstract int ColumnCount
        {
            get;
        }

        /// <summary>
        /// Gets the size of the card's face in characters.
        /// </summary>
        public abstract Size FaceSize
        {
            get;
        }

        /// <summary>
        /// Gets or sets an array of bytes containing the card's face 
        /// numbers.
        /// </summary>
        /// <exception cref="System.ArgumentException">The face contains too 
        /// many numbers.</exception>
        public byte[] Face
        {
            get
            {
                return m_face;
            }
            set
            {
                if(value != null && value.Length > MaxNumberCount)
                    throw new ArgumentException(Resources.BingoCardTooLong);
                else
                    m_face = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a standard bingo card played in a 75 number bingo game.
    /// </summary>
    public class Standard75NumberCard : BingoCard, IEquatable<Standard75NumberCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 25;
        protected readonly Size Size = new Size(14, 7);
        // PDTS 1098
        protected const int Rows = 5;
        protected const int Columns = 5;
        protected readonly Point Offset = new Point(6, 16);
        protected readonly Size NumberSize = new Size(76, 16);
        protected readonly Size CellSize = new Size(37, 32); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 2;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Standard75NumberCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public Standard75NumberCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the Standard75NumberCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public Standard75NumberCard(Standard75NumberCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header 
        /// instead of BINGO.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Standard card data is arranged sequentially, in 
            // columnar order.
            // B  I  N  G  O
            // --------------
            // 0  5  10 15 20
            // 1  6  11 16 21
            // 2  7  12 17 22
            // 3  8  13 18 23
            // 4  9  14 19 24

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                if(!printLotto)
                    lines.Add(Resources.BingoCardStandardHeader);
                else
                    lines.Add(Resources.BingoCardStandardHeaderLotto);

                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                            (int)m_face[0 + 0],
                                            (int)m_face[5 + 0],
                                            (int)m_face[10 + 0],
                                            (int)m_face[15 + 0],
                                            (int)m_face[20 + 0]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 1],
                                     (int)m_face[5 + 1],
                                     (int)m_face[10 + 1],
                                     (int)m_face[15 + 1],
                                     (int)m_face[20 + 1]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 2],
                                     (int)m_face[5 + 2],
                                     (int)m_face[10 + 2],
                                     (int)m_face[15 + 2],
                                     (int)m_face[20 + 2]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 3],
                                     (int)m_face[5 + 3],
                                     (int)m_face[10 + 3],
                                     (int)m_face[15 + 3],
                                     (int)m_face[20 + 3]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 4],
                                     (int)m_face[5 + 4],
                                     (int)m_face[10 + 4],
                                     (int)m_face[15 + 4],
                                     (int)m_face[20 + 4]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount)
                throw new ArgumentException();

            // Face Numbers
            Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        // PDTS 1098
        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCard75;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Size = NumberSize;
                label.Location = new Point(Offset.X, 0);
                label.Text = m_num.ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        label = new Label();
                        label.AutoSize = false;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two Standard75NumberCard instances are equal. 
        /// </summary>
        /// <param name="obj">The Standard75NumberCard to compare with the 
        /// current Standard75NumberCard.</param>
        /// <returns>true if the specified Standard75NumberCard is equal to the 
        /// current Standard75NumberCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Standard75NumberCard card = obj as Standard75NumberCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a Standard75NumberCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Standard75NumberCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Standard75NumberCard instances are equal. 
        /// </summary>
        /// <param name="other">The Standard75NumberCard to compare with the 
        /// current Standard75NumberCard.</param>
        /// <returns>true if the specified Standard75NumberCard is equal to the 
        /// current Standard75NumberCard; otherwise, false. </returns>
        public bool Equals(Standard75NumberCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a "Quick Draw" bingo card played in a 75 number bingo game.
    /// </summary>
    public class QuickDrawCard : BingoCard, IEquatable<QuickDrawCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 3;
        protected readonly Size Size = new Size(9, 3);
        protected const int Rows = 1;
        protected const int Columns = 3;

        protected readonly Point Offset = new Point(10, 30);
        protected readonly Size NumberSize = new Size(90, 24);
        protected readonly Size CellSize = new Size(126, 108); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 4;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the QuickDrawCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many numbers.</exception>
        public QuickDrawCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the QuickDrawCard class from an existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null reference.</exception>
        public QuickDrawCard(QuickDrawCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header instead of BINGO.</param>
        /// <returns>An array of strings that represents the current BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Standard card data is arranged sequentially, in 
            // columnar order.
            // QuickDraw
            // --------------
            // 0  5  10 

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                if(!printLotto)
                    lines.Add(Resources.BingoCardQuickDrawHeader);
                else
                    lines.Add(Resources.BingoCardStandardHeaderLotto);

                lines.Add(string.Format(" {0:D2} {1:D2} {2:D2}", (int)m_face[0], (int)m_face[1], (int)m_face[2]));

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                String temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 1) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            //if(numCount != FaceNumberCount)
            //    throw new ArgumentException();
            if(numCount < FaceNumberCount)
                throw new ArgumentException();

            Face = dataReader.ReadBytes(FaceNumberCount);

            if(numCount > FaceNumberCount)
                dataReader.ReadBytes(numCount - FaceNumberCount);

            // Face Numbers
            //Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCardQuickDraw;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Size = NumberSize;
                label.Location = new Point(Offset.X, 0);
                label.Text = m_num.ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the labels.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        label = new Label();
                        label.AutoSize = false;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two QuickDrawCard instances are equal. 
        /// </summary>
        /// <param name="obj">The QuickDrawCard to compare with the current QuickDrawCard.</param>
        /// <returns>true if the specified QuickDrawCard is equal to the current QuickDrawCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            QuickDrawCard card = obj as QuickDrawCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a QuickDrawCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current QuickDrawCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two QuickDrawCard instances are equal. 
        /// </summary>
        /// <param name="other">The QuickDrawCard to compare with the current QuickDrawCard.</param>
        /// <returns>true if the specified QuickDrawCard is equal to the current QuickDrawCard; otherwise, false. </returns>
        public bool Equals(QuickDrawCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a star bingo card played in a 75 number bingo game.
    /// </summary>
    public class Star75NumberCard : BingoCard, IEquatable<Star75NumberCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 26;
        protected readonly Size Size = new Size(14, 8);
        // PDTS 1098
        protected const int Rows = 5;
        protected const int Columns = 5;
        protected readonly Point Offset = new Point(6, 16);
        protected readonly Size NumberSize = new Size(76, 16);
        protected readonly Size CellSize = new Size(37, 32); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 2;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Star75NumberCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public Star75NumberCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the Star75NumberCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public Star75NumberCard(Star75NumberCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header 
        /// instead of BINGO.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Star card data is arranged sequentially, in 
            // columnar order and the star number is the last number.
            // B  I  N  G  O
            // --------------
            // 0  5  10 15 20
            // 1  6  11 16 21
            // 2  7  12 17 22
            // 3  8  13 18 23
            // 4  9  14 19 24
            // STAR 25

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                if(!printLotto)
                    lines.Add(Resources.BingoCardStandardHeader);
                else
                    lines.Add(Resources.BingoCardStandardHeaderLotto);

                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                            (int)m_face[0 + 0],
                                            (int)m_face[5 + 0],
                                            (int)m_face[10 + 0],
                                            (int)m_face[15 + 0],
                                            (int)m_face[20 + 0]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 1],
                                     (int)m_face[5 + 1],
                                     (int)m_face[10 + 1],
                                     (int)m_face[15 + 1],
                                     (int)m_face[20 + 1]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 2],
                                     (int)m_face[5 + 2],
                                     (int)m_face[10 + 2],
                                     (int)m_face[15 + 2],
                                     (int)m_face[20 + 2]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 3],
                                     (int)m_face[5 + 3],
                                     (int)m_face[10 + 3],
                                     (int)m_face[15 + 3],
                                     (int)m_face[20 + 3]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 4],
                                     (int)m_face[5 + 4],
                                     (int)m_face[10 + 4],
                                     (int)m_face[15 + 4],
                                     (int)m_face[20 + 4]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format(Resources.BingoCardStar, (int)m_face[25]).PadRight(Size.Width);
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount - 1)
                throw new ArgumentException();

            // Face Numbers
            byte[] face = new byte[FaceNumberCount];

            Array.Copy(dataReader.ReadBytes(numCount), face, numCount);

            // Skip Bonus numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            // Star Numbers
            byte starNums = dataReader.ReadByte();
            Array.Copy(dataReader.ReadBytes(starNums), 0, face, FaceNumberCount - 1, 1);

            Face = face;
        }

        // PDTS 1098
        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCard75;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Size = NumberSize;
                label.Location = new Point(Offset.X, 0);
                label.Text = m_num.ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        if(m_face[currNum] == m_face[FaceNumberCount - 1])
                        {
                            label = new ImageLabel();
                            ((ImageLabel)label).Background = Resources.Star;
                            ((ImageLabel)label).Stretch = false;
                        }
                        else
                            label = new Label();

                        label.AutoSize = false;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two Star75NumberCard instances are equal. 
        /// </summary>
        /// <param name="obj">The Star75NumberCard to compare with the 
        /// current Star75NumberCard.</param>
        /// <returns>true if the specified Star75NumberCard is equal to the 
        /// current Star75NumberCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Star75NumberCard card = obj as Star75NumberCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a Star75NumberCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Star75NumberCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Star75NumberCard instances are equal. 
        /// </summary>
        /// <param name="other">The Star75NumberCard to compare with the 
        /// current Star75NumberCard.</param>
        /// <returns>true if the specified Star75NumberCard is equal to the 
        /// current Star75NumberCard; otherwise, false. </returns>
        public bool Equals(Star75NumberCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a bonus line bingo card played in a 75 number bingo game.
    /// </summary>
    public class Bonus75NumberCard : BingoCard, IEquatable<Bonus75NumberCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 28;
        protected const int MaxBonusNums = 3;
        protected readonly Size Size = new Size(14, 8);
        // PDTS 1098
        protected const int Rows = 5;
        protected const int Columns = 5;
        protected readonly Point Offset = new Point(6, 16);
        protected readonly Size NumberSize = new Size(76, 16);
        protected readonly Size CellSize = new Size(37, 32); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 2;
        protected readonly Point BonusOffset = new Point(84, 0);
        protected readonly Size BonusCellSize = new Size(37, 16);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Bonus75NumberCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public Bonus75NumberCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the Bonus75NumberCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public Bonus75NumberCard(Bonus75NumberCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header 
        /// instead of BINGO.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Bonus line card data is arranged sequentially, in 
            // columnar order and the star number is the last number.
            // B  I  N  G  O
            // --------------
            // 0  5  10 15 20
            // 1  6  11 16 21
            // 2  7  12 17 22
            // 3  8  13 18 23
            // 4  9  14 19 24
            // BONUS 25 26 27

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                if(!printLotto)
                    lines.Add(Resources.BingoCardStandardHeader);
                else
                    lines.Add(Resources.BingoCardStandardHeaderLotto);

                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                            (int)m_face[0 + 0],
                                            (int)m_face[5 + 0],
                                            (int)m_face[10 + 0],
                                            (int)m_face[15 + 0],
                                            (int)m_face[20 + 0]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 1],
                                     (int)m_face[5 + 1],
                                     (int)m_face[10 + 1],
                                     (int)m_face[15 + 1],
                                     (int)m_face[20 + 1]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 2],
                                     (int)m_face[5 + 2],
                                     (int)m_face[10 + 2],
                                     (int)m_face[15 + 2],
                                     (int)m_face[20 + 2]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 3],
                                     (int)m_face[5 + 3],
                                     (int)m_face[10 + 3],
                                     (int)m_face[15 + 3],
                                     (int)m_face[20 + 3]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 4],
                                     (int)m_face[5 + 4],
                                     (int)m_face[10 + 4],
                                     (int)m_face[15 + 4],
                                     (int)m_face[20 + 4]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format(Resources.BingoCardBonus, (int)m_face[25], (int)m_face[26], (int)m_face[27]);
                temp = temp.Replace("00", "--"); // Replace the unused bonus numbers.
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount - MaxBonusNums)
                throw new ArgumentException();

            // Face Numbers
            byte[] face = new byte[FaceNumberCount];

            Array.Copy(dataReader.ReadBytes(numCount), face, numCount);

            // Bonus numbers.
            byte bonusNums = dataReader.ReadByte();

            if(bonusNums > MaxBonusNums)
                throw new ArgumentException();

            Array.Copy(dataReader.ReadBytes(bonusNums), 0, face, FaceNumberCount - MaxBonusNums, bonusNums);

            // Skip Star numbers.
            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);

            Face = face;
        }

        // PDTS 1098
        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCard75;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Size = NumberSize;
                label.Location = new Point(Offset.X, 0);
                label.Text = m_num.ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        label = new Label();
                        label.AutoSize = false;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }

                // Print the bonus numbers.
                currX = BonusOffset.X;
                currY = BonusOffset.Y;

                label = new Label();
                label.AutoSize = false;
                label.ForeColor = Color.White;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = BonusCellSize;
                label.Location = new Point(currX, currY);

                if(m_face[currNum] != 0)
                    label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
                currX += BonusCellSize.Width + XPadding;
                currNum++;

                label = new Label();
                label.AutoSize = false;
                label.ForeColor = Color.White;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = BonusCellSize;
                label.Location = new Point(currX, currY);

                if(m_face[currNum] != 0)
                    label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
                currX += BonusCellSize.Width + XPadding;
                currNum++;

                label = new Label();
                label.AutoSize = false;
                label.ForeColor = Color.White;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = BonusCellSize;
                label.Location = new Point(currX, currY);

                if(m_face[currNum] != 0)
                    label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two Bonus75NumberCard instances are equal. 
        /// </summary>
        /// <param name="obj">The Bonus75NumberCard to compare with the 
        /// current Bonus75NumberCard.</param>
        /// <returns>true if the specified Bonus75NumberCard is equal to the 
        /// current Bonus75NumberCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Bonus75NumberCard card = obj as Bonus75NumberCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a Bonus75NumberCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Bonus75NumberCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Bonus75NumberCard instances are equal. 
        /// </summary>
        /// <param name="other">The Bonus75NumberCard to compare with the 
        /// current Bonus75NumberCard.</param>
        /// <returns>true if the specified Bonus75NumberCard is equal to the 
        /// current Bonus75NumberCard; otherwise, false. </returns>
        public bool Equals(Bonus75NumberCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a double action bingo card played in a 75 number bingo game.
    /// </summary>
    public class DoubleAction75NumberCard : BingoCard, IEquatable<DoubleAction75NumberCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 50;
        protected readonly Size Size = new Size(29, 7);
        // PDTS 1098
        protected const int Rows = 5;
        protected const int Columns = 5;
        protected readonly Point Offset = new Point(6, 16);
        protected readonly Size NumberSize = new Size(76, 16);
        protected readonly Size CellSize = new Size(37, 32); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 2;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DoubleAction75NumberCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public DoubleAction75NumberCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the DoubleAction75NumberCard class 
        /// from an existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public DoubleAction75NumberCard(DoubleAction75NumberCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header 
        /// instead of BINGO.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Double action card data is arranged sequentially, in 
            // columnar order.
            // B  I  N  G  O
            // --------------
            // 0  10 20 30 40
            // 2  12 22 32 42
            // 4  14 24 34 44
            // 6  16 26 36 46
            // 8  18 28 38 48

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                if(!printLotto)
                    lines.Add(Resources.BingoCardDAHeader);
                else
                    lines.Add(Resources.BingoCardDAHeaderLotto);

                string temp = string.Format("{0:D2}/{1:D2} {2:D2}/{3:D2} {4:D2}/{5:D2} {6:D2}/{7:D2} {8:D2}/{9:D2}",
                                                 (int)m_face[0 + 0],
                                                 (int)m_face[1 + 0],
                                                 (int)m_face[10 + 0],
                                                 (int)m_face[11 + 0],
                                                 (int)m_face[20 + 0],
                                                 (int)m_face[21 + 0],
                                                 (int)m_face[30 + 0],
                                                 (int)m_face[31 + 0],
                                                 (int)m_face[40 + 0],
                                                 (int)m_face[41 + 0]);
                temp = temp.Replace("00", "--");  // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2}/{1:D2} {2:D2}/{3:D2} {4:D2}/{5:D2} {6:D2}/{7:D2} {8:D2}/{9:D2}",
                                      (int)m_face[0 + 2],
                                      (int)m_face[1 + 2],
                                      (int)m_face[10 + 2],
                                      (int)m_face[11 + 2],
                                      (int)m_face[20 + 2],
                                      (int)m_face[21 + 2],
                                      (int)m_face[30 + 2],
                                      (int)m_face[31 + 2],
                                      (int)m_face[40 + 2],
                                      (int)m_face[41 + 2]);
                temp = temp.Replace("00", "--");  // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2}/{1:D2} {2:D2}/{3:D2} {4:D2}/{5:D2} {6:D2}/{7:D2} {8:D2}/{9:D2}",
                                      (int)m_face[0 + 4],
                                      (int)m_face[1 + 4],
                                      (int)m_face[10 + 4],
                                      (int)m_face[11 + 4],
                                      (int)m_face[20 + 4],
                                      (int)m_face[21 + 4],
                                      (int)m_face[30 + 4],
                                      (int)m_face[31 + 4],
                                      (int)m_face[40 + 4],
                                      (int)m_face[41 + 4]);
                temp = temp.Replace("00", "--");  // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2}/{1:D2} {2:D2}/{3:D2} {4:D2}/{5:D2} {6:D2}/{7:D2} {8:D2}/{9:D2}",
                                      (int)m_face[0 + 6],
                                      (int)m_face[1 + 6],
                                      (int)m_face[10 + 6],
                                      (int)m_face[11 + 6],
                                      (int)m_face[20 + 6],
                                      (int)m_face[21 + 6],
                                      (int)m_face[30 + 6],
                                      (int)m_face[31 + 6],
                                      (int)m_face[40 + 6],
                                      (int)m_face[41 + 6]);
                temp = temp.Replace("00", "--");  // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2}/{1:D2} {2:D2}/{3:D2} {4:D2}/{5:D2} {6:D2}/{7:D2} {8:D2}/{9:D2}",
                                      (int)m_face[0 + 8],
                                      (int)m_face[1 + 8],
                                      (int)m_face[10 + 8],
                                      (int)m_face[11 + 8],
                                      (int)m_face[20 + 8],
                                      (int)m_face[21 + 8],
                                      (int)m_face[30 + 8],
                                      (int)m_face[31 + 8],
                                      (int)m_face[40 + 8],
                                      (int)m_face[41 + 8]);
                temp = temp.Replace("00", "--");  // Replace the free square.
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount)
                throw new ArgumentException();

            // Face Numbers
            Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        // PDTS 1098
        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCard75;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Size = NumberSize;
                label.Location = new Point(Offset.X, 0);
                label.Text = m_num.ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        label = new Label();
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.AutoSize = false;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        if(m_face[currNum + 1] != 0)
                            label.Text += Environment.NewLine + m_face[currNum + 1].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum += 2;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two DoubleAction75NumberCard instances are equal. 
        /// </summary>
        /// <param name="obj">The DoubleAction75NumberCard to compare with the 
        /// current DoubleAction75NumberCard.</param>
        /// <returns>true if the specified DoubleAction75NumberCard is equal to the 
        /// current DoubleAction75NumberCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            DoubleAction75NumberCard card = obj as DoubleAction75NumberCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a DoubleAction75NumberCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current 
        /// DoubleAction75NumberCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two DoubleAction75NumberCard instances are 
        /// equal. 
        /// </summary>
        /// <param name="other">The DoubleAction75NumberCard to compare with 
        /// the current DoubleAction75NumberCard.</param>
        /// <returns>true if the specified DoubleAction75NumberCard is equal to
        /// the current DoubleAction75NumberCard; otherwise, false.</returns>
        public bool Equals(DoubleAction75NumberCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a standard bingo card played in a 90 number bingo game.
    /// </summary>
    public class Standard90NumberCard : BingoCard, IEquatable<Standard90NumberCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 27;
        protected readonly Size Size = new Size(26, 4); // Rally US 139
        // PDTS 1098
        protected const int Rows = 3;
        protected const int Columns = 9;
        protected readonly Point Offset = new Point(10, 30);
        protected readonly Size NumberSize = new Size(90, 24);
        protected readonly Size CellSize = new Size(42, 36); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 4;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Standard90NumberCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public Standard90NumberCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the Standard90NumberCard class 
        /// from an existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public Standard90NumberCard(Standard90NumberCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">This parameter is ignored for 90 # 
        /// cards.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Standard 90# card data is arranged sequentially, in 
            // columnar order.
            // --------------------------
            // 00 03 06 09 12 15 18 21 24
            // 01 04 07 10 13 16 19 22 25
            // 02 05 08 11 14 17 20 23 26

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2} {8:D2}",
                                                 (int)m_face[0 + 0],
                                                 (int)m_face[3 + 0],
                                                 (int)m_face[6 + 0],
                                                 (int)m_face[9 + 0],
                                                 (int)m_face[12 + 0],
                                                 (int)m_face[15 + 0],
                                                 (int)m_face[18 + 0],
                                                 (int)m_face[21 + 0],
                                                 (int)m_face[24 + 0]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2} {8:D2}",
                                            (int)m_face[0 + 1],
                                            (int)m_face[3 + 1],
                                            (int)m_face[6 + 1],
                                            (int)m_face[9 + 1],
                                            (int)m_face[12 + 1],
                                            (int)m_face[15 + 1],
                                            (int)m_face[18 + 1],
                                            (int)m_face[21 + 1],
                                            (int)m_face[24 + 1]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2} {8:D2}",
                                            (int)m_face[0 + 2],
                                            (int)m_face[3 + 2],
                                            (int)m_face[6 + 2],
                                            (int)m_face[9 + 2],
                                            (int)m_face[12 + 2],
                                            (int)m_face[15 + 2],
                                            (int)m_face[18 + 2],
                                            (int)m_face[21 + 2],
                                            (int)m_face[24 + 2]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount)
                throw new ArgumentException();

            // Face Numbers
            Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        // PDTS 1098
        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCard90;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Size = NumberSize;
                label.Location = new Point(Offset.X, 0);
                label.Text = m_num.ToString(CultureInfo.CurrentCulture);

                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        label = new Label();
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.AutoSize = false;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two Standard90NumberCard instances are equal. 
        /// </summary>
        /// <param name="obj">The Standard90NumberCard to compare with the 
        /// current Standard90NumberCard.</param>
        /// <returns>true if the specified Standard90NumberCard is equal to the 
        /// current Standard90NumberCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Standard90NumberCard card = obj as Standard90NumberCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a Standard90NumberCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current 
        /// Standard90NumberCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Standard90NumberCard instances are 
        /// equal. 
        /// </summary>
        /// <param name="other">The Standard90NumberCard to compare with 
        /// the current Standard90NumberCard.</param>
        /// <returns>true if the specified Standard90NumberCard is equal to
        /// the current Standard90NumberCard; otherwise, false.</returns>
        public bool Equals(Standard90NumberCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    // PDTS 1098
    /// <summary>
    /// Represents a standard bingo card played in a 80 number bingo game.
    /// </summary>
    public class Standard80NumberCard : BingoCard, IEquatable<Standard80NumberCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 32;
        protected readonly Size Size = new Size(23, 5); // Rally US139
        protected const int Rows = 4;
        protected const int Columns = 8;
        protected readonly Point Offset = new Point(18, 8);
        protected readonly Size NumberSize = new Size(18, 142);
        protected readonly Size CellSize = new Size(46, 34); // Pixels
        protected const int XPadding = 2;
        protected const int YPadding = 2;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Standard80NumberCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public Standard80NumberCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the Standard80NumberCard class 
        /// from an existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public Standard80NumberCard(Standard80NumberCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">This parameter is ignored for 80# 
        /// cards.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Standard 80# card data is arranged sequentially, in 
            // columnar order.
            // -----------------------
            // 00 04 08 12 16 20 24 28
            // 01 05 09 13 17 21 25 29
            // 02 06 10 14 18 22 26 30
            // 03 07 11 15 19 23 27 31

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2}",
                                                 (int)m_face[0 + 0],
                                                 (int)m_face[4 + 0],
                                                 (int)m_face[8 + 0],
                                                 (int)m_face[12 + 0],
                                                 (int)m_face[16 + 0],
                                                 (int)m_face[20 + 0],
                                                 (int)m_face[24 + 0],
                                                 (int)m_face[28 + 0]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2}",
                                            (int)m_face[0 + 1],
                                            (int)m_face[4 + 1],
                                            (int)m_face[8 + 1],
                                            (int)m_face[12 + 1],
                                            (int)m_face[16 + 1],
                                            (int)m_face[20 + 1],
                                            (int)m_face[24 + 1],
                                            (int)m_face[28 + 1]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2}",
                                            (int)m_face[0 + 2],
                                            (int)m_face[4 + 2],
                                            (int)m_face[8 + 2],
                                            (int)m_face[12 + 2],
                                            (int)m_face[16 + 2],
                                            (int)m_face[20 + 2],
                                            (int)m_face[24 + 2],
                                            (int)m_face[28 + 2]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2} {5:D2} {6:D2} {7:D2}",
                                            (int)m_face[0 + 3],
                                            (int)m_face[4 + 3],
                                            (int)m_face[8 + 3],
                                            (int)m_face[12 + 3],
                                            (int)m_face[16 + 3],
                                            (int)m_face[20 + 3],
                                            (int)m_face[24 + 3],
                                            (int)m_face[28 + 3]);

                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount)
                throw new ArgumentException();

            // Face Numbers
            Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            Label label;

            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCard80;
            panel.Size = panel.BackgroundImage.Size;

            // Set the card number.
            if(m_num != 0)
            {
                Font numFont = new Font(panel.Font.FontFamily, panel.Font.SizeInPoints - 2);

                label = new Label();
                label.Font = numFont;
                label.ForeColor = Color.White;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = NumberSize;
                label.Location = new Point(0, Offset.Y);
                
                // The unit number is vertical.
                StringBuilder builder = new StringBuilder();
                string temp = m_num.ToString(CultureInfo.CurrentCulture);

                // Insert new lines after each letter.
                for(int x = 0; x < temp.Length; x++)
                {
                    builder.Append(temp[x]);

                    if(x != temp.Length - 1)
                        builder.Append(Environment.NewLine);
                }

                label.Text = builder.ToString();
                panel.Controls.Add(label);
            }

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        label = new Label();
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.AutoSize = false;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two Standard80NumberCard instances are equal. 
        /// </summary>
        /// <param name="obj">The Standard80NumberCard to compare with the 
        /// current Standard80NumberCard.</param>
        /// <returns>true if the specified Standard80NumberCard is equal to the 
        /// current Standard80NumberCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Standard80NumberCard card = obj as Standard80NumberCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a Standard80NumberCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current 
        /// Standard80NumberCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Standard80NumberCard instances are 
        /// equal. 
        /// </summary>
        /// <param name="other">The Standard80NumberCard to compare with 
        /// the current Standard80NumberCard.</param>
        /// <returns>true if the specified Standard80NumberCard is equal to
        /// the current Standard80NumberCard; otherwise, false.</returns>
        public bool Equals(Standard80NumberCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a standard Crystal Ball Bingo card.
    /// </summary>
    public class CrystalBallCard : BingoCard, IEquatable<CrystalBallCard>
    {
        #region Constants and Data Types
        // Rally US229
        protected const int FaceNumCount = 16;
        protected readonly Size Size = new Size(11, 5);
        // PDTS 1098
        protected const int Rows = 4;
        protected const int Columns = 4;
        protected readonly Point Offset = new Point(47, 68);
        protected readonly Size CellSize = new Size(58, 50); // Pixels
        protected const int XPadding = 3;
        protected const int YPadding = 3;
        #endregion

        #region Member Variables
        protected bool m_isQuickPick;
        protected bool m_isFavorite; // Rally US505
        protected ProductType m_productType; // Rally DE2312 - Selling two different types of CBB at the same time returns an error.
        #endregion

        #region Constructors
        // Rally US505
        // Rally DE2312
        /// <summary>
        /// Initializes a new instance of the CrystalBallCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <param name="isQuickPick">true if this card is a QuickPick card; 
        /// otherwise false.</param>
        /// <param name="isFavorite">true if this card is one of the player's
        /// favorite cards; otherwise false.</param>
        /// <param name="type">The CBB product type of the card.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public CrystalBallCard(int number, CardMedia media, byte[] face, bool isQuickPick, bool isFavorite, ProductType type)
            : base(number, null, media, face)
        {
            m_isQuickPick = isQuickPick;
            m_isFavorite = isFavorite;
            m_productType = type;
        }

        /// <summary>
        /// Initializes a new instance of the CrystalBallCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public CrystalBallCard(CrystalBallCard card)
            : base(card)
        {
            m_isQuickPick = card.m_isQuickPick;
            m_isFavorite = card.m_isFavorite;
        }
        #endregion

        #region Member Methods
        // Rally US229
        /// <summary>
        /// Returns a string that represents the current bingo card.
        /// </summary>
        /// <returns>A string that represents the current 
        /// BingoCard.</returns>
        public override string ToString()
        {
            string returnVal = string.Empty;

            if(m_face != null && m_face.Length > 0)
            {
                returnVal += "#" + m_num.ToString(CultureInfo.CurrentCulture) + ": ";

                foreach(byte num in m_face)
                {
                    if(num != 0)
                        returnVal += num.ToString(CultureInfo.CurrentCulture) + ", ";
                }

                // Remove the last comma.
                if(returnVal.EndsWith(", "))
                    returnVal = returnVal.Substring(0, returnVal.Length - 2);
            }

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">Whether to print LOTTO on the header 
        /// instead of BINGO.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Standard card data is arranged sequentially, in 
            // columnar order.
            // -----------
            // 0  4   8 12
            // 1  5   9 13
            // 2  6  10 14
            // 3  7  11 15

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2}",
                                            (int)m_face[0 + 0],
                                            (int)m_face[4 + 0],
                                            (int)m_face[8 + 0],
                                            (int)m_face[12 + 0]);
                temp = temp.Replace("00", "--"); // Replace the free squares.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2}",
                                     (int)m_face[0 + 1],
                                     (int)m_face[4 + 1],
                                     (int)m_face[8 + 1],
                                     (int)m_face[12 + 1]);
                temp = temp.Replace("00", "--"); // Replace the free squares.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2}",
                                     (int)m_face[0 + 2],
                                     (int)m_face[4 + 2],
                                     (int)m_face[8 + 2],
                                     (int)m_face[12 + 2]);
                temp = temp.Replace("00", "--"); // Replace the free squares.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2}",
                                     (int)m_face[0 + 3],
                                     (int)m_face[4 + 3],
                                     (int)m_face[8 + 3],
                                     (int)m_face[12 + 3]);
                temp = temp.Replace("00", "--"); // Replace the free squares.
                lines.Add(temp);

                temp = "#" + m_num.ToString();

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString();
                    lines.Add(temp.PadRight(Size.Width));
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount)
                throw new ArgumentException();

            // Face Numbers
            Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        // PDTS 1098
        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // Set the background.
            panel.BackgroundImage = Resources.BingoCardCBB;
            panel.Size = panel.BackgroundImage.Size;

            // Loop through the entire face and add the lables.
            if(m_face != null && m_face.Length > 0)
            {
                int currX = Offset.X, currY = Offset.Y, currNum = 0;

                for(int col = 0; col < ColumnCount; col++)
                {
                    for(int row = 0; row < RowCount; row++)
                    {
                        Label label = new Label();
                        label.AutoSize = false;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Size = CellSize;
                        label.Location = new Point(currX, currY);

                        if(m_face[currNum] != 0)
                            label.Text = m_face[currNum].ToString(CultureInfo.CurrentCulture);

                        panel.Controls.Add(label);
                        currY += CellSize.Height + YPadding;
                        currNum++;
                    }

                    currY = Offset.Y;
                    currX += CellSize.Width + XPadding;
                }
            }

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two CrystalBallCard instances are equal. 
        /// </summary>
        /// <param name="obj">The CrystalBallCard to compare with the 
        /// current CrystalBallCard.</param>
        /// <returns>true if the specified CrystalBallCard is equal to the 
        /// current CrystalBallCard; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            CrystalBallCard card = obj as CrystalBallCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a CrystalBallCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current CrystalBallCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two CrystalBallCard instances are equal. 
        /// </summary>
        /// <param name="other">The CrystalBallCard to compare with the 
        /// current CrystalBallCard.</param>
        /// <returns>true if the specified CrystalBallCard is equal to the 
        /// current CrystalBallCard; otherwise, false. </returns>
        public bool Equals(CrystalBallCard other)
        {
            bool equal = (other != null &&
                          (GetType().Equals(other.GetType())));

            if(equal)
            {
                // The only thing we care about is if all the numbers are 
                // the same.
                if(m_face != null && other.m_face != null
                   && m_face.Length == other.m_face.Length)
                {
                    for(int x = 0; x < m_face.Length; x++)
                    {
                        if(m_face[x] != other.m_face[x])
                            return false;
                    }

                    return true;
                }
                else if(m_face == null && other.m_face == null)
                    return true;
                else
                    return false;
            }
            else
                return equal;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        // PDTS 1098
        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }

        /// <summary>
        /// Gets or sets whether this is a QuickPick card.
        /// </summary>
        public bool IsQuickPick
        {
            get
            {
                return m_isQuickPick;
            }
            set
            {
                m_isQuickPick = value;
            }
        }

        // Rally US505
        /// <summary>
        /// Gets or sets if this card is one of the player's favorite cards.
        /// </summary>
        public bool IsFavorite
        {
            get
            {
                return m_isFavorite;
            }
            set
            {
                m_isFavorite = value;
            }
        }

        // Rally DE2312
        /// <summary>
        /// Gets or sets the CBB product type of the card.
        /// </summary>
        public ProductType ProductType
        {
            get
            {
                return m_productType;
            }
            set
            {
                m_productType = value;
            }
        }
        #endregion
    }

    // Rally TA6385 - Add support for melange special games.
    /// <summary>
    /// Represents a bingo card played a Pot-of-Gold bingo game.
    /// </summary>
    public class PotOfGoldCard : BingoCard, IEquatable<PotOfGoldCard>
    {
        #region Contants and Data Types
        protected const int MaxFaceNumCount = 25;
        protected readonly Size Size = new Size(14, 6);
        protected const int Rows = 5;
        protected const int Columns = 5;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PotOfGoldCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public PotOfGoldCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > MaxFaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the PotOfGoldCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public PotOfGoldCard(PotOfGoldCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">This parameter is ignored for Pot-of-Gold 
        /// cards.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Most Pot-of-Gold card data is arranged sequentially, in 
            // columnar order.
            // --------------
            // 0  5  10 15 20
            // 1  6  11 16 21
            // 2  7  12 17 22
            // 3  8  13 18 23
            // 4  9  14 19 24
            //
            // The Match 3 card only contains 3 numbers
            // --------------
            // - - - - -
            // - - - - -
            // - 0 1 2 -
            // - - - - -
            // - - - - -

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                string temp;

                if(m_face.Length == 3)
                {
                    temp = string.Format("-- -- -- -- --");
                    lines.Add(temp);

                    temp = string.Format("-- -- -- -- --");
                    lines.Add(temp);

                    temp = string.Format("-- {0:D2} {1:D2} {2:D2} --",
                                         (int)m_face[0],
                                         (int)m_face[1],
                                         (int)m_face[2]);
                    lines.Add(temp);

                    temp = string.Format("-- -- -- -- --");
                    lines.Add(temp);

                    temp = string.Format("-- -- -- -- --");
                    lines.Add(temp);
                }
                else
                {
                    temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                          (int)m_face[0 + 0],
                                          (int)m_face[5 + 0],
                                          (int)m_face[10 + 0],
                                          (int)m_face[15 + 0],
                                          (int)m_face[20 + 0]);
                    temp = temp.Replace("00", "--"); // Replace the free square.
                    lines.Add(temp);

                    temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                         (int)m_face[0 + 1],
                                         (int)m_face[5 + 1],
                                         (int)m_face[10 + 1],
                                         (int)m_face[15 + 1],
                                         (int)m_face[20 + 1]);
                    temp = temp.Replace("00", "--"); // Replace the free square.
                    lines.Add(temp);

                    temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                         (int)m_face[0 + 2],
                                         (int)m_face[5 + 2],
                                         (int)m_face[10 + 2],
                                         (int)m_face[15 + 2],
                                         (int)m_face[20 + 2]);
                    temp = temp.Replace("00", "--"); // Replace the free square.
                    lines.Add(temp);

                    temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                         (int)m_face[0 + 3],
                                         (int)m_face[5 + 3],
                                         (int)m_face[10 + 3],
                                         (int)m_face[15 + 3],
                                         (int)m_face[20 + 3]);
                    temp = temp.Replace("00", "--"); // Replace the free square.
                    lines.Add(temp);

                    temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                         (int)m_face[0 + 4],
                                         (int)m_face[5 + 4],
                                         (int)m_face[10 + 4],
                                         (int)m_face[15 + 4],
                                         (int)m_face[20 + 4]);
                    temp = temp.Replace("00", "--"); // Replace the free square.
                    lines.Add(temp);
                }

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount == 0 || numCount > MaxFaceNumCount)
                throw new ArgumentException();

            // Face Numbers
            Face = dataReader.ReadBytes(numCount);

            // Skip Bonus and Star numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);
        }

        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // We don't support this in WinForms.

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two PotOfGoldCard instances are equal. 
        /// </summary>
        /// <param name="obj">The PotOfGoldCard to compare with the 
        /// current PotOfGoldCard.</param>
        /// <returns>true if the specified PotOfGoldCard is equal to the
        /// current PotOfGoldCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            PotOfGoldCard card = obj as PotOfGoldCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a PotOfGoldCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current PotOfGoldCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two PotOfGoldCard instances are equal. 
        /// </summary>
        /// <param name="other">The PotOfGoldCard to compare with the
        /// current PotOfGoldCard.</param>
        /// <returns>true if the specified PotOfGoldCard is equal to the 
        /// current PotOfGoldCard; otherwise, false. </returns>
        public bool Equals(PotOfGoldCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return MaxFaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }
    // END: TA6385

    // Rally TA8845 
    /// <summary>
    /// Represents a Slingo bingo card played in a 75 number bingo game.
    /// </summary>
    public class SlingoCard : BingoCard, IEquatable<SlingoCard>
    {
        #region Contants and Data Types
        protected const int FaceNumCount = 31;
        protected readonly Size Size = new Size(14, 9);
        protected const int Rows = 7;
        protected const int Columns = 5;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SlingoCard class.
        /// </summary>
        /// <param name="number">The card's number.</param>
        /// <param name="level">The card's level.</param>       
        /// <param name="media">The card's media.</param>
        /// <param name="face">An array of bytes containing the 
        /// card's face numbers.</param>
        /// <exception cref="System.ArgumentException">face contains too many 
        /// numbers.</exception>
        public SlingoCard(int number, CardLevel level, CardMedia media, byte[] face)
            : base(number, level, media, face)
        {
            if(face != null && face.Length > FaceNumCount)
                throw new ArgumentException(Resources.BingoCardTooLong);
        }

        /// <summary>
        /// Initializes a new instance of the SlingoCard class from an 
        /// existing instance.
        /// </summary>
        /// <param name="card">The existing instance.</param>
        /// <exception cref="System.ArgumentNullException">card is a null 
        /// reference.</exception>
        public SlingoCard(SlingoCard card)
            : base(card)
        {
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string representation of a bingo card.
        /// </summary>
        /// <returns>A string representation of a bingo card.</returns>
        public override string ToString()
        {
            string[] faceLines = ToString(false);
            string returnVal = string.Empty;

            foreach(string line in faceLines)
            {
                returnVal += line + Environment.NewLine;
            }

            // Remove the final new line.
            if(returnVal.EndsWith(Environment.NewLine))
                returnVal = returnVal.Substring(0, returnVal.Length - Environment.NewLine.Length);

            return returnVal;
        }

        /// <summary>
        /// Returns an array of strings that represents the current bingo card.
        /// </summary>
        /// <param name="printLotto">This parameter is ignored for Slingo 
        /// cards.</param>
        /// <returns>An array of strings that represents the current 
        /// BingoCard.</returns>
        public override string[] ToString(bool printLotto)
        {
            // Slingo card data is arranged sequentially, in 
            // columnar order with the joker numbers in the first row and the
            // super joker is the last number.
            //   S L I N G O
            // --------------
            // J1 J2 J3 J4 J5
            // 0  5  10 15 20
            // 1  6  11 16 21
            // 2  7  12 17 22
            // 3  8  13 18 23
            // 4  9  14 19 24
            // SJ

            List<string> lines = new List<string>();

            if(m_face != null && m_face.Length > 0)
            {
                lines.Add(Resources.SlingoCardHeader);

                string temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[25],
                                     (int)m_face[26],
                                     (int)m_face[27],
                                     (int)m_face[28],
                                     (int)m_face[29]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                            (int)m_face[0 + 0],
                                            (int)m_face[5 + 0],
                                            (int)m_face[10 + 0],
                                            (int)m_face[15 + 0],
                                            (int)m_face[20 + 0]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 1],
                                     (int)m_face[5 + 1],
                                     (int)m_face[10 + 1],
                                     (int)m_face[15 + 1],
                                     (int)m_face[20 + 1]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 2],
                                     (int)m_face[5 + 2],
                                     (int)m_face[10 + 2],
                                     (int)m_face[15 + 2],
                                     (int)m_face[20 + 2]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 3],
                                     (int)m_face[5 + 3],
                                     (int)m_face[10 + 3],
                                     (int)m_face[15 + 3],
                                     (int)m_face[20 + 3]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2} {1:D2} {2:D2} {3:D2} {4:D2}",
                                     (int)m_face[0 + 4],
                                     (int)m_face[5 + 4],
                                     (int)m_face[10 + 4],
                                     (int)m_face[15 + 4],
                                     (int)m_face[20 + 4]);
                temp = temp.Replace("00", "--"); // Replace the free square.
                lines.Add(temp);

                temp = string.Format("{0:D2}            ", (int)m_face[30]);
                temp = temp.Replace("00", "--"); // Replace the unused bonus numbers.
                lines.Add(temp);

                string tempLevel = string.Empty;

                if(m_level != null)
                    tempLevel = m_level.Name;

                temp = "#" + m_num.ToString() + " " + tempLevel;

                if(temp.Length > Size.Width)
                    lines.Add(temp.Substring(0, Size.Width - 2) + "…");
                else
                {
                    temp = "#" + m_num.ToString() + " ";
                    lines.Add(temp.PadRight(Size.Width - tempLevel.Length) + tempLevel);
                }
            }

            return lines.ToArray();
        }

        // Rally US498
        /// <summary>
        /// Parses the face data specified to numbers.
        /// </summary>
        /// <param name="dataReader">A BinaryReader containing the data to
        /// read.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurred
        /// reading the stream.</exception>
        /// <exception cref="System.ArgumentException">The data in the reader
        /// is invalid.</exception>
        public override void ParseFaceData(BinaryReader dataReader)
        {
            // First read the count of numbers.
            byte numCount = dataReader.ReadByte();

            if(numCount != FaceNumberCount)
                throw new ArgumentException();

            // Face Numbers
            byte[] face = new byte[FaceNumberCount];

            Array.Copy(dataReader.ReadBytes(numCount), face, numCount);

            // Skip Bonus numbers.
            byte bonusNums = dataReader.ReadByte();
            dataReader.ReadBytes(bonusNums);

            // Skip Star numbers.
            byte starNums = dataReader.ReadByte();
            dataReader.ReadBytes(starNums);

            Face = face;
        }

        /// <summary>
        /// Uses the specified Panel to "Draw" (add Labels) where the card 
        /// numbers should be. 
        /// </summary>
        /// <param name="panel">The Panel to which to add the number 
        /// Labels.</param>
        public override void Draw(Panel panel)
        {
            panel.SuspendLayout();

            // First, clear the panel.
            panel.Controls.Clear();

            // We don't support this in WinForms.

            panel.ResumeLayout();
        }

        /// <summary>
        /// Determines whether two SlingoCard instances are equal. 
        /// </summary>
        /// <param name="obj">The SlingoCard to compare with the 
        /// current SlingoCard.</param>
        /// <returns>true if the specified SlingoCard is equal to the 
        /// current SlingoCard; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            SlingoCard card = obj as SlingoCard;

            if(card == null)
                return false;
            else
                return Equals(card);
        }

        /// <summary>
        /// Serves as a hash function for a SlingoCard. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current SlingoCard.</returns>
        public override int GetHashCode()
        {
            return m_num.GetHashCode();
        }

        /// <summary>
        /// Determines whether two SlingoCard instances are equal. 
        /// </summary>
        /// <param name="other">The SlingoCard to compare with the 
        /// current SlingoCard.</param>
        /// <returns>true if the specified SlingoCard is equal to the 
        /// current SlingoCard; otherwise, false. </returns>
        public bool Equals(SlingoCard other)
        {
            return base.Equals(other);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the count of numbers on this bingo card's face.
        /// </summary>
        public override int FaceNumberCount
        {
            get
            {
                return FaceNumCount;
            }
        }

        /// <summary>
        /// Gets the size of the card's face (in characters).
        /// </summary>
        public override Size FaceSize
        {
            get
            {
                return Size;
            }
        }

        /// <summary>
        /// Gets the count of rows on this bingo card's face.
        /// </summary>
        public override int RowCount
        {
            get
            {
                return Rows;
            }
        }

        /// <summary>
        /// Gets the count of columns on this bingo card's face.
        /// </summary>
        public override int ColumnCount
        {
            get
            {
                return Columns;
            }
        }
        #endregion
    }
    // END: TA8845

    // PDTS 1098
    /// <summary>
    /// This class contains methods for creating bingo cards based on game and 
    /// card types.
    /// </summary>
    public static class BingoCardFactory
    {
        #region Static Methods
        /// <summary>
        /// Creates an instance of a derived BingoCard class based on the 
        /// specified game and card type.
        /// </summary>
        /// <param name="gameType">The type of game played with the 
        /// card.</param>
        /// <param name="cardType">The type of card used to play the 
        /// game.</param>
        /// <param name="cardMedia">The card's media type.</param>
        /// <returns>An instance of a derived BingoCard class.</returns>
        /// <remarks>All properties of the card will remain default values 
        /// except CardMedia.</remarks>
        public static BingoCard CreateBingoCard(GameType gameType, CardType cardType, CardMedia cardMedia)
        {
            BingoCard card;

            // Rally US498
            // Rally TA6385
            if ((gameType == GameType.SeventyFiveNumberBingo || gameType == GameType.ThreeOn ||
                gameType == GameType.FourOn || gameType == GameType.SixOn || gameType == GameType.NineOn ||
                gameType == GameType.AllStar || gameType == GameType.B13 || gameType == GameType.BingoStorm) &&
                (cardType == CardType.Standard || cardType == CardType.NoFree || cardType == CardType.DoubleFree))
            {
                card = new Standard75NumberCard(0, null, cardMedia, null);
            }
            else if (gameType == GameType.SeventyFiveNumberBingo && cardType == CardType.Star)
                card = new Star75NumberCard(0, null, cardMedia, null);
            else if (gameType == GameType.SeventyFiveNumberBingo && cardType == CardType.BonusLine)
                card = new Bonus75NumberCard(0, null, cardMedia, null);
            else if (gameType == GameType.SeventyFiveNumberBingo && cardType == CardType.DoubleAction)
                card = new DoubleAction75NumberCard(0, null, cardMedia, null);
            else if (gameType == GameType.DoubleAction && cardType == CardType.Standard)
                card = new DoubleAction75NumberCard(0, null, cardMedia, null);
            else if (gameType == GameType.NinetyNumberBingo && cardType == CardType.Standard)
                card = new Standard90NumberCard(0, null, cardMedia, null);
            else if (gameType == GameType.EightyNumberBingo && cardType == CardType.Standard)
                card = new Standard80NumberCard(0, null, cardMedia, null);
            else if ((gameType == GameType.CrystalBall || gameType == GameType.PickYurPlatter) &&
                    cardType == CardType.Standard) // Rally US229 & US505
                card = new CrystalBallCard(0, cardMedia, null, false, false, 0); // Rally DE2312
            else if (gameType == GameType.PotOfGold && cardType == CardType.Standard)
                card = new PotOfGoldCard(0, null, cardMedia, null);
            else if (gameType == GameType.Slingo && cardType == CardType.Standard) // Rally TA8845
                card = new SlingoCard(0, null, cardMedia, null);
            else if (cardType == CardType.QuickDraw)
                card = new QuickDrawCard(0, null, cardMedia, null);
            else
                throw new ModuleException(Resources.UnknownBingoCard);
            // END: TA6385

            return card;
        }
        #endregion
    }

    /// <summary>
    /// Represents a bingo game.
    /// </summary>
    public class BingoGame : IEquatable<BingoGame>, IComparable, IComparable<BingoGame>
    {
        #region Member Variables
        protected GameType m_type;
        protected int m_linearNum;
        protected int m_displayNum;
        protected bool m_consecutiveCards;
        protected List<BingoCard> m_cards = new List<BingoCard>();
        #endregion

        #region Constructors.
        /// <summary>
        /// Initializes a new instance of the BingoGame class.
        /// </summary>
        public BingoGame()
        {
        }

        //US4804. added linear game number parameter
        /// <summary>
        /// Initializes a new instance of the BingoGame class.
        /// </summary>
        /// <param name="type">The game's type.</param>
        /// <param name="gameSequenceNumber"></param>
        /// <param name="linearNumber">The game's unique linear game 
        /// number.</param>
        /// <param name="displayNumber">The game's display number.</param>
        /// <param name="consecutiveCards">Whether the cards in this game are consecutively numbered.</param>
        /// <param name="continuationGameCount">Number of parts of a continuation game. Default is 0</param>
        /// <param name="useLinearGameNumbers">determines whether to use linear display number</param>
        public BingoGame(GameType type,int gameSequenceNumber, int linearNumber, int displayNumber, bool consecutiveCards, int continuationGameCount, bool useLinearGameNumbers = false)
        {
            m_type = type;
            LinearDisplayNumber = linearNumber;//US4804
            m_linearNum = gameSequenceNumber;//US4804
            ContinuationGameCount = continuationGameCount;//US4804
            m_displayNum = displayNumber;
            m_consecutiveCards = consecutiveCards;
            UseLinearGameNumbers = useLinearGameNumbers;//US4804
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two BingoGame instances are equal. 
        /// </summary>
        /// <param name="obj">The BingoGame to compare with the 
        /// current BingoGame.</param>
        /// <returns>true if the specified BingoGame is equal to the current 
        /// BingoGame; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            BingoGame game = obj as BingoGame;

            if(game == null)
                return false;
            else
                return Equals(game);
        }

        /// <summary>
        /// Serves as a hash function for a BingoGame. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current BingoGame.</returns>
        public override int GetHashCode()
        {
            return m_linearNum.GetHashCode();
        }

        /// <summary>
        /// Determines whether two BingoGame instances are equal. 
        /// </summary>
        /// <param name="other">The BingoGame to compare with the 
        /// current BingoGame.</param>
        /// <returns>true if the specified BingoGame is equal to the current 
        /// BingoGame; otherwise, false. </returns>
        public bool Equals(BingoGame other)
        {
            return (other != null &&
                    m_linearNum == other.m_linearNum);
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// obj. Zero - This instance is equal to obj. Greater than zero - 
        /// This instance is greater than obj.</returns>
        /// <exception cref="System.ArgumentException">obj is not a 
        /// BingoCard.</exception>
        public int CompareTo(object obj)
        {
            BingoGame game = obj as BingoGame;

            if(game == null)
                throw new ArgumentException(Resources.NotAClass + "BingoGame");

            return CompareTo(game);
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// other. Zero - This instance is equal to other. Greater than zero - 
        /// This instance is greater than other.</returns>
        public int CompareTo(BingoGame other)
        {
            if(other == null)
                return 1;
            else
                return m_displayNum.CompareTo(other.m_displayNum);
        }

        /// <summary>
        /// Adds a bingo card to this game.
        /// </summary>
        /// <param name="card">The card to be added.</param>
        public void AddCard(BingoCard card)
        {
            m_cards.Add(card);
        }

        /// <summary>
        /// Removes any cards in the game which match the specified card.
        /// </summary>
        /// <param name="card">The card to remove.</param>
        public void RemoveCard(BingoCard card)
        {
            m_cards.Remove(card);
        }

        /// <summary>
        /// Gets the bingo cards in this game.
        /// </summary>
        /// <returns>An array of BingoCard objects.</returns>
        public BingoCard[] GetCards()
        {
            return m_cards.ToArray();
        }

        // US2228
        // Rally TA5749
        /// <summary>
        /// Returns the card numbers for every card in this game.
        /// If the game has consecutive cards, then only the first and last
        /// card numbers will be returned, unless the card range has wrapped or
        /// has skipped cards, in which case multiple sets of number will be
        /// returned.
        /// </summary>
        /// <param name="onlyStartingCards">true to return only starting cards;
        /// otherwise all cards will be returned.</param>
        /// <returns>An array of card numbers.</returns>
        public int[] GetCardNumbers(bool onlyStartingCards)
        {
            List<int> nums = new List<int>();

            if(m_cards.Count > 0)
            {
                if(m_consecutiveCards)
                {
                    // Add each card to a temp array.
                    List<int> sortedNums = new List<int>();

                    foreach(BingoCard card in m_cards)
                    {
                        sortedNums.Add(card.Number);
                    }

                    if(sortedNums.Count == 1) // Special case: one card.
                    {
                        nums.Add(sortedNums[0]);
                        nums.Add(sortedNums[0]);
                    }
                    else
                    {
                        // Sort the cards.
                        sortedNums.Sort();

                        for(int x = 0; x < sortedNums.Count; x++)
                        {
                            // Add the number if it's the first one or last one. 
                            if(x == 0 || x == sortedNums.Count - 1)
                                nums.Add(sortedNums[x]);
                            
                            // If it is not the last one and the numbers step
                            // greater than one, add it.
                            if((x < sortedNums.Count - 1) && (sortedNums[x] + 1 != sortedNums[x + 1]))
                            {
                                nums.Add(sortedNums[x]);
                                nums.Add(sortedNums[x + 1]);
                            }
                        }
                    }
                }
                else
                {
                    // Return all the card numbers.
                    foreach(BingoCard card in m_cards)
                    {
                        if(!onlyStartingCards || (onlyStartingCards && card.IsStartingCard))
                            nums.Add(card.Number);
                    }
                }
            }

            return nums.ToArray();
        }
        // END: TA5749
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the type of this game.
        /// </summary>
        public GameType Type
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

        // Rally US505
        /// <summary>
        /// Gets or sets whether this game contains Crystal Ball cards.
        /// </summary>
        public bool HasCrystalBallCards
        {
            get
            {
                // Assume that a game can only contain CBB or not.
                return (m_cards.Count > 0 && m_cards[0] is CrystalBallCard);
            }
        }

        /// <summary>
        /// Gets or sets the linear number of this game. This is the game sequence number
        /// </summary>
        public int LinearNumber
        {
            get
            {
                return m_linearNum;
            }
            set
            {
                m_linearNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the display number of this game.
        /// </summary>
        public int DisplayNumber
        {
            get
            {
                return m_displayNum;
            }
            set
            {
                m_displayNum = value;
            }
        }

        //US4804
        /// <summary>
        /// Gets or sets the linear display number.
        /// </summary>
        public int LinearDisplayNumber { get; set; }

        //US4804
        /// <summary>
        /// Gets or sets the linear display number.
        /// </summary>
        public int ContinuationGameCount { get; set; }

        /// <summary>
        /// Gets or sets whether this game has consecutive card numbers.
        /// </summary>
        public bool ConsecutiveCards
        {
            get
            {
                return m_consecutiveCards;
            }
            set
            {
                m_consecutiveCards = value;
            }
        }

        /// <summary>
        /// Gets the total number of cards in this game.
        /// </summary>
        public int TotalCards
        {
            get
            {
                return m_cards.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use linear game numbers].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use linear game numbers]; otherwise, <c>false</c>.
        /// </value>
        public bool UseLinearGameNumbers { get; set; }


        /// <summary>
        /// Gets the display number to string.
        /// </summary>
        /// <value>
        /// The get display number to string.
        /// </value>
        public string GetDisplayNumberToString
        {
            get
            {
                if (UseLinearGameNumbers)
                {
                    if (ContinuationGameCount > 1)
                    {
                        return string.Format("{0}-{1}", LinearDisplayNumber,
                            LinearDisplayNumber + ContinuationGameCount - 1);
                    }

                    return LinearDisplayNumber.ToString();
                }

                return DisplayNumber.ToString();
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a bingo session.
    /// </summary>
    public class BingoSession : IEquatable<BingoSession>, IComparable, IComparable<BingoSession>
    {
        #region Member Variables
        protected short m_sessionNum;
        protected bool m_sameCards;
        protected List<BingoGame> m_games = new List<BingoGame>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BingoSession class.
        /// </summary>
        /// <param name="number">The session's number.</param>
        /// <param name="sameCards">Whether every game in this session uses the 
        /// same cards.</param>
        public BingoSession(short number, bool sameCards)
        {
            m_sessionNum = number;
            m_sameCards = sameCards;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two BingoSession instances are equal. 
        /// </summary>
        /// <param name="obj">The BingoSession to compare with the 
        /// current BingoSession.</param>
        /// <returns>true if the specified BingoSession is equal to the current 
        /// BingoSession; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            BingoSession session = obj as BingoSession;

            if(session == null)
                return false;
            else
                return Equals(session);
        }

        /// <summary>
        /// Serves as a hash function for a BingoSession. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current BingoSession.</returns>
        public override int GetHashCode()
        {
            return m_sessionNum.GetHashCode();
        }

        /// <summary>
        /// Determines whether two BingoSession instances are equal. 
        /// </summary>
        /// <param name="other">The BingoSession to compare with the 
        /// current BingoSession.</param>
        /// <returns>true if the specified BingoSession is equal to the current 
        /// BingoSession; otherwise, false. </returns>
        public bool Equals(BingoSession other)
        {
            return (other != null &&
                    m_sessionNum == other.m_sessionNum);
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// obj. Zero - This instance is equal to obj. Greater than zero - 
        /// This instance is greater than obj.</returns>
        /// <exception cref="System.ArgumentException">obj is not a 
        /// BingoSession.</exception>
        public int CompareTo(object obj)
        {
            BingoSession session = obj as BingoSession;

            if(session == null)
                throw new ArgumentException(Resources.NotAClass + "BingoSession");

            return CompareTo(session);
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// other. Zero - This instance is equal to other. Greater than zero - 
        /// This instance is greater than other.</returns>
        public int CompareTo(BingoSession other)
        {
            if(other == null)
                return 1;
            else
                return m_sessionNum.CompareTo(other.m_sessionNum);
        }

        /// <summary>
        /// Adds a game to this session.
        /// </summary>
        /// <param name="game">The bingo game to add.</param>
        public void AddGame(BingoGame game)
        {
            m_games.Add(game);
        }

        /// <summary>
        /// Adds more than one game to this session.
        /// </summary>
        /// <param name="games">The bingo games to add.</param>
        public void AddGames(BingoGame[] games)
        {
            m_games.AddRange(games);
        }

        /// <summary>
        /// Removes any game that matches the specified game.
        /// </summary>
        /// <param name="game">The bingo game to remove.</param>
        public void RemoveGame(BingoGame game)
        {
            m_games.Remove(game);
        }

        /// <summary>
        /// Gets an array of BingoGames associated with this session.
        /// </summary>
        /// <returns>A sorted array of BingoGames.</returns>
        public BingoGame[] GetGames()
        {
            m_games.Sort();
            return m_games.ToArray();
        }

        /// <summary>
        /// Returns a BingoGame based on the specified linear number (Game Sequence Number). 
        /// </summary>
        /// <param name="linearNumber">The linear number of the game.</param>
        /// <returns>A BingoGame object or null if the game was 
        /// not found.</returns>
        public BingoGame GetGameByLinearNumber(int linearNumber)
        {
            BingoGame returnVal = null;

            foreach(BingoGame game in m_games)
            {
                if(game.LinearNumber == linearNumber)
                {
                    returnVal = game;
                    break;
                }
            }

            return returnVal;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the session's number.
        /// </summary>
        public short Number
        {
            get
            {
                return m_sessionNum;
            }
            set
            {
                m_sessionNum = value;
            }
        }

        /// <summary>
        /// Gets or sets whether every game in this session uses the same 
        /// cards.
        /// </summary>
        public bool SameCards
        {
            get
            {
                return m_sameCards;
            }
            set
            {
                m_sameCards = value;
            }
        }

        /// <summary>
        /// Gets the number of games in this session.
        /// </summary>
        public int GameCount
        {
            get
            {
                return m_games.Count;
            }
        }

        #endregion
    }
}