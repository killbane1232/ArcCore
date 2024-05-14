using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;

namespace Arcam.Security
{
    public class AccessCheck
    {
        public static IQueryable<Strategy>? GetAllowedStrategies(ApplicationContext db, string token)
        {
            var user = GetUserFromToken(db, token);
            return GetAllowedStrategies(db, user);
        }

        public static IQueryable<Strategy>? GetAllowedStrategies(ApplicationContext db, User? user)
        {
            if (user == null)
                return null;
            IQueryable<Strategy> result = db.Strategy.Where(x=>x.Name != null);
            if (user.Access.MatrixParameters.Any(x => x.Param.Name == "admin" || x.Param.Name == "allow_all_strategies"))
                return result;
            if (user.Access.MatrixParameters.Any(x => x.Param.Name == "allow_public_strategies"))
                return result.Where(x => x.AuthorId == user.Id || x.IsPublic);
            return result.Where(x => x.AuthorId == user.Id);
        }
        public static IQueryable<Account>? GetAllowedAccounts(ApplicationContext db, string token)
        {
            var user = GetUserFromToken(db, token);
            return GetAllowedAccounts(db, user);
        }

        public static IQueryable<Account>? GetAllowedAccounts(ApplicationContext db, User? user)
        {
            if (user == null)
                return null;
            IQueryable<Account> result = db.Account.Where(x => x.Name != null);
            if (user.Access.MatrixParameters.Any(x => x.Param.Name == "admin"))
                return result;
            if (user.Access.MatrixParameters.Any(x => x.Param.Name == "allow_accounts"))
                return result.Where(x => x.UserId == user.Id);
            return null;
        }

        public static bool CheckAccessAccounts(ApplicationContext db, string token)
        {
            var user = GetUserFromToken(db, token);
            return CheckAccessAccounts(db, user);
        }

        public static bool CheckAccessAccounts(ApplicationContext db, User? user)
        {
            if (user == null)
                return false;
            if (user.Access.MatrixParameters.Any(x => x.Param.Name == "admin" || x.Param.Name == "allow_accounts"))
                return true;
            return false;
        }

        public static User? GetUserFromToken(ApplicationContext db, string token)
        {
            var tokens = db.Token.Where(x => x.Token == token).ToList();
            if (tokens.Count == 0)
                return null;
            var user = db.User.Where(x => x.Id == tokens[0].UserId).First();
            db.Entry(user).Reference(x => x.Access).Load();
            db.Entry(user.Access).Collection(x => x.MatrixParameters).Load();
            foreach (var each in user.Access.MatrixParameters)
                db.Entry(each).Reference(x => x.Param).Load();
            return user;
        }
    }
}
