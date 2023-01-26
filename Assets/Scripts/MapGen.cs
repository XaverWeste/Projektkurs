using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MapGen{

	 private static Coord last;
    

     public static Room[,] Gen(int width, int height, int amountOfRooms) {
            var random = new System.Random();
            var map = new Room[width, height];
            
            var coords = new Coord(random.Next(0, width), random.Next(0, height));
            map[coords.X, coords.Y] = new Room(RoomType.Starting, new List<Dir>());
		
            FillRec(map, coords, amountOfRooms - 1, random);

			map[last.X, last.Y] = new Room(RoomType.Boss, map[last.X, last.Y].Dirs);

            return map;
     }

     private static void FillRec(Room[,] map, Coord coord, int amountOfRooms, System.Random random) {
         var paths = FreeAround(map, coord); 
		 if (paths.Count < 1 || amountOfRooms < 1) {
             return;
         }
            
         var amount = random.Next(paths.Count - 1) + 1; 
         //var amount = paths.Count;
         var nDistribution = Distribute(amountOfRooms, amount, random);
         
         
         for (var i = 0; i < nDistribution.Count; i++) { 
             var pCoord = paths[i];
             
             map[coord.X, coord.Y].Dirs.Add(pCoord.Item2); 
             var room = new Room(GetRandomRoom(random), new List<Dir> { Opposite(pCoord.Item2) }); 
             map[pCoord.Item1.X, pCoord.Item1.Y] = room;
             last = pCoord.Item1;
             FillRec(map, pCoord.Item1, nDistribution[i] - 1, random);
         }
     }

     private static RoomType GetRandomRoom(System.Random r) { 
    	 var rooms = new List<RoomType>{
        	RoomType.Enemy,
        	RoomType.Loot,
        	RoomType.Empty
         };
    	 int index = r.Next(rooms.Count);
    	 return rooms[index];
     }

    private static List<int> Distribute(int total, int amount, System.Random random) { 
        var distribution = new List<double>(); 
        double sum = 0; 
        for (var i = 0; i < amount; i++) { 
            var d = random.NextDouble(); 
            sum += d; 
            distribution.Add(d);
        }
            
        return distribution.Select(d => (int) Math.Round(d / sum * total)).ToList();
    }

    private static List<(Coord, Dir)> FreeAround(Room[,] rooms, Coord coord) { 
        var x = coord.X; 
        var y = coord.Y;
        
        var coords = new List<(Coord, Dir)> { 
            (new Coord(x - 1, y), Dir.Left), 
            (new Coord(x + 1, y), Dir.Right), 
            (new Coord(x, y - 1), Dir.Up), 
            (new Coord(x, y + 1), Dir.Down),
        };

        return coords.FindAll(c => InBounds(rooms, c.Item1) &&
                                   rooms[c.Item1.X, c.Item1.Y] == null);
    }
    
    private static bool InBounds<T>(T[,] arr, Coord coord) {
        return 0 <= coord.X && coord.X < arr.GetLength(0) && 
               0 <= coord.Y && coord.Y < arr.GetLength(1);
    }
    
    public enum Dir { 
        Left, 
        Right, 
        Up, 
        Down,
		Null
    }
        
    public enum RoomType {
		Starting,
        Empty, 
        Enemy, 
        Boss, 
        Loot
    }

    private static Dir Opposite(Dir d) { 
        return d switch { 
            Dir.Up => Dir.Down, 
            Dir.Down => Dir.Up, 
            Dir.Left => Dir.Right, 
            Dir.Right => Dir.Left, 
            _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
        };
    }

    public readonly struct Coord { 
        public readonly int X; 
        public readonly int Y;
        
        public Coord(int x, int y) { 
            X = x; 
            Y = y;
        }

        public override string ToString() { 
            return $"({X}, {Y})";
        }
    }

    public class Room { 
        public readonly RoomType Type; 
        public readonly List<Dir> Dirs;

		private PlayerController.Item item=PlayerController.Item.Null;
		private string weapon=null;
		private bool visited;
        
        public Room(RoomType type, List<Dir> dirs) { 
            Type = type; 
            Dirs = dirs;
			visited=false;
			if(type==RoomType.Loot){
				//while(!GenerateLoot()){}
			}
        }

        public string GetLoot(){
            if(item!=PlayerController.Item.Null) return "item";
			if(weapon!=null) return "weapon";
			return "null";
        }

        public PlayerController.Item TakeItem(){
            var itemVar = item;
            item = PlayerController.Item.Null;
            return itemVar;
        }

		public WeaponController TakeWeapon(){
            //var weaponVar = weapon;
            //weapon = null;
            return new WeaponController("",false,0,0);
        }

        public bool Visited(bool b){
			if(b){
				visited=true;
			}
			return visited;
		}

		private bool GenerateLoot(){
			Random rnd = new Random();
        	double r=rnd.Next(0,1);
			if(r<0.5){
				r=rnd.Next(0,3);
				if(r<1){
					if (Config.ItemAvailable(0)){
						item=PlayerController.Item.AmmoBox;
						return true;
					}
				}else if(r>2){
					if (Config.ItemAvailable(0)){
						item=PlayerController.Item.RepairKit;
						return true;
					}
				}else{
					if (Config.ItemAvailable(0)){
						item=PlayerController.Item.HealingPotion;
						return true;
					}
				}
			}else if(r>0.5){
				r=rnd.Next(0,8);
				if(r<1){
					if (Config.WeaponAvailable(0)){
						
						return true;
					}
				}else if(r<2){
					if (Config.WeaponAvailable(1)){
						
						return true;
					}
				}else if(r<3){
					if (Config.WeaponAvailable(2)){
						
						return true;
					}
				}else if(r<4){
					if (Config.WeaponAvailable(3)){
						
						return true;
					}
				}else if(r<5){
					if (Config.WeaponAvailable(4)){
						
						return true;
					}
				}else if(r<6){
					if (Config.WeaponAvailable(5)){
						
						return true;
					}
				}else if(r<7){
					if (Config.WeaponAvailable(6)){
						
						return true;
					}
				}else if(r<8){
					if (Config.WeaponAvailable(7)){
						
						return true;
					}
				}else{
					if (Config.WeaponAvailable(8)){
						
						return true;
					}
				}
			}
			return false;
		}
        
        public override string ToString() { 
            var l = Dirs.Contains(Dir.Left) ? "<" : " ";
            var r = Dirs.Contains(Dir.Right) ? ">" : " "; 
            var u = Dirs.Contains(Dir.Up) ? "^" : " "; 
            var d = Dirs.Contains(Dir.Down) ? "v" : " ";
            return $"{l}{u} {Type} {r}{d}";
        }
    }
}
