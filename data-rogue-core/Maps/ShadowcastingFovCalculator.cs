using System;
using System.Collections.Generic;

namespace data_rogue_core.Maps
{
    public class ShadowcastingFovCalculator
    {
        public static List<Vector> InFov(int range, Func<Vector, bool> getTransparent)
        {
            throw new NotImplementedException();
        }
    }
}

/*
                  Shared
             edge by
  Shared     1 & 2      Shared
  edge by\      |      /edge by
  1 & 8   \     |     / 2 & 3
           \1111|2222/
           8\111|222/3
           88\11|22/33
           888\1|2/333
  Shared   8888\|/3333  Shared
  edge by-------@-------edge by
  7 & 8    7777/|\4444  3 & 4
           777/6|5\444
           77/66|55\44
           7/666|555\4
           /6666|5555\
  Shared  /     |     \ Shared
  edge by/      |      \edge by
  6 & 7      Shared     4 & 5
             edge by 
             5 & 6
     */
