namespace BDSM
{
    public static class Enums
    {
        public enum AvailableMaps : uint
        {
            UNKNOWN = 0,
            SERPUHOV = 1,
            SERPUHOV_WINTER = 2,
            KELN = 3,
            MUROM = 4,
            MUROM_WINTER = 5,
            SOLNECHNOGORSK = 6
        }

        public enum AvailableBuses : uint
        {
            UNKNOWN = 0,
            VECTOR_NEXT = 1,
            CITARO = 2,
            CITARO_L = 3,
            ICARUS = 4,
            LAZ695 = 5,
            LAZ699 = 6,
            LIAZ5292 = 7,
            LIAZ677 = 8,
            MAN15 = 9,
            MAN = 10,
            NEWPAZ3205 = 11,
            PAZ672 = 12,
            SPRINTER = 13,
        }
    }

    public static class EnumUtils
    {
        #region AvailableMaps enum utils.
        public static Enums.AvailableMaps MapUintToEnum(uint l_map)
        {
            switch(l_map)
            {
                case 0: return Enums.AvailableMaps.UNKNOWN;
                case 1: return Enums.AvailableMaps.SERPUHOV;
                case 2: return Enums.AvailableMaps.SERPUHOV_WINTER;
                case 3: return Enums.AvailableMaps.KELN;
                case 4: return Enums.AvailableMaps.MUROM;
                case 5: return Enums.AvailableMaps.MUROM_WINTER;
                case 6: return Enums.AvailableMaps.SOLNECHNOGORSK;
                default: return Enums.AvailableMaps.UNKNOWN;
            }
        }

        public static uint MapEnumToUint(Enums.AvailableMaps l_map)
        {
            switch (l_map)
            {
                case Enums.AvailableMaps.UNKNOWN:           return 0;
                case Enums.AvailableMaps.SERPUHOV:          return 1;
                case Enums.AvailableMaps.SERPUHOV_WINTER:   return 2;
                case Enums.AvailableMaps.KELN:              return 3;
                case Enums.AvailableMaps.MUROM:             return 4;
                case Enums.AvailableMaps.MUROM_WINTER:      return 5;
                case Enums.AvailableMaps.SOLNECHNOGORSK:    return 6;
                default: return 0;
            }
        }

        public static string MapEnumToString(Enums.AvailableMaps mapEnum)
        {
            switch (mapEnum)
            {
                case Enums.AvailableMaps.UNKNOWN:           return ("Unknown");
                case Enums.AvailableMaps.SERPUHOV:          return ("Serpuhov");
                case Enums.AvailableMaps.SERPUHOV_WINTER:   return ("Serpuhov Winter");
                case Enums.AvailableMaps.KELN:              return ("Keln");
                case Enums.AvailableMaps.MUROM:             return ("Murom");
                case Enums.AvailableMaps.MUROM_WINTER:      return ("Murom Winter");
                case Enums.AvailableMaps.SOLNECHNOGORSK:    return ("Solnechnogorsk");
                default: return ("Unknown");
            }
        }
        #endregion

        #region AvailableBuses enum utils.
        public static Enums.AvailableBuses BusUintToEnum(uint busUint)
        {
            switch (busUint)
            {
                case 0: return Enums.AvailableBuses.UNKNOWN;
                case 1: return Enums.AvailableBuses.VECTOR_NEXT;
                case 2: return Enums.AvailableBuses.CITARO;
                case 3: return Enums.AvailableBuses.CITARO_L;
                case 4: return Enums.AvailableBuses.ICARUS;
                case 5: return Enums.AvailableBuses.LAZ695;
                case 6: return Enums.AvailableBuses.LAZ699;
                case 7: return Enums.AvailableBuses.LIAZ5292;
                case 8: return Enums.AvailableBuses.LIAZ677;
                case 9: return Enums.AvailableBuses.MAN15;
                case 10: return Enums.AvailableBuses.MAN;
                case 11: return Enums.AvailableBuses.NEWPAZ3205;
                case 12: return Enums.AvailableBuses.PAZ672;
                case 13: return Enums.AvailableBuses.SPRINTER;
                default: return Enums.AvailableBuses.UNKNOWN;
            }
        }

