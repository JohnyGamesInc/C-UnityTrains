using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Game2D
{
    public static class ItemUtil
    {

        private static List<string> _energyRevolverTagsNames = new List<string>()
        {
            "EnergyRevolver",
            "BulletRevolver",
            "Bullet_45F",
            "RevolverMagazine"
        };

        private static List<string> _classicRevolverTagsNames = new List<string>()
        {
            "ClassicRevolver",
            "ClassicBulletRevolver",
            "Bullet45",
            "RevovlerMagazineClassic"
        };

        private static List<string> _doublePistolsTagsNames = new List<string>()
        {
            "DoublePistols",
            "DoublePistolsBullets",
            "9mm"
        };


        private static Dictionary<WeaponType, Dictionary<MagazineType, List<string>>> GunMagazineMatcherTagsNames = new Dictionary<WeaponType, Dictionary<MagazineType, List<string>>>()
        {
            [WeaponType.ENERGY_REVOLVER] = { },
            [WeaponType.CLASSIC_REVOLVER] = { },
            [WeaponType.DOUBLE_PISTOLS] = { },
            [WeaponType.AKA] = { },
            [WeaponType.UZI] = { },
            [WeaponType.WINCHESTER] = { },
            [WeaponType.GRANADE] = { },
        };


        public static void InitializeMatcher()
        {
            GunMagazineMatcherTagsNames.Add(WeaponType.ENERGY_REVOLVER, new Dictionary<MagazineType, List<string>>()
            {
                { MagazineType.ENERGY_REVOVLER_BULLET, _energyRevolverTagsNames }
            });

            GunMagazineMatcherTagsNames.Add(WeaponType.CLASSIC_REVOLVER, new Dictionary<MagazineType, List<string>>()
            {
                { MagazineType.CLASSIC_REVOLVER_BULLET, _classicRevolverTagsNames }
            });

            GunMagazineMatcherTagsNames.Add(WeaponType.DOUBLE_PISTOLS, new Dictionary<MagazineType, List<string>>()
            {
                { MagazineType.TWO_PISTOLS_BULLET, _doublePistolsTagsNames }
            });
        }

        private static Dictionary<WeaponType, Dictionary<MagazineType, List<string>>> GetGunsToMagazineToTagNameMatcher()
        {
            return GunMagazineMatcherTagsNames;
        }

        public static Dictionary<WeaponType, MagazineType> GetReferencedTypesOfItemByTagName(GameObject item)
        {
            var tag = item.tag;
            var name = item.name;
            var refsParams = new Dictionary<WeaponType, MagazineType>() { };

            GunMagazineMatcherTagsNames.ToList()
                .ForEach(wt =>
                {
                    wt.Value.ToList()
                    .ForEach(mt =>
                    {
                        var shortName = name.Split()[0];

                        if (mt.Value.Contains(tag) || mt.Value.Contains(name))
                        {
                            WeaponType wtt = wt.Key;
                            MagazineType mtt = mt.Key;
                            refsParams.Add(wtt, mtt);
                        }
                    });
                });

            return refsParams;
        }

        public static List<string> GetAllRelevantTagsNamesByItemTypes(WeaponType wtype, MagazineType mtype)
        {
            Dictionary<MagazineType, List<string>> dict;
            List<string> allTagsNames;

            GunMagazineMatcherTagsNames.TryGetValue(wtype, out dict);
            dict.TryGetValue(mtype, out allTagsNames);

            return allTagsNames;
        }

        public static WeaponType DetermineWeaponClassByItem(Item item)
        {
            if (!item.Type.Equals(ItemType.Weapon))
            {
                throw new System.Exception("Invalid item type for this method");
            }
            var refDefinitions = GetReferencedTypesOfItemByTagName(item.GameItem);
            return refDefinitions.Keys.First();
        }

        public static MagazineType DetermineMagazineTypeByWeapon(Item weapon)
        {
            if (!weapon.Type.Equals(ItemType.Weapon))
            {
                throw new System.Exception("Invalid item type for this method");
            }
            var refDefinitions = GetReferencedTypesOfItemByTagName(weapon.GameItem);
            return refDefinitions.Values.First();
        }
    }
}