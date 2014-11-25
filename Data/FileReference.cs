namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a reference to a specific file.
	/// A file reference consists of a folder and file index.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class FileReference
	{

		public int Folder, File;

		public FileReference(int folder, int file)
		{
			this.Set(folder, file);
		}

		public override int GetHashCode()
		{
			return Folder * 10000 + File; //We can have 10000 files per folder
		}

		public override bool Equals(object @ref)
		{
			if (@ref is FileReference)
			{
				return this.File == ((FileReference)@ref).File && this.Folder == ((FileReference)@ref).Folder;
			}
			else
			{
				return false;
			}
		}

		public virtual void Set(int folder, int file)
		{
			this.Folder = folder;
			this.File = file;
		}

		public virtual void Set(FileReference @ref)
		{
			this.Set(@ref.Folder, @ref.File);
		}

		public virtual bool HasFile()
		{
			return this.File != -1;
		}

		public virtual bool HasFolder()
		{
			return this.Folder != -1;
		}

		public override string ToString()
		{
			return "[folder: " + Folder + ", file: " + File + "]";
		}

	}

}