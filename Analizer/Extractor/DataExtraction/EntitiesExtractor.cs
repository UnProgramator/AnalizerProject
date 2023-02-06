﻿using DRSTool.Extractor.InternalModels;
using DRSTool.FileHelper.Implementation;

namespace DRSTool.Extractor.DataExtraction;

class EntitiesExtractor
{
    private static ConstructionModel initModel(int count)
    {
        return new ConstructionModel(count, new string[] { "" });
    }

    public static ConstructionModel CreateConstructionModel(string filePath)
    {
        ConstructionModel analizerModel;

        var input = CsvFileHelper.getInstance().getArrayContent<EntityModel>(filePath);

        if (input == null)
            throw new Exception("Error during reading struct file");

        input = input.Where(m => m.Extension.Equals("java")).OrderBy(m => m.PackageName).ThenBy(m => m.Filename);

        //fore some reason, count remove the elements from the IEnumerable implementation
        //because of that, I converted it to an array

        input = input.ToArray();

        Console.WriteLine(input.GetType());

        analizerModel = initModel(input.Count());

        foreach (var file in input)
        {
            var properties = new Dictionary<string, dynamic>() {
                { "path", file.RawPath},
                { "LOC", file.LinesOfCode },
                { "changes", file.Changes}
            };
            
            if (file.Module != null) properties.Add("module", file.Module);
            if (file.PackageName != null)  properties.Add("package", file.PackageName);

            analizerModel.addEntity(file.RawPath + "/" + file.Filename, properties);
        }

        return analizerModel;
    }
}
