import React from 'react';
import { motion } from 'framer-motion';
import { User, Mail, Lock, ShieldCheck, UserPlus } from 'lucide-react';
import './Register.css';
import { useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { setUserData } from '../../assets/redux/authSlice';
import { toast } from 'react-toastify';
import { registerUsers } from '../../services/authService';

const Register = () => {
  const { user } = useSelector((state) => state.auth);
  const { isLoading } = useSelector((state) => state.ui);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleInputValues = (e) => {
    const { name, value } = e.target;
    dispatch(setUserData({ [name]: value }));
  }

  const validatePasswordMatch = (e) => {
    if (user.Password !== e.target.value) {
      toast.error("كلمة السر غير متطابقة!", {
        theme: "colored",
        position: "top-left"
      });
    }
  }

  const handleRegister = async () => {
    try {
      const result = await dispatch(registerUsers(user)).unwrap();
      if (result.token) {
        toast.success("تم إنشاء الحساب بنجاح", {
          theme: "colored",
          position: "top-left",
        });
        navigate('/projects');
      } else if (result.isExisted) {
        toast.error("هذا البريد الإلكتروني موجود بالفعل", {
          theme: "colored",
          position: "top-left",
        });
      }
    } catch (error) {
      toast.error("حدث خطأ في الاتصال بالخادم!", {
        theme: "colored",
        position: "top-left"
      });
    }
  }

  return (
    <div className="auth-page-container">
      <motion.div 
        initial={{ opacity: 0, y: 30 }}
        animate={{ opacity: 1, y: 0 }}
        className="auth-wrapper"
      >
        <div className="auth-main-card">
          <div className="auth-header">
            <h2 className="auth-title">إنشاء حساب جديد</h2>
            <p style={{ color: '#64748b', fontWeight: 600 }}>
              قم بتعبئة البيانات للوصول إلى المنظومة
            </p>
          </div>

          <div className="auth-form">
            <div className="auth-form-row">
              <div className="auth-input-group">
                <label className="auth-label"><User size={18}/> اسم المستخدم</label>
                <input 
                  type="text" 
                  className="auth-control"
                  name='UserName'
                  value={user.UserName}
                  onChange={handleInputValues}
                />
              </div>

              <div className="auth-input-group">
                <label className="auth-label"><Mail size={18}/> البريد الالكتروني</label>
                <input 
                  type="email" 
                  className="auth-control"
                  name='Email'
                  value={user.Email}
                  onChange={handleInputValues}
                />
              </div>

              <div className="auth-input-group">
                <label className="auth-label"><Lock size={18}/> كلمة السر</label>
                <input 
                  type="password" 
                  className="auth-control"
                  name='Password'
                  value={user.Password}
                  onChange={handleInputValues}
                />
              </div>

              <div className="auth-input-group">
                <label className="auth-label"><ShieldCheck size={18}/> تأكيد كلمة السر</label>
                <input 
                  type="password" 
                  className="auth-control"
                  onBlur={(e) => validatePasswordMatch(e)}
                />
              </div>

              <div className="auth-input-group">
                <label className="auth-label"><UserPlus size={18}/> الصلاحية</label>
                <select 
                  className="auth-control"
                  name='Role'
                  value={user.Role}
                  onChange={handleInputValues}
                >
                  <option value="-1">-إختر-</option>
                  <option value="Employee">Employee</option>
                  <option value="Admin">Admin</option>
                </select>
              </div>
            </div> 

            <div className="button-container">
              <button 
                className="auth-submit-btn"
                disabled={isLoading}
                onClick={() => handleRegister()}
              >
                {isLoading ? 'جاري التسجيل...' : 'تسجيل الحساب في المنظومة'}
              </button>
            </div>
          </div>

          <div className="auth-footer">
            لديك حساب مفعل؟ 
            <span className="auth-link" onClick={() => navigate('/login')}>
               دخول النظام
            </span>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default Register;