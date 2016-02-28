using System.Configuration;

namespace SteveBagnall.Trading.Shared.Configuration.MarketConfig
{
    public class MarketCollection : ConfigurationElementCollection
	{
		public MarketCollection()
		{
			AddElementName = "market";
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		public Market this[int index]
		{
			get { return (Market)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		public Market this[Symbol Pair]
		{
			get { return (Market)BaseGet(Pair); }
		}

		public void Add(Market element)
		{
			BaseAdd(element);
		}

		public void Clear()
		{
			BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new Market();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((Market)element).Pair;
		}

		public void Remove(Market element)
		{
			BaseRemove(element.Pair);
		}

		public void Remove(Symbol Pair)
		{
		    BaseRemove(Pair);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

	}
}
