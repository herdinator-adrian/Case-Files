using System;

public enum DocumentType
{
	Text,
	Dialogue,
}

public class GameDocument
{
	public string id;
	public string fileName;
	public string sourcePath;
	public DocumentType type;
	public bool unlocked = false;
}


