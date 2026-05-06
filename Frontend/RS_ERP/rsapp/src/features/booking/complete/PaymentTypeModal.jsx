import React, { useEffect, useRef } from "react";
import "./PaymentTypeModal.css";
import { X, CreditCard, UploadCloud, CheckCircle, Image as ImageIcon,Building2,Ungroup } from 'lucide-react';
import { useDispatch, useSelector } from "react-redux";
import { CiImageOn } from "react-icons/ci";
import { 
  confirmpaidStatus, 
  setPaymentModalValues, 
  togglePaymentModal 
} from "../../../assets/redux/bookingSlice";
import { variables } from "../../../assets/variables";
import { saveInstallmentCheck } from "../../../services/bookingService";
const PaymentTypeModal = () => {
const {
  paymentType,
  installmentCheckImageName
} = useSelector((state) => state.booking);
const dispatch = useDispatch();

const handleChange=(e)=>{
  const {name,value}=e.target;
  dispatch(setPaymentModalValues({[name]:value}));
}
const handleChangeImage=async(e)=>{
   const { name } = e.target;
    if (!e.target.files || e.target.files.length === 0) return; 
       const file = e.target.files[0];
       const formData = new FormData();
       const fileName = file.name;
        formData.append("checkfile", file, fileName);    
          await  dispatch(saveInstallmentCheck(formData));
          await  dispatch(setPaymentModalValues({[name]:fileName}));         
}

const confirmReversal=()=>{
  dispatch(confirmpaidStatus());
}
console.log("PaymentType",paymentType)
  return (
   <div className="payment_modal_overlay" dir="rtl">
  <div className="payment_modal_card">
    <header className="payment_modal_header">
      <div className="header_title_section">
        <CreditCard size={20} />
        <h2>تفاصيل عملية الدفع</h2>
      </div>
      <button 
        className="close_x_btn"
        onClick={() => dispatch(togglePaymentModal(false))}
      >
        <X size={24} />
      </button>
    </header>

    <div className="payment_modal_body">
      <div className="modal_grid_layout">
        <div className="inputs_side">
          <div className="modern_input_group">
            <label>طريقة الدفع</label>
            <div className="select_wrapper">
              <select 
                name='PaymentType'
                value={paymentType?.PaymentType || ""}
                onChange={handleChange}
              >
                <option value="-1">إختر الطريقة</option>
                <option value="كاش">دفع نقدي (كاش)</option>
                <option value="شيك">شيك بنكي</option>
              </select>
            </div>
          </div>

          <div className="modern_input_group">
            <label>مرفق صورة الشيك</label>
            <div className="modern_file_upload">
              <input 
                type="file" 
                id="check-file"
                name="CheckImage"
                onChange={handleChangeImage}
              />
              <label htmlFor="check-file" className="file_upload_label">
                <UploadCloud size={20} />
                <span>{installmentCheckImageName ? "تغيير الصورة" : "إضغط لرفع صورة الشيك"}</span>
              </label>
            </div>
          </div>
        </div>

        <div className="preview_side">
          <div className="image_preview_box">
            {(installmentCheckImageName || paymentType?.CheckImage) ? (
              <img 
                src={`${variables.INSTALLMENT_CHECKS_IMAGES_URL}/${installmentCheckImageName || paymentType?.CheckImage}`}
                alt="Check Preview" 
              />
            ) : (
              <div className="preview_placeholder">
                <ImageIcon size={48} />
                <p>معاينة مستند الدفع</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>

    <footer className="payment_modal_footer">
    
      <button className="btn btn-danger" onClick={() => dispatch(togglePaymentModal(false))}>
        إلغاء
      </button>
       <button className="confirm_btn" onClick={() => confirmReversal()}>
        <CheckCircle size={18} />
        تأكيد
      </button> 
    </footer>
  </div>
</div>
  );
};

export default PaymentTypeModal;
