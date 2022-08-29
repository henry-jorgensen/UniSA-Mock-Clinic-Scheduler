using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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