import React, { useEffect, useRef } from "react";
import "./AddClients.css";
import { 
  MdCleaningServices, 
  MdAdd ,
  MdDeleteOutline,
  MdLastPage,
  MdFirstPage,
  MdNavigateNext,
  MdChevronLeft
} from "react-icons/md";
import { RiSave3Fill, RiDeleteBinLine } from "react-icons/ri";
import { FaSearch } from "react-icons/fa";
import { CiEdit } from "react-icons/ci";
import { motion } from "framer-motion";
import { IoMdCheckmarkCircleOutline } from "react-icons/io";
import { AiOutlineUserDelete,AiOutlineUserAdd,AiOutlineClear } from "react-icons/ai";
import { LuUserRoundSearch } from "react-icons/lu"
import {
  User,
  Hash,
  Phone,
  ReceiptText,
  NotebookPen,
  Hourglass,
 Calendar ,
 ArrowRight ,
 ArrowLeft ,
 ArrowBigLeftDash ,
 ArrowBigRightDash 
} from "lucide-react";
import { useDispatch, useSelector } from "react-redux";
import {
  editingNegotiationRow,
  prepareNewNegotiation,
  resetClientForm,
  selectedDeleteNegotiationRow,
  setClientData,
  toggleDeleteClientModal,
  toggleSearchClientsModal,
} from "../../../assets/redux/clientSlice";
import {toast}from 'react-toastify'
import SearchClientsModal from "../modals/SearchClientsModal";
import { IoClose } from "react-icons/io5";
import DeleteNegotiationModal from "./DeleteNegotiationModal";
import NegotiationModal from "./NegotiationModal"; 
import { 
  fetchFirstClient, 
  fetchLastClient, 
  fetchNextClient, 
  fetchPreviousClient, 
  fetchProjects, 
  saveClient 
} from "../../../services/clientService";
import ClientsDeleteModal from "../modals/ClientsDeleteModal";

const AddClients = () => {
  const {
    client,
    negotiationsList,
    isNegotiationModalOpen,
    isDeleteClientModalOpen,
    isDeleteNegotiationModalOpen,
    isSearchClientsModalOpen,
    units,
    projects
  }= useSelector((state) => state.clients);
  const {userDetails}=useSelector((state)=>state.auth);
  const {isLoading}=useSelector((state)=>state.ui);
  const dispatch = useDispatch();
  const nameRef = useRef();
  const codeRef = useRef();
    
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    dispatch(setClientData({[name]:value}));
  };

 const handleResetForm = () => {
    dispatch(resetClientForm());
    nameRef.current.focus();
    codeRef.current.disabled = false;
 };

 const prepareForm = () => {
    handleResetForm();
    codeRef.current.disabled = true;
};
 
  const handleAddNegotiation=()=>{
    dispatch(editingNegotiationRow(-1));
    dispatch(prepareNewNegotiation(negotiationsList.length +1));
    dispatch(fetchProjects());
  }

const handleEditNegotiation=(index)=>{
dispatch(editingNegotiationRow(index));
}
const handleSaveClient=async()=>{  
const parms={...client,negotiations:negotiationsList.map(item=>({ ...item,Requester:userDetails.UserName}))}
       try {
        const result = await dispatch(saveClient(parms)).unwrap();
         if(result.saved){
          toast.success("تم حفظ البيانات بنجاح ", {
            theme: "colored",
            position: "top-left",
            }); 
        }   
         if(result.updated){
          toast.success("تم تعديل البيانات بنجاح ", {
            theme: "colored",
            position: "top-left",
            }); 
        }   
       } catch (error) {
         toast.error("حدث خطأ في الاتصال بالخادم", {
           theme: "colored",
           position: "top-center",
         });
       }     
}

const handlePreviousClient=()=>{
  try {
    dispatch(fetchPreviousClient(client.ClientID))
  } 
  catch (error) {
    toast.error("حدث خطأ , يرجي إعادة المحاولة", {
           theme: "colored",
           position: "top-center",
    });
  }
}

