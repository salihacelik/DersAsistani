using System;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Utils;

namespace DersAsistani.Services
{
    public class AuthService
    {
        private readonly UserRepository _repo;

        public AuthService(UserRepository repo)
        {
            _repo = repo;
        }

        public Tuple<bool, string, User> Login(string username, string password)
        {
            if (!Validation.IsValidUsername(username))
                return Tuple.Create(false, "Kullanıcı adı geçersiz.", (User)null);
            if (!Validation.IsValidPassword(password))
                return Tuple.Create(false, "Şifre en az 6 karakter olmalı.", (User)null);

            var user = _repo.GetByUsername(username);
            if (user == null) return Tuple.Create(false, "Kullanıcı bulunamadı.", (User)null);

            var hash = PasswordHasher.Hash(password);
            if (user.PasswordHash != hash) return Tuple.Create(false, "Şifre hatalı.", (User)null);

            return Tuple.Create(true, "Giriş başarılı.", user);
        }

        public Tuple<bool, string> Register(string username, string password, string fullName)
        {
            if (!Validation.IsValidUsername(username)) return Tuple.Create(false, "Kullanıcı adı geçersiz.");
            if (!Validation.IsValidPassword(password)) return Tuple.Create(false, "Şifre en az 6 karakter olmalı.");
            if (!Validation.IsValidFullName(fullName)) return Tuple.Create(false, "Ad-soyad geçersiz.");

            if (_repo.Exists(username)) return Tuple.Create(false, "Kullanıcı adı zaten kullanımda.");

            var user = new User();
            user.Username = username.Trim();
            user.PasswordHash = PasswordHasher.Hash(password);
            user.FullName = fullName.Trim();
            user.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            var created = _repo.Create(user);
            return created ? Tuple.Create(true, "Kayıt başarılı.") : Tuple.Create(false, "Kayıt sırasında hata oluştu.");
        }
    }
}