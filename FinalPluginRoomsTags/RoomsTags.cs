
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPluginRoomsTags
{
    //[Transaction(TransactionMode.Manual)]
    public class RoomsTags : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //доступ к документу Revit
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //фильтр по помещениям
            var rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
            .Select(r => r as Room)  //выбор  помещения
            .GroupBy(x => x.LevelId);  //группировка по помещениям

            //добавляем помещения в коллекцию
            //var roomids = rooms.ToElementIds() as IList<ElementId>;
            ////фильтр по видам
            //List<ViewPlan> listView = new FilteredElementCollector(doc)
            //    .OfClass(typeof(ViewPlan))
            //    .OfType<ViewPlan>()
            //    .ToList();
            Transaction ts = new Transaction(doc, "Создание нумерации помещений");
            ts.Start();
            foreach (var Levelid in rooms)
            {
                string levelName = doc.GetElement(Levelid.Key).Name;
                levelName = levelName.Replace("Этаж ", "");
                int Num = 1;
                foreach (var item in Levelid)
                {

                    item.LookupParameter("Номер").Set(levelName + "_" + Num.ToString()); 
                    Num++;
                }

            }
            ts.Commit();
            //    Transaction ts = new Transaction(doc, "Создание нумерации помещений");
            //    ts.Start();
            //    foreach (View view in listView)
            //    {
            //        foreach (var roomid in roomids)
            //        {
            //            Element element = doc.GetElement(roomid);
            //            Room room = element as Room;
            //            UV tagCenter = GetRoomCenter(room);

            //            //создание новой марки помещения
            //            doc.Create.NewRoomTag(new LinkElementId(roomid), tagCenter, view.Id);

            //        }
            //    }
            //    ts.Commit();
            return Result.Succeeded;
            //}

            ////метод возвращает центр элемента на основе его BoundingBox.
            //public XYZ GetElementCenter(Element element)
            //{
            //    BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            //    //центральная точка
            //    XYZ center = (bounding.Max + bounding.Min) * 0.5;
            //    return center;
            //}
            ////метод возвращает центр помещеия в двухмерном пространстве
            //public UV GetRoomCenter(Room room)
            //{
            //    // определяем точку в центре комнаты
            //    XYZ boundCenter = GetElementCenter(room);
            //    //преобразуем в двухмерное пространство
            //    UV roomCenter = new UV(boundCenter.X, boundCenter.Y);
            //    return roomCenter;
            //}
        }
    }
}


