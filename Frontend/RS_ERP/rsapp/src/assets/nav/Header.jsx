import React from 'react';
import './Header.css';
import { Link, useLocation } from 'react-router-dom';
import { useSelector } from 'react-redux';
import HeaderActions from './HeaderActions';

const Header = () => {
  const { token, userDetails } = useSelector((state) => state.auth);
  const location = useLocation(); // عشان نعرف الصفحة الحالية وننور اللينك بتاعها

  if (!token) return null;

  return (
    <div dir="rtl">
      {/* غيرنا navbar-light لـ navbar-dark عشان الكلام ينور أبيض */}
      <nav className="navbar navbar-expand-lg navbar-dark main-header-nav">
        <div className="container-fluid">
          
          {/* زرار الموبايل */}
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#navbarSupportedContent"
            aria-controls="navbarSupportedContent"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon" />
          </button>

          <div className="collapse navbar-collapse" id="navbarSupportedContent">
            {/* القائمة الرئيسية - ms-auto هي اللي بتزق كل حاجة لليمين في الـ RTL */}
            <ul className="navbar-nav ms-auto mb-2 mb-lg-0 p-0">
              {userDetails?.Role === "Admin" && (
                <li className="nav-item">
                  <Link className={`nav-link ${location.pathname === '/dashboard' ? 'active' : ''}`} to="/dashboard">
                    الرئيسية
                  </Link>
                </li>
              )}
              {userDetails?.Role === "Admin" && (
                <li className="nav-item">
                  <Link className={`nav-link ${location.pathname === '/addprojects' ? 'active' : ''}`} to="/addprojects">
                    إضافة المشاريع
                  </Link>
                </li>
              )}
              <li className="nav-item">
                <Link className={`nav-link ${location.pathname === '/projects' ? 'active' : ''}`} to="/projects">
                  المشاريع
                </Link>
              </li>
              <li className="nav-item">
                <Link className={`nav-link ${location.pathname === '/addclients' ? 'active' : ''}`} to="/addclients">
                  إضافة العملاء
                </Link>
              </li>
              {userDetails?.Role === "Admin" && (
                <li className="nav-item">
                  <Link className={`nav-link ${location.pathname === '/negotiation_requests' ? 'active' : ''}`} to="/negotiation_requests">
                    طلبات الشراء
                  </Link>
                </li>
              )}
              <li className="nav-item">
                <Link className={`nav-link ${location.pathname === '/booking' ? 'active' : ''}`} to="/booking">
                  إدارة الحجوزات
                </Link>
              </li>
              <li className="nav-item">
                <Link className={`nav-link ${location.pathname === '/clientpage' ? 'active' : ''}`} to="/clientpage">
                  العملاء
                </Link>
              </li>
            </ul>

            {/* الجزء الخاص بزرار الخروج - هيفضل أقصى اليسار */}
            <div className="header-actions-wrapper">
              {token && <HeaderActions />}
            </div>
          </div>
        </div>
      </nav>
    </div>
  );
};

export default Header;