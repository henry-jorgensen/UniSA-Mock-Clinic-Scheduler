using Firebase.Auth;
using Google.Cloud.Firestore;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseSharedFunctions
    {
        FirestoreDb db;
        FirebaseAuthProvider auth;

        public FirebaseSharedFunctions()
        {
            db = FirestoreDb.Create("unisa-rt-mock-clinic");
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyByVk8XwhbQGoeeqkcxr5vRJWhOjep5Ulc"));
        }

        public async Task<UserModel> GetUserModelAsync(string UserToken)
        {
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == UserToken)
                {
                    Console.WriteLine("User: {0}", document.Id);
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string FirstName = documentDictionary["FirstName"].ToString();
                    string LastName = documentDictionary["LastName"].ToString();
                    string CCCode = documentDictionary["CCCode"].ToString();

                    return new UserModel(UserToken, FirstName, LastName, CCCode);
                }
            }

            return null;
        }

        public async Task<UserModel> LoginAccountAsync(AccountModel accountModel)
        {
            var firebaseAuth = await auth.SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
            string token = firebaseAuth.User.LocalId;

            if (token != null)
            {
                CollectionReference usersRef = db.Collection("Users");
                QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Id == token)
                    {
                        Console.WriteLine("User: {0}", document.Id);
                        Dictionary<string, object> documentDictionary = document.ToDictionary();
                        string FirstName = documentDictionary["FirstName"].ToString();
                        string LastName = documentDictionary["LastName"].ToString();
                        string CCCode = documentDictionary["CCCode"].ToString();

                        return new UserModel(token, FirstName, LastName, CCCode);
                    }
                }
            }

            return null;
        }

        public async Task<UserModel> CreateAccountAsync(AccountModel accountModel)
        {
            await auth.CreateUserWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
            var firebaseAuth = await auth.SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);

            string token = firebaseAuth.User.LocalId;

            if (token != null)
            {
                //Insert this into cloud firestore database
                DocumentReference docRef = db.Collection("Users").Document(token);
                Dictionary<string, object> user = new Dictionary<string, object>
                    {
                        { "FirstName", accountModel.FirstName },
                        { "LastName", accountModel.LastName },
                        { "CCCode", accountModel.CCCode }
                    };
                await docRef.SetAsync(user);

                return new UserModel(token, accountModel.FirstName, accountModel.LastName, accountModel.CCCode);
            }

            return null;
        }

        /// <summary>
        /// Create a new class document with the provided name as the document id under the current users in firebase. 
        /// If there is no Classes it will create a new collection.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="classModel">An object contained all the required information for the new entry</param>
        /// <returns>A string representing the ID(name) of the new document</returns>
        public async Task<string?> CreateNewClassAsync(string token, ClassModel classModel)
        {
            if (token != null)
            {
                //Insert this into cloud firestore database
                DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(classModel.Name);
                Dictionary<string, object> classObject = new Dictionary<string, object>
                    {
                        { "Name", classModel.Name },
                        { "Study Period", classModel.StudyPeriod },
                        { "Semester", classModel.Semester },
                        { "Year", classModel.Year },
                        { "Students", classModel.Students },
                        { "Schedules", classModel.Schedules }
                    };

                await docRef.SetAsync(classObject);

                return classModel.Name;
            }

            return null;
        }

        /// <summary>
        ///
        ///
        /// </summary>
        public async Task<Array?> CollectAllClassAsync(string token)
        {
            if (token != null)
            {
                Query allClassesQuery = db.Collection("Users").Document(token).Collection("Classes");
                QuerySnapshot allClassesQuerySnapshot = await allClassesQuery.GetSnapshotAsync();

                var docs = allClassesQuerySnapshot.Documents;
                // Dictionary<string, Dictionary<string, object>> container = new Dictionary<string, Dictionary<string, object>>();

                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    // container.Add(documentSnapshot.Id, documentSnapshot.ToDictionary());
                    Console.WriteLine("Document data for {0} document:", documentSnapshot.Id);
                    Dictionary<string, object> city = documentSnapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                    }
                    Console.WriteLine("");
                }

                // return docs;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a saved classes information from firebase. The retrieved data depends on the provided user token
        /// and class name.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="className">A string representing the ID of the class to retrieve</param>
        /// <returns>A ClassModel object containing the information that had been saved previously</returns>
        public async Task<ClassModel?> CollectClassAsync(string token, string className)
        {
            if (token != null)
            {
                //Insert this into cloud firestore database
                DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(className);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    ClassModel classModel = snapshot.ConvertTo<ClassModel>();
                    Console.WriteLine("Name: {0}", classModel.Name);
                    Console.WriteLine("Study Period: {0}", classModel.StudyPeriod);
                    Console.WriteLine("Semester: {0}", classModel.Semester);
                    Console.WriteLine("Year: {0}", classModel.Year);

                    return classModel;
                }
            }

            return null;
        }

        public string testingFunction(string value)
        {
            string response = value + " testing";
            return response;
        }
    }
}