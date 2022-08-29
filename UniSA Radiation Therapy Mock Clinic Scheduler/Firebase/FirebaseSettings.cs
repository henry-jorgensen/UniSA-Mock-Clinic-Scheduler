using System;
using System.Text.Json.Serialization;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseSettings
    {
        [JsonPropertyName("type")]
        public string Type => "service_account";

        [JsonPropertyName("project_id")]
        public string ProjectId => "unisa-rt-mock-clinic";

        [JsonPropertyName("private_key_id")]
        public string PrivateKeyId => "6aa0d9f7dc80f52392dd906a85d0f4f462432f52";

        [JsonPropertyName("private_key")]
        public string PrivateKey => "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDP+Pn9NZkcvnxs\n7sTgbHBZJqlqK/wv5qVSdyQeUZuC4Dp1uFffRcGS2OSrq+v0CMgRxaBtF5ox4oca\nrk5+B0AqtDpuWLoKVOlAbzEjbdZgcV2WZccfxb1VqMMeNJRFF1g3O+idQIAzI0ax\nDH+RQoPaco6pc8LjfTG5NAfcyZQcSoYGyUgFVaMfmbay7DQwyvIsyY9uZ52ZPYWG\n3N2KZI69uKYVPKflgqOtzZuedAtDG2MMnyHe8Majjt/MtGFLKeI/hE+TpYvpn5c6\ncV33BpFyN7D1LP2pVCP83rtmCswWbZPfZL+Fpjnc/QANqzVVJb3SLIBWOmeEkuIi\n8dw60uaNAgMBAAECggEAAd04dD9Q9bibYYRSjXGkbj8ZCN65ihdVd/A9iaOvn2C8\nzcGV5wIhGjAq5qRrYe2CDIqO0NPcxESSc3J363iqH31vB8bJeVTBbL2EfsKsGx7Z\nth5858iRIxEeOsli0lKAl/eKlyTb82dOdN0dr29VU1An/hMxFJP5hvM8OcNSLxvD\nC8a+uiSWl0OBGnDNBulkmffZsdjiphPVooY/khLGpTxUN8ec7HZEhhlGvaPvvBPW\n6ywKxtjssblc22CkIkexVsnFqtf1ZkUSAKggnmv8aKImzKkkG+8PQcPiJNAp6FC9\nZ9bdXwrFDtrPU+V3gFCS4Z7yogU1fcaTM9wI0tjPoQKBgQD+poIAr4BYmVIaO86S\nSFifnurNfYOsTBEa8cn//i8W125LyV93Ee4ThCV4zgP9sT7HblnxT6U+mB+Hmkz7\nNeIfbYfL3RR+R7GFqkbYs8P5GPUP8FSsTMzyLvdMHF1TQz4G0Qbn+r1AgwXZcVdL\nuvNyE8MYKft2xAz8uCSKwL0ahQKBgQDREyOvKOKE0JadMLrE1wCaapUamHmLZiAO\nJsYzQjz/186FwKKyQyLAZSAbGdkzLK2U8ZSosoQZnrokeDxfmXoZniKqcFbuFIJi\ngyFhpfp6mK4GPhyD+bzsIoT3WX9Xwtrmv2OfudalY1hd6fNaapYomyYqTdoWUA88\nvxdLe/zOaQKBgQCaD2B9S7ApafC7AE3UQEKlpz5EvdfIiGic1YUxA7W3avRGk3jX\nD5jqY7tL38+YTwA9JWzyyg2d1ejVYCuMm6fG/bv3QTRhxbwHsuGTvwYkEM5KK0r+\nxqQDLRjeChcIBZlkBFfaRt7yRZJnX+PBZEReUshoORXyX1/AESPCciK2BQKBgD64\nCyBkl29YU5ZcI+sgxGGOT6Rm0S9sN3mHUDXYTQxC5QViwGvRj/8/Vt5KZsnfQUNJ\nJVtmEhLNdvGx0Aqts98zfRq8EJfjNynuRHlSnU1ht/LPdyZwKKh9wn2hL35YSeqm\nx3AHA8khgETMBeC90MXlpRFTwXSoF6oVeRt/2lrhAoGBAL8IsbiE9hKv7MUK1xR3\nrW194MfKocnFuAi+GdbhJjw6jG88/3l3nnVE+unz8ORP5Kj6m9XwISWrU9/H0KJ7\n/lJ8pafeBWsc8i+B+x2/vwZNjxGVzwpWzBLnHwjgsZ3qT1w2h3I7boAnn3Ezouwv\n5w2vKJXCIjteoyLxZ83Zao3w\n-----END PRIVATE KEY-----\n";

        [JsonPropertyName("client_email")]
        public string ClientEmail => "firebase-adminsdk-vjevh@unisa-rt-mock-clinic.iam.gserviceaccount.com";

        [JsonPropertyName("client_id")]
        public string ClientId => "103711456653250716580";

        [JsonPropertyName("auth_uri")]
        public string AuthUri => "https://accounts.google.com/o/oauth2/auth";

        [JsonPropertyName("token_uri")]
        public string TokenUri => "https://oauth2.googleapis.com/token";

        [JsonPropertyName("auth_provider_x509_cert_url")]
        public string AuthProviderx508CertUrl => "https://www.googleapis.com/oauth2/v1/certs";

        [JsonPropertyName("client_x509_cert_url")]
        public string Clientx509CertUrl => "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-vjevh%40unisa-rt-mock-clinic.iam.gserviceaccount.com";
    }
}

