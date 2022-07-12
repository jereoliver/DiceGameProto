namespace Extensions
{
    public static class IntExtensions
    {
        public static int ConvertAmountOfCrossesToPoints(this int value)
        {
            var returnValue = 0;
            switch (value)
            {
                case 0:
                    returnValue = 0;
                    break;
                case 1:
                    returnValue = 1;
                    break;
                case 2:
                    returnValue = 3;
                    break;
                case 3:
                    returnValue = 6;
                    break;
                case 4:
                    returnValue = 10;
                    break;
                case 5:
                    returnValue = 15;
                    break;
                case 6:
                    returnValue = 21;
                    break;
                case 7:
                    returnValue = 28;
                    break;
                case 8:
                    returnValue = 36;
                    break;
                case 9:
                    returnValue = 45;
                    break;
                case 10:
                    returnValue = 55;
                    break;
                case 11:
                    returnValue = 66;
                    break;
                case 12:
                    returnValue = 78;
                    break;
                default:
                    return 0;
            }

            return returnValue;
        }
    }
}