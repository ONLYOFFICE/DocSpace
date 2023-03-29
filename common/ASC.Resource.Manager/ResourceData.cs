// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Resource.Manager;

[Scope]
public class ResourceData
{
    private readonly IDbContextFactory<ResourceDbContext> _dbContextFactory;

    public ResourceData(IDbContextFactory<ResourceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /* public DateTime GetLastUpdate()
    {
        return DbContext.ResFiles.Max(r => r.LastUpdate);
    } */

    /* public List<ResCulture> GetListLanguages(int fileId, string title)
    {
        var sql = DbContext.ResCultures
            .Join(DbContext.ResData.DefaultIfEmpty(), r => r.Title, r => r.CultureTitle, (c, d) => new { data = d, culture = c })
            .Where(r => r.data.FileId == fileId)
            .Where(r => r.data.Title == title);

        var language = sql.Select(r => GetCultureFromDB(r.culture)).ToList();

        language.Remove(language.Find(p => p.Title == "Neutral"));

        return language;
    } */

    /* public Dictionary<ResCulture, List<string>> GetCulturesWithAuthors()
    {
        return
            DbContext.ResAuthorsLang
            .Join(DbContext.ResCultures, r => r.CultureTitle, r => r.Title, (al, rc) => new { cultures = rc, authorsLang = al })
            .Join(DbContext.Authors, r => r.authorsLang.AuthorLogin, r => r.Login, (a, al) => new { authors = al, a.cultures, a.authorsLang })
            .Where(r => r.authors.IsAdmin == false)
            .ToList()
            .GroupBy(r => new ResCulture { Title = r.cultures.Title, Value = r.cultures.Value }, r => r.authorsLang.AuthorLogin)
            .ToDictionary(r => r.Key, r => r.ToList());
    } */

    /* public void AddCulture(string cultureTitle, string name)
    {
        var culture = new ResCultures
        {
            Title = cultureTitle,
            Value = name
        };

        DbContext.ResCultures.Add(culture);
        DbContext.SaveChanges();
    } */

    public void AddResource(string cultureTitle, string resType, DateTime date, ResWord word, bool isConsole, string authorLogin, bool updateIfExist = true)
    {
        using var DbContext = _dbContextFactory.CreateDbContext();

        var resData = DbContext.ResData
            .Where(r => r.FileId == word.ResFile.FileID)
            .Where(r => r.CultureTitle == cultureTitle)
            .Where(r => r.Title == word.Title)
            .Select(r => r.TextValue)
            .FirstOrDefault();

        var resReserve = DbContext.ResReserve
            .Where(r => r.FileId == word.ResFile.FileID)
            .Where(r => r.CultureTitle == cultureTitle)
            .Where(r => r.Title == word.Title)
            .Select(r => r.TextValue)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(resData))
        {
            var newResData = new ResData
            {
                Title = word.Title,
                TextValue = word.ValueFrom,
                CultureTitle = cultureTitle,
                FileId = word.ResFile.FileID,
                ResourceType = resType,
                TimeChanges = date,
                Flag = 2,
                AuthorLogin = authorLogin
            };

            DbContext.ResData.Add(newResData);
            DbContext.SaveChanges();

            if (isConsole)
            {
                var newResReserve = new ResReserve
                {
                    Title = word.Title,
                    TextValue = word.ValueFrom,
                    CultureTitle = cultureTitle,
                    FileId = word.ResFile.FileID
                };

                DbContext.ResReserve.Add(newResReserve);
                DbContext.SaveChanges();
            }
        }
        else
        {
            if (cultureTitle == "Neutral" && isConsole)
            {
                updateIfExist =
                    DbContext.ResData
                    .Where(r => r.FileId == word.ResFile.FileID)
                    .Where(r => r.CultureTitle != cultureTitle)
                    .Where(r => r.Title == word.Title)
                    .Count() == 0;
            }

            var isChangeResData = resData != word.ValueFrom;
            var isChangeResReserve = resReserve != word.ValueFrom;

            if (!updateIfExist) return;

            if ((isConsole && isChangeResData && isChangeResReserve) || !isConsole)
            {
                if (cultureTitle == "Neutral")
                {
                    var forUpdate = DbContext.ResData
                        .Where(r => r.FileId == word.ResFile.FileID)
                        .Where(r => r.Title == word.Title)
                        .ToList();

                    foreach (var f in forUpdate)
                    {
                        f.Flag = 3;
                    }

                    DbContext.SaveChanges();
                }

                var newResData = new ResData
                {
                    Title = word.Title,
                    TextValue = word.ValueFrom,
                    CultureTitle = cultureTitle,
                    FileId = word.ResFile.FileID,
                    ResourceType = resType,
                    TimeChanges = date,
                    Flag = 2,
                    AuthorLogin = authorLogin
                };

                DbContext.ResData.Add(newResData);
                DbContext.SaveChanges();

                if (isConsole)
                {
                    var resReserveForUpdate = DbContext.ResReserve
                        .Where(r => r.FileId == word.ResFile.FileID)
                        .Where(r => r.Title == word.Title)
                        .Where(r => r.CultureTitle == cultureTitle)
                        .ToList();

                    foreach (var r in resReserveForUpdate)
                    {
                        r.Flag = 2;
                        r.TextValue = word.ValueFrom;
                    }

                    DbContext.SaveChanges();
                }
            }
            else if (isChangeResData)
            {
                var resReserveForUpdate = DbContext.ResReserve
                        .Where(r => r.FileId == word.ResFile.FileID)
                        .Where(r => r.Title == word.Title)
                        .Where(r => r.CultureTitle == cultureTitle)
                        .ToList();

                foreach (var r in resReserveForUpdate)
                {
                    r.Flag = 2;
                    r.TextValue = word.ValueFrom;
                }

                DbContext.SaveChanges();
            }
        }
    }

