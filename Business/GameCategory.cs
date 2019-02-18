// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2017 Fortunet


namespace GTI.Modules.Shared
{
    //US5328
    public class GameCategory
    {
        public GameCategory()
        {
        }


        public GameCategory(GameCategory category)
        {
            Id = category.Id;
            Name = category.Name;
            MaxCardLimit = category.MaxCardLimit;
        }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the maximum card limit.
        /// </summary>
        /// <value>
        /// The maximum card limit.
        /// </value>
        public int MaxCardLimit { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}