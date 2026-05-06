import React from 'react';
import './RevertPaymentModal.css'; 
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'react-toastify';
import { 
    confirmRevertPayment, 
    toggleRevertModal 
} from '../../../assets/redux/bookingSlice';

const RevertPaymentModal = () => {
const dispatch = useDispatch();
    return (
        <div className="rv-modal-overlay">
            <div className="rv-modal-card">
                <p className="rv-modal-text">
                    هل أنت متأكد من إلغاء حالة الدفع؟ <br/>
                    <span style={{fontSize: '12px', fontWeight: '400', color: '#666'}}>
                        سيتم حذف صورة الشيك المرفقة.
                    </span>
                </p>
                <div className="rv-modal-buttons">
                    <button 
                        className="rv-btn-confirm"
                        onClick={()=>dispatch(confirmRevertPayment())}
                    >
                        تأكيد الإلغاء
                    </button>
                     <button 
                        className="rv-btn-cancel" 
                        onClick={()=>dispatch(toggleRevertModal(false))}
                    >
                        تراجع
                    </button>
                </div>
            </div>
        </div>
    );
};

export default RevertPaymentModal;