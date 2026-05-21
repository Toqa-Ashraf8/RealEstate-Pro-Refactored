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
import { RiSave3Fill } from "react-icons/ri";
import { AiOutlineClear } from "react-icons/ai";
import { FiPrinter } from "react-icons/fi";
import {  
    setReservationStatus, 
} from '../../../assets/redux/bookingSlice';
import {toast} from 'react-toastify'
import { useNavigate } from 'react-router-dom';
import { LuPrinter } from "react-icons/lu";
import { 
    bookingDetailRequest,
    saveChecksImages, 
    saveNationalIdImage 
} from '../../../services/bookingService';
import './ManageBookingPage.css'
import { 
    calculateNewDownPayment,
  hydrateFromStorage, 
  setClientB, 
  setInstallmentB 
} from '../../../assets/redux/manageBookingSlice';
import { variables } from '../../../assets/variables';

const ManageBookingPage = () => {
   const dispatch = useDispatch();
    const focusRef = useRef();
    const downPaymentRef=useRef();
    const navigate=useNavigate();
    const reservationRef=useRef();
    const {
      initialClientBookedData,
        clientBooked,
        installmentInfoBooked,
        installmentsBooked,
        reserved,
        nationalIdImage,
        checkImage,
        BookingDate,
    }=useSelector((state)=>state.manageBooking);
  
const handleChange = (e) => {
    const { name, value } = e.target;
    dispatch(setClientB({ [name]: value }));
    }
const handleChangeinstallment=(e)=>{
    const { name, value } = e.target;
    dispatch(setInstallmentB({ [name]: value }));
}
  const handleFileChange =async (e) => {
       const { name } = e.target;
        if (!e.target.files || e.target.files.length === 0) return; 
        if(e.target.name==='NationalIdImagePath'){
            const file = e.target.files[0];
            const formData = new FormData();
            const fileName = file.name;
            formData.append("file", file);    
           await  dispatch(saveNationalIdImage({
                data:formData,
                folder:"NationalIDCard_Images"
           }));
           await  dispatch(setClientB({[name]:fileName}));         
        }
        if(e.target.name==='CheckImagePath'){
                const file = e.target.files[0];
                const formData_ = new FormData();
                const fileName = file.name;
                formData_.append("file", file);    
               await dispatch(saveChecksImages({
                data:formData_,
                folder:"Checks_Images"
              }));
              await dispatch(setInstallmentB({[name]:fileName})); 
        }
    
}; 
const updateBookingData=async()=>{
   const parms={
            ClientExtraDetails:{...initialClientBookedData,...clientBooked},
            UnitBooking:{...initialClientBookedData,...installmentInfoBooked,BookingDate}
            ,installments:[]
    }; 
   if (!clientBooked) {
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
    } catch (error) {} 
}

const calcutlateDownpayment=()=>{
      const newtotalPrice=initialClientBookedData.NegotiationPrice;
      dispatch(calculateNewDownPayment({
            total:newtotalPrice,
            newReservationAmount:reservationRef.current.value
        }))
}

 const createInstallments=()=>{
    dispatch(setReservationStatus(0))
    if(installmentInfoBooked.ReservationAmount !=""){
         dispatch(generateInstallments(installmentInfoBooked))
         navigate('/manage-installments-details');

    } 
     else{
        toast.error(" أكمل إدخال البيانات لإنشاء جدول الأقساط!", {
            theme: "colored",
            position: "top-left",
        });
    }  
} 

const getinstallmentsData=()=>{
     navigate('/manage-installments-details');
}


/* console.log("installmentsBooked",installmentsBooked);
console.log("clientBooked",clientBooked);
console.log("installmentInfoBooked",installmentInfoBooked); */

    return (
        <div className="final_page_wrapper">
            <div className="final_booking_container">
                
                <div className="final_header_area">
                    <h2 className="final_main_title">استكمال بيانات الحجز والأقساط</h2>
                </div>
                        <div className="final_floating_actions row">
                            <div className="final_circle_btn" title="حفظ"><RiSave3Fill size={24} color="#10b981" onClick={()=>SavedData()} /></div>
                            <div className="final_circle_btn" title="جدول الاقساط"><NotepadText  size={24} color="#42025e" onClick={()=>getinstallmentsData()}/></div>
                            
                        </div> 
                    <div className="final_content_box animate__animated animate__fadeIn">
                   
                    <div className="final_form_body" >   
                                <div className="row mb-4">
                                    <div className="col-md-4">
                                        <div className="final_field_group">
                                             <input type="text" value={initialClientBookedData?.ClientID } hidden className="final_input_modern final_disabled" />
                                            <label className="final_label"><User size={18} /> إسم العميل</label>
                                            <input type="text" value={initialClientBookedData?.ClientName } readOnly className="final_input_modern final_disabled" />
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="final_field_group">
                                            <label className="final_label"><Building2 size={18} /> المشروع</label>
                                            <input type="text" value={initialClientBookedData?.ProjectName} readOnly className="final_input_modern final_disabled" />
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="final_field_group">
                                            <label className="final_label"><Activity size={18} /> الوحدة</label>
                                            <input type="text" value={initialClientBookedData?.unitName} readOnly className="final_input_modern final_disabled" />
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
                                        value={installmentInfoBooked.BookingID || 0}
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
                                        value={installmentInfoBooked.BookingDate?.split('T')[0] }
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
                                        value={clientBooked.NationalID ||  ""}  
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
                                    value={clientBooked.SecondaryPhone ||   "" }
                                    onChange={handleChange}
                                    />
                                </div>
                                <div className="final_field_group mt-3">
                                    <label className="final_label"><MapPin size={18} /> العنوان </label>
                                    <input 
                                    type="text" 
                                    name="Address" 
                                    className="final_input_modern"
                                    value={clientBooked.Address ||  ""}
                                    onChange={handleChange}
                                    />
                                </div>
                                 <div className="final_field_group mt-3">
                                    <label className="final_label"><BriefcaseBusiness  size={18} />الوظيفة</label>
                                    <input 
                                    type="text" 
                                    name="Job" 
                                    className="final_input_modern"
                                    value={clientBooked.Job || ""}
                                    onChange={handleChange}
                                    />
                                </div>
                            </div>
                            <div className="col-lg-4">
                           <div className="final_image_preview_big">
                          {(() => {
                          const imgName = nationalIdImage || clientBooked?.NationalIdImagePath;  
                            if (imgName && imgName !== "null") {
                            return (
                                <img 
                                src={variables.NATIONAL_ID_IMAGES_URL+imgName} 
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
                                        value={installmentInfoBooked.ReservationAmount || ""} 
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
                                        value={installmentInfoBooked.DownPayment || ""}
                                        onChange={handleChangeinstallment}
                                    />
                                </div>

                                <div className="final_field_group mt-3">
                                    <label className="final_label"><Calendar size={18} /> تاريخ أول قسط</label>
                                    <input
                                        type="date"
                                        name="FirstInstallmentDate"
                                        className="final_input_modern"
                                        value={installmentInfoBooked.FirstInstallmentDate?.split('T')[0] || ""}
                                        onChange={handleChangeinstallment}
                                    />
                                </div>
                                <div className="final_field_group">
                                    <label 
                                    className="final_label"><Banknote  size={18} /> طريقة الدفع</label>
                                    <select 
                                    name="PaymentMethod" 
                                    className="final_select_modern"
                                    value={installmentInfoBooked.PaymentMethod || ""}
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
                                            value={installmentInfoBooked.InstallmentYears||""}
                                            onChange={handleChangeinstallment}
                                        >
                                            <option value="-1">-إختر السنين-</option>
                                            <option value="1">1 سنة</option>
                                            <option value="3">3 سنوات</option>
                                            <option value="5">5 سنوات</option>
                                            <option value="7">7 سنوات</option>
                                        </select>
                                        {installmentInfoBooked.InstallmentYears !== "-1"&&
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
                                    const imgName = checkImage || installmentInfoBooked.CheckImagePath;

                                    if (imgName && imgName !== "null") {
                                    return (
                                        <img 
                                        src={variables.CHECKS_IMAGES_URL+imgName} 
                                        className="final_img_fluid" 
                                        alt="" 
                                        />
                                    );
                                    } else {
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
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ManageBookingPage