    /* public void EditEnglish(ResWord word)
    {
        var data = DbContext.ResData
            .Where(r => r.FileId == word.ResFile.FileID)
            .Where(r => r.Title == word.Title)
            .Where(r => r.CultureTitle == "Neutral");

        foreach (var d in data)
        {
            d.TextValue = word.ValueFrom;
        }

        DbContext.SaveChanges();
    } */

    /*  public void AddComment(ResWord word)
     {
         var data = DbContext.ResData
             .Where(r => r.Title == word.Title)
             .Where(r => r.FileId == word.ResFile.FileID)
             .Where(r => r.CultureTitle == "Neutral");

         foreach (var d in data)
         {
             d.Description = word.TextComment;
         }

         DbContext.SaveChanges();
     } */

    public int AddFile(string fileName, string projectName, string moduleName)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        if (fileNameWithoutExtension != null && fileNameWithoutExtension.Split('.').Length > 1)
        {
            fileName = fileNameWithoutExtension.Split('.')[0] + Path.GetExtension(fileName);
        }

        using var DbContext = _dbContextFactory.CreateDbContext();

        var count = DbContext.ResFiles
            .Where(r => r.ResName == fileName)
            .Where(r => r.ProjectName == projectName)
            .Where(r => r.ModuleName == moduleName)
            .Count();

        if (count == 0)
        {
            var file = new ResFiles
            {
                ResName = fileName,
                ProjectName = projectName,
                ModuleName = moduleName
            };

            DbContext.ResFiles.Add(file);
            DbContext.SaveChanges();
        }

        var files = DbContext.ResFiles.ToList();
        var lastUpdate = DateTime.UtcNow.AddHours(4);

        foreach (var f in files)
        {
            f.LastUpdate = lastUpdate;
        }

        DbContext.SaveChanges();

