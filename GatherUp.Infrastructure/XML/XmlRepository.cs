using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.XML
{
	public class XmlRepository<T> : IRepository<T> where T : class, IEntity
	{
		protected readonly string _filePath;

		public XmlRepository()
		{
			string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataXML");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			_filePath = Path.Combine(folderPath, $"{typeof(T).Name}.xml");
		}

		protected List<T> LoadAll()
		{
			var data = XMLSerializer.Load<List<T>>(_filePath);
			return data ?? new List<T>(); 
		}

		protected void SaveAll(List<T> items)
		{
			XMLSerializer.Save(_filePath, items);
		}

		public void Add(T entity)
		{
			var items = LoadAll();
			items.Add(entity);
			SaveAll(items);
		}

		public T? GetById(int id)
		{
			var items = LoadAll();
			return items.FirstOrDefault(x => x.Id == id);
		}

		public IEnumerable<T> GetAll()
		{
			return LoadAll();
		}

		public void Update(T entity)
		{
			var items = LoadAll();
			var existing = items.FirstOrDefault(x => x.Id == entity.Id);
			if (existing != null)
			{
				int index = items.IndexOf(existing);
				items[index] = entity;
				SaveAll(items);
			}
		}

		public void Delete(int id)
		{
			var items = LoadAll();
			var existing = items.FirstOrDefault(x => x.Id == id);
			if (existing != null)
			{
				items.Remove(existing);
				SaveAll(items);
			}
		}
	}
}