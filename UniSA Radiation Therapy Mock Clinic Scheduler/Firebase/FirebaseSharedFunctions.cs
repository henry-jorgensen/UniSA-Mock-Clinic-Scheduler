using Firebase.Auth;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using Google.Rpc;
using System.Linq;

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
            //TODO hide this in the future with ENV
            db = new FirestoreDbBuilder
            {
                ProjectId = "unisa-rt-mock-clinic",
                JsonCredentials = "{ 'type': 'service_account',   'project_id': 'unisa-rt-mock-clinic',   'private_key_id': '6aa0d9f7dc80f52392dd906a85d0f4f462432f52',   'private_key': '-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDP+Pn9NZkcvnxs\n7sTgbHBZJqlqK/wv5qVSdyQeUZuC4Dp1uFffRcGS2OSrq+v0CMgRxaBtF5ox4oca\nrk5+B0AqtDpuWLoKVOlAbzEjbdZgcV2WZccfxb1VqMMeNJRFF1g3O+idQIAzI0ax\nDH+RQoPaco6pc8LjfTG5NAfcyZQcSoYGyUgFVaMfmbay7DQwyvIsyY9uZ52ZPYWG\n3N2KZI69uKYVPKflgqOtzZuedAtDG2MMnyHe8Majjt/MtGFLKeI/hE+TpYvpn5c6\ncV33BpFyN7D1LP2pVCP83rtmCswWbZPfZL+Fpjnc/QANqzVVJb3SLIBWOmeEkuIi\n8dw60uaNAgMBAAECggEAAd04dD9Q9bibYYRSjXGkbj8ZCN65ihdVd/A9iaOvn2C8\nzcGV5wIhGjAq5qRrYe2CDIqO0NPcxESSc3J363iqH31vB8bJeVTBbL2EfsKsGx7Z\nth5858iRIxEeOsli0lKAl/eKlyTb82dOdN0dr29VU1An/hMxFJP5hvM8OcNSLxvD\nC8a+uiSWl0OBGnDNBulkmffZsdjiphPVooY/khLGpTxUN8ec7HZEhhlGvaPvvBPW\n6ywKxtjssblc22CkIkexVsnFqtf1ZkUSAKggnmv8aKImzKkkG+8PQcPiJNAp6FC9\nZ9bdXwrFDtrPU+V3gFCS4Z7yogU1fcaTM9wI0tjPoQKBgQD+poIAr4BYmVIaO86S\nSFifnurNfYOsTBEa8cn//i8W125LyV93Ee4ThCV4zgP9sT7HblnxT6U+mB+Hmkz7\nNeIfbYfL3RR+R7GFqkbYs8P5GPUP8FSsTMzyLvdMHF1TQz4G0Qbn+r1AgwXZcVdL\nuvNyE8MYKft2xAz8uCSKwL0ahQKBgQDREyOvKOKE0JadMLrE1wCaapUamHmLZiAO\nJsYzQjz/186FwKKyQyLAZSAbGdkzLK2U8ZSosoQZnrokeDxfmXoZniKqcFbuFIJi\ngyFhpfp6mK4GPhyD+bzsIoT3WX9Xwtrmv2OfudalY1hd6fNaapYomyYqTdoWUA88\nvxdLe/zOaQKBgQCaD2B9S7ApafC7AE3UQEKlpz5EvdfIiGic1YUxA7W3avRGk3jX\nD5jqY7tL38+YTwA9JWzyyg2d1ejVYCuMm6fG/bv3QTRhxbwHsuGTvwYkEM5KK0r+\nxqQDLRjeChcIBZlkBFfaRt7yRZJnX+PBZEReUshoORXyX1/AESPCciK2BQKBgD64\nCyBkl29YU5ZcI+sgxGGOT6Rm0S9sN3mHUDXYTQxC5QViwGvRj/8/Vt5KZsnfQUNJ\nJVtmEhLNdvGx0Aqts98zfRq8EJfjNynuRHlSnU1ht/LPdyZwKKh9wn2hL35YSeqm\nx3AHA8khgETMBeC90MXlpRFTwXSoF6oVeRt/2lrhAoGBAL8IsbiE9hKv7MUK1xR3\nrW194MfKocnFuAi+GdbhJjw6jG88/3l3nnVE+unz8ORP5Kj6m9XwISWrU9/H0KJ7\n/lJ8pafeBWsc8i+B+x2/vwZNjxGVzwpWzBLnHwjgsZ3qT1w2h3I7boAnn3Ezouwv\n5w2vKJXCIjteoyLxZ83Zao3w\n-----END PRIVATE KEY-----\n',   'client_email': 'firebase-adminsdk-vjevh@unisa-rt-mock-clinic.iam.gserviceaccount.com',   'client_id': '103711456653250716580',   'auth_uri': 'https://accounts.google.com/o/oauth2/auth',   'token_uri': 'https://oauth2.googleapis.com/token',   'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs',   'client_x509_cert_url': 'https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-vjevh%40unisa-rt-mock-clinic.iam.gserviceaccount.com' }"
            }.Build();

            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyByVk8XwhbQGoeeqkcxr5vRJWhOjep5Ulc"));
        }

        public async Task<Boolean> VerifyLoggedIn(string UserToken)
        {
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == UserToken) return true;
            }

            return false;
        }

        public async Task<Boolean> VerifyCoordinatorCode(string code)
        {
            CollectionReference usersRef = db.Collection("CoordinatorCodes");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == code) return true;
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
                CollectionReference usersRef = db.Collection("Users");
                QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Id == token)
                    {
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

        public async Task<List<AppointmentModel>?> CollectAllAppointmentsAsync(string token)
        {
            if (token != null)
            {
                Query allAppointmentsQuery = db.Collection("Appointments");
                QuerySnapshot allAppointmentsQuerySnapshot = await allAppointmentsQuery.GetSnapshotAsync();
                List<AppointmentModel> appointments = new List<AppointmentModel>();

                foreach (DocumentSnapshot documentSnapshot in allAppointmentsQuerySnapshot.Documents)
                {
                    var currentAppointment = documentSnapshot.ConvertTo<AppointmentModel>();
                    UserModel userPatient = await GetUserModelAsync(currentAppointment.Patient);
                    currentAppointment.Patient = userPatient.FirstName + " " + userPatient.LastName;

                    UserModel userRT1 = await GetUserModelAsync(currentAppointment.RadiationTherapist1);
                    currentAppointment.RadiationTherapist1 = userRT1.FirstName + " " + userRT1.LastName;

                    UserModel userRT2 = await GetUserModelAsync(currentAppointment.RadiationTherapist2);
                    currentAppointment.RadiationTherapist2 = userRT2.FirstName + " " + userRT2.LastName;

                    appointments.Add(currentAppointment);
                }

                return appointments;
            }

            return null;

        }

        public async void DataRequest(string token, string type)
        {
            try
            {
                UserModel user = GetUserModelAsync(token).Result;

                CollectionReference RequestsRef = db.Collection("DataRequests");
                QuerySnapshot snapshot = await RequestsRef.GetSnapshotAsync();

                bool found = false;
                
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string typeDict = documentDictionary["Type"].ToString();
                    string uid = documentDictionary["UID"].ToString();
                    if (uid == token && typeDict == type)
                    {
                        found = true;
                        Debug.WriteLine("ALREADY EXIST");
                    } 
                    
                }
                if (found == false)
                {
                    Debug.WriteLine("Hit 2");
                    DocumentReference docRef = db.Collection("DataRequests").Document();
                    Dictionary<string, object> dataRequest = new Dictionary<string, object>
                        {
                            { "UID", token },
                            { "Name",  user.FirstName + " " + user.LastName},
                            { "Type", type },
                            { "Date", DateTime.Now.ToString() }
                        };
                    await docRef.SetAsync(dataRequest);
                }

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