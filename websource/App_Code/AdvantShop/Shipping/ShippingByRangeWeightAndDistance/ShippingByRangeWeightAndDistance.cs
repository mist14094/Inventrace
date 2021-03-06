//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping
{
    public class ShippingByRangeWeightAndDistance : IShippingMethod
    {
        [Serializable]
        public class WeightLimit
        {
            public float Amount { get; set; }
            public float Price { get; set; }
        }
        [Serializable]
        public class DistanceLimit
        {
            public float Amount { get; set; }
            public bool PerUnit { get; set; }
            public float Price { get; set; }
        }

        private readonly List<WeightLimit> _weightLimits;
        private readonly List<DistanceLimit> _distanceLimits;
        private readonly bool _useDistance;
        public ShoppingCart ShoppingCart { get; set; }

        public ShippingByRangeWeightAndDistance(IDictionary<string, string> parameters)
        {
            try
            {
                _weightLimits = JsonConvert.DeserializeObject<List<WeightLimit>>(parameters.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.WeightLimit));
                _distanceLimits = JsonConvert.DeserializeObject<List<DistanceLimit>>(parameters.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.DistanceLimit));
                _useDistance = parameters.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.UseDistance).TryParseBool();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

	public float GetRate(float distance)
        {
            var totalWeight = ShoppingCart.Sum(item => item.Offer.Product.Weight * item.Amount);
            var price = _weightLimits.Where(x => x.Amount >= totalWeight).OrderBy(x => x.Amount).Select(x => x.Price).FirstOrDefault();
            if (distance != 0)
            {
                var temprice = _distanceLimits.Where(x => x.Amount >= distance).OrderBy(x => x.Amount).FirstOrDefault();
                if (temprice != null)
                    price += temprice.PerUnit ? temprice.Price * distance : temprice.Price;
            }
            return price;
        }
		
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for ShippingByWeight");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            throw new Exception("GetShippingOptions method isnot avalible for ShippingByWeight");
        }
    }
}