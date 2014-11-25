namespace SaNi.Spriter.Data
{


	/// <summary>
	/// Represents all the data which necessary to animate a Spriter generated SCML file.
	/// An instance of this class holds <seealso cref="Folder"/>s and <seealso cref="Entity"/> instances.
	/// Specific <seealso cref="Folder"/> and <seealso cref="Entity"/> instances can be accessed via the corresponding methods, i.e. getEntity() and getFolder().
	/// @author Trixt0r
	/// 
	/// </summary>
	public class SpriterData
	{

		internal readonly Folder[] Folders;
		internal readonly Entity[] Entities;
		private int folderPointer = 0, entityPointer = 0;
		public readonly string ScmlVersion, Generator, GeneratorVersion;

        internal SpriterData(string scmlVersion, string generator, string generatorVersion, int folders, int entities)
		{
			this.ScmlVersion = scmlVersion;
			this.Generator = generator;
			this.GeneratorVersion = generatorVersion;
			this.Folders = new Folder[folders];
			this.Entities = new Entity[entities];
		}

		/// <summary>
		/// Adds a folder to this data. </summary>
		/// <param name="folder"> the folder to add </param>
		internal virtual void AddFolder(Folder folder)
		{
			this.Folders[folderPointer++] = folder;
		}

		/// <summary>
		/// Adds an entity to this data. </summary>
		/// <param name="entity"> the entity to add </param>
		internal virtual void AddEntity(Entity entity)
		{
			this.Entities[entityPointer++] = entity;
		}

		/// <summary>
		/// Returns a <seealso cref="Folder"/> instance with the given name. </summary>
		/// <param name="name"> the name of the folder </param>
		/// <returns> the folder with the given name or null if no folder with the given name exists </returns>
		public virtual Folder GetFolder(string name)
		{
			int index = GetFolderIndex(name);
			if (index >= 0)
			{
				return GetFolder(index);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a folder index with the given name. </summary>
		/// <param name="name"> name of the folder </param>
		/// <returns> the folder index of the Folder with the given name or -1 if no folder with the given name exists </returns>
		internal virtual int GetFolderIndex(string name)
		{
			foreach (Folder folder in this.Folders)
			{
				if (folder.Name.Equals(name))
				{
					return folder.Id;
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns a <seealso cref="Folder"/> instance at the given index. </summary>
		/// <param name="index"> the index of the folder </param>
		/// <returns> the <seealso cref="Folder"/> instance at the given index </returns>
		internal virtual Folder GetFolder(int index)
		{
			return this.Folders[index];
		}

		/// <summary>
		/// Returns an <seealso cref="Entity"/> instance with the given index. </summary>
		/// <param name="index"> index of the entity to return. </param>
		/// <returns> the entity with the given index </returns>
		/// @throws <seealso cref="IndexOutOfBoundsException"/> if the index is out of range  </exception>
		public virtual Entity GetEntity(int index)
		{
			return this.Entities[index];
		}

		/// <summary>
		/// Returns an <seealso cref="Entity"/> instance with the given name. </summary>
		/// <param name="name"> the name of the entity </param>
		/// <returns> the entity with the given name or null if no entity with the given name exists </returns>
		public virtual Entity GetEntity(string name)
		{
			int index = GetEntityIndex(name);
			if (index >= 0)
			{
				return GetEntity(index);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns an entity index with the given name. </summary>
		/// <param name="name"> name of the entity </param>
		/// <returns> the entity index of the entity with the given name or -1 if no entity with the given name exists </returns>
		internal virtual int GetEntityIndex(string name)
		{
			foreach (Entity entity in this.Entities)
			{
				if (entity.Name.Equals(name))
				{
					return entity.Id;
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns a <seealso cref="File"/> instance in the given <seealso cref="Folder"/> instance at the given file index. </summary>
		/// <param name="folder"> <seealso cref="Folder"/> instance to search in. </param>
		/// <param name="file"> index of the file </param>
		/// <returns> the <seealso cref="File"/> instance in the given folder at the given file index </returns>
		public virtual File GetFile(Folder folder, int file)
		{
			return folder.GetFile(file);
		}

		/// <summary>
		/// Returns a <seealso cref="File"/> instance in the given folder at the given file index. </summary>
		/// <param name="folder"> index of the folder </param>
		/// <param name="file"> index of the file </param>
		/// <returns> the <seealso cref="File"/> instance in the given folder at the given file index </returns>
		/// @throws <seealso cref="IndexOutOfBoundsException"/> if the folder or file index are out of range  </exception>
		public virtual File GetFile(int folder, int file)
		{
			return GetFile(this.GetFolder(folder), file);
		}

		/// <summary>
		/// Returns a <seealso cref="File"/> instance for the given <seealso cref="FileReference"/> instance. </summary>
		/// <param name="ref"> reference to the file </param>
		/// <returns> the <seealso cref="File"/> instance for the given reference </returns>
		public virtual File GetFile(FileReference @ref)
		{
			return this.GetFile(@ref.Folder, @ref.File);
		}

		public override string ToString()
		{
			string toReturn = this.GetType().Name + "|[Version: " + ScmlVersion + ", Generator: " + Generator + " (" + GeneratorVersion + ")]";
			foreach (Folder folder in Folders)
			{
				toReturn += "\n" + folder;
			}
			foreach (Entity entity in Entities)
			{
				toReturn += "\n" + entity;
			}
			toReturn += "]";
			return toReturn;
		}

	}

}