using System;

public class Grid<T> {
    public int Width;
    public int Height;
    T[,] Mapping;

    private void check_bounds(int x, int y, string function){
      //Bounds checking
        if(x < 0 || x >= Width)
            throw new Exception($"Illegal grid access for Grid.{function}({x}, {y}) with max width {Width}");
        if(y < 0 || y >= Height)
            throw new Exception($"Illegal grid access for Grid.{function}({x}, {y}) with max height {Height}");
    }

    public T[] Get_Neighbours (int x, int y){
        check_bounds(x, y, "Get_Neighbours");
        T north = (y < Height) ?    Mapping[x, y+1] : default(T); 
        T south = (y > 0) ?         Mapping[x, y-1] : default(T); 
        T east = (x < Width) ?      Mapping[x+1, y] : default(T); 
        T west = (x > 0) ?          Mapping[x-1, y] : default(T); 
        return new T[4] {north, east, south, west};
    }

    public T Get(int x, int y){
        //Bounds checking
        check_bounds(x, y, "Get");
        return Mapping[x, y];
    }

    public void Set(int x, int y, T replacement){
        //Bounds checking
        check_bounds(x, y, "Set");
        Mapping[x, y] = replacement;;
    }

    public void ForEach( Action<T> function){
        foreach(T instance in Mapping){
            function.Invoke(instance);
        }
    }

    public void ForEach( Action<int, int> function){
        for(int i = 0; i < Width; i++){
            for (int j = 0; j < Height; j++)
            {
                function.Invoke(i, j);
            }
        }
    }

    public Grid (int width, int height){
        this.Width      = width;
        this.Height     = height;
        Mapping = new T[width, height];
    }
}
