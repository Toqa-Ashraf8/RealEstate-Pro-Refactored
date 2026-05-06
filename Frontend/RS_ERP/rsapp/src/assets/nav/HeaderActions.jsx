import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { LogOut } from 'lucide-react';
import { Key } from 'lucide-react';
import { Navigate, useNavigate } from 'react-router-dom';
import { logOut, resetUserForm } from '../redux/authSlice';
import { MdLogout } from "react-icons/md";
const HeaderActions = () => {
   const {token}=useSelector((state)=>state.auth);
  const navigate=useNavigate();
  const dispatch=useDispatch();

  if (!token) return null;

  const handleLogOut=()=>{
      dispatch(logOut());
      navigate('/login');
  }
  return (
    <div className="header-actions-container">
      <button 
        className="logout-btn-minimal"
        onClick={handleLogOut}
        title="تسجيل الخروج"
      >
        <span>خروج</span>
        <MdLogout />
      </button>
    </div>
  );
};

export default HeaderActions;