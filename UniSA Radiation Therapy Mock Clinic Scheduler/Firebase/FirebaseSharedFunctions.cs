using Firebase.Auth;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using Google.Rpc;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseSharedFunctions
    {
        FirestoreDb db;
        FirebaseAuthProvider auth;

        public FirestoreDb DB()
        {
            return db;
        }
       
        public FirebaseAuthProvider Auth()
        {
            return auth;
        }

        public FirebaseSharedFunctions()
        {
            db = FirestoreDb.Create("unisa-rt-mock-clinic");
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyByVk8XwhbQGoeeqkcxr5vRJWhOjep5Ulc"));
        }

        public async Task<Boolean> VerifyLoggedIn(string UserToken)
        {
            if (UserToken == null) return false;

            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                return document.Id == UserToken;
            }

            return false;
        }

        public async Task<Boolean> VerifyCoordinatorCode(string code)
        {
            CollectionReference usersRef = db.Collection("CoordinatorCodes");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                return document.Id == code;
            }

            return false;
        }

        public async Task<Boolean> LoggedInAsCoordinator(string UserToken)
        {
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == UserToken)
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string CCCode = documentDictionary["CCCode"].ToString();

                    return VerifyCoordinatorCode(CCCode).Result;
                }
            }

            return false;
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

            Debug.WriteLine(token);
            if (token != null)
            {
                Debug.WriteLine("hit");
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
            //Insert this into cloud firestore database
            if (token != null)
            {
                DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(classModel.Name);
                await docRef.SetAsync(classModel);
                return classModel.Name;
            }

            return null;
        }

        /// <summary>
        /// Collect all class details, excluding the student and schedules lists that belong to a
        /// particular course coordinator.
        /// <param name="token">A string representing the current user signed in</param>
        /// </summary>
        public async Task<List<ClassModel>?> CollectAllClassAsync(string token)
        {
            if (token != null)
            {
                Query allClassesQuery = db.Collection("Users").Document(token).Collection("Classes");
                QuerySnapshot allClassesQuerySnapshot = await allClassesQuery.GetSnapshotAsync();
                List<ClassModel> classes = new List<ClassModel>();

                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentClass = documentSnapshot.ConvertTo<ClassModel>();
                    classes.Add(currentClass);
                }

                return classes;
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
                //Collect the class details
                DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(className);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists) return null;

                var classDetails = snapshot.ConvertTo<ClassModel>();

                //Retrieve the list of students
                Query colRef = db.Collection("Users").Document(token).Collection("Classes").Document(className).Collection("Students");
                QuerySnapshot allClassesQuerySnapshot = await colRef.GetSnapshotAsync();
                List<string> students = new List<string>();

                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                    students.Add(JsonConvert.SerializeObject(currentStudent));
                }

                //Join the students with a unique character as to easily separate them in javascript
                classDetails.Students = string.Join("|", students.ToArray());

                return classDetails;
            }

            return null;
        }

        /// <summary>
        /// Save a new class string to under the currently selected class.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="className">A string representing the ID of the class to save to</param>
        /// <param name="studentList">A string of student values, each value separated by a ':' and each student
        ///                             separated by a ','
        /// <returns>A boolean representing if the operation was a success</returns>
        public async Task<bool> SaveAClassListAsync(string token, string className, string[] studentList)
        {
            if (token != null)
            {
                WriteBatch batch = db.StartBatch();

                foreach (string student in studentList)
                {
                    var studentObject = JsonConvert.DeserializeObject<StudentModel>(student);

                    if (studentObject == null) return false;

                    //Insert the student list into the selected class
                    DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(className).Collection("Students").Document(studentObject.StudentId);
                    batch.Set(docRef, studentObject);
                }

                await batch.CommitAsync();

                return true;
            }

            return false;
        }

        public async void DataRequest(string token, string type)
        {
            try
            {
                UserModel user = GetUserModelAsync(token).Result;

                CollectionReference RequestsRef = db.Collection("DataRequests");
                QuerySnapshot snapshot = await RequestsRef.GetSnapshotAsync();


                DocumentReference docRef = db.Collection("DataRequests").Document();
                Dictionary<string, object> dataRequest = new Dictionary<string, object>
                    {
                        { "UID", token },
                        { "Name",  user.FirstName + " " + user.LastName},
                        { "Type", type },
                        { "Date", DateTime.Now.ToString() }
                    };
                await docRef.SetAsync(dataRequest);
            } catch(Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async void DeleteUserData(string token)
        {
            try
            {
                DocumentReference userRef = db.Collection("Users").Document(token);
                await userRef.DeleteAsync();
                
            } catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async void ResetPassword(string email)
        {
            try
            {
                await auth.SendPasswordResetEmailAsync(email);
            } catch(Exception e)
            {
                Debug.WriteLine(e);
            }           
        }
    }
}