import React, { useRef, useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { 
    User,  
    CreditCard, 
    Phone,  
    MapPin,  
    FilePenLine ,
    Banknote,
    CircleDollarSign,
    Building2, 
    BriefcaseBusiness , 
    Calendar,
    Image as ImageIcon, 
    CheckCircle,  
    FileText, 
    Hash,  
    Activity ,  
    NotepadText  
} from 'lucide-react';
import './CompleteBooking.css';
import { RiSave3Fill } from "react-icons/ri";
import { AiOutlineClear } from "react-icons/ai";
import { FiPrinter } from "react-icons/fi";
import { 
    calculateNewDownPayment, 
    caluclateDownPayment,  
    resetBookingForm,  
    setBookingClientData, 
    setInstallmentData, 
    setReservationStatus, 
    updateDownPaymentManual
} from '../../../assets/redux/bookingSlice';
import { variables } from '../../../assets/variables';
import {toast} from 'react-toastify'
import { useNavigate } from 'react-router-dom';
import { LuPrinter } from "react-icons/lu";
import { 
    bookingDetailRequest,
    fetchReservedClientById, 
    fillClientData, 
    generateInstallments, 
    saveChecksImages, 
    saveNationalIdImage 
} from '../../../services/bookingService';

const CompleteBooking = () => {
    const dispatch = useDispatch();
    const focusRef = useRef();
    const downPaymentRef=useRef();
    const navigate=useNavigate();
    const reservationRef=useRef();
    const {bookingClient,
        InstallmentInformation,
        initialClientData,
        reserved,
        nationalIdImage,
        checkImage,
        BookingDate,
    }=useSelector((state)=>state.booking);
  
const handleChange=(e)=>{
    const {name,value}=e.target;
    dispatch(setBookingClientData({[name]:value}));
}
const resetForm=()=>{
    dispatch(resetBookingForm());
    focusRef.current.focus();
}
const handleChangeinstallment=(e)=>{
    const {name,value}=e.target;
    const totalamount=initialClientData.NegotiationPrice;
    dispatch(setInstallmentData({[name]:value}));

}
 const handleFileChange =async (e) => {
       const { name } = e.target;
        if (!e.target.files || e.target.files.length === 0) return; 
        if(e.target.name==='NationalIdImagePath'){
            const file = e.target.files[0];
            const formData = new FormData();
            const fileName = file.name;
            formData.append("formFile", file, fileName);    
           await  dispatch(saveNationalIdImage(formData));
           await  dispatch(setBookingClientData({[name]:fileName}));         
        }
        if(e.target.name==='CheckImagePath'){
                const file2 = e.target.files[0];
                const formData_ = new FormData();
                const fileName_ = file2.name;
                formData_.append("file_c", file2, fileName_);    
              await dispatch(saveChecksImages(formData_));
              await dispatch(setInstallmentData({[name]:fileName_})); 
        }
    
};
const SavedData=async()=>{
   const parms={
            ClientExtraDetails:{...initialClientData,...bookingClient},
            UnitBooking:{...initialClientData,...InstallmentInformation,BookingDate}
            ,installments:[]
    }; 
   if (!bookingClient) {
        toast.error("بيانات العميل غير مكتملة!",{
            theme:'colored'
        });
        return;
    }   
   try {
        const result=await dispatch(bookingDetailRequest(parms)).unwrap();
         if(result.savedBooking){
         toast.success("تم الحجز بنجاح!", {
           theme: "colored",
           position: "top-left",
        });
         }
    else if(result.updatedBooking){
        toast.success("تم تحديث البيانات بنجاح!", {
        theme: "colored",
        position: "top-left",
        });
        }
    } catch (error) {
         toast.error("حدث خطأ في الاتصال بالخادم!", {
            theme: "colored",
            position: "top-left",
        });
    }    
}

const calcutlateDownpayment=()=>{
    const totalamount=initialClientData.NegotiationPrice;
    if(InstallmentInformation.BookingID===0 ){
       dispatch(caluclateDownPayment(totalamount));
    }
    else{
        const newtotalPrice=initialClientData.NegotiationPrice;
        dispatch(calculateNewDownPayment({
            total:newtotalPrice,
            newReservationAmount:reservationRef.current.value
        }))
    }
}
const createInstallments=()=>{
    dispatch(setReservationStatus(0))
    if(InstallmentInformation.ReservationAmount !=""){
         dispatch(generateInstallments(InstallmentInformation))
         navigate('/installments_schedule');
    }
    else{
        toast.error(" أكمل إدخال البيانات لإنشاء جدول الأقساط!", {
            theme: "colored",
            position: "top-left",
        });
    }
}
const getinstallmentsData=(id)=>{
    if(reserved===1){
         dispatch(fetchReservedClientById(id));
         navigate('/installments_schedule');
    }
}
  useEffect(() => {
  if (focusRef.current) focusRef.current.focus();
   const savedData = localStorage.getItem('activeBookingClient');
     if (savedData) {
        const parsedData = JSON.parse(savedData);
        dispatch(fillClientData(parsedData));
    }
}, [dispatch]);

    return (
        <div className="final_page_wrapper">
            <div className="final_booking_container">
                
                <div className="final_header_area">
                    <h2 className="final_main_title">استكمال بيانات الحجز والأقساط</h2>
                </div>
              

                <div className="final_content_box animate__animated animate__fadeIn">
                     
                    <form className="final_form_body" >   
                         <div className="final_floating_actions row">
                         <div 
                        className="final_circle_btn"
                        title="تنظيف"> <AiOutlineClear size={28} color="#14213d" onClick={()=>resetForm()} /></div>       
                        <div className="final_circle_btn" title="طباعة"><LuPrinter  size={24} color="#1086b9" onClick={()=>window.print()} /></div>
                        <div className="final_circle_btn" title="حفظ"><RiSave3Fill size={24} color="#10b981" onClick={()=>SavedData()} /></div>
                        {reserved===1 && 
                        <div className="final_circle_btn" title="جدول الاقساط"><NotepadText  size={24} color="#42025e" onClick={()=>getinstallmentsData(InstallmentInformation?.BookingID)}/></div>
                        }
                        </div>                   
                          
                                <div className="row mb-4">
                                    <div className="col-md-4">
                                        <div className="final_field_group">
                                             <input type="text" value={initialClientData.ClientID } hidden className="final_input_modern final_disabled" />
                                            <label className="final_label"><User size={18} /> إسم العميل</label>
                                            <input type="text" value={initialClientData.ClientName } readOnly className="final_input_modern final_disabled" />
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="final_field_group">
                                            <label className="final_label"><Building2 size={18} /> المشروع</label>
                                            <input type="text" value={initialClientData.ProjectName} readOnly className="final_input_modern final_disabled" />
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="final_field_group">
                                            <label className="final_label"><Activity size={18} /> الوحدة</label>
                                            <input type="text" value={initialClientData.unitName} readOnly className="final_input_modern final_disabled" />
                                        </div>
                                    </div>
                                </div>
                           
                        <hr className="final_divider" />
                        <div className="row mt-4">
                            <div className="col-lg-8">
                                <div style={{display:'flex'}}>
                
                                <div className="final_field_group mt-3">
                                    <label className="final_label"><Hash size={18} />كود الحجز</label>
                                    <div className="final_upload_btn">
                                        <input 
                                        type="text" 
                                        className="final_input_modern final_disabled" 
                                        name='BookingID'
                                        readOnly
                                        value={InstallmentInformation.BookingID || 0}
                                        onChange={handleChangeinstallment}
                                        />
                                    </div>
                                </div>

                                 <div className="final_field_group mt-3 m-4">
                                    <label className="final_label"><Calendar size={18} />تاريخ الحجز </label>
                                    <div className="final_upload_btn">
                                        <input 
                                        type="text" 
                                        style={{marginRight:'-20px'}}
                                        className="final_input_modern final_disabled" 
                                        name='BookingDate'
                                        readOnly
                                        value={BookingDate .split('T')[0]}
                                        />
                                    </div>
                                </div>
                            </div>
                                <div className="final_field_group">
                                    <label className="final_label"><CreditCard size={18} /> رقم البطاقة</label>
                                    <input
                                        type="text"
                                        name="NationalID"
                                        className="final_input_modern"
                                        ref={focusRef}
                                        value={bookingClient.NationalID ||  ""}  
                                        onChange={handleChange}
                                    />
                                </div>

                                <div className="final_field_group mt-3">
                                    <label className="final_label"><ImageIcon size={18} /> صورة البطاقة</label>
                                    <div className="final_upload_btn">
                                        <input type="file" name='NationalIdImagePath' onChange={handleFileChange} />
                                        <div className="final_upload_label">
                                            <span>إضغط لرفع صورة البطاقة الشخصية</span>
                                            <ImageIcon size={18} />
                                        </div>
                                    </div>
                                </div>
                                <div className="final_field_group mt-3">
                                    <label className="final_label"><Phone size={18} /> تليفون إضافي</label>
                                    <input 
                                    type="text" 
                                    name="SecondaryPhone" 
                                    className="final_input_modern"
                                    value={bookingClient.SecondaryPhone ||   "" }
                                    onChange={handleChange}
                                    />
                                </div>
                                <div className="final_field_group mt-3">
                                    <label className="final_label"><MapPin size={18} /> العنوان </label>
                                    <input 
                                    type="text" 
                                    name="Address" 
                                    className="final_input_modern"
                                    value={bookingClient.Address ||  ""}
                                    onChange={handleChange}
                                    />
                                </div>
                                 <div className="final_field_group mt-3">
                                    <label className="final_label"><BriefcaseBusiness  size={18} />الوظيفة</label>
                                    <input 
                                    type="text" 
                                    name="Job" 
                                    className="final_input_modern"
                                    value={bookingClient.Job || ""}
                                    onChange={handleChange}
                                    />
                                </div>
                            </div>
                            <div className="col-lg-4">
                           <div className="final_image_preview_big">
                          {(() => {
                          const imgName = nationalIdImage || bookingClient?.NationalIdImagePath;  
                            if (imgName && imgName !== "null") {
                            return (
                                <img 
                                src={`${variables.NATIONAL_ID_IMAGES_URL}/${imgName}`} 
                                className="final_img_fluid" 
                                alt="" 
                                />
                            );
                            } else {
                            return (
                                <div className="final_empty_msg">
                                <ImageIcon size={40} className="final_icon_fade" />
                                <p>معاينة البطاقة</p>
                                </div>
                            );
                            }
                        })()}
                        </div>
                        </div>
                        </div>

                        <hr className="final_divider" />

                        <div className="row mt-4">
                            <div className="col-lg-8">
                                 <div className="final_field_group mt-3">
                                    <label className="final_label"><CircleDollarSign  size={18} />مبلغ الحجز</label>
                                    <input
                                        type="text"
                                        name="ReservationAmount"
                                        className="final_input_modern"
                                        ref={reservationRef}
                                        value={InstallmentInformation.ReservationAmount || ""} 
                                        onBlur={()=>calcutlateDownpayment()}
                                        onChange={handleChangeinstallment}
                                        
                                    />
                                </div>
                                <div className="final_field_group mt-3">
                                    <label className="final_label"><CircleDollarSign  size={18} />المقدم (25%)</label>
                                    <input
                                        type="text"
                                        name="DownPayment"
                                        className="final_input_modern"         
                                        ref={downPaymentRef}
                                        value={InstallmentInformation.DownPayment || ""}
                                        onChange={handleChangeinstallment}
                                    />
                                </div>

                                <div className="final_field_group mt-3">
                                    <label className="final_label"><Calendar size={18} /> تاريخ أول قسط</label>
                                    <input
                                        type="date"
                                        name="FirstInstallmentDate"
                                        className="final_input_modern"
                                        value={InstallmentInformation.FirstInstallmentDate?.split('T')[0] || ""}
                                        onChange={handleChangeinstallment}
                                    />
                                </div>
                                <div className="final_field_group">
                                    <label 
                                    className="final_label"><Banknote  size={18} /> طريقة الدفع</label>
                                    <select 
                                    name="PaymentMethod" 
                                    className="final_select_modern"
                                    value={InstallmentInformation.PaymentMethod || ""}
                                    onChange={handleChangeinstallment}
                                    >
                                        <option value="-1">-إختر-</option>
                                        <option value="كاش">كاش (نقدي)</option>
                                        <option value="شيكات بنكية">شيكات بنكية</option>
                                    </select>
                                </div>
                                 <div className="final_field_group mt-3 animate__animated animate__fadeIn">
                                    <label className="final_label"><FileText size={18} /> إرفاق صورة الشيك</label>
                                    <div className="final_upload_btn">
                                        <input type="file" name='CheckImagePath' onChange={handleFileChange} />
                                        <div className="final_upload_label">
                                            <span>رفع صورة الشيك</span>
                                            <FileText size={18} />
                                        </div>
                                    </div>
                                </div>
                                <div className="final_field_group mt-3">
                                    <label className="final_label"><Calendar size={18} /> مدة التقسيط (بالسنوات)</label>
                                    
                                    <div className="d-flex gap-2 flex-grow-1">
                                        <select
                                            name="InstallmentYears"
                                            className="final_select_modern"
                                            value={InstallmentInformation.InstallmentYears||""}
                                            onChange={handleChangeinstallment}
                                        >
                                            <option value="-1">-إختر السنين-</option>
                                            <option value="1">1 سنة</option>
                                            <option value="3">3 سنوات</option>
                                            <option value="5">5 سنوات</option>
                                            <option value="7">7 سنوات</option>
                                        </select>
                                        {InstallmentInformation.InstallmentYears !== "-1"&&
                                        (<button 
                                        type="button" 
                                        className="mini_btn primary"
                                        onClick={()=>createInstallments()}
                                        >
                                        <CheckCircle size={16} /> إنشاء الأقساط
                                        </button>
                                        )}
                                    </div>
                                </div>
                               
                            </div>
                            <div className="col-lg-4">
                                <div className="final_image_preview_big" style={{ height: '220px' }}>
                                {(() => {
                                    const imgName = checkImage || InstallmentInformation.CheckImagePath;

                                    if (imgName && imgName !== "null") {
                                    // الحالة الأولى: لو فيه صورة
                                    return (
                                        <img 
                                        src={`${variables.CHECKS_IMAGES_URL}/${imgName}`} 
                                        className="final_img_fluid" 
                                        alt="" 
                                        />
                                    );
                                    } else {
                                    // الحالة الثانية: لو مفيش صورة (الرسالة البديلة)
                                    return (
                                    <div className="final_empty_msg" >
                                        <FileText size={40} className="final_icon_fade" />
                                        <p>معاينة الشيك</p>
                                     </div>
                                    );
                                    }
                                })()}                               
                                </div>
                            </div>
                        </div>

                       
                    </form>
                </div>

              
            </div>
        </div>
    );
};

export default CompleteBooking;