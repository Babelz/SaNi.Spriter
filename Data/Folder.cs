namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a folder in a Spriter SCML file.
	/// A folder has at least an <seealso cref="#id"/>, <seealso cref="#name"/> and <seealso cref="#files"/> may be empty.
	/// An instance of this class holds an array of <seealso cref="File"/> instances.
	/// Specific <seealso cref="File"/> instances can be accessed via the corresponding methods, i.e getFile().
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Folder
	{

		internal readonly File[] Files;
		private int filePointer = 0;
		public readonly int Id;
		public readonly string Name;

		internal Folder(int id, string name, int files)
		{
			this.Id = id;
			this.Name = name;
			this.Files = new File[files];
		}

		/// <summary>
		/// Adds a <seealso cref="File"/> instance to this folder. </summary>
		/// <param name="file"> the file to add </param>
		internal virtual void AddFile(File file)
		{
			this.Files[filePointer++] = file;
		}

		/// <summary>
		/// Returns a <seealso cref="File"/> instance with the given index. </summary>
		/// <param name="index"> the index of the file </param>
		/// <returns> the file with the given name </returns>
		public virtual File GetFile(int index)
		{
			return Files[index];
		}

		/// <summary>
		/// Returns a <seealso cref="File"/> instance with the given name. </summary>
		/// <param name="name"> the name of the file </param>
		/// <returns> the file with the given name or null if no file with the given name exists </returns>
		public virtual File GetFile(string name)
		{
			int index = GetFileIndex(name);
			if (index >= 0)
			{
				return GetFile(index);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a file index with the given name. </summary>
		/// <param name="name"> the name of the file </param>
		/// <returns> the file index with the given name or -1 if no file with the given name exists </returns>
		internal virtual int GetFileIndex(string name)
		{
			foreach (File file in this.Files)
			{
				if (file.Name.Equals(name))
				{
					return file.Id;
				}
			}
			return -1;
		}

		public override string ToString()
		{
			string toReturn = this.GetType().Name + "|[id: " + Id + ", name: " + Name;
			foreach (File file in Files)
			{
				toReturn += "\n" + file;
			}
			toReturn += "]";
			return toReturn;
		}
	}

}