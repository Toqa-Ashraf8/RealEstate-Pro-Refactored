import React, { useEffect, useState } from 'react';
import './BookingsReport.css';
import { variables } from '../variables';
import { Printer } from 'lucide-react';

const BookingsReport = React.forwardRef((ref) => {
const [parsedData, setParsedData] = useState(null);

    useEffect(() => {
        const savedData = localStorage.getItem('activeBookingClient');
        if (savedData) {
            setParsedData(JSON.parse(savedData));
        }
    }, []);

    const summarizedInstallments = parsedData?.installments?.slice(0, 5) || [];
    if (!parsedData) return <div className="p-5 text-center">جاري تحميل التقرير...</div>;
    return (
      <div className="report-view-container">
      
            <div className="report-scroll-wrapper">
                <div ref={ref} className="print-only-report">
                    <div className="report-header">
                        <div className="company-brand">
                            <h1>شركة العقارات للتطوير</h1>
                            <span>سجل حجز وحدة سكنية</span>
                        </div>
                        <div className="report-meta">
                            <p>كود الحجز: {parsedData?.BookingDetails?.BookingID}</p>
                            <p>التاريخ: {new Date().toLocaleDateString('ar-EG')}</p>
                        </div>
                    </div>

                    <div className="data-grid">
                        <div className="info-section">
                        
                            <div className="info-items-wrapper">
                                <div className="info-item"><label>اسم العميل:</label><span>{parsedData.InitialClientData?.ClientName}</span></div>
                                <div className="info-item"><label>الرقم القومي:</label><span>{parsedData.ClientExDetails?.NationalID || "---"}</span></div>
                                <div className="info-item"><label>المشروع:</label><span>{parsedData.InitialClientData?.ProjectName}</span></div>
                                <div className="info-item"><label>رقم الوحدة:</label><span>{parsedData.InitialClientData?.unitName}</span></div>
                                <div className="info-item"><label>إجمالي السعر:</label><span>{parsedData.InitialClientData?.NegotiationPrice?.toLocaleString()} ج.م</span></div>
                                <div className="info-item"><label>المقدم المدفوع:</label><span>{parsedData.BookingDetails?.ReservationAmount?.toLocaleString()} ج.م</span></div>
                            </div>
                        </div>
                        
                        <div className="id-card-section">
                            <label>صورة إثبات الشخصية</label>
                            <div>
                                {parsedData?.ClientExDetails?.NationalIdImagePath ?(
                                    <img src={variables.NATIONAL_ID_IMAGES_URL + parsedData.ClientExDetails.NationalIdImagePath} alt="ID Card" />
                                ) : (
                                    <p className="no-image">لا يوجد صورة مرفقة</p>
                                )}
                            </div>
                        </div>
                    </div>

                    <div className="table-section">
                        <h4 className="section-title">جدولة الأقساط (ملخص)</h4>
                        <table className="report-table">
                            <thead>
                                <tr>
                                    <th>م</th>
                                    <th>قيمة القسط</th>
                                    <th>تاريخ الاستحقاق</th>
                                    <th>الحالة</th>
                                </tr>
                            </thead>
                            <tbody>
                                {summarizedInstallments.map((inst, index) => (
                                    <tr key={index}>
                                        <td>{index + 1}</td>
                                        <td>{inst.MonthlyAmount?.toLocaleString()} ج.م</td>
                                        <td>{inst.DueDate.split('T')[0]}</td>
                                        <td>{inst.Paid ? "تم السداد" : "معلق"}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="report-footer">
                        <div className="sig-box">
                            <p>توقيع الموظف المختص</p>
                            <div className="signature-space"></div>
                        </div>
                        <div className="sig-box">
                            <p>توقيع العميل</p>
                            <div className="signature-space"></div>
                        </div>
                    </div>
                </div>
            </div>

            <button className="print-action-button" onClick={()=>window.print()}>
                <Printer size={18} />
                طباعة التقرير الرسمي
            </button>
        </div>
    );
});

export default BookingsReport;