        return DbContext.ResFiles
            .Where(r => r.ResName == fileName)
            .Where(r => r.ProjectName == projectName)
            .Where(r => r.ModuleName == moduleName)
            .Select(r => r.Id)
            .FirstOrDefault();
    }


    public IEnumerable<ResCulture> GetCultures()
    {
        using var DbContext = _dbContextFactory.CreateDbContext();
        return DbContext.ResCultures
            .OrderBy(r => r.Title)
            .Select(GetCultureFromDB)
            .ToList();
    }

    /* public void SetCultureAvailable(string title)
    {
        var cultures = DbContext.ResCultures.Where(r => r.Title == title);

        foreach (var c in cultures)
        {
            c.Available = true;
        }

        DbContext.SaveChanges();
    }

    private static ResCulture GetCultureFromDB(IList<object> r)
    {
        return new ResCulture { Title = (string)r[0], Value = (string)r[1], Available = (bool)r[2] };
    }
                    */

    private static ResCulture GetCultureFromDB(ResCultures r)
    {
        return new ResCulture { Title = r.Title, Value = r.Value, Available = r.Available };
    }
    public List<ResFile> GetAllFiles()
    {
        using var DbContext = _dbContextFactory.CreateDbContext();

        return DbContext.ResFiles.Select(r => new ResFile
        {
            FileID = r.Id,
            ProjectName = r.ProjectName,
            ModuleName = r.ModuleName,
            FileName = r.ResName
        }).ToList();
    }

    public IEnumerable<ResWord> GetListResWords(ResCurrent current, string search)
    {
        using var DbContext = _dbContextFactory.CreateDbContext();

        IQueryable<TempResData> exist = DbContext.ResData
            .Where(r => r.Flag != 4)
            .Where(r => r.ResourceType == "text")
            .Where(r => r.CultureTitle == "Neutral")
            .Join(DbContext.ResData.DefaultIfEmpty(), r => new { r.FileId, r.Title }, r => new { r.FileId, r.Title }, (r1, r2) => new { r1, r2 })
            .Join(DbContext.ResFiles, r => r.r1.FileId, f => f.Id, (d, f) => new TempResData { R1 = d.r1, R2 = d.r2, RF = f })
            .Where(r => r.R2.CultureTitle == current.Language.Title)
            .Where(r => !r.R1.Title.StartsWith(@"del\_"))
            .Where(r =>
                !DbContext.ResData
                .Where(d => d.FileId == r.R1.FileId)
                .Where(d => d.Title == "del_" + r.R1.Title)
                .Where(d => d.CultureTitle == r.R1.CultureTitle)
                .Any()
            )
            .OrderBy(r => r.R1.Id);

        if (current.Module != null && !string.IsNullOrEmpty(current.Module.Name))
        {
            exist = exist.Where(r => r.RF.ModuleName == current.Module.Name);
        }

        if (current.Project != null && !string.IsNullOrEmpty(current.Project.Name))
        {
            exist = exist.Where(r => r.RF.ProjectName == current.Project.Name);
        }

        if (current.Word != null && current.Word.ResFile != null && !string.IsNullOrEmpty(current.Word.ResFile.FileName))
        {
            exist = exist.Where(r => r.RF.ResName == current.Word.ResFile.FileName);
        }

        if (!string.IsNullOrEmpty(search))
        {
            exist = exist.Where(r => r.R1.TextValue == search);
        }

        return exist.ToList().Select(r =>
        {
            var word = new ResWord
            {
                Title = r.R1.Title,
                ResFile = new ResFile
                {
                    FileID = r.R1.FileId
                },
                ValueFrom = r.R1.TextValue,
                TextComment = r.R1.Description,
                Flag = r.R1.Flag,
                Link = r.R1.Link
            };
            word.ResFile.FileName = r.RF.ResName;

            if (r.R2 != null)
            {
                word.Status = r.R2.Flag == 3 ? WordStatusEnum.Changed : WordStatusEnum.Translated;
                word.ValueTo = r.R2.TextValue;
            }
            else
            {
                word.Status = WordStatusEnum.Untranslated;
            }

            return word;
        }).OrderBy(r => r.ValueFrom);
    }

    /*public List<ResWord> GetListResWords(ResFile resFile, string to, string search)
    {
        var dbManager = GetDb();
        var sql = new SqlQuery(ResDataTable)
.Select("title", "fileid", "textValue", "description", "flag", "link")
.InnerJoin(ResFilesTable, Exp.EqColumns(ResFilesTable + ".ID", ResDataTable + ".fileID"))
.Where("moduleName", resFile.ModuleName)
.Where("projectName", resFile.ProjectName)
.Where("cultureTitle", to)
.Where("flag != 4")
.Where("resourceType", "text")
.OrderBy(ResDataTable + ".id", true);

        if (!string.IsNullOrEmpty(resFile.FileName))
            sql.Where("resName", resFile.FileName);

        if (!string.IsNullOrEmpty(search))
            sql.Where(Exp.Like("textvalue", search));

        return dbManager.ExecuteList(sql).ConvertAll(GetWord);
    }
    */
    /* public void GetListModules(ResCurrent currentData)
    {
        var dbManager = GetDb();
        var notExist = new SqlQuery(ResDataTable + " rd1")
.Select("1")
.Where("rd1.fileid = rd.fileid")
.Where("rd1.title = concat('del_', rd.title)")
.Where("rd1.cultureTitle = 'Neutral'");

        var exist = new SqlQuery(ResDataTable + " rd2")
            .Select("1")
            .Where("rd2.fileid = rd.fileid")
            .Where("rd2.title = rd.title")
            .Where("rd2.cultureTitle = 'Neutral'");

        var sql = new SqlQuery(ResFilesTable + " rf").Select("rf.moduleName",
                                                      string.Format("sum(case rd.cultureTitle when '{0}' then (case rd.flag when 3 then 0 else 1 end) else 0 end)", currentData.Language.Title),
                                                      string.Format("sum(case rd.cultureTitle when '{0}' then (case rd.flag when 3 then 1 else 0 end) else 0 end)", currentData.Language.Title),
                                                      string.Format("sum(case rd.cultureTitle when '{0}' then 1 else 0 end)", "Neutral"))
                                              .InnerJoin(ResDataTable + " rd", Exp.EqColumns("rd.fileid", "rf.id"))
                                              .Where("rf.projectName", currentData.Project.Name)
                                              .Where("rd.resourceType", "text")
                                              .Where(!Exp.Like("rd.title", @"del\_", SqlLike.StartWith) & Exp.Exists(exist) & !Exp.Exists(notExist))
                                              .GroupBy("moduleName");


        dbManager.ExecuteList(sql).ForEach(r =>
        {
            var module = currentData.Project.Modules.Find(mod => mod.Name == r[0].ToString());
            if (module == null) return;
            module.Counts[WordStatusEnum.Translated] = Convert.ToInt32(r[1]);
            module.Counts[WordStatusEnum.Changed] = Convert.ToInt32(r[2]);
            module.Counts[WordStatusEnum.All] = Convert.ToInt32(r[3]);
            module.Counts[WordStatusEnum.Untranslated] = module.Counts[WordStatusEnum.All] - module.Counts[WordStatusEnum.Changed] - module.Counts[WordStatusEnum.Translated];
        });
    }*/

    /* public void LockModules(string projectName, string modules)
    {
        var dbManager = GetDb();
        var sqlUpdate = new SqlUpdate(ResFilesTable);
        sqlUpdate.Set("isLock", 1).Where("projectName", projectName).Where(Exp.In("moduleName", modules.Split(',')));
        dbManager.ExecuteNonQuery(sqlUpdate);
    } */

    /* public void UnLockModules()
    {
        var dbManager = GetDb();
        var sqlUpdate = new SqlUpdate(ResFilesTable);
        sqlUpdate.Set("isLock", 0);
        dbManager.ExecuteNonQuery(sqlUpdate);
    } */

    /* public void AddLink(string resource, string fileName, string page)
    {
        var dbManager = GetDb();
        var query = new SqlQuery(ResDataTable);
        query.Select(ResDataTable + ".id")
             .InnerJoin(ResFilesTable, Exp.EqColumns(ResFilesTable + ".id", ResDataTable + ".fileid"))
             .Where(ResDataTable + ".title", resource).Where(ResFilesTable + ".resName", fileName).Where(ResDataTable + ".cultureTitle", "Neutral");

        var key = dbManager.ExecuteScalar<int>(query);

        var update = new SqlUpdate(ResDataTable);
        update.Set("link", page).Where("id", key);
        dbManager.ExecuteNonQuery(update);
    } */

    /* public void GetResWordByKey(ResWord word, string to)
    {
        var dbManager = GetDb();
        var sql = new SqlQuery(ResDataTable)
.Select("textvalue", "description", "link")
.Where("fileID", word.ResFile.FileID)
.Where("cultureTitle", "Neutral")
.Where("title", word.Title);

        dbManager.ExecuteList(sql).ForEach(r => GetValue(word, to, r));

        GetValueByKey(word, to);

        sql = new SqlQuery(ResDataTable + " as res1").Select("res1.textvalue").Distinct()
                                              .InnerJoin(ResDataTable + " as res2", Exp.EqColumns("res1.title", "res2.title") & Exp.EqColumns("res1.fileid", "res2.fileid"))
                                              .Where("res1.cultureTitle", to)
                                              .Where("res2.cultureTitle", "Neutral")
                                              .Where("res2.textvalue", word.ValueFrom);

        word.Alternative = new List<string>();
        dbManager.ExecuteList(sql).ForEach(r => word.Alternative.Add((string)r[0]));
        word.Alternative.Remove(word.ValueTo);

        sql = new SqlQuery(ResFilesTable)
            .Select("resname")
            .Where("id", word.ResFile.FileID);

        word.ResFile.FileName = dbManager.ExecuteScalar<string>(sql);
    } */

    public void GetValueByKey(ResWord word, string to)
    {
        using var DbContext = _dbContextFactory.CreateDbContext();

        var valueTo = DbContext.ResData
            .Where(r => r.FileId == word.ResFile.FileID)
            .Where(r => r.CultureTitle == to)
            .Where(r => r.Title == word.Title)
            .Select(r => r.TextValue)
            .FirstOrDefault();

        word.ValueTo = valueTo ?? "";
    }

    /* private void GetValue(ResWord word, string to, IList<object> r)
    {
        word.ValueFrom = (string)r[0] ?? "";
        word.TextComment = (string)r[1] ?? "";

        var langs = (ConfigurationManager.AppSettings["resources.com-lang"] ?? string.Empty).Split(';').ToList();
        var dom = langs.Exists(lang => lang == to) ? ".info" : ".com";

        word.Link = !string.IsNullOrEmpty((string)r[2]) ? string.Format("http://{0}-translator.teamlab{1}{2}", to, dom, r[2]) : "";
    } */

    /* public List<Author> GetListAuthors()
    {
        return DbContext.Authors.Select(r => new Author
        {
            Login = r.Login,
            Password = r.Password,
            IsAdmin = r.IsAdmin
        }).ToList();
    } */

    /* public Author GetAuthor(string login)
    {
        var dbManager = GetDb();
        var sql = new SqlQuery(ResAuthorsTable)
.Select("login", "password", "isAdmin")
.Where("login", login);

        var author = dbManager.ExecuteList(sql)
                              .ConvertAll(r => new Author
                              {
                                  Login = (string)r[0],
                                  Password = (string)r[1],
                                  IsAdmin = Convert.ToBoolean(r[2])
                              }).FirstOrDefault();

        if (author != null)
        {
            sql = new SqlQuery("res_cultures rc")
                .Select("rc.title", "rc.value", "rc.available")
                .InnerJoin(ResAuthorsLangTable + " ral", Exp.EqColumns("rc.title", "ral.cultureTitle"))
                .Where("ral.authorLogin", author.Login);

            author.Langs = dbManager.ExecuteList(sql).ConvertAll(GetCultureFromDB);

            sql = new SqlQuery(ResFilesTable + " rf")
                .Select("rf.projectName").Distinct()
                .InnerJoin(ResAuthorsFileTable + " raf", Exp.EqColumns("raf.fileid", "rf.id"))
                .Where("raf.authorlogin", login)
                .Where("rf.isLock", 0);

            var projects = dbManager.ExecuteList(sql).Select(r => new ResProject { Name = (string)r[0] }).ToList();

            foreach (var resProject in projects)
            {
                sql = new SqlQuery(ResFilesTable + " rf")
                    .Select("rf.moduleName").Distinct()
                    .InnerJoin(ResAuthorsFileTable + " raf", Exp.EqColumns("raf.fileid", "rf.id"))
                    .Where("rf.projectName", resProject.Name)
                    .Where("raf.authorlogin", login)
                    .Where("rf.isLock", 0);
                resProject.Modules = dbManager.ExecuteList(sql).Select(r => new ResModule { Name = (string)r[0] }).ToList();
            }

            author.Projects = projects;
        }

        return author;
    }*/

    /* public void CreateAuthor(Author author, IEnumerable<string> languages, string modules)
    {
        var dbManager = GetDb();
        var sqlInsert = new SqlInsert(ResAuthorsTable, true)
.InColumnValue("login", author.Login)
.InColumnValue("password", author.Password)
.InColumnValue("isAdmin", author.IsAdmin);

        dbManager.ExecuteNonQuery(sqlInsert);

        var delete = new SqlDelete(ResAuthorsLangTable).Where("authorLogin", author.Login);
        dbManager.ExecuteNonQuery(delete);

        delete = new SqlDelete(ResAuthorsFileTable).Where("authorLogin", author.Login);
        dbManager.ExecuteNonQuery(delete);

        foreach (var lang in languages)
        {
            sqlInsert = new SqlInsert(ResAuthorsLangTable, true)
                .InColumnValue("authorLogin", author.Login)
                .InColumnValue("cultureTitle", lang);

            dbManager.ExecuteNonQuery(sqlInsert);
        }

        var resFiles = GetAllFiles();
        //project1:module1-access1,module2-access2;project2:module3-access3,module4-access4
        foreach (var projectData in modules.Split(';').Select(project => project.Split(':')))
        {
            foreach (var mod in projectData[1].Split(','))
            {
                //var modData = mod.Split('-');
                var fileid = resFiles.Where(r => r.ModuleName == mod && r.ProjectName == projectData[0]).Select(r => r.FileID).FirstOrDefault();
                sqlInsert = new SqlInsert(ResAuthorsFileTable, true)
                    .InColumnValue("authorLogin", author.Login)
                    .InColumnValue("fileId", fileid); //.InColumnValue("writeAccess", Convert.ToBoolean(modData[1]));
                dbManager.ExecuteNonQuery(sqlInsert);
            }
        }
    }

    public List<ResWord> SearchAll(string projectName, string moduleName, string languageTo, string searchText, string searchType)
    {
        var q = DbContext.ResData
            .Where(r => r.CultureTitle == languageTo)
            .Where(r => r.Flag != 4)
            //.Where(r=> searchtype.containts(searchtext))
            .Where(r => r.ResourceType == "text")
            .OrderBy(r => r.TextValue)
            .Join(DbContext.ResFiles, r => r.FileId, r => r.Id, (a, b) => new { data = a, files = b });

        if (!string.IsNullOrEmpty(projectName) && projectName != "All")
        {
            q = q.Where(r => r.files.ProjectName == projectName);

            if (!string.IsNullOrEmpty(moduleName) && moduleName != "All")
            {
                q = q.Where(r => r.files.ModuleName == moduleName);
            }
        }

        return q.Select(r => new ResWord()
        {
            Title = r.data.Title,
            ValueFrom = r.data.TextValue,
            ResFile = new ResFile
            {
                FileID = r.files.Id,
                FileName = r.files.ResName,
                ModuleName = r.files.ModuleName,
                ProjectName = r.files.ProjectName
            }
        })
        .ToList();
    }

    public void UpdateHashTable(ref Hashtable table, DateTime date)
    {
        var dbManager = GetDb("tmresourceTrans");
        var sql = new SqlQuery(ResDataTable)
.Select(ResDataTable + ".textValue", ResDataTable + ".title", ResFilesTable + ".ResName", ResDataTable + ".cultureTitle")
.InnerJoin(ResFilesTable, Exp.EqColumns(ResFilesTable + ".id", ResDataTable + ".fileID"))
.Where(Exp.Ge("timechanges", date));

        var list = dbManager.ExecuteList(sql);

        foreach (var t in list)
        {
            var key = t[1] + t[2].ToString() + t[3];

            if (table.ContainsKey(key))
                table[key] = t[0];
            else
                table.Add(key, t[0]);
        }
    }
    */
}

class TempResData
{
    public ResData R1 { get; set; }
    public ResData R2 { get; set; }
    public ResFiles RF { get; set; }
}