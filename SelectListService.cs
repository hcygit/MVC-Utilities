using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using WMNET;

namespace NT.Web.App.Services
{

    public interface ISelectListService
    {
        IDictionary<string, string> GetSelectListDict(string code, bool firstOption = false);

        string CreateNewFileUploadWord(string word);
    }

    public class SelectListService : ISelectListService
    {
        private readonly string _connStr;
        public SelectListService()
        {
            _connStr = new Sqlconfig(AgencyConfig.Instance.DbKey).connStr;
        }

        public IDictionary<string, string> GetSelectListDict(string code, bool firstOption)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            using (var sql = new SqlConnection(_connStr))
            {
                var query = @"select data_value, chin_wd from dbo.trword where cls_cd=@code order by sort_sq";
                dynamic rows = sql.Query<dynamic>(query, new { code });

                if (rows.Count == 0)
                {
                    sql.Execute(@"insert into trword (chin_wd, cls_cd, data_value, sort_sq, word_rk) values ('國內-飯店', 'UP_FILE', '1', 0, '新國旅檔案類型參照'), ('國內-餐廳', 'UP_FILE', '2', 1, '新國旅檔案類型參照')");
                    rows = sql.Query<dynamic>(query, new { code });
                }

                if (firstOption)
                {
                    dict.Add("0", "不限");
                }

                foreach (var row in rows)
                {
                    dict.Add(row.data_value, row.chin_wd);
                }

                return dict;
            }
        }

        public string CreateNewFileUploadWord(string word)
        {
            using (var sql = new SqlConnection(_connStr))
            {
                var currentWord = sql.Query<string>("select top 1 chin_wd from dbo.trword where cls_cd='UP_FILE' and chin_wd like '%' + @word + '%'", new { word }).FirstOrDefault();
                if (!string.IsNullOrEmpty(currentWord))
                {
                    throw new Exception($"分類[{word}]已經存在");
                }

                var lastestWord = sql.Query<dynamic>("select top 1 data_value, sort_sq from dbo.trword where cls_cd='UP_FILE' order by sort_sq desc").FirstOrDefault();

                var newCode = (Convert.ToInt32(lastestWord.data_value) + 1).ToString();

                sql.Execute("insert into dbo.trword (chin_wd, cls_cd, data_value, sort_sq, word_rk) values (@word, @code, @value, @sq, @remark)", new
                {
                    word = word, code = "UP_FILE", value = newCode, sq = Convert.ToInt16(lastestWord.sort_sq + 1), remark = "新國旅檔案類型參照"
                });

                return newCode;
            }
        }
    }
}
