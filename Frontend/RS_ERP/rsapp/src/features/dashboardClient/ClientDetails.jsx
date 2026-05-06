import React, { useState } from 'react';
import './ClientDetails.css'; 
import { 
    ArrowRight, 
    User, 
    CreditCard, 
    Home, 
    Calendar, 
    Phone, 
    MapPin, 
    Hash, 
    Image as ImageIcon,
    FileText,
    CheckCircle2,
    Clock
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { variables } from '../../assets/variables';

const ClientDetails = () => {
    const { bookingData, bookedUnitsData } = useSelector((state) => state.clientsProfile);
    const navigate = useNavigate();
    const [openUnitIndex, setOpenUnitIndex] = useState(null);

    return (
        <div className="details-page-container animate-fade">
            <div className="details-nav-header">
                <button className="back-link-btn" onClick={() => navigate('/clientpage')}>
                    <ArrowRight size={18} /> العودة لقاعدة البيانات
                </button>
                <div className="page-identity">
                    <FileText size={20} className="text-primary" />
                    <h2>الملف التحليلي للعميل</h2>
                </div>
            </div>

            <div className="info-card-main">
                <div className="card-section-title">
                    <User size={18} /> بيانات الهوية والاتصال
                </div>    
                {bookingData && bookingData.map((c, idx) => (
                    <div className="details-layout-grid" key={idx}>
                        <div className="data-fields-side">
                            <div className="inputs-modern-grid">
                                <div className="field-group">
                                    <label><Hash size={13}/> كود العميل</label>
                                    <div className="static-value">{c.ClientID}</div>
                                </div>
                                <div className="field-group">
                                    <label><User size={13}/> الاسم بالكامل</label>
                                    <div className="static-value bold">{c.ClientName}</div>
                                </div>
                                <div className="field-group">
                                    <label><FileText size={13}/> الرقم القومي</label>
                                    <div className="static-value">{c.NationalID}</div>
                                </div>
                                <div className="field-group">
                                    <label><Phone size={13}/> رقم الهاتف (1)</label>
                                    <div className="static-value">{c.PhoneNumber}</div>
                                </div>
                                <div className="field-group">
                                    <label><Phone size={13}/> رقم الهاتف (2)</label>
                                    <div className="static-value">{c.SecondaryPhone || '---'}</div>
                                </div>
                                <div className="field-group full-span">
                                    <label><MapPin size={13}/> عنوان المراسلات</label>
                                    <div className="static-value">{c.Address}</div>
                                </div>
                            </div>
                        </div>

                        <div className="id-preview-side">
                            <label className="preview-label">
                                <ImageIcon size={14} /> نسخة مستند الهوية
                            </label>
                            <div className="id-photo-frame">
                                <img 
                                    src={variables.NATIONAL_ID_IMAGES_URL + c.NationalIdImagePath} 
                                    alt="National ID" 
                                    className="img-fluid" 
                                />
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            <div className="info-card-main">
                <div className="card-section-title">
                    <Home size={18} /> الوحدات السكنية المحجوزة
                </div>
                
                <div className="responsive-table-holder">
                    <table className="units-data-table">
                        <thead>
                            <tr>
                                <th>الوحدة</th>
                                <th>المشروع</th>
                                <th>تاريخ التعاقد</th>
                                <th className="text-center">خطة السداد</th>
                            </tr>
                        </thead>
                        <tbody>
                            {bookedUnitsData && bookedUnitsData.map((unit, index) => (
                                <React.Fragment key={index}>
                                    <tr className={openUnitIndex === index ? 'row-active' : ''}>
                                        <td><span className="unit-tag">{unit.unitName}</span></td>
                                        <td><span className="project-text">{unit.ProjectName}</span></td>
                                        <td>{unit.BookingDate?.split('T')[0]}</td> 
                                        <td className="text-center">
                                            <button 
                                                className={`toggle-details-btn ${openUnitIndex === index ? 'active' : ''}`}
                                                onClick={() => setOpenUnitIndex(openUnitIndex === index ? null : index)}
                                            >
                                                <CreditCard size={14} /> 
                                                {openUnitIndex === index ? "إغلاق الأقساط" : "عرض الأقساط"}
                                            </button>
                                        </td>
                                    </tr>

                                    {openUnitIndex === index && (
                                        <tr className="installments-row-container">
                                            <td colSpan="4">
                                                <div className="installments-nested-card">
                                                    <h5 className="nested-header">
                                                        <Calendar size={15} /> دفعات السداد المستحقة
                                                    </h5>
                                                    <div className="mini-responsive-table">
                                                        <table className="installments-table">
                                                            <thead>
                                                                <tr>
                                                                    <th>#</th>
                                                                    <th>تاريخ الاستحقاق</th>
                                                                    <th>القيمة</th>
                                                                    <th>الحالة</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                {unit.Installments.map((inst, i) => (
                                                                    <tr key={i}>
                                                                        <td>{i + 1}</td>
                                                                        <td>{inst.DueDate.split('T')[0]}</td>
                                                                        <td className="amount-text">{inst.MonthlyAmount.toLocaleString()} ج.م</td>
                                                                        <td>
                                                                            <span className={`status-pill ${inst.Paid ? 'paid' : 'pending'}`}>
                                                                                {inst.Paid ? <CheckCircle2 size={12}/> : <Clock size={12}/>}
                                                                                {inst.Paid ? 'تم الدفع' : 'قيد الانتظار'}
                                                                            </span>
                                                                        </td>
                                                                    </tr>
                                                                ))}
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    )}
                                </React.Fragment>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div> 
        </div>
    );
};

export default ClientDetails;