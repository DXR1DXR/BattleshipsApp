using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipsApp.Model
{
    public record ShotDTO(bool IsHit, bool IsDestroyed, Boat? Boat);
}