const handleNextClient=()=>{
  try {

    dispatch(fetchNextClient(client.ClientID))
  } catch (error) {
    toast.error("حدث خطأ , يرجي إعادة المحاولة", {
           theme: "colored",
           position: "top-center",
    });
  }
}

useEffect(()=>{
  if(nameRef.current.focus())nameRef.current.focus();
},[])

  return (
    <div
      dir="rtl"
      className="page-container"
    >   
      {isNegotiationModalOpen && <NegotiationModal/>} 
      {isDeleteClientModalOpen && <ClientsDeleteModal/>}
      {isSearchClientsModalOpen && <SearchClientsModal/>}
      {isDeleteNegotiationModalOpen && <DeleteNegotiationModal/>}
  
      <div className="crm_header">
        <h2 className="crm_title">إضافة عملاء ومهتمين جدد</h2>
      </div>
    <div className="btns_toc">
    <button className="icon-btn" disabled={isLoading} onClick={handleResetForm}>
        <span className="btn_c" title="تنظيف">
            <AiOutlineClear size={24} color="#14213d"/>
        </span>
    </button>

    <button className="icon-btn" disabled={isLoading} onClick={prepareForm}>
        <span className="btn_c" title="إضافة جديد">
            <AiOutlineUserAdd size={24} color="#1e40af"/>
        </span>
    </button>

    <button className="icon-btn" disabled={isLoading} onClick={handleSaveClient}>
        <span className="btn_c" title="حفظ">
            <RiSave3Fill size={24} color="#10b981"/>
        </span>
    </button>

    <button className="icon-btn" disabled={isLoading} onClick={() => dispatch(toggleDeleteClientModal(true))}>
        <span className="btn_c" title="حذف">
            <AiOutlineUserDelete size={24} color="#ef4444"/>
        </span>
    </button>

    <button className="icon-btn" disabled={isLoading} onClick={() => dispatch(toggleSearchClientsModal(true))}>
        <span className="btn_c" title="بحث">
            <LuUserRoundSearch size={24} color="#3b82f6"/>
        </span>
    </button>
</div>
    < motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} div className="main_crm">
       <div className="data_user">
      <label className="lbl_crm"><User size={18} />المسئول</label>
      <input type="text" className="userName_inp" disabled value={userDetails.UserName} />
    </div>
  <div className="crm_cnt">
    <div className="data_crm">
      <label className="lbl_crm"><Hash size={18} /> كود العميل</label>
      <input type="text" className="crm_inp" name="ClientID" value={client.ClientID || 0}  ref={codeRef} />
    </div>
     <div className="data_crm">
      <label className="lbl_crm"><User size={18} /> إسم العميل</label>
      <input type="text" className="crm_inp" name="ClientName" value={client.ClientName || ""} onChange={handleInputChange} autoComplete="off" ref={nameRef} />
    </div>

    <div className="data_crm">
      <label className="lbl_crm"><Phone size={18} /> رقم الموبايل</label>
      <input type="text" className="crm_inp" name="PhoneNumber" value={client.PhoneNumber || ""} onChange={handleInputChange} />
    </div>

    <div className="data_crm">
      <label className="lbl_crm"><ReceiptText size={18} /> الحالة</label>
      <select className="crm_select" name="ClientStatus" value={client.ClientStatus || ""} onChange={handleInputChange}>
        <option value="-1">-إختر-</option>
        <option value="عميل جديد">عميل جديد</option>
        <option value="استفسار">استفسار</option>
        <option value="مهتم">مهتم</option>
        <option value="متابعة">متابعة</option>
      </select>
    </div>
    <div className="data_crm">
      <label className="lbl_crm"><NotebookPen size={18} /> ملاحظات</label>
      <textarea className="crm_inp crm_notes" name="Notes" value={client.Notes || ""} onChange={handleInputChange}></textarea>
    </div>
  </div>
</motion.div>