        public static uint BusEnumToUint(Enums.AvailableBuses bus)
        {
            switch (bus)
            {
                case Enums.AvailableBuses.UNKNOWN:      return 0;
                case Enums.AvailableBuses.VECTOR_NEXT:  return 1;
                case Enums.AvailableBuses.CITARO:       return 2;
                case Enums.AvailableBuses.CITARO_L:     return 3;
                case Enums.AvailableBuses.ICARUS:       return 4;
                case Enums.AvailableBuses.LAZ695:       return 5;
                case Enums.AvailableBuses.LAZ699:       return 6;
                case Enums.AvailableBuses.LIAZ5292:     return 7;
                case Enums.AvailableBuses.LIAZ677:      return 8;
                case Enums.AvailableBuses.MAN15:        return 9;
                case Enums.AvailableBuses.MAN:          return 10;
                case Enums.AvailableBuses.NEWPAZ3205:   return 11;
                case Enums.AvailableBuses.PAZ672:       return 12;
                case Enums.AvailableBuses.SPRINTER:     return 13;
                default: return 0;
            }
        }

        public static string BusEnumToString(Enums.AvailableBuses bus)
        {
            switch (bus)
            {
                case Enums.AvailableBuses.UNKNOWN:      return "Unknown";
                case Enums.AvailableBuses.VECTOR_NEXT:  return "Vector Next";
                case Enums.AvailableBuses.CITARO:       return "Citaro";
                case Enums.AvailableBuses.CITARO_L:     return "Citaro Large";
                case Enums.AvailableBuses.ICARUS:       return "Icarus";
                case Enums.AvailableBuses.LAZ695:       return "LAZ 695";
                case Enums.AvailableBuses.LAZ699:       return "LAZ 699";
                case Enums.AvailableBuses.LIAZ5292:     return "LIAZ 5292";
                case Enums.AvailableBuses.LIAZ677:      return "LIAZ 677";
                case Enums.AvailableBuses.MAN15:        return "MAN 15";
                case Enums.AvailableBuses.MAN:          return "MAN";
                case Enums.AvailableBuses.NEWPAZ3205:   return "New PAZ 3205";
                case Enums.AvailableBuses.PAZ672:       return "PAZ 672";
                case Enums.AvailableBuses.SPRINTER:     return "Sprinter";
                default: return "Unknown";
            }
        }

        public static string GetShortBusName(Enums.AvailableBuses bus)
        {
            switch (bus)
            {
                case Enums.AvailableBuses.UNKNOWN:      return "VN";
                case Enums.AvailableBuses.VECTOR_NEXT:  return "VN";
                case Enums.AvailableBuses.CITARO:       return "CT";
                case Enums.AvailableBuses.CITARO_L:     return "BIGCT";
                case Enums.AvailableBuses.ICARUS:       return "IC";
                case Enums.AvailableBuses.LAZ695:       return "LA";
                case Enums.AvailableBuses.LAZ699:       return "TOURIST";
                case Enums.AvailableBuses.LIAZ5292:     return "LZ5292";
                case Enums.AvailableBuses.LIAZ677:      return "LZ";
                case Enums.AvailableBuses.MAN15:        return "BIGMN";
                case Enums.AvailableBuses.MAN:          return "MN";
                case Enums.AvailableBuses.NEWPAZ3205:   return "PZ";
                case Enums.AvailableBuses.PAZ672:       return "OLDPZ672";
                case Enums.AvailableBuses.SPRINTER:     return "SPR";
                default: return "VN";
            }
        }

        public static string GetShortBusNameById(uint l_busId)
        {
            switch(l_busId)
            {
                case 0: return (GetShortBusName(Enums.AvailableBuses.VECTOR_NEXT));
                case 1: return (GetShortBusName(Enums.AvailableBuses.LIAZ5292));
                case 2: return (GetShortBusName(Enums.AvailableBuses.LIAZ677));
                case 3: return (GetShortBusName(Enums.AvailableBuses.SPRINTER));
                case 4: return (GetShortBusName(Enums.AvailableBuses.NEWPAZ3205));
                case 5: return (GetShortBusName(Enums.AvailableBuses.CITARO));
                case 6: return (GetShortBusName(Enums.AvailableBuses.MAN));
                case 7: return (GetShortBusName(Enums.AvailableBuses.LAZ699));
                case 8: return (GetShortBusName(Enums.AvailableBuses.PAZ672));
                case 9: return (GetShortBusName(Enums.AvailableBuses.LAZ695));
                case 10: return (GetShortBusName(Enums.AvailableBuses.ICARUS));
                case 11: return (GetShortBusName(Enums.AvailableBuses.CITARO_L));
                case 12: return (GetShortBusName(Enums.AvailableBuses.MAN15));
                default: return (GetShortBusName(Enums.AvailableBuses.VECTOR_NEXT));
            }
        }
        #endregion
    }
}
