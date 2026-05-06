import React from 'react';
import './BookingsReport.css';
import { variables } from '../variables';
import { Printer } from 'lucide-react';

const BookingsReport = React.forwardRef(({ client, installments}, ref) => {
    if (!client) return null;
    const summarizedInstallments = installments?.slice(0, 5) || [];
    
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
                            <p>كود الحجز: {client.BookingID}</p>
                            <p>التاريخ: {new Date().toLocaleDateString('ar-EG')}</p>
                        </div>
                    </div>

                    <div className="data-grid">
                        <div className="info-section">
                        
                            <div className="info-items-wrapper">
                                <div className="info-item"><label>اسم العميل:</label><span>{client.ClientName}</span></div>
                                <div className="info-item"><label>الرقم القومي:</label><span>{client.NationalID || "---"}</span></div>
                                <div className="info-item"><label>المشروع:</label><span>{client.ProjectName}</span></div>
                                <div className="info-item"><label>رقم الوحدة:</label><span>{client.unitName}</span></div>
                                <div className="info-item"><label>إجمالي السعر:</label><span>{client.NegotiationPrice?.toLocaleString()} ج.م</span></div>
                                <div className="info-item"><label>المقدم المدفوع:</label><span>{client.ReservationAmount?.toLocaleString()} ج.م</span></div>
                            </div>
                        </div>
                        
                        <div className="id-card-section">
                            <label>صورة إثبات الشخصية</label>
                            <div>
                                {client.NationalIdImagePath ? (
                                    <img src={variables.NATIONAL_ID_IMAGES_URL + client.NationalIdImagePath} alt="ID Card" />
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