<div className="btns_bottomcrm">
  <div className="btns_actions_left">
    <button 
    className="btn_nego" 
    disabled={isLoading}
    onClick={() => handleAddNegotiation()}>
    طلب تفاوض / شراء
    </button>
  </div>

  <div className="btns_arrows">
    <button 
          className="icon-btn"
          disabled={isLoading}
          onClick={()=>dispatch(fetchFirstClient())}
     >
     <span className="btn_c" title="السجل الأول" >
      <MdLastPage size={22}/>
      </span>
   </button>

     <button 
          className="icon-btn"
          disabled={isLoading}
          onClick={()=> handleNextClient()}
      >
     <span className="btn_c" title="التالي" >
      < MdChevronLeft size={22}/>
      </span>
   </button>

    <button 
          className="icon-btn"
          disabled={isLoading}
          onClick={()=>handlePreviousClient()}
      >
      <span className="btn_c" title="السابق" >
        <MdNavigateNext  size={22} />
        </span>
   </button>
    <button 
          className="icon-btn"
          disabled={isLoading}
          onClick={()=>dispatch(fetchLastClient())}
      >
     <span className="btn_c" title="السجل الأخير" >
      <MdFirstPage size={22} />
      </span>
   </button>  
  </div>
</div>
      
      {negotiationsList.length>0 && 
      <div className="tbl_negotiation">
        <table className="table table-striped tbl_n">
            <thead>
              <tr>
                <th>كود الطلب</th>
                <th>المشروع</th>
                <th>الوحدة</th>
                <th>السعر الأصلي للوحدة</th>
                <th>سعر التفاوض</th>
                <th>قيمة الخصم</th>
               {negotiationsList.length > 0 ? 
               ( <>  
               <th>حالة الطلب</th>
               <th>تاريخ الطلب</th>
                </>) 
               :""}
                <th className="events_col">الأحداث</th>
              </tr>
            </thead>
            <tbody>
              {negotiationsList.length>0 &&
               negotiationsList.map((neg,index)=>
              <tr key={index}>
                <td>{neg.serialCode}</td>
                <td>{neg.ProjectName}</td>
                <td>{neg.unitName}</td>
                <td>{neg.OriginalPrice} ج</td>
                <td>{neg.NegotiationPrice} ج</td>
                <td>{neg.DiscountAmount} %</td>
              {negotiationsList.length > 0 ? (
          <>
            <td style={{ fontWeight: 'bold' }}>
           <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            {neg.NegotiationStatus === 'مقبول' && (
           <span style={{ color: '#2ecc71', display: 'flex', alignItems: 'center' }}>
              {neg.NegotiationStatus} <IoMdCheckmarkCircleOutline size={20} style={{ marginRight: '5px' }} />
           </span>
          )}
         {neg.NegotiationStatus === 'مرفوض' && (
          <span style={{ color: '#e74c3c', display: 'flex', alignItems: 'center'}}>
            {neg.NegotiationStatus} <IoClose size={20} style={{ marginRight: '5px' }} />
          </span>
        )}
        {(neg.NegotiationStatus !== 'مقبول' && neg.NegotiationStatus !== 'مرفوض') && (
          <span style={{ display: 'flex', alignItems: 'center' }}>
            {neg.NegotiationStatus} <Hourglass size={18} className="hourglass_" style={{ marginRight: '5px' }} />
              </span>
            )}
          </div>
        </td>
          <td>{neg.NegotiationDate ? neg.NegotiationDate.split('T')[0] : ""}</td>
        </>
      ) : ""}
                <td>
                  <div className="btns_dtls_n">
                <span  title="تعديل" onClick={()=>handleEditNegotiation(index)}>
                  <CiEdit size={25} color="blue"/>
                </span>
                
                <span  title="تعديل"
                  onClick={()=>dispatch(selectedDeleteNegotiationRow(index))}>
                  <MdDeleteOutline size={25} color="red"/>
                  </span>
                  </div>
                </td>
              </tr>
              )}
            </tbody>
        </table>
      </div>
      }
     
    </div>
  );
};

export default AddClients;
