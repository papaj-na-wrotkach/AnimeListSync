namespace AnimeListSync.DB.Entity.Attribute;

using System;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class CrudEditableAttribute(bool allowEdit = true) : Attribute
{
	public bool AllowEdit => allowEdit;
}
