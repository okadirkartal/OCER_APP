﻿using Application.Core;
using Application.Core.Models;
using Application.Infrastructure.DAL;
using System.Collections.Generic;
using System.Linq;

namespace Application.Business
{
   public class PriceCalculator:IPriceCalculator
    {

        private readonly IRepository<EquipmentTypes>  _equipmentTypesRepository;

        public PriceCalculator() { }

        public PriceCalculator(IRepository<EquipmentTypes> equipmentTypesRepository)
        {
            _equipmentTypesRepository = equipmentTypesRepository;
        }

        public virtual int FindPreDefinedDay(Equipments model)
        {
            int equipmentType = (int)model.Type;

            return (from q in _equipmentTypesRepository.Table
                    where q.Id == equipmentType
                    select new
                    {
                        q.PreDefinedDay}).Single().PreDefinedDay;
        }

        public virtual int CalculateLoyaltyPoint(List<UserEquipments> model)
        {
            var loyaltyPoint = 0;

            foreach (var item in model)
            {
                loyaltyPoint += (item.EquipmentTypeId == (int)EnmEquipmentTypes.Heavy) ? 2 : 1;
            }

            return loyaltyPoint;
        }

        public virtual  decimal CalculateRentalFee(Equipments equipment,int rentDay,IQueryable<RentalFeeTypes> feeTypes)
        {
            decimal resultFee = 0m;

            var preDefinedDay = FindPreDefinedDay(equipment);


            string OneTimeRentalFee = nameof(EnmFeeTypes.OneTimeRentalFee);
            string PremiumDailyFee = nameof(EnmFeeTypes.PremiumDailyFee);
            string RegularDailyFee = nameof(EnmFeeTypes.RegularDailyFee);

            switch (equipment.Type)
            {
                case (int)EnmEquipmentTypes.Heavy:
                    {
                        resultFee = feeTypes.Single(x => x.FeeType == OneTimeRentalFee).Fee +
                                    (feeTypes.Single(x => x.FeeType == PremiumDailyFee).Fee *
                                     rentDay);
                        break;
                    }
                case (int)EnmEquipmentTypes.Regular:
                    {
                        var premiumDailyFee =
                            feeTypes.Single(x => x.FeeType == PremiumDailyFee);


                        resultFee += feeTypes.Single(x => x.FeeType == OneTimeRentalFee).Fee +
                                     premiumDailyFee.Fee *(rentDay>preDefinedDay?preDefinedDay:rentDay);
                        if (rentDay > preDefinedDay)
                        {
                            resultFee += (feeTypes.Single(x => x.FeeType == RegularDailyFee).Fee *
                                          (rentDay-preDefinedDay));
                        }

                        break;
                    }
                case (int)EnmEquipmentTypes.Specialized:
                    {
                        resultFee += (feeTypes.Single(x => x.FeeType == PremiumDailyFee).Fee *
                                      (rentDay>preDefinedDay?preDefinedDay:rentDay));
                        if (rentDay > preDefinedDay)
                        {
                            resultFee += (feeTypes.Single(x => x.FeeType == RegularDailyFee).Fee *
                                          (rentDay - preDefinedDay));
                        }

                        break;
                    }
            }
            return resultFee;
        } 
    }
}
